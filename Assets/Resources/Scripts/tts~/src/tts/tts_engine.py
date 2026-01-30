"""Small abstraction that returns raw audio for the configured backend and language."""

from __future__ import annotations

import asyncio
import os
from dataclasses import dataclass
from io import BytesIO
from pathlib import Path
from tempfile import NamedTemporaryFile
from typing import Awaitable, Literal, Protocol, TypeVar

import edge_tts
import azure.cognitiveservices.speech as speechsdk
from gtts import gTTS

BackendName = Literal["gtts", "edge", "azure"]
T = TypeVar("T")


def _run_coroutine(coro: Awaitable[T]) -> T:
    loop = asyncio.new_event_loop()
    try:
        return loop.run_until_complete(coro)
    finally:
        loop.close()


class BackendError(RuntimeError):
    """Raised when the requested backend cannot be initialized or run."""


class LanguageNotSupportedError(ValueError):
    """Raised when the user requests a language that we do not know."""


class MissingAzureCredentialsError(RuntimeError):
    """Raised when Azure credentials are missing from env/config."""


class _Backend(Protocol):
    def synthesize(self, text: str, spec: "LanguageConfig") -> bytes:
        ...


@dataclass(frozen=True)
class LanguageConfig:
    gtts_lang: str | None
    edge_voice: str | None
    azure_locale: str | None
    azure_voice: str | None


LANGUAGE_CONFIGS: dict[str, LanguageConfig] = {
    "english": LanguageConfig(
        gtts_lang="en",
        edge_voice="en-US-JennyNeural",
        azure_locale="en-US",
        azure_voice="en-US-JennyNeural",
    ),
    "japanese": LanguageConfig(
        gtts_lang="ja",
        edge_voice="ja-JP-NanamiNeural",
        azure_locale="ja-JP",
        azure_voice="ja-JP-NanamiNeural",
    ),
    "korean": LanguageConfig(
        gtts_lang="ko",
        edge_voice="ko-KR-SunHiNeural",
        azure_locale="ko-KR",
        azure_voice="ko-KR-SunHiNeural",
    ),
}


def supported_languages() -> tuple[str, ...]:
    """Returns the language keys that the engine can synthesize."""

    return tuple(LANGUAGE_CONFIGS.keys())


def supported_backends() -> tuple[BackendName, ...]:
    """Returns the list of backend names we can instantiate."""

    return ("gtts", "edge", "azure")


class _GttsBackend:
    def synthesize(self, text: str, spec: LanguageConfig) -> bytes:
        lang = spec.gtts_lang
        if not lang:
            raise BackendError("gTTS is not configured for this language")
        buffer = BytesIO()
        gTTS(text=text, lang=lang, slow=False).write_to_fp(buffer)
        return buffer.getvalue()


class _EdgeBackend:
    def synthesize(self, text: str, spec: LanguageConfig) -> bytes:
        voice = spec.edge_voice
        if not voice:
            raise BackendError("Edge TTS is not configured for this language")
        with NamedTemporaryFile(delete=False, suffix=".mp3") as tmp:
            temp_path = Path(tmp.name)
        try:
            _run_coroutine(edge_tts.Communicate(text, voice).save(str(temp_path)))
            return temp_path.read_bytes()
        finally:
            temp_path.unlink(missing_ok=True)


class _AzureBackend:
    def __init__(self, subscription_key: str | None = None, region: str | None = None) -> None:
        key = subscription_key or os.environ.get("AZURE_SPEECH_KEY")
        region_value = region or os.environ.get("AZURE_SPEECH_REGION")
        if not key or not region_value:
            raise MissingAzureCredentialsError(
                "Azure Speech key and region are required to use the azure backend"
            )
        self._config = speechsdk.SpeechConfig(subscription=key, region=region_value)

    def synthesize(self, text: str, spec: LanguageConfig) -> bytes:
        locale = spec.azure_locale
        voice = spec.azure_voice
        if not locale or not voice:
            raise BackendError("Azure TTS is not configured for this language")
        self._config.speech_synthesis_language = locale
        self._config.speech_synthesis_voice_name = voice
        with NamedTemporaryFile(delete=False, suffix=".mp3") as tmp:
            destination = Path(tmp.name)
        try:
            audio_config = speechsdk.audio.AudioOutputConfig(filename=str(destination))
            synthesizer = speechsdk.SpeechSynthesizer(
                speech_config=self._config, audio_config=audio_config
            )
            result = synthesizer.speak_text_async(text).get()
            if result.reason != speechsdk.ResultReason.SynthesizingAudioCompleted:
                cancellation = getattr(result, "cancellation_details", None)
                extra: list[str] = []
                if cancellation is not None:
                    if cancellation.reason is not None:
                        extra.append(f"cancellation reason {cancellation.reason}")
                    if cancellation.error_details:
                        extra.append(cancellation.error_details)
                suffix = f" ({'; '.join(extra)})" if extra else ""
                raise BackendError(
                    f"azure backend failed: {result.reason}{suffix}"
                )
            return destination.read_bytes()
        finally:
            destination.unlink(missing_ok=True)


def _create_backend(name: BackendName, **kwargs: str) -> _Backend:
    if name == "gtts":
        return _GttsBackend()
    if name == "edge":
        return _EdgeBackend()
    if name == "azure":
        return _AzureBackend(subscription_key=kwargs.get("subscription_key"), region=kwargs.get("region"))
    raise BackendError(f"unsupported backend '{name}'")


class TtsEngine:
    """High level TTS client that produces raw audio bytes."""

    def __init__(self, backend: BackendName, language: str, **backend_kwargs: str) -> None:
        config = LANGUAGE_CONFIGS.get(language)
        if config is None:
            raise LanguageNotSupportedError(
                f"Language '{language}' is not supported ({', '.join(LANGUAGE_CONFIGS)})"
            )
        self._config = config
        self._backend = _create_backend(backend, **backend_kwargs)

    def synthesize(self, text: str) -> bytes:
        """Synthesize the provided text and return raw audio bytes."""

        if not text.strip():
            raise ValueError("text must contain printable characters")
        return self._backend.synthesize(text, self._config)


__all__ = ["TtsEngine", "supported_languages", "supported_backends", "BackendName"]

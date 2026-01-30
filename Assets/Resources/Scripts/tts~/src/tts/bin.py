#!/usr/bin/env python3
"""Command line entrypoint that writes raw TTS audio for a string."""

from __future__ import annotations

import argparse
import os
from pathlib import Path
from typing import Iterable

try:
    from tts.tts_engine import TtsEngine, supported_backends, supported_languages
except ImportError:  # pragma: no cover - best effort when running as script
    from .tts_engine import TtsEngine, supported_backends, supported_languages

SCRIPT_ROOT = Path(__file__).resolve().parents[2]
_ENV_PATH = SCRIPT_ROOT / ".env"


def _load_dotenv(path: Path) -> None:
    if not path.is_file():
        return
    for raw_line in path.read_text(encoding="utf-8").splitlines():
        line = raw_line.strip()
        if not line or line.startswith("#"):
            continue
        key, _, value = line.partition("=")
        key = key.strip()
        if not key:
            continue
        value = value.strip()
        if len(value) >= 2 and value[0] == value[-1] and value[0] in {"'", '"'}:
            value = value[1:-1]
        os.environ.setdefault(key, value)


_load_dotenv(_ENV_PATH)

LANGUAGE_ALIASES: dict[str, str] = {
    "en": "english",
    "english": "english",
    "ja": "japanese",
    "jp": "japanese",
    "japanese": "japanese",
    "ko": "korean",
    "kr": "korean",
    "korean": "korean",
}


def _language_choices() -> Iterable[str]:
    return sorted({*supported_languages(), *LANGUAGE_ALIASES.keys()})


def _normalize_language(value: str) -> str:
    normalized = LANGUAGE_ALIASES.get(value.lower())
    if not normalized:
        raise argparse.ArgumentTypeError(
            f"unsupported language {value!r}; choose from {', '.join(sorted(set(supported_languages())))}"
        )
    return normalized


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Generate mp3 byte payloads for a given string")
    parser.add_argument(
        "--language",
        type=_normalize_language,
        default="english",
        choices=_language_choices(),
        help="Language to synthesize (default: english)",
    )
    parser.add_argument(
        "--backend",
        choices=supported_backends(),
        default="gtts",
        help="Backend to power the synthesis",
    )
    parser.add_argument(
        "--azure-key",
        default=os.environ.get("AZURE_SPEECH_KEY"),
        help="Azure Speech subscription key (default from .env or environment)",
    )
    parser.add_argument(
        "--azure-region",
        default=os.environ.get("AZURE_SPEECH_REGION"),
        help="Azure Speech region (default from .env or environment)",
    )
    parser.add_argument("text", help="String to synthesize")
    parser.add_argument("output", type=Path, help="Destination file for raw mp3 data")
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    args.output.parent.mkdir(parents=True, exist_ok=True)
    backend_kwargs = {}
    if args.backend == "azure":
        backend_kwargs["subscription_key"] = args.azure_key
        backend_kwargs["region"] = args.azure_region
    engine = TtsEngine(backend=args.backend, language=args.language, **backend_kwargs)
    audio = engine.synthesize(args.text)
    args.output.write_bytes(audio)
    print(f"wrote {len(audio)} bytes to {args.output}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

#!/usr/bin/env python3
"""Dump TTS output for every row in a CSV map."""

from __future__ import annotations

import argparse
import csv
import os
import sys
from pathlib import Path
from typing import Iterable

SCRIPT_ROOT = Path(__file__).resolve().parents[2]
_SRC_PATH = SCRIPT_ROOT / "src"
if str(_SRC_PATH) not in sys.path:
    sys.path.insert(0, str(_SRC_PATH))
_ENV_PATH = SCRIPT_ROOT / ".env"

from tts.tts_engine import TtsEngine, supported_backends, supported_languages


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
    parser = argparse.ArgumentParser(description="Generate mp3 files from a locale CSV")
    parser.add_argument(
        "csv",
        type=Path,
        help="Source CSV with file name in the first column and text in the second column",
    )
    parser.add_argument("output", type=Path, help="Base directory to write results")
    parser.add_argument(
        "--language",
        type=_normalize_language,
        default="english",
        choices=_language_choices(),
        help="Language of the CSV entries (default: english)",
    )
    parser.add_argument(
        "--backend",
        choices=supported_backends(),
        default="gtts",
        help="Backend to spawn for synthesis",
    )
    parser.add_argument(
        "--azure-key",
        default=os.environ.get("AZURE_SPEECH_KEY"),
        help="Azure Speech subscription key (default from .env or env)",
    )
    parser.add_argument(
        "--azure-region",
        default=os.environ.get("AZURE_SPEECH_REGION"),
        help="Azure Speech region (default from .env or env)",
    )
    return parser.parse_args()


def _load_entries(csv_path: Path) -> list[tuple[str, str]]:
    if not csv_path.is_file():
        raise FileNotFoundError(csv_path)
    rows: list[tuple[str, str]] = []
    with csv_path.open(newline='') as fd:
        reader = csv.reader(fd)
        for raw in reader:
            if not raw:
                continue
            name = raw[0].strip()
            if not name:
                continue
            text = raw[1].strip() if len(raw) > 1 else ""
            if not text:
                continue
            rows.append((name, text))
    return rows


def main() -> int:
    args = parse_args()
    backend_kwargs = {}
    if args.backend == "azure":
        backend_kwargs["subscription_key"] = args.azure_key
        backend_kwargs["region"] = args.azure_region
    engine = TtsEngine(backend=args.backend, language=args.language, **backend_kwargs)
    entries = _load_entries(args.csv)
    target_dir = args.output / args.language
    target_dir.mkdir(parents=True, exist_ok=True)
    for name, text in entries:
        destination = target_dir / f"{name}.mp3"
        audio = engine.synthesize(text)
        destination.write_bytes(audio)
        print(f"{name} -> {destination} ({len(audio)} bytes)")
    print(f"wrote {len(entries)} files to {target_dir}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

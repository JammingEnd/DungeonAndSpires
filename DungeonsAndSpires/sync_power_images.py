#!/usr/bin/env python3
"""Fill in missing small power icons from the big ones.

For every image in images/powers/big/ that has no counterpart in images/powers/,
copy it down and resize it to 94x94 (the in-combat power icon size).

Usage (from anywhere):
    python3 sync_power_images.py            # create the missing icons
    python3 sync_power_images.py --dry-run  # just list what would be created
    python3 sync_power_images.py --force    # also re-create icons that already exist
    python3 sync_power_images.py --size 64  # different target size

Afterwards Godot needs to re-import the new files before the PCK export.
"""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

try:
    from PIL import Image
except ImportError:  # pragma: no cover
    sys.exit("Pillow is required: pip install pillow")

IMAGE_SUFFIXES = {".png", ".webp", ".jpg", ".jpeg"}


def find_powers_dir() -> Path:
    """Locate images/powers, accepting both the project root and its parent."""
    here = Path(__file__).resolve().parent
    candidates = [
        here / "DungeonsAndSpires" / "images" / "powers",
        here / "images" / "powers",
        Path.cwd() / "DungeonsAndSpires" / "images" / "powers",
        Path.cwd() / "images" / "powers",
    ]
    for candidate in candidates:
        if (candidate / "big").is_dir():
            return candidate
    sys.exit(
        "Could not find images/powers/big — expected it next to this script "
        "(DungeonsAndSpires/images/powers/big)."
    )


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--size", type=int, default=94, help="target size in pixels (default: 94)")
    parser.add_argument("--dry-run", action="store_true", help="only report what would be created")
    parser.add_argument("--force", action="store_true", help="overwrite icons that already exist")
    args = parser.parse_args()

    powers_dir = find_powers_dir()
    big_dir = powers_dir / "big"

    big_images = sorted(p for p in big_dir.iterdir() if p.suffix.lower() in IMAGE_SUFFIXES)
    small_images = {p.stem.lower() for p in powers_dir.iterdir() if p.suffix.lower() in IMAGE_SUFFIXES}

    created, skipped = [], []
    for big_path in big_images:
        target = powers_dir / f"{big_path.stem}.png"
        if target.exists() and not args.force:
            skipped.append(target.name)
            continue

        if args.dry_run:
            created.append(target.name)
            continue

        with Image.open(big_path) as image:
            resized = image.convert("RGBA").resize((args.size, args.size), Image.LANCZOS)
            resized.save(target)
        created.append(target.name)

    orphans = sorted(name for name in small_images if name not in {p.stem.lower() for p in big_images})

    print(f"powers dir: {powers_dir}")
    print(f"big images: {len(big_images)}   small images: {len(small_images)}")
    print(f"{'would create' if args.dry_run else 'created'}: {len(created)}")
    for name in created:
        print(f"  + {name} ({args.size}x{args.size})")

    if skipped and not args.force:
        print(f"already present: {len(skipped)}")
    if orphans:
        print(f"small images without a big source (untouched): {len(orphans)}")
        for name in orphans:
            print(f"  ? {name}")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())

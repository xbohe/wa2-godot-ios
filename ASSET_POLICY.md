# Asset Policy

This repository contains only the code and configuration needed to port WHITE
ALBUM2 to Godot on iOS. It does **not** contain, and does **not** license, any
WHITE ALBUM2 game assets.

## What is excluded

The following proprietary material is intentionally absent from this repository
and must be supplied separately by the user:

- Game archives and data packs (`*.pak`, `grp`, `se`, `IC`, `movie`, etc.);
- Audio, video, images, fonts, and text belonging to WHITE ALBUM2;
- Any other copyrighted or trademarked material owned by AQUAPLUS or other
  rightsholders.

The repository's `.gitignore` excludes these directories so they are never
committed. At runtime the app reads assets placed by the user under its own
data directory (for example `Wa2Res/`).

## Contributor rule

Do not commit game data, or any material derived from it (extracted, copied,
translated, transcribed, converted, or decompiled), unless you hold sufficient
rights to redistribute it. Material without sufficient redistribution
permission must not be committed or released.

This project is unofficial and is not affiliated with or endorsed by AQUAPLUS.

# Third-Party Notices

This repository is not licensed as a single work. Only files explicitly identified by a component license or file-level notice receive that license. The following components retain their own licenses.

## WA2 Godot porting code

- License: Apache License 2.0
- Scope: the exact paths listed in `PORTING_CODE.md`
- Full license: `LICENSES/Apache-2.0.txt`

These files are contributor-authored resource readers, runtime logic, shaders, Godot scenes, animation configuration, font mapping, and project integration files. They are intended to work with data supplied separately by a user. The Apache-2.0 grant covers only that original implementation and does not license referenced game data or other proprietary material.

## Texture Fonts editor addon

- Location: `addons/texture_fonts`
- License: MIT
- Copyright: (c) 2021-2024 Micky (Mickeon), Laila L. (ElectronicBlueberry)
- Full license: `addons/texture_fonts/LICENSE`

This Godot editor plugin is independent of the Apache-2.0 porting code and is distributed under its own MIT terms.

## Subtitle data

- Location: `assets/sub.yaml`
- License: mixed third-party content; not covered by Apache-2.0
- Detailed notice: `SUBTITLE_NOTICE.md`

Embedded comments attribute portions to 萌娘百科 (Moegirlpedia) and CK-GAL汉化组. Moegirlpedia states that its text is generally available under CC BY-NC-SA 3.0 CN unless otherwise noted, but the exact source pages and any underlying third-party lyrics or dialogue are not fully documented in this repository. No redistribution license has been identified for the CK-GAL portions. Redistributors must verify, replace, or remove this data as appropriate.

## Godot Engine and export-template components

Godot Engine is licensed under the MIT License. iOS export templates also contain third-party components whose notices are distributed by the Godot project. See the Godot Engine license and copyright documentation for the template version used to create a release.

## Excluded proprietary material

WHITE ALBUM2 game files, media, fonts, trademarks, and other original assets are not licensed by this repository. See `ASSET_POLICY.md`.

Files extracted, copied, translated, transcribed, converted, decompiled, or otherwise derived from proprietary material are likewise not covered by the Apache-2.0 License. No license is granted for other unmarked repository files; see the root `LICENSE` for the controlling scope notice.

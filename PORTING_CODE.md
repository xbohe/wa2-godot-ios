# Apache-2.0 Porting Code Scope

This iOS port is derived from WA2 Godot
(https://github.com/dorakyuraduang/wa2-godot). The following paths contain
contributor-authored WA2 Godot porting code and are licensed under the Apache
License, Version 2.0 in `LICENSES/Apache-2.0.txt`:

```text
/script/**/*.cs
/scene/**/*.tscn
/shader/**/*.gdshader
/assets/ani/**/*.tres
/assets/font.map
/main.tscn
/project.godot
/wa2.csproj
/default_bus_layout.tres
/export_presets.cfg
```

Anyone may fork, use, modify, port, and redistribute these covered files in a
different repository or product. No pull request, separate approval, or
contribution of modifications back to this repository is required. A
redistributor must still comply with Apache-2.0, including providing the
license, marking modified files, and retaining applicable attribution and
NOTICE information.

The scripts implement resource loading, runtime behavior, and Godot integration.
The shaders and animation resources are contributor-authored implementations
created by reference to visible game behavior and effects, without copying
original source code. The scenes contain contributor-authored Godot node
structures and configuration. `assets/font.map` is contributor-authored project
mapping data.

This license covers only the original expression in the listed files. It does
not cover any external file or content referenced or loaded by them. In
particular, it does not grant rights to WHITE ALBUM2 software, scripts,
dialogue, images, audio, video, fonts, data archives, trademarks, or other
proprietary material.

## Modifications in this iOS fork

The base porting code above is carried from WA2 Godot (dorakyuraduang/wa2-godot).
This iOS fork (`xbohe/wa2-godot-ios`, initial iOS port by Gdadfk in
`Gdadfk/wa2-godot-ios`, commit `5c393ba`) applies the following iOS-specific
**additions / modifications** to those Apache-2.0 files. These are made under the
same Apache License, Version 2.0; redistributors must comply with Apache-2.0
Section 4(b) and 4(d) (mark modified files, retain NOTICE, provide the license
text).

iOS-specific added or modified paths:

- `script/Wa2EngineMain.cs` — added the `user://Wa2Res/` resource directory, the
  `RequiredPakPaths` (13 paks) / `ExpectedMoviePaths` (16 ogv) startup validation
  and `ValidateIosMovies()`, and the `VideoStreamTheora` `PlayMovie` branch that
  plays `movie/mvXXX0.ogv` (the upstream `gde_gozen` / `wmv_video` video plugins
  have no iOS build, so this fork keeps the built-in Theora decoder).
- `script/TitleMenu.cs` — added the iOS-only `SetResourcesReady(bool)` entry point.
- `export_presets.cfg` — added `[preset.1] name="iOS"` and the Files-app /
  iTunes file-sharing switches (`accessible_from_files_app`,
  `accessible_from_itunes_sharing`).
- `project.godot` — iOS-related tweaks (`config/name`, `config/features`).
- `wa2.csproj` — added the `ios` .NET workload / Godot.NET.Sdk targeting.

All other listed `script/**`, `scene/**`, `shader/**`, `assets/ani/**`, and
`assets/font.map` files are carried from upstream with no iOS-specific change
unless noted above.

## Not adopted from upstream

The following upstream mechanisms were deliberately **not adopted** on iOS:

- `addons/gde_gozen` (upstream 0.2.3+) and `addons/wmv_video` (upstream v0.3b) —
  both are native GDExtensions without an iOS binary, so this fork retains the
  built-in Theora/`ogv` video path instead.
- `addons/texture_fonts` is retained in the tree but is **not used at runtime**;
  it is only an editor helper for regenerating Chinese font atlases.
- Upstream `v0.3b` non-video changes were not merged; this fork stays on the
  `0.2.9` baseline.

## Out of Apache-2.0 scope

The following are not covered by this Apache-2.0 scope unless a file is later
added here or receives an explicit file-level license:

- `assets/sub.yaml` (third-party subtitle data; see `SUBTITLE_NOTICE.md`);
- all other `assets/**` paths not explicitly listed above;
- `addons/texture_fonts/**` (independent MIT License; see
  `addons/texture_fonts/LICENSE`).

See the root `LICENSE`, `NOTICE`, `THIRD_PARTY_NOTICES.md`, `SUBTITLE_NOTICE.md`,
and `ASSET_POLICY.md` for the complete repository boundaries.

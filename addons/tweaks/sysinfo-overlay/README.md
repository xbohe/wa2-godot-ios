# sysinfo-overlay —— LiveContainer 通用诊断悬浮窗

在 LiveContainer 运行的任意 App（含本仓库的 White Album 2 iOS 版）上叠加一个可拖动的小窗，
实时显示 **FPS / 本进程 CPU% / 内存占用**，便于后续修 bug 时的性能与资源观测。

## 特性
- 纯 Objective-C + UIKit，不写 Logos、不链接 CydiaSubstrate（避开 TweakLoader 加载失败坑）。
- 只读取指标 + 画 UIWindow，不 hook 任何函数，最稳。
- 启动后 1 秒在左上角出现，可手指拖动；绿色等宽字体。

## 指标说明
| 指标 | 来源 | 备注 |
|------|------|------|
| FPS | `CADisplayLink` 每帧计数 | 近似 App 实际渲染帧率 |
| CPU | 遍历 `task_threads` 的 `cpu_usage` 求和 / 1000 | 本进程占用 |
| MEM | `task_vm_info.phys_footprint` | 与系统“设置”看到的内存一致 |

> 暂不含系统级 CPU（头文件在不同 SDK 间不稳定），如需补充见 `sysinfo_overlay.m` 顶部注释。

## 编译（CI 自动，无需本地 Mac）
由仓库根 `.github/workflows/build-tweaks.yml` 在 GitHub macOS runner 上编译所有
`addons/tweaks/*/` 下的 `*.m` 为 arm64 `.dylib` 并作为 artifact `wa2-tweaks` 上传。
下载后导入 LiveContainer 即可，LiveContainer 会在启动前自动重签。

## 本地手编（有 Mac 时）
```bash
SDK=$(xcrun --sdk iphoneos --show-sdk-path)
clang -dynamiclib -arch arm64 -target arm64-apple-ios15.0 \
  -isysroot "$SDK" -fobjc-arc \
  -framework Foundation -framework UIKit -framework QuartzCore -O2 \
  sysinfo_overlay.m -o sysinfo_overlay.dylib
codesign -s - --force sysinfo_overlay.dylib
```

## 在 LiveContainer 中使用
1. 模块 选项卡 → + → 导入模块，选 `sysinfo_overlay.dylib`。
2. 打开 White Album 2 的应用设置 → 将「模块文件夹 / Tweak Folder」指向该模块。
3. 启动 App，左上角出现悬浮窗。

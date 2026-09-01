# wa2-godot-iOS

> **非官方** WHITE ALBUM2 iOS 移植版，基于 Godot 4.7.2-stable（mono / C#）引擎。

本仓库只包含让游戏在 iOS 上运行所需的代码与配置，**不包含 WHITE ALBUM2 的任何游戏资源**，游戏本体资源请自行合法获取。

---

## 一、移植来源与派生关系（上游凭证）

本项目并非从零自研，而是对他人开源成果的逐层派生。完整的派生链如下：

```text
xbohe/wa2-godot-ios        ← 本仓库（当前 origin，iOS 适配持续维护）
        ↑ fork
Gdadfk/wa2-godot-ios       ← 初始 iOS 移植（commit 5c393ba "Initial iOS port"，作者 Gdadfk）
        ↑ fork
dorakyuraduang/wa2-godot   ← 上游基线（Android 版，AQUAPLUS 同人 Godot 引擎重制）
```

- **上游基线**：[`dorakyuraduang/wa2-godot`](https://github.com/dorakyuraduang/wa2-godot) 是把 WHITE ALBUM2 用 Godot 重制的 Android 版开源项目，版本节奏由它决定。
- **初始 iOS 移植**：[`Gdadfk/wa2-godot-ios`](https://github.com/Gdadfk/wa2-godot-ios) 完成了第一版把该 Android 项目搬到 iOS 的工作（commit `5c393ba`）。**本仓库的初始 iOS 代码即来自此 fork**，作者归 Gdadfk。
- **当前维护**：本仓库 `xbohe/wa2-godot-ios` 在 Gdadfk 的初始移植之上，继续同步上游、升级引擎、修复问题、完善构建与许可。

> 因此，本仓库的 iOS 适配代码同时承载了两层上游关系：底层的 Godot 重制逻辑来自 `dorakyuraduang/wa2-godot`，iOS 平台移植骨架来自 `Gdadfk/wa2-godot-ios`。两者均在下方许可与署名中体现。

---

## 二、上游许可与凭证说明

### 上游的许可演变

- **上游 `v0.2.9` 基线（本 fork 的起点）没有 LICENSE 文件** —— 当时上游仓库未声明任何许可证。
- 上游后续在 `main` 改为**按组件混合许可**框架（与本项目采用的框架一致）：
  - 移植代码（`script/**`、`scene/**`、`shader/**` 等）→ **Apache License 2.0**
  - `addons/wmv_video` → **MIT**（但其内置的 FFmpeg / godot-cpp 等仍各自许可）
  - `addons/texture_fonts` → **MIT**
  - `assets/sub.yaml` 字幕数据 → **第三方内容**（非 Apache-2.0 覆盖）
  - WHITE ALBUM2 游戏素材 → **明确不授予任何权利**

### 本 fork 的许可证选择

本仓库**跟随上游主线的混合许可框架**：

- 移植代码（范围见 [`PORTING_CODE.md`](./PORTING_CODE.md)）采用 **Apache License 2.0**；
- 保留上游的 [`NOTICE`](./NOTICE) 署名与 **AQUAPLUS 免责声明**；
- 本 fork 的 iOS 专属改动，是对上述 Apache-2.0 文件的**修改（modifications）**，再分发时同样按 Apache-2.0 提供（见第九节）。

> 本仓库**不是单一协议**作品，不同文件归属不同版权与协议，详见根目录协议文件（第九节）。

---

## 三、同步与版本状态

- 当前已同步到上游 **`wa2-godot 0.2.9`**，并叠加 iOS 平台适配。
- **未整体升级到上游 `v0.3b`**。决策依据：
  1. `v0.3b` 把视频播放换成 `addons/wmv_video`（FFmpeg GDExtension），但其二进制**只有 Android `.so` + Windows `.dll`，无 iOS 库**；官方也自述 beta、视频不稳；
  2. 上游 `v0.3b` 目标 Godot **4.5**，低于本 fork 的 **4.7.2**，脚本 API 适配方向相反；
  3. 非视频改动极小，收益低、风险高。
- 维持 **v0.2.9 基线 + Godot 4.7.2 + ogv 视频方案**。

---

## 四、移植内容：保留了上游哪些 / 未采用哪些

下表说明本 fork 相对上游 `dorakyuraduang/wa2-godot`（及 Gdadfk 初始移植）的取舍。带「iOS 新增/修改」的条目即本 fork 在 Apache-2.0 文件上的改动。

| 组成 | 处理 | 说明 |
|---|---|---|
| 引擎运行时脚本 `script/**/*.cs` | **保留上游（Apache-2.0）** | 资源读取 / 运行时逻辑；本 fork 仅在其中加入 iOS 分支（见下） |
| 场景 `scene/**/*.tscn`、动画 `assets/ani/**/*.tres`、着色器 `shader/**/*.gdshader` | **保留上游** | 贡献者原创实现，未复制原版源码 |
| `project.godot` / `wa2.csproj` / `default_bus_layout.tres` / `main.tscn` / `assets/font.map` | **保留上游（微调）** | iOS 侧仅改 `config/name`、features、`ios` workload 等 |
| **iOS 资源目录 `user://Wa2Res/`** | **iOS 新增** | 替代 Android 外部存储根目录；首启自动建 `IC`/`movie`/`sav` |
| **ogv / Theora 视频方案** | **iOS 保留（未采用上游方案）** | 上游 0.2.3+ 改用 `gde_gozen`（无 iOS 库）；v0.3b 改用 `wmv_video`（无 iOS 库）。iOS 保留内置解码器播 16 个 `movie/mvXXX0.ogv` |
| **启动校验**（13 pak + 16 ogv + `TitleMenu.SetResourcesReady`） | **iOS 新增** | Android 版无此逻辑 |
| **iOS 导出预设 + 文件共享** | **iOS 新增** | `export_presets.cfg` `[preset.1] name="iOS"`，开启「文件」App 与 iTunes 共享 |
| `addons/texture_fonts` | **保留但未采用（运行时无关）** | 仅编辑器生成中文字体贴图；运行时不依赖该插件 |
| `addons/gde_gozen` / `addons/wmv_video` 视频插件 | **未采用** | 无 iOS 原生库，无法在 iOS 构建 |
| 上游 `v0.3b` 非视频改动 | **未采用** | 维持 0.2.9 基线 |

> 简言之：**游戏逻辑 / 场景 / 着色器 / 工程骨架全部沿用上游 Apache-2.0 代码**；**iOS 平台适配（资源目录、视频、启动校验、导出预设）是叠加在那些文件上的修改**；**上游依赖原生扩展的视频方案因无 iOS 库而改回 ogv**。

---

## 五、本次升级带来的变化（0.1.8 → 0.2.9）

同步自上游的改动：

- 补全天气特效（雨、雪、烟等，改为 GPU 粒子实现）；
- 修复图片透明度错误；
- 修复视频播放闪退；
- 保存存档时同步保存历史对话（PC 原版无此功能）；
- 新增存档完整性检查，避免读取到损坏的存档；
- 修复部分语音逻辑错误；
- 修复部分情况下角色名字显示错误的问题。

iOS 平台适配（与上游 Android 版的差异）：

- **资源目录**：使用应用自身的文件共享目录（`user://`），不使用安卓的外部存储根目录；
- **视频播放**：沿用 Godot 内置解码器播放 `ogv`。上游 0.2.3 之后改用 `gde_gozen` 插件直接解码原版视频文件，但该插件是原生扩展、没有 iOS 版本，因此 iOS 保留 `ogv` 方案（需自行转码，见下文）；
- **启动校验**：首次启动会自动创建资源目录，并检查 `.pak` 与视频文件是否齐全，缺失时给出提示而不是直接崩溃；
- **导出配置**：保留 iOS 导出预设，并开启「文件」App 与 iTunes 文件共享，方便直接往应用目录里拖资源。

---

## 六、当前状态

- 仍建议做好存档备份：修复未经更长流程的穷尽回归，极端路径仍可能存在未发现问题。
- 旧存档兼容性：0.1.8 的存档可以被本版本读取，读档后立绘与天气状态不会恢复（旧存档里没有这些数据）。反过来，**本版本的存档不能给 0.1.8 使用**。

仅在以下设备上做过验证：

- iPad Pro 2020

---

## 七、安装方法

由于 iOS 系统限制较多，安装和资源导入流程相对复杂。

以下步骤以爱思助手为例进行说明，也可使用其他自签或文件传输工具。

### 1. 安装 IPA

1. 下载 `.ipa` 文件（自行构建，见「八、自行构建 IPA」一节）。
2. 将 iPhone 或 iPad 连接到电脑。
3. 在电脑上安装并打开爱思助手。
4. 使用爱思助手对 `.ipa` 文件进行自签。

   参考教程：[爱思助手 Apple ID 签名教程](https://pc.i4.cn/news_detail_38195.html)

5. 在爱思助手中打开：

   ```text
   我的设备 → 应用 → 导入安装
   ```

   然后选择已经自签完成的 `.ipa` 文件进行安装。

免费 Apple ID 自签的证书有效期为 7 天，到期后需要重新签名安装。

如果安装失败，可以尝试开启 iOS 开发者模式：

[开启开发者模式参考教程](https://jingyan.baidu.com/article/5bbb5a1b98f3f152eaa17975.html)

如果安装后提示"未受信任的开发者"，这是正常现象。请取消弹窗，然后手动信任开发者证书：

[信任开发者证书参考教程](https://pc.i4.cn/news_detail_27929.html)

### 2. 初始化游戏文件夹

安装完成后，请先打开一次游戏。

这一步是必须的。游戏首次启动后会在应用文件共享目录中自动创建资源文件夹，否则下一步无法访问对应目录。

### 3. 导入游戏资源

打开应用文件夹后，将 PC 版游戏资源复制到以下目录：

```text
White Album 2/Wa2Res/
```

需要复制的内容为：

```text
除 mv 开头以外的所有 .pak 文件
```

也就是说，PC 版中的主要 `.pak` 资源文件需要放入 `Wa2Res` 目录。请保持原有的目录结构（例如 `IC` 子目录也要一起复制），不要额外套一层文件夹。

### 4. 导入视频文件，可选

iOS 版使用 Godot 内置解码器，只能播放 **Theora 编码的 `ogv`** 文件，需要自行把原版视频转码后放入：

```text
White Album 2/Wa2Res/movie/
```

文件名需要与下表一致：

```text
mv000.ogv  mv010.ogv  mv020.ogv  mv070.ogv
mv080.ogv  mv090.ogv  mv100.ogv  mv110.ogv
mv120.ogv  mv130.ogv  mv140.ogv  mv200.ogv
mv210.ogv  mv220.ogv  mv230.ogv  mv240.ogv
```

视频文件缺失不会阻止游戏运行，对应的视频播放内容会被自动跳过。

### 5. 存档位置

游戏存档文件位于：

```text
White Album 2/sav/
```

### 6. 文件存放示例

打开 iPhone 或 iPad 系统自带的 文件 应用，打开：

```text
   浏览 → 我的iPhone/我的iPad → White Album 2
   ```
<p align="center">
  <img src="https://github.com/user-attachments/assets/69bf1bc8-a06c-4f3f-8f4b-f138378634c1" width="220" alt="打开 White Album 2 文件夹" />
  <img src="https://github.com/user-attachments/assets/2a4abda7-94bf-4c54-a4ed-6b8e6386f402" width="220" alt="Wa2Res 文件夹示例" />
  <img src="https://github.com/user-attachments/assets/dd2a693b-15c1-4a3f-8884-c30b0790c83b" width="220" alt="movie 文件夹示例" />
</p>

---

## 八、自行构建 IPA

仓库内置了 GitHub Actions 工作流，可以在 GitHub 提供的 macOS 机器上导出**未签名 IPA**，本地不需要 Mac。

> **构建已验证**：在 GitHub `macos-latest` runner 上可完整跑通「Godot 导出 Xcode 工程 → `xcodebuild archive`（关闭签名）→ 打包未签名 IPA → 上传 artifact」。无素材时也能成功产出可启动的壳（缺界面贴图/字体）。

步骤：

1. 打开自己仓库的 `Actions` 页面，选择 `Build iOS IPA (unsigned)`；
2. 点击 `Run workflow`，按需选择 `debug` 或 `release`；
3. 等待构建结束（首次约需较长时间，导出模板超过 1 GB，之后会走缓存）；
4. 在本次运行的 `Artifacts` 中下载 `wa2-ios-unsigned`，解压得到 `.ipa`；
5. 用爱思助手自签后安装。

### 关于界面素材

按上游约定，仓库的 `.gitignore` 排除了原游戏素材目录（属于版权内容）。**直接构建会得到一个缺少界面贴图与字体的包**——游戏能启动，但按钮和文字显示不出来。要得到可正常游玩的包，需要自行提供合法持有的游戏素材，注入方式见下文。

### 配置素材注入（可选）

工作流支持三种素材来源，按以下优先级生效：

1. 手动触发工作流时填写 `assets_url` 输入（zip 直链）；
2. 仓库 Secret `WA2_ASSETS_URL`（zip 直链）；
3. 从私有仓库 Release 下载，需同时配置以下三个 Secret：

   | Secret | 说明 |
   |---|---|
   | `WA2_ASSETS_REPO` | 私有素材仓库的 `owner/repo` 全名 |
   | `WA2_ASSETS_FILE` | 该仓库 Release `v1` 中的资产文件名（含扩展名，区分大小写） |
   | `WA2_ASSETS_TOKEN` | 具备该私有仓库读取权限的 Personal Access Token（需 `repo` scope） |

   > Release tag 固定为 `v1`，无法用 Secret 覆盖。zip 解压后须得到 `assets/grp`、`assets/se`、`assets/fonts/cn` 等目录。

工作流检测到仓库内已存在 `assets/grp` 时会直接使用；以上来源都没提供时会打印警告并继续构建（可用于验证工程能否编译通过）。

### 本地构建（有 Mac 的情况）

需要 Godot 4.7.2-stable **mono** 版与 .NET 9 SDK（含 `ios` workload）：

```bash
dotnet workload install ios
godot --headless --path . --import
godot --headless --path . --export-debug "iOS" ./build/ios/wa2.ipa
```

Godot 的 iOS 导出会生成 Xcode 工程，再用 `xcodebuild archive` 关闭签名打包即可，具体参数见 `.github/workflows/build-ios.yml`。

---

## 九、版权与 Apache-2.0 声明

本项目是 [WA2 Godot](https://github.com/dorakyuraduang/wa2-godot) 的 iOS 移植版（派生链见第一节），只包含移植所需的代码与配置，**不包含 White Album 2 的任何游戏资源**，游戏本体资源请自行合法获取。

本仓库**不是单一协议**作品，不同文件归属不同版权与协议，详见根目录协议文件：

- `LICENSE` — 仓库级复合许可声明（本文件为总纲）。
- `LICENSES/Apache-2.0.txt` — Apache License 2.0 全文。
- `NOTICE` — Apache-2.0 要求的署名声明（含对上游 WA2 Godot 的派生说明、Gdadfk 初始 iOS 移植署名、与 AQUAPLUS 免责声明）。
- `PORTING_CODE.md` — 明确标注为 **Apache-2.0** 的移植代码范围，及**本 fork 的 iOS 修改清单**。
- `THIRD_PARTY_NOTICES.md` — 第三方组件许可（texture_fonts 插件 MIT、字幕数据第三方、Godot 引擎 MIT 等）。
- `SUBTITLE_NOTICE.md` — `assets/sub.yaml` 字幕数据的第三方版权说明。
- `ASSET_POLICY.md` — 不纳入游戏素材的边界与贡献者规则。

### 本 fork 对上游 Apache-2.0 文件的修改声明

依据 **Apache License 2.0 第 4(b)、(d) 条**，本 fork 在 [`PORTING_CODE.md`](./PORTING_CODE.md) 所列 Apache-2.0 文件上叠加的 iOS 适配属于「修改（modifications）」。再分发者须：

1. **标注改动**：在被修改的文件或随附说明中保留显著的修改提示（见 `PORTING_CODE.md` 的「Modifications in this iOS fork」一节列出的 iOS 专属新增/修改路径）；
2. **保留 NOTICE**：随分发保留 [`NOTICE`](./NOTICE)；
3. **附许可全文**：随分发提供 `LICENSES/Apache-2.0.txt`。

本 fork 的 iOS 专属新增 / 修改（相对上游 Apache-2.0 文件）主要包括：

- `script/Wa2EngineMain.cs` — 加入 `user://Wa2Res/` 资源目录、`RequiredPakPaths` / `ExpectedMoviePaths` / `ValidateIosMovies()` 启动校验、`VideoStreamTheora` 播 ogv 的 `PlayMovie` 分支；
- `script/TitleMenu.cs` — 新增 iOS-only 的 `SetResourcesReady(bool)`；
- `export_presets.cfg` — 新增 `[preset.1] name="iOS"` 及文件共享开关；
- `project.godot` / `wa2.csproj` — iOS 相关微调；
- 其余 `script/**`、`scene/**`、`shader/**`、`assets/ani/**` 等沿用上游，未改动或仅随同步小幅调整。

### 协议摘要

| 组成 | 协议 | 说明 |
|---|---|---|
| 移植代码（`PORTING_CODE.md` 列出的路径） | **Apache-2.0** | 贡献者原创的资源读取、运行时逻辑、着色器、场景与工程集成 |
| `addons/texture_fonts` | **MIT** | 独立的 Godot 编辑器插件（© 2021-2024 Micky / Laila L.） |
| `assets/sub.yaml` 字幕数据 | **第三方** | 含萌娘百科 / CK-GAL汉化组内容，非 Apache-2.0 覆盖，见 `SUBTITLE_NOTICE.md` |
| White Album 2 游戏素材 | **未授权** | 不随仓库分发，AQUAPLUS 保留全部权利 |

> 本项目为非官方作品，与 AQUAPLUS 无关、未获其授权或背书。

# wa2-godot-iOS

本项目移植自 [dorakyuraduang/wa2-godot](https://github.com/dorakyuraduang/wa2-godot)。

当前项目已同步到上游 `wa2-godot 0.2.9`，并叠加 iOS 平台适配。

## 本次升级带来的变化（0.1.8 → 0.2.9）

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

## 当前状态

- 仍建议做好存档备份：修复未经更长流程的穷尽回归，极端路径仍可能存在未发现问题。
- 旧存档兼容性：0.1.8 的存档可以被本版本读取，读档后立绘与天气状态不会恢复（旧存档里没有这些数据）。反过来，**本版本的存档不能给 0.1.8 使用**。

仅在以下设备上做过验证：

- iPad Pro 2020

## 安装方法

由于 iOS 系统限制较多，安装和资源导入流程相对复杂。

以下步骤以爱思助手为例进行说明，也可使用其他自签或文件传输工具。

### 1. 安装 IPA

1. 下载 `.ipa` 文件（自行构建，见「自行构建 IPA」一节）。
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

## 自行构建 IPA

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

## 版权声明

本项目只包含移植所需的代码与配置，不包含 White Album 2 的任何游戏资源。游戏本体资源请自行合法获取。

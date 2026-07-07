# wa2-godot-iOS

本项目移植自 [dorakyuraduang/wa2-godot](https://github.com/dorakyuraduang/wa2-godot)。

当前项目基于 `wa2-godot 0.1.8` 版本进行 iOS 移植与适配。

## 当前状态

目前已知仍存在以下问题：

- 缺少天气特效；
- 存在语音对话相关 bug；
- 部分功能尚未经过完整测试。

本项目目前仅在以下设备和系统版本上进行了少量功能调试：

- iPhone 13 mini，iOS 17.6.1
- iPad Air 6，M2，iPadOS 18.5

目前主线剧情暂未发现严重bug。由于测试设备和作者精力有限，其他设备、系统版本或剧情分支内容可能仍存在未发现的问题。

## 安装方法

由于 iOS 系统限制较多，安装和资源导入流程相对复杂。

以下步骤以爱思助手为例进行说明，也可使用其他自签或文件传输工具。

### 1. 安装 IPA

1. 下载本项目提供的 `.ipa` 文件。
2. 将 iPhone 或 iPad 连接到电脑。
3. 在电脑上安装并打开爱思助手。
4. 使用爱思助手对 `.ipa` 文件进行自签。

   参考教程：[爱思助手 Apple ID 签名教程](https://pc.i4.cn/news_detail_38195.html)

5. 在爱思助手中打开：

   ```text
   我的设备 → 应用 → 导入安装
   ```

   然后选择已经自签完成的 `.ipa` 文件进行安装。

如果安装失败，可以尝试开启 iOS 开发者模式：

[开启开发者模式参考教程](https://jingyan.baidu.com/article/5bbb5a1b98f3f152eaa17975.html)

如果安装后提示“未受信任的开发者”，这是正常现象。请取消弹窗，然后手动信任开发者证书：

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

也就是说，PC 版中的主要 `.pak` 资源文件需要放入 `Wa2Res` 目录。

### 4. 导入视频文件，可选

如果需要播放游戏内视频，请准备 `ogv` 格式的视频文件，并放入：

```text
White Album 2/Wa2Res/movie/
```

视频文件缺失不会阻止游戏运行，但对应的视频播放内容会被跳过。

请自行从合法持有的游戏资源中准备所需视频文件。本项目不提供任何原游戏资源、视频、音频或其他版权内容。

### 5. 存档位置

游戏存档文件位于：

```text
White Album 2/sav/
```

该存档目录与原项目 `wa2-godot 0.1.8` 版本的存档格式兼容。

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



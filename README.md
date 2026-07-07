本项目移植自dorakyuraduang/wa2-godot
（https://github.com/dorakyuraduang/wa2-godot）
目前项目移植自wa2-godot 0.18版本，已知缺少天气特效，且存在语音对话bug
本项目已在以下机型及系统上进行少量功能调试（精力有限敬请谅解）：
iPhone 13 mini (iOS 17.6.1)
iPad Air 6 (M2) (iPadOS 18.5)
目前主线剧情推进暂时没有发现严重bug

游戏安装方法(由于iOS系统封闭性强，所以安装过程较为复杂)
（PS：因为iOS设备用爱思助手挺方便的，所以这里的教程基本上都是用爱思助手（不是在打广告）
  如果各位有别的工具进行自签和文件传输的话请自行选择）  
1.下载.ipa文件
2.将iPhone/iPad连接电脑
3.电脑下载爱思助手，使用爱思助手对ipa文件进行自签
（教程参考：https://pc.i4.cn/news_detail_38195.html，使用 Apple ID 签名方法） 
4.在爱思助手中打开我的设备→应用→导入安装，然后选择自签好的ipa文件进行安装
（如果安装时失败，尝试开启开发者模式：https://jingyan.baidu.com/article/5bbb5a1b98f3f152eaa17975.html；
  如果安装后弹窗显示未受信任的开发者，是正常现象，直接点取消，然后按下述教程信任即可：
  https://pc.i4.cn/news_detail_27929.html）
5.安装完成后先打开游戏（必须先打开游戏，否则下一步的游戏文件夹无法访问）
6.把pc版的所有除了mv开头.pak后缀文件传输到游戏应用文件夹的Wa2Res文件夹下
7、如果需要播放视频，请使用ogv格式的视频，将视频文件放置在游戏应用文件夹的Wa2Res/movie下
(6、7两步也可以使用爱思助手完成，参照这个教程：https://zhuanlan.zhihu.com/p/524725414
  但是在爱思助手电脑端选择应用文件夹时直接选择White Album 2，不用选择爱思助手极速版
  项目原作者给出了PS3版ogv视频的分享地址，这里借用一下：
  pan.baidu.com/s/1UTSgchekEH0PVtM7yck94w 密码 qjqa)
8.游戏存档文件存放在游戏应用文件夹的sav文件夹里，与原项目0.18版本的存档文件兼容

# FFmpegOutRTSP
# 项目简介

在Unity中，使用FFmpegOut插件，对Unity中摄像机画面进行捕捉，并通过RTSP进行推流。项目中包含RTSP服务器“rtsp-simple-server.exe”，以当前电脑活跃的ip接口作为推流地址，端口号默认为8554，因此可在项目中输入自己电脑的ip实现推流功能。

# 项目结构

Unity项目位于分支master中。FFmpegOut插件路径为\Assets\FFmpegOut；RTSP服务器路径为\Assets\RTSPServer。此外，ffmpeg.exe程序位于\Assets\StreamingAssets\FFmpegOut\Windows。

# 使用示例

项目中默认场景test1中默认配置了4个摄像机，可在检查器中对CameraCapture脚本进行参数配置。

## 配置编码器

项目中默认的编码器是“H.264 QSV (MP4)”，可根据自己需求修改为“H.264 NVIDIA (MP4)”等，如下图所示：

![image.png](https://cdn.nlark.com/yuque/0/2024/png/39091616/1711441022103-c6959c7b-d250-496f-8038-fc5590c30140.png#averageHue=%233d3d3d&clientId=ue664da53-b751-4&from=paste&height=130&id=u4aeeb8cd&originHeight=162&originWidth=454&originalType=binary&ratio=1.25&rotation=0&showTitle=false&size=17970&status=done&style=none&taskId=u7d0930b9-c965-47c2-9537-8de38ae89d5&title=&width=363.2)
![image.png](https://cdn.nlark.com/yuque/0/2024/png/39091616/1711441042956-b46cfd1f-cf56-4b7c-be83-2024c09fa277.png#averageHue=%239f9d9b&clientId=ue664da53-b751-4&from=paste&height=352&id=u5439c910&originHeight=440&originWidth=458&originalType=binary&ratio=1.25&rotation=0&showTitle=false&size=50622&status=done&style=none&taskId=u83fc34af-25b4-4969-b116-f3158193965&title=&width=366.4)

## 配置推流地址

在Url处配置推流地址，输入你自己电脑的ip，如下图：

![image.png](https://cdn.nlark.com/yuque/0/2024/png/39091616/1711441124444-142419bc-4a58-4894-8c90-8b9d5222cfa7.png#averageHue=%233b3b3b&clientId=ue664da53-b751-4&from=paste&height=133&id=u17b6258c&originHeight=166&originWidth=447&originalType=binary&ratio=1.25&rotation=0&showTitle=false&size=18019&status=done&style=none&taskId=u2d0b9ced-b74d-43a6-8957-07fd1587253&title=&width=357.6)

此外，还可以配置推流画面的宽度、高度以及帧率，这里不再赘述。

# 修改项目

如果您对于默认功能不够满意，想要按照自己的需求进行修改，可修改脚本CameraCapture.cs和RTSPServerLoader.cs，以及使用到的其他脚本。这些脚本均位于路径\Assets\FFmpegOut\Runtime下。

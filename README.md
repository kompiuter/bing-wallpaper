# Bing每日壁纸

发布一个开源小软件，[Bing每日壁纸](https://github.com/jadepeng/bing-wallpaper)。

该小软件可以自动获取Bing的精美图片设置为壁纸，并且支持随机切换历史壁纸，查看壁纸故事。

欢迎大家下载使用，点star！有问题请留言或者提issue。

想了解技术原理的请看 [技术方案](./技术方案.md)

## V1.3.2 新增特性

- 新增桌面控件显示设置
- 桌面控件增加灰色背景，字体不再有毛边

## V1.2.1 新增特性

### 国际化

支持中英文，跟随系统语言

### 桌面widget

桌面右下角，新增一个widget窗体，可以切换壁纸

![桌面widget](https://www.github.com/jadepeng/blogpic/raw/master/pic/21/1534851591284.png)

点击标题，会打开壁纸故事。

### 定时切换支持选择时间

不再是默认的1小时，允许你自己选择周期

![定时切换支持选择时间](https://www.github.com/jadepeng/blogpic/raw/master/pic/21/1534851696908.png)

<p align="center">
  <img src="https://github.com/kompiuter/bing-wallpaper/blob/master/resources/geckorain.jpg?raw=true" alt="gecko" width="728"/>
</p>


## 如何使用

该程序没有主窗口，是托盘程序，点击图片，操作相关菜单即可。


## 功能特性

- 自动获取Bing最新图片并设置为壁纸
- 壁纸故事,  你还可以查看壁纸后面的故事
- 历史壁纸，支持查看最近两年的壁纸
- 随机切换，随机获取几年的壁纸，穿梭时光之中
- 定时切换，开启后每一小时自动切换壁纸


### 壁纸故事

你还可以查看壁纸后面的故事，支持上下切换

![壁纸故事](https://www.github.com/jadepeng/blogpic/raw/master/pic/20/1534757210215.png)

### 随机切换

点击后，会随机从历史数据中挑选一张并显示

![随机切换](https://www.github.com/jadepeng/blogpic/raw/master/pic/20/1534757121712.png)


### 定时切换

开启后，每1小时自动切换，相当于自动点击随机切换。


## 开发缘起

后知后觉的发现，搜狗壁纸助手已经关闭服务，不能获取新的壁纸，回想起Bing每日提供精美的图片，因此考虑写一个小工具，可以自动从bing获取图片并设置为壁纸。

## Usage

### 自己编译

 - 下载代码

```bash
git clone https://github.com/jadepeng/bing-wallpaper.git

````

 - Open .sln file in Visual Studio

 - Build

 - Run

```
.../BingWallpaper/bin/Release/BingWallpaper.exe
```

### 下载二进制

从[Release](https://github.com/jadepeng/bing-wallpaper/releases)下载最新的构建包，笔者是win10 X64。不能运行的自己下载代码编译。

## 兼容性

仅支持Windows

已在win10 X64测试，有问题请反馈!

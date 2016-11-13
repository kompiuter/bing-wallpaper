<p align="center">
  <img src="https://github.com/kompiuter/bing-wallpaper/blob/master/resources/geckorain.jpg?raw=true" alt="gecko" width="728"/>
</p>
# Bing Wallpaper
Keep your wallpaper up to date with Bing's great image of the day, every day.

# Usage

## Build yourself (recommended)

 - Grab the code

```bash
git clone https://github.com/kompiuter/bing-wallpaper.git
````

 - Open .sln file in Visual Studio

 - Build the application

 - Run the application

```
.../BingWallpaper/bin/Release/BingWallpaper.exe
```

## Download executable

If you don't want to go through the process of building you can find an executable on my personal website:

https://kyriacos.me/projects/bing-wallpaper/binary.zip

However I do recommend you download the source code and build it yourself!

# What does this do?

Running the executable will create a background process that changes your desktop background to Bing's image of the day then does so again every 24 hours.

It adds a key to the registry so that it is run on startup.

A tray icon is visible while the process is running (thanks @MichaelMK) which allows you to either force a wallpaper update, disable startup running or terminate the process.

Errors are written to `log.txt`, found in the executable's directory.

# Uninstall

Disable running on startup through the tray icon and delete the executable.

If you forgot to disable startup running and deleted the executable, you can still disable it by either:
 
 - Going to your registry (regedit in Run) and deleting the key `BingWallpaper` under:
 ```
 HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run
 ```
 
**OR**
 
 - Going to Task Manager -> Startup and disabling `BingWallpaper` from there.

# Compatibility

This only works on Windows systems.

So far I've tested it on Windows 7 and Windows 10 as an admin user. If you face any problems please open an issue!

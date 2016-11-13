<img src="https://github.com/kompiuter/bing-wallpaper/blob/master/resources/geckorain.jpg?raw=true" alt="gecko" width="768">
# Bing Wallpaper
Keep your wallpaper up to date with Bing's great image of the day.

# Usage

 - Grab the code

```bash
git clone https://github.com/MichaelMK/bing-wallpaper.git
````

 - Build

 - Run the application

```
.../BingWallpaper/bin/Release/BingWallpaper.exe
```

# What does this do?

Running the executable will create a background process that immediately changes your desktop background to Bing's image of the day then does so again every 24 hours.

It will automatically add a key to the registry to execute on startup, so that you don't have to run it after each reboot.

Messages and/or errors are written to `log.txt`, found in the executable's directory.

I (MichaelMK) added the application to the system tray, so if that breaks let me know

# Uninstall

 - Go to Task Manager and end the process `BingWallpaper.exe` - or do it through the system tray

 - Go to your registry (regedit in Run) and delete the key `BingWallpaper` under:

```
HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run
```

# Compatibility

This only works on Windows systems.

So far I've tested it on Windows 7 and Windows 10 as an admin user. If you face any problems please open an issue!

# Bing Wallpaper
Background process that changes your wallpaper every day according to Bing's image of the day.

# Usage

Grab the code

```bash
https://github.com/kompiuter/bing-wallpaper.git
````

Build the code

Run the application

```
.../BingWallpaper/bin/Release/BingWallpaper.exe
```

# What does this do?

Running the application will create a background process that changes your desktop background to Bing's image of the day and continues to do so every 24 hours.

It will add a key to the registry so the application executes on startup, so you don't have to run it each time.

It will also log information to a file log.txt, found in the executable's directory. You can look at this log to determine wether the application is running successfully.

# I don't want this anymore, how do I remove it?

First go to Task Manager and end the process called "BingWallpaper.exe".

Finally go to your registry and delete the key named "BingWallpaper", found here:

```
HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run

# Compatibility

I've only tested this on Windows 10 as an admin user. If you face any problems please open an issue!
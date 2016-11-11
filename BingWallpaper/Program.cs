using BingWallpaper.Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Timers;
using System.Runtime.InteropServices;

namespace BingWallpaper
{
    class Program
    {
        #region unsafe
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        #endregion

        static void Main(string[] args)
        {
            // Hide console
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);

            Log.Print("bing wallpaper process started");
            IImageProvider provider = new BingDayImageProvider("en-CY");
            Log.Print("setting program to run on startup");
            SetStartup();

            Timer timer = new Timer();
            timer.Interval = 1000 * 60 * 60 * 24; // 24 hours
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += (s, e) => SetWallpaper(provider);
            timer.Start();
            
            // Set wallpaper on first run
            SetWallpaper(provider);

            // Keep process alive
            Console.Read();
        }

        static void SetWallpaper(IImageProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            try
            {
                Log.Print("asking for image uri");
                var uri = provider.Uri().Result;
                Log.Print($"fetching image from \"{uri.ToString()}\"");
                Wallpaper.Set(uri, Wallpaper.Style.Stretched);
                Log.Print("wallpaper has been updated");
            }
            catch (Exception e)
            {
                Log.Print("failed to updated wallpaper");
                Log.Print(e);
            }
        }

        static void SetStartup()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            rk.SetValue("BingWallpaper", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BingWallpaper.exe"));
        }
    }
}

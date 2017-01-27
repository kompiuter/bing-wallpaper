using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace BingWallpaper
{
    public partial class MainForm : Form
    {
        private NotifyIcon _trayIcon;
        private ContextMenu _trayMenu;

        private IImageProvider _provider;

        public MainForm(IImageProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            _provider = provider;
            
            // Register application with registry so that it runs on startup
            SetStartup(true);

            AddTrayIcons();
            
            // Set wallpaper every 24 hours
            var timer = new System.Timers.Timer();
            timer.Interval = 1000 * 60 * 60 * 24; // 24 hours
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += (s, e) => SetWallpaper();
            timer.Start();

            // Set wallpaper on first run
            SetWallpaper();
        }

        /// <summary>
        /// SetStartup will set the application to automatically launch on startup if b is true,
        /// else it will remove the key from the registry.
        /// </summary>
        public void SetStartup(bool b)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (b)
                rk.SetValue("BingWallpaper", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BingWallpaper.exe"));
            else
            {
                if (rk.GetValue("BingWallpaper") != null)
                    rk.DeleteValue("BingWallpaper");
            }
        }

        public void AddTrayIcons()
        {
            // Create a simple tray menu with only one item.
            _trayMenu = new ContextMenu();
    
            _trayMenu.MenuItems.Add("Force wallpaper update", (s, e) => SetWallpaper());

            var launch = new MenuItem("Launch on startup");
            launch.Checked = true;
            launch.Click += OnLaunch;
            _trayMenu.MenuItems.Add(launch);

            _trayMenu.MenuItems.Add("Exit", (s,e) => Application.Exit());

            // Create a tray icon. Here we are setting the tray icon to be the same as the application's icon
            _trayIcon = new NotifyIcon();
            _trayIcon.Text = "Bing Wallpaper";
            //_trayIcon.Icon = new Icon("Resources/bing-icon.ico", 40, 40);
            _trayIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            // Add menu to tray icon and show it.
            _trayIcon.ContextMenu = _trayMenu;
            _trayIcon.Visible = true;
        }

        private void OnLaunch(object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            item.Checked = !item.Checked;
            SetStartup(item.Checked);
        }

        public async void SetWallpaper()
        {
            try
            {
                var img = await _provider.GetImage();
                Wallpaper.Set(img, Wallpaper.Style.Stretched);
                ShowSetWallpaperNotification();
            }
            catch (Exception e)
            {
                ShowErrorNotification();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }
             
        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                _trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }

        #region Notifications

        private void ShowSetWallpaperNotification()
        {
            _trayIcon.BalloonTipText = "Wallpaper has been set to Bing's image of the day!";
            _trayIcon.BalloonTipIcon = ToolTipIcon.Info;
            _trayIcon.Visible = true;
            _trayIcon.ShowBalloonTip(5000);
        }

        private void ShowErrorNotification()
        {
            _trayIcon.BalloonTipText = "Could not update wallpaper, please check your internet connection.";
            _trayIcon.BalloonTipIcon = ToolTipIcon.Error;
            _trayIcon.Visible = true;
            _trayIcon.ShowBalloonTip(5000);
        }

        #endregion
    }
}

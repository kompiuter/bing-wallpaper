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
        private IImageProvider _provider;
        private Settings _settings;

        public MainForm(IImageProvider provider, Settings settings)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            _provider = provider;

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            _settings = settings;
            
            // Register application with registry
            SetStartup(_settings.LaunchOnStartup);

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

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        /// <summary>
        /// SetStartup will set the application to automatically launch on startup if launch is true,
        /// else it will prevent it from doing so.
        /// </summary>
        public void SetStartup(bool launch)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (launch)
            {
                if (rk.GetValue("BingWallpaper") == null)
                    rk.SetValue("BingWallpaper", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BingWallpaper.exe"));
            }
            else
            {
                if (rk.GetValue("BingWallpaper") != null)
                    rk.DeleteValue("BingWallpaper");
            }
        }

        /// <summary>
        /// SetWallpaper fetches the wallpaper from Bing and sets it
        /// </summary>
        public async void SetWallpaper()
        {
            try
            {
                var img = await _provider.GetImage();
                Wallpaper.Set(img, Wallpaper.Style.Stretched);
                ShowSetWallpaperNotification();
            }
            catch
            {
                ShowErrorNotification();
            }
        }
        
        #region Tray Icons

        private NotifyIcon _trayIcon;
        private ContextMenu _trayMenu;

        public void AddTrayIcons()
        {
            // Create a simple tray menu with only one item.
            _trayMenu = new ContextMenu();

            _trayMenu.MenuItems.Add("Force wallpaper update", (s, e) => SetWallpaper());

            var launch = new MenuItem("Launch on startup");
            launch.Checked = _settings.LaunchOnStartup;
            launch.Click += OnStartupLaunch;
            _trayMenu.MenuItems.Add(launch);

            _trayMenu.MenuItems.Add("Exit", (s, e) => Application.Exit());

            // Create a tray icon. Here we are setting the tray icon to be the same as the application's icon
            _trayIcon = new NotifyIcon();
            _trayIcon.Text = "Bing Wallpaper";
            //_trayIcon.Icon = new Icon("Resources/bing-icon.ico", 40, 40);
            _trayIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            // Add menu to tray icon and show it.
            _trayIcon.ContextMenu = _trayMenu;
            _trayIcon.Visible = true;
        }

        private void OnStartupLaunch(object sender, EventArgs e)
        {
            var launch = (MenuItem)sender;
            launch.Checked = !launch.Checked;
            SetStartup(launch.Checked);
            _settings.LaunchOnStartup = launch.Checked;
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

        #endregion

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

using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;

namespace BingWallpaper
{
    public partial class MainForm : Form, IWallpaperControl
    {
        private BingImageProvider _provider;
        private Settings _settings;
        private HistoryImage _currentWallpaper;

      
        System.Timers.Timer autoChangeTimer;

        #region 控制Interface

        public HistoryImage CurrentWallpaper
        {
            get { return _currentWallpaper; }

            set {
                this._currentWallpaper = value;
                this.WallpaperChange(value);
            }
        }

        public delegate void WallpaperChangeHandler(HistoryImage paper);

        public event WallpaperChangeHandler OnWallpaperChange;

        public void WallpaperChange(HistoryImage paper)
        {
            OnWallpaperChange?.Invoke(paper);
        }

        public async void NextWallpaper()
        {
            this.CurrentWallpaper = HistoryImageProvider.Next(this.CurrentWallpaper.Date);
            await UpdateWallpaper();
        }

        public async void PreWallpaper()
        {
            this.CurrentWallpaper = HistoryImageProvider.Previous(this.CurrentWallpaper.Date);
            await UpdateWallpaper();
        }

        public void RandomWallpaper()
        {
            this.SetRandomWallpaper();
        }
        #endregion

        public MainForm(BingImageProvider provider, Settings settings)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            _provider = provider;

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            _settings = settings;

            // LaunchOnStartup
            SetStartup(_settings.LaunchOnStartup);

            AddTrayIcons();
            
            // 定时更新
            var timer = new System.Timers.Timer();
            timer.Interval = 1000 * 60 * 60 * 24; // 24 hours
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += (s, e) => GetLatestWallpaper();
            timer.Start();

            // Create Auto Change Task
            if (_settings.AutoChange)
            {
                CreateAutoChangeTask();
            }

            // Get the latest wallpaper
            GetLatestWallpaper();

            // open Desk Widget
            new DeskWidget(this).Show();
        }

        private void CreateAutoChangeTask()
        {
            autoChangeTimer = new System.Timers.Timer();
            autoChangeTimer.Interval = getChangeInterval(); // 1 hour
            autoChangeTimer.AutoReset = true;
            autoChangeTimer.Enabled = true;
            autoChangeTimer.Elapsed += (s, e) => SetRandomWallpaper();
            autoChangeTimer.Start();
        }

        private int getChangeInterval()
        {
            if (_settings.AutoChangeInterval.Contains("minutes"))
            {
                return 1000 * 60 * int.Parse(_settings.AutoChangeInterval.Replace("minutes", "").Trim());
            }
            return 1000 * 60 * 60 * int.Parse(_settings.AutoChangeInterval.Replace("hours", "").Replace("hour", "").Trim());
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        /// <summary>
        /// Launch On Startup
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
        /// Get Latest Wallpaper
        /// </summary>
        public async void GetLatestWallpaper()
        {
            try
            {
                CurrentWallpaper = await _provider.GetLatestImage();
                await UpdateWallpaper();
                // 保存到历史记录
                HistoryImageProvider.AddImage(CurrentWallpaper);
            }
            catch
            {
                ShowErrorNotification();
            }
        }

        private async System.Threading.Tasks.Task UpdateWallpaper()
        {
            if (CurrentWallpaper != null)
            {
                var img = await CurrentWallpaper.getImage();
                Wallpaper.Set(img, Wallpaper.Style.Stretched);
                ShowSetWallpaperNotification();
            }
        }

        /// <summary>
        /// Fetch a random Wallpaper from history
        /// </summary>
        public async void SetRandomWallpaper()
        {
            try
            {
                CurrentWallpaper = HistoryImageProvider.getRandom();
                await UpdateWallpaper();
            }
            catch(Exception ex)
            {
                ShowErrorNotification();
            }
        }

        public void SetCopyrightTrayLabel(string copyright, string copyrightLink)
        {
            _settings.ImageCopyright = copyright;
            _settings.ImageCopyrightLink = copyrightLink;

            _copyrightLabel.Text = copyright;
            _copyrightLabel.Tag = copyrightLink;
        }
        
        #region Tray Icons

        private NotifyIcon _trayIcon;
        private ContextMenu _trayMenu;
        private MenuItem _copyrightLabel;

        public void AddTrayIcons()
        {
            // Create a simple tray menu with only one item.
            _trayMenu = new ContextMenu();

            // Copyright button
            _copyrightLabel = new MenuItem(Resource.AppName);
            _copyrightLabel.Click += (s, e) =>
            {
                //var url = ((MenuItem)s).Tag.ToString();
                //if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                //    System.Diagnostics.Process.Start(url);
            };
            _trayMenu.MenuItems.Add(_copyrightLabel);

            _trayMenu.MenuItems.Add("-");

            _trayMenu.MenuItems.Add(Resource.WallPaperStory, (s, e) =>
            {
                new WallpaperStoryForm(CurrentWallpaper).ShowDialog();
            });

            _trayMenu.MenuItems.Add(Resource.ForceUpdate, (s, e) => GetLatestWallpaper());

            _trayMenu.MenuItems.Add(Resource.Random, (s, e) => SetRandomWallpaper());

            // Save image button
            var save = new MenuItem(Resource.Save);
            save.Click += async (s, e) =>
            {
                if (CurrentWallpaper != null)
                {
                    var fileName = string.Join("_", _settings.ImageCopyright.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
                    var dialog = new SaveFileDialog
                    {
                        DefaultExt = "jpg",
                        Title = Resource.SaveDialgName,
                        FileName = fileName,
                        Filter = "Jpeg Image|*.jpg",
                    };
                    if (dialog.ShowDialog() == DialogResult.OK && dialog.FileName != "")
                    {
                        var image = await CurrentWallpaper.getImage();
                        image.Save(dialog.FileName, ImageFormat.Jpeg);
                        System.Diagnostics.Process.Start(dialog.FileName);
                    }
                }
            };
            _trayMenu.MenuItems.Add(save);

            // Separator,下面显示设置
            _trayMenu.MenuItems.Add("-");

            var launch = new MenuItem(Resource.LaunchOnStartup);
            launch.Checked = _settings.LaunchOnStartup;
            launch.Click += OnStartupLaunch;
            _trayMenu.MenuItems.Add(launch);


            var timerChange = new MenuItem(Resource.IntervalChange);
            timerChange.Checked = _settings.AutoChange;

            var timeRanges = new string[] { "10 minutes", "30 minutes", "1 hour", "2 hours", "3 hours", "4 hours", "5 hours", "6 hours", "12 hours" };

            foreach(var timeRange in timeRanges)
            {
                var rangeMenu = new MenuItem(timeRange);
                rangeMenu.Checked = _settings.AutoChangeInterval == timeRange;
                rangeMenu.Click += RangeMenu_Click; ;
                timerChange.MenuItems.Add(rangeMenu);
            }
            // 
            //timerChange.Click += OnAutoChange;
            _trayMenu.MenuItems.Add(timerChange);

            // Separator
            _trayMenu.MenuItems.Add("-");

            _trayMenu.MenuItems.Add(Resource.Exit, (s, e) => Application.Exit());

            _trayIcon = new NotifyIcon();
            _trayIcon.Text = Resource.AppName;
            //_trayIcon.Icon = new Icon("Resources/bing-icon.ico", 40, 40);
            _trayIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            // open tray icon on left click
            _trayIcon.MouseUp += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    mi.Invoke(_trayIcon, null);
                }
            };

            // Add menu to tray icon and show it.
            _trayIcon.ContextMenu = _trayMenu;
            _trayIcon.Visible = true;
        }

        private void RangeMenu_Click(object sender, EventArgs e)
        {
            var intervalMenu = (MenuItem)sender;      
            foreach(MenuItem subMenu in intervalMenu.Parent.MenuItems)
            {
                subMenu.Checked = false;
            }
            intervalMenu.Checked = !intervalMenu.Checked;

            if (autoChangeTimer != null)
            {
                autoChangeTimer.Stop();
                autoChangeTimer.Dispose();
                autoChangeTimer = null;
            }

            _settings.AutoChange = intervalMenu.Checked;
            _settings.AutoChangeInterval = intervalMenu.Text;

            if (intervalMenu.Checked)
            {
                CreateAutoChangeTask();
            }
           
        }

        private void OnStartupLaunch(object sender, EventArgs e)
        {
            var launch = (MenuItem)sender;
            launch.Checked = !launch.Checked;
            SetStartup(launch.Checked);
            _settings.LaunchOnStartup = launch.Checked;
        }

        //private void OnAutoChange(object sender, EventArgs e)
        //{
        //    var autochange = (MenuItem)sender;
        //    autochange.Checked = !autochange.Checked;

        //    if (autochange.Checked)
        //    {
        //        CreateAutoChangeTask();
        //    }
        //    else
        //    {
        //        if (autoChangeTimer != null)
        //        {
        //            autoChangeTimer.Stop();
        //            autoChangeTimer.Dispose();
        //            autoChangeTimer = null;
        //        }
        //    }

        //    _settings.AutoChange = autochange.Checked;
        //}

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

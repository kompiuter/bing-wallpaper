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
            
            // 开机启动注册
            SetStartup(_settings.LaunchOnStartup);

            AddTrayIcons();
            
            // 定时更新
            var timer = new System.Timers.Timer();
            timer.Interval = 1000 * 60 * 60 * 24; // 24 hours
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += (s, e) => SetWallpaper();
            timer.Start();

            // 自动切换
            if (_settings.AutoChange)
            {
                CreateAutoChangeTask();
            }

            // 更新壁纸
            SetWallpaper();

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
            if (_settings.AutoChangeInterval.Contains("分钟"))
            {
                return 1000 * 60 * int.Parse(_settings.AutoChangeInterval.Replace("分钟", ""));
            }
            return 1000 * 60 * 60 * int.Parse(_settings.AutoChangeInterval.Replace("小时", ""));
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        /// <summary>
        /// 开启启动
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
        /// 获取最新壁纸
        /// </summary>
        public async void SetWallpaper()
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
            var img = await CurrentWallpaper.getImage();
            Wallpaper.Set(img, Wallpaper.Style.Stretched);
            ShowSetWallpaperNotification();
        }

        /// <summary>
        /// 随机壁纸
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
            _copyrightLabel = new MenuItem("Bing每日壁纸");
            _copyrightLabel.Click += (s, e) =>
            {
                //var url = ((MenuItem)s).Tag.ToString();
                //if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                //    System.Diagnostics.Process.Start(url);
            };
            _trayMenu.MenuItems.Add(_copyrightLabel);

            _trayMenu.MenuItems.Add("-");

            _trayMenu.MenuItems.Add("壁纸故事", (s, e) =>
            {
                new WallpaperStoryForm(CurrentWallpaper).ShowDialog();
            });

            _trayMenu.MenuItems.Add("强制更新", (s, e) => SetWallpaper());

            _trayMenu.MenuItems.Add("随机切换", (s, e) => SetRandomWallpaper());

            // Save image button
            var save = new MenuItem("保存壁纸");
            save.Click += async (s, e) =>
            {
                if (CurrentWallpaper != null)
                {
                    var fileName = string.Join("_", _settings.ImageCopyright.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
                    var dialog = new SaveFileDialog
                    {
                        DefaultExt = "jpg",
                        Title = "保存当前壁纸",
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

            var launch = new MenuItem("开机启动");
            launch.Checked = _settings.LaunchOnStartup;
            launch.Click += OnStartupLaunch;
            _trayMenu.MenuItems.Add(launch);


            var timerChange = new MenuItem("定时切换");
            timerChange.Checked = _settings.AutoChange;

            var timeRanges = new string[] { "10分钟","30分钟","1小时","2小时","3小时","4小时", "5小时", "6小时", "12小时" };

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

            _trayMenu.MenuItems.Add("退出", (s, e) => Application.Exit());

            _trayIcon = new NotifyIcon();
            _trayIcon.Text = "Bing每日壁纸";
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

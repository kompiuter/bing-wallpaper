using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BingWallpaper
{
    public partial class DeskWidget : Form
    {
        IWallpaperControl wallpaperControl;
        public DeskWidget(IWallpaperControl wallpaperControl)
        {
            this.wallpaperControl = wallpaperControl;
            InitializeComponent();
            this.wallpaperControl.OnWallpaperChange += WallpaperControl_OnWallpaperChange;

            this.btnNext.Click += BtnNext_Click;
            this.btnPre.Click += BtnPre_Click;

            this.lblLocate.Click += LblLocate_Click;
        }

        private void LblLocate_Click(object sender, EventArgs e)
        {
            new WallpaperStoryForm(wallpaperControl.CurrentWallpaper).ShowDialog();
        }

        private void BtnPre_Click(object sender, EventArgs e)
        {
            wallpaperControl.PreWallpaper();
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            wallpaperControl.NextWallpaper();
        }

        private void WallpaperControl_OnWallpaperChange(HistoryImage paper)
        {
            if (wallpaperControl.CurrentWallpaper != null) {
                var locate = wallpaperControl.CurrentWallpaper.Locate.Split(',').ToList().Last();
                this.lblLocate.Text = locate;
            }
        }

        private void DeskWidget_Load(object sender, EventArgs e)
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //this.lblLocate.BackColor = 
                //this.btnNext.BackColor = this.btnPre.BackColor = 
                //Color.FromArgb(50, 50, 50, 50);
            this.lblLocate.Parent = this.btnNext.Parent = this.btnPre.Parent = this.panel1;
            var screenWidth = SystemInformation.WorkingArea.Width;
            var screenHeight = SystemInformation.WorkingArea.Height;
            this.StartPosition = FormStartPosition.Manual; //窗体的位置由Location属性决定
            this.Location = (Point)new Size(screenWidth - 20 - this.Width, screenHeight - this.Height - 50);

            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(this.btnNext, "下一个");
            toolTip1.SetToolTip(this.btnPre, "上一个");
        }
    }
}

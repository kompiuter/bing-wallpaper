namespace BingWallpaper
{
    partial class WallpaperStoryForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WallpaperStoryForm));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblLocation = new System.Windows.Forms.Label();
            this.lblDesc = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnPre = new System.Windows.Forms.PictureBox();
            this.btnNext = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnPre)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNext)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.ErrorImage = global::BingWallpaper.Properties.Resources.error;
            this.pictureBox1.InitialImage = global::BingWallpaper.Properties.Resources.error;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1327, 696);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.lblDate);
            this.panel1.Controls.Add(this.lblLocation);
            this.panel1.Controls.Add(this.lblDesc);
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 507);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1327, 189);
            this.panel1.TabIndex = 1;
            // 
            // lblDate
            // 
            this.lblDate.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblDate.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblDate.Location = new System.Drawing.Point(4, 37);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(136, 31);
            this.lblDate.TabIndex = 3;
            this.lblDate.Text = "2018-08-20";
            // 
            // lblLocation
            // 
            this.lblLocation.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblLocation.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblLocation.Location = new System.Drawing.Point(145, 37);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(960, 31);
            this.lblLocation.TabIndex = 2;
            this.lblLocation.Text = "欧洲, 英国, 凯恩戈姆山国家公园";
            // 
            // lblDesc
            // 
            this.lblDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDesc.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblDesc.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblDesc.Location = new System.Drawing.Point(3, 70);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(1312, 114);
            this.lblDesc.TabIndex = 1;
            this.lblDesc.Text = "凯恩戈姆山国家公园坐落于苏格兰高地中心地区，是英国最大的国家公园。喜欢刺激的人将这里视为天堂，苏格兰的五个滑雪胜地中有三座位于这里，此外这里还有两处水上活动中心、" +
    "进行飞钓和跟踪雄鹿的区域。这里也是景观壮丽的野生区域，内有水獭、獾、红松鼠、鹿以及鹗等。你瞧，那粗壮的树干后面还藏着一个可爱的小家伙——松貂，它无忧无虑的在这里" +
    "生存着！";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblTitle.Location = new System.Drawing.Point(4, 4);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1066, 31);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "凯恩戈姆山国家公园里的一只松貂，苏格兰 (© SCOTLAND: The Big Picture/Minden Pictures)";
            // 
            // btnPre
            // 
            this.btnPre.BackColor = System.Drawing.Color.Transparent;
            this.btnPre.Image = global::BingWallpaper.Properties.Resources.br_prev;
            this.btnPre.Location = new System.Drawing.Point(0, 361);
            this.btnPre.Name = "btnPre";
            this.btnPre.Size = new System.Drawing.Size(49, 53);
            this.btnPre.TabIndex = 2;
            this.btnPre.TabStop = false;
            this.btnPre.Click += new System.EventHandler(this.btnPre_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.BackColor = System.Drawing.Color.Transparent;
            this.btnNext.Image = global::BingWallpaper.Properties.Resources.br_next;
            this.btnNext.Location = new System.Drawing.Point(1283, 361);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(44, 53);
            this.btnNext.TabIndex = 3;
            this.btnNext.TabStop = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // WallpaperStoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1327, 696);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPre);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WallpaperStoryForm";
            this.Text = "WallpaperStoryForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.WallpaperStoryForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnPre)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNext)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.Label lblDesc;
        private System.Windows.Forms.PictureBox btnPre;
        private System.Windows.Forms.PictureBox btnNext;
    }
}
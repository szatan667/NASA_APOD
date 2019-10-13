namespace NASA_APOD
{
    partial class MainWindow
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
                myIcon.Dispose();
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.textURL = new System.Windows.Forms.TextBox();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusBar = new System.Windows.Forms.ToolStripStatusLabel();
            this.checkSaveToDisk = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.checkAutoRefresh = new System.Windows.Forms.CheckBox();
            this.buttonPath = new System.Windows.Forms.Button();
            this.textPath = new System.Windows.Forms.TextBox();
            this.dialogPath = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonPrev = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.buttonCopyLink = new System.Windows.Forms.Button();
            this.buttonCopyImage = new System.Windows.Forms.Button();
            this.myIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.buttonToday = new System.Windows.Forms.Button();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.labelImageDesc = new System.Windows.Forms.Label();
            this.textBoxImgDesc = new System.Windows.Forms.TextBox();
            this.Calendar = new System.Windows.Forms.MonthCalendar();
            this.comboDates = new System.Windows.Forms.ComboBox();
            this.buttonPickDate = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.groupBoxSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(15, 410);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1022, 20);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 0;
            // 
            // textURL
            // 
            this.textURL.Location = new System.Drawing.Point(15, 14);
            this.textURL.Name = "textURL";
            this.textURL.ReadOnly = true;
            this.textURL.Size = new System.Drawing.Size(640, 23);
            this.textURL.TabIndex = 1;
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.Color.Black;
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.ImageLocation = "";
            this.pictureBox.Location = new System.Drawing.Point(15, 44);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(640, 360);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            this.pictureBox.Visible = false;
            this.pictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Paint);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRefresh.Location = new System.Drawing.Point(847, 14);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(87, 25);
            this.buttonRefresh.TabIndex = 3;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 436);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.Size = new System.Drawing.Size(1049, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusBar
            // 
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(58, 17);
            this.statusBar.Text = "status bar";
            // 
            // checkSaveToDisk
            // 
            this.checkSaveToDisk.AutoSize = true;
            this.checkSaveToDisk.Location = new System.Drawing.Point(7, 22);
            this.checkSaveToDisk.Name = "checkSaveToDisk";
            this.checkSaveToDisk.Size = new System.Drawing.Size(129, 19);
            this.checkSaveToDisk.TabIndex = 5;
            this.checkSaveToDisk.Text = "Save images to disk";
            this.checkSaveToDisk.UseVisualStyleBackColor = true;
            this.checkSaveToDisk.CheckedChanged += new System.EventHandler(this.checkSaveToDisk_CheckedChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 15000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // checkAutoRefresh
            // 
            this.checkAutoRefresh.AutoSize = true;
            this.checkAutoRefresh.Checked = true;
            this.checkAutoRefresh.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkAutoRefresh.Location = new System.Drawing.Point(7, 78);
            this.checkAutoRefresh.Name = "checkAutoRefresh";
            this.checkAutoRefresh.Size = new System.Drawing.Size(150, 19);
            this.checkAutoRefresh.TabIndex = 6;
            this.checkAutoRefresh.Text = "Auto refresh every hour";
            this.checkAutoRefresh.UseVisualStyleBackColor = true;
            this.checkAutoRefresh.CheckedChanged += new System.EventHandler(this.checkAutoRefresh_CheckedChanged);
            // 
            // buttonPath
            // 
            this.buttonPath.Enabled = false;
            this.buttonPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPath.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPath.Location = new System.Drawing.Point(7, 48);
            this.buttonPath.Name = "buttonPath";
            this.buttonPath.Size = new System.Drawing.Size(35, 23);
            this.buttonPath.TabIndex = 7;
            this.buttonPath.Text = "...";
            this.buttonPath.UseVisualStyleBackColor = true;
            this.buttonPath.Click += new System.EventHandler(this.buttonPath_Click);
            // 
            // textPath
            // 
            this.textPath.Enabled = false;
            this.textPath.Location = new System.Drawing.Point(48, 48);
            this.textPath.Name = "textPath";
            this.textPath.ReadOnly = true;
            this.textPath.Size = new System.Drawing.Size(322, 23);
            this.textPath.TabIndex = 8;
            // 
            // buttonPrev
            // 
            this.buttonPrev.Enabled = false;
            this.buttonPrev.Location = new System.Drawing.Point(661, 377);
            this.buttonPrev.Name = "buttonPrev";
            this.buttonPrev.Size = new System.Drawing.Size(111, 27);
            this.buttonPrev.TabIndex = 11;
            this.buttonPrev.Text = "<< PREVIOUS";
            this.buttonPrev.UseVisualStyleBackColor = true;
            this.buttonPrev.Click += new System.EventHandler(this.buttonPrev_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.Enabled = false;
            this.buttonNext.Location = new System.Drawing.Point(926, 377);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(111, 27);
            this.buttonNext.TabIndex = 12;
            this.buttonNext.Text = "NEXT >>";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonCopyLink
            // 
            this.buttonCopyLink.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCopyLink.Location = new System.Drawing.Point(661, 14);
            this.buttonCopyLink.Name = "buttonCopyLink";
            this.buttonCopyLink.Size = new System.Drawing.Size(87, 25);
            this.buttonCopyLink.TabIndex = 13;
            this.buttonCopyLink.Text = "Copy link";
            this.buttonCopyLink.UseVisualStyleBackColor = true;
            this.buttonCopyLink.Click += new System.EventHandler(this.buttonCopyLink_Click);
            // 
            // buttonCopyImage
            // 
            this.buttonCopyImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCopyImage.Location = new System.Drawing.Point(754, 14);
            this.buttonCopyImage.Name = "buttonCopyImage";
            this.buttonCopyImage.Size = new System.Drawing.Size(87, 25);
            this.buttonCopyImage.TabIndex = 14;
            this.buttonCopyImage.Text = "Copy image";
            this.buttonCopyImage.UseVisualStyleBackColor = true;
            this.buttonCopyImage.Click += new System.EventHandler(this.buttonCopyImage_Click);
            // 
            // myIcon
            // 
            this.myIcon.Text = "NASA picture of the day";
            this.myIcon.Visible = true;
            this.myIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.myIcon_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // buttonToday
            // 
            this.buttonToday.Enabled = false;
            this.buttonToday.Location = new System.Drawing.Point(794, 377);
            this.buttonToday.Name = "buttonToday";
            this.buttonToday.Size = new System.Drawing.Size(111, 27);
            this.buttonToday.TabIndex = 15;
            this.buttonToday.Text = "TODAY";
            this.buttonToday.UseVisualStyleBackColor = true;
            this.buttonToday.Click += new System.EventHandler(this.buttonToday_Click);
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.checkAutoRefresh);
            this.groupBoxSettings.Controls.Add(this.checkSaveToDisk);
            this.groupBoxSettings.Controls.Add(this.buttonPath);
            this.groupBoxSettings.Controls.Add(this.textPath);
            this.groupBoxSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBoxSettings.Location = new System.Drawing.Point(661, 44);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(376, 104);
            this.groupBoxSettings.TabIndex = 16;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Settings";
            // 
            // labelImageDesc
            // 
            this.labelImageDesc.AutoSize = true;
            this.labelImageDesc.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelImageDesc.Location = new System.Drawing.Point(665, 151);
            this.labelImageDesc.Name = "labelImageDesc";
            this.labelImageDesc.Size = new System.Drawing.Size(53, 15);
            this.labelImageDesc.TabIndex = 17;
            this.labelImageDesc.Text = "imgdesc";
            // 
            // textBoxImgDesc
            // 
            this.textBoxImgDesc.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxImgDesc.Location = new System.Drawing.Point(662, 169);
            this.textBoxImgDesc.Multiline = true;
            this.textBoxImgDesc.Name = "textBoxImgDesc";
            this.textBoxImgDesc.ReadOnly = true;
            this.textBoxImgDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxImgDesc.Size = new System.Drawing.Size(375, 167);
            this.textBoxImgDesc.TabIndex = 18;
            // 
            // Calendar
            // 
            this.Calendar.Location = new System.Drawing.Point(724, 207);
            this.Calendar.Name = "Calendar";
            this.Calendar.TabIndex = 21;
            this.Calendar.Visible = false;
            this.Calendar.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.Calendar_DateSelected);
            // 
            // comboDates
            // 
            this.comboDates.FormattingEnabled = true;
            this.comboDates.Location = new System.Drawing.Point(724, 344);
            this.comboDates.Name = "comboDates";
            this.comboDates.Size = new System.Drawing.Size(313, 23);
            this.comboDates.TabIndex = 22;
            this.comboDates.SelectedIndexChanged += new System.EventHandler(this.comboDates_SelectedIndexChanged);
            // 
            // buttonPickDate
            // 
            this.buttonPickDate.Location = new System.Drawing.Point(662, 342);
            this.buttonPickDate.Name = "buttonPickDate";
            this.buttonPickDate.Size = new System.Drawing.Size(56, 27);
            this.buttonPickDate.TabIndex = 23;
            this.buttonPickDate.Text = "Date...";
            this.buttonPickDate.UseVisualStyleBackColor = true;
            this.buttonPickDate.Click += new System.EventHandler(this.buttonPickDate_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1049, 458);
            this.Controls.Add(this.buttonPickDate);
            this.Controls.Add(this.comboDates);
            this.Controls.Add(this.labelImageDesc);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.buttonToday);
            this.Controls.Add(this.buttonCopyImage);
            this.Controls.Add(this.buttonCopyLink);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.buttonPrev);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.textURL);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.textBoxImgDesc);
            this.Controls.Add(this.Calendar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NASA Astronomy Picture of the Day";
            this.Resize += new System.EventHandler(this.windowResize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox textURL;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusBar;
        private System.Windows.Forms.CheckBox checkSaveToDisk;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox checkAutoRefresh;
        private System.Windows.Forms.Button buttonPath;
        private System.Windows.Forms.TextBox textPath;
        private System.Windows.Forms.FolderBrowserDialog dialogPath;
        private System.Windows.Forms.Button buttonPrev;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Button buttonCopyLink;
        private System.Windows.Forms.Button buttonCopyImage;
        private System.Windows.Forms.NotifyIcon myIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button buttonToday;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.Label labelImageDesc;
        private System.Windows.Forms.TextBox textBoxImgDesc;
        private System.Windows.Forms.MonthCalendar Calendar;
        private System.Windows.Forms.ComboBox comboDates;
        private System.Windows.Forms.Button buttonPickDate;
    }
}


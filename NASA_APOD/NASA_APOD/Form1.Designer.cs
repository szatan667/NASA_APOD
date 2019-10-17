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
            this.textDate = new System.Windows.Forms.TextBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabImage = new System.Windows.Forms.TabPage();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.textCustomKey = new System.Windows.Forms.TextBox();
            this.linkHowToKey = new System.Windows.Forms.LinkLabel();
            this.checkCustomKey = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.groupBoxSettings.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabImage.SuspendLayout();
            this.tabSettings.SuspendLayout();
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
            this.statusStrip1.Size = new System.Drawing.Size(1046, 22);
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
            this.checkSaveToDisk.Location = new System.Drawing.Point(6, 22);
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
            this.timer1.Interval = 3600000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // checkAutoRefresh
            // 
            this.checkAutoRefresh.AutoSize = true;
            this.checkAutoRefresh.Checked = true;
            this.checkAutoRefresh.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkAutoRefresh.Location = new System.Drawing.Point(6, 76);
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
            this.buttonPath.Location = new System.Drawing.Point(6, 47);
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
            this.textPath.Location = new System.Drawing.Point(47, 47);
            this.textPath.Name = "textPath";
            this.textPath.ReadOnly = true;
            this.textPath.Size = new System.Drawing.Size(303, 23);
            this.textPath.TabIndex = 8;
            // 
            // buttonPrev
            // 
            this.buttonPrev.Enabled = false;
            this.buttonPrev.Location = new System.Drawing.Point(6, 298);
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
            this.buttonNext.Location = new System.Drawing.Point(240, 298);
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
            this.buttonToday.Location = new System.Drawing.Point(123, 298);
            this.buttonToday.Name = "buttonToday";
            this.buttonToday.Size = new System.Drawing.Size(111, 27);
            this.buttonToday.TabIndex = 15;
            this.buttonToday.Text = "TODAY";
            this.buttonToday.UseVisualStyleBackColor = true;
            this.buttonToday.Click += new System.EventHandler(this.buttonToday_Click);
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.checkCustomKey);
            this.groupBoxSettings.Controls.Add(this.textCustomKey);
            this.groupBoxSettings.Controls.Add(this.linkHowToKey);
            this.groupBoxSettings.Controls.Add(this.checkAutoRefresh);
            this.groupBoxSettings.Controls.Add(this.checkSaveToDisk);
            this.groupBoxSettings.Controls.Add(this.buttonPath);
            this.groupBoxSettings.Controls.Add(this.textPath);
            this.groupBoxSettings.Enabled = false;
            this.groupBoxSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBoxSettings.Location = new System.Drawing.Point(6, 6);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(356, 319);
            this.groupBoxSettings.TabIndex = 16;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Settings";
            // 
            // labelImageDesc
            // 
            this.labelImageDesc.AutoSize = true;
            this.labelImageDesc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelImageDesc.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelImageDesc.Location = new System.Drawing.Point(6, 3);
            this.labelImageDesc.MaximumSize = new System.Drawing.Size(356, 32);
            this.labelImageDesc.MinimumSize = new System.Drawing.Size(356, 32);
            this.labelImageDesc.Name = "labelImageDesc";
            this.labelImageDesc.Size = new System.Drawing.Size(356, 32);
            this.labelImageDesc.TabIndex = 17;
            this.labelImageDesc.Text = "imgdesc\r\ncpy";
            this.labelImageDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxImgDesc
            // 
            this.textBoxImgDesc.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxImgDesc.Location = new System.Drawing.Point(6, 38);
            this.textBoxImgDesc.Multiline = true;
            this.textBoxImgDesc.Name = "textBoxImgDesc";
            this.textBoxImgDesc.ReadOnly = true;
            this.textBoxImgDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxImgDesc.Size = new System.Drawing.Size(356, 221);
            this.textBoxImgDesc.TabIndex = 18;
            this.textBoxImgDesc.MouseHover += new System.EventHandler(this.TextBoxImgDesc_MouseHover);
            // 
            // Calendar
            // 
            this.Calendar.Location = new System.Drawing.Point(68, 128);
            this.Calendar.Name = "Calendar";
            this.Calendar.TabIndex = 21;
            this.Calendar.Visible = false;
            this.Calendar.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.Calendar_DateSelected);
            // 
            // comboDates
            // 
            this.comboDates.FormattingEnabled = true;
            this.comboDates.Location = new System.Drawing.Point(15, 410);
            this.comboDates.Name = "comboDates";
            this.comboDates.Size = new System.Drawing.Size(313, 23);
            this.comboDates.TabIndex = 22;
            this.comboDates.Visible = false;
            this.comboDates.SelectedIndexChanged += new System.EventHandler(this.comboDates_SelectedIndexChanged);
            // 
            // buttonPickDate
            // 
            this.buttonPickDate.Location = new System.Drawing.Point(6, 265);
            this.buttonPickDate.Name = "buttonPickDate";
            this.buttonPickDate.Size = new System.Drawing.Size(56, 27);
            this.buttonPickDate.TabIndex = 23;
            this.buttonPickDate.Text = "Date...";
            this.buttonPickDate.UseVisualStyleBackColor = true;
            this.buttonPickDate.Click += new System.EventHandler(this.buttonPickDate_Click);
            // 
            // textDate
            // 
            this.textDate.BackColor = System.Drawing.SystemColors.Control;
            this.textDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textDate.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textDate.Location = new System.Drawing.Point(68, 267);
            this.textDate.Name = "textDate";
            this.textDate.ReadOnly = true;
            this.textDate.Size = new System.Drawing.Size(94, 23);
            this.textDate.TabIndex = 24;
            this.textDate.Text = "dupa";
            this.textDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabImage);
            this.tabControl.Controls.Add(this.tabSettings);
            this.tabControl.Location = new System.Drawing.Point(661, 45);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(376, 359);
            this.tabControl.TabIndex = 25;
            // 
            // tabImage
            // 
            this.tabImage.Controls.Add(this.labelImageDesc);
            this.tabImage.Controls.Add(this.textDate);
            this.tabImage.Controls.Add(this.textBoxImgDesc);
            this.tabImage.Controls.Add(this.buttonPickDate);
            this.tabImage.Controls.Add(this.buttonPrev);
            this.tabImage.Controls.Add(this.buttonNext);
            this.tabImage.Controls.Add(this.buttonToday);
            this.tabImage.Controls.Add(this.Calendar);
            this.tabImage.Location = new System.Drawing.Point(4, 24);
            this.tabImage.Name = "tabImage";
            this.tabImage.Padding = new System.Windows.Forms.Padding(3);
            this.tabImage.Size = new System.Drawing.Size(368, 331);
            this.tabImage.TabIndex = 0;
            this.tabImage.Text = "Image of the day";
            this.tabImage.UseVisualStyleBackColor = true;
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.groupBoxSettings);
            this.tabSettings.Location = new System.Drawing.Point(4, 24);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabSettings.Size = new System.Drawing.Size(368, 331);
            this.tabSettings.TabIndex = 1;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // textCustomKey
            // 
            this.textCustomKey.Location = new System.Drawing.Point(6, 126);
            this.textCustomKey.MaxLength = 40;
            this.textCustomKey.Name = "textCustomKey";
            this.textCustomKey.Size = new System.Drawing.Size(344, 23);
            this.textCustomKey.TabIndex = 10;
            this.textCustomKey.TextChanged += new System.EventHandler(this.textCustomKey_TextChanged);
            // 
            // linkHowToKey
            // 
            this.linkHowToKey.AutoSize = true;
            this.linkHowToKey.Location = new System.Drawing.Point(6, 152);
            this.linkHowToKey.Name = "linkHowToKey";
            this.linkHowToKey.Size = new System.Drawing.Size(147, 15);
            this.linkHowToKey.TabIndex = 11;
            this.linkHowToKey.TabStop = true;
            this.linkHowToKey.Text = "How do I get my own key?";
            this.linkHowToKey.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkHowToKey_LinkClicked);
            // 
            // checkCustomKey
            // 
            this.checkCustomKey.AutoSize = true;
            this.checkCustomKey.Location = new System.Drawing.Point(6, 101);
            this.checkCustomKey.Name = "checkCustomKey";
            this.checkCustomKey.Size = new System.Drawing.Size(130, 19);
            this.checkCustomKey.TabIndex = 9;
            this.checkCustomKey.Text = "Use custom API key";
            this.checkCustomKey.UseVisualStyleBackColor = true;
            this.checkCustomKey.CheckedChanged += new System.EventHandler(this.checkCustomKey_CheckedChanged);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1046, 458);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.comboDates);
            this.Controls.Add(this.buttonCopyImage);
            this.Controls.Add(this.buttonCopyLink);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.textURL);
            this.Controls.Add(this.progressBar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NASA Astronomy Picture of the Day";
            this.Resize += new System.EventHandler(this.windowResize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabImage.ResumeLayout(false);
            this.tabImage.PerformLayout();
            this.tabSettings.ResumeLayout(false);
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
        private System.Windows.Forms.TextBox textDate;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabImage;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.CheckBox checkCustomKey;
        private System.Windows.Forms.TextBox textCustomKey;
        private System.Windows.Forms.LinkLabel linkHowToKey;
    }
}
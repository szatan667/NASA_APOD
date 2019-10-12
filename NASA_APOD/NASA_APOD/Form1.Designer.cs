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
            this.checkDefaultURL = new System.Windows.Forms.CheckBox();
            this.textDefaultURL = new System.Windows.Forms.TextBox();
            this.buttonPrev = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.buttonCopyLink = new System.Windows.Forms.Button();
            this.buttonCopyImage = new System.Windows.Forms.Button();
            this.myIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.buttonToday = new System.Windows.Forms.Button();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxImgDesc = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.groupBoxSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(13, 369);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(775, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 0;
            // 
            // textURL
            // 
            this.textURL.Location = new System.Drawing.Point(13, 12);
            this.textURL.Name = "textURL";
            this.textURL.ReadOnly = true;
            this.textURL.Size = new System.Drawing.Size(532, 20);
            this.textURL.TabIndex = 1;
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.Color.Black;
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Location = new System.Drawing.Point(13, 38);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(442, 325);
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            this.pictureBox.Visible = false;
            this.pictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Paint);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRefresh.Location = new System.Drawing.Point(713, 10);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(75, 23);
            this.buttonRefresh.TabIndex = 3;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 401);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
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
            this.checkSaveToDisk.Location = new System.Drawing.Point(6, 19);
            this.checkSaveToDisk.Name = "checkSaveToDisk";
            this.checkSaveToDisk.Size = new System.Drawing.Size(121, 17);
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
            this.checkAutoRefresh.Location = new System.Drawing.Point(6, 68);
            this.checkAutoRefresh.Name = "checkAutoRefresh";
            this.checkAutoRefresh.Size = new System.Drawing.Size(136, 17);
            this.checkAutoRefresh.TabIndex = 6;
            this.checkAutoRefresh.Text = "Auto refresh every hour";
            this.checkAutoRefresh.UseVisualStyleBackColor = true;
            this.checkAutoRefresh.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // buttonPath
            // 
            this.buttonPath.Enabled = false;
            this.buttonPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPath.Location = new System.Drawing.Point(6, 42);
            this.buttonPath.Name = "buttonPath";
            this.buttonPath.Size = new System.Drawing.Size(29, 20);
            this.buttonPath.TabIndex = 7;
            this.buttonPath.Text = "...";
            this.buttonPath.UseVisualStyleBackColor = true;
            this.buttonPath.Click += new System.EventHandler(this.buttonPath_Click);
            // 
            // textPath
            // 
            this.textPath.Enabled = false;
            this.textPath.Location = new System.Drawing.Point(41, 42);
            this.textPath.Name = "textPath";
            this.textPath.ReadOnly = true;
            this.textPath.Size = new System.Drawing.Size(279, 20);
            this.textPath.TabIndex = 8;
            // 
            // checkDefaultURL
            // 
            this.checkDefaultURL.AutoSize = true;
            this.checkDefaultURL.Location = new System.Drawing.Point(171, 68);
            this.checkDefaultURL.Name = "checkDefaultURL";
            this.checkDefaultURL.Size = new System.Drawing.Size(149, 17);
            this.checkDefaultURL.TabIndex = 9;
            this.checkDefaultURL.Text = "Change default URL base";
            this.checkDefaultURL.UseVisualStyleBackColor = true;
            this.checkDefaultURL.Visible = false;
            this.checkDefaultURL.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // textDefaultURL
            // 
            this.textDefaultURL.Enabled = false;
            this.textDefaultURL.Location = new System.Drawing.Point(462, 314);
            this.textDefaultURL.Name = "textDefaultURL";
            this.textDefaultURL.Size = new System.Drawing.Size(327, 20);
            this.textDefaultURL.TabIndex = 10;
            this.textDefaultURL.Text = "https://apod.nasa.gov/apod/astropix.html";
            // 
            // buttonPrev
            // 
            this.buttonPrev.Enabled = false;
            this.buttonPrev.Location = new System.Drawing.Point(461, 340);
            this.buttonPrev.Name = "buttonPrev";
            this.buttonPrev.Size = new System.Drawing.Size(110, 23);
            this.buttonPrev.TabIndex = 11;
            this.buttonPrev.Text = "<< PREVIOUS";
            this.buttonPrev.UseVisualStyleBackColor = true;
            this.buttonPrev.Click += new System.EventHandler(this.buttonPrev_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.Enabled = false;
            this.buttonNext.Location = new System.Drawing.Point(678, 340);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(110, 23);
            this.buttonNext.TabIndex = 12;
            this.buttonNext.Text = "NEXT >>";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonCopyLink
            // 
            this.buttonCopyLink.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCopyLink.Location = new System.Drawing.Point(632, 10);
            this.buttonCopyLink.Name = "buttonCopyLink";
            this.buttonCopyLink.Size = new System.Drawing.Size(75, 23);
            this.buttonCopyLink.TabIndex = 13;
            this.buttonCopyLink.Text = "Copy link";
            this.buttonCopyLink.UseVisualStyleBackColor = true;
            this.buttonCopyLink.Click += new System.EventHandler(this.buttonCopyLink_Click);
            // 
            // buttonCopyImage
            // 
            this.buttonCopyImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCopyImage.Location = new System.Drawing.Point(551, 10);
            this.buttonCopyImage.Name = "buttonCopyImage";
            this.buttonCopyImage.Size = new System.Drawing.Size(75, 23);
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
            this.buttonToday.Location = new System.Drawing.Point(577, 340);
            this.buttonToday.Name = "buttonToday";
            this.buttonToday.Size = new System.Drawing.Size(95, 23);
            this.buttonToday.TabIndex = 15;
            this.buttonToday.Text = "TODAY";
            this.buttonToday.UseVisualStyleBackColor = true;
            this.buttonToday.Click += new System.EventHandler(this.buttonToday_Click);
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.checkDefaultURL);
            this.groupBoxSettings.Controls.Add(this.checkAutoRefresh);
            this.groupBoxSettings.Controls.Add(this.checkSaveToDisk);
            this.groupBoxSettings.Controls.Add(this.buttonPath);
            this.groupBoxSettings.Controls.Add(this.textPath);
            this.groupBoxSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBoxSettings.Location = new System.Drawing.Point(461, 39);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(326, 99);
            this.groupBoxSettings.TabIndex = 16;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Settings";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(467, 145);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 17;
            // 
            // textBoxImgDesc
            // 
            this.textBoxImgDesc.Location = new System.Drawing.Point(462, 145);
            this.textBoxImgDesc.Multiline = true;
            this.textBoxImgDesc.Name = "textBoxImgDesc";
            this.textBoxImgDesc.ReadOnly = true;
            this.textBoxImgDesc.Size = new System.Drawing.Size(325, 163);
            this.textBoxImgDesc.TabIndex = 18;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 423);
            this.Controls.Add(this.textBoxImgDesc);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.buttonToday);
            this.Controls.Add(this.buttonCopyImage);
            this.Controls.Add(this.buttonCopyLink);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.buttonPrev);
            this.Controls.Add(this.textDefaultURL);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.textURL);
            this.Controls.Add(this.progressBar);
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
        private System.Windows.Forms.CheckBox checkDefaultURL;
        private System.Windows.Forms.TextBox textDefaultURL;
        private System.Windows.Forms.Button buttonPrev;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Button buttonCopyLink;
        private System.Windows.Forms.Button buttonCopyImage;
        private System.Windows.Forms.NotifyIcon myIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button buttonToday;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxImgDesc;
    }
}


using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace NASA_APOD
{
    public partial class MainWindow : Form
    {
        //Main photo of the day object
        public APOD apod;

        //Paths
        public string pathToSave; //will hold path from folder dialog
        public string apodPath = "temp.jpg"; //default file to save

        //Interface items
        ContextMenu myIconMenu; //context menu for tray icon
        bool hidden = false;    //main form state - hidden or not

        //setup ini file to store usage statistics
        IniFile iniFile = new IniFile();

        //Wallpapering
        [DllImport("kernel32.dll")]
        static extern uint GetLastError();
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_SENDCHANGE = 0x2;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        //Logging switch - disabled on startup, requires cmd line param to be enabled
        bool logging = false;

        //--- Main class methods -----------------------------------------------------------------

        //Default constructor - will create program window and set everything up
        public MainWindow()
        {
            //Enable logging if required - just pass anything as pgm parameter
            if (Environment.GetCommandLineArgs().Length > 1)
                logging = true;

            Log("\n--- PROGRAM START -----------------------------------------------");
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
            Log("initializing...");
            InitializeComponent();
            Log("DONE!");

            //read anything from INI file to check if it was already created
            //if not, write initial zero number of runs
            Log("Ini file first read");
            var lastDate = iniFile.Read("lastDate");

            if (lastDate == String.Empty) //write default values to INI file
            {
                Log("INI file empty, fill in");
                iniFile.Write("lastDate", DateTime.Today.ToString());
                iniFile.Write("autoRefresh", checkAutoRefresh.Checked.ToString());
                iniFile.Write("saveToDisk", checkSaveToDisk.Checked.ToString());
                iniFile.Write("pathToSave", String.Empty);
                iniFile.Write("useCustomKey", "False");
                iniFile.Write("customKey", String.Empty);
                iniFile.Write("enableHistory", String.Empty);
            }

            //GUI items
            Log("setting up GUI items...");

            if (!logging)
                tabDebug.Dispose();

            LinkLabel.Link lnk = new LinkLabel.Link();
            lnk.LinkData = "https://api.nasa.gov/#signUp";
            linkHowToKey.Links.Add(lnk);
            foreach (Control ctl in tabSettings.Controls)
            {
                ctl.Enabled = true;
            }
            labelImageDesc.Text = String.Empty;
            textDate.Text = String.Empty;
            myIconMenu = new ContextMenu();
            myIconMenu.MenuItems.Add("Previous", buttonPrev_Click);
            myIconMenu.MenuItems.Add("Next", buttonNext_Click);
            myIconMenu.MenuItems.Add("Today", OnMenuToday);
            myIconMenu.MenuItems.Add("Exit", OnMenuExit);
            myIcon.Icon = Properties.Resources.NASA;
            this.Icon = Properties.Resources.NASA;
            myIcon.ContextMenu = myIconMenu;
            pathToSave = iniFile.Read("pathToSave");
            textPath.Text = pathToSave;
            if (iniFile.Read("saveToDIsk") == "True")
                checkSaveToDisk.Checked = true;
            else
                checkSaveToDisk.Checked = false;
            if (iniFile.Read("autoRefresh") == "True")
                checkAutoRefresh.Checked = true;
            else
                checkAutoRefresh.Checked = false;
            if (iniFile.Read("useCustomKey") == "True")
            {
                checkCustomKey.Checked = true;
                textCustomKey.Enabled = true;
            }
            else
            {
                checkCustomKey.Checked = false;
                textCustomKey.Enabled = false;
            }
            textCustomKey.Text = iniFile.Read("customKey");
            listHistory.Columns[0].Text = "Date";
            listHistory.Columns[1].Text = "Title";
            if (iniFile.Read("enableHistory") == "True")
            {
                checkEnableHistory.Checked = true;
                tabHistory.Show();
            }
            else
            {
                checkEnableHistory.Checked = false;
                tabHistory.Hide();
            }
            Log("DONE!");

            //Subscribe for events
            //update progress bar and mostly important - process downloaded image
            pictureBox.LoadProgressChanged += PictureBox_LoadProgressChanged;
            pictureBox.LoadCompleted += PictureBox_LoadCompleted;

            //Create 'photo of the day' object and for starters set API date to today
            //at this point we should have custom key read from settings
            //if not, default one will be used
            Log("Create main apod object");
            apod = new APOD();
            apod.setAPIKey(textCustomKey.Text);
            Log("setting date - STARTUP...");
            //If auto-refresh option is set, always use today's date
            if (checkAutoRefresh.Checked)
            {
                setAPIDate(apod, DateTime.Today);
            }
            else
            {
                setAPIDate(apod, DateTime.Parse(iniFile.Read("lastDate")));
            }
            Log("STARTUP DATE = " + apod.apiDate);

            //Get the image on startup, but only if API call was succesful
            //(eg. prevent networking errors)
            Log("Getting the image on startup...");
            if (apod.media_type != null)
                getNASAApod();
        }

        //Logging to file
        public void Log(string msg)
        {
            if (logging)
                try
                {
                    string fn = "apod.log";
                    using (TextWriter tw = new StreamWriter(fn, true))
                        tw.WriteLine(DateTime.Now.ToString() + " - " + msg);
                }
                catch (Exception ex)
                {
                    Trace.Write(ex.ToString());
                }
        }

        //Logging to tab
        public void DebugTab(APOD apod)
        {
            if (logging)
            {
                //nothing fancy, use hard-coded list items to display API call response
                listDebug.Items[0].SubItems[1].Text = apod.copyright;
                listDebug.Items[1].SubItems[1].Text = apod.date;
                listDebug.Items[2].SubItems[1].Text = apod.explanation;
                listDebug.Items[3].SubItems[1].Text = apod.hdurl;
                listDebug.Items[4].SubItems[1].Text = apod.media_type;
                listDebug.Items[5].SubItems[1].Text = apod.service_version;
                listDebug.Items[6].SubItems[1].Text = apod.title;
                listDebug.Items[7].SubItems[1].Text = apod.url;
                listDebug.Columns[0].Width = 75;
                listDebug.Columns[1].Width = 1350;
            }
        }

        //Fill in history tab
        private void fillHistory()
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            APOD _apod = new APOD(); //local APOD just to get img title list
            _apod.setAPIKey(textCustomKey.Text);

            //Column headers and rest of GUI
            listHistory.Items.Clear();
            statusBar.Text = "Getting history items... (0/0)";
            Application.DoEvents();

            //Some history setup
            byte maxDays = 8; //how many history items
            byte cnt = 0; //items loaded
            int daysback = 0; //count days bacwards

            //Now go back in time and fetch history items
            while (cnt < maxDays)
            {
                _apod.setAPIDate(apod.apiDate.AddDays(-daysback)); //go back further back in time
                if (_apod.media_type == "image") //add item only if media is image
                {
                    listHistory.Items.Add(_apod.apiDate.ToShortDateString());
                    listHistory.Items[cnt].SubItems.Add(_apod.title);
                    cnt++;
                    progressBar.Value = 100 * cnt / maxDays;
                    statusBar.Text = "Getting history items... (" + cnt + "/" + maxDays + ")";
                    Application.DoEvents();
                    Log("History item added " + _apod.date);
                }
                daysback++;
            }
            _apod.Dispose(); //destroy local APOD
            listHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            statusBar.Text = String.Empty;
        }

        //Set current date for API - create URL and get json
        private void setAPIDate(APOD apod, DateTime datetime)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
            Log("trying this date: " + datetime);

            //Clear date related GUI fields
            textDate.ForeColor = Color.LightSlateGray;
            textDate.Text = "Wait...";
            Log("date field \"wait\" and grey");

            try
            {
                Log("calling apod.setAPIDate()...");
                apod.setAPIDate(datetime);
                //←→►◄
                buttonPrev.Text = "<< " + apod.apiDate.AddDays(-1).ToShortDateString();
                buttonNext.Text = apod.apiDate.AddDays(1).ToShortDateString() + ">>";

                Log("Call succesful! apod.date = " + apod.apiDate);
                DebugTab(apod);

                if (checkEnableHistory.Checked)
                    fillHistory();
                else
                    listHistory.Items.Clear();
            }
            catch (Exception e)
            {
                Log("API SET DATE FAILED! requested date = " + datetime.ToString());
                Log(e.Message);
                statusBar.Text = e.Message;
                apod.media_type = null;
                textDate.ForeColor = Color.Black;
                textDate.Text = "(none)";
                //setupButtons();
            }
        }

        //Context menu "today" event handler
        private void OnMenuToday(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            //force getting today's image
            apod.setAPIDate(DateTime.Today);
            if (apod.media_type != null)
                getNASAApod();
        }

        //Tray icon menu "exit" event handler
        private void OnMenuExit(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            Application.Exit();
        }

        //App exit event?
        private void OnAppExit(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
            Log("--- THE END! ------------------------------------------------");
        }

        //Get the picture and possibly save it to disk (if required in GUI)
        private void getNASAApod()
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            Log("setting some gui items...");
            statusBar.Text = "Getting NASA picture of the day...";
            myIcon.Text = statusBar.Text;
            buttonPrev.Enabled = false;
            buttonToday.Enabled = false;
            buttonNext.Enabled = false;
            buttonRefresh.Enabled = false;
            labelImageDesc.Text = String.Empty;
            textBoxImgDesc.Text = String.Empty;
            Log("done!");

            //Download image to picture box
            Log("trying to download the image...");
            try
            {
                //Get it from apod url, only if it's an image
                //(sometimes they post videos)
                if (apod.media_type == "image")
                {
                    //Download the image directly to image box
                    //"Image" property will allow to save it later
                    //(try normal-res picture by default)
                    //(hd images are large and use too much memory)
                    Log("API says that image is there: " + apod.hdurl);
                    pictureBox.LoadAsync(apod.hdurl);
                    Log("async download started");
                }
                else
                {
                    Log("not a picture section - either a video link or null");
                    //Not a picture today, clear the image and show some text
                    pictureBox.Image = null;
                    statusBar.Text = "Sorry, no picture today.";
                    textBoxImgDesc.Text = "(media_type = " + apod.media_type + ")";
                    textDate.Text = String.Empty;
                    myIcon.Text = statusBar.Text;
                    myIcon.BalloonTipText = statusBar.Text;
                    myIcon.ShowBalloonTip(5000);
                    Log("\"no image\" gui setup done");
                    //Enable/disable 'previous' and 'next' buttons, depending on the API date
                    setupButtons();
                }
            }
            //Download errors
            catch (Exception e)
            {
                Log("error while downloading image!");
                Log(e.Message);
                statusBar.Text = e.Message;
            }
        }

        //Download progress bar - event handler
        private void PictureBox_LoadProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            //Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            progressBar.Value = e.ProgressPercentage;
            statusBar.Text = "Getting NASA picture of the day... " + e.ProgressPercentage + "%";
        }

        //Download completed event - do rest of the logic - actual wallpapering and saving to disk
        private void PictureBox_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            WebClient _wc = new WebClient(); //to download hi-res image

            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Visible = true;

            //Save the image to disk - required to set the wallpaper
            //even if "save to disk" is not set in GUI
            saveToDisk();

            //Before setting the wallpaper, we have to build full path of TEMP.JPG
            //but only if not saving to custom path, because then it was already built
            if (!checkSaveToDisk.Checked)
                apodPath = System.Reflection.Assembly.GetEntryAssembly().Location.Substring(0,
                           System.Reflection.Assembly.GetEntryAssembly().Location.LastIndexOf("\\") + 1)
                         + "temp.jpg";

            //do actual wallpapering
            //SystemParametersInfo(SPI_SETDESKWALLPAPER,
            //    1,
            //    apodPath,
            //    SPIF_SENDCHANGE);
            Log("Wallpapering...");
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            Log("registry file BEFORE = " + key.GetValue("Wallpaper"));
            key.SetValue(@"WallpaperStyle", 6.ToString()); //always fit image to screen (zoom mode)
            key.SetValue(@"TileWallpaper", 0.ToString()); //do not tile
            Log("registry set...");

            int _out = SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                apodPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);

            Log("Wallpapering result = " + _out);
            Log("error msg = " + GetLastError());
            key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            Log("registry file AFTER = " + key.GetValue("Wallpaper"));

            //save last image date
            iniFile.Write("lastDate", apod.apiDate.ToString());

            //setup UI elements
            //Enable/disable 'previous' and 'next' buttons, depending on the API date
            setupButtons();
            myIcon.Text = apod.title;
            labelImageDesc.Text = apod.title;
            if (apod.copyright != null && apod.copyright != String.Empty)
                labelImageDesc.Text += "\n© " + apod.copyright.Replace("\n", " ");
            labelImageDesc.Width = 375;
            myIcon.BalloonTipTitle = "NASA Astronomy Picture of the Day";
            myIcon.BalloonTipText = apod.title;
            myIcon.ShowBalloonTip(5000);
            textBoxImgDesc.Text = apod.explanation;
            textURL.Text = apod.hdurl;
            if (_out == 0)
                statusBar.Text = "Downloaded but not set :(";
            else
                statusBar.Text = "Done!";
            this.Text = "NASA Astronomy Picture of the Day - " +
                apod.apiDate.ToShortDateString() + " - " +
                apod.title;
            textDate.ForeColor = Color.Black;
            textDate.Text = apod.apiDate.ToShortDateString();

            //Invalidate picture box to force redrawing, just in case
            pictureBox.Invalidate();
        }

        //Enable or disable prev/today/buttons depending on current API date
        private void setupButtons()
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            //Enable/disable 'previous' and 'next' buttons, depending on the API date
            //TODAY - previous enabled, today enabled, next disabled
            if (apod.apiDate == DateTime.Today)
            {
                buttonPrev.Enabled = true;
                buttonToday.Enabled = false;
                buttonNext.Text = "NEXT >>"; //←→►◄
                buttonNext.Enabled = false;
                buttonRefresh.Enabled = true;
            }
            else
            //EARLIER - all enabled
            {
                buttonPrev.Enabled = true;
                buttonToday.Enabled = true;
                buttonNext.Enabled = true;
                buttonRefresh.Enabled = true;
            }
        }

        //Save current image to disk
        private void saveToDisk()
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            //First build custom path if desired
            if ((pathToSave != null && pathToSave != String.Empty) &&
                (apod.hdurl != null && apod.hdurl != String.Empty)) //custom path found, concatenate path with image filename
            {
                int begin = apod.hdurl.LastIndexOf('/') + 1;
                int len = apod.hdurl.Length - begin;
                apodPath = pathToSave + "\\" + //folder path
                    apod.apiDate.ToShortDateString().Replace(' ', '_') + '_' + //inject date string at the begining of filename
                    apod.hdurl.Substring(begin, len); //filename from URL
            }

            //Do actual save
            try
            {
                pictureBox.Image.Save(apodPath);
            }
            catch (Exception e)
            {
                Log(e.Message);
                statusBar.Text = e.Message;
            }
        }

        //Timer event handler - reload image with today date
        private void timer1_Tick(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
            Log("--- TIMER TICK! ---");

            if (DateTime.Today != apod.apiDate)
            {
                Log("AUTO REFRESH - date rolled over");
                Log("API date = " + apod.apiDate);
                Log("TODAy date = " + DateTime.Today);
                setAPIDate(apod, DateTime.Today);
                if (apod.media_type != null)
                    getNASAApod();
            }
            else
             {
                Log("Date not changed, nothing to do in auto-refresh end");
            }
        }

        //Auto refresh checkbox - enable or disable automatic refresh
        private void checkAutoRefresh_CheckedChanged(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            if (checkAutoRefresh.Checked)
                timer1.Enabled = true;
            else
                timer1.Enabled = false;

            iniFile.Write("autoRefresh", checkAutoRefresh.Checked.ToString());
        }

        //Save to custom path checkbox
        private void checkSaveToDisk_CheckedChanged(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            if (checkSaveToDisk.Checked) //custom path checked
            {
                buttonPath.Enabled = true; //enable path selection button [...]
                textPath.Enabled = true;   //enable custom path text box
                if (textPath.Text == null || textPath.Text == String.Empty) //first click, no custom path saved
                {
                    buttonPath_Click(this, e); //invoke path selection dialog
                    if (dialogPath.SelectedPath == String.Empty) //if dialog cancelled, revert UI changes
                    {
                        buttonPath.Enabled = false;
                        textPath.Enabled = false;
                        checkSaveToDisk.Checked = false;
                    }
                }
                else
                    pathToSave = textPath.Text;
            }
            else //disable custom path
            {
                buttonPath.Enabled = false;
                textPath.Enabled = false;
                pathToSave = null;
                apodPath = "temp.jpg";
            }

            iniFile.Write("saveToDisk", checkSaveToDisk.Checked.ToString());
            iniFile.Write("pathToSave", pathToSave);
        }

        //Custom path selection button
        private void buttonPath_Click(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            dialogPath.ShowDialog(); //display path selection dialog

            if (dialogPath.SelectedPath != String.Empty)
            {
                pathToSave = dialogPath.SelectedPath; //save the path
                textPath.Text = pathToSave; //and display it in path text box
                iniFile.Write("saveToDisk", checkSaveToDisk.Checked.ToString());
                iniFile.Write("pathToSave", pathToSave);
            }
        }

        //Copy link to clipboard
        private void buttonCopyLink_Click(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            if (textURL.Text != String.Empty) Clipboard.SetText(textURL.Text);
        }

        //Copy image to clipboard
        private void buttonCopyImage_Click(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            if (pictureBox.Image != null) Clipboard.SetImage(pictureBox.Image);
        }

        //Tray icon click - hide/show window
        private void myIcon_MouseClick(object sender, MouseEventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            if (hidden)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                hidden = false;
            }
            else
            {
                this.Hide();
                hidden = true;
            }
        }

        //Minimize to system tray
        private void windowResize(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                hidden = true;
                myIcon.BalloonTipTitle = "NASA Astronomy Picture of the Day";
                myIcon.BalloonTipText = myIcon.Text;
                myIcon.ShowBalloonTip(1000);
            }
        }

        //Previous button click
        private void buttonPrev_Click(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            setAPIDate(apod, apod.apiDate.AddDays(-1));
            getNASAApod();
        }

        //Next button click
        private void buttonNext_Click(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            //TODO: handle today date - should be prohibited to click next if date = today
            setAPIDate(apod, apod.apiDate.AddDays(1));
            getNASAApod();
        }

        //Today button click
        private void buttonToday_Click(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            setAPIDate(apod, DateTime.Today);
            if (apod.media_type != null)
                getNASAApod();
        }

        //Refresh button - simply reload current image
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            setAPIDate(apod, apod.apiDate); //refresh json strings with current setup
            if (apod.media_type != null)
                getNASAApod();
        }

        //EXPERIMANTAL - draw the title over the picture
        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            /*
            //get picture box size
            Size picSize = pictureBox.Size;

            //find maximum font size for current picture box size
            //(it will be ususally pic box width that determines max size)

            //initial font - consolas bold, 1pt
            int fs = 12;
            Font myFont = new Font("Consolas", fs, FontStyle.Bold);
            Size textSize;

            //now loop over font sizes
            do
            {
                myFont = new Font("Consolas", ++fs, FontStyle.Bold);
                textSize = TextRenderer.MeasureText(myIcon.Text, myFont);
            }
            //stop when text size is bigger than picture box size
            while (textSize.Width < picSize.Width * 0.75 &&
                textSize.Height < picSize.Height * 0.35);
            myFont = new Font("Consolas", --fs, FontStyle.Bold);

            //draw text, center it using it's own size
            //(half image widht and height, then subtract half of text's widht and height)
            e.Graphics.DrawString(myIcon.Text,  //text to show 
                myFont,                         //font to use
                Brushes.Yellow,                 //text color (brush)
                pictureBox.Width/2 -            //text X position
                TextRenderer.MeasureText(myIcon.Text, myFont).Width / 2, 
                pictureBox.Height * (float)(0.90) -           //text Y position
                TextRenderer.MeasureText(myIcon.Text, myFont).Height / 2);
            myFont.Dispose();   //release font, don't need it anymore
        */
        }

        //Pick a date - show calendar
        private void buttonPickDate_Click(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            if (!Calendar.Visible)
            {
                Calendar.ShowTodayCircle = true;
                Calendar.BringToFront();
                Calendar.SetDate(apod.apiDate);
                Calendar.Visible = true;
            }
            else
                Calendar.Visible = false;
        }

        //Calendar click - set the date and get the image at once
        private void Calendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            Calendar.Visible = false;
            if (Calendar.SelectionStart > DateTime.Today)
                Calendar.SelectionStart = DateTime.Today;
            setAPIDate(apod, Calendar.SelectionStart);
            if (apod.media_type != null)
                getNASAApod();
        }

        //Just move your mouse over the description to be able to scroll it
        private void TextBoxImgDesc_MouseHover(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            textBoxImgDesc.Focus();
            textBoxImgDesc.Select(0, 0);
        }

        //Go to 'how to' link
        private void linkHowToKey_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            Process.Start(e.Link.LinkData as string);
        }

        //Use custom key checkbox
        private void checkCustomKey_CheckedChanged(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            if (checkCustomKey.Checked) textCustomKey.Enabled = true;
            else textCustomKey.Enabled = false;

            iniFile.Write("useCustomKey", checkCustomKey.Checked.ToString());
        }

        //Custom key value - save in INI file
        private void textCustomKey_TextChanged(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            iniFile.Write("customKey", textCustomKey.Text);
        }

        private void listHistory_DoubleClick(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            setAPIDate(apod, DateTime.Parse(listHistory.SelectedItems[0].Text));
            getNASAApod();
        }

        private void buttonGrabAll_Click(object sender, EventArgs e)
        {
            Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            buttonGrabAll.Enabled = false;
            timer1.Enabled = false;

            WebClient wc = new WebClient();
            DateTime _min = DateTime.Parse("1995-06-16");
            int _span = (int)(DateTime.Today - _min).TotalDays;
            int _prog = 0;

            APOD _apod = new APOD();
            _apod.setAPIKey(textCustomKey.Text);
            _apod.setAPIDate(_min);

            textGrabAll.Text = "Download progress: " + _prog * 100 / _span + '%';
            textGrabAll.Text += " (" + (_span - _prog) + " left to process)";

            string _dir = Directory.CreateDirectory("APOD_ARCHIVE").Name;
            while (_apod.apiDate <= DateTime.Today)
            {
                if (_apod.media_type == "image")
                {
                    int begin = _apod.hdurl.LastIndexOf('/') + 1;
                    int len = _apod.hdurl.Length - begin;

                    wc.DownloadFile(new Uri(_apod.hdurl),
                        _dir + '\\' +
                        _apod.apiDate.ToShortDateString().Replace(' ', '_') + '_' +
                        _apod.hdurl.Substring(begin, len));
                } //end image type

                _prog++;
                textGrabAll.Text = "Download progress: " + _prog * 100 / _span + '%';
                textGrabAll.Text += " (" + (_span - _prog) + " dates left to process)";
                textGrabAll.Invalidate();
                _apod.setAPIDate(_apod.apiDate.AddDays(1));
            } //end loop

            _apod.Dispose();
            buttonGrabAll.Enabled = true;
            timer1.Enabled = true;
        }

        //Enable or disable history function
        private void checkEnableHistory_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEnableHistory.Checked)
            {
                iniFile.Write("enableHistory", "True");
                listHistory.Enabled = true;
            }
            else
            {
                iniFile.Write("enableHistory", "False");
                listHistory.Enabled = false;
            }
        }
    }
}
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
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
        public string imagePath = "temp.jpg"; //default file to save

        //GUI items
        ContextMenu myIconMenu; //context menu for tray icon
        bool hidden = false;    //main form state - hidden or not

        //setup ini file to store usage statistics
        IniFile iniFile = new IniFile();

        //Wallpapering
        [DllImport("kernel32.dll")]
        static extern uint GetLastError();
        [DllImport("user32.dll")]
        static extern int SystemParametersInfo(int Action, int nParam, string sParam, int WinIni);
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        //Logging switch - disabled on startup, requires cmd line param to be enabled
        readonly bool logging = false;

        //--- Main class methods -----------------------------------------------------------------

        //Default constructor - will create program window and set everything up
        public MainWindow()
        {
            //Enable logging if required - just pass anything as pgm parameter
            if (Environment.GetCommandLineArgs().Length > 1)
                logging = true;

            Log("\n--- PROGRAM START -----------------------------------------------");
            Log(MethodBase.GetCurrentMethod().Name);
            Log("Initializing...");
            InitializeComponent();
            Log("DONE!");

            //Read anything from INI file to check if it was already created
            //If not, write initial zero number of runs
            Log("Ini file first read");
            var lastDate = iniFile.Read("lastDate");

            if (lastDate == string.Empty) //write default values to INI file
            {
                Log("INI file empty, fill in with default values");
                iniFile.Write("lastDate", DateTime.Today.ToString());
                iniFile.Write("autoRefresh", checkAutoRefresh.Checked.ToString());
                iniFile.Write("saveToDisk", checkSaveToDisk.Checked.ToString());
                iniFile.Write("pathToSave", string.Empty);
                iniFile.Write("useCustomKey", checkEnableHistory.Checked.ToString());
                iniFile.Write("customKey", string.Empty);
                iniFile.Write("enableHistory", checkEnableHistory.Checked.ToString());
            }

            //GUI items - START -------------------------------------------------------
            Log("setting up GUI items...");

            if (!logging) //remove debugging tab if not desired by pgm parameter
                tabControl.TabPages.Remove(tabDebug);

            //Add actual link to link label
            LinkLabel.Link lnk = new LinkLabel.Link
            {
                LinkData = "https://api.nasa.gov/#signUp"
            };
            linkHowToKey.Links.Add(lnk);

            //Clear some labels
            labelImageDesc.Text = string.Empty;
            textDate.Text = string.Empty;
            
            //Add menu to tray icon
            myIconMenu = new ContextMenu();
            myIconMenu.MenuItems.Add("Previous", buttonPrev_Click);
            myIconMenu.MenuItems[myIconMenu.MenuItems.Count - 1].Name = "menuPrev";
            myIconMenu.MenuItems.Add("Next", buttonNext_Click);
            myIconMenu.MenuItems[myIconMenu.MenuItems.Count - 1].Name = "menuNext";
            myIconMenu.MenuItems.Add("Today", OnMenuToday);
            myIconMenu.MenuItems[myIconMenu.MenuItems.Count - 1].Name = "menuToday";
            myIconMenu.MenuItems.Add("-");
            myIconMenu.MenuItems[myIconMenu.MenuItems.Count - 1].Name = "menuSeparator";
            myIconMenu.MenuItems.Add("Exit", OnMenuExit);
            myIconMenu.MenuItems[myIconMenu.MenuItems.Count - 1].Name = "menuExit";
            myIcon.ContextMenu = myIconMenu;

            //Setup icons for window and tray icon
            myIcon.Icon = Properties.Resources.NASA;
            this.Icon = Properties.Resources.NASA;

            //Now setup settings tab according to values stored in INI file
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
            
            //Setup history tab columns
            listHistory.Columns[0].Text = "Date";
            listHistory.Columns[1].Text = "Title";

            //Remove history tab by default, then add if desired
            tabControl.TabPages.Remove(tabHistory);
            if (iniFile.Read("enableHistory") == "True")
            {
                checkEnableHistory.Checked = true;
            }
            else
            {
                checkEnableHistory.Checked = false;
            }
            Log("DONE!");
            //GUI items - END -------------------------------------------------------

            //Subscribe for events
            AppDomain.CurrentDomain.ProcessExit += OnAppExit;
            pictureBox.LoadProgressChanged += PictureBox_LoadProgressChanged; //image download progress bar
            pictureBox.LoadCompleted += PictureBox_LoadCompleted; //image downloaded - rest of the logis is there

            //Create 'photo of the day' object and set API date to today for starters
            //At this point we should have custom key read from settingss. If not, default one will be used
            Log("Create main apod object");
            apod = new APOD();
            apod.setAPIKey(textCustomKey.Text);

            Log("setting date - STARTUP...");
            //If auto-refresh option is set, always use today's date
            if (checkAutoRefresh.Checked)
            {
                setApodDate(apod, DateTime.Today);
            }
            else
            {
                setApodDate(apod, DateTime.Parse(iniFile.Read("lastDate")));
            }
            Log("STARTUP DATE = " + apod.apiDate);

            //Get the image on startup, but only if API call was succesful (eg. prevent network errors)
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
            Log(MethodBase.GetCurrentMethod().Name);

            //Local APOD just to get img title list
            APOD _apod = new APOD();
            _apod.setAPIKey(textCustomKey.Text);

            //Clear history list
            listHistory.Items.Clear();
            statusBar.Text = "Getting history items... (0/0)";
            Application.DoEvents();

            //Some history setup
            byte maxDays = 8; //how many history items, hardcoded
            byte cnt = 0; //items loaded
            int daysback = 0; //start going back in time today

            //Now go back in time and fetch history items
            while (cnt < maxDays)
            {
                _apod.setAPIDate(apod.apiDate.AddDays(daysback)); //go back further back in time
                if (_apod.isImage) //add item only if media is image
                {
                    listHistory.Items.Add(_apod.apiDate.ToShortDateString()); //history date
                    listHistory.Items[cnt].SubItems.Add(_apod.title); //history img name 
                    cnt++; //item loaded!
                    progressBar.Value = 100 * cnt / maxDays; //update progress bar and status text
                    statusBar.Text = "Getting history items... (" + cnt + "/" + maxDays + ")";
                    Application.DoEvents();
                    Log("History item added " + _apod.date);
                }
                daysback--; //time travel!
            }
            _apod.Dispose(); //destroy local APOD
            listHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            statusBar.Text = string.Empty;
        }

        //Set current date for APOD object
        private void setApodDate(APOD apod, DateTime datetime)
        {
            Log(MethodBase.GetCurrentMethod().Name);
            Log("trying this date: " + datetime);

            //Clear date related GUI fields
            textDate.ForeColor = Color.LightSlateGray;
            textDate.Text = "Wait...";
            Log("date field \"wait\" and grey");

            //Try to call apod with desired date
            try
            {
                Log("calling apod.setAPIDate()...");
                apod.setAPIDate(datetime);
                //Setup prev/next buttons with previous and next dates ←→►◄
                buttonPrev.Text = "<< " + apod.apiDate.AddDays(-1).ToShortDateString();
                buttonNext.Text = apod.apiDate.AddDays(1).ToShortDateString() + ">>";

                Log("Call succesful! apod.date = " + apod.apiDate);
                DebugTab(apod);

                //Fill history items
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

        //Tray icon menu "today" event handler
        private void OnMenuToday(object sender, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            //Get today's image
            apod.setAPIDate(DateTime.Today);
            if (apod.media_type != null)
                getNASAApod();
        }

        //Tray icon menu "exit" event handler
        private void OnMenuExit(object sender, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            Application.Exit();
        }

        //App exit event, just for logging
        private void OnAppExit(object sender, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);
            Log("--- THE END! ------------------------------------------------");
        }

        //Get the picture and possibly save it to disk (if required in GUI)
        private void getNASAApod()
        {
            Log(MethodBase.GetCurrentMethod().Name);

            //Setup GUI items for download time
            Log("setting some gui items...");
            statusBar.Text = "Getting NASA picture of the day...";
            myIcon.Text = statusBar.Text;
            buttonPrev.Enabled = false;
            buttonToday.Enabled = false;
            buttonNext.Enabled = false;
            buttonRefresh.Enabled = false;
            labelImageDesc.Text = string.Empty;
            textBoxImgDesc.Text = string.Empty;
            buttonPickDate.Enabled = false;
            foreach (MenuItem mi in myIconMenu.MenuItems)
            {
                mi.Enabled = false;
            }
            Log("done!");

            //Download image to picture box
            Log("trying to download the image...");
            try
            {
                //Get it from apod url, only if it's an image (sometimes they post videos, gifs, etc.)
                if (apod.isImage)
                {
                    //Download the image directly to image box
                    //"Image" property will allow to save it later
                    //(try normal-res picture by default)
                    //(hd images are large and use too much memory)
                    Log("API says that image is there: " + apod.hdurl);
                    //Since this is async action, rest of the logic will be called whenever download is completed
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
                    textDate.Text = string.Empty;
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
            //Log(MethodBase.GetCurrentMethod().Name); //no need to log this

            progressBar.Value = e.ProgressPercentage;
            statusBar.Text = "Getting NASA picture of the day... " + e.ProgressPercentage + "%";
        }

        //Download completed event - do rest of the logic - actual wallpapering and saving to disk
        private void PictureBox_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            //Setup picture box properties
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Visible = true;

            //Save the image to disk - required to set the wallpaper
            //even if "save to disk" is not set in GUI
            saveToDisk();

            //Before setting the wallpaper, we have to build full path of TEMP.JPG
            //but only if not saving to custom path, because then it was already built
            if (!checkSaveToDisk.Checked)
                imagePath = Assembly.GetEntryAssembly().Location.Substring(0,
                           Assembly.GetEntryAssembly().Location.LastIndexOf("\\") + 1)
                         + "temp.jpg";

            //Do actual wallpapering
            Log("Wallpapering...");
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            Log("registry file BEFORE = " + key.GetValue("Wallpaper"));
            key.SetValue(@"Wallpaper", imagePath);
            key.SetValue(@"WallpaperStyle", 6.ToString()); //always fit image to screen (zoom mode)
            key.SetValue(@"TileWallpaper", 0.ToString()); //do not tile
            Log("registry set...");

            //Save wallpaper proc output
            int _out = SystemParametersInfo(SPI_SETDESKWALLPAPER, //action
                0, //parm
                imagePath, //parm
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE); //winini

            Log("Wallpapering result = " + _out);
            Log("error msg = " + GetLastError());
            key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            Log("registry file AFTER = " + key.GetValue("Wallpaper"));

            //Save last image date to INI file
            iniFile.Write("lastDate", apod.apiDate.ToString());

            //Setup UI elements
            //Activate tray menu and then enable/disable 'previous' and 'next' buttons, depending on the API date
            setupButtons();

            //Set image title
            myIcon.Text = apod.title;
            labelImageDesc.Text = apod.title;
            labelImageDesc.Width = 375;

            //Copyright is not always there
            if (apod.copyright != null && apod.copyright != string.Empty)
                labelImageDesc.Text += "\n© " + apod.copyright.Replace("\n", " ");

            //Tray ballon
            myIcon.BalloonTipTitle = "NASA Astronomy Picture of the Day";
            myIcon.BalloonTipText = apod.title;
            myIcon.ShowBalloonTip(5000);
            
            //Image description and URL
            textBoxImgDesc.Text = apod.explanation;
            textURL.Text = apod.hdurl;
            this.Text = "NASA Astronomy Picture of the Day - " +
                apod.apiDate.ToShortDateString() + " - " +
                apod.title;
            textDate.ForeColor = Color.Black;
            textDate.Text = apod.apiDate.ToShortDateString();

            //TODO: Sometimes wallpapering does not work - why? This happens in Win7 only
            if (_out == 0)
                statusBar.Text = "Downloaded but not set :(";
            else
                statusBar.Text = "Done!";

            //Invalidate picture box to force redrawing, just in case
            pictureBox.Invalidate();
        }

        //Enable or disable prev/today/buttons depending on current API date
        private void setupButtons()
        {
            Log(MethodBase.GetCurrentMethod().Name);

            buttonPickDate.Enabled = true;
            myIconMenu.MenuItems["menuToday"].Enabled = true;
            myIconMenu.MenuItems["menuExit"].Enabled = true;

            //Enable/disable 'previous' and 'next' buttons, depending on the API date
            //TODAY - previous enabled, today enabled, next disabled
            if (apod.apiDate == DateTime.Today)
            {
                //Window buttons
                buttonPrev.Enabled = true;
                buttonToday.Enabled = false;
                buttonNext.Text = "NEXT >>"; //←→►◄
                buttonNext.Enabled = false;
                buttonRefresh.Enabled = true;

                //Tray menu buttons
                myIconMenu.MenuItems["menuPrev"].Enabled = true;
                myIconMenu.MenuItems["menuNext"].Enabled = false;
            }
            else
            //EARLIER - all enabled
            {
                //Window buttons
                buttonPrev.Enabled = true;
                buttonToday.Enabled = true;
                buttonNext.Enabled = true;
                buttonRefresh.Enabled = true;

                //Tray menu buttons
                myIconMenu.MenuItems["menuPrev"].Enabled = true;
                myIconMenu.MenuItems["menuNext"].Enabled = true;
            }
        }

        //Save current image to disk
        private void saveToDisk()
        {
            Log(MethodBase.GetCurrentMethod().Name);

            //First build custom path if desired
            if ((pathToSave != null && pathToSave != string.Empty) &&
                (apod.hdurl != null && apod.hdurl != string.Empty)) //custom path found, concatenate path with image filename
            {
                int begin = apod.hdurl.LastIndexOf('/') + 1;
                int len = apod.hdurl.Length - begin;
                imagePath = pathToSave + "\\" + //folder path
                    apod.apiDate.ToShortDateString().Replace(' ', '_') + '_' + //inject date string at the begining of filename
                    apod.hdurl.Substring(begin, len); //filename from URL
            }

            //Do actual saving
            try
            {
                pictureBox.Image.Save(imagePath);
            }
            catch (Exception e)
            {
                Log(e.Message);
                statusBar.Text = e.Message;
            }
        }

        //Timer event handler - reload image with today date
        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);
            Log("--- TIMER TICK! ---");

            //Reload image only if today date has changed
            if (DateTime.Today != apod.apiDate)
            {
                Log("AUTO REFRESH - date rolled over");
                Log("API date = " + apod.apiDate);
                Log("TODAy date = " + DateTime.Today);
                setApodDate(apod, DateTime.Today);
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
            Log(MethodBase.GetCurrentMethod().Name);

            if (checkAutoRefresh.Checked)
                timerRefresh.Enabled = true;
            else
                timerRefresh.Enabled = false;

            iniFile.Write("autoRefresh", checkAutoRefresh.Checked.ToString());
        }

        //Save to custom path checkbox
        private void checkSaveToDisk_CheckedChanged(object sender, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            //Saving to file is always active - we need image file to set the wallpaper
            //It is file path that we will manipulate here
            if (checkSaveToDisk.Checked) //save to disak enabled
            {
                //Enable path selection [...] button and file path text box
                buttonPath.Enabled = true;
                textPath.Enabled = true;

                //Is it first click? Custom path would be null in this case
                if (textPath.Text == null || textPath.Text == string.Empty)
                {
                    buttonPath_Click(this, e); //invoke path selection dialog
                    if (dialogPath.SelectedPath == string.Empty) //if dialog cancelled, revert UI changes
                    {
                        buttonPath.Enabled = false;
                        textPath.Enabled = false;
                        checkSaveToDisk.Checked = false;
                    }
                }
                else
                    pathToSave = textPath.Text;
            }
            else //save to disk disabled
            {
                buttonPath.Enabled = false;
                textPath.Enabled = false;
                pathToSave = null;
                imagePath = "temp.jpg";
            }

            iniFile.Write("saveToDisk", checkSaveToDisk.Checked.ToString());
            iniFile.Write("pathToSave", pathToSave);
        }

        //Custom path selection button
        private void buttonPath_Click(object sender, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            dialogPath.ShowDialog(); //display path selection dialog

            if (dialogPath.SelectedPath != string.Empty)
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
            Log(MethodBase.GetCurrentMethod().Name);

            if (textURL.Text != string.Empty)
                Clipboard.SetText(textURL.Text);
        }

        //Copy image to clipboard
        private void buttonCopyImage_Click(object sender, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            if (pictureBox.Image != null)
                Clipboard.SetImage(pictureBox.Image);
        }

        //Tray icon click - hide/show window
        private void myIcon_MouseClick(object sender, MouseEventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            //Left click only
            if (e.Button == MouseButtons.Left)
            
            {
                //Toggle window state
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
        }

        //Minimize to system tray
        private void windowResize(object sender, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

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
            Log(MethodBase.GetCurrentMethod().Name);

            setApodDate(apod, apod.apiDate.AddDays(-1));
            if (apod.media_type != null)
                getNASAApod();
        }

        //Next button click
        private void buttonNext_Click(object sender, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            if (apod.apiDate < DateTime.Today)
            {
                setApodDate(apod, apod.apiDate.AddDays(1));
                if (apod.media_type != null)
                    getNASAApod();
            }
        }

        //Today button click
        private void buttonToday_Click(object sender, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            setApodDate(apod, DateTime.Today);
            if (apod.media_type != null)
                getNASAApod();
        }

        //Refresh button - simply reload current image
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            setApodDate(apod, apod.apiDate); //refresh json strings with current setup
            if (apod.media_type != null)
                getNASAApod();
        }

        //EXPERIMANTAL - draw the title over the picture
        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

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
            Log(MethodBase.GetCurrentMethod().Name);

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
            Log(MethodBase.GetCurrentMethod().Name);

            Calendar.Visible = false;
            if (Calendar.SelectionStart > DateTime.Today) //don't allow future date
                Calendar.SelectionStart = DateTime.Today;
            setApodDate(apod, Calendar.SelectionStart);
            if (apod.media_type != null)
                getNASAApod();
        }

        //Just move your mouse over the description to be able to scroll it
        private void TextBoxImgDesc_MouseHover(object sender, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            textBoxImgDesc.Focus();
            textBoxImgDesc.Select(0, 0);
        }

        //Go to 'how to' link
        private void linkHowToKey_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            Process.Start(e.Link.LinkData as string);
        }

        //Use custom key checkbox
        private void checkCustomKey_CheckedChanged(object sender, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            if (checkCustomKey.Checked)
                textCustomKey.Enabled = true;
            else 
                textCustomKey.Enabled = false;

            iniFile.Write("useCustomKey", checkCustomKey.Checked.ToString());
        }

        //Custom key value - save in INI file
        private void textCustomKey_TextChanged(object sender, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            iniFile.Write("customKey", textCustomKey.Text);
        }

        //Go to history item
        private void listHistory_DoubleClick(object sender, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            setApodDate(apod, DateTime.Parse(listHistory.SelectedItems[0].Text));
            getNASAApod();
        }

        //EXPERIMENTAL, NOT FULLY TESTED, HIGH POTENTIAL TO CRASH SOMEHOW
        //Try to grab WHOLE apod archive
        //TODO: create list of URLs first and then download everything in parallel
        private void buttonGrabAll_Click(object sender, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            WebClient _wc = new WebClient();
            _wc.DownloadProgressChanged += PictureBox_LoadProgressChanged;
            DateTime _min = DateTime.Parse("1995-06-16"); //first day in APOD archive
            int _span = 1 + (int)(DateTime.Today - _min).TotalDays; //number of days from today to minimum date
            int _prog = 0; //progress percentage
            int _err = 0; //number of errors or non-images

            //Create local apod object
            APOD _apod = new APOD();
            _apod.setAPIKey(textCustomKey.Text);

            //Set some GUI items
            buttonGrabAll.Enabled = false;
            timerRefresh.Enabled = false;
            statusBar.Text = "Grabbing the archive...";
            textGrabAll.Text = "Download begins... (" + _prog * 100 / _span + '%';
            textGrabAll.Text += ", " + (_span - _prog) + " left to download)";
            textGrabAll.Invalidate();
            Application.DoEvents();

            //Save files in subdir of current folder
            string _dir = Directory.CreateDirectory("APOD_ARCHIVE").Name;
            
            //Go from today (progress = 0) to minimum date (progress = span)
            while (_prog < _span)
            {
                //Set API date according to progress (starting with zero increase)
                try
                {
                    _apod.setAPIDate(_min.AddDays(_prog));
                }
                catch (Exception)
                {}

                //Try to download actual image
                if (_apod.isImage)
                {
                    try
                    {
                        //Image name begin and length - to create final filename
                        int begin = _apod.hdurl.LastIndexOf('/') + 1;
                        int len = _apod.hdurl.Length - begin;

                        //Double check image URL, if HD not available try regular
                        if (_apod.hdurl != null && _apod.hdurl != string.Empty)
                            _wc.DownloadFileAsync(new Uri(_apod.hdurl),
                                _dir + '\\' +
                                _apod.apiDate.ToShortDateString().Replace(' ', '_') + '_' +
                                _apod.hdurl.Substring(begin, len));
                        else if (_apod.url != null && _apod.url != string.Empty)
                            _wc.DownloadFileAsync(new Uri(_apod.url),
                                _dir + '\\' +
                                _apod.apiDate.ToShortDateString().Replace(' ', '_') + '_' +
                                _apod.url.Substring(begin, len));
                    }
                    catch (Exception)
                    {
                        _err++;
                    }
                }
                else
                {
                    _err++;
                }

                while (_wc.IsBusy) ; //dummy wait until previous download completes

                _prog++;
                textGrabAll.Text = "Download progress: " + _prog * 100 / _span + '%';
                textGrabAll.Text += " (" + (_span - _prog) + " left to download)";
                textDate.Text = _apod.apiDate.ToShortDateString();
                textGrabAll.Invalidate();
                Application.DoEvents();
            }

            _apod.Dispose();
            _wc.Dispose();
            buttonGrabAll.Enabled = true;
            timerRefresh.Enabled = true;
            textGrabAll.Text = "Done! Downloaded " + (_span - _err).ToString() + " images (" + _span.ToString() + " total)";
            textDate.Text = _apod.apiDate.ToShortDateString();
            statusBar.Text = "Done!";
            Application.DoEvents();
        }

        //Enable or disable history function
        private void checkEnableHistory_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEnableHistory.Checked)
            {
                iniFile.Write("enableHistory", "True");
                tabControl.TabPages.Add(tabHistory);
                if (apod != null) //fill history tab only if apod object has been initialized
                    fillHistory();
            }
            else
            {
                iniFile.Write("enableHistory", "False");
                tabControl.TabPages.Remove(tabHistory);
            }
        }
    }
}
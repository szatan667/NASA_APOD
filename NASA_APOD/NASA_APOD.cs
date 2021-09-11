using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NASA_APOD
{
    public partial class NASA_APOD : Form
    {
        //Main photo of the day object
        public APOD Apod;

        //Paths
        public string PathToSave; //will hold path from folder dialog
        public string ImagePath = @".\temp.jpg"; //default file to save

        //GUI items
        private bool FormHidden = false;
        private Color AvgColor = SystemColors.ActiveCaption;

        //setup ini file to store usage statistics
        readonly IniFile IniFile = new();

        //Wallpapering
        [DllImport("kernel32.dll")]
        static extern uint GetLastError();
        [DllImport("user32.dll")]
        static extern int SystemParametersInfo(int Action, int nParam, string sParam, int WinIni);
        const int SPI_SETDESKWALLPAPER = 0x14;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        //Logging switch - disabled on startup, requires cmd line param to be enabled
        readonly bool Logging = false;

        //Grab archive
        static readonly DateTime DateMin = DateTime.Parse("1995-06-16"); //first day in APOD archive
        static readonly int DaysSpan = 1 + (int)(DateTime.Today - DateMin).TotalDays; //number of days from today to minimum date
        int DaysProg = 0; //download progress
        int DaysErrors = 0; //number of errors or non-images

        //---- Default constructor - create program window and set everything up ----
        public NASA_APOD()
        {
            //Enable logging if required - just pass anything as pgm parameter
            //if (Environment.GetCommandLineArgs().Length > 1)
            Logging = true;

            //Disable certificate checking - dirty and temporary
            //ServicePointManager.ServerCertificateValidationCallback += (a, b, c, d) => true;

            Log("\n--- PROGRAM START -----------------------------------------------");
            Log(MethodBase.GetCurrentMethod().Name);
            Log("Initializing...");
            InitializeComponent();
            Log("DONE!");

            //Read anything from INI file to check if it was already created
            //If not, write initial zero number of runs
            Log("Ini file first read");
            string lastDate = IniFile.Read("lastDate");

            if (lastDate == string.Empty) //write default values to INI file
            {
                Log("INI file empty, fill in with default values");
                IniFile.Write("lastDate", DateTime.Today.ToString());
                IniFile.Write("autoRefresh", checkAutoRefresh.Checked.ToString());
                IniFile.Write("saveToDisk", checkSaveToDisk.Checked.ToString());
                IniFile.Write("pathToSave", string.Empty);
                IniFile.Write("useCustomKey", checkEnableHistory.Checked.ToString());
                IniFile.Write("customKey", string.Empty);
                IniFile.Write("enableHistory", checkEnableHistory.Checked.ToString());
            }

            //GUI items - START -------------------------------------------------------
            Log("setting up GUI items...");

            pictureBox.ErrorImage = Properties.Resources.NASA.ToBitmap();
            pictureBox.InitialImage = Properties.Resources.NASA.ToBitmap();

            if (!Logging) //remove debugging tab if not desired by pgm parameter
                tabControl.TabPages.Remove(tabDebug);

            //Add actual link to link label
            LinkLabel.Link lnk = new() { LinkData = "https://api.nasa.gov/#signUp" };
            linkHowToKey.Links.Add(lnk);

            //Clear some labels
            labelImageDesc.Text = string.Empty;
            textDate.Text = string.Empty;

            //Add menu to tray icon
            trayIcon.ContextMenu = new(new MenuItem[]
            {
                new("Previous", ButtonPrev_Click) { Name = "menuPrev", OwnerDraw = true },
                new("Next", ButtonNext_Click)     { Name = "menuNext", OwnerDraw = true },
                new("Today", OnMenuToday)         { Name = "menuToday", OwnerDraw = true },
                new("-")                          { OwnerDraw = false },
                new("Exit", OnMenuExit)           { Name = "menuExit", DefaultItem = true, OwnerDraw = true }
            });

            //Register custom menu measure&draw routines
            foreach (MenuItem mi in trayIcon.ContextMenu.MenuItems)
            {
                mi.MeasureItem += MenuItemMeasure;
                mi.DrawItem += MenuItemDraw;
            }

            //Setup icons for window and tray icon
            trayIcon.Icon = Properties.Resources.NASA;
            Icon = Properties.Resources.NASA;

            //Now setup settings tab according to values stored in INI file
            PathToSave = IniFile.Read("pathToSave");
            textPath.Text = PathToSave;

            if (IniFile.Read("saveToDIsk") == "True")
                checkSaveToDisk.Checked = true;
            else
                checkSaveToDisk.Checked = false;

            if (IniFile.Read("autoRefresh") == "True")
                checkAutoRefresh.Checked = true;
            else
                checkAutoRefresh.Checked = false;

            if (IniFile.Read("useCustomKey") == "True")
            {
                checkCustomKey.Checked = true;
                textCustomKey.Enabled = true;
            }
            else
            {
                checkCustomKey.Checked = false;
                textCustomKey.Enabled = false;
            }
            textCustomKey.Text = IniFile.Read("customKey");

            //Setup history tab columns
            listHistory.Columns[0].Text = "Date";
            listHistory.Columns[1].Text = "Title";

            //Remove history tab by default, then add if desired
            tabControl.TabPages.Remove(tabHistory);
            if (IniFile.Read("enableHistory") == "True")
                checkEnableHistory.Checked = true;
            else
                checkEnableHistory.Checked = false;

            //Now for run at startup birdie
            checkRunAtStartup.Checked = new TaskService().RootFolder.GetTasks(new("NASA_APOD")).Count != 0 &&
                                        new TaskService().RootFolder.GetTasks(new("NASA_APOD"))[new Regex("NASA_APOD").ToString()].Enabled;

            Log("DONE!");
            //GUI items - END -------------------------------------------------------

            //Subscribe for events
            AppDomain.CurrentDomain.ProcessExit += OnAppExit;
            pictureBox.LoadProgressChanged += PictureBox_LoadProgressChanged; //image download progress bar
            pictureBox.LoadCompleted += PictureBox_LoadCompleted; //image downloaded - rest of the logis is there

            //Create 'photo of the day' object and set API date to today for starters
            //At this point we should have custom key read from settingss. If not, default one will be used
            Log("Create main apod object");
            Apod = new();
            Apod.SetApiKey(textCustomKey.Text);

            Log("setting date - STARTUP...");
            //If auto-refresh option is set, always use today's date
            if (checkAutoRefresh.Checked)
                SetApodDate(Apod, DateTime.Today);
            else
                SetApodDate(Apod, DateTime.Parse(IniFile.Read("lastDate")));

            Log("STARTUP DATE = " + Apod.ApiDate);

            //Get the image on startup
            Log("Getting the image on startup...");
            GetNasaApod();
        }

        /// <summary>
        /// Log string value to "apod.log" file
        /// </summary>
        /// <param name="logMessage">String value to be saved in log file</param>
        private void Log(string logMessage)
        {
            if (Logging)
                try
                {
                    using TextWriter tw = new StreamWriter("apod.log", true);
                    tw.WriteLine(DateTime.Now.ToString() + " - " + logMessage);
                }
                catch (Exception ex)
                {
                    Trace.Write(ex.ToString());
                }
        }

        /// <summary>
        /// Logging to tab - fill debug tab with raw API output json keys
        /// </summary>
        /// <param name="apod">APOD object</param>
        private void DebugTab(APOD apod)
        {
            if (Logging)
            {
                //nothing fancy, use hard-coded list items to display API call response
                listDebug.Items[0].SubItems[1].Text = apod.Copyright;
                listDebug.Items[1].SubItems[1].Text = apod.Date;
                listDebug.Items[2].SubItems[1].Text = apod.Explanation;
                listDebug.Items[3].SubItems[1].Text = apod.HdUrl;
                listDebug.Items[4].SubItems[1].Text = apod.MediaType;
                listDebug.Items[5].SubItems[1].Text = apod.ServiceVersion;
                listDebug.Items[6].SubItems[1].Text = apod.Title;
                listDebug.Items[7].SubItems[1].Text = apod.Url;
                listDebug.Items[8].SubItems[1].Text = string.Empty;
                listDebug.Columns[0].Width = 75;
                listDebug.Columns[1].Width = 1350;
            }
        }

        /// <summary>
        /// Fill history tab list with dates and headers
        /// </summary>
        private void FillHistory()
        {
            Log(MethodBase.GetCurrentMethod().Name);

            //Local APOD to get img title list
            using APOD a = new();
            a.SetApiKey(textCustomKey.Text);

            //Clear history list
            listHistory.Items.Clear();
            statusBar.Text = "Getting history items... (0/0)";
            Application.DoEvents();

            //Some history setup
            byte maxDays = 8; //how many history items, hardcoded
            byte cnt = 0; //items loaded
            int daysback = 0; //start going back in time today

            //Now go back in time and fetch history items
            try
            {
                a.SetApiDate(Apod.ApiDate.AddDays(daysback)); //go back further back in time

                while (cnt < maxDays && Apod.ApiDate.AddDays(daysback) >= DateTime.Parse("1995-06-16"))
                {
                    listHistory.Items.Add(a.ApiDate.ToShortDateString()); //history date
                    listHistory.Items[cnt].SubItems.Add(a.Title); //history img name 
                    cnt++; //item loaded!
                    progressBar.Value = 100 * cnt / maxDays; //update progress bar and status text
                    statusBar.Text = "Getting history items... (" + cnt + "/" + maxDays + ")";
                    Application.DoEvents();
                    Log("History item added " + a.Date);

                    //Time travel
                    daysback--;
                    a.SetApiDate(Apod.ApiDate.AddDays(daysback));
                }
                listHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                progressBar.Value = 100;
                statusBar.Text = string.Empty;
            }
            catch (Exception e)
            {
                statusBar.Text = e.Message;
            }
        }

        /// <summary>
        /// Set current date for APOD object
        /// </summary>
        /// <param name="apod">APOD object</param>
        /// <param name="apodDate">Desired date</param>
        private void SetApodDate(APOD apod, DateTime apodDate)
        {
            Log(MethodBase.GetCurrentMethod().Name);
            Log("trying this date: " + apodDate);

            //Clear date related GUI fields
            textDate.ForeColor = SystemColors.GrayText;
            textDate.Text = "Wait...";
            Log("date field \"wait\" and grey");

            //Try to call apod with desired date
            try
            {
                Log("calling apod.setAPIDate()...");
                apod.SetApiDate(apodDate);

                //Save last image date to INI file
                IniFile.Write("lastDate", apod.ApiDate.ToString());

                Log("Call succesful! apod.date = " + apod.ApiDate);
                DebugTab(apod);

                //Fill history items
                if (checkEnableHistory.Checked)
                    FillHistory();
                else
                    listHistory.Items.Clear();

            }
            catch (Exception e)
            {
                Log("API SET DATE FAILED! requested date = " + apodDate.ToString());
                Log(e.Message);
                statusBar.Text = e.Message;
                textDate.ForeColor = SystemColors.ControlText;
                textDate.Text = "(none)";
            }
            finally
            {
                //Always try to setup prev/next buttons with previous and next dates ←→►◄
                buttonPrev.Text = "<< " + apod.ApiDate.AddDays(-1).ToShortDateString();
                buttonNext.Text = apod.ApiDate.AddDays(1).ToShortDateString() + " >>";
            }
        }

        //Tray icon menu "today" event handler
        private void OnMenuToday(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            //Get today's image
            Apod.SetApiDate(DateTime.Today);
            GetNasaApod();
        }

        //Tray icon menu "exit" event handler
        private void OnMenuExit(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            Application.Exit();
        }

        //App exit event, just for logging
        private void OnAppExit(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);
            Log("--- THE END! ------------------------------------------------");
        }

        /// <summary>
        /// Get the picture and possibly save it to disk (if required in GUI)
        /// </summary>
        private void GetNasaApod()
        {
            Log(MethodBase.GetCurrentMethod().Name);

            //Setup GUI items for download time
            tabImage.Focus();
            Log("setting some gui items...");
            statusBar.Text = "Getting NASA picture of the day...";
            trayIcon.Text = statusBar.Text;
            textURL.Text = string.Empty;
            buttonPrev.Enabled = false;
            buttonToday.Enabled = false;
            buttonNext.Enabled = false;
            buttonRefresh.Enabled = false;
            labelImageDesc.Text = string.Empty;
            textBoxImgDesc.Text = string.Empty;
            buttonPickDate.Enabled = false;
            buttonCopyImage.Enabled = false;
            buttonCopyLink.Enabled = false;
            foreach (MenuItem mi in trayIcon.ContextMenu.MenuItems)
                mi.Enabled = false;
            Log("done!");

            //Download image to picture box
            Log("trying to download the image...");

            //At this point apod object should be already initialized (ie. date was set)
            //APOD can be either an image or video
            //If it's an image - use picture box method to download picture in async...
            if (Apod.IsImage)
            {
                //Download the image directly to image box
                //"Image" property will allow to save it later
                //(try normal-res picture by default)
                //(hd images are large and use too much memory)
                Log("API says that image is there: " + Apod.HdUrl);

                //Since this is async action, rest of the logic will be called whenever download is completed
                labelImageDesc.Text = Apod.Title;
                textBoxImgDesc.Text = "Loading...";
                pictureBox.Visible = true;
                web.Visible = false;
                web.DocumentText = string.Empty;
                pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
                pictureBox.Image = Properties.Resources.NASA.ToBitmap();

                //There's a chance that if 'save to disk' is enabled, the image was downloaded before
                //In that case create full local path and try to load image from disk
                Apod.IsDownloading = true;
                if (checkSaveToDisk.Checked)
                {
                    ImagePath = CreateFullPath(PathToSave, Apod);
                    if (File.Exists(ImagePath))
                    {
                        pictureBox.LoadAsync(ImagePath);
                        Log("async download started - from cache");
                    }
                    else
                    {
                        pictureBox.LoadAsync(Apod.HdUrl);
                        Log("async download started");
                    }
                }
                else
                {
                    pictureBox.LoadAsync(Apod.HdUrl);
                    Log("async download started");
                }
            }
            //...otherwise try to play video link
            else
            {
                //Not a picture today, clear the image box with nasa icon and try to play video link
                Log("not a picture section - either a video link or null");
                pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
                pictureBox.Image = Properties.Resources.NASA.ToBitmap();
                pictureBox.Visible = true;
                web.Visible = false;
                //web.DocumentText = string.Empty;

                //Now try to pick up video link
                //Video link usually appears in 'url', but it's worth checking 'hdurl' as well
                //So far only youtube and vimeo links were spotted, so let's tray that
                if (Apod.VideoType != APOD.NasaVideoType.NONE)
                {
                    //Switch to web player box, hide picture
                    web.Visible = true;
                    pictureBox.Visible = false;

                    //Create document to be displayed by web browser control
                    string DocumentText =
                        "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=Edge\"/>" +
                        "<iframe width=" + web.Width + " " + "height=" + web.Height + " " +
                        "style=\"overflow: hidden; overflow - x:hidden; overflow - y:hidden; height: 100 %; width: 100 %; position: absolute; top: 0px; left: 0px; right: 0px; bottom: 0px\" " +
                        "src=\"" + "{0}" + "\" " +
                        "frameborder=\"0\" allow=\"autoplay; encrypted-media\" allowfullscreen></iframe>";

                    //Try to inject video link into web document
                    string vsrc = string.Empty;

                    switch (Apod.VideoType)
                    {
                        case APOD.NasaVideoType.YOUTUBE:
                            vsrc = (Apod.Url != string.Empty) ? Apod.Url.Substring(Apod.Url.IndexOf(APOD.VID_LINK_YT)) : Apod.HdUrl.Substring(Apod.HdUrl.IndexOf(APOD.VID_LINK_YT));
                            break;
                        case APOD.NasaVideoType.VIMEO:
                            vsrc = (Apod.Url != string.Empty) ? Apod.Url.Substring(Apod.Url.IndexOf(APOD.VID_LINK_VM)) : Apod.HdUrl.Substring(Apod.HdUrl.IndexOf(APOD.VID_LINK_VM));
                            break;
                        default:
                            break;
                    }

                    web.DocumentText = string.Format(DocumentText, "https://" + vsrc + (vsrc.Contains("?") ? "&autoplay=1" : "?autoplay=1"));
                }

                SetupGuiWhenCompleted();
            }
        }

        /// <summary>
        /// Create full path to local file. Put path picked from GUI together with picture filename and current date
        /// </summary>
        /// <param name="path">Local disk path</param>
        /// <param name="apod">APOD type object</param>
        /// <returns></returns>
        private string CreateFullPath(string path, APOD apod)
        {
            string filename = apod.ApiDate.ToShortDateString() + '_' +
                              apod.HdUrl.Substring(apod.Url.LastIndexOf('/') + 1);

            foreach (char c in Path.GetInvalidFileNameChars())
                filename = filename.Replace(c, '_');

            return path + "\\" + filename;
        }

        //Download progress bar - event handler
        private void PictureBox_LoadProgressChanged(object s, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            statusBar.Text = "Getting NASA picture of the day... " + e.ProgressPercentage + "%";

            //Draw progress circle over tray icon, only if minimized
            //if (e.ProgressPercentage < 100 && e.ProgressPercentage > 0 && formHidden)
            //    using (Bitmap bmp = new Bitmap(32, 32))
            //    using (Graphics gfx = Graphics.FromImage(bmp))
            //    using (Pen p = new Pen(new LinearGradientBrush(gfx.VisibleClipBounds,
            //        Color.FromArgb(0, 57, 146), Color.FromArgb(255, 57, 21), 90),
            //        gfx.VisibleClipBounds.Width / 4))
            //    {
            //        gfx.Clear(Color.Transparent);
            //        gfx.SmoothingMode = SmoothingMode.AntiAlias;
            //        gfx.DrawArc(p,
            //            p.Width / 2, p.Width / 2,
            //            gfx.VisibleClipBounds.Width - p.Width, gfx.VisibleClipBounds.Height - p.Width,
            //            -90, 360 * e.ProgressPercentage / 100);
            //        try
            //        { trayIcon.Icon = Icon.FromHandle(bmp.GetHicon()); }
            //        catch (Exception ex){ Log("tray icon from handle error"); Log(ex.Message); }
            //    }
            //else
            //    try
            //    { trayIcon.Icon = Properties.Resources.NASA; }
            //    catch (Exception ex){ Log("tray icon from resource error"); Log(ex.Message); }
        }

        //Download completed event - do rest of the logic - actual wallpapering and saving to disk
        private void PictureBox_LoadCompleted(object s, AsyncCompletedEventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            //Setup picture box properties
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Visible = true;

            //Save the image to disk - required to set the wallpaper
            //even if "save to disk" is not set in GUI
            SaveToDisk();

            //Before setting the wallpaper, we have to build full path of TEMP.JPG
            //but only if not saving to custom path, because then it was already built
            //if (!checkSaveToDisk.Checked)
            //    imagePath = Assembly.GetEntryAssembly().Location.Substring(0,
            //               Assembly.GetEntryAssembly().Location.LastIndexOf("\\") + 1)
            //             + "temp.jpg";

            //Do actual wallpapering
            Log("Wallpapering...");
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            Log("registry file BEFORE = " + key.GetValue("Wallpaper"));
            key.SetValue(@"Wallpaper", ImagePath);
            key.SetValue(@"WallpaperStyle", 6.ToString()); //always fit image to screen (zoom mode)
            key.SetValue(@"TileWallpaper", 0.ToString()); //do not tile
            key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors", true);
            key.SetValue(@"Background", "0 0 0"); //black desktop around the wallpaper
            Log("registry set...");

            //Save wallpaper proc output
            int _out = SystemParametersInfo(SPI_SETDESKWALLPAPER, //action
                0, //parm
                ImagePath, //parm
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE); //winini

            Log("Wallpapering result = " + _out);
            Log("error msg = " + GetLastError());
            key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            Log("registry file AFTER = " + key.GetValue("Wallpaper"));

            //Setup UI elements
            //Activate tray menu and then enable/disable 'previous' and 'next' buttons, depending on the API date
            SetupGuiWhenCompleted();

            //TODO: Sometimes wallpapering does not work - why? This happens in Win7 only
            if (_out == 0)
                statusBar.Text = "Downloaded but not set :(";
            else
            {
                statusBar.Text = "Done!";
                listDebug.Items[8].SubItems[1].Text = new FileInfo(ImagePath).Length / 1024 + " kB";
                Log("Wallpapering succesful, downloaded img size = " + listDebug.Items[8].SubItems[1].Text);
            }

            //Get average color from picture - to use it later in custom menu draw routine
            AvgColor = SystemColors.ActiveCaption; //use system color by default and then calc image average if possible
            if (pictureBox.Image != null)
            {
                int r = 0;
                int g = 0;
                int b = 0;
                int cnt = 0;

                using (Bitmap bmp = new(pictureBox.Image))
                    //Crop the image by 1/8 in both directions (there's usually dark frame or background there)
                    for (int x = bmp.Width / 8; x < bmp.Width * 7 / 8; x += bmp.Width / 16)
                        for (int y = bmp.Height / 8; y < bmp.Height * 7 / 8; y += bmp.Height / 16)
                        {
                            AvgColor = bmp.GetPixel(x, y); //use avg as temporary storage

                            if (AvgColor.GetBrightness() > 0.35 && AvgColor.GetSaturation() > 0.35) //brightness treshold
                            {
                                r += AvgColor.R;
                                g += AvgColor.G;
                                b += AvgColor.B;
                                cnt++;
                            }
                        }

                if (cnt > 0) AvgColor = Color.FromArgb(r / cnt, g / cnt, b / cnt); //in case nothing went above brightness threshold
                else AvgColor = SystemColors.ActiveCaption;
            }

            //Invalidate picture box to force redrawing, just in case
            pictureBox.Invalidate();
            Apod.IsDownloading = false;
        }

        /// <summary>
        /// Enable or disable GUI/tray items depending on current API date
        /// </summary>
        private void SetupGuiWhenCompleted()
        {
            Log(MethodBase.GetCurrentMethod().Name);

            tabImage.Focus();

            //Set image title
            //tray icon won't take more that 63 chars
            trayIcon.Text = (Apod.Title.Length > 63) ? Apod.Title.Substring(0, 63) : Apod.Title;
            labelImageDesc.Text = Apod.Title;

            //Copyright is not always there
            if (Apod.Copyright != string.Empty)
                labelImageDesc.Text += Environment.NewLine + "© " + Apod.Copyright.Replace("\n", " ");

            //Tray ballon
            trayIcon.BalloonTipTitle = "NASA Astronomy Picture of the Day";
            trayIcon.BalloonTipText = Apod.Title + ((Apod.IsImage) ? "" : " (non-image)");
            if (trayIcon.BalloonTipText != string.Empty)
                trayIcon.ShowBalloonTip(1);

            //Image description and URL
            textBoxImgDesc.Text = Apod.Explanation;
            if (Apod.HdUrl != string.Empty)
                textURL.Text = Apod.HdUrl;
            else if (Apod.Url != string.Empty)
                textURL.Text = Apod.Url;
            else
                textURL.Text = string.Empty;
            buttonCopyLink.Enabled = (textURL.Text != string.Empty);
            if (!Apod.IsImage)
            {
                buttonCopyImage.Enabled = false;
                progressBar.Value = 100;
                statusBar.Text = "Done!";
            }
            else
                buttonCopyImage.Enabled = true;
            this.Text = "NASA Astronomy Picture of the Day - " +
                Apod.ApiDate.ToShortDateString() +
                ((Apod.Title != string.Empty) ? " - " + Apod.Title : string.Empty);
            textDate.ForeColor = SystemColors.ControlText;
            textDate.Text = Apod.ApiDate.ToShortDateString();

            buttonPickDate.Enabled = true;

            trayIcon.ContextMenu.MenuItems["menuToday"].Enabled = true;
            trayIcon.ContextMenu.MenuItems["menuExit"].Enabled = true;

            //Enable/disable 'previous' and 'next' buttons, depending on the API date
            //TODAY - previous enabled, today enabled, next disabled
            if (Apod.ApiDate == DateTime.Today)
            {
                //Window buttons
                buttonPrev.Enabled = true;
                buttonToday.Enabled = false;
                buttonNext.Text = "NEXT >>"; //←→►◄
                buttonNext.Enabled = false;
                buttonRefresh.Enabled = true;

                //Try to get previous title from api
                setupMenuItem("menuPrev", Apod.ApiDate.AddDays(-1));
                trayIcon.ContextMenu.MenuItems["menuNext"].Enabled = false;
                trayIcon.ContextMenu.MenuItems["menuNext"].Text = "Next - not available";
                trayIcon.ContextMenu.MenuItems["menuToday"].Text = string.Join(" - ", "Today", Apod.Date, Apod.Title);
            }
            else
            //EARLIER - all enabled, or disable previous if minimum date reached
            {
                //Window buttons
                if (Apod.ApiDate <= DateTime.Parse("1995-06-16"))
                    buttonPrev.Enabled = false;
                else
                    buttonPrev.Enabled = true;

                buttonToday.Enabled = true;
                buttonNext.Enabled = true;
                buttonRefresh.Enabled = true;

                //Try to get today, previous and next title from api
                //PREVIOUS
                if (Apod.ApiDate <= DateTime.Parse("1995-06-16"))
                {
                    trayIcon.ContextMenu.MenuItems["menuPrev"].Enabled = false;
                    trayIcon.ContextMenu.MenuItems["menuPrev"].Text = "Previous";
                }
                else
                {
                    setupMenuItem("menuPrev", Apod.ApiDate.AddDays(-1));
                }
                //NEXT
                setupMenuItem("menuNext", Apod.ApiDate.AddDays(1));
                //TODAY
                setupMenuItem("menuToday", DateTime.Today);
            }

            //Setup menu item label with api title for desired date
            void setupMenuItem(string menuItem, DateTime date)
            {
                try
                {
                    using APOD a = new(date);
                    trayIcon.ContextMenu.MenuItems[menuItem].Enabled = true;
                    trayIcon.ContextMenu.MenuItems[menuItem].Text =
                        string.Join(" - ", menuItem.Substring(4), a.Date, a.Title + ((a.IsImage) ? "" : " (video)"));
                }
                catch (Exception)
                {
                    trayIcon.ContextMenu.MenuItems[menuItem].Enabled = false;
                    trayIcon.ContextMenu.MenuItems[menuItem].Text = menuItem.Substring(4) + " - not available";
                    switch (menuItem)
                    {
                        case "menuPrev":
                            break;
                        case "menuToday":
                            break;
                        case "menuNext":
                            break;
                        default:
                            break;
                    }
                    //statusBar.Text = e.Message; //not really necessary to display this
                }
            }
        }

        /// <summary>
        /// Save current image to disk
        /// </summary>
        private void SaveToDisk()
        {
            Log(MethodBase.GetCurrentMethod().Name);

            //First build custom path if desired and possible
            if (PathToSave != string.Empty && Apod.HdUrl != string.Empty)
                ImagePath = CreateFullPath(PathToSave, Apod);

            //Do actual saving
            try
            {
                pictureBox.Image.Save(ImagePath);
            }
            catch (Exception e)
            {
                Log(e.Message);
                statusBar.Text = e.Message;
            }
        }

        //Timer event handler - reload image with today date
        private void TimerRefresh_Tick(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);
            Log("--- TIMER TICK! ---");

            //Reload image only if today date has changed
            if (DateTime.Today != Apod.ApiDate)
            {
                Log("AUTO REFRESH - date rolled over");
                Log("API date   = " + Apod.ApiDate);
                Log("TODAY date = " + DateTime.Today);
                SetApodDate(Apod, DateTime.Today);
                GetNasaApod();
            }
            else
                Log("Date not changed, nothing to do in auto-refresh end");
        }

        //Auto refresh checkbox - enable or disable automatic refresh
        private void CheckAutoRefresh_CheckedChanged(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            if (checkAutoRefresh.Checked)
                timerRefresh.Enabled = true;
            else
                timerRefresh.Enabled = false;

            IniFile.Write("autoRefresh", checkAutoRefresh.Checked.ToString());
        }

        //Save to custom path checkbox
        private void CheckSaveToDisk_CheckedChanged(object s, EventArgs e)
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
                    ButtonPath_Click(this, e); //invoke path selection dialog
                    if (dialogPath.SelectedPath == string.Empty) //if dialog cancelled, revert UI changes
                    {
                        buttonPath.Enabled = false;
                        textPath.Enabled = false;
                        checkSaveToDisk.Checked = false;
                    }
                }
                else
                    PathToSave = textPath.Text;
            }
            else //save to disk disabled
            {
                buttonPath.Enabled = false;
                textPath.Enabled = false;
                PathToSave = string.Empty;
                ImagePath = @".\temp.jpg";
            }

            IniFile.Write("saveToDisk", checkSaveToDisk.Checked.ToString());
            IniFile.Write("pathToSave", PathToSave);
        }

        //Custom path selection button
        private void ButtonPath_Click(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            dialogPath.ShowDialog(); //display path selection dialog

            if (dialogPath.SelectedPath != string.Empty)
            {
                PathToSave = dialogPath.SelectedPath; //save the path
                textPath.Text = PathToSave; //and display it in path text box
                IniFile.Write("saveToDisk", checkSaveToDisk.Checked.ToString());
                IniFile.Write("pathToSave", PathToSave);
            }
        }

        //Copy link to clipboard
        private void ButtonCopyLink_Click(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            if (textURL.Text != string.Empty)
                Clipboard.SetText(textURL.Text);
        }

        //Copy image to clipboard
        private void ButtonCopyImage_Click(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            if (pictureBox.Image != null)
                Clipboard.SetImage(pictureBox.Image);
        }

        //Tray icon click - hide/show window
        private void MyIcon_MouseClick(object s, MouseEventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            //Left click only
            if (e.Button == MouseButtons.Left)
                //Toggle window state
                if (FormHidden)
                {
                    Show();
                    WindowState = FormWindowState.Normal;
                    FormHidden = false;
                }
                else
                {
                    Hide();
                    FormHidden = true;
                }
        }

        //Minimize to system tray
        private void WindowResize(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                FormHidden = true;
                trayIcon.BalloonTipTitle = "NASA Astronomy Picture of the Day";
                trayIcon.BalloonTipText = (trayIcon.Text == string.Empty) ? "No image today :(" : trayIcon.Text;
                trayIcon.ShowBalloonTip(1);
            }
        }

        //Previous button click
        private void ButtonPrev_Click(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            SetApodDate(Apod, Apod.ApiDate.AddDays(-1));
            GetNasaApod();
        }

        //Next button click
        private void ButtonNext_Click(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            if (Apod.ApiDate < DateTime.Today)
            {
                SetApodDate(Apod, Apod.ApiDate.AddDays(1));
                GetNasaApod();
            }
        }

        //Today button click
        private void ButtonToday_Click(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            SetApodDate(Apod, DateTime.Today);
            GetNasaApod();
        }

        //Refresh button - simply reload current image
        private void ButtonRefresh_Click(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            SetApodDate(Apod, Apod.ApiDate);
            GetNasaApod();
        }

        //EXPERIMANTAL - draw the title over the picture
        //private void pictureBox_Paint(object s, PaintEventArgs e)
        //{
        //    Log(MethodBase.GetCurrentMethod().Name);

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
        //}

        //Pick a date - show calendar
        private void ButtonPickDate_Click(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            if (!Calendar.Visible)
            {
                Calendar.ShowTodayCircle = true;
                Calendar.BringToFront();
                Calendar.SetDate(Apod.ApiDate);
                Calendar.Visible = true;
            }
            else
                Calendar.Visible = false;
        }

        //Calendar click - set the date and get the image at once
        private void Calendar_DateSelected(object s, DateRangeEventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            Calendar.Visible = false;
            if (Calendar.SelectionStart > DateTime.Today) //don't allow future date
                Calendar.SelectionStart = DateTime.Today;
            SetApodDate(Apod, Calendar.SelectionStart);
            GetNasaApod();
        }

        //Just move your mouse over the description to be able to scroll it
        private void TextBoxImgDesc_MouseHover(object s, EventArgs e)
        {
            //Log(MethodBase.GetCurrentMethod().Name);

            textBoxImgDesc.Focus();
            textBoxImgDesc.Select(0, 0);
        }

        //Go to 'how to' link
        private void LinkHowToKey_LinkClicked(object s, LinkLabelLinkClickedEventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            Process.Start(e.Link.LinkData as string);
        }

        //Use custom key checkbox
        private void CheckCustomKey_CheckedChanged(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            if (checkCustomKey.Checked)
                textCustomKey.Enabled = true;
            else
                textCustomKey.Enabled = false;

            IniFile.Write("useCustomKey", checkCustomKey.Checked.ToString());
        }

        //Custom key value - save in INI file
        private void TextCustomKey_TextChanged(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            IniFile.Write("customKey", textCustomKey.Text);
        }

        //Go to history item
        private void ListHistory_DoubleClick(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            SetApodDate(Apod, DateTime.Parse(listHistory.SelectedItems[0].Text));
            GetNasaApod();
        }

        /// <summary>
        ///EXPERIMENTAL, NOT FULLY TESTED, HIGH POTENTIAL TO CRASH SOMEHOW
        ///Try to grab WHOLE apod archive
        ///TODO: create list of URLs first and then download everything in parallel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonGrabAll_Click(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            //Some initial values
            DaysProg = 0; //global download progress
            DaysErrors = 0;
            int _daysProg; //queueing progress (different that actual download progress!)

            //Create local apod object
            using APOD a = new();

            //List of download tasks
            List<System.Threading.Tasks.Task> tasks = new();

            a.SetApiKey(textCustomKey.Text);

            //Set some GUI items
            buttonGrabAll.Enabled = false;
            timerRefresh.Enabled = false;
            progressBar.Value = 0;
            statusBar.Text = "Grabbing the archive...";
            textGrabAll.Text = "Creating download queue...";
            textGrabAll.Invalidate();
            progressBar.Invalidate();
            Application.DoEvents();

            //Save files in subdir of current folder
            string _dir = Directory.CreateDirectory("APOD_ARCHIVE").Name;

            //Go from today (progress = 0) to minimum date (progress = span)
            Log("GRAB WHOLE ARCHIVE - main loop start ----------------------------");
            for (_daysProg = 0; _daysProg < DaysSpan; _daysProg++)
            {
                //Some tasks may have completed already, remove them from task list
                Log("Number of download tasks = " + tasks.Count);
                while (tasks.Count > 20)
                    foreach (System.Threading.Tasks.Task t in tasks)
                        if (t.IsCompleted)
                        {
                            Log("Removing completed task: " + t.ToString());
                            tasks.Remove(t);
                            break;
                        }

                //Set API date according to progress (starting with zero increase)
                try
                {
                    Log("Setting api date to " + DateMin.AddDays(_daysProg).ToShortDateString());
                    a.SetApiDate(DateMin.AddDays(_daysProg));
                }
                catch (Exception ex)
                {
                    Log("Error setting date to " + DateMin.AddDays(_daysProg).ToShortDateString());
                    Log("msg = " + ex.Message);
                }

                //Try to download actual image
                if (a.IsImage)
                {
                    //Image name begin and length - to create final filename
                    int begin = a.HdUrl.LastIndexOf('/') + 1;
                    int len = a.HdUrl.Length - begin;

                    //Double check image URL, if HD not available try regular
                    if (a.HdUrl != string.Empty)
                    {
                        Log("Trying hd url " + a.HdUrl);
                        tasks.Add(DownloadAsync(a.HdUrl,
                            _dir + '\\' + a.ApiDate.ToShortDateString().Replace(' ', '_') + '_' +
                            a.HdUrl.Substring(begin, len)));
                    }
                    else if (a.Url != string.Empty)
                    {
                        Log("Trying regular url " + a.Url);
                        tasks.Add(DownloadAsync(a.Url,
                            _dir + '\\' + a.ApiDate.ToShortDateString().Replace(' ', '_') + '_' +
                            a.Url.Substring(begin, len)));
                    }
                }
                else
                {
                    Log("Date set to " + DateMin.AddDays(_daysProg).ToShortDateString() +
                        ", no image found");// + a.media_type + ")") ;
                    DaysErrors++;
                }

                textGrabAll.Text = "Queueing " + a.ApiDate.ToShortDateString() + " (" +
                    _daysProg * 100 / DaysSpan + "%, " +
                    (DaysSpan - _daysProg) + " left)";
                textDate.Text = a.ApiDate.ToShortDateString();
                textGrabAll.Invalidate();
                Application.DoEvents();
            }

            //Wait for remaining download tasks to finish
            Log("GRAB WHOLE ARCHIVE - main loop stop ----------------------------");
            tasks.Clear();

            buttonGrabAll.Enabled = true;
            timerRefresh.Enabled = true;

            Application.DoEvents();
        }

        /// <summary>
        /// Asynchronous download task used to grab whole archive of images
        /// </summary>
        /// <param name="url">Image url</param>
        /// <param name="path">Image path</param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task DownloadAsync(string url, string path)
        {
            using WebClient _wc = new();
            try
            {
                Log("Trying to download " + url + "...");
                _wc.DownloadFileCompleted += WcDownloadFileCompleted;
                await _wc.DownloadFileTaskAsync(url, path);
                Log("Download succesful " + url + "...");
            }
            catch (Exception e)
            {
                Log("Error downloading " + url);
                Log("msg = " + e.Message);
            }
        }

        // Update GUI when single donwload thread is completed
        private void WcDownloadFileCompleted(object s, AsyncCompletedEventArgs e)
        {
            DaysProg++;

            if (DaysProg < DaysSpan)
                textGrabAll.Text = "Downloading... " +
                    DaysProg * 100 / DaysSpan + "%, " +
                    (DaysSpan - DaysProg) + " left";
            else
            {
                textGrabAll.Text = "Download complete! " +
                    "(" + DaysProg + " downloaded, " +
                    DaysErrors + " errors)";
                statusBar.Text = textGrabAll.Text;
            }

            progressBar.Value = DaysProg * 100 / DaysSpan;
            textGrabAll.Invalidate();
            progressBar.Invalidate();
            Application.DoEvents();
        }

        //Enable or disable history function
        private void CheckEnableHistory_CheckedChanged(object s, EventArgs e)
        {
            if (checkEnableHistory.Checked)
            {
                IniFile.Write("enableHistory", "True");
                tabControl.TabPages.Add(tabHistory);
                if (Apod != null) //fill history tab only if apod object has been initialized
                    FillHistory();
            }
            else
            {
                IniFile.Write("enableHistory", "False");
                tabControl.TabPages.Remove(tabHistory);
            }
        }

        //Custom menu item drawing - measure the area
        private void MenuItemMeasure(object ClickedItem, MeasureItemEventArgs e)
        {
            //Use bold font for default menu item and regular one for other items
            //(works as long as system default menu font is not bold ;))
            using Font f = (ClickedItem as MenuItem).DefaultItem ?   //is it default
                new(SystemFonts.MenuFont, FontStyle.Bold) :         //yes it is, use bold font
                SystemFonts.MenuFont;                              //no, use default font
            SizeF sz = e.Graphics.MeasureString((ClickedItem as MenuItem).Text, f);

            e.ItemWidth = (int)(1.1 * sz.Width);
            e.ItemHeight = (int)(1.30 * sz.Height);
        }

        //Custom menu item drawing - draw item
        private void MenuItemDraw(object ClickedItem, DrawItemEventArgs e)
        {
            //Bold font for default menu item, regular font for other items
            using Font f = (ClickedItem as MenuItem).DefaultItem ?
                new(SystemFonts.MenuFont, FontStyle.Bold) :
                SystemFonts.MenuFont;
            //Draw backgrounds - mouse over...
            if ((e.State & DrawItemState.Selected) != DrawItemState.None)
                //Distinguish between enabled and disabled items
                if ((ClickedItem as MenuItem).Enabled)
                {
                    //Horizontal gradient and outside box
                    e.Graphics.FillRectangle(new LinearGradientBrush(e.Bounds, AvgColor, SystemColors.Control, 0.0), e.Bounds);
                    e.Graphics.DrawRectangle(SystemPens.ControlDark, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
                }
                else { }
            //...and mouse out
            else
            {
                //Clear gradient and box with "control" color so they disappear
                e.Graphics.FillRectangle(SystemBrushes.Control, e.Bounds);
                e.Graphics.DrawRectangle(SystemPens.Control, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
            }

            //Distinguish between enabled and disabled items
            using Brush b = (ClickedItem as MenuItem).Enabled ?
                new SolidBrush(SystemColors.ControlText) :
                new SolidBrush(SystemColors.GrayText);
            //Finally, draw menu item text
            e.Graphics.DrawString((ClickedItem as MenuItem).Text, f, b,
            e.Bounds.X + e.Graphics.MeasureString("-", f).Width,
            e.Bounds.Y + (e.Bounds.Height - e.Graphics.MeasureString((ClickedItem as MenuItem).Text, f).Height) / 2);
        }

        //Define keyboard shortcuts here
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            if (!Apod.IsDownloading)
                switch (keyData)
                {
                    case Keys.F5: //refresh
                        ButtonRefresh_Click(this, null);
                        return true;
                    case Keys.Left: //minus one day
                        ButtonPrev_Click(this, null);
                        return true;
                    case Keys.Right: //plus one day
                        ButtonNext_Click(this, null);
                        return true;
                    case Keys.Control | Keys.Tab:
                        tabControl.SelectedIndex = (tabControl.SelectedIndex == tabControl.TabCount - 1) ? 0 : tabControl.SelectedIndex + 1;
                        return true;
                    case Keys.Control | Keys.Shift | Keys.Tab:
                        tabControl.SelectedIndex = (tabControl.SelectedIndex == 0) ? tabControl.TabCount - 1 : tabControl.SelectedIndex - 1;
                        return true;
                    default:
                        break;
                }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        //Simple window snapping
        protected override void OnResizeEnd(EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            base.OnResizeEnd(e);

            Screen s = Screen.FromPoint(Location);

            if (Snap(Left, s.WorkingArea.Left)) Left = s.WorkingArea.Left;
            if (Snap(Top, s.WorkingArea.Top)) Top = s.WorkingArea.Top;
            if (Snap(s.WorkingArea.Right, Right)) Left = s.WorkingArea.Right - Width;
            if (Snap(s.WorkingArea.Bottom, Bottom)) Top = s.WorkingArea.Bottom - Height;

            static bool Snap(int pos, int edge) =>
                pos - edge > 0 && pos - edge <= 25;
        }

        //Toggle startup run
        private void CheckRunAtStartup_CheckedChanged(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);

            using TaskCollection t = new TaskService().RootFolder.GetTasks(new("NASA_APOD"));
            if (!(s as CheckBox).Checked)
                t["NASA_APOD"].Enabled = false;
            else
                if (t.Count > 0)
                t["NASA_APOD"].Enabled = true;
            else
                CreateStartupTask();
        }

        //Create startup scheduled task
        private void CreateStartupTask()
        {
            Log(MethodBase.GetCurrentMethod().Name);

            using TaskDefinition definition = new TaskService().NewTask();

            definition.RegistrationInfo.Description = "NASA APOD STARTUP TASK";
            definition.RegistrationInfo.Author = WindowsIdentity.GetCurrent().Name;

            definition.Triggers.Add<Trigger>(new LogonTrigger
            {
                Enabled = true,
                UserId = WindowsIdentity.GetCurrent().Name
            });

            definition.Actions.Add(new ExecAction
            {
                Path = Assembly.GetEntryAssembly().Location,
                WorkingDirectory = Assembly.GetEntryAssembly().Location.Substring(0, Assembly.GetEntryAssembly().Location.LastIndexOf("\\"))
            });

            definition.Settings.DisallowStartIfOnBatteries = false;
            definition.Settings.StopIfGoingOnBatteries = false;
            definition.Settings.ExecutionTimeLimit = TimeSpan.Zero;

            _ = new TaskService().RootFolder.RegisterTaskDefinition("NASA_APOD", definition);
        }

        private void checkStartMinimized_CheckedChanged(object s, EventArgs e)
        {
            Log(MethodBase.GetCurrentMethod().Name);
        }
    }
}
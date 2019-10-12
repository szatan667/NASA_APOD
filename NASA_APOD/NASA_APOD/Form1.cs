using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NASA_APOD
{
    public partial class MainWindow : Form
    {
        public string pathToSave; //will hold path from folder dialog
        public string apodPath = "temp.jpg"; //default file to save
        public string baseURL = "https://apod.nasa.gov/apod/";
        public string apiURL = "https://api.nasa.gov/planetary/apod?api_key=DFihYXvddhhd1KnnPtw3BgSxAXlx9yHz1CSTwbN8";
        //public string finalURL = string.Empty; //final URL of image file
        public string prevImgURL = string.Empty; //previous final URL -- if same as current, will not be refreshed automatically

        public WebClient wc = new WebClient();

        //public Match prevURL; //regex matches for previous and next day URLs
        //public Match nextURL;

        //string html = string.Empty; //container for html file

        ContextMenu myIconMenu; //context menu for tray icon
        bool hidden = false;    //main form state - hidden or not

        //setup ini file to store usage statistics
        IniFile iniFile = new IniFile();

        //WALLPAPER
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(
                        int uAction, int uParam,
                        string lpvParam, int fuWinIni);
        public const int SPI_SETDESKWALLPAPER = 20;
        public const int SPIF_SENDCHANGE = 0x2;
        //WALLPAPER END

        public MainWindow()
        {
            InitializeComponent();

            //read anything from INI file to check if it was already created
            //if not, write initial zero number of runs
            var lastURL = iniFile.Read("lastURL");

            if (lastURL == String.Empty) //write default values to INI file
            {
                iniFile.Write("lastURL", "https://apod.nasa.gov/apod/");
                iniFile.Write("customPath", false.ToString());
            }
            else
            {
                if (lastURL != textDefaultURL.Text)
                    buttonToday.Enabled = true;
                textDefaultURL.Text = lastURL;
            }

            //add simple tray icon with "refresh" menu
            myIconMenu = new ContextMenu();
            myIconMenu.MenuItems.Add("Previous", buttonPrev_Click);
            myIconMenu.MenuItems.Add("Next", buttonNext_Click);
            myIconMenu.MenuItems.Add("-");
            myIconMenu.MenuItems.Add("Refresh", OnMenuRefresh);
            myIconMenu.MenuItems.Add("Exit", OnMenuExit);
            myIcon.Icon = SystemIcons.Asterisk;
            myIcon.ContextMenu = myIconMenu;
            statusBar.Text = "Ready!";

            //this.WindowState = FormWindowState.Minimized;
            //this.Hide();
            getNASAApod(); //get the image on startup
        }

        //Context menu "refresh" event handler
        private void OnMenuRefresh(object sender, EventArgs e)
        {
            getNASAApod();
        }
        //Context menu "exit" event handler
        private void OnMenuExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //Main logic procedure
        private void getNASAApod()
        {
            //some initial values
            //System.Uri apodURL = new System.Uri(textDefaultURL.Text); //new url for apod pic
            wc.DownloadProgressChanged += Wc_DownloadProgressChanged; //progress bar when downloading

            statusBar.Text = "Getting NASA picture of the day...";
            myIcon.Text = statusBar.Text;

            //get json using NASA API
            try
            {
                string json = wc.DownloadString(apiURL);

                //simple brute-force json parsing
                string mediaType = jsonGetSingle(wc.DownloadString(apiURL), "media_type");
                string imgTitle = jsonGetSingle(wc.DownloadString(apiURL), "title");
                string imgDesc = jsonGetSingle(wc.DownloadString(apiURL), "explanation");
                string imgURL = jsonGetSingle(wc.DownloadString(apiURL), "url");
                string imgURLHD = jsonGetSingle(wc.DownloadString(apiURL), "hdurl");

                //if custom path not entered, put default (system) TEMP path
                //if (pathToSave == null)
                //    pathToSave = System.Environment.GetEnvironmentVariable("TEMP");

                //download the picture
                //but first check if custom path for storing files was selected
                if (pathToSave != null && imgURLHD != String.Empty) //custom path found, concatenate file name
                {
                    int begin = imgURLHD.LastIndexOf('/') + 1;
                    int end = imgURLHD.Length;
                    apodPath = pathToSave + "\\" + imgURLHD.Substring(begin, end); //concatenate path with filename
                }

                if (mediaType != "image")
                {
                    statusBar.Text = "Not a picture?"; //nothing downloaded, just show some text
                    myIcon.Text = statusBar.Text;
                    myIcon.BalloonTipTitle = "Error getting the image";
                    myIcon.BalloonTipText = statusBar.Text;
                    myIcon.ShowBalloonTip(5);
                }
                else
                {
                    //print current image URL
                    textURL.Text = imgURLHD;

                    //save current URL as last processed URL in INI file
                    iniFile.Write("lastURL", imgURLHD);

                    //download image file, save the file to temp or destination path
                    //download only if current URL is different than previous one
                    //(auto-refresh the image only if it has changed)
                    if (imgURLHD != prevImgURL)
                    {
                        prevImgURL = imgURLHD;
                        wc.DownloadFileAsync(
                            new System.Uri(imgURLHD),
                            apodPath);
                    }
                    else
                    {
                        statusBar.Text = string.Empty;
                        myIcon.Text = statusBar.Text;
                    }

                    //put the image to img box (done inside event handler)
                    wc.DownloadFileCompleted += Wc_DownloadFileCompleted;

                    //setup image title and description box
                    myIcon.Text = imgTitle;
                    label1.Text = imgTitle;
                    myIcon.BalloonTipTitle = "NASA Astronomy Picture of the Day";
                    myIcon.BalloonTipText = imgTitle;
                    myIcon.ShowBalloonTip(5);
                    textBoxImgDesc.Text = imgDesc;
                }

                //now try to parse next and previous links
                //prevURL = Regex.Match(html, "<a href.*&lt;");
                //nextURL = Regex.Match(html, "<a href.*&gt;");
                //if (prevURL.Value != null || prevURL.Value != string.Empty)
                //    buttonPrev.Enabled = true;
                //if (nextURL.Value != null || nextURL.Value != string.Empty)
                //    buttonNext.Enabled = true;
            }
            catch (WebException e)
            {
                statusBar.Text = e.Message;
            }
            catch (Exception e)
            {
                statusBar.Text = e.Message;
            }
        }
        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            myIcon.Text = string.Empty;

            //download complete, display the image in the image box
            long fileLength = new FileInfo(apodPath).Length;

            if (fileLength != 0)
            {
                pictureBox.ImageLocation = apodPath;
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox.Visible = true;

                //and finally, set it as wallpaper
                //build wallpaper file path
                if (!checkSaveToDisk.Checked)
                {
                    apodPath = System.Reflection.Assembly.GetEntryAssembly().Location.Substring(0,
                                System.Reflection.Assembly.GetEntryAssembly().Location.LastIndexOf("\\") + 1)
                                + "temp.jpg";
                }

                //do actual wallpapering
                SystemParametersInfo(SPI_SETDESKWALLPAPER,
                    1,
                    apodPath,
                    SPIF_SENDCHANGE);

                statusBar.Text = "Done!";

                pictureBox.Invalidate();
            }
            else
            {
                pictureBox.Visible = false;
                statusBar.Text = "Couldn't parse actual picture :(";
            }
        }
        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //update download progress bar
            progressBar.Value = e.ProgressPercentage;
        }

        private void buttonRefresh_Click(object sender, System.EventArgs e)
        {
            getNASAApod();
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            getNASAApod();
        }

        //Auto refresh every hour checkbox
        private void checkBox2_CheckedChanged(object sender, System.EventArgs e)
        {
            if (checkAutoRefresh.Checked)
            {
                timer1.Enabled = true;
                textDefaultURL.Text = baseURL;
            }
            else
            {
                timer1.Enabled = false;
            }
        }

        //Save to custom path checkbox
        private void checkSaveToDisk_CheckedChanged(object sender, System.EventArgs e)
        {
            if (checkSaveToDisk.Checked) //custom path checked
            {
                buttonPath.Enabled = true; //enable path selection button [...]
                textPath.Enabled = true;   //enable custom path text box
                if (textPath.Text == null || textPath.Text == string.Empty) //first click, no custom path saved
                {
                    buttonPath_Click(this, e); //invoke path selection dialog
                    if (dialogPath.SelectedPath == string.Empty) //if dialog cancelled, revert UI changes
                    {
                        buttonPath.Enabled = false;
                        textPath.Enabled = false;
                        checkSaveToDisk.Checked = false;
                    }
                }
            }
            else //disable custom path
            {
                buttonPath.Enabled = false;
                textPath.Enabled = false;
                //textPath.Text = null;
                pathToSave = null;
                apodPath = "temp.jpg";
            }
        }

        //Custom path selection button
        private void buttonPath_Click(object sender, System.EventArgs e)
        {
            dialogPath.ShowDialog(); //display path selection dialog
            pathToSave = dialogPath.SelectedPath; //save the path
            textPath.Text = pathToSave; //and display it in path text box
        }

        //Default URL check box
        private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
        {
            if (checkDefaultURL.Checked)
            {
                textDefaultURL.Enabled = true;
            }
            else
            {
                textDefaultURL.Enabled = false;
            }
        }

        //Previous button
        private void buttonPrev_Click(object sender, System.EventArgs e)
        {
            //parse actual html file of previous link
            //textDefaultURL.Text = baseURL + prevURL.Value.Substring(9, prevURL.Value.Length - 15);
            //getNASAApod();
            //buttonToday.Enabled = true;
        }

        //Next button
        private void buttonNext_Click(object sender, System.EventArgs e)
        {
            //parse actual html file of previous link
            //textDefaultURL.Text = baseURL + nextURL.Value.Substring(9, nextURL.Value.Length - 15);
            //getNASAApod();
            //buttonToday.Enabled = true;
        }

        //Copy link to clipboard
        private void buttonCopyLink_Click(object sender, System.EventArgs e)
        {
            if (textURL.Text != string.Empty) Clipboard.SetText(textURL.Text);
        }

        //Copy image to clipboard
        private void buttonCopyImage_Click(object sender, System.EventArgs e)
        {
            if (pictureBox.Image != null) Clipboard.SetImage(pictureBox.Image);
        }

        //Tray icon double-click - hide/show window
        private void myIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
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
            if (this.WindowState == FormWindowState.Minimized)
            { this.Hide();
                hidden = true;
                myIcon.BalloonTipTitle = "NASA Astronomy Picture of the Day";
                myIcon.BalloonTipText = myIcon.Text;
                myIcon.ShowBalloonTip(1); }
        }

        private void buttonToday_Click(object sender, EventArgs e)
        {
            textDefaultURL.Text = baseURL;
            getNASAApod();
            buttonNext.Enabled = false;
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {/*
            //get picture box size
            Size picSize = pictureBox.Size;

            //find maximum font size for current picture box size
            //(it will be ususally pic box width that determines max size)

            //initial font - consolas bold, 1pt
            int fs = 1;
            Font myFont = new Font("Consolas", fs, FontStyle.Bold);
            Size textSize;

            //now loop over font sizes
            do
            {
                myFont = new Font("Consolas", ++fs, FontStyle.Bold);
                textSize = TextRenderer.MeasureText(myIcon.Text, myFont);
            }
            //stop when text size is bigger than picture box size
            while (textSize.Width < picSize.Width &&
                textSize.Height < picSize.Height);
            myFont = new Font("Consolas", --fs, FontStyle.Bold);

            //draw text, center it using it's own size
            //(half image widht and height, then subtract half of text's widht and height)
            e.Graphics.DrawString(myIcon.Text,  //text to show 
                myFont,                         //font to use
                Brushes.Yellow,                 //text color (brush)
                pictureBox.Width/2 -            //text X position
                TextRenderer.MeasureText(myIcon.Text, myFont).Width / 2, 
                pictureBox.Height/2 -           //text Y position
                TextRenderer.MeasureText(myIcon.Text, myFont).Height / 2);
            myFont.Dispose();   //release font, don't need it anymore
            */
        }

        private String jsonGetSingle(String json, String key)
        {
            string _key = '"' + key + '"';
            if (json.Contains(_key))
            {
                int keyStartPos = json.IndexOf(_key);
                int valueStartPos = keyStartPos + _key.Length + 2;
                int valueLength = json.IndexOf('"', valueStartPos);
                String outstr = json.Substring(valueStartPos, valueLength - valueStartPos);
                return outstr;
            }
            else return null;
        }
    }
}
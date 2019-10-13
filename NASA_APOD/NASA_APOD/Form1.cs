﻿using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        public const int SPI_SETDESKWALLPAPER = 20;
        public const int SPIF_SENDCHANGE = 0x2;

        //--- Main class methods -----------------------------------------------------------------

        //Default constructor - will create program window and set everything up
        public MainWindow()
        {
            InitializeComponent();

            //read anything from INI file to check if it was already created
            //if not, write initial zero number of runs
            var lastDate = iniFile.Read("lastDate");

            if (lastDate == String.Empty) //write default values to INI file
            {
                iniFile.Write("lastDate", DateTime.Today.ToString());
                iniFile.Write("saveToDisk", checkSaveToDisk.Checked.ToString());
                iniFile.Write("pathToSave", string.Empty);
            }

            //Interface items
            labelImageDesc.Text = string.Empty;
            myIconMenu = new ContextMenu();
            myIconMenu.MenuItems.Add("Previous", buttonPrev_Click);
            myIconMenu.MenuItems.Add("Next", buttonNext_Click);
            myIconMenu.MenuItems.Add("-");
            myIconMenu.MenuItems.Add("Refresh", OnMenuRefresh);
            myIconMenu.MenuItems.Add("Exit", OnMenuExit);
            myIcon.Icon = SystemIcons.Asterisk;
            myIcon.ContextMenu = myIconMenu;
            pathToSave = iniFile.Read("pathToSave");
            textPath.Text = pathToSave;
            if (iniFile.Read("saveToDIsk") == "True")
                checkSaveToDisk.Checked = true;
            else
                checkSaveToDisk.Checked = false;
            statusBar.Text = "Ready!";

            //Create 'photo of the day' object and for starters set API date to today
            apod = new APOD();
            setAPIDate(DateTime.Parse(iniFile.Read("lastDate")));

            //Get the image on startup
            getNASAApod();
        }

        //Set current date for API - create URL and get json
        private void setAPIDate(DateTime datetime)
        {
            try
            {
                apod.setAPIDate(datetime);
                textPickDate.Text = apod.apiDate.ToString().Substring(0, 10);
            }
            catch (Exception e)
            {
                statusBar.Text = e.Message;
                textPickDate.Text = apod.apiDate.ToString().Substring(0, 10);
                apod.media_type = null;
            }
        }

        //Fill combo box with current date and 7 days back and ahead
        private void fillDatesCombo(DateTime datetime)
        {
        }

        //Context menu "refresh" event handler
        private void OnMenuRefresh(object sender, EventArgs e)
        {
            getNASAApod(); //don't change anything, just get the pic with currently set date
        }
        
        //Tray icon menu "exit" event handler
        private void OnMenuExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //Get the picture and possibly save it to disk (if required in GUI)
        private void getNASAApod()
        {
            statusBar.Text = "Getting NASA picture of the day...";
            myIcon.Text = statusBar.Text;
            buttonPrev.Enabled = false;
            buttonToday.Enabled = false;
            buttonNext.Enabled = false;
            buttonRefresh.Enabled = false;

            //Download image to picture box
            try
            {
                //Get it from apod url, only if it's an image
                //(sometimes they post videos)
                if (apod.media_type == "image")
                {
                    //Subscribe to "download progress" and "download completed" events before starting to download
                    pictureBox.LoadProgressChanged += PictureBox_LoadProgressChanged;
                    pictureBox.LoadCompleted += PictureBox_LoadCompleted;

                    //Download the image directly to image box
                    //"Image" property will allow to save it later
                    pictureBox.LoadAsync(apod.hdurl);
                }
                else
                {
                    //Not a picture today, clear the image and show some text
                    pictureBox.Image = null;
                    statusBar.Text = "Sorry, no picture today.";
                    myIcon.Text = statusBar.Text;
                    myIcon.BalloonTipText = statusBar.Text;
                    myIcon.ShowBalloonTip(5);
                    //Enable/disable 'previous' and 'next' buttons, depending on the API date
                    setupButtons();
                }
            }
            //Download errors
            catch (WebException e)
            {
                statusBar.Text = e.Message;
            }
            catch (Exception e)
            {
                statusBar.Text = e.Message;
            }
        }

        //Download progress bar - event handler
        private void PictureBox_LoadProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        //Download completed event - do rest of the logic - actual wallpapering and saving to disk
        private void PictureBox_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Visible = true;

            //Save the image to disk - required to set the wallpaper
            //even if "save to disk" is not set i GUI
            saveToDisk();

            //Before setting the wallpaper, we have to build full path of TEMP.JPG
            //but only if not saving to custom path, because then it was already built
            if (!checkSaveToDisk.Checked)
                apodPath = System.Reflection.Assembly.GetEntryAssembly().Location.Substring(0,
                           System.Reflection.Assembly.GetEntryAssembly().Location.LastIndexOf("\\") + 1)
                         + "temp.jpg";

            //do actual wallpapering
            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                1,
                apodPath,
                SPIF_SENDCHANGE);

            //setup UI elements
            myIcon.Text = apod.title;
            labelImageDesc.Text = apod.title;
            myIcon.BalloonTipTitle = "NASA Astronomy Picture of the Day";
            myIcon.BalloonTipText = apod.title;
            myIcon.ShowBalloonTip(5);
            textBoxImgDesc.Text = apod.explanation;
            textURL.Text = apod.hdurl;
            statusBar.Text = "Done!";
            this.Text = "NASA Astronomy Picture of the Day - " + apod.date;

            //save current URL as last processed URL in INI file
            //TODO: image date would be more relevant, since we're using API now
            iniFile.Write("lastDate", apod.apiDate.ToString());

            //Enable/disable 'previous' and 'next' buttons, depending on the API date
            setupButtons();

            //Invalidate picture box to force redrawing, just in case
            pictureBox.Invalidate();
        }

        //Enable or disable prev/today/buttons depending on current API date
        private void setupButtons()
        {
            //Enable/disable 'previous' and 'next' buttons, depending on the API date
            //TODAY - previous enabled, today enabled, next disabled
            if (apod.apiDate == DateTime.Today)
            {
                buttonPrev.Text = "<< " + apod.apiDate.AddDays(-1).ToString().Substring(0, 10); ;
                buttonPrev.Enabled = true;
                buttonToday.Enabled = false;
                buttonNext.Enabled = false;
                buttonRefresh.Enabled = true;
            }
            else
            //EARLIER - all enabled
            {
                buttonPrev.Text = "<< " + apod.apiDate.AddDays(-1).ToString().Substring(0, 10);
                buttonPrev.Enabled = true;
                buttonToday.Enabled = true;
                buttonNext.Text = apod.apiDate.AddDays(1).ToString().Substring(0, 10) + " >>";
                buttonNext.Enabled = true;
                buttonRefresh.Enabled = true;
            }
        }

        //Save current image to disk
        private void saveToDisk()
        {
            //First build custom path if desired
            if ((pathToSave != null && pathToSave != string.Empty) && 
                (apod.hdurl != null && apod.hdurl != String.Empty)) //custom path found, concatenate path with image filename
            {
                int begin = apod.hdurl.LastIndexOf('/') + 1;
                int end = apod.hdurl.Length - begin;
                apodPath = pathToSave + "\\" + apod.hdurl.Substring(begin, end); //concatenate path with filename
            }

            //Do actual save
            try
            {
                pictureBox.Image.Save(apodPath);
            }
            catch (Exception e)
            {
                statusBar.Text = e.Message;
            }
        }

        //Timer event handler - reload image with today date
        private void timer1_Tick(object sender, System.EventArgs e)
        {
            setAPIDate(DateTime.Today);
            getNASAApod();
        }

        //Auto refresh checkbox - enable or disable automatic refresh
        private void checkAutoRefresh_CheckedChanged(object sender, System.EventArgs e)
        {
            if (checkAutoRefresh.Checked)
                timer1.Enabled = true;
            else
                timer1.Enabled = false;
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
        private void buttonPath_Click(object sender, System.EventArgs e)
        {
            dialogPath.ShowDialog(); //display path selection dialog
            pathToSave = dialogPath.SelectedPath; //save the path
            textPath.Text = pathToSave; //and display it in path text box
            iniFile.Write("saveToDisk", checkSaveToDisk.Checked.ToString());
            iniFile.Write("pathToSave", pathToSave);
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

        //Previous button click
        private void buttonPrev_Click(object sender, System.EventArgs e)
        {
            setAPIDate(apod.apiDate.AddDays(-1));
            getNASAApod();
        }

        //Next button click
        private void buttonNext_Click(object sender, System.EventArgs e)
        {
            //TODO: handle today date - should be unable to go to next day
            setAPIDate(apod.apiDate.AddDays(1));
            getNASAApod();
        }

        //Today button click
        private void buttonToday_Click(object sender, EventArgs e)
        {
            setAPIDate(DateTime.Today);
            getNASAApod();
        }

        //Refresh button - simply reload current image
        private void buttonRefresh_Click(object sender, System.EventArgs e)
        {
            setAPIDate(apod.apiDate); //just refresh json strings
            getNASAApod();
        }

        //EXPERIMANTAL - draw the title over the picture
        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {/*
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
        */}

        //Click on date text box - show calendar
        void textPickDate_Click(object sender, EventArgs e)
        {
            Calendar.ShowTodayCircle = true;
            Calendar.BringToFront();
            Calendar.Visible = true;
        }

        //Calendar click - set the date and get the image at once
        private void Calendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            Calendar.Visible = false;
            if (Calendar.SelectionStart > DateTime.Today)
                Calendar.SelectionStart = DateTime.Today;
            textPickDate.Text = Calendar.SelectionStart.ToString().Substring(0, 10);
            setAPIDate(Calendar.SelectionStart);
            getNASAApod();
        }
    }
}
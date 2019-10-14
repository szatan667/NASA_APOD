using System;
using System.Drawing;
using System.Net;

namespace NASA_APOD
{
    //Simple container for API fields - used to deserialize retrieved json
    public class APOD_API
    {
        //Json fields for NASA API
        public string copyright;
        public string date;
        public string explanation;
        public string hdurl;
        public string media_type;
        public string service_version;
        public string title;
        public string url;
    }

    public class APOD : APOD_API
    {
        //--- Public fields ---------------------------------------------------------
        public DateTime apiDate;
        public Image img, imghd;

        //--- Private fields --------------------------------------------------------
        private const string _baseURL = "https://api.nasa.gov/planetary/apod";
        private const string _apiKey = "DFihYXvddhhd1KnnPtw3BgSxAXlx9yHz1CSTwbN8";
        private WebClient _wc;

        //--- Default constructor ---------------------------------------------------
        public APOD()
        {
            //nothing special...
        }

        //--- Public methods --------------------------------------------------------

        //Set current date for API - create URL and get json
        public void setAPIDate(DateTime datetime)
        {
            if (datetime < DateTime.Parse("1995-06-16"))
                datetime = DateTime.Parse("1995-06-16");
            string _apiURL;

            //Create API URL
            _apiURL  = _baseURL;
            _apiURL += "?api_key=" + _apiKey;
            _apiURL += "&hd=true";
            _apiURL += "&date=" + datetime.Year + "-";
            if (datetime.Month < 10) _apiURL += "0" + datetime.Month + "-";
            else _apiURL += datetime.Month + "-";
            if (datetime.Day < 10) _apiURL += "0" + datetime.Day;
            else _apiURL += datetime.Day;

            //Call websvc and strip json to local vars
            try
            {
                apiDate = datetime; //set the date for APOD object
                _wc = new WebClient();
                jsonDeserialize(_wc.DownloadString(_apiURL));
                _wc.Dispose();
            }
            catch (Exception)
            {
                _wc.Dispose();
                copyright       = null;
                date            = null;
                explanation     = null;
                hdurl           = null;
                media_type      = null;
                service_version = null;
                title           = null;
                url             = null;
                //throw e;
            }
        }

        //Download image - standard resolution
        public void getImage()
        {
            try
            {
                if (img != null)
                    img.Dispose(); //trash previous image
                _wc = new WebClient();
                img = new Bitmap(_wc.OpenRead(url));
                _wc.Dispose();
            }
            catch (Exception)
            {
                _wc.Dispose();
                img.Dispose();
                throw;
            }
        }

        //Download image - HD resolution
        public void getImageHD()
        {
            try
            {
                if (imghd != null)
                    imghd.Dispose(); //trash previous image
                _wc = new WebClient();
                imghd = new Bitmap(_wc.OpenRead(hdurl));
                _wc.Dispose();
            }
            catch (Exception)
            {
                _wc.Dispose();
                imghd.Dispose();
                throw;
            }
        }

        //--- Private methods -------------------------------------------------------

        //Parse APOD json
        private void jsonDeserialize(string json)
        {
            json = System.Text.RegularExpressions.Regex.Unescape(json); //get rid of escape slashes that API returns

            copyright       = jsonGetSingle(json, "copyright");
            date            = jsonGetSingle(json, "date");
            explanation     = jsonGetSingle(json, "explanation");
            hdurl           = jsonGetSingle(json, "hdurl");
            media_type      = jsonGetSingle(json, "media_type");
            service_version = jsonGetSingle(json, "service_version");
            title           = jsonGetSingle(json, "title");
            url             = jsonGetSingle(json, "url");
        }

        //Parse out single field from json string
        private String jsonGetSingle(String json, String key)
        {
            string _key = '"' + key + '"'; //build key with quotes
            if (json.Contains(_key)) //if key found in json, look for it's value
            {
                int keyStartPos = json.IndexOf(_key);
                int valueStartPos = keyStartPos + _key.Length + 2; //2 - quote and comma?
                int valueLength = valueLength = json.IndexOf("\",", valueStartPos);

                if (valueLength == -1) //last key in order, try different parse
                    valueLength = json.IndexOf('"', valueStartPos);

                return json.Substring(valueStartPos, valueLength - valueStartPos);
            }
            else return null;
        }
    }
}
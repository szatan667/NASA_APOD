using System;
using System.Net;
using System.Text.RegularExpressions;

namespace NASA_APOD
{
    /// <summary>
    /// Simple container for NASA APOD returned fields
    /// </summary>
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

    /// <summary>
    /// Astronomy picture of the day object. Allows to call NASA API and retrieve image URL and description for given date.
    /// </summary>
    public partial class APOD : APOD_API
    {
        //--- Public fields ---------------------------------------------------------
        public DateTime apiDate;
        public bool isImage;
        public VideoType videoType;
        public enum VideoType
        {
            NONE,
            YOUTUBE,
            VIMEO
        }

        //--- Private fields --------------------------------------------------------
        private const string _baseURL = "https://api.nasa.gov/planetary/apod";
        //private const string _baseURL = "https://bing.biturl.top/";
        private const string _apiKeyDefault = "DFihYXvddhhd1KnnPtw3BgSxAXlx9yHz1CSTwbN8";
        private string _apiKey;
        private static readonly WebClient _wc = new WebClient();
        private static readonly DateTime _DATE_MIN = DateTime.Parse("1995-06-16");

        private const string VID_TYPE_YT = "youtube";
        private const string VID_TYPE_VM = "vimeo";
        public const string VID_LINK_YT = "youtube";
        public const string VID_LINK_VM = "player.vimeo";

        //--- Default constructor ---------------------------------------------------
        /// <summary>
        /// Create empty picture of the day object. 
        /// </summary>
        public APOD()
        {
            //nothing special...
            videoType = VideoType.NONE;
        }

        //--- Date-based constructor ---------------------------------------------------
        /// <summary>
        /// Create picture of the date object for given date,
        /// </summary>
        /// <param name="apiDate">Date of picture of the day</param>
        public APOD(DateTime apiDate)
        {
            setAPIDate(apiDate);
        }

        //--- Public methods --------------------------------------------------------

        //Set API key
        /// <summary>
        /// Set key for NASA API. Has to be 40-char custom key or "DEMO_KEY".
        /// </summary>
        /// <param name="apiKey">40-character long key string</param>
        public void setAPIKey(string apiKey)
        {
            if (apiKey.Length == 40 || apiKey == "DEMO_KEY")
                _apiKey = apiKey;
            else
                _apiKey = _apiKeyDefault;
        }

        //Set current date for API - create URL and get json
        /// <summary>
        /// Set current date for APOD API
        /// </summary>
        /// <param name="apiDate">API date</param>
        public void setAPIDate(DateTime apiDate)
        {
            //Don't go below minimum date
            if (apiDate < _DATE_MIN) apiDate = _DATE_MIN;

            //Double check API key and fall back to default if needed
            if (_apiKey == null || _apiKey == string.Empty || _apiKey.Length != 40)
                if (_apiKey != "DEMO_KEY")
                    _apiKey = _apiKeyDefault;

            //Create API URL
            string _apiURL = _baseURL;
            _apiURL += "?api_key=" + _apiKey;
            _apiURL += "&hd=true";
            _apiURL += "&date=" + apiDate.Year + "-";
            if (apiDate.Month < 10) _apiURL += "0" + apiDate.Month + "-";
            else _apiURL += apiDate.Month + "-";
            if (apiDate.Day < 10) _apiURL += "0" + apiDate.Day;
            else _apiURL += apiDate.Day;

            //Call websvc and strip json to local vars
            try
            {
                string json = _wc.DownloadString(_apiURL);
                jsonDeserialize(json);
                this.apiDate = apiDate; //set the date for APOD object

                if (media_type == "image") //set media type tag
                    isImage = true;
                else
                {
                    isImage = false;
                    videoType = VideoType.NONE;
                    if (url.Contains(VID_TYPE_YT) || hdurl.Contains(VID_TYPE_YT)) videoType = VideoType.YOUTUBE;
                    if (url.Contains(VID_TYPE_VM) || hdurl.Contains(VID_TYPE_VM)) videoType = VideoType.VIMEO;
                }
            }
            catch (Exception e)
            {
                _wc.Dispose();
                copyright       = string.Empty;
                date            = string.Empty;
                explanation     = string.Empty;
                hdurl           = string.Empty;
                media_type      = string.Empty;
                service_version = string.Empty;
                title           = string.Empty;
                url             = string.Empty;
                this.apiDate    = apiDate;// DateTime.MinValue;
                isImage         = false;
                throw e;
            }
        }

        //--- Private methods -------------------------------------------------------

        //Parse APOD json
        /// <summary>
        /// Deserialize json string returned by APOD API
        /// </summary>
        /// <param name="json">Json string in APOD API format</param>
        private void jsonDeserialize(string json)
        {
            json = Regex.Unescape(json); //get rid of escape slashes that API returns

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
        /// <summary>
        /// Get single key value from APOD API json string
        /// </summary>
        /// <param name="json">Json string in APOD API format</param>
        /// <param name="key">Key name</param>
        /// <returns></returns>
        private string jsonGetSingle(string json, string key)
        {
            string _key = '"' + key + '"'; //build key with quotes
            if (json.Contains(_key)) //if key found in json, look for it's value
            {
                int keyStartPos = json.IndexOf(_key);
                int valueStartPos = keyStartPos + _key.Length + 2; //2 - quote and comma?
                int valueLength = json.IndexOf("\",", valueStartPos);

                if (valueLength == -1) //last key in order, try different parse
                    valueLength = json.IndexOf('"', valueStartPos);

                return json.Substring(valueStartPos, valueLength - valueStartPos);
            }
            else return string.Empty;
        }
    }
    
    //Dispose it properly
    public partial class APOD : IDisposable
    {
        private bool isDisposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
                if (disposing)
                    if (_wc != null)
                        _wc.Dispose();
            isDisposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
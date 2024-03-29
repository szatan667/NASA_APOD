﻿using System;
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
        public string Copyright { get; set; }
        public string Date { get; set; }
        public string Explanation { get; set; }
        public string HdUrl { get; set; }
        public string MediaType { get; set; }
        public string ServiceVersion { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }

    /// <summary>
    /// Astronomy picture of the day object. Allows to call NASA API and retrieve image URL and description for given date.
    /// </summary>
    public partial class APOD : APOD_API
    {
        //--- Public fields ---------------------------------------------------------
        public DateTime ApiDate { get; set; }
        public bool IsDownloading { get; set; }
        public bool IsImage { get; set; }
        public NasaVideoType VideoType { get; set; }
        public enum NasaVideoType
        {
            NONE,
            YOUTUBE,
            VIMEO
        }

        //--- Private fields --------------------------------------------------------
        private const string _baseURL = "https://api.nasa.gov/planetary/apod";
        private const string _apiKeyDefault = "DFihYXvddhhd1KnnPtw3BgSxAXlx9yHz1CSTwbN8";
        private string _apiKey;
        private static readonly DateTime _DATE_MIN = DateTime.Parse("1995-06-16");

        private const string VID_TYPE_YT = "youtube";
        private const string VID_TYPE_VM = "vimeo";
        public const string VID_LINK_YT = "youtube";
        public const string VID_LINK_VM = "player.vimeo";

        //--- Default constructor ---------------------------------------------------
        /// <summary>
        /// Create empty picture of the day object
        /// </summary>
        public APOD()
        {
            //nothing special here...
            VideoType = NasaVideoType.NONE;
            IsDownloading = false;
        }

        //--- Date-based constructor ---------------------------------------------------
        /// <summary>
        /// Create picture of the date object for given date
        /// </summary>
        /// <param name="apiDate">Date of picture of the day</param>
        public APOD(DateTime apiDate)
        {
            SetApiDate(apiDate);
            VideoType = NasaVideoType.NONE;
            IsDownloading = false;
        }

        //--- Public methods --------------------------------------------------------

        /// <summary>
        /// Set key for NASA API. Has to be 40-char custom key or "DEMO_KEY".
        /// </summary>
        /// <param name="apiKey">40-character long key string</param>
        public void SetApiKey(string apiKey)
        {
            if (apiKey.Length == 40 || apiKey == "DEMO_KEY")
                _apiKey = apiKey;
            else
                _apiKey = _apiKeyDefault;
        }

        /// <summary>
        /// Set given date for APOD API
        /// </summary>
        /// <param name="apiDate">API date</param>
        public void SetApiDate(DateTime apiDate)
        {
            //Don't go below minimum date
            if (apiDate < _DATE_MIN)
                apiDate = _DATE_MIN;

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
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using WebClient wc = new();
                JsonDeserialize(wc.DownloadString(_apiURL));
                ApiDate = apiDate;

                //Set media type flags
                if (MediaType == "image")
                {
                    IsImage = true;
                    VideoType = NasaVideoType.NONE;
                }
                else
                {
                    IsImage = false;
                    VideoType = NasaVideoType.NONE;
                    if (Url.Contains(VID_TYPE_YT) || HdUrl.Contains(VID_TYPE_YT)) VideoType = NasaVideoType.YOUTUBE;
                    if (Url.Contains(VID_TYPE_VM) || HdUrl.Contains(VID_TYPE_VM)) VideoType = NasaVideoType.VIMEO;
                }
            }
            catch (Exception e)
            {
                Copyright = string.Empty;
                Date = string.Empty;
                Explanation = string.Empty;
                HdUrl = string.Empty;
                MediaType = string.Empty;
                ServiceVersion = string.Empty;
                Title = string.Empty;
                Url = string.Empty;
                ApiDate = apiDate;
                IsImage = false;
                VideoType = NasaVideoType.NONE;
                throw e;
            }
        }

        //--- Private methods -------------------------------------------------------

        /// <summary>
        /// Deserialize json string returned by APOD API
        /// </summary>
        /// <param name="json">Json string in APOD API format</param>
        private void JsonDeserialize(string json)
        {
            json = Regex.Unescape(json).Replace("�", string.Empty); //get rid of escape slashes and dummy chars that API sometimes returns

            Copyright = JsonGetSingle(json, "copyright");
            Date = JsonGetSingle(json, "date");
            Explanation = JsonGetSingle(json, "explanation");
            HdUrl = JsonGetSingle(json, "hdurl");
            MediaType = JsonGetSingle(json, "media_type");
            ServiceVersion = JsonGetSingle(json, "service_version");
            Title = JsonGetSingle(json, "title");
            Url = JsonGetSingle(json, "url");
        }

        /// <summary>
        /// Get single key value from APOD API json string
        /// </summary>
        /// <param name="json">Json string in APOD API format</param>
        /// <param name="key">Key name</param>
        /// <returns>Single key from apod json</returns>
        private string JsonGetSingle(string json, string key)
        {
            string _key = '"' + key + '"'; //build key with quotes

            if (json.Contains(_key))
            {
                int keyStartPos = json.IndexOf(_key);
                int valueStartPos = keyStartPos + _key.Length + 2; //2 - quote and comma?
                int valueLength = json.IndexOf("\",", valueStartPos);

                if (valueLength == -1) //last key in order, try different parse
                    valueLength = json.IndexOf('"', valueStartPos);

                return json.Substring(valueStartPos, valueLength - valueStartPos);
            }
            else
                return string.Empty;
        }
    }

    //Dispose it properly
    public partial class APOD : IDisposable
    {
        private bool isDisposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
                isDisposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
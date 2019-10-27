using System;
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

        //--- Private fields --------------------------------------------------------
        private const string _baseURL = "https://api.nasa.gov/planetary/apod";
        private const string _apiKeyDefault = "DFihYXvddhhd1KnnPtw3BgSxAXlx9yHz1CSTwbN8";
        private string _apiKey;
        private static readonly WebClient _wc = new WebClient();

        //--- Default constructor ---------------------------------------------------
        public APOD()
        {
            //nothing special...
        }

        //--- Dispose stuff ---------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                _wc.Dispose();
            }
            // free native resources
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //--- Public methods --------------------------------------------------------

        //Set API key
        public void setAPIKey(string key)
        {
            if (key.Length == 40 || key == "DEMO_KEY")
                _apiKey = key;
            else
                _apiKey = _apiKeyDefault;
        }

        //Set current date for API - create URL and get json
        public void setAPIDate(DateTime datetime)
        {
            if (datetime < DateTime.Parse("1995-06-16"))
                datetime = DateTime.Parse("1995-06-16");

            string _apiURL;

            //Double check API key and fall back to default if needed
            if (_apiKey == null || _apiKey == string.Empty || _apiKey.Length != 40)
                if (_apiKey != "DEMO_KEY")
                    _apiKey = _apiKeyDefault;

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
                string json = _wc.DownloadString(_apiURL);
                jsonDeserialize(json);
                apiDate = datetime; //set the date for APOD object
            }
            catch (Exception e)
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
                throw e;
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
            else return null;
        }
    }
}
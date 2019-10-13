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
        private const string _apiKey = "DFihYXvddhhd1KnnPtw3BgSxAXlx9yHz1CSTwbN8";

        //--- Default constructor ---------------------------------------------------
        public APOD()
        {
            //nothing special...
        }

        //--- Public methods --------------------------------------------------------

        //Set current date for API - create URL and get json
        public void setAPIDate(DateTime datetime)
        {
            string _apiURL;
            WebClient _wc = new WebClient();

            //Set the date and setup API URL
            apiDate = datetime; //set the date
            _apiURL = _baseURL + //put together full API URL
                      "?api_key=" + _apiKey +
                      "&date=" + apiDate.ToShortDateString();

            //Call websvc and strip json to local vars
            try
            {
                jsonDeserialize(_wc.DownloadString(_apiURL));
                _wc.Dispose();
            }
            catch (Exception)
            {
                _wc.Dispose();
                //throw;
                copyright = null;
                date = null;
                explanation = null;
                hdurl = null;
                media_type = null;
                service_version = null;
                title = null;
                url = null;
            }
}

        //--- Private methods -------------------------------------------------------

        //Parse APOD json
        private void jsonDeserialize(string json)
        {
            copyright = jsonGetSingle(json, "copyright");
            date = jsonGetSingle(json, "date");
            explanation = jsonGetSingle(json, "explanation");
            hdurl = jsonGetSingle(json, "hdurl");
            media_type = jsonGetSingle(json, "media_type");
            service_version = jsonGetSingle(json, "service_version");
            title = jsonGetSingle(json, "title");
            url = jsonGetSingle(json, "url");
        }

        //Parse out single field from json string
        private String jsonGetSingle(String json, String key)
        {
            string _key = '"' + key + '"'; //build key with quotes
            if (json.Contains(_key)) //if key found in json, look for it's value
            {
                int keyStartPos = json.IndexOf(_key);
                int valueStartPos = keyStartPos + _key.Length + 2; //2 - quote and comma?
                int valueLength = json.IndexOf('"', valueStartPos);
                    //valueLength = json.LastIndexOf()
                String outstr = json.Substring(valueStartPos, valueLength - valueStartPos);
                return outstr;
            }
            else return null;
        }
    }
}
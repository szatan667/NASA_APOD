using System;
using System.Net;

namespace NASA_APOD
{
    public class APOD
    {
        //--- Public fields ------------------------------------------------------------

        //Json fields for NASA API
        public string copyright { get; set; }
        public string date { get; set; }
        public string explanation { get; set; }
        public string hdurl { get; set; }
        public string media_type { get; set; }
        public string service_version { get; set; }
        public string title { get; set; }
        public string url { get; set; }

        //temporary storage for json string
        //public string json;

        //Public property to get the API's date
        //public DateTime apiDate
        //{
        //    get
        //    {
        //        return DateTime.Parse(_apiDate);
        //    }
        //}

        //--- Private fields -----------------------------------------------------------

        //API URL
        //private const string _baseURL = "https://api.nasa.gov/planetary/apod";
        //private const string _apiKey = "DFihYXvddhhd1KnnPtw3BgSxAXlx9yHz1CSTwbN8";
        //private string _apiDate;
        //private string _apiURL;
        //private WebClient _wc;

        //--- Public methods ------------------------------------------------------------

        //Default constructor
        //public APOD()
        //{
            //Web client for downloading stuff
        //    _wc = new WebClient();

            //Setup default API URL with current date
        //    setDate(DateTime.Today);
        //}

        //Set the date for API
        //public void setDate(DateTime datetime)
        //{
            //Get date string from date+time input object
        //    _apiDate = datetime.ToString().Substring(0, 10);

            //Put together API URL
        //    _apiURL = _baseURL +
        //        "?api_key=" + _apiKey +
        //        "&date=" + _apiDate;

            //Call the webservice to get the json
        //    json = _wc.DownloadString(_apiURL);

            //Clear json container, it will be de-serialized outside the class
            //copyright = null;
            //date = null;
            //explanation = null;
            //hdurl = null;
            //media_type = null;
            //service_version = null;
            //title = null;
            //url = null;
        //}
    }
}
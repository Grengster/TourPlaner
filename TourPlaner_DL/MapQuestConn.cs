using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Net;
using TourPlaner_Models;
using System.Configuration;

namespace TourPlanner_DL
{
    public sealed class MapQuestConn
    {
        private static MapQuestConn instance = null;
        private static HttpClient httpClient = null;
        private readonly static string key = ConfigurationManager.AppSettings["mqKey"];
        private readonly static string storeMap = @"C:\Users\Gregor\source\repos\TourPlaner\TourPlaner_DL\TourMaps";
        private readonly static string placePic = @"C:\Users\Gregor\source\repos\TourPlaner\TourPlaner_DL\GoalPics";

        public static MapQuestConn Instance()
        {
            if (instance == null)
            {
                instance = new MapQuestConn();
            }
            if (httpClient == null)
            {
                httpClient = new HttpClient();
            }
            return instance;
        }

        public string FindRoute(string fromDestination, string toDestination, string method)
        {
            try
            {
                var response = httpClient.GetStringAsync("http://www.mapquestapi.com/directions/v2/route?key=" + key + "&from=" + fromDestination + "&to=" + toDestination + "&routeType=" + method);
                string respBody = response.Result;
                return respBody;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("Exception Caught!!");
                Debug.WriteLine("Message :{0} ", e.Message);
                return e.Message;
            }
        }

        public string FindPlace(string place)
        {
            try
            {
                var response = httpClient.GetStringAsync("https://maps.googleapis.com/maps/api/place/findplacefromtext/json?input=" + place + "&inputtype=textquery&fields=photos,formatted_address,name,rating,opening_hours,geometry&key=AIzaSyBZVpaxV9VjavY3CTiKDI1Zeajgfa_fXYs");
                string respBody = response.Result;
                return respBody;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("Exception Caught!!");
                Debug.WriteLine("Message :{0} ", e.Message);
                return e.Message;
            }
        }

        public async Task GetPhoto(string reference, string tourName)
        {
            try
            {
                System.IO.Directory.CreateDirectory($@"{placePic}\");

                string fileLocation = $@"{placePic}\{tourName}.png";

                using WebClient client = new();
                await client.DownloadFileTaskAsync(
                new Uri(
                    $@"https://maps.googleapis.com/maps/api/place/photo?maxwidth=400&photoreference=" + reference + "&sensor=false&key=AIzaSyBZVpaxV9VjavY3CTiKDI1Zeajgfa_fXYs"),
                fileLocation);
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("Exception Caught!!");
                Debug.WriteLine("Message :{0} ", e.Message);
            }
        }
        
        public static async Task GetAndSaveImage(string start, string end, string tourName)
        {
            System.IO.Directory.CreateDirectory($@"{storeMap}\");

            string fileLocation = $@"{storeMap}\{tourName}.png";
            using WebClient client = new();
            await client.DownloadFileTaskAsync(
                new Uri(
                    $@"https://www.mapquestapi.com/staticmap/v5/map?key={key}&size=1920,1080&start={start}&end={end}&format=png"),
                fileLocation);
        }

    }
}
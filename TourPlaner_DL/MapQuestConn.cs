using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Net;
using TourPlaner_Models;

namespace TourPlanner_DL
{
    public class MapQuestConn
    {
        private static MapQuestConn instance = null;
        private static HttpClient httpClient = null;
        private static string key = "8O7UfQqRKqYcc4gvWcnAeYCNCOmKmKxn";
        private static string RoutePath = @"C:\Users\Gregor\source\repos\TourPlaner\TourPlaner_DL\TourJson\";
        private static string storeMap = @"C:\Users\Gregor\source\repos\TourPlaner\TourPlaner_DL\TourMaps";

        public static MapQuestConn Instance()
        {
            if (instance == null)
            {
                instance = new MapQuestConn();
            }
            return instance;
        }


        private MapQuestConn()
        {
            if (httpClient == null)
            {
                httpClient = new HttpClient();
            }
        }

        public string TryConnection()
        {
            try
            {
                var response = httpClient.GetStringAsync("http://www.mapquestapi.com/directions/v2/route?key=" + key + "&from=Wien&to=Graz");

                Task filetask = File.WriteAllTextAsync(RoutePath + "TourData.json", response.Result.ToString());
                return response.Result;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("Exception Caught!!");
                Debug.WriteLine($"Message :{e.Message} ");
                return e.Message;
            }
        }
        public string FindRoute(string fromDestination, string toDestination)
        {
            try
            {
                var response = httpClient.GetStringAsync("http://www.mapquestapi.com/directions/v2/route?key=" + key + "&from=" + fromDestination + "&to=" + toDestination);
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

        public async Task GetAndSaveImage(string start, string end, string tourName)
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
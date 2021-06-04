using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TourPlaner_Models;
using TourPlanner_DL;

namespace TourPlaner_DL
{
    public class FileSystem : IDataAccess
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FileSystem));
        static readonly List<TourItem> mockList = new();

        public static List<TourItem> LoadJson()
        {
            List<TourItem> tempTour = new();
            using (StreamReader r = new(@"C:\Users\Gregor\source\repos\TourPlaner\TourPlaner_DL\TourJson\TourData.json"))
            {
                string json = r.ReadToEnd();
                tempTour = JsonConvert.DeserializeObject<List<TourItem>>(json);
            }
            if (tempTour != null)
                return tempTour;
            else
                return null;
        }

        public static List<string> GetItemString()
        {
            //get items from database
            //Retrieve all rows
            List<string> tempList = new();
            if (mockList.Count > 0)
            {
                foreach (var item in mockList)
                {
                    tempList.Add(item.Name);
                }
                return tempList;
            }
            else
                return null;
        }

        public List<TourItem> GetItems()
        {
            if (mockList.Count > 0)
            {
                return mockList;
            }
            else
            {
                return null;
            }
        }


        public List<TourItem> AddLogs(string tourName, string logEntry, int rating, int actualTime, string description, DateTime date)
        {
            if(tourName == "" || logEntry == "" || rating > 5 || rating < 0 || actualTime < 0 || description == "" || date == DateTime.Parse("1 / 1 / 0001"))
            {
                log.Error(new ArgumentNullException("Empty Arguments"));
                throw new ArgumentNullException("Empty Arguments");
            }
            var account = mockList.FirstOrDefault(p => p.Name == tourName);
            if (account == null)
            {
                throw new NullReferenceException("No Tour found");
            }
            else
            {
                UserRating tempRating = new();
                tempRating.Logs = logEntry;
                tempRating.Rating = rating;
                tempRating.ActualTime = actualTime;
                tempRating.Weather = description;
                tempRating.TravelDate = date;
                account.TourLogs.Add(tempRating);
                return GetItems();
            }
        }

        public List<TourItem> EditLogs(string tourName, string oldLogEntry, string logEntry, int rating, int actualTime, string description, DateTime? date = null, bool toDelete = false)
        {
            var account = mockList.FirstOrDefault(p => p.Name == tourName);
            var oldLog = account.TourLogs[0];
            try
            {
                oldLog = account.TourLogs.FirstOrDefault(p => p.Logs == oldLogEntry);
            }
            catch (Exception e)
            {
                throw new NullReferenceException("Wrong Log" + e);
            }
            if (toDelete)
            {
                account.TourLogs.Remove(oldLog);
            }
            else
            {
                oldLog.Logs = logEntry;
                oldLog.Rating = rating;
                oldLog.ActualTime = actualTime;
                oldLog.Weather = description;
                oldLog.TravelDate = (DateTime)date;
            }
            return GetItems();
        }


        public List<TourItem> AddTour(string tourName, string startName, string goalName, DateTime dateTime, string method)
        {
            bool foundTour = mockList.Any(x => x.Name == tourName);
            //var account = mockList.FirstOrDefault(p => p.Name == tourName);
            if (foundTour)
                return null;
            else
            {
                float jDistance = 1f;
                var jTime = "";
                string jsonString = "";
                try
                {
                    var jsonResponse = JObject.Parse(MapQuestConn.Instance().FindRoute(startName, goalName, method));
                    jDistance = float.Parse(jsonResponse["route"]["distance"].ToString()); //reads out of mapquest json response
                    jTime = jsonResponse["route"]["formattedTime"].ToString();
                    jsonString = jsonResponse.ToString();
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
                    mockList.Add(new TourItem()
                    {
                        Name = tourName,
                        TourInfo = new TourInfo()
                        {
                            Start = startName,
                            Goal = goalName,
                            Method = method,
                            Distance = jDistance,
                            MapImagePath = $@"C:\Users\Gregor\source\repos\TourPlaner\TourPlaner_DL\TourMaps\{tourName}.png",
                            CreationTime = dateTime,
                            TotalTime = jTime,
                            JsonData = jsonString
                        }
                    });
                    return GetItems();
            }
        }

        public List<TourItem> EditTour(string tourName, string newTourName, string startName, string goalName, DateTime dateTime, string method)
        {
            var account = mockList.FirstOrDefault(p => p.Name == tourName);
            //var account = mockList.FirstOrDefault(p => p.Name == tourName);
            if (account == null)
                throw new ArgumentException("No tour with this name found!");
            else
            {
                float jDistance = 1f;
                var jTime = "";
                string jsonString = "";
                try
                {
                    var jsonResponse = JObject.Parse(MapQuestConn.Instance().FindRoute(startName, goalName, method));
                    jDistance = float.Parse(jsonResponse["route"]["distance"].ToString()); //reads out of mapquest json response
                    jTime = jsonResponse["route"]["formattedTime"].ToString();
                    jsonString = jsonResponse.ToString();
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
                try
                {
                    account.Name = newTourName;
                    account.TourInfo.Start = startName;
                    account.TourInfo.Goal = goalName;
                    account.TourInfo.Method = method;
                    account.TourInfo.Distance = jDistance;
                    account.TourInfo.MapImagePath = $@"C:\Users\Gregor\source\repos\TourPlaner\TourPlaner_DL\TourMaps\{newTourName}.png";
                    account.TourInfo.CreationTime = dateTime;
                    account.TourInfo.TotalTime = jTime;
                    account.TourInfo.JsonData = jsonString;
                }
                catch (Exception e)
                {
                    throw new Exception("Error occurred! " + e);
                }

                return GetItems();
            }
        }


        public List<TourItem> RemoveTour(string tourName, bool mockItem = false)
        {
            bool foundTour = mockList.Any(x => x.Name == tourName);
            if (!foundTour)
                return null;

            foreach (var item in mockList)
            {
                if (item.Name == tourName)
                {
                    mockList.Remove(item);
                    return GetItems();
                }
            }
            return null;
        }

        public List<TourItem> Import(string fileName)
        {
            throw new NotImplementedException();
        }

        public void Export(IEnumerable<TourItem> tourList, string filename)
        {
            throw new NotImplementedException();
        }
    }
}

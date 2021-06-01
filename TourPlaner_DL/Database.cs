using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Npgsql;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using TourPlaner_Models;
using Newtonsoft.Json;
using log4net;
using log4net.Config;
using TourPlanner_DL;
using NugetJObject;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Text.Json.Serialization;

namespace TourPlaner_DL
{
    public class Database : IDataAccess
    {
        private readonly string connString;
        private NpgsqlConnection conn;
        private static readonly ILog log = LogManager.GetLogger(typeof(Database));
        private static readonly string userPath = ConfigurationManager.ConnectionStrings["userPath"].ConnectionString;

        public Database()
        {
            connString = ConfigurationManager.ConnectionStrings["dbConfig"].ConnectionString;
            DBConnect();
        }

        public static List<TourItem> LoadJson()
        {
            List<TourItem> tempTour = new();
            using (StreamReader r = new(userPath + @"TourPlaner_DL\TourJson\TourData.json"))
            {
                string json = r.ReadToEnd();
                tempTour = JsonConvert.DeserializeObject<List<TourItem>>(json);
            }
            if (tempTour != null)
                return tempTour;
            else
                return null;
        }

        public void DBConnect()
        {
            try
            {
                conn = new NpgsqlConnection(connString);
                conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<string> GetItemString()
        {
            //get items from database
            //Retrieve all rows
            List<string> tempList = new();
            using var cmd = new NpgsqlCommand("SELECT * FROM tours", conn);
            cmd.ExecuteNonQuery();
            bool isFound = false;
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    isFound = true;
                    tempList.Add((Convert.ToString(reader["name"])));
                }
            if (isFound)
                return tempList;
            else
                return null;
        }

        public List<TourItem> GetItems()
        {
            List<string> tempList = GetItemString();
            List<TourItem> tempTour = new();
            List<TourItem> jsonTour = LoadJson();
            if (tempList != null)
            {
                foreach (var item in tempList)
                {

                    //tempTour.Add(new TourItem { Name = item });
                    foreach (var jsonitem in jsonTour)
                    {
                        if (jsonitem.Name == item)
                            tempTour.Add(jsonitem);
                    }

                };
                return tempTour;
            }
            else
            {
                return null;
            }
        }


        public List<TourItem> AddLogs(string tourName, string logEntry, int rating, int actualTime, string description, DateTime date)
        {

            var jsonData = System.IO.File.ReadAllText(userPath + @"TourPlaner_DL\TourJson\TourData.json");
            var employeeList = JsonConvert.DeserializeObject<List<TourItem>>(jsonData) ?? new List<TourItem>();
            var account = employeeList.FirstOrDefault(p => p.Name == tourName);
            UserRating tempRating = new();
            tempRating.Logs = logEntry;
            tempRating.Rating = rating;
            tempRating.ActualTime = actualTime;
            tempRating.Weather = description;
            tempRating.TravelDate = date;
            account.TourLogs.Add(tempRating);

            jsonData = JsonConvert.SerializeObject(employeeList);
            File.WriteAllText(userPath + @"TourPlaner_DL\TourJson\TourData.json", jsonData);
            return GetItems();
        }

        public List<TourItem> EditLogs(string tourName, string oldLogEntry, string logEntry, int rating, int actualTime, string description, DateTime? date = null, bool toDelete = false)
        {

            var jsonData = System.IO.File.ReadAllText(userPath + @"TourPlaner_DL\TourJson\TourData.json");
            var employeeList = JsonConvert.DeserializeObject<List<TourItem>>(jsonData) ?? new List<TourItem>();
            var account = employeeList.FirstOrDefault(p => p.Name == tourName);
            var oldLog = account.TourLogs.FirstOrDefault(p => p.Logs == oldLogEntry);
            if(toDelete)
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
            

            jsonData = JsonConvert.SerializeObject(employeeList);
            File.WriteAllText(userPath + @"TourPlaner_DL\TourJson\TourData.json", jsonData);
            return GetItems();
        }


        public List<TourItem> AddTour(string tourName, string startName, string goalName, DateTime dateTime, string method)
        {
            bool isFound = false, isInserted = true;
            using (var cmd = new NpgsqlCommand("SELECT * FROM tours where name = @n", conn))
            {
                cmd.Parameters.AddWithValue("n", tourName);
                cmd.ExecuteNonQuery();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        isFound = true;
                    }
                if (isFound)
                    return null;
            };
            float jDistance = 1f;
            var jTime = "";
            string jsonString = "";
            try
            {
                var jsonResponse = JObject.Parse(MapQuestConn.Instance().FindRoute(startName, goalName, method));
                var placeDesc = JObject.Parse(MapQuestConn.Instance().FindPlace(goalName));

                var stringo = placeDesc["candidates"][0]["photos"][0]["photo_reference"].ToString();

                var task = Task.Run(async () => await MapQuestConn.Instance().GetPhoto(stringo, tourName));

                jDistance = float.Parse(jsonResponse["route"]["distance"].ToString()); //reads out of mapquest json response
                jTime = jsonResponse["route"]["formattedTime"].ToString();
                jsonString = jsonResponse.ToString();
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            

           // log.Info("Got ");

            //log.Info(response);
            using (var cmd = new NpgsqlCommand("INSERT INTO tours (name, goal, start, distance, response, esttime) VALUES (@n,@g,@s,@d,@r,@t)", conn))
            {
                cmd.Parameters.AddWithValue("n", tourName);
                cmd.Parameters.AddWithValue("g", goalName);
                cmd.Parameters.AddWithValue("s", startName);
                cmd.Parameters.AddWithValue("d", jDistance);
                cmd.Parameters.AddWithValue("r", jsonString);
                cmd.Parameters.AddWithValue("t", jTime);
                int a = cmd.ExecuteNonQuery();
                if (a == 0)
                    isInserted = false;

            };
            if (isInserted)
            {
                var jsonData = System.IO.File.ReadAllText(userPath + @"TourPlaner_DL\TourJson\TourData.json");
                var employeeList = JsonConvert.DeserializeObject<List<TourItem>>(jsonData) ?? new List<TourItem>();
                employeeList.Add(new TourItem()
                {
                    Name = tourName,
                    TourInfo = new TourInfo()
                    {
                        Start = startName,
                        Goal = goalName,
                        Method = method,
                        Distance = jDistance,
                        MapImagePath = userPath + $@"TourPlaner_DL\TourMaps\{tourName}.png",
                        CreationTime = dateTime,
                        TotalTime = jTime,
                        JsonData = jsonString
                    }
                });
                jsonData = JsonConvert.SerializeObject(employeeList);
                File.WriteAllText(userPath + @"TourPlaner_DL\TourJson\TourData.json", jsonData);
                return GetItems();
            }
            else
                return null;
        }


        public List<TourItem> EditTour(string tourName, string newTourName, string startName, string goalName, DateTime dateTime, string method)
        {
            bool isFound = false, isInserted = true;
            using (var cmd = new NpgsqlCommand("SELECT * FROM tours where name = @n", conn))
            {
                cmd.Parameters.AddWithValue("n", tourName);
                cmd.ExecuteNonQuery();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        isFound = true;
                    }
                if (!isFound)
                    return null;
            };
            float jDistance = 1f;
            var jTime = "";
            string jsonString = "";
            try
            {
                var jsonResponse = JObject.Parse(MapQuestConn.Instance().FindRoute(startName, goalName, method));
                var placeDesc = JObject.Parse(MapQuestConn.Instance().FindPlace(goalName));

                var stringo = placeDesc["candidates"][0]["photos"][0]["photo_reference"].ToString();

                var task = Task.Run(async () => await MapQuestConn.Instance().GetPhoto(stringo, newTourName));

                jDistance = float.Parse(jsonResponse["route"]["distance"].ToString()); //reads out of mapquest json response
                jTime = jsonResponse["route"]["formattedTime"].ToString();
                jsonString = jsonResponse.ToString();
            }
            catch (Exception e)
            {
                log.Error(e);
            }


            // log.Info("Got ");

            //log.Info(response);
            using (var cmd = new NpgsqlCommand("UPDATE tours SET name = @n, goal = @g, start = @s, distance = @d, response = @r, esttime = @t WHERE name = @o", conn))
            {
                cmd.Parameters.AddWithValue("n", newTourName);
                cmd.Parameters.AddWithValue("g", goalName);
                cmd.Parameters.AddWithValue("s", startName);
                cmd.Parameters.AddWithValue("d", jDistance);
                cmd.Parameters.AddWithValue("r", jsonString);
                cmd.Parameters.AddWithValue("t", jTime);
                cmd.Parameters.AddWithValue("o", tourName);
                int a = cmd.ExecuteNonQuery();
                if (a == 0)
                    isInserted = true;

            };
            if (isInserted)
            {
                var jsonData = System.IO.File.ReadAllText(userPath + @"TourPlaner_DL\TourJson\TourData.json");
                var employeeList = JsonConvert.DeserializeObject<List<TourItem>>(jsonData) ?? new List<TourItem>();
                var account = employeeList.FirstOrDefault(p => p.Name == tourName);
                try
                {
                    if (File.Exists(Path.Combine(userPath + $@"TourPlaner_DL\TourMaps\", $@"{tourName}.png")))
                    {
                        // If file found, delete it    
                        File.Delete(Path.Combine(userPath + $@"TourPlaner_DL\TourMaps\", $@"{tourName}.png"));
                    }

                    if (File.Exists(Path.Combine(userPath + $@"TourPlaner_DL\GoalPics\", $@"{tourName}.png")))
                    {
                        // If file found, delete it    
                        File.Move(Path.Combine(userPath + $@"TourPlaner_DL\GoalPics\", $@"{tourName}.png"), Path.Combine(userPath + $@"TourPlaner_DL\GoalPics\", $@"{newTourName}.png"));
                    }

                    if (File.Exists(Path.Combine(userPath + $@"TourPlaner_DL\TourPDFs\", $@"{tourName}Log.pdf")))
                    {
                        // If file found, delete it    
                        File.Move(Path.Combine(userPath + $@"TourPlaner_DL\TourPDFs\", $@"{tourName}Log.pdf"), Path.Combine(userPath + $@"TourPlaner_DL\TourPDFs\", $@"{newTourName}Log.pdf"));
                    }


                    account.Name = newTourName;
                    account.TourInfo.Start = startName;
                    account.TourInfo.Goal = goalName;
                    account.TourInfo.Method = method;
                    account.TourInfo.Distance = jDistance;
                    account.TourInfo.MapImagePath = userPath + $@"TourPlaner_DL\TourMaps\{newTourName}.png";
                    account.TourInfo.CreationTime = dateTime;
                    account.TourInfo.TotalTime = jTime;
                    account.TourInfo.JsonData = jsonString;
                    jsonData = JsonConvert.SerializeObject(employeeList);
                    File.WriteAllText(userPath + $@"TourPlaner_DL\TourJson\TourData.json", jsonData);
                }
                catch (Exception e)
                {
                    throw new Exception("Error occurred! " + e);
                }
                return GetItems();
            }
            else
                return null;
        }


        public List<TourItem> RemoveTour(string tourName, bool mockItem = false)
        {
            bool isFound = false, isDeleted = true;
            using (var cmd = new NpgsqlCommand("SELECT * FROM tours where name = @n", conn))
            {
                cmd.Parameters.AddWithValue("n", tourName);
                cmd.ExecuteNonQuery();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        isFound = true;
                    }
                if (!isFound)
                    return null;
            };

            using (var cmd = new NpgsqlCommand("DELETE FROM tours where name = @n", conn))
            {
                cmd.Parameters.AddWithValue("n", tourName);
                int a = cmd.ExecuteNonQuery();
                if (a == 0)
                    isDeleted = false;

            };
            if (isDeleted)
            {
                var jsonData = System.IO.File.ReadAllText(userPath + $@"TourPlaner_DL\TourJson\TourData.json");
                var employeeList = JsonConvert.DeserializeObject<List<TourItem>>(jsonData) ?? new List<TourItem>();
                var account = employeeList.FirstOrDefault(p => p.Name == tourName);
                employeeList.Remove(account);
                jsonData = JsonConvert.SerializeObject(employeeList);
                File.WriteAllText(userPath + $@"TourPlaner_DL\TourJson\TourData.json", jsonData);
                if (File.Exists(Path.Combine(userPath + $@"TourPlaner_DL\TourMaps\", $@"{tourName}.png")))
                {
                    // If file found, delete it    
                    File.Delete(Path.Combine(userPath + $@"TourPlaner_DL\TourMaps\", $@"{tourName}.png"));
                }

                if (File.Exists(Path.Combine(userPath + $@"TourPlaner_DL\GoalPics\", $@"{tourName}.png")))
                {
                    // If file found, delete it    
                    File.Delete(Path.Combine(userPath + $@"TourPlaner_DL\GoalPics\", $@"{tourName}.png"));
                }

                if (File.Exists(Path.Combine(userPath + $@"TourPlaner_DL\TourPDFs\", $@"{tourName}Log.pdf")))
                {
                    // If file found, delete it    
                    File.Delete(Path.Combine(userPath + $@"TourPlaner_DL\TourPDFs\", $@"{tourName}Log.pdf"));
                }
                return GetItems();
            }
            else
                return null;
        }



        public void Export(IEnumerable<TourItem> tourList, string filename)
        {
            try
            {
                JsonSerializerOptions options = new() { WriteIndented = true, MaxDepth = 0, ReferenceHandler = ReferenceHandler.Preserve };
                string jsonString = System.Text.Json.JsonSerializer.Serialize<IEnumerable<TourItem>>(tourList, options);

                FileInfo file = new FileInfo(userPath + @"TourPlaner_DL\TourExport\");
                file.Directory.Create();

                File.WriteAllText(userPath + @"TourPlaner_DL\TourExport\" + filename, jsonString);
            }
            catch (Exception e)
            {
                log.Info(e.ToString() + " Source: " + e.Source + "\n" + e.Message);
            }
        }


        public List<TourItem> Import(string fileName)
        {
            try
            {
                JsonSerializerOptions options = new() { WriteIndented = true, MaxDepth = 0, ReferenceHandler = ReferenceHandler.Preserve };
                string fileString = File.ReadAllText(userPath + @"TourPlaner_DL\TourExport\" + fileName);


                List<TourItem> tourList = System.Text.Json.JsonSerializer.Deserialize<List<TourItem>>(fileString, options);

                foreach (TourItem TourItem in tourList)
                {
                    AddTour(TourItem.Name, TourItem.TourInfo.Start, TourItem.TourInfo.Goal, TourItem.TourInfo.CreationTime, TourItem.TourInfo.Method);
                    if (TourItem.TourLogs != null)
                    {
                        foreach (UserRating log in TourItem.TourLogs)
                        {
                            AddLogs(TourItem.Name, log.Logs, log.Rating, log.ActualTime, log.Weather, log.TravelDate);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                 log.Error(e.ToString() + " Source: " + e.Source + "\n" + e.Message);
            }
            return GetItems();
        }

    }
}

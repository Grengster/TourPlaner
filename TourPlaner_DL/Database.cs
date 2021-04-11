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


namespace TourPlaner_DL
{
    public class Database : IDataAccess
    {
        private readonly string connString;
        private NpgsqlConnection conn;

        public Database()
        {
            connString = "Host=localhost;Username=admin;Password=1234;Database=TourPlaner";
            DBConnect();
        }

        public List<TourItem> LoadJson()
        {
            List<TourItem> tempTour = new();
            using (StreamReader r = new StreamReader(@"C:\Users\Gregor\source\repos\TourPlaner_DL\TourJson\TourData.json"))
            {
                string json = r.ReadToEnd();
                tempTour = JsonConvert.DeserializeObject<List<TourItem>>(json);
            }
            if (tempTour != null)
                return tempTour;
            else
                return null;
        }

        public T GetFirstInstance<T>(string propertyName, string json)
        {
            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.PropertyName
                        && (string)jsonReader.Value == propertyName)
                    {
                        jsonReader.Read();

                        var serializer = new Newtonsoft.Json.JsonSerializer();
                        return serializer.Deserialize<T>(jsonReader);
                    }
                }
                return default(T);
            }
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

        public List<TourItem> AddTour(string tourName, string startName, string goalName, DateTime dateTime)
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

            using (var cmd = new NpgsqlCommand("INSERT INTO tours (name) VALUES (@n)", conn))
            {
                cmd.Parameters.AddWithValue("n", tourName);
                int a = cmd.ExecuteNonQuery();
                if (a == 0)
                    isInserted = false;
                
            };
            if (isInserted)
            {
                var jsonData = System.IO.File.ReadAllText(@"C:\Users\Gregor\source\repos\TourPlaner_DL\TourJson\TourData.json");
                var employeeList = JsonConvert.DeserializeObject<List<TourItem>>(jsonData) ?? new List<TourItem>();
                employeeList.Add(new TourItem()
                {
                    Name = tourName,
                    _tourInfo = new TourInfo()
                    {
                        Start = startName,
                        Goal = goalName,
                        Length = 100,
                        MapImagePath = $@"C:\Users\Gregor\source\repos\TourPlaner_DL\TourMaps\{tourName}.png",
                        CreationTime = dateTime,
                        StartTime = dateTime,
                        EndTime = dateTime
                    }
                });
                jsonData = JsonConvert.SerializeObject(employeeList);
                File.WriteAllText(@"..\..\..\..\TourPlaner_DL\TourJson\TourData.json", jsonData);
                return GetItems();
            }
            else
                return null;
        }

        public List<TourItem> RemoveTour(string tourName)
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
                var jsonData = System.IO.File.ReadAllText(@"C:\Users\Gregor\source\repos\TourPlaner_DL\TourJson\TourData.json");
                var employeeList = JsonConvert.DeserializeObject<List<TourItem>>(jsonData) ?? new List<TourItem>();
                var account = employeeList.FirstOrDefault(p => p.Name == tourName);
                employeeList.Remove(account);
                jsonData = JsonConvert.SerializeObject(employeeList);
                File.WriteAllText(@"C:\Users\Gregor\source\repos\TourPlaner_DL\TourJson\TourData.json", jsonData);
                return GetItems();
            }
            else
                return null;
        }
    }
}

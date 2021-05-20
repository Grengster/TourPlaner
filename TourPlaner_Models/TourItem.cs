using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TourPlaner_Models
{

    public class TourItem
    {
        public string Name { get; set; }

        public TourInfo TourInfo { get; set; }
        public List<UserRating> TourLogs { get; set; }
        public TourItem()
        {
            TourLogs = new List<UserRating>();
        }
    }

    public class TourInfo
    {
        public string Start { get; set; }
        public string Goal { get; set; }
        public string Method { get; set; }
        public float Distance { get; set; }
        public string MapImagePath { get; set; }
        public DateTime CreationTime { get; set; }
        public string TotalTime { get; set; }
        public string JsonData { get; set; }
    }
    public class UserRating
    { //KALENDERFUNKTION EINBAUEN ALS UNIQUE FEATURE
        public DateTime TravelDate { get; set; }
        public string Logs { get; set; }
        public string Weather { get; set; }
        public int ActualTime { get; set; }
        public int Rating { get; set; } 
    }
}

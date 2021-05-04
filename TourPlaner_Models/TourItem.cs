using Newtonsoft.Json;
using System;

namespace TourPlaner_Models
{

    public class TourItem
    {
        public string Name { get; set; }

        public TourInfo TourInfo { get; set; }
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
        public string Description { get; set; }
        public int ActualTime { get; set; }
        public int Rating { get; set; }
    }
}

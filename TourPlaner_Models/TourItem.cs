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
    }
}

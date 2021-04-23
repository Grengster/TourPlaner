using Newtonsoft.Json;
using System;

namespace TourPlaner_Models
{

    public class TourItem
    {
        public string Name { get; set; }

        public TourInfo tourInfo { get; set; }
    }
    public class TourInfo
    {
        public string Start { get; set; }
        public string Goal { get; set; }
        public int Distance { get; set; }
        public string MapImagePath { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}

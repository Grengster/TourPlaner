using Newtonsoft.Json;
using System;

namespace TourPlaner_Models
{

    public class TourItem
    {
        public string Name { get; set; }

        public TourInfo _tourInfo { get; set; }
    }
    public class TourInfo
    {
        public string Start { get; set; }
        public string Goal { get; set; }
        public int Length { get; set; }
        public string MapImagePath { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}

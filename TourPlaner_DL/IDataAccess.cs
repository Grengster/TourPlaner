using System;
using System.Collections.Generic;
using TourPlaner_Models;

namespace TourPlaner_DL
{
    public interface IDataAccess
    {
        public List<TourItem> GetItems();
        public List<TourItem> AddTour(string itemName, string startName, string goalName, DateTime dateTime, string method);
        public List<TourItem> AddLogs(string tourName, string logEntry, int rating, int actualTime, string description, DateTime date);
        public List<TourItem> RemoveTour(string itemName);
    }
}

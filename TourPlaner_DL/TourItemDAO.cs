using System;
using System.Collections.Generic;
using TourPlaner_Models;

namespace TourPlaner_DL
{
    public class TourItemDAO
    {

        private readonly IDataAccess dataAccess;

        public TourItemDAO()
        {
            dataAccess = new Database();
        }

        public List<TourItem> GetItems()
        {
            return dataAccess.GetItems();
        }

        public List<TourItem> AddTour(string itemName, string startName, string goalName, DateTime dateTime, string method)
        {
            return dataAccess.AddTour(itemName, startName, goalName, dateTime, method);
        }

        public List<TourItem> AddLogs(string tourName, string logEntry, int rating, int actualTime, string description, DateTime date)
        {
            return dataAccess.AddLogs(tourName, logEntry, rating, actualTime, description, date);
        }

        public List<TourItem> EditLogs(string tourName, string oldLogEntry, string logEntry, int rating, int actualTime, string description, DateTime date, bool toDelete = false)
        {
            return dataAccess.EditLogs(tourName, oldLogEntry, logEntry, rating, actualTime, description, date, toDelete);
        }

        public List<TourItem> RemoveTour(string itemName)
        {
            return dataAccess.RemoveTour(itemName);
        }
    }
}

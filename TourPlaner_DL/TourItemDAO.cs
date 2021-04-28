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

        public List<TourItem> RemoveTour(string itemName)
        {
            return dataAccess.RemoveTour(itemName);
        }
    }
}

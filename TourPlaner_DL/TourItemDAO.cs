using System;
using System.Collections.Generic;
using TourPlaner_Models;

namespace TourPlaner_DL
{
    public class TourItemDAO 
    {

        private IDataAccess dataAccess;

        public TourItemDAO()
        {
            dataAccess = new Database();
        }

        public List<TourItem> GetItems()
        {
            return dataAccess.GetItems();
        }

        public List<TourItem> AddTour(string itemName, string startName, string goalName, DateTime dateTime)
        {
            return dataAccess.AddTour(itemName, startName, goalName, dateTime);
        }

        public List<TourItem> RemoveTour(string itemName)
        {
            return dataAccess.RemoveTour(itemName);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourPlaner_Models;

namespace TourPlaner_BL
{
    public interface ITourItemFactory
    {
        IEnumerable<TourItem> GetItems();
        IEnumerable<TourItem> Search(string itemName, bool caseSensitive = false);
        IEnumerable<TourItem> AddTour(string itemName, string startName, string goalName, DateTime dateTime, string method);
        IEnumerable<TourItem> AddLogs(string tourName, string logEntry, int rating, int actualTime, string description, DateTime date);
        IEnumerable<TourItem> RemoveTour(string itemName);
        Task ShowMapTourAsync(string start, string end, string tourName);
        Task CreatePDF(string tourName);
    }
}

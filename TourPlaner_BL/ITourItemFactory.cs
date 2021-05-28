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
        Task<IEnumerable<TourItem>> AddTourAsync(string itemName, string startName, string goalName, DateTime dateTime, string method);
        Task<IEnumerable<TourItem>> EditTour(string itemName, string newTourName, string startName, string goalName, DateTime dateTime, string method);
        IEnumerable<TourItem> AddLogs(string tourName, string logEntry, int rating, int actualTime, string description, DateTime date);
        IEnumerable<TourItem> EditLogs(string tourName, string oldLogEntry, string logEntry, int rating, int actualTime, string description, DateTime? date = null, bool toDelete = false);
        IEnumerable<TourItem> RemoveTour(string itemName, bool mockItem = false);
        Task ShowMapTourAsync(string start, string end, string tourName);
        Task CreatePDF(string tourName, TourItem mockItem = null);
        Task CreateSummary(List<TourItem> mockList = null);
    }
}

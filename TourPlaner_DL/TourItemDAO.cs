using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourPlaner_Models;

namespace TourPlaner_DL
{
    public class TourItemDAO
    {

        private readonly IDataAccess dataAccess;

        public TourItemDAO(bool mockDatabase = false)
        {
            if (!mockDatabase)
                dataAccess = new Database();
            else
                dataAccess = new FileSystem();
        }

        public List<TourItem> GetItems()
        {
            return dataAccess.GetItems();
        }

        public List<TourItem> AddTour(string itemName, string startName, string goalName, DateTime dateTime, string method)
        {
            return dataAccess.AddTour(itemName, startName, goalName, dateTime, method);
        }

        public List<TourItem> EditTour(string itemName, string newTourName, string startName, string goalName, DateTime dateTime, string method)
        {
            return dataAccess.EditTour(itemName, newTourName, startName, goalName, dateTime, method);
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

        public List<TourItem> Import(string fileName)
        {
            return dataAccess.Import(fileName);
        }

        public void Export(IEnumerable<TourItem> tourList, string filename)
        {
            dataAccess.Export(tourList, filename);
        }

        public async Task CreatePDF(string tourName, TourItem mockItem = null)
        {
            PDFHandler tempHandler = new();
            await tempHandler.CreatePDF(tourName, mockItem);
        }

        public async Task CreateSummary(List<TourItem> mockList = null)
        {
            PDFHandler tempHandler = new();
            await tempHandler.CreateSummary(mockList);
        }

    }
}

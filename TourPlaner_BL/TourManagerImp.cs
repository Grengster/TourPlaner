using System;
using System.Collections.Generic;
using System.Linq;
using TourPlaner_Models;
using TourPlaner_DL;
using System.IO;
using Newtonsoft.Json;
using TourPlanner_DL;
using System.Threading.Tasks;
using log4net;

namespace TourPlaner_BL
{
    internal class TourManagerImp : ITourItemFactory
    {

        private readonly TourItemDAO tourItemDAO = new(); //anhand config file variieren


        public IEnumerable<TourItem> GetItems()
        {
            // usually querying the disk, or from a DB, or ...
            return tourItemDAO.GetItems();
        }

        public IEnumerable<TourItem> Search(string itemName, bool caseSensitive = false)
        {
            IEnumerable<TourItem> tours = GetItems();
            if (itemName == null || itemName == "")
                return tours;
            if (caseSensitive)
            {
                return tours.Where(x => x.Name.Contains(itemName));
            }
            return tours.Where(x => x.Name.ToLower().Contains(itemName.ToLower()));
        }

        private async Task<List<TourItem>> GetListAsync(string itemName, string startName, string goalName, DateTime dateTime, string method)
        {
            List<TourItem> list = (List<TourItem>)await Task.Run(() => tourItemDAO.AddTour(itemName, startName, goalName, dateTime, method));
            return list;
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(TourManagerImp));

        public async Task<IEnumerable<TourItem>> AddTourAsync(string itemName, string startName, string goalName, DateTime dateTime, string method)
        {
            IEnumerable<TourItem> tours = null;
            if (GetItems() != null)
            {
                tours = GetItems();
                foreach (var item in tours)
                {
                    var find = tours.FirstOrDefault(x => x.Name == itemName);
                    if (find == null)
                    {
                        try
                        {
                            var result = await GetListAsync(itemName, startName, goalName, dateTime, method);
                            if(result ==  null)
                                return null;
                            else
                                return tours;
                        }
                        catch (Exception e)
                        {
                            log.Error(e);
                        }
                    }
                }
                return tours;
            }
            else
                tourItemDAO.AddTour(itemName, startName, goalName, dateTime, method);
            return tours;

        }

        public IEnumerable<TourItem> AddLogs(string tourName, string logEntry, int rating, int actualTime, string description, DateTime date)
        {
            IEnumerable<TourItem> tours = null;
            if (GetItems() != null)
            {
                tours = GetItems();
                if (tourItemDAO.AddLogs(tourName, logEntry, rating, actualTime, description, date) == null)
                    return null;
                return tours;
            }
            return tours;
        }

        public IEnumerable<TourItem> EditLogs(string tourName, string oldLogEntry, string logEntry, int rating, int actualTime, string description, DateTime date, bool toDelete = false)
        {
            IEnumerable<TourItem> tours = null;
            if (GetItems() != null)
            {
                tours = GetItems();
                if (tourItemDAO.EditLogs(tourName, oldLogEntry, logEntry, rating, actualTime, description, date, toDelete) == null)
                    return null;
                return tours;
            }
            return tours;
        }


        public IEnumerable<TourItem> RemoveTour(string itemName)
        {
            IEnumerable<TourItem> tours = null;
            if (GetItems() != null)
            {
                tours = GetItems();
                foreach (var item in tours)
                {
                    var find = tours.FirstOrDefault(x => x.Name == itemName);
                    if (find != null)
                    {
                        if (tourItemDAO.RemoveTour(itemName) == null)
                            return null;
                        else
                            return tours;
                    }
                }
                return tours;
            }
            else
                return tours;
        }


        public async Task ShowMapTourAsync(string start, string end, string tourName)
        {
            await MapQuestConn.GetAndSaveImage(start, end, tourName);
        }

        public async Task CreatePDF(string tourName)
        {
            await PDFHandler.CreatePDF(tourName);
        }

        public async Task CreateSummary()
        {
            await PDFHandler.CreateSummary();
        }
    }
}

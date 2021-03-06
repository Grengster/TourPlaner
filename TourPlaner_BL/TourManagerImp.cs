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
        private readonly TourItemDAO tourItemDAO = null;
        public TourManagerImp(bool mockDatabase = false)
        {
            if (!mockDatabase)
            {
                tourItemDAO = new TourItemDAO();
            }
            else
                tourItemDAO = new TourItemDAO(true);
        }

        public IEnumerable<TourItem> GetItems()
        {
            // usually querying the disk, or from a DB, or ...
            return tourItemDAO.GetItems();
        }

        public IEnumerable<TourItem> Search(string itemName, bool caseSensitive = false)
        {
            IEnumerable<TourItem> tours = GetItems();
            IEnumerable<TourItem> searchTours;
            if (itemName == null || itemName == "") 
                return tours;
            if (caseSensitive)
            {
                searchTours = tours.Where(x => x.Name.Contains(itemName));
                searchTours = searchTours.Union(tours.Where(x => x.TourInfo.JsonData.Contains(itemName)));
                searchTours = searchTours.Union(tours.Where(x => x.TourInfo.Goal.Contains(itemName)));
                searchTours = searchTours.Union(tours.Where(x => x.TourInfo.Start.Contains(itemName)));
                searchTours = searchTours.Union(tours.Where(x => x.TourLogs.Any(y => y.Logs.Contains(itemName))));
                searchTours = searchTours.Union(tours.Where(x => x.TourLogs.Any(y => y.Weather.Contains(itemName))));
                return searchTours;
            }
            searchTours = tours.Where(x => x.Name.ToLower().Contains(itemName.ToLower()));
            searchTours = searchTours.Union(tours.Where(x => x.TourInfo.JsonData.ToLower().Contains(itemName.ToLower())));
            searchTours = searchTours.Union(tours.Where(x => x.TourInfo.Goal.ToLower().Contains(itemName.ToLower())));
            searchTours = searchTours.Union(tours.Where(x => x.TourInfo.Start.ToLower().Contains(itemName.ToLower())));
            searchTours = searchTours.Union(tours.Where(x => x.TourLogs.Any(y => y.Logs.ToLower().Contains(itemName.ToLower()))));
            searchTours = searchTours.Union(tours.Where(x => x.TourLogs.Any(y => y.Weather.ToLower().Contains(itemName.ToLower()))));
            return searchTours;
            
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
                    else
                    {
                        log.Error("User tried to add duplicate Tour: " + itemName);
                        return null;
                    }
                }
                return tours;
            }
            else
            {
                try
                {
                    tourItemDAO.AddTour(itemName, startName, goalName, dateTime, method);
                }
                catch (Exception e)
                {
                    log.Error("Error adding Tour: " + e);
                    return null;
                }
                tours = GetItems();
            }
            return tours;
        }

        public async Task<IEnumerable<TourItem>> EditTour(string itemName, string newTourName, string startName, string goalName, DateTime dateTime, string method)
        {
            if (GetItems() == null)
            {
                log.Error("User tried to edit non existing Tour: " + itemName);
                return null;
            }
            else
            {
                tourItemDAO.EditTour(itemName, newTourName,startName, goalName, dateTime, method);
            }
            return GetItems();
        }



        public IEnumerable<TourItem> AddLogs(string tourName, string logEntry, int rating, int actualTime, string description, DateTime date)
        {
            if (GetItems() != null)
            {
                if (tourItemDAO.AddLogs(tourName, logEntry, rating, actualTime, description, date) == null)
                    return null;
                return GetItems();
            }
            return GetItems();
        }

        public IEnumerable<TourItem> EditLogs(string tourName, string oldLogEntry, string logEntry, int rating, int actualTime, string description, DateTime? date = null, bool toDelete = false)
        {
            if (GetItems() != null)
            {
                if (tourItemDAO.EditLogs(tourName, oldLogEntry, logEntry, rating, actualTime, description, (DateTime)date, toDelete) == null)
                    return null;
                return GetItems();
            }
            return GetItems();
        }


        public IEnumerable<TourItem> RemoveTour(string itemName, bool mockItem = false)
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

        public List<TourItem> Import(string fileName)
        {
            IEnumerable<TourItem> tours = null;
            tours = tourItemDAO.Import(fileName);
            if (tours != null)
                return (List<TourItem>)tours;
            else
            {
                log.Info("Trying to Import Empty Tours");
                return null;
            }
        }

        public void Export(IEnumerable<TourItem> tourList, string filename)
        {
            tourItemDAO.Export(tourList, filename);
        }

        public async Task ShowMapTourAsync(string start, string end, string tourName)
        {
            await MapQuestConn.GetAndSaveImage(start, end, tourName);
        }

        public async Task CreatePDF(string tourName, TourItem mockItem = null)
        {
            await tourItemDAO.CreatePDF(tourName, mockItem);
        }

        public async Task CreateSummary(List<TourItem> mockList = null)
        {
            await tourItemDAO.CreateSummary(mockList);
        }
    }
}

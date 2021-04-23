using System;
using System.Collections.Generic;
using System.Linq;
using TourPlaner_Models;
using TourPlaner_DL;
using System.IO;
using Newtonsoft.Json;
using TourPlanner_DL;
using System.Threading.Tasks;

namespace TourPlaner_BL
{
    internal class TourManagerImp : ITourItemFactory
    {

        private readonly TourItemDAO tourItemDAO = new(); //anhand config file variieren
        private readonly MapQuestConn mapConnect = MapQuestConn.Instance();


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

        public IEnumerable<TourItem> AddTour(string itemName, string startName, string goalName, DateTime dateTime, int distance)
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
                        if (tourItemDAO.AddTour(itemName, startName, goalName, dateTime, distance) == null)
                            return null;
                        else
                            return tours;
                    }
                }
                return tours;
            }
            else
                tourItemDAO.AddTour(itemName, startName, goalName, dateTime, distance);
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

        public async Task CreatePDF(string tourName, string start, string goal, int distance)
        {
            await PDFHandler.CreatePDF(tourName, start, goal, distance);
        }

    }
}

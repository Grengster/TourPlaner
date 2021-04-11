﻿using System;
using System.Collections;
using System.Collections.Generic;
using TourPlaner_Models;

namespace TourPlaner_BL
{
    public interface ITourItemFactory {
        IEnumerable<TourItem> GetItems();
        IEnumerable<TourItem> Search(string itemName, bool caseSensitive = false);
        IEnumerable<TourItem> AddTour(string itemName, string startName, string goalName, DateTime dateTime);
        IEnumerable<TourItem> RemoveTour(string itemName);
        void ShowMapTourAsync(string start, string end, string tourName);
    }
}
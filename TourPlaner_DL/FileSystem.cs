﻿using System;
using System.Collections.Generic;
using TourPlaner_Models;

namespace TourPlaner_DL
{
    class FileSystem : IDataAccess
    {

        //private readonly string filePath;
        public FileSystem()
        {
            //this.filePath = "";
        }

        public List<TourItem> AddTour(string itemName, string startName, string goalName, DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public List<TourItem> GetItems()
        {
            //Get items from folder
            throw new NotImplementedException();
        }

        public List<TourItem> RemoveTour(string itemName)
        {
            throw new NotImplementedException();
        }
    }
}
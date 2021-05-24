using System.Collections.Generic;
using TourPlaner_Models;

namespace TourPlaner_BL
{
    public static class TourItemFactory
    {
        private static ITourItemFactory manager;

        public static ITourItemFactory GetInstance(bool mockDatabase = false)
        {
            if (manager == null && !mockDatabase)
            {
                manager = new TourManagerImp();
            }
            else if (manager == null && mockDatabase)
            {
                manager = new TourManagerImp(true);
            }
            return manager;
        }
    }
}

using System.Collections.Generic;
using TourPlaner_Models;

namespace TourPlaner_BL
{
    public static class TourItemFactory
    {
        private static ITourItemFactory manager;

        public static ITourItemFactory GetInstance()
        {
            if (manager == null)
            {
                manager = new TourManagerImp();
            }
            return manager;
        }
    }
}

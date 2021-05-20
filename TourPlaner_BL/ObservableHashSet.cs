using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlaner_BL
{
    public class ObservableHashSet<T> : ObservableCollection<T>
    {
            protected override void InsertItem(int index, T item)
            {
                if (Contains(item)) throw new Exception("Error: " + item.ToString());

                base.InsertItem(index, item);
            }

            protected override void SetItem(int index, T item)
            {
                int i = IndexOf(item);
                if (i >= 0 && i != index) throw new Exception("Error: " + item.ToString());

                base.SetItem(index, item);
            }
    }
}

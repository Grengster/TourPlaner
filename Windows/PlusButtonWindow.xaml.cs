using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TourPlaner
{
    /// <summary>
    /// Interaktionslogik für PlusButtonWindow.xaml
    /// </summary>
    public partial class PlusButtonWindow : Window
    {
        public PlusButtonWindow()
        {
            InitializeComponent();
            /*AddTourViewModel viewmodel = new();
            this.DataContext = viewmodel;
            Closing += viewmodel.ForceClosing; //NOT WORKING https://stackoverflow.com/questions/3683450/handling-the-window-closing-event-with-wpf-mvvm-light-toolkit
            */
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TourPlaner_Models;

namespace TourPlaner.UserControls
{
    /// <summary>
    /// Interaction logic for TourData.xaml
    /// </summary>
    public partial class TourLogs : UserControl
    {

        /// <summary>
        /// The ImageBytesProperty.
        /// </summary>
        public static readonly DependencyProperty TourDataProperty =
            DependencyProperty.Register(
                "TourLogInformations",
                typeof(ObservableCollection<TourItem>),
                typeof(TourData),
                new FrameworkPropertyMetadata(TourDataValueChangedCallBack));

        public TourLogs()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the image bytes.
        /// </summary>
        public ObservableCollection<TourItem> TourInformations { get; set; }

        public static event PropertyChangedEventHandler PropertyChanged;

        private static void TourDataValueChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            TourData control = sender as TourData;
            control.TourDatas.ItemsSource = args.NewValue as ObservableCollection<TourItem>;
        }


    }
}

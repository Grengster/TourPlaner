using System;
using System.Collections.Generic;
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


namespace TourPlaner
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _output = "Hello";
        private string _input = "Test";
        

        public string Output
        {
            get
            {
                return "blabla" + _input;
            }
        }

        public string Input
        {
            get
            {
                return _input;
            }
            set
            {
                _input = value;
            }
        }


        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TourPlaner_Models;
using TourPlaner_BL;
using System.Text.RegularExpressions;
using System.Windows.Markup;

namespace TourPlaner
{
    public class AddLogsViewModel : INotifyPropertyChanged
    {
        private string _rating, _actualTime;
        private string _logs, _weather;
        int ratingNum, actualTimeNum;

        public string Rating
        {
            get
            {
                Debug.Print("read Input");
                return _rating;
            }
            set
            {
                Debug.Print("write Input");
                if(Rating != value && IsTextAllowed(value))
                {
                    Debug.Print("set Input-value");
                    _rating = value;
                    Debug.Print("fire propertyChanged: Input");
                    OnPropertyChanged(nameof(Rating));
                }
            }
        }
        public string ActualTime
        {
            get
            {
                Debug.Print("read Input");
                return _actualTime;
            }
            set
            {
                Debug.Print("write Input");
                if (ActualTime != value && IsTextAllowed(value))
                {
                    Debug.Print("set Input-value");
                    _actualTime = value;
                    Debug.Print("fire propertyChanged: Input");
                    OnPropertyChanged(nameof(ActualTime));
                }
            }
        }
        public string Logs
        {
            get
            {
                Debug.Print("read Input");
                return _logs;
            }
            set
            {
                Debug.Print("write Input");
                if (Logs != value)
                {
                    Debug.Print("set Input-value");
                    _logs = value;
                    Debug.Print("fire propertyChanged: Input");
                    OnPropertyChanged(nameof(Logs));
                }
            }
        }
        public string Weather
        {
            get
            {
                Debug.Print("read Output");
                return _weather;
            }
            set
            {
                Debug.Print("write Output");
                if (_weather != value)
                {
                    Debug.Print("set Output");
                    _weather = value;
                    Debug.Print("fire propertyChanged: Output");
                    OnPropertyChanged(nameof(Weather));
                }
            }
        }

        private static readonly Regex _regex = new("[^0-9]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        public ICommand CloseLogsCommand { get; }
        public ICommand AddLogsCommand { get; }


        private ObservableCollection<TourItem> tourList = new();

        public ObservableCollection<TourItem> TourList
        {
            get
            {
                return tourList;
            }
            set
            {
                if (tourList != value)
                {
                    tourList = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AddLogsViewModel()
        {

            this.CloseLogsCommand = new RelayCommand((w) =>
            {
                //MessageBox.Show("Input: " + Input + "\n" + "Start: " + Start + "\n" + "End: " + Goal + "\n");
                Weather = null;
                Logs = null;
                CloseWindow(w);
            },
                (_) => { return true; }
            );
            this.AddLogsCommand = new RelayCommand((_) =>
            {
                if (Logs != null && Logs != "" && Weather != null && Weather != "" && Convert.ToInt32(Rating) >= 1 && Convert.ToInt32(Rating) <= 5 && Convert.ToInt32(ActualTime) >= 0)
                {
                    MessageBox.Show("Information has been added!");
                    CloseWindow(_);
                }
                else if (Convert.ToInt32(Rating) < 1 || Convert.ToInt32(Rating) > 5)
                {
                    MessageBox.Show("Please choose a Rating between 1 and 5!");
                }
                else if(Convert.ToInt32(ActualTime) < 0)
                {
                    MessageBox.Show("Please choose a valid Time");
                }
                else
                    MessageBox.Show("Please fill out all forms!");

            }, (_) =>
            {
                if (Logs != null && Logs != "" && Weather != null && Weather != "")
                    return true;
                else
                {
                    return false;
                }
            }
            );
        }

        public void ForceClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string msg = "Data is dirty. Close without saving?";
            MessageBoxResult result =
              MessageBox.Show(
                msg,
                "Data App",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
            {
                // If user doesn't want to close, cancel closure
                e.Cancel = true;
            }
            else
            {
                this.Weather = null;
                this.Logs = null;
            }
        }

        private static void CloseWindow(object w)
        {

            if (w != null && w is Window)
            {
                (w as Window).Close();
            }
        }

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Print($"propertyChanged \"{propertyName}\"");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

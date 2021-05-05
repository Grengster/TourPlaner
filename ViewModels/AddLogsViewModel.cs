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
        private int _rating, _actualTime;
        private string _logs, _description;

        public int Rating
        {
            get
            {
                Debug.Print("read Input");
                return _rating;
            }
            set
            {
                Debug.Print("write Input");
                if (Rating != value)
                {
                    Debug.Print("set Input-value");
                    _rating = value;
                    Debug.Print("fire propertyChanged: Input");
                    OnPropertyChanged(nameof(Rating));
                }
            }
        }
        public int ActualTime
        {
            get
            {
                Debug.Print("read Input");
                return _actualTime;
            }
            set
            {
                Debug.Print("write Input");
                if (ActualTime != value)
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
        public string Description
        {
            get
            {
                Debug.Print("read Output");
                return _description;
            }
            set
            {
                Debug.Print("write Output");
                if (_description != value)
                {
                    Debug.Print("set Output");
                    _description = value;
                    Debug.Print("fire propertyChanged: Output");
                    OnPropertyChanged(nameof(Description));
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
                Description = null;
                Logs = null;
                CloseWindow(w);
            },
                (_) => { return true; }
            );
            this.AddLogsCommand = new RelayCommand((_) =>
            {
                if (Logs != null || Description != null)
                {
                    MessageBox.Show("Information has been added!");
                    CloseWindow(_);
                }
                else
                {
                    MessageBox.Show("Please fill out all fields!");
                }
            }, (_) =>
            {
                if (Logs != null || Logs != "" && Description != null || Description != "")
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
                this.Description = null;
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

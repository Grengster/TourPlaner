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


namespace TourPlaner
{
    public class AddTourViewModel : INotifyPropertyChanged
    {
        private string _output = "Hello World!";
        private string _input;

        public string Input
        {
            get
            {
                Debug.Print("read Input");
                return _input;
            }
            set
            {
                Debug.Print("write Input");
                if (Input != value)
                {
                    Debug.Print("set Input-value");
                    _input = value;
                    Debug.Print("fire propertyChanged: Input");
                    OnPropertyChanged(nameof(Input));
                }
            }
        }

        public string Output
        {
            get
            {
                Debug.Print("read Output");
                return _output;
            }
            set
            {
                Debug.Print("write Output");
                if (_output != value)
                {
                    Debug.Print("set Output");
                    _output = value;
                    Debug.Print("fire propertyChanged: Output");
                    OnPropertyChanged();
                }
            }
        }

        public ICommand AddTourExecute { get; }
        public ICommand PlusButtonClose { get; }
        public ICommand AddItems { get; }


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

        public AddTourViewModel()
        {

            this.PlusButtonClose = new RelayCommand((w) =>
            {
                Input = null;
                CloseWindow(w);
            },
                (_) => { return true; }
            );
            this.AddItems = new RelayCommand((_) =>
            {

                var find = TourList.FirstOrDefault(x => x.Name == Input);
                if (find == null)
                {
                    MessageBox.Show(Input + " has been added!");
                    CloseWindow(_);
                }
                else
                {
                    MessageBox.Show(Input + " already exists, please use another name!");
                    Input = null;

                }
            }, (_) =>
            {
                if (Output != null && Output != "")
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
                this.Input = null;

        }

        private static void CloseWindow(object w)
        {

            if (w != null && w is Window)
            {
                (w as Window).Close();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Print($"propertyChanged \"{propertyName}\"");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

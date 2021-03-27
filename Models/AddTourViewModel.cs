using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

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


        private ObservableCollection<string> stringList = new ObservableCollection<string>();

        public ObservableCollection<string> StringList
        {
            get
            {
                return stringList;
            }
            set
            {
                if (stringList != value)
                {
                    stringList = value;
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
                if(!StringList.Contains(Input))
                {
                    MessageBox.Show(Input + " has been added!");
                    CloseWindow(_);
                }
                else
                {
                    MessageBox.Show("Duplicate entries not allowed!");
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

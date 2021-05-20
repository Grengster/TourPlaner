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
using System.Windows.Data;
using System.Globalization;

namespace TourPlaner
{
    public class AddTourViewModel : INotifyPropertyChanged
    {
        private string _output = "Hello World!";
        private string _input, _start, _goal;
        SelectedMethod _method;
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
        public string Start
        {
            get
            {
                Debug.Print("read Input");
                return _start;
            }
            set
            {
                Debug.Print("write Input");
                if (Start != value)
                {
                    Debug.Print("set Input-value");
                    _start = value;
                    Debug.Print("fire propertyChanged: Input");
                    OnPropertyChanged(nameof(Start));
                }
            }
        }

        public HashSet<DateTime> dates = new HashSet<DateTime>();

        public HashSet<DateTime> Dates {
            get
            {
                return dates;
            }
            set
            {
                dates = value;
                OnPropertyChanged();
            }
        }

        private static readonly Regex _regex = new("[^0-9]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        public SelectedMethod Method
        {
            get
            {
                Debug.Print("read Input");
                return _method;
            }
            set
            {
                Debug.Print("write Input");
                if (Method != value)
                {
                    Debug.Print("set Input-value");
                    _method = value;
                    Debug.Print("fire propertyChanged: Input");
                    OnPropertyChanged(nameof(_method));
                }
            }
        }

        public string Goal
        {
            get
            {
                Debug.Print("read Input");
                return _goal;
            }
            set
            {
                Debug.Print("write Input");
                if (Goal != value)
                {
                    Debug.Print("set Input-value");
                    _goal = value;
                    Debug.Print("fire propertyChanged: Input");
                    OnPropertyChanged(nameof(Goal));
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

        private DateTime selectedDate;
        public DateTime SelectedDate
        {
            get
            {
                return selectedDate;
            }
            set
            {
                if (selectedDate != value)
                {
                    selectedDate = value;
                    OnPropertyChanged(nameof(SelectedDate));
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
                //MessageBox.Show("Input: " + Input + "\n" + "Start: " + Start + "\n" + "End: " + Goal + "\n");
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
                if (Output != null || Output != "" && Start != "" && Start != null || Goal != "" && Goal != null || Input != "" && Input != null)
                    return true;
                else
                {
                    return false;
                }
            }
            );
        }

        public void RefreshList()
        {
            foreach (var item in this.TourList)
            {
                this.Dates.Add(item.TourInfo.CreationTime.Date);
            }
            OnPropertyChanged(nameof(Dates));
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

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Print($"propertyChanged \"{propertyName}\"");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }

    public class LookupConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var date = (DateTime)values[0];
            var dates = values[1] as HashSet<DateTime>;
            return dates.Contains(date);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ComboEnumBinding : MarkupExtension
    {
        public Type EnumType { get; private set; }

        public ComboEnumBinding(Type enumType)
        {
            if (enumType is null || !enumType.IsEnum)
                throw new Exception("EnumType must not be null and of type Enum!");

            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(EnumType);
        }
    }

    
}

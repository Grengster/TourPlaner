using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TourPlaner_Models;
using TourPlaner_BL;

namespace TourPlaner
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _output = "";
        private string _input;
        public AddTourViewModel plusMod = new();
        public ObservableCollection<string> stringList = new();
        

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


        public ICommand ExecuteCommand { get; }
        public ICommand RemoveItems { get; }
        public ICommand AddItems { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        //xaml gets from viewmodel -> only provide data here

        public MainViewModel()
        {
            /*AddTourViewModel plusButtonVM;
            Debug.Print("ctor MainViewModel");
            this.ExecuteCommand = new RelayCommand((_) =>
            {
                PlusButtonWindow plusWin = new();
                plusButtonVM = (AddTourViewModel)plusWin.DataContext; //creates link to new window's data --> IMPORTANT
                Debug.Print("Before");
                foreach (var item in StringList)
                {
                    plusButtonVM.StringList.Add(item);
                }
                plusWin.ShowDialog();  //when using List List<string> tempList = new List<string>(stringList); StringList = tempList; 
                if (plusButtonVM.Input != null)
                    stringList.Add(plusButtonVM.Input);
            }, (_) => { return true; }
                );*/
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Print($"propertyChanged \"{propertyName}\"");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TourPlaner_BL;
using TourPlaner_Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace TourPlaner.ViewModels
{
    public class TourFolderVM : ViewModelBase
    {
        private readonly ITourItemFactory tourItemFactory;
        private string searchName;
        private string _output = "";
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
        private TourItem currentItem;
        private ICommand searchCommand;
        private ICommand clearCommand;
        public ICommand executeCommand;
        public ICommand RemoveItems { get; }
        public ICommand AddItems { get; }
        public ICommand SearchCommand => searchCommand ??= new RelayCommand(Search);
        public ICommand ClearCommand => clearCommand ??= new RelayCommand(Clear);
        public ICommand ExecuteCommand => executeCommand ??= new RelayCommand(AddTourWindow);
        public new event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<TourItem> Tours { get; set; }
        public TourItem CurrentItem
        {
            get
            {
                return currentItem;
            }
            set
            {
                if((currentItem != value) && (value != null))
                {
                    currentItem = value;
                    RaisePropertyChangedEvent(nameof(CurrentItem));
                }
            }
        }
        public string SearchName
        {
            get
            {
                return searchName;
            }
            set
            {
                if (searchName != value)
                {
                    searchName = value;
                    RaisePropertyChangedEvent(nameof(SearchName));
                }
            }
        }


        public TourFolderVM()
        {
            this.tourItemFactory = TourItemFactory.GetInstance();
            InitListbox();
        }


        private void InitListbox()
        {
            Tours = new ObservableCollection<TourItem>();
            FillListBox();
        }

        private void FillListBox()
        {
            if(this.tourItemFactory.GetItems() == null)
                Tours.Add(new TourItem { Name = "No Tours added yet"});
            else
                foreach (TourItem item in this.tourItemFactory.GetItems())
                {
                    Tours.Add(item);
                }
        }

        private void AddTourWindow(object commandParameter)
        {
            AddTourViewModel plusButtonVM;
            PlusButtonWindow plusWin = new();
            plusButtonVM = (AddTourViewModel)plusWin.DataContext; //creates link to new window's data --> IMPORTANT
            Debug.Print("Before");
            foreach (var item in Tours)
            {
                plusButtonVM.TourList.Add(item);
            }
            plusWin.ShowDialog();  //when using List List<string> tempList = new List<string>(stringList); StringList = tempList; 
            if (plusButtonVM.Input != null)
            {
                if (this.tourItemFactory.AddTour(plusButtonVM.Input, plusButtonVM.Start, plusButtonVM.Goal, DateTime.Now) == null)
                    MessageBox.Show("There has been an error inserting your tour, please try again!");
                Tours.Clear();
                FillListBox();
            }
        }

        private void Search(object commandParameter)
        {
            IEnumerable foundItems = this.tourItemFactory.Search(SearchName);
            Tours.Clear();
            foreach (TourItem item in foundItems)
            {
                Tours.Add(item);
            }
        }

        private void Clear(object commandParameter)
        {
            Tours.Clear();
            SearchName = "";
            FillListBox();
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Print($"propertyChanged \"{propertyName}\"");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

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
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using log4net;
using Microsoft.VisualBasic;
using log4net.Config;

namespace TourPlaner.ViewModels
{
    public class TourFolderVM : ViewModelBase
    {
        #region Declarations
        private readonly ITourItemFactory tourItemFactory;
        private string searchName;
        private string _output = "";
        private string _input;
        public new string Input
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
        public new string Output
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
        private UserRating currentLog;
        private TourItem currentItem;
        private ICommand searchCommand, exportCommand, updateCommand, importCommand, removeLogCommand, clearCommand, executeCommand, removeCommand, showMap, createCommand, logsCommand, createSummaryCommand, editLogsCommand;
        public ICommand RemoveItems { get; }
        public ICommand AddItems { get; }
        public ICommand SearchCommand => searchCommand ??= new RelayCommand(Search);
        public ICommand ShowMap => showMap ??= new RelayCommand(Show);
        public ICommand CreateCommand => createCommand ??= new RelayCommand(Create, (_) => { if (currentItem == null || this.tourItemFactory.GetItems() == null) return false; else return true; });
        public ICommand UpdateCommand => updateCommand ??= new RelayCommand(EditTour, (_) => { if (currentItem == null || this.tourItemFactory.GetItems() == null) return false; else return true; });
        public ICommand CreateSummaryCommand => createSummaryCommand ??= new RelayCommand(CreateSummary, (_) => { if (currentItem == null || this.tourItemFactory.GetItems() == null) return false; else return true; });
        public ICommand ClearCommand => clearCommand ??= new RelayCommand(Clear);
        public ICommand ExecuteCommand => executeCommand ??= new RelayCommand(AddTourWindow);
        public ICommand LogsCommand => logsCommand ??= new RelayCommand(AddLogWindow, (_) => { if (currentItem == null || this.tourItemFactory.GetItems() == null || currentLog == null) return false; else return true; });
        public ICommand EditLogsCommand => editLogsCommand ??= new RelayCommand(EditLogWindow, (_) => { if (currentItem == null || this.tourItemFactory.GetItems() == null || currentLog == null || currentLog.Logs == "No Logs added yet.") return false; else return true; });
        public ICommand RemoveCommand => removeCommand ??= new RelayCommand(RemoveTourWindow);
        public ICommand RemoveLogCommand => removeLogCommand ??= new RelayCommand(RemoveLogWindow, (_) => { if (currentItem == null || this.tourItemFactory.GetItems() == null || currentLog == null || currentLog.Logs == "No Logs added yet.") return false; else return true; });
        public ICommand ExportCommand => exportCommand ??= new RelayCommand(ExportTours, (_) => { if (this.tourItemFactory.GetItems() == null) return false; else return true; });
        public ICommand ImportCommand => importCommand ??= new RelayCommand(ImportTours);
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
                if ((currentItem != value) && (value != null))
                {
                    currentItem = value;
                    RaisePropertyChangedEvent(nameof(TourInformations));
                    RaisePropertyChangedEvent(nameof(TourLogInformations));
                    RaisePropertyChangedEvent(nameof(CurrentMap));
                    RaisePropertyChangedEvent(nameof(CurrentPic));
                    RaisePropertyChangedEvent(nameof(CurrentItem));
                }
            }
        }

        public UserRating CurrentLog
        {
            get
            {
                return currentLog;
            }
            set
            {
                if ((currentLog != value) && (value != null))
                {
                    currentLog = value;
                    RaisePropertyChangedEvent(nameof(CurrentLog));
                }
            }
        }

        

        public ImageSource CurrentMap
        {
            get
            {
                if (this?.CurrentItem?.TourInfo?.MapImagePath != null)
                {
                    try
                    {
                        if (File.Exists(this?.CurrentItem?.TourInfo?.MapImagePath))
                        {
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            bitmap.UriSource = new Uri(this?.CurrentItem?.TourInfo?.MapImagePath);
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                            return bitmap;
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Exception was thrown when setting Tour Map Image: {e}");
                    }
                }
                return null;
            }
        }


        public ImageSource CurrentPic
        {
            get
            {
                var previewString = $@"C:\Users\Gregor\source\repos\TourPlaner\TourPlaner_DL\GoalPics\{this?.CurrentItem?.Name}.png";
                if (previewString != null)
                {
                    try
                    {
                        if (File.Exists(previewString))
                        {
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            bitmap.UriSource = new Uri(previewString);
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                            return bitmap;
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Exception was thrown when setting Tour Map Image: {e}");
                    }
                }
                return null;
            }
        }

        public ObservableCollection<TourItem> TourInformations
        {
            get
            {
                if (CurrentItem != null)
                {
                    var collection = new ObservableCollection<TourItem>
                    {
                        CurrentItem
                    };
                    return collection;
                }
                else
                    return new ObservableCollection<TourItem>();
            }
        }

        public ObservableCollection<UserRating> TourLogInformations
        {
            get
            {
                if (CurrentItem != null)
                {
                    var collection = new ObservableCollection<UserRating>();
                    foreach (var item in CurrentItem.TourLogs)
                    {
                        collection.Add(item);
                    }
                    if(CurrentItem.TourLogs.Count > 0)
                        return collection;
                    else
                    {
                        return new ObservableCollection<UserRating>()
                        {
                            new UserRating()
                            {
                                Logs = "No Logs added yet.",
                                Weather = "-"
                            }
                        };
                    }
                        
                }
                else
                    return new ObservableCollection<UserRating>();
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



        private static readonly ILog log = LogManager.GetLogger(typeof(TourFolderVM));
        #endregion
        

        public TourFolderVM()
        {
            try
            {
                this.tourItemFactory = TourItemFactory.GetInstance();
                log.Info("Started ItemFactory...");
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            InitListbox();
        }


        private void InitListbox()
        {
            log.Info("test");
            Tours = new ObservableCollection<TourItem>();
            FillListBox();
        }

        private void FillListBox()
        {
            if (this.tourItemFactory.GetItems() == null)
                Tours.Add(new TourItem { Name = "No Tours added yet" });
            else
                foreach (TourItem item in this.tourItemFactory.GetItems())
                {
                    Tours.Add(item);
                    log.Info("Added Tour called: " + item.Name);
                }
        }

        private void AddLogWindow(object commandParameter)
        {
            AddLogsViewModel logVM;
            AddLogs logsWin = new();
            logVM = (AddLogsViewModel)logsWin.DataContext; //creates link to new window's data --> IMPORTANT

            Debug.Print("Before");
            logsWin.ShowDialog();  //when using List List<string> tempList = new List<string>(stringList); StringList = tempList; 
            if (logVM.Logs != null || logVM.Weather != null )
            {
                try
                {
                    var temp = this.tourItemFactory.AddLogs(currentItem.Name, logVM.Logs, Convert.ToInt32(logVM.Rating), Convert.ToInt32(logVM.ActualTime), logVM.Weather, DateTime.Today);
                    if (temp == null)
                        throw (new Exception("Error inserting Logs"));
                }
                catch (Exception e)
                {
                    log.Error(e);
                    MessageBox.Show("There has been an error inserting your tour, please try again!");
                }
                RaisePropertyChangedEvent(nameof(TourInformations));
                Tours.Clear();
                FillListBox();
            }
        }

        

        private void RemoveLogWindow(object commandParameter)
        {
            if (commandParameter != null)
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete: " + commandParameter.ToString() + "?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                    if (this.tourItemFactory.EditLogs(currentItem.Name, commandParameter.ToString(), "Test", 1, 1, "Test", DateTime.Today, true) == null)
                        MessageBox.Show("There has been an error deleting your log, please try again!");
                Tours.Clear();
                currentItem = null;
                currentLog = null;
                RaisePropertyChangedEvent(nameof(CurrentMap));
                RaisePropertyChangedEvent(nameof(CurrentPic));
                RaisePropertyChangedEvent(nameof(CurrentItem));
                RaisePropertyChangedEvent(nameof(CurrentLog));
                FillListBox();
            }
            else
            {
                MessageBox.Show("Please click on a Tour and then click on this Button again to delete a tour!");
            }
        }


        private void EditLogWindow(object commandParameter)
        {
            AddLogsViewModel logVM;
            AddLogs logsWin = new();
            logVM = (AddLogsViewModel)logsWin.DataContext; //creates link to new window's data --> IMPORTANT
            string oldLog = CurrentLog.Logs;
            logVM.Weather = CurrentLog.Weather;
            logVM.ActualTime = CurrentLog.ActualTime.ToString();
            logVM.Logs = CurrentLog.Logs;
            logVM.Rating = CurrentLog.Rating.ToString();

            Debug.Print("Before");
            logsWin.ShowDialog();  //when using List List<string> tempList = new List<string>(stringList); StringList = tempList; 
            if (logVM.Logs != null || logVM.Weather != null)
            {
                try
                {
                    var temp = this.tourItemFactory.EditLogs(currentItem.Name, oldLog, logVM.Logs, Convert.ToInt32(logVM.Rating), Convert.ToInt32(logVM.ActualTime), logVM.Weather, DateTime.Today);
                    if (temp == null)
                        throw (new Exception("Error editing Logs"));
                }
                catch (Exception e)
                {
                    log.Error(e);
                    MessageBox.Show("There has been an error inserting your tour, please try again!");
                }
                RaisePropertyChangedEvent(nameof(TourInformations));
                Tours.Clear();
                FillListBox();
            }
        }

        private async void AddTourWindow(object commandParameter)
        {
            AddTourViewModel plusButtonVM;
            PlusButtonWindow plusWin = new();
            plusButtonVM = (AddTourViewModel)plusWin.DataContext; //creates link to new window's data --> IMPORTANT
            Debug.Print("Before");
            foreach (var item in Tours)
            {
                plusButtonVM.TourList.Add(item);
            }
            plusButtonVM.RefreshList();

            plusWin.ShowDialog();  //when using List List<string> tempList = new List<string>(stringList); StringList = tempList; 
            if (plusButtonVM.Input != null)
            {
                try
                {
                    var tempVar = await this.tourItemFactory.AddTourAsync(plusButtonVM.Input, plusButtonVM.Start, plusButtonVM.Goal, plusButtonVM.SelectedDate, EnumDescriptionExtension.GetDescription(plusButtonVM.Method));
                    if (tempVar == null)
                        MessageBox.Show("There has been an error inserting your tour, please try again!");
                    else
                        await this.tourItemFactory.ShowMapTourAsync(plusButtonVM.Start, plusButtonVM.Goal, plusButtonVM.Input);

                }
                catch (Exception e)
                {
                    log.Error(e);
                }
                Tours.Clear();
                FillListBox();
            }
        }

        private async void EditTour(object commandParameter)
        {
            AddTourViewModel plusButtonVM;
            PlusButtonWindow plusWin = new();
            plusButtonVM = (AddTourViewModel)plusWin.DataContext; //creates link to new window's data --> IMPORTANT
            plusButtonVM.Input = CurrentItem.Name;
            plusButtonVM.Goal = CurrentItem.TourInfo.Goal;
            plusButtonVM.Start = CurrentItem.TourInfo.Start;
            plusButtonVM.SelectedDate = CurrentItem.TourInfo.CreationTime;

            Debug.Print("Before");
            foreach (var item in Tours)
            {
                plusButtonVM.TourList.Add(item);
            }
            plusButtonVM.RefreshList();

            plusWin.ShowDialog();  //when using List List<string> tempList = new List<string>(stringList); StringList = tempList; 
            if (plusButtonVM.Input != null)
            {
                try
                {
                    var tempVar = await this.tourItemFactory.EditTour(CurrentItem.Name, plusButtonVM.Input, plusButtonVM.Start, plusButtonVM.Goal, plusButtonVM.SelectedDate, EnumDescriptionExtension.GetDescription(plusButtonVM.Method));
                    if (tempVar == null)
                        MessageBox.Show("There has been an error updating your tour, please try again!");
                    else
                        await this.tourItemFactory.ShowMapTourAsync(plusButtonVM.Start, plusButtonVM.Goal, plusButtonVM.Input);

                }
                catch (Exception e)
                {
                    log.Error(e);
                }
                Tours.Clear();
                FillListBox();
            }
        }

        private void RemoveTourWindow(object commandParameter)
        {
            if (commandParameter != null)
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete: " + commandParameter.ToString() + "?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                    if (this.tourItemFactory.RemoveTour(commandParameter.ToString()) == null)
                        MessageBox.Show("There has been an error deleting your tour, please try again!");
                Tours.Clear();
                currentItem = null;
                RaisePropertyChangedEvent(nameof(CurrentMap));
                RaisePropertyChangedEvent(nameof(CurrentPic));
                FillListBox();
            }
            else
            {
                MessageBox.Show("Please click on a Tour and then click on this Button again to delete a tour!");
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

        private void Show(object commandParameter)
        {
            //MessageBox.Show(this.tourItemFactory.ShowMapTourAsync(plusButtonVM.Input, plusButtonVM.Start, plusButtonVM.Goal));
        }

        private void Create(object commandParameter)
        {
            if (commandParameter != null)
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to create PDF for: " + commandParameter.ToString() + "?", "Select Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                    if (this?.CurrentItem?.TourInfo?.MapImagePath != null)
                    {
                        var task = Task.Run(async () => await this.tourItemFactory.CreatePDF(commandParameter.ToString()));
                        if (task == null)
                            MessageBox.Show("There has been an error inserting your tour, please try again!");
                    }
                Tours.Clear();
                FillListBox();
            }
            else
            {
                MessageBox.Show("Please click on a Tour and then click on this Button again to create a single-detailed-Report.");
            }    
        }

        private void CreateSummary(object commandParameter)
        {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to create a summary PDF?", "Select Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        var task = Task.Run(async () => await this.tourItemFactory.CreateSummary());
                        if (task == null)
                            MessageBox.Show("There has been an error inserting your tour, please try again!");
                    }
                Tours.Clear();
                FillListBox();
        }

        private void ImportTours(object commandParameter)
        {
            string messageBoxResult = Interaction.InputBox("Type in your Filename you wish to import?", "Title", "Default Text");
            if (messageBoxResult != "" || messageBoxResult != null)
            {
                var task = this.tourItemFactory.Import(messageBoxResult);
                if (task == null)
                    MessageBox.Show("There has been an error inserting your tour, please try again!");
            }
            Tours.Clear();
            FillListBox();
        }


        private void ExportTours(object commandParameter)
        {
            string messageBoxResult = Interaction.InputBox("Type in your Filename you wish to import?", "Title", "Default Text");
            if (messageBoxResult != "" || messageBoxResult != null)
            {
                var tourList = this.tourItemFactory.GetItems();
                if (tourList != null)
                this.tourItemFactory.Export(this.tourItemFactory.GetItems(), messageBoxResult);
            }
            Tours.Clear();
            FillListBox();
        }


        protected virtual new void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Print($"propertyChanged \"{propertyName}\"");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
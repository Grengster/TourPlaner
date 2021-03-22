﻿using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TourPlaner
{
    public class MainViewModel : INotifyPropertyChanged
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

        public ICommand ExecuteCommand { get; }
        public ICommand RemoveItems { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            Debug.Print("ctor MainViewModel");
            this.ExecuteCommand = new RelayCommand((_) => { PlusButtonWindow plusWin = new PlusButtonWindow(); plusWin.Show(); }, (_) => { return true; } 
                );
            /* Dont know how to do this
            this.RemoveItems = new RelayCommand((_) =>

            {
                var myListView = (ListView)this.FindName("FuckYou");
                foreach (ListViewItem eachItem in listView1.SelectedItems)
                {
                    listView1.Items.Remove(eachItem);
                }
            },
                (_) => { return true; }
            );
            */


            #region Simpler Solution

            // Alternative: https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern#id0090030
            // this.ExecuteCommand = new RelayCommand(() => Output = $"Hello {Input}!");

            #endregion
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Print($"propertyChanged \"{propertyName}\"");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

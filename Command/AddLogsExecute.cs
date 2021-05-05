using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TourPlaner_Models;
using TourPlaner_BL;
using TourPlaner.ViewModels;
using System.Diagnostics;
using System;

namespace TourPlaner
{
    class AddLogsExecute : ICommand
    {
        private readonly ViewModelBase _mainViewModel;
        public AddLogsExecute(ViewModelBase mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _mainViewModel.PropertyChanged += (sender, args) =>
            {
                Debug.Print("command: reveived prop changed");
                if (args.PropertyName == "Input")
                {
                    Debug.Print("command: reveived prop changed of Input");
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            };
        }

        public bool CanExecute(object parameter)
        {
            Debug.Print("command: can execute");
            return !string.IsNullOrWhiteSpace(_mainViewModel.Input);
        }

        public void Execute(object parameter)
        {
            Debug.Print("command: execute");
            _mainViewModel.Output = _mainViewModel.Input;
            _mainViewModel.Input = string.Empty;
            Debug.Print("command: execute done");
        }

        public event EventHandler CanExecuteChanged;
    }
}

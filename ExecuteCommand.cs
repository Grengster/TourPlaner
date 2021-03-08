using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TourPlaner
{
    public class ExecuteCommand : ICommand
    {
        private readonly MainViewModel _mainViewModel;
        public ExecuteCommand(MainViewModel mainViewModel)
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
            Random rn = new Random();
            Debug.Print("command: execute");
            _mainViewModel.Output = $"Hello {_mainViewModel.Input}, nice to see you!";
            // mw.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0, 0));
            Application.Current.MainWindow.Background = PickBrush();
            ColorAnimation ca = new ColorAnimation(Colors.Blue, new Duration(TimeSpan.FromSeconds(4)));
            Application.Current.MainWindow.Background = new SolidColorBrush(Colors.Red);
            Application.Current.MainWindow.Background.BeginAnimation(SolidColorBrush.ColorProperty, ca);
            _mainViewModel.Input = string.Empty;
            Debug.Print("command: execute done");
        }

        private Brush PickBrush()
        {
            Brush result = Brushes.Transparent;

            Random rnd = new Random();

            Type brushesType = typeof(Brushes);

            PropertyInfo[] properties = brushesType.GetProperties();

            int random = rnd.Next(properties.Length);
            result = (Brush)properties[random].GetValue(null, null);

            return result;
        }




        public event EventHandler CanExecuteChanged;
    }
}

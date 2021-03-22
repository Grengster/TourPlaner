using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace TourPlaner
{
    public class PlusButtonViewModel : INotifyPropertyChanged
    {

        private string _output = "Hello World!";
        private string _input;
        private PlusButtonWindow tempName = null;

        public class MyCommands
        {
            public static readonly ICommand CloseCommand =
                new RelayCommand(o => ((Window)o).Close());
        }

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

        public ICommand PlusButtonExecute { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public PlusButtonViewModel()
        {
            Debug.Print("ctor MainViewModel");
            this.PlusButtonExecute = new RelayCommand((_) => 
            { 
                
                PlusButtonWindow plusWin = new PlusButtonWindow(); plusWin.Show();
                tempName = plusWin;

            },
                (_) => { return true; }
            );

            this.CloseShitExecute = new RelayCommand((w) =>
            {
                if(w != null && w is Window)
                {
                    (w as Window).Close();
                }
            },
                (_) => { return true; }
            );
            #region Simpler Solution

            // Alternative: https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern#id0090030
            //this.PlusButtonViewModel = new RelayCommand((_) => Output = $"Hello {Input}!");

            #endregion
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Print($"propertyChanged \"{propertyName}\"");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand CloseShitExecute { get; }

        public void CloseShit(PlusButtonWindow temp)
        {
            temp.Close();
        }


    }
}

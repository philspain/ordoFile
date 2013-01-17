using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;

namespace ordoFile.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        Visibility _windowVisible;
        string _windowOpacity;

        public MainViewModel()
        {
            OnInitialise();
        }

        public Visibility WindowVisible
        {
            get { return _windowVisible; }
            set
            {
                _windowVisible = value;
                OnPropertyChanged("WindowVisible");
            }
        }

        public string WindowOpacity
        {
            get { return _windowOpacity; }
            set
            {
                _windowOpacity = value;
                OnPropertyChanged("WindowOpacity");
            }
        }

        void OnInitialise()
        {
            WindowVisible = Visibility.Visible;
            WindowOpacity = "0.5";
        }
    }
}

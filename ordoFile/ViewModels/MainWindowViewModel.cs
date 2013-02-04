using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows;

namespace ordoFile.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        BitmapImage _minimise,
                    _minimise_MouseOver,
                    _exit,
                    _exit_MouseOver;

        bool _minimiseIsMouseOver,
             _exitIsMouseOver;

        int _screenHeight, _screenWidth;

        public MainWindowViewModel()
        {
            LoadImages();
        }

        private void LoadImages()
        {
            _minimise = new BitmapImage();
            _minimise.BeginInit();
            _minimise.StreamSource = System.Windows.Application.GetResourceStream(
                new Uri("/Resources/Minimise.gif", UriKind.Relative)).Stream;
            _minimise.EndInit();

            _minimise_MouseOver = new BitmapImage();
            _minimise_MouseOver.BeginInit();
            _minimise_MouseOver.StreamSource = System.Windows.Application.GetResourceStream(
                new Uri("/Resources/Minimise_MouseOver.gif", UriKind.Relative)).Stream;
            _minimise_MouseOver.EndInit();

            _exit = new BitmapImage();
            _exit.BeginInit();
            _exit.StreamSource = System.Windows.Application.GetResourceStream(
                new Uri("/Resources/Exit.gif", UriKind.Relative)).Stream;
            _exit.EndInit();

            _exit_MouseOver = new BitmapImage();
            _exit_MouseOver.BeginInit();
            _exit_MouseOver.StreamSource = System.Windows.Application.GetResourceStream(
                new Uri("/Resources/Exit_MouseOver.gif", UriKind.Relative)).Stream;
            _exit_MouseOver.EndInit();

            OnPropertyChanged("MinmiseImage");
        }

        public BitmapImage MinimiseImage
        {
            get
            {
                if (_minimiseIsMouseOver)
                    return _minimise_MouseOver;
                else
                    return _minimise;
            }
        }

        public void MinimiseMouseEnter()
        {
            _minimiseIsMouseOver = true; 
            OnPropertyChanged("MinimiseImage");
        }

        public void MinimiseMouseExit(object sender, RoutedEventArgs reArgs)
        {
            _minimiseIsMouseOver = false;
            OnPropertyChanged("MinimiseImage");
        }       

        public BitmapImage ExitImage
        {
            get
            {
                if (_exitIsMouseOver)
                    return _exit_MouseOver;
                else
                    return _exit;
            }
        }

        public void ExitMouseEnter(object sender, RoutedEventArgs reArgs)
        {
            _exitIsMouseOver = true;
            OnPropertyChanged("ExitImage");
        }

        public void ExitMouseExit(object sender, RoutedEventArgs reArgs)
        {
            _exitIsMouseOver = false;
            OnPropertyChanged("ExitImage");
        }
    }
}

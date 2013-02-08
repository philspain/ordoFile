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

        TrayApp _trayApp;

        OrganisationSyncer _organisationSyncer;

        bool _windowVisible;

        public MainWindowViewModel(TrayApp trayApp, OrganisationSyncer organisationSyncer)
        {
            _trayApp = trayApp;
            _organisationSyncer = organisationSyncer;
            OnInitialise();
        }

        private void LoadImages()
        {
            // Image to be used for minimise icon
            _minimise = new BitmapImage();
            _minimise.BeginInit();
            _minimise.StreamSource = System.Windows.Application.GetResourceStream(
                new Uri("/Resources/images/Minimise.gif", UriKind.Relative)).Stream;
            _minimise.EndInit();

            // Image to be used for minimise icon when mouse hovers over it
            _minimise_MouseOver = new BitmapImage();
            _minimise_MouseOver.BeginInit();
            _minimise_MouseOver.StreamSource = System.Windows.Application.GetResourceStream(
                new Uri("/Resources/images/Minimise_MouseOver.gif", UriKind.Relative)).Stream;
            _minimise_MouseOver.EndInit();

            // Image for exit application button
            _exit = new BitmapImage();
            _exit.BeginInit();
            _exit.StreamSource = System.Windows.Application.GetResourceStream(
                new Uri("/Resources/images/Exit.gif", UriKind.Relative)).Stream;
            _exit.EndInit();

            // Image for exit application button when mouse hovers over it
            _exit_MouseOver = new BitmapImage();
            _exit_MouseOver.BeginInit();
            _exit_MouseOver.StreamSource = System.Windows.Application.GetResourceStream(
                new Uri("/Resources/images/Exit_MouseOver.gif", UriKind.Relative)).Stream;
            _exit_MouseOver.EndInit();
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

        

        public bool WindowVisible
        {
            get { return _windowVisible; }
            set
            {
                _windowVisible = value;
                System.Diagnostics.Debug.WriteLine("MainView set: " + _windowVisible);
                OnPropertyChanged("WindowVisible");
            }
        }

        void OnInitialise()
        {

            LoadImages();
            WindowVisible = _organisationSyncer.WindowVisible;
            _organisationSyncer.UpdateWindowVisibility += CheckVisibilityStatus;
        }

        void CheckVisibilityStatus(object sender, EventArgs e)
        {
            WindowVisible = _organisationSyncer.WindowVisible;
        }
    }
}

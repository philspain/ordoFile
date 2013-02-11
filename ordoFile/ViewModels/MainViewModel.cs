using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using ordoFile.Commands;

namespace ordoFile.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        // Views for data-binding to MainView's content grid.
        UserControl _foregroundView, 
                    _backgroundView,
                    _presetsView,
                    _selectedView;

        ICommand _setContentViewCommand;

        bool _foregroundImageVisible,
             _backgroundImageVisible,
             _presetsImageVisible;

        public MainViewModel()
        {
            Initialise();
        }

        /// <summary>
        /// Property for getting/setting whether or not the foreground
        /// organiser button's background image is visible.
        /// </summary>
        public bool ForegroundImageVisible
        {
            get { return _foregroundImageVisible; }
            set
            {
                _foregroundImageVisible = value;
                OnPropertyChanged("ForegroundImageVisible");
            }
        }

        /// <summary>
        /// Property for getting/setting whether or not the background
        /// organiser button's background image is visible.
        /// </summary>
        public bool BackgroundImageVisible
        {
            get { return _backgroundImageVisible; }
            set
            {
                _backgroundImageVisible = value;
                OnPropertyChanged("BackgroundImageVisible");
            }
        }

        /// <summary>
        /// Property for getting/setting whether or not the presets
        /// button's background image is visible.
        /// </summary>
        public bool PresetsImageVisible
        {
            get { return _presetsImageVisible; }
            set
            {
                _presetsImageVisible = value;
                OnPropertyChanged("PresetsImageVisible");
            }
        }


        /// <summary>
        /// View that is to be shown in MainView's content grid.
        /// </summary>
        public UserControl SelectedView
        {
            get { return _selectedView; }
            set 
            { 
                _selectedView = value;
                OnPropertyChanged("SelectedView");
            }
        }

        /// <summary>
        /// View for background organisation.
        /// </summary>
        public UserControl BackgroundView
        {
            set { _backgroundView = value; }
        }

        /// <summary>
        /// View for foreground organisation.
        /// </summary>
        public UserControl ForegroundView
        {
            set { _foregroundView = value; }
        }

        /// <summary>
        /// View for foreground organisation.
        /// </summary>
        public UserControl PresetsView
        {
            set { _presetsView = value; }
        }

        public ICommand SetContentViewCommand
        {
            get
            {
                if (_setContentViewCommand == null)
                    _setContentViewCommand = new DelegateCommand<object>(SetContentView);

                return _setContentViewCommand;
            }
        }

        void Initialise()
        {
            ForegroundImageVisible = true;

            BackgroundImageVisible = PresetsImageVisible = false;
        }

        void SetContentView(object text)
        {
            string buttonContent = ((string) text);

            switch (buttonContent)
            {
                case "ForegroundOrganiser":
                    SelectedView = _foregroundView;
                    ForegroundImageVisible = true;
                    BackgroundImageVisible = PresetsImageVisible = false;
                    break;
                case "BackgroundOrganiser":
                    SelectedView = _backgroundView;
                    BackgroundImageVisible = true;
                    ForegroundImageVisible = PresetsImageVisible = false;
                    break;
                case "Presets":
                    SelectedView = _presetsView;
                    PresetsImageVisible = true;
                    BackgroundImageVisible = ForegroundImageVisible = false;
                    break;
            }
        }
    }
}

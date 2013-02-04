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

        int _screenWidth, _screenHeight;

        public MainViewModel()
        {
            
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

        void SetContentView(object text)
        {
            string buttonContent = ((string) text);

            switch (buttonContent)
            {
                case "Foreground Organiser":
                    SelectedView = _foregroundView;
                    break;
                case "Background Organiser":
                    SelectedView = _backgroundView;
                    break;
                case "Presets":
                    SelectedView = _presetsView;
                    break;
            }
        }
    }
}

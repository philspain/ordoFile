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
        TrayApp _trayApp;

        OrganisationSyncer _organisationSyncer;

        bool _windowVisible;

        public MainViewModel(TrayApp trayApp, OrganisationSyncer organisationSyncer)
        {
            _trayApp = trayApp;
            _organisationSyncer = organisationSyncer;
            OnInitialise();
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
            WindowVisible = _organisationSyncer.WindowVisible;
            _organisationSyncer.UpdateWindowVisibility += CheckVisibilityStatus;
        }

        void CheckVisibilityStatus(object sender, EventArgs e)
        {
            WindowVisible = _organisationSyncer.WindowVisible;
        }
    }
}

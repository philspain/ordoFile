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

        Visibility _windowVisibility;

        public MainViewModel(TrayApp trayApp, OrganisationSyncer organisationSyncer)
        {
            _trayApp = trayApp;
            _organisationSyncer = organisationSyncer;
            OnInitialise();
        }

        public Visibility WindowVisibility
        {
            get { return _windowVisibility; }
            set
            {
                _windowVisibility = value;
                OnPropertyChanged("WindowVisibility");
            }
        }

        void OnInitialise()
        {
            WindowVisibility = _organisationSyncer.WindowVisibilty;
            _organisationSyncer.UpdateWindowVisibility += CheckVisibilityStatus;
        }

        void CheckVisibilityStatus(object sender, EventArgs e)
        {
            WindowVisibility = _organisationSyncer.WindowVisibilty;
        }
    }
}

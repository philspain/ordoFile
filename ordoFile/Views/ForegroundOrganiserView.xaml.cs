using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ordoFile.ViewModels;
using ordoFile.DataAccess;
using ordoFile.Models.Organisers;

namespace ordoFile.Views
{
    /// <summary>
    /// Interaction logic for ForegroundOrganiserView.xaml
    /// </summary>
    public partial class ForegroundOrganiserView : UserControl
    {
        public ForegroundOrganiserView()
        {
            InitializeComponent();

            ForegroundViewModel fgViewModel = new ForegroundViewModel(
                (TrayApp)DependencyFactory.Container.Resolve(typeof(TrayApp), "trayApp"),
                (PresetFilters)DependencyFactory.Container.Resolve(typeof(PresetFilters), "presets"),
                (OrganisationSyncer)DependencyFactory.Container.Resolve(typeof(OrganisationSyncer), "organisationSyncer"),
                (ForegroundOrganiser)DependencyFactory.Container.Resolve(typeof(ForegroundOrganiser), "fgOrganiser"),
                (Logger)DependencyFactory.Container.Resolve(typeof(Logger), "logger"));

            this.DataContext = fgViewModel;
        }
    }
}

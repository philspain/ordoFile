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
    /// Interaction logic for BackgroundView.xaml
    /// </summary>
    public partial class BackgroundView : UserControl
    {
        public BackgroundView()
        {
            InitializeComponent();

            BackgroundViewModel bgViewModel = new BackgroundViewModel(
                (TrayApp)DependencyFactory.Container.Resolve(typeof(TrayApp), "trayApp"),
                (PresetFilters)DependencyFactory.Container.Resolve(typeof(PresetFilters), "presets"),
                (Configs)DependencyFactory.Container.Resolve(typeof(Configs), "configs"),
                (OrganisationSyncer)DependencyFactory.Container.Resolve(typeof(OrganisationSyncer), "organisationSyncer"),
                (BackgroundOrganiser)DependencyFactory.Container.Resolve(typeof(BackgroundOrganiser), "bgOrganiser"));

            this.DataContext = bgViewModel;
        }
    }
}

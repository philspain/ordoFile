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

namespace ordoFile.Views
{
    /// <summary>
    /// Interaction logic for PresetsView.xaml
    /// </summary>
    public partial class PresetsView : UserControl
    {
        public PresetsView()
        {
            InitializeComponent();

            PresetsViewModel presetsViewModel = new PresetsViewModel();
			
			// Get presets
            presetsViewModel.Presets = (PresetFilters)DependencyFactory.Container.Resolve(typeof(PresetFilters), "presets");

            this.DataContext = presetsViewModel;
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Unity;
using ordoFile.ViewModels;
using ordoFile.Views;

namespace ordoFile
{
	public partial class MainView : UserControl
	{
		public MainView()
		{
			// Required to initialize variables
			InitializeComponent();

            MainViewModel mainViewModel = new MainViewModel();
            mainViewModel.BackgroundView =
                DependencyFactory.Container.Resolve<BackgroundView>();
            mainViewModel.ForegroundView =
                DependencyFactory.Container.Resolve<ForegroundView>();
            mainViewModel.PresetsView =
                DependencyFactory.Container.Resolve<PresetsView>();
            mainViewModel.SelectedView =
                ((ForegroundView)DependencyFactory.Container.Resolve(typeof(ForegroundView), "foregroundView"));

            this.DataContext = mainViewModel;
		}
	}
}
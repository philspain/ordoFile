using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ordoFile.ViewModels;
using ordoFile.Views;
using Microsoft.Practices.Unity;
using ordoFile.DataAccess;

namespace ordoFile
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		Configs _configs;
        TrayApp _trayApp;

        public MainWindow()
		{
			this.InitializeComponent();

            _configs = (Configs)DependencyFactory.Container.Resolve(typeof(Configs), "configs");
            _trayApp = (TrayApp)DependencyFactory.Container.Resolve(typeof(TrayApp), "trayApp");

            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel(_trayApp,
                (OrganisationSyncer)DependencyFactory.Container.Resolve(typeof(OrganisationSyncer), "organisationSyncer"));

            this.DataContext = mainWindowViewModel;
		}

        public void ClickToMove(object sender, RoutedEventArgs reArgs)
        {
            Window.DragMove();
        }

        public void ClickToExit(object sender, RoutedEventArgs reArgs)
        {
            this.Close();
        }

        public void ClickToMinimise(object sender, RoutedEventArgs reArgs)
        {
            Window.WindowState = WindowState.Minimized;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_trayApp.WindowShouldMinimise)
            {
                e.Cancel = true;
                _trayApp.ChangeGUIVisibility();
            }
            else
            {
                _configs.SaveConfigs();
            }

            base.OnClosing(e);
        }
	}
}
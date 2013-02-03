﻿using System;
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

namespace ordoFile
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();

			// Insert code required on object creation below this point.

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

        public void ClickToMove(object sender, RoutedEventArgs reArgs)
        {
            Window.DragMove();
        }

        public void ClickToExit(object sender, RoutedEventArgs reArgs)
        {
            Window.Close();
        }

        public void ClickToMinimise(object sender, RoutedEventArgs reArgs)
        {
            Window.WindowState = WindowState.Minimized;
        }

        public void ClickToMaximise(object sender, RoutedEventArgs reArgs)
        {
            if(Window.WindowState != WindowState.Maximized)
                Window.WindowState = WindowState.Maximized;
            else
                Window.WindowState = WindowState.Normal;
        }
	}
}
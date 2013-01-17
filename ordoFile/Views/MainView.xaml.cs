using System;
using System.IO;
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
using System.Threading;
using System.Security;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using ordoFile.Models;
using ordoFile.DataAccess;
using ordoFile.GUITools;
using ordoFile.Models.Organisers;
using ordoFile.ViewModels;


namespace ordoFile.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        GifBitmapDecoder loadingGif = new GifBitmapDecoder(
            new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase) + "\\img\\blocks.gif"),
            BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

        Configs _configs;
        TrayApp _trayApp;

        int loadingFrameCount;

        public MainView(Configs configs, TrayApp trayApp, MainViewModel viewModel)
        {
            _configs = configs;
            _trayApp = trayApp;
            this.DataContext = viewModel;

            InitializeComponent();            
        }

        //void ShowLoadingAnimation()
        //{
        //    //ThreadPool.QueueUserWorkItem(
        //    //    delegate 
        //    //    {
        //    //        int index = 0;
                    
                    
        //    //        GUIDispatcherUpdates.UpdateZIndex(WaitGrid, 1);
        //    //        GUIDispatcherUpdates.UpdateZIndex(WaitImageGrid, 2);

        //    //        while (_showLoadingAnimation)
        //    //        {
        //    //            GUIDispatcherUpdates.SetImageSource(LoadingImage, loadingGif, index);

        //    //            if (index == loadingFrameCount)
        //    //            {
        //    //                index = 0;
        //    //            }

        //    //            index++;

        //    //            Thread.Sleep(200);
        //    //        }

        //    //        GUIDispatcherUpdates.UpdateZIndex(WaitGrid, -1);
        //    //        GUIDispatcherUpdates.UpdateZIndex(WaitImageGrid, -2);
        //    //    }
        //    //);
        //}

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

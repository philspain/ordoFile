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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ordoFile.ViewModels;
using ordoFile.DataAccess;
using ordoFile.Models.Organisers;
using System.IO;

namespace ordoFile.Views
{
	/// <summary>
	/// Interaction logic for ForegroundView.xaml
	/// </summary>
	public partial class ForegroundView : UserControl
	{
        int _imageIndex = 1;
        int _numOfImages = 24;
        List<BitmapImage> _bitmaps = new List<BitmapImage>();

		public ForegroundView()
		{
			this.InitializeComponent();

            ForegroundViewModel fgViewModel = new ForegroundViewModel(
                (TrayApp)DependencyFactory.Container.Resolve(typeof(TrayApp), "trayApp"),
                (PresetFilters)DependencyFactory.Container.Resolve(typeof(PresetFilters), "presets"),
                (OrganisationSyncer)DependencyFactory.Container.Resolve(typeof(OrganisationSyncer), "organisationSyncer"),
                (ForegroundOrganiser)DependencyFactory.Container.Resolve(typeof(ForegroundOrganiser), "fgOrganiser"),
                (Logger)DependencyFactory.Container.Resolve(typeof(Logger), "logger"));

            this.DataContext = fgViewModel;

            LoadBitmaps();
		}

        private void Begin_Animation(object sender, EventArgs e)
        {
            ShowImage();
            this.DA.BeginAnimation(Image.WidthProperty, this.DA);
        }

        private void ShowImage()
        {
            this.LoadingImage.Source = _bitmaps[_imageIndex];
            _imageIndex++;

            if (_imageIndex == _numOfImages)
                _imageIndex = 0;
        }

        private void LoadBitmaps()
        { 
            for(int i = 0; i < _numOfImages; i++)
            {
                string imageName = "/Resources/images/blocks/frame" + i + ".gif";

                Stream bitmapStream = Application.GetResourceStream(new Uri(imageName, UriKind.Relative)).Stream;

                BitmapImage bmp = new BitmapImage();

                bmp.BeginInit();
                bmp.StreamSource = bitmapStream;
                bmp.EndInit();

                _bitmaps.Add(bmp);
            }
        }
	}
}
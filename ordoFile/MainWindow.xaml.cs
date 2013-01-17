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

namespace ordoFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)this.DataContext).WindowVisible = Visibility.Hidden;
            ((MainViewModel)this.DataContext).WindowOpacity = "1.0";
        }
    }
}

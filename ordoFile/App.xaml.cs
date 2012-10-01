using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace ordoFile
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e != null && e.Args.Count() > 0)
            {
                if(e.Args[0] == "-bg")
                {
                    this.Properties["bgStartup"] = true;
                }
                else
                {
                    this.Properties["bgStartup"] = false;
                }
            }
        }
    }
}

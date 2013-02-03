using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Unity;
using ordoFile.GUITools;
using ordoFile.Models.Organisers;
using ordoFile.DataAccess;
using ordoFile.Models;
using ordoFile.Views;
using ordoFile.ViewModels;

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

            this.Properties["bgStartup"] = false;

            if (e != null && e.Args.Count() > 0)
            {
                if (e.Args[0] == "-bg")
                {
                    this.Properties["bgStartup"] = true;
                }
            }

            Configs configs = new Configs();
            PresetFilters presets = new PresetFilters();
            OrganisationSyncer organisationSyncer = new OrganisationSyncer();
            TrayApp trayApp = new TrayApp(organisationSyncer, configs);
            Logger logger = new Logger();
            BackgroundOrganiser backgroundOrganiser = new BackgroundOrganiser(new DirectoryModel());
            ForegroundOrganiser foregroundOrganiser = new ForegroundOrganiser(new DirectoryModel());
            BackgroundView backgroundView = new BackgroundView();
            ForegroundView foregroundView = new ForegroundView();

            DependencyFactory.Container.RegisterInstance<Configs>("configs", configs);
            DependencyFactory.Container.RegisterInstance<PresetFilters>("presets", presets);
            DependencyFactory.Container.RegisterInstance<OrganisationSyncer>("organisationSyncer", organisationSyncer);
            DependencyFactory.Container.RegisterInstance<BackgroundOrganiser>("backgroundOrganiser", backgroundOrganiser);
            DependencyFactory.Container.RegisterInstance<ForegroundOrganiser>("foregroundOrganiser", foregroundOrganiser);
            DependencyFactory.Container.RegisterType<BackgroundView>();
            DependencyFactory.Container.RegisterType<ForegroundView>();
            DependencyFactory.Container.RegisterType<PresetsView>();
            DependencyFactory.Container.RegisterInstance<TrayApp>("trayApp", trayApp);
            DependencyFactory.Container.RegisterInstance<Logger>("logger", logger);

        }
	}
}
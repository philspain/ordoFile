using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using ordoFile.DataAccess;
using ordoFile.Models.Organisers;

namespace ordoFile
{
    public partial class TrayApp : Form
    {
        // Icon for tray app
        NotifyIcon _trayIcon = new NotifyIcon();

        //Menu for tray app
        ContextMenu _trayMenu = new ContextMenu();

        // Menu items to be placed in context menu
        MenuItem _showGUI, _bgState, _exit;

        // Reading and saving of config settings
        Configs _configs;

        // Ensures background/foreground organiser do not organise
        // the same folder; also syncs view states between GUI and
        // tray app
        OrganisationSyncer _organisationSyncer;

        // Represent states for whether or not the MainWindow should
        // be hidden, and whether or not it close button should minimise
        // window or exit application
        bool _hideWindow,
             _windowShouldMinimise;

        public TrayApp(OrganisationSyncer organisationSyncer, Configs configs)
        {
            _organisationSyncer = organisationSyncer;
            _configs = configs;

            OnInitialise();

            InitializeComponent();
        }

        /// <summary>
        /// Returns wether or the window should be minimised
        /// when MainWindow's close button is clicked
        /// </summary>
        public bool WindowShouldMinimise
        {
            get { return _windowShouldMinimise; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Hide form window.
            this.Visible = false;
            this.ShowInTaskbar = false;
        }

        void OnInitialise()
        {
            // Pass the event used for changing the _bgState display text
            // to organisation syncer, which will be raised on input
            // in the GUI
            _organisationSyncer.ChangeOrganisationTextInTray += ChangeOrganisationText;

            // Set the application to minimise when close button
            // of main window is clicked
            _windowShouldMinimise = true;

            // Set whether or not the GUI should be hidden on startup
            _hideWindow = ((bool) System.Windows.Application.Current.Properties["bgStartup"]);

            if (_hideWindow)
            {
                _organisationSyncer.WindowVisibilty = System.Windows.Visibility.Hidden;
                _organisationSyncer.UpdateVisibility();
            }

            // Instantiate tray menu, and menu items to be added to it
            _showGUI = new MenuItem();
            _showGUI.Text = (_hideWindow) ? "Show Window" : "Hide Window";
            _showGUI.Click += (a, e) => { ChangeGUIVisibility(); };

            _bgState = new MenuItem();
            _bgState.Text = (_organisationSyncer.BackgroundOrganiserRunning) ? "Stop Organising" : "Start Organising";
            _bgState.Enabled = _configs.BGDirectoryExists;
            _bgState.Click += (a, e) => { CheckOrganisationState(); };

            _exit = new MenuItem();
            _exit.Text = "Exit";
            _exit.Click += (a, e) => { OnExit(); };

            _trayMenu.MenuItems.Add(0, _showGUI);
            _trayMenu.MenuItems.Add(1, _bgState);
            _trayMenu.MenuItems.Add(2, _exit);

            _trayIcon.Icon = new Icon("icon.ico", 38, 42);
            _trayIcon.Text = "ordoFile";
            _trayIcon.DoubleClick += (a, b) => { ChangeGUIVisibility(); };
            _trayIcon.ContextMenu = _trayMenu;
            _trayIcon.Visible = true;
        }

        /// <summary>
        /// Method which sets visibility of GUI and tray menu text
        /// </summary>
        public void ChangeGUIVisibility()
        {
            if (_organisationSyncer.WindowVisibilty == System.Windows.Visibility.Visible)
            {
                _showGUI.Text = "Show window";
                _organisationSyncer.WindowVisibilty = System.Windows.Visibility.Hidden;
                _organisationSyncer.UpdateVisibility();
            }
            else
            {
                _showGUI.Text = "Hide window";
                _organisationSyncer.WindowVisibilty = System.Windows.Visibility.Visible;
                _organisationSyncer.UpdateVisibility();
            }
        }

        /// <summary>
        /// Check organisation current organisation state, start or 
        /// stop organisation depending on whether or not background
        /// directory exists then update menu item with appropriate text
        /// </summary>
        void CheckOrganisationState()
        {
            if (!_organisationSyncer.BackgroundOrganiserRunning)
            {
                if (_configs.BGDirectoryExists)
                {
                    _bgState.Text = "Stop Organising";
                    _organisationSyncer.ShouldOrganiseInBackground(true);
                }
            }
            else
            {
                _bgState.Text = "Start Organising";
                _organisationSyncer.ShouldOrganiseInBackground(false);
            }
        }

        /// <summary>
        /// Event to update organisation state text in context menu
        /// depending on current organisation state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChangeOrganisationText(object sender, EventArgs e)
        {
            if (_organisationSyncer.BackgroundOrganiserRunning)
            {
                _bgState.Text = "Stop Organising";
            }
            else
            {
                _bgState.Text = "Start Organising";
            }
        }

        /// <summary>
        /// Checks to run on exit of application.
        /// </summary>
        void OnExit()
        {
            _organisationSyncer.ShouldOrganiseInBackground(false);
            _configs.SaveConfigs();
            _windowShouldMinimise = false;
            System.Windows.Application.Current.Shutdown();
            this.Close();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                _trayIcon.Dispose();
            }

            if (isDisposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(isDisposing);
        }
    }
}

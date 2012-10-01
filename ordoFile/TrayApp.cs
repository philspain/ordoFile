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
using ordoFile.Organisers;

namespace ordoFile
{
    partial class TrayApp : Form
    {
        NotifyIcon _trayIcon = new NotifyIcon();
        ContextMenu _trayMenu = new ContextMenu();
        MenuItem _showGUI, _bgState, _exit;
        BackgroundOrganiser _bgThread;
        System.Windows.Controls.Button _bgChooseFolderBtn, _bgRunStateBtn;
        System.Windows.Controls.CheckBox _bgSubDirCheckBox;
        MainWindow _mainWindow;
        System.Windows.Visibility _windowVisible = System.Windows.Visibility.Visible;
        System.Windows.Visibility _windowHidden = System.Windows.Visibility.Hidden;
        Configs _configs = Configs.GetConfigs();
        bool _hideWindow;
        Thread _updateBGStateThread;

        public TrayApp(MainWindow mainWindow, BackgroundOrganiser bgThread, System.Windows.Controls.Button bgChooseFolderBtn,
            System.Windows.Controls.Button bgRunStateBtn, System.Windows.Controls.CheckBox bgSubDirCheckBox, bool hideWindow)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            _bgThread = bgThread;
            _bgChooseFolderBtn = bgChooseFolderBtn;
            _bgRunStateBtn = bgRunStateBtn;
            _hideWindow = hideWindow;
            _bgSubDirCheckBox = bgSubDirCheckBox;

            OnInitialise();
        }

        public bool CanOrganise
        {
            set { _bgState.Enabled = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.Visible = false;
            this.ShowInTaskbar = false;
        }

        void OnInitialise()
        {
            _mainWindow.ShouldMinimise = true;

            if (_hideWindow)
            {
                _mainWindow.Visibility = _windowHidden;                
            }
            else
            {
                _mainWindow.Visibility = _windowVisible;
            }

            _showGUI = new MenuItem();
            _showGUI.Text = (_mainWindow.Visibility == _windowVisible) ? "Hide window" : "Show window";
            _showGUI.Click += (a, b) => { ShowGUI(); };

            _bgState = new MenuItem();
            _bgState.Text = (_bgThread.IsWorking == true) ? "Stop organising" : "Start organising";
            _bgState.Enabled = _configs.BGDirectoryExists;
            _bgState.Click += new EventHandler(BGProcessState);

            _exit = new MenuItem();
            _exit.Text = "Exit";
            _exit.Click += new EventHandler(OnExit);

            _trayMenu.MenuItems.Add(0, _showGUI);
            _trayMenu.MenuItems.Add(1, _bgState);
            _trayMenu.MenuItems.Add(2, _exit);

            _trayIcon.Icon = new Icon("icon.ico", 38, 42);
            _trayIcon.Text = "ordoFile";
            _trayIcon.DoubleClick += (a, b) => { ShowGUI(); };
            _trayIcon.ContextMenu = _trayMenu;
            _trayIcon.Visible = true;
        }

        public void ShowGUI()
        {
            if (_mainWindow.Visibility == _windowVisible)
            {
                _showGUI.Text = "Show window";
                _mainWindow.Visibility = _windowHidden;
            }
            else
            {
                _showGUI.Text = "Hide window";
                _mainWindow.Visibility = _windowVisible;
            }
        }

        void UpdateBGStateCallback()
        {
            while (_bgThread.IsWorking)
            {
                _bgRunStateBtn.Dispatcher.Invoke((Action) (() => {_bgRunStateBtn.Content = _bgState.Text = "Stopping...";}));
            }

            _bgSubDirCheckBox.Dispatcher.Invoke((Action)(() => { _bgSubDirCheckBox.IsEnabled = _configs.BGDirectoryExists; }));
            _bgChooseFolderBtn.Dispatcher.Invoke((Action)(() => { _bgChooseFolderBtn.IsEnabled = true; }));
            _bgState.Enabled = _configs.BGDirectoryExists;
            _bgRunStateBtn.Dispatcher.Invoke((Action)(() => { _bgRunStateBtn.IsEnabled = _configs.BGDirectoryExists; }));
            _bgState.Text = "Start Organising";
            _bgRunStateBtn.Dispatcher.Invoke((Action) (() => {_bgRunStateBtn.Content = "Start";}));
        }

        public void UpdateBGState()
        {
            if (_bgThread.IsWorking || _bgThread.IsSleeping)
            {
                System.Diagnostics.Debug.WriteLine("here");
                if (_updateBGStateThread == null ||
                    _updateBGStateThread.ThreadState != ThreadState.Running)
                {
                    _updateBGStateThread = new Thread(UpdateBGStateCallback);
                }

                if (!(_updateBGStateThread.ThreadState == ThreadState.Running))
                {
                    _bgThread.Stop();
                    _bgState.Enabled = false;
                    _bgRunStateBtn.Dispatcher.Invoke((Action)(() => { _bgRunStateBtn.IsEnabled = false; }));
                    _bgChooseFolderBtn.Dispatcher.Invoke((Action)(() => { _bgChooseFolderBtn.IsEnabled = false; }));
                    _bgSubDirCheckBox.Dispatcher.Invoke((Action)(() => { _bgSubDirCheckBox.IsEnabled = false; }));
                    _updateBGStateThread.Start();
                }
            }
            else
            {
                _bgState.Text = "Stop Organising";
                _bgRunStateBtn.Dispatcher.Invoke((Action)(() => { _bgRunStateBtn.Content = "Stop"; }));
                _bgChooseFolderBtn.Dispatcher.Invoke((Action)(() => { _bgChooseFolderBtn.IsEnabled = false; }));
                _bgThread.Start();
            }
        }

        void BGProcessState(object sender, EventArgs e)
        {
            UpdateBGState();
        }

        void StopBGProcess(object sender, EventArgs e)
        {
            _bgThread.Stop();
        }

        void OnExit(object sender, EventArgs e)
        {
            _mainWindow.ShouldMinimise = false;
            _mainWindow.Close();
            this.Close();
            Application.Exit();
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

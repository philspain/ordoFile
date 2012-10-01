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
using ordoFile.Organisers;


namespace ordoFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GifBitmapDecoder loadingGif = new GifBitmapDecoder(
            new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase) + "\\img\\blocks.gif"),
            BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

        int loadingFrameCount;

        // Declare instance of Configs which holds application's
        // configuration data.
        Configs _config = Configs.GetConfigs();

        // Declare instance of Presets which holds application's
        // preset filter data.
        PresetFilters _presets = PresetFilters.GetPresets();

        // Thread for updating background button text
        Thread _updateOrganiseButtonsThread;

        // Declare instance or DirectoryModel to be passed into organiser.
        //DirectoryModel _directory;

        // Declare instance or DirectoryModel to be passed into background organiser.
        //DirectoryModel _bgDirectory;

        // Declare instance of organiser to be used for manual organistion of files.
        Organiser _organiser;

        // Declare instance of organiser to be used for background organisation thread
        Organiser _bgOrganiser;

        // Current and previous directories to be used to check whether or not  to
        // traverse subdirectory structure.
        string _previousDirectory;
        string _currentDirectory;

        // Declare instance of BackgroundThread to run continual organisation of files until
        // user decides otherwise.
        BackgroundOrganiser _bgThread;

        // Declare instance of ForegroundOrganiserThread to run organisation of files on user input.
        ForegroundOrganiser _fgThread;

        // Declare variable to store applications source directory.
        string _appLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;

        // Boolean value to decide whether or not GUI is minimised when
        // FormClosing event is fired.
        bool _minimise = true;

        // Boolean value which represents whether or not the program is
        // being started up on loading of OS.
        bool _isBGStartup = false;

        // Boolean value to tell LoadingAnimation callback when to quit
        // animation of image.
        bool _showLoadingAnimation = false;

        //Declare variable to hold instance of form to be used in
        //system tray.
        TrayApp _trayApp;

        // Collection that holds filetypes to displayed so user can edit
        // contents of a preset filter.
        ObservableCollection<string> _typesAvailableForPresetEditing = new ObservableCollection<string>();

        List<string> _typesSelectedInPresetEditing = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            OnInitialise();
        }

        public bool ShouldMinimise
        {
            set { _minimise = value; }
        }

        void OnInitialise()
        {
            GUIDispatcherUpdates.GUIDispatcher = Dispatcher;

            // Set directory and organisation variables.
            //_directory = new DirectoryModel(FilterSubDirsCheckBox);
            //_bgDirectory = new DirectoryModel(BGFilterSubDirsCheckBox);

            _organiser = new Organiser(FilterSubDirsCheckBox);
            _organiser.ProgressBar = OrganisationProgress;

            _bgOrganiser = new Organiser(BGFilterSubDirsCheckBox);
            _bgOrganiser.ProgressBar = BGOrganisationProgress;

            if (_config.BGDirectory != String.Empty && _config.BGDirectoryExists)
            {
                BGDirectoryLabel.Content = "Chosen Directory: " + _config.BGDirectory;
                BGRunStateBtn.IsEnabled = true;
                BGFilterSubDirsCheckBox.IsEnabled = true;
            }

            _bgThread = new BackgroundOrganiser(_bgOrganiser, _bgOrganiser.RootDirectory, _config);

            if (Application.Current.Properties.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine(Application.Current.Properties["bgStartup"]);
                _isBGStartup = (bool) Application.Current.Properties["bgStartup"];
            }

            StartupCheckBox.IsChecked = _isBGStartup;

            BGStartup(true, _isBGStartup);

            if (_isBGStartup && _config.BGDirectoryExists)
            {
                _bgThread.Start();
                _trayApp.UpdateBGState();
            }

            _trayApp = new TrayApp(this, _bgThread, BGChooseFolderBtn, BGRunStateBtn, BGFilterSubDirsCheckBox, _isBGStartup);

            // Set number of frames present in loading image
            loadingFrameCount = loadingGif.Frames.Count - 1;

            EditPresetTypesPanel.ItemsSource = _typesAvailableForPresetEditing;
        }

        void ShowLoadingAnimation()
        {
            ThreadPool.QueueUserWorkItem(
                delegate 
                {
                    int index = 0;
                    
                    
                    GUIDispatcherUpdates.UpdateZIndex(WaitGrid, 1);
                    GUIDispatcherUpdates.UpdateZIndex(WaitImageGrid, 2);

                    while (_showLoadingAnimation)
                    {
                        GUIDispatcherUpdates.SetImageSource(LoadingImage, loadingGif, index);

                        if (index == loadingFrameCount)
                        {
                            index = 0;
                        }

                        index++;

                        Thread.Sleep(200);
                    }

                    GUIDispatcherUpdates.UpdateZIndex(WaitGrid, -1);
                    GUIDispatcherUpdates.UpdateZIndex(WaitImageGrid, -2);
                }
            );
        }

        /// <summary>
        /// Set all buttons to be Enabled or Disabled depending whether or not
        /// user has selected directory to be organised.
        /// </summary>
        /// <param name="enabled"></param>
        void setButtonStates(bool enabled)
        {
            SubdirCheckbox.IsEnabled = enabled;
            FilterTypesBtn.IsEnabled = enabled;
            ChoosePresetBtn.IsEnabled = enabled;
            OrganiseBtn.IsEnabled = enabled;
        }

        /* Event callbacks. */

        /// <summary>
        /// Callback to be used to updates main and background organisers
        /// so that the same directory can't be organised by both.
        /// </summary>
        void UpdateOrganiseButtonsCallback()
        {
            while (_currentDirectory == _config.BGDirectory &&
                    _bgThread.IsWorking)
            {
                continue;
            }

            // Set root directory to be organised by organisation class.
            _organiser.RootDirectory.Path = _currentDirectory;

            // Initialise list of directories with root directory, this is
            // to ensure that root directory will be organised regardless
            // of whether user chooses to organise sub-directories.
            // _moveFiles.DirectoryPaths = new List<string>(new string[] { folderBrowserDialog.SelectedPath });

            DirTextbox.Text = _currentDirectory;
            FilterLabel.Content = "No preset currently chosen. Press 'Organise Now' to organise by all available file types.";
            setButtonStates(true);
        }

        /// <summary>
        /// Callback to be called when ChooseFolder button has been clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reArgs"></param>
        public void OnChooseFolderClick(object sender, RoutedEventArgs reArgs)
        {
            System.Windows.Forms.FolderBrowserDialog browser = new System.Windows.Forms.FolderBrowserDialog();

            // Check folder has been selected
            if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _previousDirectory = _currentDirectory;
                _currentDirectory = browser.SelectedPath;

                // If currently selected directory is the same as the previously
                // selected one then do nothing as the list of subdirectories (should)
                // be the same.
                if (_currentDirectory != _organiser.RootDirectory.Path)
                {
                    // Make sure the currently selected directory is not the same as the
                    // directory currently selected in the background thread.
                    if (_currentDirectory == _bgOrganiser.RootDirectory.Path &&
                        _bgThread.IsWorking)
                    {
                        FilterLabel.Content = "Directory is being organised by background organiser, please choose another.";

                        setButtonStates(false);
                    }
                    else
                    {
                        SubdirCheckbox.IsChecked = false;
                        FilterSubdirBtn.IsEnabled = false;
                        _organiser.SelectedFileTypes.Clear();
                        _organiser.AvailableFileTypes.Clear();

                        //Set root directory to be organised by organisation class.
                        _organiser.RootDirectory.Path = _currentDirectory;
                        _organiser.RootDirectory.Subdirectories.Clear();

                        DirTextbox.Text = _currentDirectory;

                        FilterLabel.Content = "No preset currently chosen. Press 'Organise Now' to organise by all available file types.";

                        string[] files = Directory.GetFiles(_organiser.RootDirectory.Path);
                        _organiser.ClearLists();
                        Organiser.PopulateFileTypes(files, Dispatcher, _organiser.AvailableFileTypes);
                        _organiser.SelectedFileTypes.AddRange(_organiser.AvailableFileTypes);

                        setButtonStates(true);
                    }
                }
            }
        }

        /// <summary>
        /// Callback to be run when user checks the CheckBox that represents whether
        /// or not the application should organise subdirectories within the currently
        /// selected root directory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reArgs"></param>
        public void Subdir_Check(object sender, RoutedEventArgs reArgs)
        {
            CheckBox checkBox = sender as CheckBox;

            _organiser.OrganiseSubDirectories = (bool) checkBox.IsChecked;

            if ((bool)checkBox.IsChecked)
            {
                if (_previousDirectory != _currentDirectory || _organiser.AvailableFileTypes.Count == 0 || _organiser.OrganisationCount > 0)
                {
                    _showLoadingAnimation = true;
                    ShowLoadingAnimation();

                    ThreadPool.QueueUserWorkItem(
                        delegate
                        {
                            _organiser.RootDirectory.Subdirectories.Clear();
                            Organiser.TraverseDirectories(_organiser.RootDirectory, Dispatcher, _organiser.AvailableFileTypes);
                            _organiser.SelectedFileTypes.AddRange(_organiser.AvailableFileTypes);
                            _showLoadingAnimation = false;
                        });
                }

                FilterSubdirBtn.IsEnabled = true;
            }
            else
            {
                FilterSubdirBtn.IsEnabled = false;
            }
        }

        public void FilterSubdir_Click(object sender, RoutedEventArgs reArgs)
        {
            GUIDispatcherUpdates.UpdateZIndex(WaitGrid, 1);
            GUIDispatcherUpdates.UpdateZIndex(SidebarGrid, 3);
            GUIDispatcherUpdates.UpdateZIndex(FilterDirsGrid, 1);

            ObservableCollection<DirectoryModel> treeviewSource = new ObservableCollection<DirectoryModel>();
            treeviewSource.Add(_organiser.RootDirectory);
            FolderTree.ItemsSource = treeviewSource;
        }

        private void ExitSubdirGridBtn_Click(object sender, RoutedEventArgs e)
        {
            GUIDispatcherUpdates.UpdateZIndex(WaitGrid, -1);
            GUIDispatcherUpdates.UpdateZIndex(SidebarGrid, -3);
            GUIDispatcherUpdates.UpdateZIndex(FilterDirsGrid, -1);
        }

        private void FilterTypesBtn_Click(object sender, RoutedEventArgs e)
        {
            TypesControl.ItemsSource = _organiser.AvailableFileTypes;

            GUIDispatcherUpdates.UpdateZIndex(WaitGrid, 1);
            GUIDispatcherUpdates.UpdateZIndex(SidebarGrid, 3);
            GUIDispatcherUpdates.UpdateZIndex(FileTypeGrid, 2);
        }

        private void ExitTypeGridBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_organiser.SelectedFileTypes.Count > 0 && _organiser.SelectedFileTypes.Count != _organiser.AvailableFileTypes.Count)
            {
                FilterLabel.Content = "Custom preset currently chosen. Press 'Organise Now' to organise with selected filters.";
            }
            else if (_organiser.SelectedFileTypes.Count > 0 && _organiser.SelectedFileTypes.Count == _organiser.AvailableFileTypes.Count)
            {
                FilterLabel.Content = "No preset currently chosen. Press 'Organise Now' to organise by all available file types.";
            }
            
            GUIDispatcherUpdates.UpdateZIndex(WaitGrid, -1);
            GUIDispatcherUpdates.UpdateZIndex(SidebarGrid, -3);
            GUIDispatcherUpdates.UpdateZIndex(FileTypeGrid, -2);
        }

        private void TypeCheckBox_Check(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            string type = checkBox.Content as string;

            if ((bool)checkBox.IsChecked)
            {
                if (!_organiser.SelectedFileTypes.Contains(type))
                {
                    _organiser.SelectedFileTypes.Add(type);
                }
            }
            else
            {
                _organiser.SelectedFileTypes.Remove(type);
            }

            foreach (string s in _organiser.SelectedFileTypes)
            {
                System.Diagnostics.Debug.WriteLine(s);
            }
        }

        private void SavePresetBtn_Click(object sender, RoutedEventArgs e)
        {
            _presets.SavePreset(PresetTextBox.Text, _organiser.SelectedFileTypes);
            GUIDispatcherUpdates.UpdateZIndex(SavePresetGrid, -1);
            PresetsListBox.ItemsSource = null;
            PresetsListBox.ItemsSource = _presets.Presets.Keys;
        }

        private void SaveAsPresetBtn_Click(object sender, RoutedEventArgs e)
        {
            GUIDispatcherUpdates.UpdateZIndex(SavePresetGrid, 1);
        }

        private void ChoosePresetBtn_Click(object sender, RoutedEventArgs e)
        {
            PresetsListBox.ItemsSource = _presets.Presets.Keys;

            GUIDispatcherUpdates.UpdateZIndex(WaitGrid, 1);
            GUIDispatcherUpdates.UpdateZIndex(SidebarGrid, 3);
            GUIDispatcherUpdates.UpdateZIndex(ChoosePresetGrid, 3);
        }

        private void SelectPresetBtn_Click(object sender, RoutedEventArgs e)
        {
            string selectedPreset = PresetsListBox.SelectedItem as string;
            _organiser.PresetName = selectedPreset;
            _organiser.SelectedFileTypes = _presets.Presets[selectedPreset];
            FilterLabel.Content = String.Format("{0} preset currently chosen. Press 'Organise Now' to organise with selected filters.", selectedPreset);
            GUIDispatcherUpdates.UpdateZIndex(WaitGrid, -1);
            GUIDispatcherUpdates.UpdateZIndex(SidebarGrid, -3);
            GUIDispatcherUpdates.UpdateZIndex(ChoosePresetGrid, -3);
        }

        private void ExitPresetBtn_Click(object sender, RoutedEventArgs e)
        {
            GUIDispatcherUpdates.UpdateZIndex(WaitGrid, -1);
            GUIDispatcherUpdates.UpdateZIndex(SidebarGrid, -3);
            GUIDispatcherUpdates.UpdateZIndex(ChoosePresetGrid, -3);
        }

        private void OrganiseBtn_Click(object sender, RoutedEventArgs e)
        {
            _organiser.RootDirectory = _organiser.RootDirectory;
            _organiser.SelectedFileTypes = _organiser.SelectedFileTypes;
            _organiser.Organise();
        }

        private void BGChooseFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog browser = new System.Windows.Forms.FolderBrowserDialog();

            if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (browser.SelectedPath != _bgOrganiser.RootDirectory.Path)
                {
                    _bgOrganiser.RootDirectory.Path = browser.SelectedPath;

                    if (_bgOrganiser.RootDirectory.Path == _organiser.RootDirectory.Path &&
                        _fgThread.IsWorking)
                    {
                        BGDirectoryLabel.Content = "Directory is being organised already, please choose another.";
                        
                        BGRunStateBtn.IsEnabled = false;
                        BGFilterSubDirsCheckBox.IsEnabled = false;
                    }
                    else
                    {
                        BGDirectoryLabel.Content = "Chosen Directory: " + _bgOrganiser.RootDirectory.Path;
                        _config.BGDirectory = _bgOrganiser.RootDirectory.Path;
                        _trayApp.CanOrganise = true;
                        BGRunStateBtn.IsEnabled = true;
                        BGFilterSubDirsCheckBox.IsEnabled = true;
                    }
                }
            }
        }

        private void BGRunStateBtn_Click(object sender, RoutedEventArgs e)
        {
            _trayApp.UpdateBGState();
        }

        private void BGFilterSubDirsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox bgCheckBox = sender as CheckBox;

            _bgThread.OrganiseSubDirectories = (bool) bgCheckBox.IsChecked;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_minimise)
            {
                e.Cancel = true;
                _trayApp.ShowGUI();
            }
            else
            {
                _config.SaveConfigs();
            }

            base.OnClosing(e);
        }

        
        private void StartupCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox startupCheckBox = sender as CheckBox;
            BGStartup(false, (bool) startupCheckBox.IsChecked);
        }

        [SecurityCritical]
        void BGStartup(bool init, bool add)
        {
            if (init)
            {
                bool registryKeyExists = ((IList<string>)Registry.CurrentUser.GetSubKeyNames()).Contains("ordoFile");

                if (add && !registryKeyExists)
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

                    key.SetValue("ordoFile", "\"" + _appLocation + "\" -bg");

                    key.Close();
                }
            }
            else
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

                if (add)
                {
                    key.SetValue("ordoFile", "\"" + _appLocation + "\" -bg");
                }
                else
                {
                    key.DeleteValue("ordoFile");
                }

                key.Close();
            }
        }

        void EditPresetTypeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if ((bool)checkBox.IsChecked)
            {
                if (!_typesSelectedInPresetEditing.Contains((string)checkBox.Content))
                {
                    _typesSelectedInPresetEditing.Add((string)checkBox.Content);
                }
            }
            else
            {
                _typesSelectedInPresetEditing.Remove((string)checkBox.Content);
            }
        }

        /// <summary>
        /// Save preset with types available after user is finished editing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SavePresetEditsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (EditPresetNameTextBox.Text.Trim() == String.Empty)
            {
                EditPresetLabel.Content = "Name can't be blank.";
            }
            else if (EditPresetNameTextBox.Text == (string)PresetsListBox.SelectedItem)
            {
                _presets.EditPresets(EditPresetNameTextBox.Text, EditPresetNameTextBox.Text,
                    _typesSelectedInPresetEditing.ToList<string>());
            }
            else
            {
                _presets.EditPresets((string)PresetsListBox.SelectedItem, EditPresetNameTextBox.Text,
                    _typesSelectedInPresetEditing.ToList<string>());
            }
        }

        /// <summary>
        /// Activated when user selects new item in list of available presets. Populates
        /// a WrapPanel with types present in preset.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PresetsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PresetsListBox.SelectedIndex >= 0)
            {
                _typesAvailableForPresetEditing.Clear();

                string presetName = PresetsListBox.SelectedItem as string;

                foreach(string type in _presets.Presets[presetName])
                {
                    _typesAvailableForPresetEditing.Add(type);
                }

                _typesSelectedInPresetEditing.AddRange(_typesAvailableForPresetEditing);

                EditPresetNameTextBox.Text = presetName;
                EditPresetNameTextBox.IsEnabled = true;
                SavePresetEditsBtn.IsEnabled = true;
                AddTypeTextBox.IsEnabled = true;
                AddTypeButton.IsEnabled = true;
            }
        }

        private void AddTypeBtn_Click(object sender, RoutedEventArgs e)
        {
            string newType = AddTypeTextBox.Text.Trim();

            if (newType == String.Empty)
            {
                EditPresetLabel.Content = "Type can't be blank.";
            }
            else
            {

                if (!_typesAvailableForPresetEditing.Contains(newType))
                {
                    _typesAvailableForPresetEditing.Add(newType);
                    _typesSelectedInPresetEditing.Add(newType);
                }
            }            
        }
    }
}

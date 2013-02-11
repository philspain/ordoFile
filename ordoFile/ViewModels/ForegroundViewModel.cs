using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Windows.Threading;
using System.Threading;
using ordoFile.GUITools;
using ordoFile.Models;
using ordoFile.Views;
using ordoFile.Models.Organisers;
using ordoFile.Commands;
using ordoFile.DataAccess;
using System.Windows;

namespace ordoFile.ViewModels
{
    public class ForegroundViewModel : ViewModelBase
    {
        string _previousDirectory,
               _currentDirectory,
               _filterLabelText,
               _dirTextboxText,
               _filterTypesZIndex,
               _sidebarZIndex,
               _waitGridZIndex,
               _waitImageVisibility,
               _waitGridText,
               _savePresetZIndex,
               _savePresetText,
               _selectPresetZIndex,
               _selectedPreset;

        bool _subdirCheckboxChecked,
             _filterSubdirBtnEnabled,
             _filterTypesBtnEnabled,
             _organiseBtnEnabled,
             _choosePresetBtnEnabled,
             _subdirCheckboxEnabled;

        ForegroundOrganiser _fgOrganiser;

        PresetFilters _presets;

        TrayApp _trayApp;

        OrganisationSyncer _organisationSyncer;

        Logger _logger;

        ObservableCollection<DirectoryModel> _rootDirectoryCollection;
        ObservableCollection<string> _presetNames, _selectedPresetTypes;

        ICommand _selectDirectoryCommand,
                 _filterDirectoriesCommand,
                 _filterTypesCommand,
                 _filterTypesDoneCommand,
                 _filterDirectoriesDoneCommand,
                 _typeCheckCommand,
                 _showSavePanelCommand,
                 _hideSavePanelCommand,
                 _savePresetCommand,
                 _showPresetPanelCommand,
                 _selectPresetCommand,
                 _clearSelectedtPresetCommand,
                 _exitSelectPresetCommand,
                 _organiseCommand;

        public ForegroundViewModel(TrayApp trayApp, PresetFilters presets, OrganisationSyncer organisationSyncer,
            ForegroundOrganiser fgOrganiser, Logger logger)
        {
            _trayApp = trayApp;
            _presets = presets;
            _organisationSyncer = organisationSyncer;
            _fgOrganiser = fgOrganiser;
            _logger = logger;

            OnInitialise();
        }

        /// <summary>
        /// Property for getting/setting whether or not the FilterSubdirBtn
        /// is enabled or disabled.
        /// </summary>
        public bool FilterSubdirBtnEnabled
        {
            get { return _filterSubdirBtnEnabled; }
            set
            {
                _filterSubdirBtnEnabled = value;
                OnPropertyChanged("FilterSubdirBtnEnabled");
            }
        }

        /// <summary>
        /// Property for getting/setting whether or not the CheckSubdirectoriesCheckBox
        /// is checked.
        /// </summary>
        public bool CheckSubdirectories
        {
            get { return _fgOrganiser.RootDirectory.CheckSubdirectories; }
            set
            {
                _fgOrganiser.RootDirectory.CheckSubdirectories = value;
                OnPropertyChanged("CheckSubdirectories");
            }
        }

        /// <summary>
        /// Property for getting/setting whether or not the FilterTypesBtn
        /// is enabled or disabled.
        /// </summary>
        public bool FilterTypesBtnEnabled
        {
            get { return _filterTypesBtnEnabled; }
            set
            {
                _filterTypesBtnEnabled = value;
                OnPropertyChanged("FilterTypesBtnEnabled");
            }
        }

        /// <summary>
        /// Property for getting/setting whether or not the SubdirCheckbox
        /// is checked.
        /// </summary>
        public bool SubdirCheckboxChecked
        {
            get { return _subdirCheckboxChecked; }
            set
            {
                _subdirCheckboxChecked = value;
                FilterSubdirBtnEnabled = value;
                _fgOrganiser.OrganiseSubDirectories = value;
                UpdateDirectoryStructure();
                OnPropertyChanged("SubdirCheckboxChecked");
            }
        }

        /// <summary>
        /// Property for getting/setting whether or not the WaitImageGrid
        /// is hidden or shown.
        /// </summary>
        public string WaitImageVisibility
        {
            get { return _waitImageVisibility; }
            private set
            {
                _waitImageVisibility = value;
                OnPropertyChanged("WaitImageVisibility");
            }
        }

        /// <summary>
        /// Property for getting/setting the WaitGridText string
        /// </summary>
        public string WaitGridText
        {
            get { return _waitGridText; }
            private set
            {
                _waitGridText = value;
                OnPropertyChanged("WaitGridText");
            }
        }


        /// <summary>
        /// Property for getting/setting whether or not the SubdirCheckbox
        /// is enabled or disabled.
        /// </summary>
        public bool SubdirCheckboxEnabled
        {
            get { return _subdirCheckboxEnabled; }
            set
            {
                _subdirCheckboxEnabled = value;
                OnPropertyChanged("SubdirCheckboxEnabled");
            }
        }

        /// <summary>
        /// Property for getting/setting whether or not the ChoosePresetBtn
        /// is enabled or disabled.
        /// </summary>
        public bool ChoosePresetBtnEnabled
        {
            get { return _choosePresetBtnEnabled; }
            set
            {
                _choosePresetBtnEnabled = value;
                OnPropertyChanged("ChoosePresetBtnEnabled");
            }
        }

        /// <summary>
        /// Property for getting/setting whether or not the OrganiseBtnEnabled
        /// is enabled or disabled.
        /// </summary>
        public bool OrganiseBtnEnabled
        {
            get { return _organiseBtnEnabled; }
            set
            {
                _organiseBtnEnabled = value;
                OnPropertyChanged("OrganiseBtnEnabled");
            }
        }

        /// <summary>
        /// Property for getting/setting the DirTextboxText string
        /// </summary>
        public string DirTextboxText
        {
            get { return _dirTextboxText; }
            set
            {
                _dirTextboxText = value;
                OnPropertyChanged("DirTextboxText");
            }
        }

        /// <summary>
        /// Property for getting/setting the FilterLabelText string
        /// </summary>
        public string FilterLabelText
        {
            get { return _filterLabelText; }
            set
            {
                _filterLabelText = value;
                OnPropertyChanged("FilterLabelText");
            }
        }

        /// <summary>
        /// Property for getting/setting the WaitGrid's ZIndex
        /// </summary>
        public string WaitGridZIndex
        {
            get { return _waitGridZIndex; }
            set
            {
                _waitGridZIndex = value;
                OnPropertyChanged("WaitGridZIndex");
            }
        }

        /// <summary>
        /// Property for getting/setting the Sidebar grid's ZIndex
        /// </summary>
        public string SidebarZIndex
        {
            get { return _sidebarZIndex; }
            set
            {
                _sidebarZIndex = value;
                OnPropertyChanged("SidebarZIndex");
            }
        }

        /// <summary>
        /// Property for getting/setting the FilterTypes grid's ZIndex
        /// </summary>
        public string FilterTypesZIndex
        {
            get { return _filterTypesZIndex; }
            set
            {
                _filterTypesZIndex = value;
                OnPropertyChanged("FilterTypesZIndex");
            }
        }

        /// <summary>
        /// Property for getting/setting the SavePreset grid's ZIndex
        /// </summary>
        public string SavePresetZIndex
        {
            get { return _savePresetZIndex; }
            set
            {
                _savePresetZIndex = value;
                OnPropertyChanged("SavePresetZIndex");
            }
        }

        /// <summary>
        /// Property for getting/setting the SavePresetText string
        /// </summary>
        public string SavePresetText
        {
            get { return _savePresetText; }
            set
            {
                _savePresetText = value;
                OnPropertyChanged("SavePresetText");
            }
        }

        /// <summary>
        /// Property for getting/setting the SelectPreset grid's ZIndex
        /// </summary>
        public string SelectPresetZIndex
        {
            get { return _selectPresetZIndex; }
            set
            {
                _selectPresetZIndex = value;
                OnPropertyChanged("SelectPresetZIndex");
            }
        }

        /// <summary>
        /// Property for getting/setting the SelectedPreset string
        /// </summary>
        public string SelectedPreset
        {
            get { return _selectedPreset; }
            set
            {
                if (value != null)
                {
                    _selectedPreset = value;
                    OnPropertyChanged("SelectedPreset");
                    OnPropertyChanged("SelectedPresetTypes");
                }
            }
        }

        /// <summary>
        /// Property for getting collection of available filetypes
        /// in selected directories
        /// </summary>
        public ObservableCollection<string> Filetypes
        {
            get
            {
                return _fgOrganiser.AvailableFileTypes;
            }
        }

        /// <summary>
        /// Property for getting collection of recursive DirectoryModels
        /// (models directory structure).
        /// </summary>
        public ObservableCollection<DirectoryModel> RootDirectory
        {
            get
            {
                if (_rootDirectoryCollection == null)
                    _rootDirectoryCollection = new ObservableCollection<DirectoryModel>();

                GUIDispatcherUpdates.ClearCollection(_rootDirectoryCollection);
                _rootDirectoryCollection.Add(_fgOrganiser.RootDirectory);

                return _rootDirectoryCollection;
            }
        }

        /// <summary>
        /// Property for getting collection of preset filter names
        /// </summary>
        public ObservableCollection<string> PresetNames
        {
            get
            {
                if (_presetNames == null)
                    _presetNames = new ObservableCollection<string>();

                GUIDispatcherUpdates.ClearCollection(_presetNames);
                GUIDispatcherUpdates.AddItemsToCollection(_presetNames, _presets.PresetNames);

                return _presetNames;
            }
        }

        /// <summary>
        /// Property for getting filetypes associated with currently
        /// selected preset filter.
        /// </summary>
        public ObservableCollection<string> SelectedPresetTypes
        {
            get
            {
                if (_selectedPresetTypes == null)
                    _selectedPresetTypes = new ObservableCollection<string>();

                GUIDispatcherUpdates.ClearCollection(_selectedPresetTypes);
                GUIDispatcherUpdates.AddItemsToCollection(_selectedPresetTypes, _presets.GetPresetTypes(_selectedPreset));

                return _selectedPresetTypes;
            }
        }

        /// <summary>
        /// Property for getting command used to show dialog for
        /// selecting the directory to be organised.
        /// </summary>
        public ICommand SelectDirectoryCommand
        {
            get
            {
                if (_selectDirectoryCommand == null)
                    _selectDirectoryCommand = new DelegateCommand(SelectDirectory);

                return _selectDirectoryCommand;
            }
        }

        /// <summary>
        /// Property for getting command used to show grid used
        /// for filtering out directories that are not to be organised.
        /// </summary>
        public ICommand FilterDirectoriesCommand
        {
            get
            {
                if (_filterDirectoriesCommand == null)
                    _filterDirectoriesCommand = new DelegateCommand(FilterDirectories);

                return _filterDirectoriesCommand;
            }
        }

        /// <summary>
        /// Property for getting command used to hide grid used
        /// for filtering out directories that are not to be organised.
        /// </summary>
        public ICommand FilterDirectoriesDoneCommand
        {
            get
            {
                if (_filterDirectoriesDoneCommand == null)
                    _filterDirectoriesDoneCommand = new DelegateCommand(FilterDirectoriesDone);

                return _filterDirectoriesDoneCommand;
            }
        }

        /// <summary>
        /// Property for getting command used to show grid used
        /// for filtering out types that are not to be organised.
        /// </summary>
        public ICommand FilterTypesCommand
        {
            get
            {
                if (_filterTypesCommand == null)
                    _filterTypesCommand = new DelegateCommand(FilterTypes);

                return _filterTypesCommand;
            }
        }

        /// <summary>
        /// Property for getting command used to hide grid used
        /// for filtering out types that are not to be organised.
        /// </summary>
        public ICommand FilterTypesDoneCommand
        {
            get
            {
                if (_filterTypesDoneCommand == null)
                    _filterTypesDoneCommand = new DelegateCommand(FilterTypesDone);

                return _filterTypesDoneCommand;
            }
        }

        /// <summary>
        /// Property for getting command used to add/remove filetype
        /// from collection of types to organise.
        /// </summary>
        public ICommand TypeCheckCommand
        {
            get
            {
                if (_typeCheckCommand == null)
                    _typeCheckCommand = new DelegateCommand<CheckBox>(AddRemoveType);

                return _typeCheckCommand;
            }
        }

        /// <summary>
        /// Property for getting command used to show grid that
        /// allows user to save checked types as a preset filter.
        /// </summary>
        public ICommand ShowSavePanelCommand
        {
            get
            {
                if (_showSavePanelCommand == null)
                    _showSavePanelCommand = new DelegateCommand(ShowSavePanel);

                return _showSavePanelCommand;
            }
        }

        /// <summary>
        /// Property for getting command used to hide grid that
        /// allows user to save checked types as a preset filter.
        /// </summary>
        public ICommand HideSavePanelCommand
        {
            get
            {
                if (_hideSavePanelCommand == null)
                    _hideSavePanelCommand = new DelegateCommand(HideSavePanel);

                return _hideSavePanelCommand;
            }
        }

        /// <summary>
        /// Property for getting command used to add new preset filter
        /// to file.
        /// </summary>
        public ICommand SavePresetCommand
        {
            get
            {
                if (_savePresetCommand == null)
                    _savePresetCommand = new DelegateCommand(SavePreset);

                return _savePresetCommand;
            }
        }

        /// <summary>
        /// Property for getting command used to show grid that
        /// allows user to select an existing preset filter.
        /// </summary>
        public ICommand ShowPresetPanelCommand
        {
            get
            {
                if (_showPresetPanelCommand == null)
                    _showPresetPanelCommand = new DelegateCommand(ShowPresetPanel);

                return _showPresetPanelCommand;
            }
        }

        /// <summary>
        /// Property for getting command used to add currently selected
        /// preset's associated filetypes to collection of types to be
        /// organised.
        /// </summary>
        public ICommand SelectPresetCommand
        {
            get
            {
                if (_selectPresetCommand == null)
                    _selectPresetCommand = new DelegateCommand(SelectPreset);

                return _selectPresetCommand;
            }
        }

        /// <summary>
        /// Property for getting command used to add currently selected
        /// preset's associated filetypes to collection of types to be
        /// organised.
        /// </summary>
        public ICommand ClearSelectedPresetCommand
        {
            get
            {
                if (_clearSelectedtPresetCommand == null)
                    _clearSelectedtPresetCommand = new DelegateCommand(ClearSelectedPreset);

                return _clearSelectedtPresetCommand;
            }
        }

        /// <summary>
        /// Property for getting command used to hide grid that
        /// allows user to select an existing preset filter.
        /// </summary>
        public ICommand ExitSelectPresetCommand
        {
            get
            {
                if (_exitSelectPresetCommand == null)
                    _exitSelectPresetCommand = new DelegateCommand(ExitSelectPreset);

                return _exitSelectPresetCommand;
            }
        }

        /// <summary>
        /// Property for getting command used to initiate organisation of
        /// currently selected directories.
        /// </summary>
        public ICommand OrganiseCommand
        {
            get
            {
                if (_organiseCommand == null)
                    _organiseCommand = new DelegateCommand(Organise);

                return _organiseCommand;
            }
        }

        /// <summary>
        /// Initialise various elements once constructor is called.
        /// </summary>
        void OnInitialise()
        {
            SetButtonEnabledStates(false);
            
            SubdirCheckboxEnabled = false;

            DirTextboxText = "No directory currently chosen.";
        }

        /// <summary>
        /// Sets 'Enabled' states for multiple buttons.
        /// </summary>
        /// <param name="enabled">Bool value to set 'Enabled' states to.</param>
        public void SetButtonEnabledStates(bool enabled)
        {
            SubdirCheckboxEnabled = enabled;
            FilterTypesBtnEnabled = enabled;
            ChoosePresetBtnEnabled = enabled;
            OrganiseBtnEnabled = enabled;
        }

        /// <summary>
        /// Show dialog used to select directory to be organised.
        /// </summary>
        public void SelectDirectory()
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
                if (_currentDirectory != _fgOrganiser.RootDirectory.Path)
                {
                    SubdirCheckboxEnabled = false;
                    _fgOrganiser.SelectedFileTypes.Clear();
                    _fgOrganiser.AvailableFileTypes.Clear();

                    // Set root directory to be organised by organisation class.
                    _fgOrganiser.RootDirectory.Path = _currentDirectory;

                    // Clear subdirectories collection of root directory to
                    // ensure no invalid or superfluous directories can be organised.
                    _fgOrganiser.RootDirectory.Subdirectories.Clear();

                    DirTextboxText = _currentDirectory;

                    FilterLabelText = "No preset currently chosen. Press 'Organise Now' to organise all available file types.";

                    // Get available files and populate appropriate collections with
                    // filetypes.
                    string[] files = Directory.GetFiles(_fgOrganiser.RootDirectory.Path);
                    _fgOrganiser.ClearLists();
                    OrganiserBase.PopulateFileTypes(files, _fgOrganiser.AvailableFileTypes);
                    _fgOrganiser.SelectedFileTypes.AddRange(_fgOrganiser.AvailableFileTypes);

                    // Directory exists and filetypes have been extracted, so enable
                    // all buttons used for organising
                    SetButtonEnabledStates(true);

                    // If checkbox for organising subdirectories has been checed
                    // update treeview with root directory's subdirectories
                    if (SubdirCheckboxChecked)
                    {
                        UpdateDirectoryStructure();
                        OnPropertyChanged("RootDirectory");
                    }
                }
            }
        }

        /// <summary>
        /// Ascertain(recursively) root directory's subdirectory structure, updating
        /// appropriate collections where needed.
        /// </summary>
        void UpdateDirectoryStructure()
        {
            if (_subdirCheckboxChecked)
            {
                if (_previousDirectory != _currentDirectory ||
                    _fgOrganiser.AvailableFileTypes.Count == 0)
                {
                    // Show the wait grid while updating structure.
                    WaitGridZIndex = "Visible";
                    WaitGridText = "Loading directories";
                    WaitImageVisibility = "Visible";

                    // Add delegate to threadpool
                    ThreadPool.QueueUserWorkItem(
                        delegate
                        {
                            GUIDispatcherUpdates.ClearCollection(_fgOrganiser.RootDirectory.Subdirectories);
                            OrganiserBase.TraverseDirectories(_fgOrganiser.RootDirectory, _fgOrganiser.AvailableFileTypes);
                            _fgOrganiser.SelectedFileTypes.AddRange(_fgOrganiser.AvailableFileTypes);

                            // Hide waitgrid when finished updating.
                            WaitGridZIndex = "Hidden";
                        });
                }
            }
        }

        /// <summary>
        /// Show grid for filtering root directory's subdirectories.
        /// </summary>
        void FilterDirectories()
        {
            SidebarZIndex = "Visible";
            WaitGridZIndex = "Visible";
            WaitImageVisibility = "Visible";
        }

        /// <summary>
        /// Hide grid for filtering root directory's subdirectories.
        /// </summary>
        void FilterDirectoriesDone()
        {
            SidebarZIndex = "Hidden";
            WaitGridZIndex = "Hidden";
        }

        /// <summary>
        /// Show grid for filtering filetypes to be organised.
        /// </summary>
        void FilterTypes()
        {
            FilterTypesZIndex = "Visible";
            SelectPresetZIndex = "Hidden";
        }

        /// <summary>
        /// Hide grid for filtering filetypes.
        /// </summary>
        void FilterTypesDone()
        {
            SidebarZIndex = "Hidden";
            FilterTypesZIndex = "Hidden";
            WaitGridZIndex = "Hidden";
        }

        /// <summary>
        /// Add or remove types to/from collection of filetypes to organise
        /// depending on whether a checkbox is checked or not.
        /// </summary>
        /// <param name="checkBox">Checkbox that represents a filetype.</param>
        void AddRemoveType(CheckBox checkBox)
        {
            string type = checkBox.Content as string;

            if ((bool)checkBox.IsChecked)
            {
                if (!_fgOrganiser.SelectedFileTypes.Contains(type))
                {
                    _fgOrganiser.SelectedFileTypes.Add(type);
                }
            }
            else
            {
                if (_fgOrganiser.SelectedFileTypes.Contains(type))
                {
                    _fgOrganiser.SelectedFileTypes.Remove(type);
                }
            }
        }

        /// <summary>
        /// Show grid to save a preset
        /// </summary>
        void ShowSavePanel()
        {
            SavePresetZIndex = "Visible";
        }

        /// <summary>
        /// Hide grid to save a preset
        /// </summary>
        void HideSavePanel()
        {
            SavePresetZIndex = "Hidden";
        }

        /// <summary>
        /// Save selected filetypes as preset filter.
        /// </summary>
        void SavePreset()
        {
            if (!(String.IsNullOrWhiteSpace(_savePresetText) ||
                  String.IsNullOrEmpty(_savePresetText)))
            {
                if (_presets.PresetExists(_savePresetText))
                {
                    MessageBox.Show("Preset already exists.");
                }
                else
                {
                    try
                    {
                        _presets.AddPreset(_savePresetText, _fgOrganiser.SelectedFileTypes);
                    }
                    catch (ArgumentException AEx)
                    {
                        _logger.LogError(AEx.Message + "\n" + AEx.StackTrace);
                    }

                    SavePresetText = String.Empty;
                    OnPropertyChanged("PresetNames");
                }
            }
        }

        /// <summary>
        /// Show grid for selecting a preset filter.
        /// </summary>
        void ShowPresetPanel()
        {
            SelectPresetZIndex = "Visible";
            FilterTypesZIndex = "Hidden";
        }

        /// <summary>
        /// Select preset and hide grid.
        /// </summary>
        void SelectPreset()
        {
            _fgOrganiser.SelectedFileTypes = _presets.GetPresetTypes(_selectedPreset);
            FilterLabelText = "The " + _selectedPreset + " filter has been chosen";

            ExitSelectPreset();
        }

        /// <summary>
        /// Select preset and hide grid.
        /// </summary>
        void ClearSelectedPreset()
        {
            _fgOrganiser.SelectedFileTypes.AddRange(_fgOrganiser.AvailableFileTypes);
            FilterLabelText = "No preset currently chosen. Press 'Organise Now' to organise all available file types.";
        }

        // Hide grid for selecting preset.
        void ExitSelectPreset()
        {
            SelectPresetZIndex = "Hidden";
        }

        /// <summary>
        /// Check whether or not the foreground organiser is trying to organise
        /// the same directory as the background organiser.
        /// </summary>
        /// <returns>bool</returns>
        bool OrganisationConflictExists()
        {
            return (_organisationSyncer.ForegroundDirectory == _organisationSyncer.BackgroundDirectory &&
                _organisationSyncer.BackgroundOrganiserRunning);
        }

        /// <summary>
        /// Begin organising files.
        /// </summary>
        void Organise()
        {
            if (OrganisationConflictExists())
            {
                OrganiseBtnEnabled = false;
                FilterLabelText = "This directory is currently being organised, please choose another or wait";

                ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (OrganisationConflictExists())
                        {
                            continue;
                        }

                        OrganiseBtnEnabled = true;
                        FilterLabelText = _currentDirectory;
                    }
                );
            }
            else
            {
                // Show wait grid
                WaitGridZIndex = "Visible";

                // 
                WaitGridText = "Organising";

                _organisationSyncer.ForegroundOrganiserRunning = true;
                _fgOrganiser.Organise();

                ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (_fgOrganiser.IsWorking)
                        {
                            continue;
                        }

                        // Hide wait grid.
                        WaitGridZIndex = "Hidden";

                        _organisationSyncer.ForegroundOrganiserRunning = false;
                    }
                );
            }
        }
    }
}
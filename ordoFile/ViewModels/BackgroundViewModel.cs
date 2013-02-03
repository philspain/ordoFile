using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ordoFile.Models.Organisers;
using ordoFile.Commands;
using ordoFile.DataAccess;
using System.Windows.Input;
using System.Windows.Controls;
using System.Security;
using System.Windows;
using System.Threading;
using System.IO;
using System.ComponentModel;
using Microsoft.Win32;
using ordoFile.GUITools;
using System.Collections.ObjectModel;

namespace ordoFile.ViewModels
{
    class BackgroundViewModel : ViewModelBase
    {
        Configs _configs;

        PresetFilters _presets;

        BackgroundOrganiser _bgOrganiser;

        TrayApp _trayApp;

        OrganisationSyncer _organisationSyncer;

        ICommand _chooseFolderCommand,
                 _organisationCommand,
                 _cancelOrganisationCommand,
                 _deselectPresetCommand;

        ObservableCollection<string> _presetNames;

        string _appLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;

        string _bgDirectoryLabelText, 
               _bgOrganiseButtonContent,
               _selectedPreset;

        bool _bgOrganiseButtonEnabled,
            _bgOrganisationEnabled;

        public BackgroundViewModel(TrayApp trayApp, PresetFilters presets, Configs configs, OrganisationSyncer organisationSyncer,
            BackgroundOrganiser bgOrganiser)
        {
            _trayApp = trayApp;
            _configs = configs;
            _presets = presets;
            _organisationSyncer = organisationSyncer;
            _bgOrganiser = bgOrganiser;

            OnInitialise();
        }

        /// <summary>
        /// Property for getting/setting the background directory to be
        /// organised.
        /// </summary>
        public string BGDirectory
        {
            get { return _bgOrganiser.RootDirectory.Path; }
            set
            {
                _bgOrganiser.RootDirectory.Path = value;
                _configs.BGDirectory = value;
                _organisationSyncer.BackgroundDirectory = value;
                BGDirectoryLabelText = value;
            }
        }

        /// <summary>
        /// Property for getting or setting the BGDirectoryLabel string.
        /// </summary>
        public string BGDirectoryLabelText
        {
            get { return _bgDirectoryLabelText; }
            set
            {
                _bgDirectoryLabelText = value;
                OnPropertyChanged("BGDirectoryLabelText");
            }
        }

        /// <summary>
        /// Property for getting/setting 'Enabled' state of the
        /// BGOrganiseButton.
        /// </summary>
        public bool BGOrganiseButtonEnabled
        {
            get { return _bgOrganiseButtonEnabled; }
            set
            {
                _bgOrganiseButtonEnabled = value;
                OnPropertyChanged("BGOrganiseButtonEnabled");
            }
        }

        /// <summary>
        /// Property for getting/setting 'Enabled' state of the
        /// BGFilterSubDirsCheckBox.
        /// </summary>
        public bool BGOrganisationEnabled
        {
            get { return _bgOrganisationEnabled; }
            set
            {
                _bgOrganisationEnabled = value;
                OnPropertyChanged("BGOrganisationEnabled");
            }
        }

        /// <summary>
        /// Property for getting/setting the 'Checked' state of the
        /// StartupCheckbox. Will add/remove program to windows startup
        /// depending on check state.
        /// </summary>
        public bool StartupChecked
        {
            get { return _configs.StartupEnabled; }
            set
            {
                if (TrySetStartup(value))
                {
                    _configs.StartupEnabled = value;
                }
                else
                {
                    _configs.StartupEnabled = false;
                    OnPropertyChanged("StartupChecked");
                }
            }
        }

        /// <summary>
        /// Property for getting/setting the 'Checked' state of the
        /// BGFilterSubDirsCheckbox.
        /// </summary>
        public bool BGFilterSubDirsChecked
        {
            get { return _configs.OrganiseSubDirectories; }
            set
            {
                _bgOrganiser.RootDirectory.CheckSubdirectories = value;
                _bgOrganiser.OrganiseSubDirectories = value;
                _configs.OrganiseSubDirectories = value;
            }
        }

        /// <summary>
        /// Property for getting/setting the BGOrganiseButton content
        /// string.
        /// </summary>
        public string BGOrganiseButtonContent
        {
            get { return _bgOrganiseButtonContent; }
            set
            {
                _bgOrganiseButtonContent = value;
                OnPropertyChanged("BGOrganiseButtonContent");
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
                    _bgOrganiser.SelectedFileTypes = _presets.GetPresetTypes(_selectedPreset);
                }
                else
                {
                    _selectedPreset = value;
                    _bgOrganiser.SelectedFileTypes.AddRange(_bgOrganiser.AvailableFileTypes);
                }
            }
        }

        /// <summary>
        /// Property for getting command for choosing the background
        /// directory to be organised.
        /// </summary>
        public ICommand ChooseFolderCommand
        {
            get
            {
                if (_chooseFolderCommand == null)
                    _chooseFolderCommand = new DelegateCommand(ChooseFolder);

                return _chooseFolderCommand;
            }
        }

        /// <summary>
        /// Command for organising files in selected directory.
        /// </summary>
        public ICommand OrganisationCommand
        {
            get
            {
                if (_organisationCommand == null)
                    _organisationCommand = new DelegateCommand(Organise);

                return _organisationCommand;
            }
        }

        /// <summary>
        /// Command for organising files in selected directory.
        /// </summary>
        public ICommand CancelOrganisationCommand
        {
            get
            {
                if (_cancelOrganisationCommand == null)
                    _cancelOrganisationCommand = new DelegateCommand(StopOrganisation);

                return _cancelOrganisationCommand;
            }
        }

        /// <summary>
        /// Command for organising files in selected directory.
        /// </summary>
        public ICommand DeselectPresetCommand
        {
            get
            {
                if (_deselectPresetCommand == null)
                    _deselectPresetCommand = new DelegateCommand(DeselectPreset);

                return _deselectPresetCommand;
            }
        }

        void OnInitialise()
        {
            _bgOrganiseButtonContent = "Start";

            if (_organisationSyncer != null)
            {
                _organisationSyncer.OrganiseInBackground += OrganiseEvent;
                _organisationSyncer.StopBackgroundOrganisation += StopOrganisationEvent;
            }

            if (_configs.BGDirectoryExists)
            {
                BGDirectory = _configs.BGDirectory;
                BGOrganiseButtonEnabled = true;
                BGOrganisationEnabled = true;

                if (((bool)Application.Current.Properties["bgStartup"]))
                {
                    Organise();
                }
            }
            else
            {
                BGDirectoryLabelText = "No directory has been chosen";
                BGOrganiseButtonEnabled = false;
                BGOrganisationEnabled = false;
            }

        }

        /// <summary>
        /// Check whether or not the background organiser is trying to organise
        /// the same directory as the foreground organiser.
        /// </summary>
        /// <returns>bool</returns>
        bool OrganisationConflictExists()
        {
            return (_organisationSyncer.BackgroundDirectory == _organisationSyncer.ForegroundDirectory &&
                _organisationSyncer.ForegroundOrganiserRunning);
        }

        /// <summary>
        /// Select directory to be organised.
        /// </summary>
        void ChooseFolder()
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();

            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BGDirectory = folderBrowser.SelectedPath;
                BGOrganiseButtonEnabled = true;
                BGOrganisationEnabled = true;
                BGDirectoryLabelText = BGDirectory;
            }
        }

        void StopOrganisationEvent(object sender, EventArgs e)
        {
            StopOrganisation();
        }

        /// <summary>
        /// Stop organisation that is currently in progress.
        /// </summary>
        void StopOrganisation()
        {
            _bgOrganiser.Stop();
            BGOrganiseButtonContent = "Start";
            _organisationSyncer.ChangeOrganisationText();
        }

        void OrganiseEvent(object sender, EventArgs e)
        {
            Organise();
        }

        /// <summary>
        /// Organise files in selected directories.
        /// </summary>
        void Organise( )
        {
            // If foreground organiser is currently organising the
            // directory that has been chosen for background organiser
            // disable organise button until directory is changed or forground
            // organisation has stopped.
            if (OrganisationConflictExists())
            {
                BGOrganiseButtonEnabled = false;
                BGDirectoryLabelText = "This directory is currently being organised, please choose another or wait";

                ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (OrganisationConflictExists())
                        {
                            continue;
                        }

                        BGOrganiseButtonEnabled = true;
                        BGDirectoryLabelText = BGDirectory;
                    }
                );
            }
            else
            {
                _organisationSyncer.BackgroundOrganiserRunning = true;
                BGOrganisationEnabled = false;
                BGOrganiseButtonContent = "Stop";

                // Change text in the tray menu to reflect organisation status
                _organisationSyncer.ChangeOrganisationText();

                _bgOrganiser.Organise();

                ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (_bgOrganiser.IsWorking)
                        {
                            continue;
                        }

                        BGOrganisationEnabled = true;
                        _organisationSyncer.BackgroundOrganiserRunning = false;
                        _organisationSyncer.ChangeOrganisationText();
                    }
                );
            }
        }

        /// <summary>
        /// Remove filetypes of currently selected preset from organiser.
        /// </summary>
        void DeselectPreset()
        {
            SelectedPreset = null;
        }

        /// <summary>
        /// Attempt to add/remove application to windows startup.
        /// </summary>
        /// <param name="isChecked">Value that decides whether to try add
        /// or remove from startup.</param>
        /// <returns>bool</returns>
        [SecurityCritical]
        bool TrySetStartup(bool isChecked)
        {
            try
            {
                bool registryKeyExists = ((IList<string>)Registry.CurrentUser.GetSubKeyNames()).Contains("ordoFile");
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

                if (isChecked && !registryKeyExists)
                {
                    key.SetValue("ordoFile", "\"" + _appLocation + "\" -bg");
                }
                else
                {
                    key.DeleteValue("ordoFile");
                }

                key.Close();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using ordoFile.Models;
using ordoFile.Organisers;
using ordoFile.Commands;
using System.IO;
using System.Windows.Threading;
using ordoFile.DataAccess;
using System.Threading;
using System.Diagnostics;

namespace ordoFile.ViewModels
{
    public class ForegroundViewModel : ViewModelBase
    {
        string _previousDirectory, _currentDirectory, _filterLabelText, _dirTextboxText = String.Empty;
        bool _subdirCheckboxChecked, _filterSubdirBtnEnabled, _organiseBtnEnabled, _choosePresetBtnEnabled, _subdirCheckboxEnabled;
        Dispatcher _dispatcher;
        ForegroundOrganiser _fgOrganiser = new ForegroundOrganiser(new CheckBox());
        BackgroundOrganiser _bgOrganiser = new BackgroundOrganiser(Configs.GetConfigs());
        ICommand _selectDirectoryCommand, _filterTypesCommand, _selectedPresetsCommand, _startOrganisationCommand;

        public ForegroundViewModel()
        {
            OnInitialise();
            SetButtonStates(false);
            SubdirCheckboxEnabled = false;
        }

        void OnInitialise()
        {
            DirTextboxText = "No directory currently chosen.";
        }

        public Dispatcher Dispatcher
        {
            set
            {
                _dispatcher = value;
                OnPropertyChanged("Dispatcher");
            }
        }

        public bool FilterSubdirBtnEnabled
        {
            get { return _filterSubdirBtnEnabled; }
            set
            {
                _filterSubdirBtnEnabled = value;
                OnPropertyChanged("FilterSubdirBtnEnabled");
            }
        }

        public bool? SubdirCheckboxChecked
        {
            get { return _subdirCheckboxChecked; }
            set
            {
                _subdirCheckboxChecked = (bool) value;
                Debug.WriteLine("hello");
                UpdateDirectoryStructure();
                
                OnPropertyChanged("SubdirCheckboxChecked");
            }
        }

        public bool SubdirCheckboxEnabled
        {
            get { return _subdirCheckboxEnabled; }
            set
            {
                _subdirCheckboxEnabled = value;
                OnPropertyChanged("SubdirCheckboxEnabled");
            }
        }

        public bool ChoosePresetBtnEnabled
        {
            get { return _choosePresetBtnEnabled; }
            set
            {
                _choosePresetBtnEnabled = value;
                OnPropertyChanged("ChoosePresetBtnEnabled");
            }
        }

        public bool OrganiseBtnEnabled
        {
            get { return _organiseBtnEnabled; }
            set
            {
                _organiseBtnEnabled = value;
                OnPropertyChanged("OrganiseBtnEnabled");
            }
        }

        public string DirTextboxText
        {
            get { return _dirTextboxText; }
            set
            {
                _dirTextboxText = value;
                OnPropertyChanged("DirTextboxText");
            }
        }

        public string FilterLabelText
        {
            get { return _filterLabelText; }
            set
            {
                _filterLabelText = value;
                OnPropertyChanged("FilterLabelText");
            }
        }

        public BackgroundOrganiser BackGroundOrganiser
        {
            set { _bgOrganiser = value; }
        }

        public ICommand SelectDirectoryCommand
        {
            get
            {
                _selectDirectoryCommand = new DelegateCommand(SelectDirectory);

                return _selectDirectoryCommand;
            }
        }

        public void SetButtonStates(bool enabled)
        {
            SubdirCheckboxEnabled = enabled;
            FilterSubdirBtnEnabled = enabled;
            ChoosePresetBtnEnabled = enabled;
            OrganiseBtnEnabled = enabled;
        }

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
                    // Make sure the currently selected directory is not the same as the
                    // directory currently selected in the background thread.
                    if (_currentDirectory == _bgOrganiser.Directory.Path &&
                        _bgOrganiser.IsWorking)
                    {
                        FilterLabelText= "Directory is being organised by background organiser, please choose another.";

                        SetButtonStates(false);
                    }
                    else
                    {
                        SubdirCheckboxEnabled = false;
                        FilterSubdirBtnEnabled = false;
                        _fgOrganiser.SelectedFileTypes.Clear();
                        _fgOrganiser.AvailableFileTypes.Clear();

                        //Set root directory to be organised by organisation class.
                        _fgOrganiser.RootDirectory.Path = _currentDirectory;
                        _fgOrganiser.RootDirectory.Subdirectories.Clear();

                        DirTextboxText = "Chosen directory: " + _currentDirectory;

                        FilterLabelText = "No preset currently chosen. Press 'Organise Now' to organise by all available file types.";

                        string[] files = Directory.GetFiles(_fgOrganiser.RootDirectory.Path);
                        _fgOrganiser.ClearLists();
                        Organiser.PopulateFileTypes(files, _dispatcher, _fgOrganiser.AvailableFileTypes);
                        _fgOrganiser.SelectedFileTypes.AddRange(_fgOrganiser.AvailableFileTypes);

                        SetButtonStates(true);
                    }
                }
            }
        }

        void UpdateDirectoryStructure()
        {
            if (_previousDirectory != _currentDirectory || _fgOrganiser.AvailableFileTypes.Count == 0 || _fgOrganiser.OrganisationCount > 0)
            {
                Debug.WriteLine("hey");
                ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        _fgOrganiser.RootDirectory.Subdirectories.Clear();
                        Organiser.TraverseDirectories(_fgOrganiser.RootDirectory, _dispatcher, _fgOrganiser.AvailableFileTypes);
                        _fgOrganiser.SelectedFileTypes.AddRange(_fgOrganiser.AvailableFileTypes);
                        Debug.WriteLine("hey");
                        Debug.WriteLine(_fgOrganiser.RootDirectory.Subdirectories.Count);
                    });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}


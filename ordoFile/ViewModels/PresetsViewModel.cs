using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ordoFile.DataAccess;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ordoFile.Commands;
using ordoFile.GUITools;
using System.Collections;
using System.Windows.Controls;

namespace ordoFile.ViewModels
{
    class PresetsViewModel : ViewModelBase
    {
        string _presetEditZIndex,
               _selectedPreset,
               _presetEditTempName,
               _presetEditType,
               _editPresetErrorText,
               _newPresetZIndex,
               _newPresetName,
               _newPresetType,
               _newPresetErrorText;

        PresetFilters _presets;

        List<string> _presetEditSelectedTypes = new List<string>();
        List<string> _newPresetSelectedTypes = new List<string>();

        ObservableCollection<string> _presetNames,
                                     _presetEditFileTypes,
                                     _newPresetFileTypes;

        ICommand _showPresetEditCommand,
                 _hidePresetEditCommand,
                 _savePresetEditsCommand,
                 _presetEditAddTypeCommand,
                 _presetEditRemoveTypesCommand,
                 _showNewPresetCommand,
                 _hideNewPresetCommand,
                 _saveNewPresetCommand,
                 _newPresetAddTypeCommand,
                 _newPresetRemoveTypesCommand;

        /// <summary>
        /// Proprty for setting instance of object used to manage
        /// preset filters.
        /// </summary>
        public PresetFilters Presets
        {
            set { _presets = value; }
        }

        /// <summary>
        /// Property to be bound to grid in UI and informs it of
        /// it's ZIndex value.
        /// </summary>
        public string PresetEditZIndex
        {
            get { return _presetEditZIndex; }
            set
            {
                _presetEditZIndex = value;
                OnPropertyChanged("PresetEditZIndex");
            }
        }

        /// <summary>
        /// Name of currently selected preset.
        /// </summary>
        public string SelectedPreset
        {
            get
            {
                if (_selectedPreset == null)
                    _selectedPreset = _presets.PresetNames[0];

                return _selectedPreset;
            }
            set
            {
                _selectedPreset = value;
                PresetEditTempName = value;
                OnPropertyChanged("SelectedPreset");
                UpdateFileTypes();
            }
        }

        /// <summary>
        /// Property for temporary preset name used for editing
        /// existing preset names.
        /// </summary>
        public string PresetEditTempName
        {
            get
            {
                if(_presetEditTempName == null)
                    _presetEditTempName = _presets.PresetNames[0];

                return _presetEditTempName;
            }
            set
            {
                _presetEditTempName = value;
                OnPropertyChanged("PresetEditTempName");
            }
        }

        /// <summary>
        /// Property for temporary preset name used for editing
        /// existing preset names.
        /// </summary>
        public string PresetEditType
        {
            get
            {
                if(_presetEditType == null)
                    _presetEditType = String.Empty;

                return _presetEditType;
            }
            set
            {
                _presetEditType = value;
                OnPropertyChanged("PresetEditType");
            }
        }

        /// <summary>
        /// Collection of preset names.
        /// </summary>
        public ObservableCollection<string> PresetNames
        {
            get
            {
                _presetNames = new ObservableCollection<string>();

                GUIDispatcherUpdates.AddItemsToCollection(_presetNames, _presets.PresetNames);

                return _presetNames;
            }
        }

        /// <summary>
        /// Collection of filetypes for selected preset.
        /// </summary>
        public ObservableCollection<string> PresetEditFileTypes
        {
            get
            {
                if (_presetEditFileTypes == null)
                {
                    _presetEditFileTypes = new ObservableCollection<string>();
                    GUIDispatcherUpdates.AddItemsToCollection(_presetEditFileTypes, _presets.GetPresetTypes(_selectedPreset));
                }

                return _presetEditFileTypes;
            }
        }

        /// <summary>
        /// Property to be bound to grid in UI and informs it of
        /// it's ZIndex value.
        /// </summary>
        public string NewPresetZIndex
        {
            get { return _newPresetZIndex; }
            set
            {
                _newPresetZIndex = value;
                OnPropertyChanged("NewPresetZIndex");
            }
        }

        /// <summary>
        /// Property to be bound to grid in UI and informs it of
        /// it's ZIndex value.
        /// </summary>
        public string NewPresetName
        {
            get { return _newPresetName; }
            set
            {
                _newPresetName = value;
                OnPropertyChanged("NewPresetName");
            }
        }

        /// <summary>
        /// Name of new preset to be created.
        /// </summary>
        public string NewPresetType
        {
            get { return _newPresetType; }
            set
            {
                _newPresetType = value;
                OnPropertyChanged("NewPresetType");
            }
        }

        /// <summary>
        /// Collection of filetypes for selected preset to be creates.
        /// </summary>
        public ObservableCollection<string> NewPresetFileTypes
        {
            get
            {
                if (_newPresetFileTypes == null)
                {
                    _newPresetFileTypes = new ObservableCollection<string>();
                }

                return _newPresetFileTypes;
            }
        }

        /// <summary>
        /// Name of new preset to be created.
        /// </summary>
        public string EditPresetErrorText
        {
            get { return _editPresetErrorText; }
            set
            {
                _editPresetErrorText = value;
                OnPropertyChanged("EditPresetErrorText");
            }
        }

        /// <summary>
        /// Error message to be displayed if anything erroneous
        /// happens while creating a new preset.
        /// </summary>
        public string NewPresetErrorText
        {
            get { return _newPresetErrorText; }
            set
            {
                _newPresetErrorText = value;
                OnPropertyChanged("NewPresetErrorText");
            }
        }

        /// <summary>
        /// Command which calls method to set EditPrest grid
        /// to be visible.
        /// </summary>
        public ICommand ShowPresetEditCommand
        {
            get
            {
                if (_showPresetEditCommand == null)
                    _showPresetEditCommand = new DelegateCommand(ShowPresetEdit);

                return _showPresetEditCommand;
            }
        }

        /// <summary>
        /// Command which calls method to set EditPrest grid
        /// to be visible.
        /// </summary>
        public ICommand HidePresetEditCommand
        {
            get
            {
                if (_hidePresetEditCommand == null)
                    _hidePresetEditCommand = new DelegateCommand(HidePresetEdit);

                return _hidePresetEditCommand;
            }
        }

        /// <summary>
        /// Command which calls method to save changes to a preset.
        /// </summary>
        public ICommand SavePresetEditsCommand
        {
            get
            {
                if (_savePresetEditsCommand == null)
                    _savePresetEditsCommand = new DelegateCommand(SavePresetEdits);

                return _savePresetEditsCommand;
            }
        }

        /// <summary>
        /// Command which calls method to add new preset
        /// to currently selected preset's filetypes collection.
        /// </summary>
        public ICommand PresetEditAddTypeCommand
        {
            get
            {
                if (_presetEditAddTypeCommand == null)
                    _presetEditAddTypeCommand = new DelegateCommand(PresetEditAddType);

                return _presetEditAddTypeCommand;
            }
        }

        /// <summary>
        /// Command which calls method to remove types from
        /// currently selected preset's filetypes collection.
        /// </summary>
        public ICommand PresetEditRemoveTypesCommand
        {
            get
            {
                if (_presetEditRemoveTypesCommand == null)
                    _presetEditRemoveTypesCommand = new DelegateCommand<object>(PresetEditRemoveTypes);

                return _presetEditRemoveTypesCommand;
            }
        }

        /// <summary>
        /// Command which calls method to show the grid
        /// used for entering a new preset filter.
        /// </summary>
        public ICommand ShowNewPresetCommand
        {
            get
            {
                if (_showNewPresetCommand == null)
                    _showNewPresetCommand = new DelegateCommand(ShowNewPreset);

                return _showNewPresetCommand;
            }
        }

        /// <summary>
        /// Command which calls method to hide the grid
        /// used for entering a new preset filter.
        /// </summary>
        public ICommand HideNewPresetCommand
        {
            get
            {
                if (_hideNewPresetCommand == null)
                    _hideNewPresetCommand = new DelegateCommand(HideNewPreset);

                return _hideNewPresetCommand;
            }
        }

        /// <summary>
        /// Command which calls method to save a new preset.
        /// </summary>
        public ICommand SaveNewPresetCommand
        {
            get
            {
                if (_saveNewPresetCommand == null)
                    _saveNewPresetCommand = new DelegateCommand(SaveNewPreset);

                return _saveNewPresetCommand;
            }
        }

        /// <summary>
        /// Command which calls method to add type to
        /// new preset's filetypes collection.
        /// </summary>
        public ICommand NewPresetAddTypeCommand
        {
            get
            {
                if (_newPresetAddTypeCommand == null)
                    _newPresetAddTypeCommand = new DelegateCommand(NewPresetAddType);

                return _newPresetAddTypeCommand;
            }
        }

        /// <summary>
        /// Command which calls method to remove types from
        /// new preset's filetypes collection.
        /// </summary>
        public ICommand NewPresetRemoveTypesCommand
        {
            get
            {
                if (_newPresetRemoveTypesCommand == null)
                    _newPresetRemoveTypesCommand = new DelegateCommand<object>(NewPresetRemoveTypes);

                return _newPresetRemoveTypesCommand;
            }
        }

        /// <summary>
        /// Method to set EditPrest grid to be visible.
        /// </summary>
        void ShowPresetEdit()
        {
            PresetEditZIndex = "Visible";
        }

        /// <summary>
        /// Method to set EditPrest grid to be hidden.
        /// </summary>
        void HidePresetEdit()
        {
            PresetEditZIndex = "Hidden";
        }

        /// <summary>
        /// Update GUI to show filetypes for newly selected
        /// preset.
        /// </summary>
        private void UpdateFileTypes()
        {
            GUIDispatcherUpdates.ClearCollection(_presetEditFileTypes);
            GUIDispatcherUpdates.AddItemsToCollection(_presetEditFileTypes, _presets.GetPresetTypes(_selectedPreset));
        }

        /// <summary>
        /// Method to set EditPrest grid to be hidden.
        /// </summary>
        void SavePresetEdits()
        {
            if (!String.IsNullOrEmpty(_presetEditTempName) &&
                !String.IsNullOrWhiteSpace(_presetEditTempName))
            {
                if (_presetEditFileTypes.Count > 0)
                {
                    if (_selectedPreset == _presetEditTempName ||
                        (!(_selectedPreset == _presetEditTempName) && !_presets.PresetExists(_presetEditTempName)))
                    {
                        // Save changes into presets object.
                        _presets.EditPresets(_selectedPreset,
                                             _presetEditTempName,
                                             GUIDispatcherUpdates.CollectionAsList(_presetEditFileTypes));

                        // Update collection for ListBox that shows preset names
                        GUIDispatcherUpdates.UpdateCollectionItem(_presetNames, _selectedPreset, PresetEditTempName);

                        OnPropertyChanged("PresetNames");

                        SelectedPreset = _presetEditTempName;
                    }
                    else
                    {
                        EditPresetErrorText = "Preset filter name currently exists.";
                    }
                }
                else
                {
                    EditPresetErrorText = "Filetypes must exist for preset to be saved.";
                }
            }
            else
            {
                EditPresetErrorText = "Filetype can't be empty or whitespace.";
            }
        }

        /// <summary>
        /// Add type to list of filetypes for currently selected
        /// preset types.
        /// </summary>
        void PresetEditAddType()
        {
            if ((!String.IsNullOrEmpty(_presetEditType) || !String.IsNullOrWhiteSpace(_presetEditType)))
            {
                if (!GUIDispatcherUpdates.CollectionContainsItem(_presetEditFileTypes, _presetEditType))
                {
                    GUIDispatcherUpdates.AddItemToCollection(_presetEditFileTypes, _presetEditType);
                    PresetEditType = String.Empty;
                }
                else
                {
                    EditPresetErrorText = "Filetype already exists.";
                }
            }
            else
            {
                EditPresetErrorText = "Filetype can't be empty or whitespace.";
            }
        }

        /// <summary>
        /// Remove selected filetypes from collection of available types.
        /// </summary>
        void PresetEditRemoveTypes(object selectedItems)
        {
            IList selectedItemList = (IList) selectedItems;
            var selectedTypes = selectedItemList.Cast<string>();
            
            GUIDispatcherUpdates.RemoveItemsFromCollection(_presetEditFileTypes, selectedTypes.ToList());
        }

        /// <summary>
        /// Method to set EditPrest grid to be visible.
        /// </summary>
        void ShowNewPreset()
        {
            NewPresetZIndex = "Visible";
        }

        /// <summary>
        /// Method to set EditPrest grid to be hidden.
        /// </summary>
        void HideNewPreset()
        {
            NewPresetZIndex = "Hidden";
        }

        /// <summary>
        /// Method for saving a new preset.
        /// </summary>
        void SaveNewPreset()
        {
            if (!String.IsNullOrEmpty(_newPresetName) &&
                !String.IsNullOrWhiteSpace(_newPresetName))
            {
                if (_newPresetFileTypes.Count > 0)
                {
                    if (!_presets.PresetExists(_newPresetName))
                    {
                        _presets.AddPreset(_newPresetName, GUIDispatcherUpdates.CollectionAsList(_newPresetFileTypes));

                        NewPresetName = String.Empty;

                        _newPresetFileTypes = null;
                        OnPropertyChanged("NewPresetFileTypes");

                        OnPropertyChanged("PresetNames");
                    }
                    else
                    {
                        NewPresetErrorText = "Preset filter name currently exists.";
                    }
                }
                else
                {
                    NewPresetErrorText = "Filetypes must exist.";
                }
            }
            else
            {
                NewPresetErrorText = "Preset filter name can't be empty or whitespace.";
            }
        }

        /// <summary>
        /// Remove selected filetypes from collection of available types.
        /// </summary>
        void NewPresetRemoveTypes(object selectedItems)
        {
            IList selectedItemList = (IList)selectedItems;
            var selectedTypes = selectedItemList.Cast<string>();

            GUIDispatcherUpdates.RemoveItemsFromCollection(_newPresetFileTypes, selectedTypes.ToList());
        }

        /// <summary>
        /// Add type to list of filetypes for currently selected
        /// preset types.
        /// </summary>
        void NewPresetAddType()
        {
            if ((!String.IsNullOrEmpty(_newPresetType) || !String.IsNullOrWhiteSpace(_newPresetType)) &&
               !GUIDispatcherUpdates.CollectionContainsItem(_newPresetFileTypes, _newPresetType))
            {
                GUIDispatcherUpdates.AddItemToCollection(_newPresetFileTypes, _newPresetType);
                NewPresetType = String.Empty;
            }
            else
            {
                NewPresetErrorText = "Filetype can't be empty or whitespace.";
            }
        }
    }
}

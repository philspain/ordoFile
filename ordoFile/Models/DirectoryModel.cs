using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace ordoFile.Models
{
    public class DirectoryModel : INotifyPropertyChanged
    {
        bool _isChecked;
        CheckBox _subdirCheckbox;

        public DirectoryModel(CheckBox subdirCheckbox)
        {
            _isChecked = true;
            _subdirCheckbox = subdirCheckbox;
            Subdirectories = new ObservableCollection<DirectoryModel>();
        }

        public string Name { get; set; }
        public string Path { get; set; }

        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                this._isChecked = value;
                NotifyPropertyChanged("IsChecked");

                if ((bool)_subdirCheckbox.IsChecked)
                {
                    RecursivelySetFolderCheckStates(this.Subdirectories, value);
                }
            }
        }

        public ObservableCollection<DirectoryModel> Subdirectories { get; set; }

        public DirectoryModel CreateAndAddSubdirectory(string path)
        {
            DirectoryModel subdirectory = new DirectoryModel(this._subdirCheckbox);
            subdirectory.Path = path;
            Subdirectories.Add(subdirectory);

            return subdirectory;
        }

        void RecursivelySetFolderCheckStates(ObservableCollection<DirectoryModel> directories, bool checkState)
        {
            if (directories.Count > 0)
            {
                foreach (DirectoryModel subfolder in Subdirectories)
                {
                    subfolder.IsChecked = checkState;
                }
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

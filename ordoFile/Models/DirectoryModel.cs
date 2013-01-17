using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using ordoFile.GUITools;

namespace ordoFile.Models
{
    public class DirectoryModel : INotifyPropertyChanged
    {
        bool _isChecked, _checkSubdirectories;
        string _name;
        ObservableCollection<DirectoryModel> _subdirectories;

        public DirectoryModel()
        {
            _isChecked = true;
            Subdirectories = new ObservableCollection<DirectoryModel>();
        }

        public string Name 
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public string Path { get; set; }

        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                this._isChecked = value;
                NotifyPropertyChanged("IsChecked");

                if (CheckSubdirectories)
                {
                    RecursivelySetCheckStates(value);
                }
            }
        }

        public bool CheckSubdirectories
        {
            get { return _checkSubdirectories; }
            set
            {
                this._checkSubdirectories = value;
                RecursivelySetCheckDirectories();
            }
        }

        public ObservableCollection<DirectoryModel> Subdirectories 
        {
            get { return _subdirectories; }
            set
            {
                _subdirectories = value;
            }
        }

        public DirectoryModel CreateAndAddSubdirectory(string path)
        {
            DirectoryModel subdirectory = new DirectoryModel();
            subdirectory.Path = path;
            GUIDispatcherUpdates.AddItemToCollection(Subdirectories, subdirectory);

            return subdirectory;
        }

        void RecursivelySetCheckStates(bool checkState)
        {
            if (Subdirectories.Count > 0)
            {
                foreach (DirectoryModel subfolder in Subdirectories)
                {
                    subfolder.IsChecked = checkState;
                }
            }
        }

        void RecursivelySetCheckDirectories()
        {
            if (Subdirectories.Count > 0)
            {
                foreach (DirectoryModel subdirectory in Subdirectories)
                {
                    subdirectory.CheckSubdirectories = CheckSubdirectories;
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

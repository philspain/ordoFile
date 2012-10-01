using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Collections.ObjectModel;
using ordoFile.Models;
using System.Windows.Threading;
using ordoFile.GUITools;

namespace ordoFile
{
    public class Organiser
    {

        // Number of organisations that have occurred
        int _organisationCount;

        // Used for background organising, tells organiser user has
        // chosen to stop process.
        bool _shouldWork = true;

        // Name of preset if user has chosen one.
        string _presetName;

        // Whether or not root directory's subdirectories should be organised.
        bool _organiseSubDirectories;

        /// Path of folder that has been chosen for organisation.
        DirectoryModel _rootDirectory;

        /// List of filenames; used to ensure all files to be moved will
        /// have unique names and not overwrite each other.
        List<string> _fileNames = new List<string>();

        /// List of filetypes contained in all selected directories.
        /// Used in form to allow user to filter which filetypes will be
        /// organised.
        ObservableCollection<string> _availableFileTypes = new ObservableCollection<string>();

        /// List of all selected filetypes the user has chosen to organise.
        List<string> _selectedFileTypes = new List<string>();

        /// List of files to be moved during organisation.
        List<FileDetailsModel> _filesToMove = new List<FileDetailsModel>();

        /// Instance of GUI's progressbar, value will be changed to
        /// reflect completion status of organisation.
        ProgressBar _progressBar;

        public Organiser(CheckBox subdirsCheckBox)
        {
            _rootDirectory = new DirectoryModel(subdirsCheckBox);
            _filesToMove = new List<FileDetailsModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public DirectoryModel RootDirectory
        {
            set { _rootDirectory = value; }
            get { return _rootDirectory; }
        }

        /// <summary>
        /// Gets or sets the available filetypes collection.
        /// </summary>
        public ObservableCollection<string> AvailableFileTypes
        {
            get
            {
                return _availableFileTypes;
            }
            set
            {
                _availableFileTypes = value;
            }
        }

        /// <summary>
        /// Gets of sets the selected filetypes collection.
        /// </summary>
        public List<string> SelectedFileTypes
        {
            get
            {
                return _selectedFileTypes;
            }
            set
            {
                _selectedFileTypes = value;
            }
        }

        public ProgressBar ProgressBar
        {
            get { return _progressBar; }
            set { _progressBar = value; }
        }

        public bool ShouldWork
        {
            set { _shouldWork = value; }
        }

        public bool OrganiseSubDirectories
        {
            set { _organiseSubDirectories = value; }
        }

        public string PresetName
        {
            set { _presetName = value; }
        }

        public int OrganisationCount
        {
            get { return _organisationCount; }
        }

        /// <summary>
        /// Traverse root directory's subdirectories to ascertain directory tree structure,
        /// also gather available filetype data.
        /// </summary>
        /// <param name="directory"> Directory to be traversed.</param>
        /// <param name="availableFileTypes"> List to be populated with all filetypes in directories</param>
        /// <param name="directory"> Directory to be traversed.</param>
        public static void TraverseDirectories(DirectoryModel directory, Dispatcher dispatcher, ObservableCollection<string> availableFileTypes = null)
        {
            directory.Name = System.IO.Path.GetFileName(directory.Path);

            try
            {
                // List of subdirectories and filetypes in the current directory.
                string[] dirs = Directory.GetDirectories(directory.Path, "*", SearchOption.TopDirectoryOnly);

                if (availableFileTypes != null)
                {
                    string[] files = Directory.GetFiles(directory.Path);

                    Organiser.PopulateFileTypes(files, dispatcher, availableFileTypes);
                }

                if (dirs.Count() > 0)
                {
                    foreach (string dir in dirs)
                    {
                        // Traverse each subdirectory contained in current directory then
                        // add to directories subdirectory list

                        DirectoryModel childDirectory = directory.CreateAndAddSubdirectory(dir);

                        Organiser.TraverseDirectories(childDirectory, dispatcher, availableFileTypes);
                    }
                }
            }
            catch (UnauthorizedAccessException uaEx)
            {
                System.Diagnostics.Debug.WriteLine(directory.Path + " is inaccessible");
                System.Diagnostics.Debug.WriteLine(uaEx.Message);
                System.Diagnostics.Debug.WriteLine("==================");
            }
            catch (IOException ioEx)
            {
                System.Diagnostics.Debug.WriteLine(directory.Path + " io exception");
                System.Diagnostics.Debug.WriteLine(ioEx.Message);
                System.Diagnostics.Debug.WriteLine("==================");
            }

        }

        public static void PopulateFileTypes(string[] files, Dispatcher dispatcher, ObservableCollection<string> availableFileTypes)
        {
            if (files.Count() > 0)
            {
                foreach (string file in files)
                {
                    // If current file has a filetype, add to list of available filetypes
                    // otherwise add the abstract "FILE" filetype to represent any files
                    // without filetypes.

                    string fileType = System.IO.Path.GetExtension(file).Length > 0 ? System.IO.Path.GetExtension(file).Substring(1) : "FILE";

                    if (!availableFileTypes.Contains(fileType))
                    {
                        GUIDispatcherUpdates.AddTypeToObservableCollection(availableFileTypes, fileType);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new, unique name for a file found to have a name similar
        /// to another file that is being organised. This will allow n files with
        /// similar names to be moved to the same desintation directory.
        /// </summary>
        /// <param name="destDir">Directory that file will be moved to.</param>
        /// <param name="fileName">Current name of file.</param>
        /// <param name="fileType">File's file-type</param>
        /// <returns>Path that file will be moved to, which uses newly created
        /// filename.</returns>
        string rename(string destDir, string fileName, string fileType)
        {
            int modifier = 1;
            bool notNew = true;
            //Console.WriteLine(fileName);

            while (notNew)
            {
                int second = fileName.LastIndexOf(fileType) - 2;

                if (fileName[second] == ')')
                {
                    int first = fileName.LastIndexOf('(', second);

                    int subStringLength = second - first - 1;
                    string checkIfNum = fileName.Substring(first + 1, subStringLength);

                    int result;

                    if (int.TryParse(checkIfNum, out result))
                    {
                        result += modifier;
                        fileName = fileName.Substring(0, first + 1) + result + fileName.Substring(second);

                        if (!(_fileNames.Contains(fileName) || new FileInfo(destDir + fileName).Exists))
                            notNew = false;
                    }
                }
                else
                {
                    fileName = fileName.Substring(0, second + 1) + "(" + modifier + ")" + fileName.Substring(second + 1);

                    if (!(_fileNames.Contains(fileName) || new FileInfo(destDir + fileName).Exists))
                        notNew = false;
                }
            }
            return fileName;
        }

        /// <summary>
        /// Iterates through all files, in all chosen directories, and calls
        /// method to create FilesInfo object for each, which is then added to
        /// //_filesToMove list.
        /// </summary>
        /// <param name="totalFileSize">Value used to calculate total size
        ///  of all files, this value is then used to calculate progress of
        ///  organistion and state of progress bar passed from GUI.</param>
        void getFileObjects(DirectoryModel directory, ref long totalFileSize)
        {
            if (directory.IsChecked)
            {
                string[] fileArray = Directory.GetFiles(directory.Path);

                foreach (string file in fileArray)
                {
                    string fileType = Path.GetExtension(file);

                    /// Check that current file has filetype, if it does not
                    /// file it under "FILE" for the purposes of organisation.
                    if (fileType.Length > 0)
                    {
                        fileType = fileType.Substring(1);
                    }
                    else
                    {
                        fileType = "FILE";
                    }


                    foreach (string s in _selectedFileTypes)
                    {
                        Console.WriteLine(fileType + " - " + s);
                    }

                    /// Check that user has selected any filte-types to filter to
                    /// and that the current file is of one of these types.
                    /// If there are no filters chosen, or the previous two states
                    /// evaluate as true, create a FilesInfo object for the file.
                    if (_selectedFileTypes.Count > 0)
                    {
                        if (_selectedFileTypes.Contains(fileType))
                        {
                            createFileObject(file, fileType, ref totalFileSize);
                        }
                    }
                    else
                    {
                        createFileObject(file, fileType, ref totalFileSize);
                    }
                }

                if (_organiseSubDirectories && directory.Subdirectories.Count > 0)
                {
                    foreach (DirectoryModel subdirectory in directory.Subdirectories)
                    {
                        getFileObjects(subdirectory, ref totalFileSize);
                    }
                }
            }
        }

        /// <summary>
        /// Create instance of FilesInfo object for a file to be moved and
        /// and add it to the _filesToMove list.
        /// </summary>
        /// <param name="filePath">File's location path.</param>
        /// <param name="fileType">File's filetype.</param>
        /// <param name="totalFileSize">Value used to calculate total size
        ///  of all files, this value is then used to calculate progress of
        ///  organistion and state of progress bar passed from GUI.</param>
        void createFileObject(string filePath, string fileType, ref long totalFileSize)
        {
            string fileName = Path.GetFileName(filePath);

            /// Create destination directory. Directory path is contigent upon the
            /// file's file-type and whether or not a preset has been chosen.
            string destDir = (_presetName == String.Empty) ?
                _rootDirectory.Path + "\\ordoFiled\\" + fileType + "\\" :
                _rootDirectory.Path + "\\ordoFiled\\" + _presetName + "\\" + fileType + "\\";

            /// If file's name currently exists in _fileNames call rename method
            /// to create a new, unique name for the file.
            if (_fileNames.Contains(fileName) || (new FileInfo(destDir + fileName).Exists))
            {
                fileName = rename(destDir, fileName, fileType);
            }

            _fileNames.Add(fileName);

            /// Get file's size and add it to totalFileSize variable which is used
            /// to calculate size of all files
            long fileSize = new FileInfo(filePath).Length;
            totalFileSize += fileSize;

            FileDetailsModel fileInfo = new FileDetailsModel(filePath, destDir + fileName, fileName, fileType, destDir, fileSize);
            _filesToMove.Add(fileInfo);
        }

        /// <summary>
        /// Method that iterates through _filesToMove and moves each one
        /// from it's current location, to it's destination path.
        /// </summary>
        /// <param name="shouldWork">Boolean value passed by reference
        /// to tell method whether or not it should continue to organise files.
        /// This parameter is only changed when the user tries to stop the
        /// background process.</param>
        public void Organise()
        {
            /// Contains total size of all files to be organised.
            long totalFileSize = 0;
            getFileObjects(_rootDirectory, ref totalFileSize);

            /// The size of all files that have currenty been moved.
            long dataMovedSize = 0;

            if (Directory.Exists(_rootDirectory.Path))
            {
                foreach (FileDetailsModel curFile in _filesToMove)
                {
                    if (!curFile.FilePath.Contains("ordoFiled"))
                    {
                        if (!_shouldWork)
                        {
                            break;
                        }

                        if (!Directory.Exists(curFile.DestDir))
                        {
                            Directory.CreateDirectory(curFile.DestDir);
                        }

                        File.Move(curFile.FilePath, curFile.NewFilePath);

                        // Won't be useful until I've implemented ability for user to choose
                        // their own destination folder.
                        //
                        ///// Force application to wait until file has been completely moved
                        ///// to it's destination path.
                        //while (new FileInfo(curFile.NewFilePath).Length < curFile.FileSize)
                        //{
                        //    continue;
                        //}

                        dataMovedSize += curFile.FileSize;
                        _progressBar.Dispatcher.Invoke((Action)(() => { 
                            _progressBar.Value = totalFileSize == 0 ? 0 : (int)(dataMovedSize / totalFileSize) * 100; 
                        }));
                    }
                }
            }
            else
            {
                MessageBox.Show("Provided directories do not exist, organisation aborted.", "Access Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            _organisationCount++;
            _fileNames.Clear();
            _filesToMove.Clear();
            _progressBar.Dispatcher.Invoke((Action) (() => { _progressBar.Value = 0; }));
        }

        public void ClearLists()
        {
            // Clear fileType lists to be ready for next organisation.
            _selectedFileTypes.Clear();
            _availableFileTypes.Clear();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Controls;
using System.Threading;
using ordoFile.Models;
using ordoFile.DataAccess;

namespace ordoFile.Organisers
{
    public class BackgroundOrganiser : OrganiserBase
    {
        //Decides whether or not the thread should run.
        bool _canWork = false;

        //Decides whether or not sub-directories of root directory
        //should be included in the organisation.
        bool _organiseSubDirectories = false;

        //Path of root directory to be organised.
        DirectoryModel _bgDirectory;

        //Thread to be run for organising files in background.
        Thread _bgProcess;

        //Declaration of variable to contain instance of the
        //configs object.
        Configs _configs;

        //Instance of the MoveFiles class used to organise files.
        Organiser _organiser;

        public BackgroundOrganiser(Organiser bgOrganiser, DirectoryModel bgDirectory, Configs configs)
        {
            _configs = configs;
            _bgProcess = new Thread(DoWork);
            _organiser = bgOrganiser;
            _bgDirectory = bgDirectory;
            _bgDirectory.Path = configs.BGDirectory;
            _organiseSubDirectories = configs.OrganiseSubDirectories;
        }

        /// <summary>
        /// Get or set the _shouldWork value which tells the thread
        /// whether or not to continue working.
        /// </summary>
        public bool CanWork
        {
            set { _canWork = value; }
            get { return _canWork; }
        }

        /// <summary>
        /// Get or set the _shouldWork value which tells the thread
        /// whether or not to continue working.
        /// </summary>
        public bool IsWorking
        {
            get { return _bgProcess.ThreadState == ThreadState.Running; }
        }

        public bool IsSleeping
        {
            get { return _bgProcess.ThreadState == ThreadState.WaitSleepJoin; }
        }

        public string Working
        {
            get { return _bgProcess.ThreadState.ToString(); }
        }

        /// <summary>
        /// Gets or sets directory that the organistion class is
        /// to organise in background process.
        /// </summary>
        public DirectoryModel BGDirectory
        {
            get { return _bgDirectory; }
            set
            {
                _bgDirectory = value;
                _configs.BGDirectory = value.Path; ;
            }
        }

        /// <summary>
        /// Sets the value which tell the organisation class whether
        /// or not to organise files in sub-directories.
        /// </summary>
        public bool OrganiseSubDirectories
        {
            set
            {
                _organiser.OrganiseSubDirectories = value;
                _organiseSubDirectories = value;
                _configs.OrganiseSubDirectories = value;
            }
        }

        /// <summary>
        /// Starts the thread for background organisation.
        /// </summary>
        public void Start()
        {
            _canWork = true;

            if (!this.IsWorking)
            {
                _bgProcess = new Thread(DoWork);
            }

            _bgProcess.Start();
        }

        /// <summary>
        /// Stops the thread for background organisation.
        /// </summary>
        public void Stop()
        {
            _canWork = false;
        }

        /// <summary>
        /// Callback for running organisation thread.
        /// </summary>
        void DoWork()
        {
            //Run as long as user does not change _canWork value.
            while (_canWork)
            {
                //Set RootDirectory of MoveFiles instance to the
                //directory currently selected for organisation.
                _organiser.RootDirectory = _bgDirectory;

                //If the user has chosen to include sub-directories in 
                //the organisation. retrieve all directories contained
                //in the root directory and pass to the MoveFiles instance.
                //Otherwise, just add the root directory to the directory list.
                if (_organiseSubDirectories)
                {
                    Organiser.TraverseDirectories(_organiser.RootDirectory, System.Windows.Threading.Dispatcher.CurrentDispatcher);
                }

                _organiser.Organise();

                if (_canWork)
                {
                    //MessageBox.Show(_bgProcess.ThreadState.ToString());
                    Thread.Sleep(60000);
                }
                else
                {
                    _bgProcess.Abort();
                }
            }
        }
    }
}


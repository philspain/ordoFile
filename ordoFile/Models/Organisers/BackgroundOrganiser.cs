using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ordoFile.DataAccess;
using System.Threading;
using System.Windows;
using ordoFile.GUITools;
using System.ComponentModel;

namespace ordoFile.Models.Organisers
{
    public class BackgroundOrganiser : OrganiserBase
    {
        //Decides whether or not the thread should run.
        bool _canWork, _sleeping = false;

        //Declaration of variable to contain instance of the
        //configs object.
        Configs _configs;

        public BackgroundOrganiser(DirectoryModel rootDirectory) : base()
        {
            _rootDirectory = rootDirectory;
            OnInitialise();
        }

        public override bool OrganiseSubDirectories
        {
            set 
            {
                base._organiseSubDirectories = value; 
                _configs.OrganiseSubDirectories = value;

                if(!value)
                    GUIDispatcherUpdates.ClearCollection(RootDirectory.Subdirectories);
            }
            get { return _configs.OrganiseSubDirectories; }
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

        private void OnInitialise()
        {
            _configs = (Configs) DependencyFactory.Container.Resolve(typeof(Configs), "configs");

            RootDirectory.Path = _configs.BGDirectory;
            OrganiseSubDirectories = _configs.OrganiseSubDirectories;

            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                if ((bool)Application.Current.Properties["bgStartup"])
                {
                    CanWork = _configs.BGDirectoryExists;
                }
            }
        }

        /// <summary>
        /// Stops the thread for background organisation.
        /// </summary>
        public void Stop()
        {
            _isWorking = false;
            _canWork = false;

            if (_sleeping)
                base._organisationThread.Abort();
        }

        /// <summary>
        /// Starts the thread for background organisation.
        /// </summary>
        public override void Organise()
        {
            if (!base._isWorking)
            {
                base._isWorking = true;

                try
                {
                    base._organisationThread = new Thread(DoWork);
                    base._organisationThread.Start();
                }
                catch(Exception ex)
                {
                    _isWorking = false;
                }
            }  
        }

        /// <summary>
        /// Callback for running organisation thread.
        /// </summary>
        void DoWork()
        {
            _canWork = true;

            //Run as long as user does not change _canWork value.
            while (_canWork)
            {
                //If the user has chosen to include sub-directories in 
                //the organisation. retrieve all directories contained
                //in the root directory and pass to the MoveFiles instance.
                //Otherwise, just add the root directory to the directory list.
                if (base._organiseSubDirectories)
                {
                    OrganiserBase.TraverseDirectories(RootDirectory);
                }

                base.Organise();
                GUIDispatcherUpdates.ClearCollection(RootDirectory.Subdirectories);

                if (_canWork)
                {
                    _sleeping = true;
                    Thread.Sleep(60000);
                    _sleeping = false;
                }
                else
                {
                    base._organisationThread.Abort();
                }
            }


            base._isWorking = false;
        }
    }
}

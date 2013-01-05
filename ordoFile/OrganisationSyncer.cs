using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ordoFile
{
    /// <summary>
    /// Class for ensuring foreground and background organisers do not 
    /// organise the same directory at the same time. Also synchronises
    /// MainViews visibility state with actions in TrayApp.
    /// </summary>
    public class OrganisationSyncer
    {
        // Background/foreground organiser running states used
        // for ensuring the same folder is not organised by both
        // organisers
        bool _backgroundOrganiserRunning,
             _foregroundOrganiserRunning;

        // Background/foreground organiser root directories used
        // for ensuring the same folder is not organised by both
        // organisers
        string _backgroundDirectory,
               _foregroundDirectory;

        System.Windows.Visibility _windowVisibility = System.Windows.Visibility.Hidden;

        /// <summary>
        /// Get or set the background directory path.
        /// </summary>
        public string BackgroundDirectory
        {
            get { return _backgroundDirectory; }
            set { _backgroundDirectory = value; }
        }

        /// <summary>
        /// Get or set the foreground directory path.
        /// </summary>
        public string ForegroundDirectory
        {
            get { return _foregroundDirectory; }
            set { _foregroundDirectory = value; }
        }

        /// <summary>
        /// Get or set the state of the background organiser
        /// </summary>
        public bool BackgroundOrganiserRunning
        {
            get { return _backgroundOrganiserRunning; }
            set { _backgroundOrganiserRunning = value; }
        }

        /// <summary>
        /// Get or set the state of the foreground organiser
        /// </summary>
        public bool ForegroundOrganiserRunning
        {
            get { return _foregroundOrganiserRunning; }
            set { _foregroundOrganiserRunning = value; }
        }

        /// <summary>
        /// Get or set the state of the foreground organiser
        /// </summary>
        public System.Windows.Visibility WindowVisibilty
        {
            get { return _windowVisibility; }
            set { _windowVisibility = value; }
        }

        /// <summary>
        /// Handler for the event to be run when background organisation
        /// is to be started.
        /// </summary>
        public event EventHandler OrganiseInBackground;

        /// <summary>
        /// Handler for the event to be run when background organisation
        /// is to be stopped.
        /// </summary>
        public event EventHandler StopBackgroundOrganisation;

        /// <summary>
        /// Method which, when called, raises the event for running
        /// background organisation or the event for cancelling it
        /// </summary>
        /// <param name="shouldOrganiseInBackground">
        /// Boolean value which decides whether or not start/stop events
        /// are raised for background organisation
        /// </param>
        public void ShouldOrganiseInBackground(bool shouldOrganiseInBackground)
        {
            if (shouldOrganiseInBackground)
            {
                if (OrganiseInBackground != null)
                {
                    OrganiseInBackground(this, EventArgs.Empty);
                }
            }
            else
            {
                if (StopBackgroundOrganisation != null)
                {
                    StopBackgroundOrganisation(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Handler for the event to be run when background organisation
        /// is to be stopped.
        /// </summary>
        public event EventHandler ChangeOrganisationTextInTray;

        /// <summary>
        /// Method which, when called, raises the event for running
        /// changing the organisation state text in the tray icon.
        /// </summary>
        public void ChangeOrganisationText()
        {
            if (ChangeOrganisationTextInTray != null)
            {
                ChangeOrganisationTextInTray(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handler for the event to be run when window visibility should be changed.
        /// </summary>
        public event EventHandler UpdateWindowVisibility;

        /// <summary>
        /// Method which, when called, raises the event for updating
        /// the main window's visibility
        /// </summary>
        public void UpdateVisibility()
        {
            if (UpdateWindowVisibility != null)
            {
                UpdateWindowVisibility(this, EventArgs.Empty);
            }
        }
    }
}

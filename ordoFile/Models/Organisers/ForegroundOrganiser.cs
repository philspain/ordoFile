using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Controls;
using System.Threading;
using ordoFile.Models;
using ordoFile.DataAccess;

namespace ordoFile.Models.Organisers
{
    public class ForegroundOrganiser : OrganiserBase
    {
        public ForegroundOrganiser(DirectoryModel rootDirectory) : base()
        {
            _rootDirectory = rootDirectory;
        }

        public override bool OrganiseSubDirectories
        {
            set { base._organiseSubDirectories = value; }
            get { return base._organiseSubDirectories; }
        }

        public override void Organise()
        {
            base._isWorking = true;

            try
            {
                base._organisationThread = new Thread(base.Organise);
                _organisationThread.Start();
            }
            catch (Exception ex)
            {
                base._isWorking = false;
            }
        }
    }
}


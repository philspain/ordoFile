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
    public class ForegroundOrganiser : OrganiserBase
    {
        public ForegroundOrganiser(CheckBox subdirCheckBox) : base()
        {
            base._rootDirectory = new DirectoryModel(subdirCheckBox);
        }

        public override bool OrganiseSubDirectories
        {
            set { base._organiseSubDirectories = value; }
        }

        public override void Run()
        {
            base._bgProcess = new Thread(Organise);
            _bgProcess.Start();
        }
    }
}


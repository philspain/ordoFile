using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace ordoFile.DataAccess
{
    public class Configs
    {
        //Decides whether or not the background process should be running
        bool _startupEnabled;

        //Directory background process should organise
        string _bgDirectory;

        //Decides whether or not the background process will organise
        //it's sub-directories.
        bool _organiseSubDirectories;

        string _configFile;

        //XmlDocument instances to hold configuration and presets data
        XmlDocument _configDoc;

        public Configs()
        {
            OnInitialise();
        }

        void OnInitialise()
        {
            _configFile = "configs.xml";

            CheckFileExists();

            _configDoc = new XmlDocument();
            _configDoc.Load(_configFile);

            SetConfigs();
        }

        /// <summary>
        /// Gets or sets enabled property which represents whether or 
        /// not background process is to run on startup.
        /// </summary>
        public bool BGDirectoryExists
        {
            get { return Directory.Exists(_bgDirectory); }
        }

        /// <summary>
        /// Gets or sets startupEnabled configuration setting which tells
        /// the background process if it can run after Windows startup.
        /// </summary>
        public bool StartupEnabled
        {
            get { return _startupEnabled; }
            set
            {
                _startupEnabled = value;
                SetBGSetting("bgStartupEnabled", value.ToString());
                System.Diagnostics.Debug.WriteLine("Startup");
                SaveConfigs();
            }
        }

        /// <summary>
        /// Gets or sets the value held in bgDirectory configuration setting.
        /// </summary>
        public string BGDirectory
        {
            get { return _bgDirectory; }
            set
            {
                _bgDirectory = value;
                SetBGSetting("bgDirectory", value);
                SaveConfigs();
            }
        }

        /// <summary>
        /// Gets or sets organiseSubDirectories configuration setting.
        /// </summary>
        public bool OrganiseSubDirectories
        {
            get { return _organiseSubDirectories; }
            set
            {
                _organiseSubDirectories = value;
                SetBGSetting("organiseSubDirectories", value.ToString());
                SaveConfigs();
            }
        }

        /// <summary>
        /// Checks that configuration files exist, if they do not, methods to 
        /// create them are called.
        /// </summary>
        void CheckFileExists()
        {
            if (!File.Exists(_configFile))
            {
                CreateConfigs();
            }
        }

        /// <summary>
        /// Create file with default configuration settings in the event one is not found.
        /// </summary>
        void CreateConfigs()
        {
            string configs = @"<?xml version=""1.0""?>
<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
    <config>
        <organiseSubDirectories>false</organiseSubDirectories>
        <bgStartupEnabled>false</bgStartupEnabled>
        <bgDirectory></bgDirectory>
    </config>
</xs:schema>";

            //Attempt to create and write to file
            try
            {
                using (FileStream newFile = File.Open(_configFile, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter writer = new StreamWriter(newFile, Encoding.UTF8))
                    {
                        writer.Write(configs);
                    }
                }

            }
            catch (IOException)
            {
            }
        }

        /// <summary>
        /// Retrieve values stored in config file and set to corresponding
        /// variables.
        /// </summary>
        void SetConfigs()
        {
            //Retrieve the value stored for "bgStartupEnabled" and attempt
            //to parse the Boolean equivalent.
            XmlNodeList bgStartupValue = _configDoc.GetElementsByTagName("bgStartupEnabled");
            Boolean.TryParse(bgStartupValue[0].InnerText, out _startupEnabled);

            //Retrieve the value stored for "organiseSubDirectories and attempt
            //to parse the Boolean equivalent.
            XmlNodeList organiseSubDirectoriesValue =
                _configDoc.GetElementsByTagName("organiseSubDirectories");
            Boolean.TryParse(organiseSubDirectoriesValue[0].InnerText, out _organiseSubDirectories);

            //Retrieve the value stored for "bgDirectory"; if the value does 
            //not represent a valid directory, set an empty string value.
            XmlNodeList bgDirectoryValue = _configDoc.GetElementsByTagName("bgDirectory");
            string dir = bgDirectoryValue[0].InnerText;

            if (Directory.Exists(dir))
            {
                _bgDirectory = dir;
            }
            else
            {
                _bgDirectory = String.Empty;
                SetBGSetting("bgDirectory", String.Empty);
            }
        }

        /// <summary>
        /// Set a value for a chosen configuration setting.
        /// </summary>
        /// <param name="setting">Setting to be changed.</param>
        /// <param name="value">Value to be set.</param>
        void SetBGSetting(string setting, string value)
        {
            XmlNodeList node = _configDoc.GetElementsByTagName(setting);

            if (node.Count > 0)
            {
                node[0].InnerText = value;
            }
        }

        /// <summary>
        /// Retrieve a value for a chosen configuration setting.
        /// </summary>
        /// <param name="setting">Setting to retrieve value for.</param>
        /// <returns>string</returns>
        string GetBGSetting(string setting)
        {
            string value = String.Empty;

            XmlNodeList node = _configDoc.GetElementsByTagName(setting);

            if (node.Count > 0)
            {
                value = node[0].InnerText;
            }

            return value;
        }

        /// <summary>
        /// Save XML data stored in global variable 'configDoc' to the
        /// file 'configs.xml'.
        /// </summary>
        public void SaveConfigs()
        {
            if (File.Exists(_configFile))
            {
                File.Delete(_configFile);
            }

            _configDoc.Save(_configFile);
        }
    }
}

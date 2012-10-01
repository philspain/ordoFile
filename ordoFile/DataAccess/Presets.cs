using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace ordoFile.DataAccess
{
    class PresetFilters
    {
        // Static instance of Presets for use as singleton return value
        static PresetFilters _presetsInstance;

        // File locations for saved variables
        string _presetsFile;

        // XmlDocument instances to hold configuration and presets data
        XmlDocument _presetsDoc;

        // Dictionary to hold presets
        Dictionary<string, List<string>> _presets = new Dictionary<string, List<string>>();

        private PresetFilters()
        {
            _presetsFile = "presets.xml";

            CheckFileExists();

            _presetsDoc = new XmlDocument();
            _presetsDoc.Load(_presetsFile);

            InitialisePresets();
        }

        /// <summary>
        /// Get XmlDocument instance that contain preset data.
        /// </summary>
        public XmlDocument PresetsDoc
        {
            get { return _presetsDoc; }
        }

        /// <summary>
        /// Returns dictionary of saved presets.
        /// </summary>
        public Dictionary<string, List<string>> Presets
        {
            get { return _presets; }
        }

        /// <summary>
        /// Static method to return singleton instance of Configs.
        /// </summary>
        public static PresetFilters GetPresets()
        {
            if (_presetsInstance == null)
            {
                _presetsInstance = new PresetFilters();
            }

            return _presetsInstance;
        }

        /// <summary>
        /// Checks that configuration files exist, if they do not, methods to 
        /// create them are called.
        /// </summary>
        void CheckFileExists()
        {
            if (!File.Exists(_presetsFile))
            {
                CreatePresets();
            }
        }

        /// <summary>
        /// Create file with default presets in the event one is not found.
        /// </summary>
        void CreatePresets()
        {
            string presets = @"<?xml version=""1.0""?>
<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema"">

    <presets>
        <preset name=""Audio"">
            <type>mp3</type>
            <type>wav</type>
            <type>gsm</type>
            <type>aac</type>
            <type>dct</type>
            <type>flac</type>
            <type>au</type>
            <type>aiff</type>
            <type>vox</type>
            <type>wma</type>
            <type>atrac</type>
            <type>ra</type>
            <type>dss</type>
            <type>msv</type>
            <type>mid</type>
            <type>ape</type>
        </preset>
        <preset name=""Images"">
            <type>gif</type>
            <type>jpg</type>
            <type>jpeg</type>
            <type>jp2</type>
            <type>jpx</type>
            <type>tiff</type>
            <type>png</type>
            <type>bmp</type>
            <type>webp</type>
            <type>psd</type>
            <type>psp</type>
            <type>img</type>
            <type>cgm</type>
            <type>svg</type>
            <type>ai</type>
            <type>xcf</type>
        </preset>
        <preset name=""Video"">
            <type>wmv</type>
            <type>mpg</type>
            <type>mpeg</type>
            <type>avi</type>
            <type>mkv</type>
            <type>3gp</type>
            <type>flv</type>
            <type>mov</type>
            <type>divx</type>
            <type>xvid</type>
            <type>mp4</type>
        </preset>
    </presets>
</xs:schema>";

            //Attempt to create and write to file
            try
            {
                using (FileStream newFile = File.Open(_presetsFile, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter writer = new StreamWriter(newFile, Encoding.UTF8))
                    {
                        writer.Write(presets);
                    }
                }

            }
            catch (IOException)
            {
            }
        }

        /// <summary>
        /// Parse XML file to load any existing preset filetype filters.
        /// </summary>
        private void InitialisePresets()
        {
            XmlNode presetNode = _presetsDoc.SelectSingleNode("//presets");

            foreach (XmlNode preset in presetNode.ChildNodes)
            {
                List<string> types = new List<string>();
                _presets.Add(preset.Attributes["name"].Value, types);

                foreach (XmlNode type in preset.ChildNodes)
                {
                    types.Add(type.InnerText);
                }
            }
        }

        /// <summary>
        /// Save changes to preset.
        /// </summary>
        /// <param name="presets">Name of preset to be edited.</param>
        /// <param name="types">List of types the preset represents after edit.</param>
        public void EditPresets(string preset, string newName, List<string> types)
        {
            XmlNode presetNode = _presetsDoc.SelectSingleNode(
                String.Format("//preset[@name='{0}']", preset));

            if (preset != newName)
            {
                presetNode.Attributes["name"].Value = newName;
                _presets[newName] = _presets[preset];
                _presets.Remove(preset);
                preset = newName;
            }

            if (presetNode != null)
            {
                string innerXml = String.Empty;

                foreach (string type in types)
                {
                    innerXml += String.Format("<type>{0}</type>", type);
                }

                presetNode.InnerXml = innerXml;
            }

            _presets[preset] = types;

            SavePresets();
        }

        public void SavePreset(string presetName, List<string> types)
        {

            XmlNode presetNode = _presetsDoc.SelectSingleNode(
                    String.Format("//preset[@name='{0}']", presetName));

            if (presetNode == null)
            {
                XmlNode presetsNode = _presetsDoc.SelectSingleNode("//presets");

                XmlNode child = _presetsDoc.CreateNode(XmlNodeType.Element, "preset", null);

                XmlAttribute name = _presetsDoc.CreateAttribute("name");
                name.Value = presetName;

                child.Attributes.Append(name);

                presetsNode.AppendChild(child);

                string innerXml = "";

                foreach (string type in types)
                {
                    innerXml += String.Format("<type>{0}</type>", type);
                }

                child.InnerXml = innerXml;
            }

            SavePresets();
        }

        public void SavePresets()
        {
            _presetsDoc.Save(_presetsFile);
        }
    }
}

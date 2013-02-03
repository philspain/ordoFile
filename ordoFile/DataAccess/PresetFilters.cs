using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace ordoFile.DataAccess
{
    public class PresetFilters
    {
        // File locations for saved variables
        string _presetsFile;

        // XmlDocument instances to hold configuration and presets data
        XmlDocument _presetsDoc;

        // Dictionary to hold presets
        Dictionary<string, List<string>> _presets = new Dictionary<string, List<string>>();

        public PresetFilters()
        {
            OnInitialise();
        }

        void OnInitialise()
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
        /// Returns list of Preset names.
        /// </summary>
        public List<string> PresetNames
        {
            get { return _presets.Keys.ToList<string>(); }
        }

        /// <summary>
        /// Checks that configuration file exists, if they do not, method to 
        /// create it are called.
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
        /// Get list of filetypes that exist in a preset.
        /// </summary>
        /// <param name="presetName">Name of the preset to return filetypes for.</param>
        /// <returns>List&lt;string&gt;</returns>
        public List<string> GetPresetTypes(string presetName)
        {
            if (_presets.ContainsKey(presetName))
            {
                return _presets[presetName];
            }

            return null;
        }

        /// <summary>
        /// Add a new preset filter.
        /// </summary>
        /// <param name="presetName">Desired name.</param>
        /// <param name="type">Chosen filetypes.</param>
        public void AddPresetType(string presetName, string type)
        {
            if (_presets.ContainsKey(presetName))
            {
                if (!String.IsNullOrEmpty(type) && !String.IsNullOrWhiteSpace(type))
                {
                    _presets[presetName].Add(type);
                }
                else
                {
                    throw new ArgumentException("Filetype provided is not valid.");
                }
            }
        }

        /// <summary>
        /// Delete a filetype from a preset filter.
        /// </summary>
        /// <param name="presetName">Name of preset to remove type from.</param>
        /// <param name="type">Filetype to remove.</param>
        public void RemovePresetType(string presetName, string type)
        {
            if (_presets.ContainsKey(presetName))
            {
                if (!String.IsNullOrEmpty(type) && !String.IsNullOrWhiteSpace(type))
                {
                    _presets[presetName].Remove(type);
                }
                else
                {
                    throw new ArgumentException("Filetype provided is not valid.");
                }
            }
        }

        /// <summary>
        /// Check if a preset exists.
        /// </summary>
        /// <param name="presetName">Name of preset to check.</param>
        /// <returns>bool</returns>
        public bool PresetExists(string presetName)
        {
            return _presets.ContainsKey(presetName);
        }

        /// <summary>
        /// Check if a filetype exists in a preset.
        /// </summary>
        /// <param name="presetName">Name of preset to check.</param>
        /// <param name="type">Filetype to check.</param>
        /// <returns>bool</returns>
        public bool PresetTypeExists(string presetName, string type)
        {
            if (_presets.ContainsKey(presetName))
            {
                return _presets[presetName].Contains(type);
            }

            return false;
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

            if (presetNode != null)
            {
                if (!String.IsNullOrWhiteSpace(newName) && String.IsNullOrEmpty(newName) &&
                        types != null && types.Count > 0)
                    throw new ArgumentException("All arguments are not valid");

                if (preset != newName)
                {
                    // Change preset name in XML
                    presetNode.Attributes["name"].Value = newName;

                    // Remove preset from collection and add under new name
                    _presets.Remove(preset);
                    _presets.Add(newName, types);
                }
                else
                {
                    // Change types in collection for preset.
                    _presets[preset] = types;
                }
            
                string innerXml = String.Empty;

                foreach (string type in types)
                {
                    innerXml += String.Format("<type>{0}</type>", type);
                }

                presetNode.InnerXml = innerXml;
            }

            SavePresets();
        }

        /// <summary>
        /// Add a new preset.
        /// </summary>
        /// <param name="presetName">Name of preset.</param>
        /// <param name="types">List of filetypes for preset.</param>
        public void AddPreset(string presetName, List<string> types)
        {
            if (presetName != string.Empty && presetName != null &&
                types != null && types.Count > 0)
            {
                // Check if there is an existing preset node with 
                // name attribute that matches new preset to add
                XmlNode presetNode = _presetsDoc.SelectSingleNode(
                        String.Format("//preset[@name='{0}']", presetName));

                // Continue of there is no matching preset
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

                _presets.Add(presetName, types);

                SavePresets();
            }
            else
            {
                throw new ArgumentException("All arguments are not valid");
            }
        }

        /// <summary>
        /// Save changes to file.
        /// </summary>
        public void SavePresets()
        {
            _presetsDoc.Save(_presetsFile);
        }
    }
}

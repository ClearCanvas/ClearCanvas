#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using ClearCanvas.Common;
using System.Xml;

namespace ClearCanvas.Server.ShredHost
{
	[Obsolete("Use standard ApplicationSettingsBase-derived classes with ApplicationSettingsExtensions to set shared settings if so desired. Use LegacyShredConfigSectionAttribute and IMigrateLegacyShredConfigSection to maintain shred settings migration compatibility.")]
    public abstract class ShredConfigSection : ConfigurationSection, ICloneable
    {
    	private Dictionary<string, string> _removedProperties;

		//[ConfigurationProperty("sampleProperty", DefaultValue="test")]
        //public string SampleProperty
        //{
        //    get { return (string)this["sampleProperty"]; }
        //    set { this["sampleProperty"] = value; }
        //}

        // Need to implement a clone to fix a .Net bug in ConfigurationSectionCollection.Add
        public abstract object Clone();

		public string GetRemovedPropertyValue(string name)
		{
			if (_removedProperties == null)
				return null;

			string value;
			return _removedProperties.TryGetValue(name, out value) ? value : null;
		}

		private void AddRemovedProperty(string name, string value)
		{
			if (_removedProperties == null)
				_removedProperties = new Dictionary<string, string>();

			_removedProperties[name] = value;
		}

    	protected sealed override bool OnDeserializeUnrecognizedAttribute(string name, string value)
    	{
			if (!ShredSettingsMigrator.IsMigrating)
				return false;

    		AddRemovedProperty(name, value);
			return true;
    	}

    	protected sealed override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
		{
			if (!ShredSettingsMigrator.IsMigrating)
				return false;

			if (!reader.IsEmptyElement)
			{
				var subTreeReader = reader.ReadSubtree();
				subTreeReader.MoveToContent();
				AddRemovedProperty(elementName, subTreeReader.ReadOuterXml());
				subTreeReader.Close();
			}

			return true;
		}

		internal void LoadXml(string xml)
		{
			using (var stringReader = new StringReader(xml))
			using (var xmlReader = XmlReader.Create(stringReader))
			{
				base.DeserializeSection(xmlReader);
			}
		}
	}

	[Obsolete("Use standard ApplicationSettingsBase-derived classes with ApplicationSettingsExtensions to set shared settings if so desired. Use LegacyShredConfigSectionAttribute and IMigrateLegacyShredConfigSection to maintain shred settings migration compatibility.")]
    public static class ShredConfigManager
    {
        public static ConfigurationSection GetConfigSection(string sectionName)
        {
            System.Configuration.Configuration config =
                    ConfigurationManager.OpenExeConfiguration(
                    ConfigurationUserLevel.None);

            return (config == null ? null : config.Sections[sectionName]);
        }

        public static bool UpdateConfigSection(string sectionName, ShredConfigSection section)
        {
            try
            {
                // Get the current configuration file.
                System.Configuration.Configuration config =
                        ConfigurationManager.OpenExeConfiguration(
                        ConfigurationUserLevel.None);

                if (config.Sections[sectionName] == null)
                {
                    section.SectionInformation.ForceSave = true;
                    config.Sections.Add(sectionName, section);
                }
                else
                {
                    config.Sections.Remove(sectionName);
                    config.Sections.Add(sectionName, section.Clone() as ConfigurationSection);
                }

                config.Save(ConfigurationSaveMode.Full);
            }
            catch (ConfigurationErrorsException err)
            {
                Platform.Log(LogLevel.Info, err);
                return false;
            }

            return true;
        }        
    }
}

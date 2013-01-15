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
using System.Text;
using System.Xml;
using System.IO;

namespace ClearCanvas.Enterprise.Common
{
    class SettingsParser
    {
        internal void FromXml(string xml, IDictionary<string, string> values)
        {
            if (xml != null)
            {
                var settings = new XmlReaderSettings() {IgnoreWhitespace = true};

                using (XmlReader reader = XmlReader.Create(new StringReader(xml), settings))
                {
                    ReadXml(reader, values);
                }
            }
        }

        internal string ToXml(IDictionary<string, string> values)
        {
            StringWriter sw = new StringWriter();
            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                WriteXml(writer, values);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Overwrites any values stored in this instance with values having the same key
        /// from a previous version
        /// </summary>
        /// <param name="previousVersion">The previous version from which to copy settings values</param>
        internal string UpgradeFromPrevious(string currentVersionXml, string previousVersionXml)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            FromXml(currentVersionXml, values);

            // overwrite any values with those from the previous version
            FromXml(previousVersionXml, values);

            // return the document that represents the upgrade
            return ToXml(values);
        }
        
        #region XML de/serialization

        private void ReadXml(XmlReader reader, IDictionary<string, string> values)
        {
            string settingName = "";

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "setting":
                            settingName = reader.GetAttribute("name");
                            break;
                        case "value":
                            values[settingName] = reader.ReadElementContentAsString();
                            break;
                    }
                }
            }
        }

        private void WriteXml(XmlWriter writer, IDictionary<string, string> values)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("settings");
            foreach (KeyValuePair<string, string> kvp in values)
            {
                writer.WriteStartElement("setting");
                writer.WriteAttributeString("name", kvp.Key);

                writer.WriteStartElement("value");
                writer.WriteCData(kvp.Value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        #endregion
    }
}

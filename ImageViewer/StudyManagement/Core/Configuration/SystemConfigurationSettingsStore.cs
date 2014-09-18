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

using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.ImageViewer.Common.Configuration;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Configuration
{
	// TODO (Marmot) - This is copied from the CC.Enterprise.Common assembly, figure out a way to share it
	internal class SettingsParser
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
			var sw = new StringWriter();
			using (var writer = new XmlTextWriter(sw))
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
		/// <param name="currentVersionXml"> </param>
		/// <param name="previousVersionXml">The previous version from which to copy settings values</param>
		internal string UpgradeFromPrevious(string currentVersionXml, string previousVersionXml)
		{
			var values = new Dictionary<string, string>();
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

	/// <summary>
	/// This class is an implementation of <see cref="ISettingsStore"/> that uses the database.
	/// </summary>
	public class SystemConfigurationSettingsStore : ISystemConfigurationSettingsStore
	{
		#region ISystemConfigurationSettingsStore Members

		/// <summary>
		/// Gets the settings group that immediately precedes the one provided.
		/// </summary>
		public SettingsGroupDescriptor GetPreviousSettingsGroup(SettingsGroupDescriptor group)
		{
			using (var context = new DataAccessContext())
			{
				var documentKey = new ConfigurationDocumentKey(group.Name, group.Version, null, null);
				var broker = context.GetConfigurationDocumentBroker();

				var document = broker.GetPriorConfigurationDocument(documentKey);

				if (document != null)
				{
					return new SettingsGroupDescriptor(document.DocumentName, VersionUtils.FromPaddedVersionString(document.DocumentVersionString), string.Empty, group.AssemblyQualifiedTypeName, group.HasUserScopedSettings);
				}

				return null;
			}
		}

		public Dictionary<string, string> GetSettingsValues(SettingsGroupDescriptor group, string user,
		                                                    string instanceKey)
		{
			using (var context = new DataAccessContext())
			{
				var documentKey = new ConfigurationDocumentKey(group.Name, group.Version, user, instanceKey);
				var broker = context.GetConfigurationDocumentBroker();

				var document = broker.GetConfigurationDocument(documentKey);

				var values = new Dictionary<string, string>();
				if (document != null)
				{
					var parser = new SettingsParser();
					parser.FromXml(document.DocumentText, values);
				}

				return values;
			}
		}

		public void PutSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey,
		                              Dictionary<string, string> dirtyValues)
		{
			using (var context = new DataAccessContext())
			{
				// next we obtain any previously stored configuration document for this settings group
				var documentKey = new ConfigurationDocumentKey(group.Name, group.Version, user, instanceKey);
				var broker = context.GetConfigurationDocumentBroker();

				var values = new Dictionary<string, string>();
				var parser = new SettingsParser();

				var document = broker.GetConfigurationDocument(documentKey);
				if (document == null)
				{
					document = new ConfigurationDocument
					           	{
					           		CreationTime = Platform.Time,
					           		DocumentName = group.Name,
					           		DocumentVersionString = VersionUtils.ToPaddedVersionString(group.Version, false, false),
					           		User = user,
					           		DocumentText = string.Empty
					           	};
					broker.AddConfigurationDocument(document);
				}
				else
				{
					// parse document
					parser.FromXml(document.DocumentText, values);
				}

				// update the values that have changed
				foreach (var kvp in dirtyValues)
					values[kvp.Key] = kvp.Value;

				try
				{
					if (values.Count > 0)
					{
						// generate the document, update local cache and server
						document.DocumentText = parser.ToXml(values);
						context.Commit();
					}
				}
				catch (EndpointNotFoundException e)
				{
					Platform.Log(LogLevel.Debug, e, "Unable to save settings to configuration service.");
				}
			}
		}

		#endregion
	}
}
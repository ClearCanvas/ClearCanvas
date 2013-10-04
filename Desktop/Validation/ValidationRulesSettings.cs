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

using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using ClearCanvas.Common.Configuration;
using System.Collections.Generic;

namespace ClearCanvas.Desktop.Validation
{
	[SettingsGroupDescription("Stores user-interface custom validation rules.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class ValidationRulesSettings
	{
		private XmlDocument _rulesDoc;

		private ValidationRulesSettings()
		{
		}

		/// <summary>
		/// Gets the XML rules document.
		/// </summary>
		public XmlDocument RulesDocument
		{
			get
			{
				InitializeOnce();
				return _rulesDoc;
			}
		}

		//TODO (CR Sept 2010): we can now save these values via ApplicationSettingsExtensions

		/// <summary>
		/// Saves any changes made to the rules document to the specified settings store.
		/// </summary>
		/// <param name="settingsStore"></param>
		public void Save(ISettingsStore settingsStore)
		{
			// need to save changes to XML doc
			if (!Initialized)
				return;

			var sb = new StringBuilder();
			var writer = new XmlTextWriter(new StringWriter(sb)) { Formatting = Formatting.Indented };
			_rulesDoc.Save(writer);
			var xml = sb.ToString();

			// if value has not changed, nothing to save
			if (xml == this.CustomRulesXml)
				return;

			var values = new Dictionary<string, string> { { "CustomRulesXml", xml } };

			settingsStore.PutSettingsValues(
				new SettingsGroupDescriptor(this.GetType()),
				null,
				null,
				values);
		}

		private bool Initialized
		{
			get { return _rulesDoc != null; }
		}

		private void InitializeOnce()
		{
			if (Initialized)
				return;

			_rulesDoc = new XmlDocument {PreserveWhitespace = true};
			_rulesDoc.LoadXml(this.CustomRulesXml);
		}
	}
}

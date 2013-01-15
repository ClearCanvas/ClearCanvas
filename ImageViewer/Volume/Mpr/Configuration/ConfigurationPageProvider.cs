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
using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Configuration
{
	[ExtensionOf(typeof (ConfigurationPageProviderExtensionPoint))]
	public class ConfigurationPageProvider : IConfigurationPageProvider, IConfigurationProvider
	{
		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			if (PermissionsHelper.IsInRole(ImageViewer.AuthorityTokens.ViewerVisible))
				yield return new ConfigurationPage(MprConfigurationComponent.Path, new MprConfigurationComponent());
		}

		#endregion

		#region Implementation of IConfigurationProvider

		public string SettingsClassName
		{
			get { return typeof(MprSettings).FullName; }
		}

		public void UpdateConfiguration(Dictionary<string, string> settings)
		{
			foreach (var key in settings.Keys)
			{
				switch (key)
				{
					case "SliceSpacingFactor":
						MprSettings.Default.SliceSpacingFactor = float.Parse(settings[key]);
						break;
					case "AutoSliceSpacing":
						MprSettings.Default.AutoSliceSpacing = bool.Parse(settings[key]);
						break;
					default:
						var message = string.Format("{0} with key={1} is not implemented", this.SettingsClassName, key);
						throw new NotImplementedException(message);
				}
			}

			MprSettings.Default.Save();
		}

		#endregion
	}
}
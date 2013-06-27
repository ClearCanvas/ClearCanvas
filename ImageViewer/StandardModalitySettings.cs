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
using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	[SettingsGroupDescription("A list of standard DICOM modalities that can be used anywhere a list of modalities is required.")]
	[SettingsProvider(typeof(StandardSettingsProvider))]
	internal sealed partial class StandardModalitySettings : IMigrateSettings
	{
		private StandardModalitySettings()
		{
		}

		private static List<string> GetModalities(string modalities)
		{
			return CollectionUtils.Map(modalities.Split(','), (string s) => s.Trim());
		}

		private static string CombineModalities(string modalities1, string modalities2)
		{
			var combined = new SortedDictionary<string, string>();
			foreach (string modality in GetModalities(modalities1 ?? ""))
				combined[modality] = modality;
			foreach (string modality in GetModalities(modalities2 ?? ""))
				combined[modality] = modality;

			return StringUtilities.Combine(combined.Values, ",");
		}

		public List<string> GetModalities()
		{
			return GetModalities(Modalities);
		}

		#region IMigrateSettings Members

		public void MigrateSettingsProperty(SettingsPropertyMigrationValues migrationValues)
		{
			if (migrationValues.PropertyName != "Modalities")
				return;

			migrationValues.CurrentValue = CombineModalities(migrationValues.CurrentValue as string,
			                                                 migrationValues.PreviousValue as string);
	}

		#endregion
	}
}

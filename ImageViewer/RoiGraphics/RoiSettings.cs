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
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// A helper class for reading ROI settings.
	/// </summary>
	public static class RoiSettingsHelper
	{
		/// <summary>
		/// Gets a value indicating whether or not ROI stats should be shown on new ROI objects by default.
		/// </summary>
		public static bool ShowAnalysisByDefault
		{
			get { return RoiSettings.Default.ShowAnalysisByDefault; }
		}

		/// <summary>
		/// Gets a value indicating the preferred linear, area and volume units of ROI analysis output.
		/// </summary>
		public static Units AnalysisUnits
		{
			get { return RoiSettings.Default.AnalysisUnits; }
		}
	}

	[SettingsGroupDescription("Configures ROI settings.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	internal sealed partial class RoiSettings
	{
		public RoiSettings()
		{
		}
	}
}
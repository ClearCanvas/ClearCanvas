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
using System.Configuration;

namespace ClearCanvas.Healthcare.Printing
{
	/// <summary>
	/// Provides application settings for print template URLs.
	/// </summary>
	/// <remarks>
	/// This code is adapted from the Visual Studio generated template code;  the generated code has been removed from the project.  Additional 
	/// settings need to be manually added to this class.
	/// </remarks>
	[SettingsGroupDescription("Configures print template URLs.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal partial class PrintTemplateSettings : ApplicationSettingsBase
	{
		public PrintTemplateSettings()
		{
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("http://localhost/ris/print_templates/")]
		[global::System.Configuration.SettingsDescription("Specifies base URL for print templates.  Must be a localhost URL and must end with a trailing slash (e.g. http://localhost/ris/print_templates/")]
		public string BaseUrl
		{
			get { return (string) (this["BaseUrl"]); }
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("report/report.htm")]
		[global::System.Configuration.SettingsDescription("Specifies template for radiology reports.")]
		public string ReportTemplateUrl
		{
			get
			{
				return Combine(this.BaseUrl, (string)this["ReportTemplateUrl"]);
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("downtime/downtime.htm")]
		[global::System.Configuration.SettingsDescription("Specifies template for downtime forms.")]
		public string DowntimeFormTemplateUrl
		{
			get
			{
				return Combine(this.BaseUrl, (string)this["DowntimeFormTemplateUrl"]);
			}
		}

		private static string Combine(string baseUrl, string relativeUrl)
		{
			return new Uri(new Uri(baseUrl), relativeUrl).ToString();
		}
	}
}

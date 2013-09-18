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
using System.Globalization;
using System.Text;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Clipboard.View.WinForms.Properties
{
	[SettingsGroupDescription("View settings in the clipboard component.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	internal sealed partial class Settings
	{
		public Settings()
		{
		}

		public int[] CustomColorsArray
		{
			get
			{
				string value = this.CustomColors;
				if (string.IsNullOrEmpty(value))
					return new int[0];

				List<int> list = new List<int>();
				foreach (string s in value.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
				{
					int v;
					if (int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out v))
						list.Add(v);
				}
				return list.ToArray();
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					this.CustomColors = string.Empty;
					return;
				}

				StringBuilder sb = new StringBuilder();
				foreach (int i in value)
				{
					sb.AppendFormat(i.ToString("x6", CultureInfo.InvariantCulture));
					sb.Append(',');
				}
				this.CustomColors = sb.ToString(0, Math.Max(0, sb.Length - 1));
			}
		}
	}
}
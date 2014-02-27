#region License

// Copyright (c) 2014, ClearCanvas Inc.
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

namespace ClearCanvas.Enterprise.Core.ServerVersion
{
	/// <summary>
	/// Specifies the compatibility version details to be reported by the <see cref="VersionService.GetVersion"/> service.
	/// </summary>
	[SettingsProvider(typeof (LocalFileSettingsProvider))]
	internal sealed partial class LegacyServiceSettings
	{
		private Version _compatibilityVersion;

		public Version GetCompatibilityVersion()
		{
			if (_compatibilityVersion == null && !Version.TryParse(CompatibilityVersion, out _compatibilityVersion))
				_compatibilityVersion = new Version(1, 0, 0, 0);
			return _compatibilityVersion;
		}
	}
}
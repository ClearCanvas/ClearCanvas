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

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common.ServerVersion;

namespace ClearCanvas.Enterprise.Core.ServerVersion
{
	[ExtensionOf(typeof (CoreServiceExtensionPoint))]
	[ServiceImplementsContract(typeof (IVersionService))]
	public class VersionService : CoreServiceLayer, IVersionService
	{
		/// <summary>
		/// Legacy version service method, which now returns the compatibility version of the server.
		/// </summary>
		public GetVersionResponse GetVersion(GetVersionRequest request)
		{
			var version = LegacyServiceSettings.Default.GetCompatibilityVersion();
			var response = new GetVersionResponse
			               	{
			               		Component = ProductInformation.Component,
			               		Edition = LegacyServiceSettings.Default.CompatibilityEdition,
			               		VersionMajor = version.Major,
			               		VersionMinor = version.Minor,
			               		VersionBuild = version.Build,
			               		VersionRevision = version.Revision
			               	};
			return response;
		}

		/// <summary>
		/// Gets the actual version of the server.
		/// </summary>
		public GetVersionResponse GetVersion2(GetVersionRequest request)
		{
			var response = new GetVersionResponse
			               	{
			               		Component = ProductInformation.Component,
			               		Edition = ProductInformation.Edition,
			               		VersionMajor = ProductInformation.Version.Major,
			               		VersionMinor = ProductInformation.Version.Minor,
			               		VersionBuild = ProductInformation.Version.Build,
			               		VersionRevision = ProductInformation.Version.Revision
			               	};
			return response;
		}
	}
}
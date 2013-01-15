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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.Manifest;
using ClearCanvas.Utilities.Manifest;

namespace ClearCanvas.Ris.Application.Services.Manifest
{
	[ServiceImplementsContract(typeof (IRisManifestService))]
	[ExtensionOf(typeof (ApplicationServiceExtensionPoint))]
	public class RisManifestService : ApplicationServiceBase, IRisManifestService
	{
		public GetStatusResponse GetStatus(GetStatusRequest request)
		{
			Platform.Log(LogLevel.Debug, "Received manifest status query.");
			return new GetStatusResponse {IsValid = ManifestVerification.Valid};
		}
	}
}
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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core.Upgrade;

namespace ClearCanvas.ImageServer.Model.SqlServer.UpgradeScripts
{
	/// <summary>
	/// Upgrade from the Rigel milestone to the Orion milestone.
	/// </summary>
	[ExtensionOf(typeof(PersistentStoreUpgradeScriptExtensionPoint))]
	internal class UpgradeFrom_11_0_14821_1095 : BaseUpgradeScript
	{
		public UpgradeFrom_11_0_14821_1095()
			: base(new Version(11, 0, 14821, 1095), null, "UpgradeFrom_11_0_14821_1095.sql")
		{
		}
	}
}

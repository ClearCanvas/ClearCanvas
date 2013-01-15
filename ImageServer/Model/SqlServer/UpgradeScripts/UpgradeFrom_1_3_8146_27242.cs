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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core.Upgrade;

namespace ClearCanvas.ImageServer.Model.SqlServer.UpgradeScripts
{
    [ExtensionOf(typeof(PersistentStoreUpgradeScriptExtensionPoint))]
    class UpgradeFrom_1_3_8146_27242 : BaseUpgradeScript
    {
        //In versions prior to 1.5 the use of Build and Revision were swapped and so it has to be swapped here in order for the utility to properly detect the older version
        public UpgradeFrom_1_3_8146_27242()
            : base(new Version(1, 3, 27242, 8146), new Version(1, 4, 29698, 9277), "UpgradeFrom_1_3_8146_27241.sql")
        {
        }
    }
}
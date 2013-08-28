
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
    /// <summary>
    /// PesoPlus to Yen milestone upgrade script
    /// </summary>
    /// <remarks>
    /// This is the PesoPlus SP1 release.  It was built on top of PesoPlus (6.2.10561.12082) and had no database changes.
    /// PesoPlus was built on top of Peso (6.1.7081.10268) and had no database changes. 
    /// Adding upgrade from PesoPlus SP1 to Yen here, which is the same as the upgrade from Peso to Yen (and PesoPlus to Yen), and hence using the Peso to Yen upgrade script. 
    /// </remarks>
    [ExtensionOf(typeof(PersistentStoreUpgradeScriptExtensionPoint))]
    class UpgradeFrom_6_2_10943_12112 : BaseUpgradeScript
    {
        public UpgradeFrom_6_2_10943_12112()
            : base(new Version(6, 2, 10943, 12112), new Version(9, 0, 9561, 11723), "UpgradeFrom_6_1_7081_10268.sql")
        {
        }
    }
}
#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core.Upgrade;

namespace ClearCanvas.Healthcare.Hibernate.UpgradeScripts
{
    /// <summary>
    /// Upgrade from the Bennu milestone to the Rigel milestone.
    /// </summary>
    [ExtensionOf(typeof(PersistentStoreUpgradeScriptExtensionPoint))]
    internal class UpgradeFrom_10_1_13120_656 : BaseUpgradeScript
    {
        public UpgradeFrom_10_1_13120_656()
            : base(new Version(10, 1, 13120, 656), new Version(11, 0, 14821, 1095), "UpgradeFrom_10_1_13120_656.sql")
        {
        }
    }
}

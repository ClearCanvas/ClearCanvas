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
    /// Upgrade from the Rigel milestone to the Orion milestone.
    /// </summary>
    [ExtensionOf(typeof(PersistentStoreUpgradeScriptExtensionPoint))]
    internal class UpgradeFrom_11_0_14821_1095 : BaseUpgradeScript
    {
        public UpgradeFrom_11_0_14821_1095()
            : base(new Version(11, 0, 14821, 1095), null, "UpgradeFrom_NoOp.sql")
        {
        }
    }
}

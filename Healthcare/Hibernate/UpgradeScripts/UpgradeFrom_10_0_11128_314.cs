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
    /// Upgrade from the Phoenix5 milestone to the Bennu milestone.
    /// </summary>
    [ExtensionOf(typeof(PersistentStoreUpgradeScriptExtensionPoint))]
    internal class UpgradeFrom_10_0_11128_314 : BaseUpgradeScript
    {
        public UpgradeFrom_10_0_11128_314()
            : base(new Version(10, 0, 11128, 314), new Version(10, 1, 13120, 656), "UpgradeFrom_NoOp.sql")
        {
        }
    }
}

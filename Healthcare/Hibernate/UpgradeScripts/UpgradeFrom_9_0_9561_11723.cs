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
    /// Upgrade from the Yen milestone to the Phoenix5 milestone.
    /// </summary>
    [ExtensionOf(typeof(PersistentStoreUpgradeScriptExtensionPoint))]
    internal class UpgradeFrom_9_0_9561_11723 : BaseUpgradeScript
    {
        public UpgradeFrom_9_0_9561_11723()
            : base(new Version(9, 0, 9561, 11723), new Version(10, 0, 11128, 314), "UpgradeFrom_NoOp.sql")
        {
        }
    }
}

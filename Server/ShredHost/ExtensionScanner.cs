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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;

namespace ClearCanvas.Server.ShredHost
{
    internal class ExtensionScanner : MarshalByRefObject
    {
        /// <summary>
        /// Uses the ExtensionPoint type to scan the plugins folder for extensions.
        /// </summary>
        /// <returns>
        /// An enumerable collection that contains information on each extension that will help the loader
        /// load them into the AppDomain. If no extensions are found, null is returned.</returns>
        public ShredStartupInfoList ScanExtensions()
        {
            Platform.Log(LogLevel.Debug, this.GetType().ToString() + ":" + this.GetType().Name + " in AppDomain [" + AppDomain.CurrentDomain.FriendlyName + "]");

            var shredInfoList = new ShredStartupInfoList();
			var xp = new ShredExtensionPoint();
            var shredObjects = xp.CreateExtensions();
            foreach (var shredObject in shredObjects)
            {
				var shredType = shredObject.GetType();
            	var asShred = shredObject as IShred;
				if (asShred == null)
				{
					Platform.Log(LogLevel.Debug, "Shred extension '{0}' does not implement IShred.", shredType.FullName);
					continue;
				}

            	var shredIsolation = shredType.GetCustomAttributes(typeof (ShredIsolationAttribute), true).OfType<ShredIsolationAttribute>().FirstOrDefault();
            	var shredIsolationLevel = shredIsolation == null ? ShredIsolationLevel.OwnAppDomain : shredIsolation.Level;

				if (shredIsolationLevel != ShredIsolationLevel.None)
				{
					Platform.Log(LogLevel.Info, "Shred {0} is running in a seperate app domain", shredType.Name);
				}

				var assemblyPath = new Uri(shredType.Assembly.CodeBase);
				var startupInfo = new ShredStartupInfo(assemblyPath, ((IShred)shredObject).GetDisplayName(), shredType.FullName, shredIsolationLevel);
                shredInfoList.Add(startupInfo);
            }

            return shredInfoList;
        }
    }
}

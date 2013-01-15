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

            ShredStartupInfoList shredInfoList = new ShredStartupInfoList();
            ShredExtensionPoint xp = new ShredExtensionPoint();
            object[] shredObjects = xp.CreateExtensions();
            foreach (object shredObject in shredObjects)
            {
                if (shredObject is IShred)
                {
                    Uri assemblyPath = new Uri(shredObject.GetType().Assembly.CodeBase);
                    shredInfoList.Add(new ShredStartupInfo(assemblyPath, (shredObject as IShred).GetDisplayName(), shredObject.GetType().FullName));
                }
            }

            return shredInfoList;
        }
    }
}

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

using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.ImageViewer.Common.Configuration;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    // TODO (CR Jun 2012): Leaving internal, at least for now, since the StorageConfiguration data contract
    // actually provides a useful abstraction and some important logic, and there's really no immediate need to change it.

    [SettingsGroupDescription("Configuration for local study storage.")]
    [SettingsProvider(typeof (SystemConfigurationSettingsProvider))]
    internal sealed partial class StorageSettings
    {
        public sealed class Proxy
        {
            private readonly StorageSettings _settings;

            public Proxy(StorageSettings settings)
            {
                _settings = settings;
            }

            private object this[string propertyName]
            {
                get { return _settings[propertyName]; }
                set { _settings.SetSharedPropertyValue(propertyName, value); }
            }

            public double MinimumFreeSpacePercent
            {
                get { return _settings.MinimumFreeSpacePercent; }
                set { this["MinimumFreeSpacePercent"] = value; }
            }

            public string FileStoreDirectory
            {
                get { return _settings.FileStoreDirectory; }
                set { this["FileStoreDirectory"] = value; }
            }

            public void Save()
            {
                _settings.Save();
            }
        }

        public Proxy GetProxy()
        {
            return new Proxy(this);
        }
    }
}

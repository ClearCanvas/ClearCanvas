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

using System.Collections.Generic;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.Common.Configuration
{
    public interface ISystemConfigurationSettingsStore
    {        
        /// <summary>
        /// Gets the settings group that immediately precedes the one provided.
        /// </summary>
        SettingsGroupDescriptor GetPreviousSettingsGroup(SettingsGroupDescriptor group);

        // TODO (CR Jun 2012): It's a bit weird that system configuration has support for "user" settings. That should be removed, I think.

        /// <summary>
        /// Obtains the settings values for the specified settings group, user and instance key.  If user is null,
        /// the shared settings are obtained.
        /// </summary>
        /// <remarks>
        /// The returned dictionary may contain values for all settings in the group, or it may
        /// contain only those values that differ from the default values defined by the settings group.
        /// </remarks>
        Dictionary<string, string> GetSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey);

        /// <summary>
        /// Store the settings values for the specified settings group, for the current user and
        /// specified instance key.  If user is null, the values are stored as shared settings.
        /// </summary>
        /// <remarks>
        /// The <paramref name="dirtyValues"/> dictionary should contain values for any settings that are dirty.
        /// </remarks>
        void PutSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey, Dictionary<string, string> dirtyValues);
    }
}

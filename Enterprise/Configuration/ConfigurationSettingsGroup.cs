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
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Enterprise.Core.Modelling;

namespace ClearCanvas.Enterprise.Configuration {


    /// <summary>
    /// ConfigurationSettingsGroup entity
    /// </summary>
	[UniqueKey("SettingsGroupKey", new string[]{"Name", "VersionString"})]
	public partial class ConfigurationSettingsGroup : ClearCanvas.Enterprise.Core.Entity
	{
        public static ConfigurationSettingsGroupSearchCriteria GetCriteria(SettingsGroupDescriptor descriptor)
        {
            ConfigurationSettingsGroupSearchCriteria where = new ConfigurationSettingsGroupSearchCriteria();
            where.Name.EqualTo(descriptor.Name);
            where.VersionString.EqualTo(VersionUtils.ToPaddedVersionString(descriptor.Version, false, false));
            return where;
        }


		/// <summary>
		/// Updates this instance from the specified descriptor.
		/// </summary>
		/// <param name="descriptor"></param>
        public virtual void UpdateFromDescriptor(SettingsGroupDescriptor descriptor)
        {
            _assemblyQualifiedTypeName = descriptor.AssemblyQualifiedTypeName;
            _name = descriptor.Name;
            _versionString = VersionUtils.ToPaddedVersionString(descriptor.Version, false, false);
            _description = descriptor.Description;
            _hasUserScopedSettings = descriptor.HasUserScopedSettings;
        }

		/// <summary>
		/// Returns a descriptor populated from this instance.
		/// </summary>
		/// <returns></returns>
        public virtual SettingsGroupDescriptor GetDescriptor()
        {
            return new SettingsGroupDescriptor(
                _name,
                VersionUtils.FromPaddedVersionString(_versionString),
                _description,
                _assemblyQualifiedTypeName,
                _hasUserScopedSettings);
        }
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}
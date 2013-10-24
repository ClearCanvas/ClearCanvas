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
using System.Collections.Generic;
using System.Configuration;

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
	/// An extension point for <see cref="ISettingsStore"/>s.
	/// </summary>
	[ExtensionPoint]
	public sealed class SettingsStoreExtensionPoint : ExtensionPoint<ISettingsStore> {}

	public abstract class SettingsStore : ISettingsStore
	{
		private static readonly SettingsStoreExtensionPoint _extensionPoint = new SettingsStoreExtensionPoint();

		static SettingsStore()
		{
			IsSupported = _extensionPoint.ListExtensions().Length > 0;
		}

		public static bool IsSupported { get; private set; }

		public static bool IsStoreOnline
		{
			get
			{
				try
				{
					return IsSupported && Create().IsOnline;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		public static ISettingsStore Create()
		{
			return (ISettingsStore) _extensionPoint.CreateExtension();
		}

		#region Implementation of ISettingsStore

		public abstract bool IsOnline { get; }
		public abstract bool SupportsImport { get; }
		public abstract IList<SettingsGroupDescriptor> ListSettingsGroups();
		public abstract SettingsGroupDescriptor GetPreviousSettingsGroup(SettingsGroupDescriptor group);
		public abstract IList<SettingsPropertyDescriptor> ListSettingsProperties(SettingsGroupDescriptor group);
		public abstract void ImportSettingsGroup(SettingsGroupDescriptor group, List<SettingsPropertyDescriptor> properties);
		public abstract Dictionary<string, string> GetSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey);
		public abstract void PutSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey, Dictionary<string, string> dirtyValues);
		public abstract void RemoveUserSettings(SettingsGroupDescriptor group, string user, string instanceKey);

		#endregion

		#region Unit Test Support

#if UNIT_TESTS

		internal static void SetIsSupported(bool value)
		{
			IsSupported = value;
		}

#endif

		#endregion
	}

	/// <summary>
	/// Defines the interface to a mechanism for the storage of application and user settings.
	/// </summary>
	/// <remarks>
	/// This interface is more specialized than <see cref="IConfigurationStore"/>, in that it is designed
	/// specifically to support derivatives of the <see cref="SettingsProvider"/> class in order to support the .NET 
	/// settings framework.
	/// </remarks>
	public interface ISettingsStore
	{
		/// <summary>
		/// Gets whether or not the settings store is online, and can be used.
		/// </summary>
		bool IsOnline { get; }

		/// <summary>
		/// Gets a value indicating whether this store supports importing of meta-data.
		/// </summary>
		bool SupportsImport { get; }

		/// <summary>
		/// Lists all settings groups for which this store maintains settings values.
		/// </summary>
		/// <remarks>
		/// Generally this corresponds to the the list of all types derived from <see cref="ApplicationSettingsBase"/> found
		/// in all installed plugins and related assemblies.
		/// </remarks>
		IList<SettingsGroupDescriptor> ListSettingsGroups();

		/// <summary>
		/// Gets the settings group that immediately precedes the one provided.
		/// </summary>
		SettingsGroupDescriptor GetPreviousSettingsGroup(SettingsGroupDescriptor group);

		/// <summary>
		/// Lists the settings properties for the specified settings group.
		/// </summary>
		IList<SettingsPropertyDescriptor> ListSettingsProperties(SettingsGroupDescriptor group);

		/// <summary>
		/// Imports meta-data for the specified settings group and its properties.
		/// </summary>
		/// <param name="group"></param>
		/// <param name="properties"></param>
		void ImportSettingsGroup(SettingsGroupDescriptor group, List<SettingsPropertyDescriptor> properties);

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

		/// <summary>
		/// Removes user settings from this group, effectively causing them to be reset to their shared default
		/// values.
		/// </summary>
		/// <remarks>
		/// Application-scoped settings are unaffected.
		/// </remarks>
		void RemoveUserSettings(SettingsGroupDescriptor group, string user, string instanceKey);
	}
}
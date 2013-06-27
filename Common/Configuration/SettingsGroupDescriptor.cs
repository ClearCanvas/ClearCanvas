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
using System.Linq;
using System.Runtime.Serialization;

namespace ClearCanvas.Common.Configuration
{
	[Flags]
	public enum SettingsGroupFilter
	{
		/// <summary>
		/// All settings groups (no filter).
		/// </summary>
		All = 0,

		/// <summary>
		/// Settings groups that will be stored in the enterprise configuration store, if one exists.
		/// </summary>
		SupportEnterpriseStorage		= 0x01,

		/// <summary>
		/// Settings groups that use some form of local storage.
		/// </summary>
		LocalStorage					= 0x02,

		/// <summary>
		/// Settings groups that support editing of the shared profile.
		/// </summary>
		SupportsEditingOfSharedProfile	= 0x04,
	}

	/// <summary>
	/// Describes a settings group.
	/// </summary>
	[DataContract]
	public class SettingsGroupDescriptor : IEquatable<SettingsGroupDescriptor>
	{
		/// <summary>
		/// Returns a list of <see cref="SettingsGroupDescriptor"/> objects describing each settings class
		/// that exists in the installed plugin base.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		public static List<SettingsGroupDescriptor> ListInstalledSettingsGroups()
		{
			return ListInstalledSettingsGroups(SettingsGroupFilter.All);
		}

		/// <summary>
		/// Returns a list of <see cref="SettingsGroupDescriptor"/> objects describing each settings class
		/// that exists in the installed plugin base, applying the specified filter.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		public static List<SettingsGroupDescriptor> ListInstalledSettingsGroups(SettingsGroupFilter filter)
		{
			return ListInstalledSettingsGroups(type => ApplyFilter(filter, type));
		}

		private static List<SettingsGroupDescriptor> ListInstalledSettingsGroups(Predicate<Type> filter)
		{
			var assemblies = Platform.PluginManager.Plugins
				.Select(p => p.Assembly.Resolve())
				.Concat(new[] { typeof(SettingsGroupDescriptor).Assembly });

			var q = from assembly in assemblies
					from t in assembly.GetTypes()
					where t.IsSubclassOf(typeof(ApplicationSettingsBase)) && !t.IsAbstract && filter(t)
					select new SettingsGroupDescriptor(t);

			return q.ToList();
		}

		//Internal for unit tests
		internal static bool ApplyFilter(SettingsGroupFilter filter, Type type)
		{
			if (filter == 0)
				return true;

			var providerClass = SettingsClassMetaDataReader.GetSettingsProvider(type);
			if ((filter & SettingsGroupFilter.SupportEnterpriseStorage) == SettingsGroupFilter.SupportEnterpriseStorage
				&& providerClass == typeof(StandardSettingsProvider))
				return true;

			if ((filter & SettingsGroupFilter.LocalStorage) == SettingsGroupFilter.LocalStorage
				&& ApplicationSettingsHelper.IsLocallyStored(type))
				return true;

			if ((filter & SettingsGroupFilter.SupportsEditingOfSharedProfile) == SettingsGroupFilter.SupportsEditingOfSharedProfile
				&& typeof(ISharedApplicationSettingsProvider).IsAssignableFrom(providerClass))
				return true;

			return false;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SettingsGroupDescriptor(string name, Version version, string description, string assemblyQualifiedTypeName,
			bool hasUserScopedSettings)
		{
			Name = name;
			Version = version;
			Description = description;
			AssemblyQualifiedTypeName = assemblyQualifiedTypeName;
			HasUserScopedSettings = hasUserScopedSettings;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SettingsGroupDescriptor(Type settingsClass)
		{
			Name = SettingsClassMetaDataReader.GetGroupName(settingsClass);
			Version = SettingsClassMetaDataReader.GetVersion(settingsClass, true);
			Description = SettingsClassMetaDataReader.GetGroupDescription(settingsClass);
			HasUserScopedSettings = SettingsClassMetaDataReader.HasUserScopedSettings(settingsClass);
			AssemblyQualifiedTypeName = GetSafeClassName(settingsClass);
		}

		/// <summary>
		/// Gets the name of the settings group.
		/// </summary>
		[DataMember]
		public string Name { get; private set; }

		/// <summary>
		/// Gets the version of the settings group.
		/// </summary>
		[DataMember]
		public Version Version { get; private set; }

		/// <summary>
		/// Gets the description of the settings group.
		/// </summary>
		[DataMember]
		public string Description { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this settings class has user-scoped settings.
		/// </summary>
		[DataMember]
		public bool HasUserScopedSettings { get; private set; }

		/// <summary>
		/// Gets the assembly-qualified type name of the class that implements the settings group.
		/// </summary>
		[DataMember]
		public string AssemblyQualifiedTypeName { get; private set; }

		/// <summary>
		/// Settings groups are considered equal if they have the same name and version.
		/// </summary>
		public override bool Equals(object obj)
		{
			return this.Equals(obj as SettingsGroupDescriptor);
		}

		/// <summary>
		/// Gets the hash code for this object.
		/// </summary>
		public override int GetHashCode()
		{
			return Name.GetHashCode() ^ Version.GetHashCode();
		}

		public override string ToString()
		{
			return Name;
		}

		#region IEquatable<SettingsGroupDescriptor> Members

		/// <summary>
		/// Settings groups are considered equal if they have the same name and version.
		/// </summary>
		public bool Equals(SettingsGroupDescriptor other)
		{
			return other != null && this.Name == other.Name && this.Version == other.Version;
		}

		#endregion

		/// <summary>
		/// Gets the assembly qualified name of the type, but without all the version and culture info.
		/// </summary>
		/// <param name="settingsClass"></param>
		/// <returns></returns>
		private static string GetSafeClassName(Type settingsClass)
		{
			return string.Format("{0}, {1}", settingsClass.FullName, settingsClass.Assembly.GetName().Name);
		}

	}
}
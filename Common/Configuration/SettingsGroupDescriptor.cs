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
using ClearCanvas.Common.Utilities;
using System.Runtime.Serialization;
using System.Reflection;

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
	/// Describes a settings group.
	/// </summary>
	[DataContract]
	public class SettingsGroupDescriptor : IEquatable<SettingsGroupDescriptor>
	{
		//TODO (CR Sept 2010): get rid of the specialized list methods and just
		//put properties on this class that say whether or not the settings group is "local"
	
		/// <summary>
		/// Returns a list of <see cref="SettingsGroupDescriptor"/> objects describing each settings class
		/// that exists in the installed plugin base, and is a locally stored setting.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		public static List<SettingsGroupDescriptor> ListInstalledLocalSettingsGroups()
		{
			return ListInstalledSettingsGroups(IsLocalSettingsGroup);
		}

		/// <summary>
		/// Returns a list of <see cref="SettingsGroupDescriptor"/> objects describing each settings class
		/// that exists in the installed plugin base.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If <param name="excludeLocalSettingsGroups"/> is true, this method only returns settings classes
		/// where:
		/// <list type="bullet">
		/// <item>the <see cref="StandardSettingsProvider"/> is used for persistence.</item>
		/// <item>the <see cref="StandardSettingsProvider"/> is using a valid <see cref="ISettingsStore"/> via
		/// the <see cref="SettingsStoreExtensionPoint"/>.  When there is no such extension, such settings are
		/// actually stored locally.</item>
		/// </list>
		/// </para>
		/// <para>
		/// This method is thread-safe.
		/// </para>
		/// </remarks>
		public static List<SettingsGroupDescriptor> ListInstalledSettingsGroups(bool excludeLocalSettingsGroups)
		{
			return ListInstalledSettingsGroups(t => !excludeLocalSettingsGroups || !IsLocalSettingsGroup(t));
		}

		private static bool IsLocalSettingsGroup(Type t)
		{
			return ApplicationSettingsHelper.IsLocal(t);
		}

		private static List<SettingsGroupDescriptor> ListInstalledSettingsGroups(Predicate<Type> includePredicate)
		{
			includePredicate = includePredicate ?? (obj => true);

			var groups = new List<SettingsGroupDescriptor>();

			List<Assembly> assemblies = CollectionUtils.Map(Platform.PluginManager.Plugins, (PluginInfo p) => p.Assembly);
			assemblies.Add(typeof(SettingsGroupDescriptor).Assembly);

			foreach (var assembly in assemblies)
			{
				foreach (Type t in assembly.GetTypes())
				{
					if (t.IsSubclassOf(typeof(ApplicationSettingsBase)) && !t.IsAbstract)
					{
						if (includePredicate(t))
							groups.Add(new SettingsGroupDescriptor(t));
					}
				}
			}

			return groups;
		}

		private string _name;
		private Version _version;
		private string _description;
		private string _assemblyQualifiedTypeName;
		private bool _hasUserScopedSettings;

		/// <summary>
		/// Constructor.
		/// </summary>
		public SettingsGroupDescriptor(string name, Version version, string description, string assemblyQualifiedTypeName,
			bool hasUserScopedSettings)
		{
			_name = name;
			_version = version;
			_description = description;
			_assemblyQualifiedTypeName = assemblyQualifiedTypeName;
			_hasUserScopedSettings = hasUserScopedSettings;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SettingsGroupDescriptor(Type settingsClass)
		{
			_name = SettingsClassMetaDataReader.GetGroupName(settingsClass);
			_version = SettingsClassMetaDataReader.GetVersion(settingsClass);
			_description = SettingsClassMetaDataReader.GetGroupDescription(settingsClass);
			_hasUserScopedSettings = SettingsClassMetaDataReader.HasUserScopedSettings(settingsClass);
			_assemblyQualifiedTypeName = GetSafeClassName(settingsClass);
		}

		/// <summary>
		/// Gets the name of the settings group.
		/// </summary>
		[DataMember]
		public string Name
		{
			get { return _name; }
			private set { _name = value; }
		}

		/// <summary>
		/// Gets the version of the settings group.
		/// </summary>
		[DataMember]
		public Version Version
		{
			get { return _version; }
			private set { _version = value; }
		}

		/// <summary>
		/// Gets the description of the settings group.
		/// </summary>
		[DataMember]
		public string Description
		{
			get { return _description; }
			private set { _description = value; }
		}

		/// <summary>
		/// Gets a value indicating whether this settings class has user-scoped settings.
		/// </summary>
		[DataMember]
		public bool HasUserScopedSettings
		{
			get { return _hasUserScopedSettings; }
			private set { _hasUserScopedSettings = value; }
		}

		/// <summary>
		/// Gets the assembly-qualified type name of the class that implements the settings group.
		/// </summary>
		[DataMember]
		public string AssemblyQualifiedTypeName
		{
			get { return _assemblyQualifiedTypeName; }
			private set { _assemblyQualifiedTypeName = value; }
		}

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
			return _name.GetHashCode() ^ _version.GetHashCode();
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
			return other != null && this._name == other._name && this._version == other._version;
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
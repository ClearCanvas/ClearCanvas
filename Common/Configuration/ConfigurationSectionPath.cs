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

namespace ClearCanvas.Common.Configuration
{
	public class ConfigurationSectionGroupPath : IEquatable<ConfigurationSectionGroupPath>
	{
		private const string _pathSeparator = "/";
		private const string _root = "";
		private const string _applicationSettings = "applicationSettings";
		private const string _userSettings = "userSettings";

		private readonly List<string> _pathSegments;

		private ConfigurationSectionGroupPath()
		{
			_pathSegments = new List<string>();
		}

		public ConfigurationSectionGroupPath(string path)
			: this((path ?? "").Split(new[] { _pathSeparator }, StringSplitOptions.RemoveEmptyEntries))
		{
		}

		public ConfigurationSectionGroupPath(params string[] pathSegments)
		{
			Platform.CheckForNullReference(pathSegments, "pathSegments");
			Platform.CheckPositive(pathSegments.Length, "pathSegments.Length");
			foreach (var pathSegment in pathSegments)
			{
				Platform.CheckForEmptyString(pathSegment, "pathSegments");
				if (pathSegment.Contains(_pathSeparator))
					throw new ArgumentException("Path segment cannot contain a forward slash");
	
			}
			
			_pathSegments = new List<string>(pathSegments);
		}

		#region IEquatable<ConfigurationSectionGroupPath> Members

		public bool Equals(ConfigurationSectionGroupPath other)
		{
			if (_pathSegments.Count != other._pathSegments.Count)
				return false;

			for (int i = 0; i < _pathSegments.Count; ++i)
			{
				if (_pathSegments[i] != other._pathSegments[i])
					return false;
			}

			return true;
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj is ConfigurationSectionGroupPath)
				return Equals((ConfigurationSectionGroupPath) obj);
			return false;
		}

		public override int GetHashCode()
		{
			return 0x256E187F ^ ToString().GetHashCode();
		}

		public override string ToString()
		{
			if (IsRoot)
				return _root;

			return StringUtilities.Combine(_pathSegments, _pathSeparator);
		}

		public bool IsRoot { get { return _pathSegments.Count == 0; } }

		public bool IsApplicationSettings
		{
			get { return _pathSegments.Count == 1 && _pathSegments[0] == _applicationSettings; }
		}

		public bool IsUserSettings
		{
			get { return _pathSegments.Count == 1 && _pathSegments[0] == _userSettings; }
		}

		public static readonly ConfigurationSectionGroupPath Root = new ConfigurationSectionGroupPath();
		public static readonly ConfigurationSectionGroupPath ApplicationSettings = new ConfigurationSectionGroupPath(_applicationSettings);
		public static readonly ConfigurationSectionGroupPath UserSettings = new ConfigurationSectionGroupPath(_userSettings);

		public static implicit operator string(ConfigurationSectionGroupPath groupPath)
		{
			return groupPath.ToString();
		}

		public static implicit operator ConfigurationSectionGroupPath(string groupPath)
		{
			return new ConfigurationSectionGroupPath(groupPath);
		}

		public static implicit operator ConfigurationSectionGroupPath(SettingScope scope)
		{
			return scope == SettingScope.Application ? ApplicationSettings : UserSettings;
		}

		public ConfigurationSectionGroupPath GetChildGroupPath(string name)
		{
			List<string> segments = new List<string>(_pathSegments);
			segments.Add(name);
			return new ConfigurationSectionGroupPath(segments.ToArray());
		}

		public ConfigurationSectionGroupPath GetParentPath()
		{
			if (IsRoot)
				throw new InvalidOperationException("Can't go above the root level of a path");

			if (_pathSegments.Count == 1)
				return Root;

			List<string> segments = new List<string>(_pathSegments);
			segments.RemoveAt(segments.Count - 1);
			return new ConfigurationSectionGroupPath(segments.ToArray());
		}

		public ConfigurationSectionGroup GetSectionGroup(System.Configuration.Configuration configuration, bool create)
		{
			bool created;
			return GetSectionGroup(configuration, create, out created);
		}
	
		public ConfigurationSectionGroup GetSectionGroup(System.Configuration.Configuration configuration, bool create, out bool created)
		{
			created = false;
			if (IsRoot)
				return configuration.RootSectionGroup;

			if (!create)
				return configuration.GetSectionGroup(this);

			var path = Root;
			ConfigurationSectionGroup group = configuration.RootSectionGroup;

			foreach (string pathSegment in _pathSegments)
			{
				path = path.GetChildGroupPath(pathSegment);
				var childGroup = group.SectionGroups[pathSegment];
				if (childGroup != null)
				{
					group = childGroup;
					continue;
				}

				created = true;
				childGroup = path.CreateSectionGroup();
				group.SectionGroups.Add(pathSegment, childGroup);
				group = group.SectionGroups[pathSegment];
			}

			return group;
		}

		public ConfigurationSectionGroup CreateSectionGroup()
		{
			if (IsApplicationSettings)
				return new ApplicationSettingsGroup();
			if (IsUserSettings)
				return new UserSettingsGroup();
			
			//There are other system defined ones, but we don't use them.
			return new ConfigurationSectionGroup();
		}
	}

	public class ConfigurationSectionPath : IEquatable<ConfigurationSectionPath>
	{
		private const string _pathSeparator = "/";

		public ConfigurationSectionPath(Type settingsClass, SettingScope scope)
		{
			ApplicationSettingsHelper.CheckType(settingsClass);
			GroupPath = scope;
			SectionName = settingsClass.FullName;
		}

		public ConfigurationSectionPath(ConfigurationSectionGroupPath groupPath, string sectionName)
		{
			Platform.CheckForNullReference(groupPath, "groupPath");
			Platform.CheckForEmptyString(sectionName, "sectionName");

			GroupPath = groupPath;
			SectionName = sectionName;
		}

		public ConfigurationSectionGroupPath GroupPath { get; private set; }
		public string SectionName { get; private set; }

		public ConfigurationSection GetSection(System.Configuration.Configuration configuration)
		{
			return GetSection(configuration, false);
		}

		public ConfigurationSection GetSection(System.Configuration.Configuration configuration, bool create)
		{
			bool created;
			return GetSection(configuration, create, out created);
		}

		public ConfigurationSection GetSection(System.Configuration.Configuration configuration, bool create, out bool created)
		{
			created = false;
			var group = GroupPath.GetSectionGroup(configuration, create);
			if (group == null) //create is false.
				return null;

			var section = group.Sections[SectionName];
			if (section == null && create)
			{
				created = true;
				section = CreateSection();
				group.Sections.Add(SectionName, section);
			}

			return section;
		}

		public ConfigurationSection CreateSection()
		{
			var section = new ClientSettingsSection();
			section.SectionInformation.RequirePermission = false;
			if (GroupPath.IsUserSettings)
				section.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
			
			return section;
		}

		#region IEquatable<ConfigurationSectionPath> Members

		public bool Equals(ConfigurationSectionPath other)
		{
			return other.GroupPath.Equals(GroupPath) && other.SectionName.Equals(SectionName);
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj is ConfigurationSectionPath)
				return Equals((ConfigurationSectionPath)obj);

			return false;
		}

		public override int GetHashCode()
		{
			int hash = 0x315387A1;
			hash ^= GroupPath.GetHashCode();
			hash ^= SectionName.GetHashCode();
			return hash;
		}

		public override string ToString()
		{
			if (GroupPath.IsRoot)
				return SectionName;

			return GroupPath + _pathSeparator + SectionName;
		}

		public static implicit operator string(ConfigurationSectionPath sectionPath)
		{
			return sectionPath.ToString();
		}
	}
}
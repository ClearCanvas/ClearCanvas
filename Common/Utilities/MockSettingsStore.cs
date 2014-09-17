#region License

// Copyright (c) 2014, ClearCanvas Inc.
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

#if UNIT_TESTS

using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Mock in-memory implementation of <see cref="ISettingsStore"/> for tests purposes.
	/// </summary>
	/// <remarks>
	/// This class may be used to simulate the
	/// </remarks>
	public sealed class MockSettingsStore : ISettingsStore
	{
		private readonly Dictionary<SettingsGroupDescriptor, Group> _groups = new Dictionary<SettingsGroupDescriptor, Group>();

		public bool IsOnline
		{
			get { return true; }
		}

		public bool SupportsImport
		{
			get { return false; }
		}

		public void Clear()
		{
			_groups.Clear();
		}

		public IList<SettingsGroupDescriptor> ListSettingsGroups()
		{
			return _groups.Keys.ToList().AsReadOnly();
		}

		public SettingsGroupDescriptor GetPreviousSettingsGroup(SettingsGroupDescriptor @group)
		{
			return null;
		}

		public IList<SettingsPropertyDescriptor> ListSettingsProperties(SettingsGroupDescriptor @group)
		{
			Group g;
			return _groups.TryGetValue(@group, out g) ? g.SettingsProperties : Enumerable.Empty<SettingsPropertyDescriptor>().ToList().AsReadOnly();
		}

		public void ImportSettingsGroup(SettingsGroupDescriptor @group, List<SettingsPropertyDescriptor> properties)
		{
			Group g;
			if (!_groups.TryGetValue(@group, out g)) _groups.Add(group, g = new Group());
			g.ImportSettingsProperties(g.SettingsProperties);
		}

		public Dictionary<string, string> GetSettingsValues(SettingsGroupDescriptor @group, string user, string instanceKey)
		{
			Group g;
			return _groups.TryGetValue(@group, out g) ? g.GetSettingsValues(user, instanceKey) : GetEmptySettingsDictionary();
		}

		public void PutSettingsValues(SettingsGroupDescriptor @group, string user, string instanceKey, Dictionary<string, string> dirtyValues)
		{
			Group g;
			if (!_groups.TryGetValue(@group, out g)) _groups.Add(group, g = new Group());
			g.PutSettingsValues(user, instanceKey, dirtyValues);
		}

		public void RemoveUserSettings(SettingsGroupDescriptor @group, string user, string instanceKey)
		{
			Group g;
			if (_groups.TryGetValue(@group, out g)) g.RemoveSettingsValues(user, instanceKey);
		}

		private static Dictionary<string, string> GetEmptySettingsDictionary()
		{
			return new Dictionary<string, string>(0);
		}

		private class Group
		{
			private readonly Dictionary<SettingsKey, IDictionary<string, string>> _settings = new Dictionary<SettingsKey, IDictionary<string, string>>();
			private IList<SettingsPropertyDescriptor> _properties = new List<SettingsPropertyDescriptor>().AsReadOnly();

			public IList<SettingsPropertyDescriptor> SettingsProperties
			{
				get { return _properties; }
			}

			public void ImportSettingsProperties(IEnumerable<SettingsPropertyDescriptor> properties)
			{
				_properties = properties.ToList().AsReadOnly();
			}

			public Dictionary<string, string> GetSettingsValues(string user, string instanceKey)
			{
				IDictionary<string, string> settings;
				var key = new SettingsKey(user, instanceKey);
				return _settings.TryGetValue(key, out settings) ? settings.ToDictionary(x => x.Key, x => x.Value) : GetEmptySettingsDictionary();
			}

			public void PutSettingsValues(string user, string instanceKey, IDictionary<string, string> dirtyValues)
			{
				IDictionary<string, string> settings;
				var key = new SettingsKey(user, instanceKey);
				if (_settings.TryGetValue(key, out settings))
				{
					foreach (var x in dirtyValues)
						settings[x.Key] = x.Value;
				}
				else _settings.Add(key, new Dictionary<string, string>(dirtyValues));
			}

			public void RemoveSettingsValues(string user, string instanceKey)
			{
				var key = new SettingsKey(user, instanceKey);
				_settings.Remove(key);
			}

			private struct SettingsKey
			{
				private readonly string _user;
				private readonly string _instanceKey;

				public SettingsKey(string user, string instanceKey)
				{
					_user = user;
					_instanceKey = instanceKey;
				}

				public override int GetHashCode()
				{
					return (_user != null ? _user.GetHashCode() : 0) ^ (_instanceKey != null ? _instanceKey.GetHashCode() : 0);
				}

				public override bool Equals(object obj)
				{
					return obj is SettingsKey && ((SettingsKey) obj)._user == _user && ((SettingsKey) obj)._instanceKey == _instanceKey;
				}

				public override string ToString()
				{
					return string.Concat(_user, ":", _instanceKey);
				}
			}
		}
	}
}

#endif
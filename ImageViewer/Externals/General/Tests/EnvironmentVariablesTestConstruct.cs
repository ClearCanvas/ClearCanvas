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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Externals.General.Tests
{
	public sealed class EnvironmentVariablesTestConstruct : IDisposable
	{
		private const string _format = "__CC_TEST_{0}";
		private readonly EnvironmentVariableTarget _target;
		private readonly IList<string> _keys;

		public EnvironmentVariablesTestConstruct(EnvironmentVariableTarget target)
		{
			_target = target;
			_keys = new List<string>();
		}

		public string this[string key]
		{
			get
			{
				if (string.IsNullOrEmpty(key))
					throw new ArgumentNullException("key");
				string fullKey = string.Format(_format, key);

				return Environment.GetEnvironmentVariable(fullKey, _target);
			}
			set
			{
				if (string.IsNullOrEmpty(key))
					throw new ArgumentNullException("key");
				string fullKey = string.Format(_format, key);

				Environment.SetEnvironmentVariable(fullKey, value, _target);
				if (!_keys.Contains(key))
					_keys.Add(key);
			}
		}

		public string Format(string key)
		{
			if (string.IsNullOrEmpty(key))
				throw new ArgumentNullException("key");
			return string.Format("%{0}%", string.Format(_format, key));
		}

		public string PercentChar
		{
			get { return "%%"; }
		}

		public void Dispose()
		{
			foreach (var key in _keys)
			{
				try
				{
					string fullKey = string.Format(_format, key);
					Environment.SetEnvironmentVariable(fullKey, string.Empty, _target);
				}
				catch (Exception)
				{
					// don't throw exceptions in a Dispose()
				}
			}
		}
	}
}

#endif
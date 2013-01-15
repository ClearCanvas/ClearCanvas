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

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Configuration
{
	/// <summary>
	/// A default implementation of <see cref="IConfigurationPage"/>.
	/// </summary>
	/// <typeparam name="T">The type of <see cref="IConfigurationApplicationComponent"/>-derived 
	/// class that will be hosted in this page.  The class must have a parameterless default constructor.</typeparam>
	public class ConfigurationPage<T> : ConfigurationPage
		 where T : IConfigurationApplicationComponent, new()
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="path">The path to the <see cref="ConfigurationPage{T}"/>.</param>
		public ConfigurationPage(string path)
			: base(path, new T())
		{
		}
	}
	
	public class ConfigurationPage : IConfigurationPage
	{
		private readonly IConfigurationApplicationComponent _component;
		private readonly string _path;

		public ConfigurationPage(string path, IConfigurationApplicationComponent component)
		{
			_path = path;
			_component = component;
		}

		#region IConfigurationPage Members

		/// <summary>
		/// Gets the path to this page.
		/// </summary>
		public string GetPath()
		{
			return _path;
		}

		/// <summary>
		/// Gets the <see cref="IApplicationComponent"/> that is hosted in this page.
		/// </summary>
		public IApplicationComponent GetComponent()
		{
			return _component;
		}

		/// <summary>
		/// Saves any configuration changes that were made.
		/// </summary>
		public void SaveConfiguration()
		{
			_component.Save();
		}

		#endregion
	}
}

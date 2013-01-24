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
using System.Reflection;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Conveys information about plugins as they are loaded.
	/// </summary>
	/// <remarks>
	/// This class is used internally by the framework.
	/// </remarks>
	/// <see cref="PluginManager"/>
	public class PluginLoadedEventArgs : EventArgs
	{
		internal PluginLoadedEventArgs(string message, Assembly pluginAssembly)
		{
			Message = message;
			PluginAssembly = pluginAssembly;
		}

		/// <summary>
		/// Gets a user-friendly message describing the plugin that was loaded.
		/// </summary>
		/// <remarks>
		/// This is typically just the full name of the plugin assembly.
		/// </remarks>
		public string Message { get; private set; }

		/// <summary>
		/// Gets the plugin assembly that was loaded, if any.
		/// </summary>
		/// <remarks>
		/// Null if no actual assembly was loaded for this particular event.
		/// </remarks>
		public Assembly PluginAssembly { get; private set; }
	}
}

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

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
	/// Defines an extension point for configuration stores.  Used by <see cref="ConfigurationStore"/>
	/// to create configuration store instances.
	/// </summary>
	[ExtensionPoint]
	public class ConfigurationStoreExtensionPoint : ExtensionPoint<IConfigurationStore>
	{
	}

	/// <summary>
	/// Factory class for creating instances of <see cref="IConfigurationStore"/>.
	/// </summary>
	public static class ConfigurationStore
	{
		static ConfigurationStore()
		{
			IsSupported = new ConfigurationStoreExtensionPoint().ListExtensions().Length > 0;
		}

		/// <summary>
		/// Gets a value indicating whether the configuration store is supported under the current deployment.
		/// </summary>
		public static bool IsSupported { get; private set; }

		/// <summary>
		/// Obtains an instance of the configuration store, if supported.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="NotSupportedException">Indicates that there is no configuration store extension.</exception>
		public static IConfigurationStore Create()
		{
			return (IConfigurationStore)new ConfigurationStoreExtensionPoint().CreateExtension();
		}
	}
}

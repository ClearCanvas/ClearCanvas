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
using System.IO;
using System.Linq;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Loads plugin assemblies dynamically from disk and exposes meta-data about the set of installed
	/// plugins, extension points, and extensions to the application.
	/// </summary>
	public class PluginManager
	{
		private readonly List<PluginInfo> _plugins = new List<PluginInfo>();
		private readonly List<ExtensionInfo> _extensions = new List<ExtensionInfo>();
		private readonly List<ExtensionPointInfo> _extensionPoints = new List<ExtensionPointInfo>();
		private readonly string _pluginDir;

		private readonly object _syncLock = new object();
		private readonly PluginLoader _loader;
		private volatile bool _pluginsLoaded;

		internal PluginManager(string pluginDir)
		{
			_pluginDir = pluginDir;
			_loader = new PluginLoader(pluginDir);
		}

		#region Public API

		/// <summary>
		/// Gets information about the set of all installed plugins.
		/// </summary>
		/// <remarks>
		/// If plugins have not yet been loaded into memory, querying this property will cause them to be loaded.
		/// </remarks>
		public IList<PluginInfo> Plugins
		{
			get
			{
				EnsurePluginsLoaded();
				return _plugins.AsReadOnly();
			}
		}

		/// <summary>
		/// Gets information about the set of extensions defined across all installed plugins,
		/// including disabled and unlicensed extensions.
		/// </summary>
		/// <remarks>
		/// If plugins have not yet been loaded into memory, querying this property will cause them to be loaded.
		/// </remarks>
		public IList<ExtensionInfo> Extensions
		{
			get
			{
				EnsurePluginsLoaded();
				return _extensions.AsReadOnly();
			}
		}

		/// <summary>
		/// Gets information about the set of extension points defined across all installed plugins.  
		/// </summary>
		/// <remarks>
		/// If plugins have not yet been loaded into memory, querying this property will cause them to be loaded.
		/// </remarks>
		public IList<ExtensionPointInfo> ExtensionPoints
		{
			get
			{
				EnsurePluginsLoaded();
				return _extensionPoints.AsReadOnly();
			}
		}

		/// <summary>
		/// Occurs when a plugin is loaded.
		/// </summary>
		public event EventHandler<PluginProcessedEventArgs> PluginProcessed
		{
			add
			{
				lock (_syncLock)
				{
					_loader.PluginProcessed += value;
				}
			}
			remove
			{
				lock (_syncLock)
				{
					_loader.PluginProcessed -= value;
				}
			}
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Ensures plugins are loaded exactly once.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <exception cref="PluginException">Specified plugin directory does not exist or 
		/// a problem has occurred while loading a plugin.</exception>
		private void EnsurePluginsLoaded()
		{
			if (!_pluginsLoaded)
			{
				lock (_syncLock)
				{
					if (!_pluginsLoaded)
					{
						LoadPlugins();
					}
				}
			}
		}

		/// <summary>
		/// Loads all plugins in the current plugin directory.
		/// </summary>
		/// <remarks>
		/// This method will traverse the plugin directory and all its subdirectories loading
		/// all valid plugin assemblies.  A valid plugin is an assembly that is marked with an assembly
		/// attribute of type <see cref="ClearCanvas.Common.PluginAttribute"/>.
		/// </remarks>
		/// <exception cref="PluginException">Specified plugin directory does not exist.</exception>
		private void LoadPlugins()
		{
			if (!Directory.Exists(_pluginDir))
				throw new PluginException(SR.ExceptionPluginDirectoryNotFound);

			_plugins.AddRange(_loader.LoadPlugins());

			// If no plugins were loaded, nothing else to do
			if (_plugins.Count == 0)
				return;

			// compile lists of all extension points and extensions
			var extensions = new List<ExtensionInfo>(_plugins.SelectMany(p => p.Extensions));
			var points = new List<ExtensionPointInfo>(_plugins.SelectMany(p => p.ExtensionPoints));

			// hack: add points and extensions from ClearCanvas.Common, which isn't technically a plugin
			PluginInfo.DiscoverExtensionPointsAndExtensions(GetType().Assembly, points, extensions);

			// #742: order the extensions according to the XML configuration
			List<ExtensionInfo> ordered, remainder;
			ExtensionSettings.Default.OrderExtensions(extensions, out ordered, out remainder);

			// create global extension list, with the ordered set appearing first
			_extensions.AddRange(CollectionUtils.Concat<ExtensionInfo>(ordered, remainder));

			// points do not need to be ordered
			_extensionPoints.AddRange(points);

			_pluginsLoaded = true;
		}


		#endregion
	}
}
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
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Desktop
{
	//TODO (CR March 2011): Move to Common, or remove altogether

	/// <summary>
	/// A Singleton class that provides a way for <see cref="ApplicationSettingsBase"/>-derived objects
	/// to be updated when a setting value was modified externally.
	/// </summary>
	/// <remarks>
    /// This class provides a way to update existing instances of settings objects derived from
    /// <see cref="ApplicationSettingsBase"/>.  The individual instances must register themselves
    /// with this class in order to receive updates.
    /// </remarks>
	public class ApplicationSettingsRegistry
	{
		private static readonly ApplicationSettingsRegistry _instance = new ApplicationSettingsRegistry();

		private readonly object _syncLock = new object();
		private readonly List<ApplicationSettingsBase> _registeredSettingsInstances;

		private ApplicationSettingsRegistry()
		{
			_registeredSettingsInstances = new List<ApplicationSettingsBase>();
		}
		
		/// <summary>
		/// Gets the single instance of this class.
		/// </summary>
		public static ApplicationSettingsRegistry Instance
		{
			get { return _instance; }
		}

		/// <summary>
		/// Registers an instance of a settings class.
		/// </summary>
		public void RegisterInstance(ApplicationSettingsBase settingsInstance)
		{
		    try
		    {
                //TODO (Phoenix5): #10730 - remove this when it's fixed.
                if (Application.GuiToolkitID == null || Application.GuiToolkitID == GuiToolkitID.Web)
                    return;
		    }
		    catch (Exception)
		    {
                //Just let it get added; this can only happen for a setting initialized before the application itself is initialized.
		    }

			lock(_syncLock)
			{
				if (!_registeredSettingsInstances.Contains(settingsInstance))
					_registeredSettingsInstances.Add(settingsInstance);
			}
		}

		/// <summary>
		/// Unregisters an instance of a settings class.
		/// </summary>
		public void UnregisterInstance(ApplicationSettingsBase settingsInstance)
		{
			lock (_syncLock)
			{
				if (_registeredSettingsInstances.Contains(settingsInstance))
					_registeredSettingsInstances.Remove(settingsInstance);
			}
		}

		/// <summary>
		/// Calls <see cref="ApplicationSettingsBase.Reload"/> on all registered 
		/// settings instances that match the specified group.
		/// </summary>
		public void Reload(SettingsGroupDescriptor group)
		{
			Type settingsClass = Type.GetType(group.AssemblyQualifiedTypeName, false);
			if (settingsClass != null)
			{
				lock (_syncLock)
				{
					_registeredSettingsInstances
						.FindAll(delegate(ApplicationSettingsBase instance) { return instance.GetType().Equals(settingsClass); })
						.ForEach(delegate(ApplicationSettingsBase instance) { instance.Reload(); });
				}
			}
		}
	}
}
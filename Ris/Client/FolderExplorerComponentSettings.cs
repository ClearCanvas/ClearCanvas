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
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Provides services for storing the folder and folder system structure 
	/// to an XML document, and rebuilding that folder structure from the document.
	/// </summary>
	[SettingsGroupDescription("Configures folder systems.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class FolderExplorerComponentSettings
	{
		private readonly IFolderExplorerConfiguration _defaultConfig;
		private readonly IFolderExplorerUserConfiguration _userConfig;

		private FolderExplorerComponentSettings()
		{
			_defaultConfig = new FolderExplorerConfiguration(() => this.DefaultConfigurationXml);
			_userConfig = new FolderExplorerUserConfiguration(() => this.UserConfigurationXml, updatedUserConfigurationXml =>
				{
					this.UserConfigurationXml = updatedUserConfigurationXml;
					Save();
				});
		}

		#region Public API

		/// <summary>
		/// Orders the folder systems according to the default and user specific settings.
		/// </summary>
		/// <param name="folderSystems">Input list of folder systems</param>
		public IEnumerable<IFolderSystem> ApplyFolderSystemsOrder(IEnumerable<IFolderSystem> folderSystems)
		{
			folderSystems = _defaultConfig.ApplyFolderSystemsOrder(folderSystems);
			return _userConfig.ApplyFolderSystemsOrder(folderSystems);
		}

		/// <summary>
		/// Customizes the folders in the specified folder system according to the default and [optionally] user specific settings,
		/// returning the folders in the order specified by customization.
		/// </summary>
		/// <param name="folderSystem"></param>
		/// <param name="includeUserCustomizations"></param>
		public IEnumerable<IFolder> ApplyFolderCustomizations(IFolderSystem folderSystem, bool includeUserCustomizations)
		{
			var folders = _defaultConfig.ApplyFolderCustomizations(folderSystem.Id, folderSystem.Folders);
			return includeUserCustomizations
				? _userConfig.ApplyFolderCustomizations(folderSystem.Id, folders)
				: folders;
		}

		/// <summary>
		/// Customizes the folders in the specified folder system according to the default and user specific settings,
		/// returning the folders in the order specified by customization.
		/// </summary>
		/// <param name="folderSystem"></param>
		public IEnumerable<IFolder> ApplyFolderCustomizations(IFolderSystem folderSystem)
		{
			return ApplyFolderCustomizations(folderSystem, true);
		}

		/// <summary>
		/// Customizes the specified folder, in the specified folder system, according to the default and user specific setting.
		/// </summary>
		/// <param name="folderSystem"></param>
		/// <param name="folder"></param>
		public void ApplyFolderCustomizations(IFolderSystem folderSystem, IFolder folder)
		{
			_defaultConfig.ApplyFolderCustomizations(folderSystem.Id, folder);
			_userConfig.ApplyFolderCustomizations(folderSystem.Id, folder);
		}

		public delegate void UpdateFolderExplorerUserConfigurationAction(IFolderExplorerUserConfigurationUpdater userConfiguration);

		/// <summary>
		/// Allows user configuration to be stored.  The update action should be used to invoke methods on the 
		/// <see cref="IFolderExplorerUserConfigurationUpdater"/> to set the required user configuration.
		/// </summary>
		/// <param name="updateAction"></param>
		public void UpdateUserConfiguration(UpdateFolderExplorerUserConfigurationAction updateAction)
		{
			_userConfig.BeginTransaction();

			try
			{
				updateAction(_userConfig);
				_userConfig.CommitTransaction();
			}
			catch
			{
				_userConfig.RollbackTransaction();
				throw;
			}
		}

		/// <summary>
		/// Indicates that the current user's folder/folder system customizations have been committed.
		/// </summary>
		public event EventHandler UserConfigurationSaved
		{
			add { _userConfig.ChangesCommitted += value; }
			remove { _userConfig.ChangesCommitted -= value; }
		}

		public bool IsFolderSystemReadOnly(IFolderSystem folderSystem)
		{
			return _defaultConfig.IsFolderSystemReadOnly(folderSystem);
		}

		#endregion
	}
}

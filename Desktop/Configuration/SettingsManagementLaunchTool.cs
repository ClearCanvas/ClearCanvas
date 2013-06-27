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
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.Configuration
{
	/// <summary>
	/// Launches the <see cref="SettingsManagementComponent"/>.
	/// </summary>
	[MenuAction("activate", "global-menus/MenuTools/MenuUtilities/MenuConfigureSettings", "Activate")]
	[ActionPermission("activate", AuthorityTokens.Desktop.SettingsManagement)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class SettingsManagementLaunchTool : Tool<IDesktopToolContext>
	{
		#region NullSettingsStore

		class NullSettingsStore : ISettingsStore
		{
			public bool IsOnline
			{
				get { return false; }
			}

			public bool SupportsImport
			{
				get { return false; }
			}

			public IList<SettingsGroupDescriptor> ListSettingsGroups()
			{
				return new List<SettingsGroupDescriptor>();
			}

			public SettingsGroupDescriptor GetPreviousSettingsGroup(SettingsGroupDescriptor @group)
			{
				return @group;
			}

			public IList<SettingsPropertyDescriptor> ListSettingsProperties(SettingsGroupDescriptor @group)
			{
				return new List<SettingsPropertyDescriptor>();
			}

			public void ImportSettingsGroup(SettingsGroupDescriptor @group, List<SettingsPropertyDescriptor> properties)
			{
			}

			public Dictionary<string, string> GetSettingsValues(SettingsGroupDescriptor @group, string user, string instanceKey)
			{
				return new Dictionary<string, string>();
			}

			public void PutSettingsValues(SettingsGroupDescriptor @group, string user, string instanceKey, Dictionary<string, string> dirtyValues)
			{
			}

			public void RemoveUserSettings(SettingsGroupDescriptor @group, string user, string instanceKey)
			{
			}
		}

		#endregion

		private IWorkspace _workspace;

		/// <summary>
		/// Constructor.
		/// </summary>
		public SettingsManagementLaunchTool()
		{
		}

		/// <summary>
		/// Launches the <see cref="SettingsManagementComponent"/> or activates it if it's already open.
		/// </summary>
		/// <remarks>
		/// This method first looks for a valid extension of <see cref="SettingsStoreExtensionPoint"/> and
		/// with which to initialize the <see cref="SettingsManagementComponent"/>.  If one is not found,
		/// an instance of <see cref="LocalSettingsStore"/> is instantiated and passed to the
		/// <see cref="SettingsManagementComponent"/>.  The <see cref="LocalSettingsStore"/> allows
		/// the local application settings to be modified, where by default they cannot be.
		/// </remarks>
		public void Activate()
		{
			if (_workspace != null)
			{
				_workspace.Activate();
				return;
			}

			ISettingsStore store;
			try
			{
				store = SettingsStore.Create();
                //If there is a store, and it is not online, then settings can't be edited.
                if (!store.IsOnline)
                {
                    Context.DesktopWindow.ShowMessageBox(SR.MessageSettingsStoreOffline, MessageBoxActions.Ok);
                    return;
                }
            }
			catch (NotSupportedException)
			{
				// There is no central settings store; all settings will be treated as though they were local.
				store = new NullSettingsStore();
			}

			_workspace = ApplicationComponent.LaunchAsWorkspace(
				this.Context.DesktopWindow,
				new SettingsManagementComponent(store),
				SR.TitleSettingsEditor,
				"Settings Management");

			_workspace.Closed += OnWorkspaceClosed;
		}

		private void OnWorkspaceClosed(object sender, ClosedEventArgs e)
		{
			_workspace = null;
		}

		protected override void Dispose(bool disposing)
		{
			if (_workspace != null)
				_workspace.Closed -= OnWorkspaceClosed;

			base.Dispose(disposing);
		}
	}
}

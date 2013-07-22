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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// A tool for launching a home page.  A home page consists of a set of folder systems and a preview component.
	/// </summary>
	/// <remarks>
	/// Subclasses of this class should specify a <see cref="MenuActionAttribute"/> attribute with the Launch method as the clickHandler
	/// </remarks>
	public abstract class HomeTool : Tool<IDesktopToolContext>
	{
		protected virtual IWorkspace Workspace { get; set; }

		/// <summary>
		/// Title displayed when the home page is active
		/// </summary>
		public abstract string Title { get; }

		/// <summary>
		/// Creates the homepage component.
		/// </summary>
		/// <returns></returns>
		protected abstract IApplicationComponent CreateComponent();

		/// <summary>
		/// Determines if the workspace that is launched is user-closable or not.
		/// </summary>
		protected virtual bool IsUserClosableWorkspace
		{
			get { return true; }
		}

		/// <summary>
		/// Default clickHandler implementation for <see cref="MenuAction"/> and/or <see cref="ButtonAction"/> attributes.
		/// These attributes must be specified on subclasses.
		/// </summary>
		public void Launch()
		{
			if (Workspace == null)
			{
				Open();
			}
			else
			{
				Workspace.Activate();
			}
		}

		/// <summary>
		/// Re-starts the homepage, close any existing open homepage.
		/// </summary>
		protected void Restart()
		{
			if (Workspace != null)
			{
				Workspace.Close();
				Workspace = null;
			}

			Open();
		}

		private void Open()
		{
			try
			{
				IApplicationComponent component = CreateComponent();

				if (component != null)
				{
					WorkspaceCreationArgs args = new WorkspaceCreationArgs(component, this.Title, null);
					args.UserClosable = this.IsUserClosableWorkspace;
					Workspace = ApplicationComponent.LaunchAsWorkspace(this.Context.DesktopWindow, args);
					Workspace.Closed += delegate
						{
							Workspace = null;
							FolderExplorerComponentSettings.Default.UserConfigurationSaved -= OnUserConfigurationSaved;
						};

					FolderExplorerComponentSettings.Default.UserConfigurationSaved += OnUserConfigurationSaved;
				}
			}
			catch (Exception e)
			{
				// could not launch component
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		private void OnUserConfigurationSaved(object sender, EventArgs e)
		{
			this.Restart();
		}
	}

	/// <summary>
	/// A tool for launching a home page with a <see cref="WorklistItemPreviewComponent"/> as the preview component
	/// </summary>
	/// <typeparam name="TFolderSystemExtensionPoint">Specifies the extension point used to create the set of folder systems</typeparam>
	public abstract class WorklistPreviewHomeTool<TFolderSystemExtensionPoint> : HomeTool
		where TFolderSystemExtensionPoint : ExtensionPoint<IFolderSystem>, new()
	{
		protected override IApplicationComponent  CreateComponent()
		{
			List<IFolderSystem> folderSystems = GetFolderSystems();
			if (folderSystems.Count == 0)
				return null;

			// Find all the folder systems
			WorklistItemPreviewComponent previewComponent = new WorklistItemPreviewComponent();
			return new HomePageContainer(folderSystems, previewComponent);
		}

		protected bool HasFolderSystems
		{
			get { return GetFolderSystems().Count > 0; }
		}

		private List<IFolderSystem> GetFolderSystems()
		{
			return CollectionUtils.Cast<IFolderSystem>(new TFolderSystemExtensionPoint().CreateExtensions());
		}
	}
}

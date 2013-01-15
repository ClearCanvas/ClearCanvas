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

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Represents the collection of <see cref="WorkspaceDialogBox"/> objects for a given workspace.
	/// </summary>
	internal class WorkspaceDialogBoxCollection : DesktopObjectCollection<WorkspaceDialogBox>
	{
		private readonly Workspace _owner;

		/// <summary>
		/// Constructor.
		/// </summary>
		internal WorkspaceDialogBoxCollection(Workspace owner)
		{
			_owner = owner;
		}

		/// <summary>
		/// Creates a new dialog box with the specified arguments.
		/// </summary>
		internal WorkspaceDialogBox AddNew(DialogBoxCreationArgs args)
		{
			var dialogBox = CreateDialogBox(args);
			Open(dialogBox);
			return dialogBox;
		}

		/// <summary>
		/// Creates a new <see cref="WorkspaceDialogBox"/>.
		/// </summary>
		private WorkspaceDialogBox CreateDialogBox(DialogBoxCreationArgs args)
		{
			var factory = CollectionUtils.FirstElement<IWorkspaceDialogBoxFactory>(
				(new WorkspaceDialogBoxFactoryExtensionPoint()).CreateExtensions()) ?? new DefaultWorkspaceDialogBoxFactory();

			return factory.CreateWorkspaceDialogBox(args, _owner);
		}
	}
}

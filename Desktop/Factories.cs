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

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Defines an extension point for providing a custom factory for creating instances of <see cref="DesktopWindow"/>.
	/// </summary>
	/// <remarks>
	/// Provide an extension to this point if you have subclassed <see cref="DesktopWindow"/> and you want to be 
	/// able to provide instances of your subclass to the framework when it requests creation of a new desktop window.
	/// </remarks>
	[ExtensionPoint]
	public sealed class DesktopWindowFactoryExtensionPoint : ExtensionPoint<IDesktopWindowFactory>
	{
	}

	/// <summary>
	/// Defines the interface to extensions of <see cref="DesktopWindowFactoryExtensionPoint"/>.
	/// </summary>
	public interface IDesktopWindowFactory
	{
		/// <summary>
		/// Creates a new desktop window for the specified arguments.
		/// </summary>
		/// <param name="args">Arguments that control the creation of the desktop window.</param>
		/// <param name="application">The application with which the window is associated.</param>
		/// <returns>A new desktop window instance.</returns>
		DesktopWindow CreateWindow(DesktopWindowCreationArgs args, Application application);
	}

	/// <summary>
	/// Default desktop window factory used when no extensions are provided.
	/// </summary>
	internal class DefaultDesktopWindowFactory : IDesktopWindowFactory
	{
		#region IDesktopWindowFactory Members

		public DesktopWindow CreateWindow(DesktopWindowCreationArgs args, Application application)
		{
			return new DesktopWindow(args, application);
		}

		#endregion
	}

	/// <summary>
	/// Defines an extension point for providing a custom factory for creating instances of <see cref="Workspace"/>.
	/// </summary>
	/// <remarks>
	/// Provide an extension to this point if you have subclassed <see cref="Workspace"/> and you want to be 
	/// able to provide instances of your subclass to the framework when it requests creation of a new workspace.
	/// </remarks>
	[ExtensionPoint]
	public sealed class WorkspaceFactoryExtensionPoint : ExtensionPoint<IWorkspaceFactory>
	{
	}

	/// <summary>
	/// Defines the interface to extensions of <see cref="WorkspaceFactoryExtensionPoint"/>.
	/// </summary>
	public interface IWorkspaceFactory
	{
		/// <summary>
		/// Creates a new workspace for the specified arguments.
		/// </summary>
		/// <param name="args">Arguments that control the creation of the workspace.</param>
		/// <param name="window">The desktop window with which the workspace is associated.</param>
		/// <returns>A new workspace instance.</returns>
		Workspace CreateWorkspace(WorkspaceCreationArgs args, DesktopWindow window);
	}

	/// <summary>
	/// Default workspace factory used when no extensions are provided.
	/// </summary>
	internal class DefaultWorkspaceFactory : IWorkspaceFactory
	{
		#region IWorkspaceFactory Members

		public Workspace CreateWorkspace(WorkspaceCreationArgs args, DesktopWindow window)
		{
			return new Workspace(args, window);
		}

		#endregion
	}

	/// <summary>
	/// Defines an extension point for providing a custom factory for creating instances of <see cref="Shelf"/>.
	/// </summary>
	/// <remarks>
	/// Provide an extension to this point if you have subclassed <see cref="Shelf"/> and you want to be 
	/// able to provide instances of your subclass to the framework when it requests creation of a new shelf.
	/// </remarks>
	[ExtensionPoint]
	public sealed class ShelfFactoryExtensionPoint : ExtensionPoint<IShelfFactory>
	{
	}

	/// <summary>
	/// Defines the interface to extensions of <see cref="ShelfFactoryExtensionPoint"/>.
	/// </summary>
	public interface IShelfFactory
	{
		/// <summary>
		/// Creates a new shelf for the specified arguments.
		/// </summary>
		/// <param name="args">Arguments that control the creation of the shelf.</param>
		/// <param name="window">The desktop window with which the shelf is associated.</param>
		/// <returns>A new shelf instance.</returns>
		Shelf CreateShelf(ShelfCreationArgs args, DesktopWindow window);
	}

	/// <summary>
	/// Default shelf factory used when no extensions are provided.
	/// </summary>
	internal class DefaultShelfFactory : IShelfFactory
	{
		#region IShelfFactory Members

		public Shelf CreateShelf(ShelfCreationArgs args, DesktopWindow window)
		{
			return new Shelf(args, window);
		}

		#endregion
	}

	/// <summary>
	/// Defines an extension point for providing a custom factory for creating instances of <see cref="DialogBox"/>.
	/// </summary>
	/// <remarks>
	/// Provide an extension to this point if you have subclassed <see cref="DialogBox"/> and you want to be 
	/// able to provide instances of your subclass to the framework when it requests creation of a new dialog box.
	/// </remarks>
	[ExtensionPoint]
	public sealed class DialogBoxFactoryExtensionPoint : ExtensionPoint<IDialogBoxFactory>
	{
	}

	/// <summary>
	/// Defines the interface to extensions of <see cref="DialogBoxFactoryExtensionPoint"/>.
	/// </summary>
	public interface IDialogBoxFactory
	{
		/// <summary>
		/// Creates a new dialog box for the specified arguments.
		/// </summary>
		/// <param name="args">Arguments that control the creation of the dialog.</param>
		/// <param name="window">The desktop window with which the dialog is associated.</param>
		/// <returns>A new dialog instance.</returns>
		DialogBox CreateDialogBox(DialogBoxCreationArgs args, DesktopWindow window);
	}

	/// <summary>
	/// Default dialog factory used when no extensions are provided.
	/// </summary>
	internal class DefaultDialogBoxFactory : IDialogBoxFactory
	{
		#region IDialogBoxFactory Members

		public DialogBox CreateDialogBox(DialogBoxCreationArgs args, DesktopWindow window)
		{
			return new DialogBox(args, window);
		}

		#endregion
	}

	/// <summary>
	/// Defines an extension point for providing a custom factory for creating instances of <see cref="WorkspaceDialogBox"/>.
	/// </summary>
	/// <remarks>
	/// Provide an extension to this point if you have subclassed <see cref="WorkspaceDialogBox"/> and you want to be 
	/// able to provide instances of your subclass to the framework when it requests creation of a new workspace dialog.
	/// </remarks>
	[ExtensionPoint]
	public sealed class WorkspaceDialogBoxFactoryExtensionPoint : ExtensionPoint<IWorkspaceDialogBoxFactory>
	{
	}

	/// <summary>
	/// Defines the interface to extensions of <see cref="WorkspaceDialogBoxFactoryExtensionPoint"/>.
	/// </summary>
	public interface IWorkspaceDialogBoxFactory
	{
		/// <summary>
		/// Creates a new dialog box for the specified arguments.
		/// </summary>
		/// <param name="args">Arguments that control the creation of the dialog.</param>
		/// <param name="workspace">The workspace with which the dialog is associated.</param>
		/// <returns>A new dialog instance.</returns>
		WorkspaceDialogBox CreateWorkspaceDialogBox(DialogBoxCreationArgs args, Workspace workspace);
	}

	/// <summary>
	/// Default dialog factory used when no extensions are provided.
	/// </summary>
	internal class DefaultWorkspaceDialogBoxFactory : IWorkspaceDialogBoxFactory
	{
		#region IDialogBoxFactory Members

		/// <summary>
		/// Creates a new dialog box for the specified arguments.
		/// </summary>
		/// <param name="args">Arguments that control the creation of the dialog.</param>
		/// <param name="workspace">The workspace with which the dialog is associated.</param>
		/// <returns>A new dialog instance.</returns>
		public WorkspaceDialogBox CreateWorkspaceDialogBox(DialogBoxCreationArgs args, Workspace workspace)
		{
			return new WorkspaceDialogBox(args, workspace);
		}

		#endregion
	}

}

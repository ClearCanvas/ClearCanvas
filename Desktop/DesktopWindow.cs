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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Utilities.Manifest;

namespace ClearCanvas.Desktop
{
	#region Extension points

	/// <summary>
	/// Defines an extension point for desktop tools, which are instantiated once per desktop window.
	/// </summary>
	/// <remarks>
	/// Desktop tools are owned by a desktop window. A desktop tool is instantiated once per desktop window.
	/// Extensions should expect to recieve a tool context of type <see cref="IDesktopToolContext"/>.
	/// </remarks>
	[ExtensionPoint()]
	public sealed class DesktopToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	/// <summary>
	/// Tool context interface provided to tools that extend <see cref="DesktopToolExtensionPoint"/>.
	/// </summary>
	public interface IDesktopToolContext : IToolContext
	{
		/// <summary>
		/// Gets the desktop window that the tool is associated with.
		/// </summary>
		DesktopWindow DesktopWindow { get; }
	}

	#endregion

	/// <summary>
	/// Represents a desktop window (an application main window).
	/// </summary>
	public class DesktopWindow : DesktopObject, IDesktopWindow
	{
		#region DesktopToolContext

		private class DesktopToolContext : ToolContext, IDesktopToolContext
		{
			private DesktopWindow _window;

			internal DesktopToolContext(DesktopWindow window)
			{
				_window = window;
			}

			public DesktopWindow DesktopWindow
			{
				get { return _window; }
			}
		}

		#endregion

		#region AlertContext

		class AlertContext : IDesktopAlertContext
		{
			private readonly DesktopWindow _window;

			public AlertContext(DesktopWindow window)
			{
				_window = window;
			}

			public void ShowAlertViewer()
			{
				_window.DesktopTools.Find<AlertViewerTool>().Show();
			}

			public void AcknowledgeAll()
			{
				AlertLog.Instance.AcknowledgeAll();
			}

			public IconSet GetIcon(AlertLevel level)
			{
				return level.GetIcon();
			}

			public int UnacknowledgedErrorWarningCount
			{
				get
				{
					// count the number of un-acknowledged errors/warnings
					return (from e in AlertLog.Instance.Entries
							where e.Level != AlertLevel.Info && !e.Acknowledged
							select e).Count();
				}
			}
		}

		#endregion

		/// <summary>
		/// Defines the global menu action site.
		/// </summary>
		protected internal const string GlobalMenus = "global-menus";

		/// <summary>
		/// Defines the global toolbar action site.
		/// </summary>
		protected internal const string GlobalToolbars = "global-toolbars";

		private readonly Application _application;
		private WorkspaceCollection _workspaces;
		private ShelfCollection _shelves;
		private DialogBoxCollection _dialogs;
		private readonly string _baseTitle;

		private IToolSet _desktopTools;
		private readonly string _menuActionSite;
		private readonly string _toolbarActionSite;

		private ActionModelNode _menuModel;
		private ActionModelNode _toolbarModel;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected internal DesktopWindow(DesktopWindowCreationArgs args, Application application)
			: base(args)
		{
			_application = application;
			_workspaces = new WorkspaceCollection(this);
			_shelves = new ShelfCollection(this);
			_dialogs = new DialogBoxCollection(this);

			// if no title supplied, create a default title
			_baseTitle = !string.IsNullOrEmpty(args.Title) ? args.Title : DefaultBaseTitle;

			_menuActionSite = args.MenuSite ?? GlobalMenus;
			_toolbarActionSite = args.ToolbarSite ?? GlobalToolbars;
		}

		#region Public properties

		/// <summary>
		/// Gets the collection of workspaces associated with this window.
		/// </summary>
		public WorkspaceCollection Workspaces
		{
			get { return _workspaces; }
		}

		/// <summary>
		/// Gets the currently active workspace, or null if there are no workspaces.
		/// </summary>
		public Workspace ActiveWorkspace
		{
			get { return _workspaces.ActiveWorkspace; }
		}

		/// <summary>
		/// Gets the collection of shelves associated with this window.
		/// </summary>
		public ShelfCollection Shelves
		{
			get { return _shelves; }
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Shows a message box in front of this window.
		/// </summary>
		/// <param name="message">The message to show in the message box.</param>
		/// <param name="buttons">The buttons to display in the message box.</param>
		/// <returns>The button the user selected to dismiss the message box.</returns>
		public DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons)
		{
			return this.DesktopWindowView.ShowMessageBox(message, null, buttons);
		}

		/// <summary>
		/// Shows a message box in front of this window.
		/// </summary>
		/// <param name="message">The message to show in the message box.</param>
		/// <param name="title">The title of the message box.</param>
		/// <param name="buttons">The buttons to display in the message box.</param>
		/// <returns>The button the user selected to dismiss the message box.</returns>
		public DialogBoxAction ShowMessageBox(string message, string title, MessageBoxActions buttons)
		{
			AssertState(new[] { DesktopObjectState.Open, DesktopObjectState.Closing });

			return this.DesktopWindowView.ShowMessageBox(message, title, buttons);
		}

		/// <summary>
		/// Shows a desktop alert.
		/// </summary>
		/// <param name="level">The alert level.</param>
		/// <param name="message">The message to display.</param>
		public void ShowAlert(AlertLevel level, string message)
		{
			var args = new AlertNotificationArgs(level, message);
			ShowAlert(args);
		}

		/// <summary>
		/// Shows a desktop alert.
		/// </summary>
		/// <param name="level">The alert level.</param>
		/// <param name="message">The message to display.</param>
		/// <param name="linkText">The link text to display.</param>
		/// <param name="linkAction">The link action to display.</param>
		/// <param name="dismissOnLinkClicked"> </param>
		public void ShowAlert(AlertLevel level, string message, string linkText, Action<DesktopWindow> linkAction, bool dismissOnLinkClicked)
		{
			var args = new AlertNotificationArgs(level, message)
			{
				LinkText = linkText,
				LinkAction = linkAction,
				DismissOnLinkClicked = dismissOnLinkClicked
			};
			ShowAlert(args);
		}

		/// <summary>
		/// Shows a desktop alert.
		/// </summary>
		/// <param name="args"></param>
		public void ShowAlert(AlertNotificationArgs args)
		{
			AssertState(new[] { DesktopObjectState.Open, DesktopObjectState.Closing });

			// log new alert
			AlertLog.Instance.Log(args);

			this.DesktopWindowView.ShowAlert(args);
		}

		/// <summary>
		/// Shows a dialog box in front of this window.
		/// </summary>
		/// <param name="args">Creation helper object.</param>
		/// <returns>The button the user selected to dismiss the dialog.</returns>
		public DialogBoxAction ShowDialogBox(DialogBoxCreationArgs args)
		{
			AssertState(new[] { DesktopObjectState.Open, DesktopObjectState.Closing });

			DialogBox dialog = _dialogs.AddNew(args);
			return dialog.RunModal();
		}

		/// <summary>
		/// Shows a 'Save File' common dialog.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public FileDialogResult ShowSaveFileDialogBox(FileDialogCreationArgs args)
		{
			AssertState(new[] { DesktopObjectState.Open, DesktopObjectState.Closing });

			return this.DesktopWindowView.ShowSaveFileDialogBox(args);
		}

		/// <summary>
		/// Shows an 'Open File' common dialog.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public FileDialogResult ShowOpenFileDialogBox(FileDialogCreationArgs args)
		{
			AssertState(new[] { DesktopObjectState.Open, DesktopObjectState.Closing });

			return this.DesktopWindowView.ShowOpenFileDialogBox(args);
		}

		/// <summary>
		/// Shows a 'Select Folder' common dialog.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public FileDialogResult ShowSelectFolderDialogBox(SelectFolderDialogCreationArgs args)
		{
			AssertState(new[] { DesktopObjectState.Open, DesktopObjectState.Closing });

			return this.DesktopWindowView.ShowSelectFolderDialogBox(args);
		}

		/// <summary>
		/// Shows a dialog box in front of this window.
		/// </summary>
		/// <param name="component">The application component to be hosted in the dialog.</param>
		/// <param name="title">The dialog's title.</param>
		/// <returns>The button the user selected to dismiss the dialog.</returns>
		public DialogBoxAction ShowDialogBox(IApplicationComponent component, string title)
		{
			return ShowDialogBox(new DialogBoxCreationArgs(component, title, null));
		}

		#endregion

		#region Protected overridables

		/// <summary>
		/// Creates the desktop tools (via <see cref="DesktopToolExtensionPoint"/>) and initializes
		/// the view.
		/// </summary>
		protected override void OnOpened(EventArgs args)
		{
			// note that we can't do this initialize in the Initialize override because the view has not been created yet
			// initialization of desktop tools must occur after the desktop view has been created
			_desktopTools = new ToolSet(new DesktopToolExtensionPoint(), new DesktopToolContext(this));

			// initialize menu and toolbar models
			UpdateView();

			// when the active workspace changes, assume that the menu/toolbar models have also changed
			_workspaces.ItemActivationChanged += delegate(object sender, ItemEventArgs<Workspace> e)
			{
				UpdateView();
				UpdateTitleChangedEventSubscription(e.Item);
			};

			base.OnOpened(args);
		}

		/// <summary>
		/// Checks if all workspaces can close without any user interaction.
		/// </summary>
		protected internal override bool CanClose()
		{
			// we can close if all workspaces can close without interacting
			return CollectionUtils.TrueForAll(_workspaces, w => w.CanClose());
		}

		/// <summary>
		/// Attempts to close all workspaces and shelves.
		/// </summary>
		/// <param name="reason">The reason for closing all workspaces and shelves.</param>
		/// <returns>Whether or not all affected objects were closed successfully.</returns>
		protected override bool PrepareClose(CloseReason reason)
		{
			var workspaces = new List<Workspace>(_workspaces);
			foreach (var workspace in workspaces)
			{
				// if the workspace is still open, try to close it
				// (the check is necessary because there is no guarantee that the workspace is still open)
				if (workspace.State == DesktopObjectState.Open &&
					!workspace.Close(UserInteraction.Allowed, reason | CloseReason.ParentClosing))
					return false;
			}

			var shelves = new List<Shelf>(_shelves);
			foreach (var shelf in shelves)
			{
				// if the shelf is still open, close it
				// (the check is necessary because there is no guarantee that the shelf is still open)
				// if the shelf fails to close, we don't care
				// (shelves are not granted the same power as workspaces to block the desktop window from closing)
				if (shelf.State == DesktopObjectState.Open)
					shelf.Close(UserInteraction.Allowed, reason | CloseReason.ParentClosing);

			}

			return true;
		}

		/// <summary>
		/// Disposes of this object, its toos, dialogs, shelves and workspaces.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				if (_desktopTools != null)
					_desktopTools.Dispose();

				if (_dialogs != null)
					(_dialogs as IDisposable).Dispose();

				if (_workspaces != null)
					(_workspaces as IDisposable).Dispose();

				if (_shelves != null)
					(_shelves as IDisposable).Dispose();

				// These types of objects (particularly tools) can subscribe to each other's events
				// so we set everything to null at the end to allow objects to unsubscribe on disposal.
				_desktopTools = null;
				_dialogs = null;
				_workspaces = null;
				_shelves = null;
			}
		}

		/// <summary>
		/// Creates the title that is displayed in the title bar.
		/// </summary>
		/// <remarks>
		/// Override this method to customize the title. 
		/// </remarks>
		/// <param name="baseTitle">The base title for the window.</param>
		/// <param name="activeWorkspace">The active workspace, from which additional text will be added to the title.</param>
		/// <returns>The title for the window.</returns>
		protected virtual string MakeTitle(string baseTitle, Workspace activeWorkspace)
		{
			return activeWorkspace != null ? string.Format(SR.FormatDesktopWindowTitle, activeWorkspace.Title, baseTitle) : baseTitle;
		}

		/// <summary>
		/// Creates a view for this object.
		/// </summary>
		/// <returns></returns>
		protected sealed override IDesktopObjectView CreateView()
		{
			var view = _application.CreateDesktopWindowView(this);
			view.SetAlertContext(new AlertContext(this));
			return view;
		}

		/// <summary>
		/// Called when the current application UI culture has changed.
		/// </summary>
		protected override void OnCurrentUICultureChanged()
		{
			base.OnCurrentUICultureChanged();

			UpdateView();
		}

		#endregion

		#region Title

		internal static readonly string LabelModifiedInstallation = "Modified Installation";

		private static string DefaultBaseTitle
		{
			get
			{
				//TODO (CR February 2011) - Low: Not for diagnostic use is a resource, but modified installation isn't?
				var tags = new List<string>();

    		    var text = ProductStateInfo.GetTitlebarText();
                if (!string.IsNullOrEmpty(text))
                {
                    tags.Add(text);
                }

				//TODO (CR February 2011) - High: We should have left this as a property on ProductInformation rather than checking for empty string.
				if (ProductInformation.Release != string.Empty)
					tags.Add(SR.LabelNotForDiagnosticUse);
				else if (new System.Text.RegularExpressions.Regex(@"\bvet(?:|team|erinary|station)\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase).IsMatch(ProductInformation.Edition))
					tags.Add(SR.LabelNotForHumanDiagnosis);
				if (!ManifestVerification.Valid)
					// should be hardcoded because manifest verification is all that prevents localizing this tag away
					tags.Add(LabelModifiedInstallation);

				var name = ProductInformation.GetName(false, true);
				var oemConfig = OemConfiguration.Load();
				if (!string.IsNullOrEmpty(oemConfig.ProductName))
					name = oemConfig.ProductName;
				if (tags.Count == 0)
					return name;

				var tagString = string.Join(" | ", tags.ToArray());
				return string.IsNullOrEmpty(name) ? tagString : string.Format("{0} - {1}", name, tagString);
			}
		}

		protected void UpdateTitle()
		{
			this.Title = MakeTitle(_baseTitle, _workspaces.ActiveWorkspace);
		}

		/// <summary>
		/// Subscribes to or unsubscribes from TitleChanged event for the specified <see cref="Workspace"/> depending on whether it is active or not
		/// </summary>
		/// <param name="workspace"></param>
		private void UpdateTitleChangedEventSubscription(Workspace workspace)
		{
			if (workspace.Active)
			{
				workspace.TitleChanged += UpdateTitle;
			}
			else
			{
				workspace.TitleChanged -= UpdateTitle;
			}
		}

		/// <summary>
		/// Updates the view's title
		/// </summary>
		private void UpdateTitle(object sender, EventArgs e)
		{
			UpdateTitle();
		}

		#endregion

		#region Helpers

		//TODO (CR February 2011) - High: maybe put these in ProductInformation so we can improve the consistency.

		/// <summary>
		/// Creates a workspace view for the specified workspace.
		/// </summary>
		internal IWorkspaceView CreateWorkspaceView(Workspace workspace)
		{
			return this.DesktopWindowView.CreateWorkspaceView(workspace);
		}

		/// <summary>
		/// Creates a shelf view for the specified shelf.
		/// </summary>
		internal IShelfView CreateShelfView(Shelf shelf)
		{
			return this.DesktopWindowView.CreateShelfView(shelf);
		}

		/// <summary>
		/// Creates a dialog box view for the specified dialog box.
		/// </summary>
		internal IDialogBoxView CreateDialogView(DialogBox dialog)
		{
			return this.DesktopWindowView.CreateDialogBoxView(dialog);
		}

		/// <summary>
		/// Gets the view for this object as an <see cref="IDesktopWindowView"/>.
		/// </summary>
		protected IDesktopWindowView DesktopWindowView
		{
			get { return (IDesktopWindowView)this.View; }
		}

		/// <summary>
		/// Gets the tool set associated with this desktop window.
		/// </summary>
		protected internal IToolSet DesktopTools
		{
			get { return _desktopTools; }
		}

		/// <summary>
		/// Gets the current menu model.
		/// </summary>
		protected internal ActionModelNode MenuModel
		{
			get { return _menuModel; }
		}

		/// <summary>
		/// Gets the current toolbar model.
		/// </summary>
		protected internal ActionModelNode ToolbarModel
		{
			get { return _toolbarModel; }
		}

		/// <summary>
		/// Updates the view's title, menu and toolbars.
		/// </summary>
		internal void UpdateView()
		{
			if (this.DesktopWindowView != null)
			{
				this.Title = MakeTitle(_baseTitle, _workspaces.ActiveWorkspace);

				_menuModel = BuildActionModel(_menuActionSite);
				_toolbarModel = BuildActionModel(_toolbarActionSite);

				this.DesktopWindowView.SetMenuModel(_menuModel);
				this.DesktopWindowView.SetToolbarModel(_toolbarModel);
			}
		}

		/// <summary>
		/// Builds the action model for the specified site.
		/// </summary>
		private ActionModelNode BuildActionModel(string site)
		{
			string @namespace = typeof(DesktopWindow).FullName;

			IActionSet actions = this.DesktopTools.Actions;
			if (this.ActiveWorkspace != null)
			{
				actions = actions.Union(this.Workspaces.ActiveWorkspace.Actions);
				if (this.ActiveWorkspace.Component is IApplicationComponent)
				{
					@namespace = ((IApplicationComponent)this.ActiveWorkspace.Component).GlobalActionsNamespace;
				}
			}

			return ActionModelRoot.CreateModel(@namespace, site, actions);
		}

		#endregion
	}
}

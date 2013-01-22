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

#region Additional permission to link with DotNetMagic

// Additional permission under GNU GPL version 3 section 7
// 
// If you modify this Program, or any covered work, by linking or combining it
// with DotNetMagic (or a modified version of that library), containing parts
// covered by the terms of the Crownwood Software DotNetMagic license, the
// licensors of this Program grant you additional permission to convey the
// resulting work.

#endregion


using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using Crownwood.DotNetMagic.Controls;
using Crownwood.DotNetMagic.Docking;
using System.IO;
using ClearCanvas.Common.Utilities;

using WinFormsScreen = System.Windows.Forms.Screen;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// WinForms implementation of <see cref="IDesktopWindowView"/>. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class may subclassed if customization is desired.  In this case, the <see cref="ApplicationView"/>
    /// class must also be subclassed in order to instantiate the subclass from 
    /// its <see cref="ApplicationView.CreateDesktopWindowView"/> method.
    /// </para>
    /// <para>
    /// Reasons for subclassing may include: overriding the <see cref="CreateDesktopForm"/> factory method to supply
    /// a custom subclass of the <see cref="DesktopForm"/> class, overriding the <see cref="CreateWorkspaceView"/>,
    /// <see cref="CreateShelfView"/>,
    /// or <see cref="CreateDialogBoxView"/> factory methods to supply custom subclasses of these view classes, overriding
    /// <see cref="SetMenuModel"/> or <see cref="SetToolbarModel"/> to customize the menu/toolbar display,
    /// and overriding <see cref="ShowMessageBox"/> to customize the display of message boxes.
    /// </para>
    /// </remarks>
    public class DesktopWindowView : DesktopObjectView, IDesktopWindowView
    {
        private static readonly OrderedSet<DesktopWindowView> _desktopWindowActivationOrder = new OrderedSet<DesktopWindowView>();

    	private readonly DesktopWindow _desktopWindow;
        private DesktopForm _form;
        private readonly OrderedSet<WorkspaceView> _workspaceActivationOrder;

    	private IDesktopAlertContext _alertContext;
    	private AlertNotificationForm _errorNotificationDialog;
		private AlertNotificationForm _infoNotificationDialog;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="window"></param>
        protected internal DesktopWindowView(DesktopWindow window)
        {
        	_desktopWindow = window;
            _form = CreateDesktopForm();
            _workspaceActivationOrder = new OrderedSet<WorkspaceView>();

            // listen to some events on the form
            _form.VisibleChanged += FormVisibleChangedEventHandler;
            _form.Activated += FormActivatedEventHandler;
            _form.Deactivate += FormDeactivateEventHandler;
            _form.FormClosing += FormFormClosingEventHandler;
            _form.TabbedGroups.PageCloseRequest += TabbedGroupPageClosePressedEventHandler;
            _form.TabbedGroups.PageChanged += TabbedGroupPageChangedEventHandler;

            // NY: We subscribe to ContentHiding instead of ContentHidden because ContentHidden
            // is fired when the user double clicks the caption bar of a docking window, which
            // results in a crash. (Ticket #144)
            _form.DockingManager.ContentHiding += DockingManagerContentHidingEventHandler;
            _form.DockingManager.ContentShown += DockingManagerContentShownEventHandler;
            _form.DockingManager.ContentAutoHideOpening += DockingManagerContentAutoHideOpeningEventHandler;
            _form.DockingManager.ContentAutoHideClosed += DockingManagerContentAutoHideClosedEventHandler;
            _form.DockingManager.WindowActivated += DockingManagerWindowActivatedEventHandler;
            _form.DockingManager.WindowDeactivated += FormDockingManagerWindowDeactivatedEventHandler;

			// init notification dialogs
			_infoNotificationDialog = new AlertNotificationForm(_form, Application.Name) {AutoDismiss = true};
			_infoNotificationDialog.OpenLogClicked += AlertDialogOpenLogClicked;
			_errorNotificationDialog = new AlertNotificationForm(_form, Application.Name);
			_errorNotificationDialog.OpenLogClicked += AlertDialogOpenLogClicked;
			_errorNotificationDialog.Dismissed += ErrorDialogDismissed;
       }

    	internal string DesktopWindowName
    	{
			get { return _desktopWindow.Name; }	
    	}

        #region Form Event Handlers

        /// <summary>
		/// Cancels the forms closing event, and raises our <see cref="IDesktopObjectView.CloseRequested"/> event instead.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormFormClosingEventHandler(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case System.Windows.Forms.CloseReason.ApplicationExitCall:
					//When there is a "fatal exception", we terminate the gui toolkit, which calls Application.Exit().
					//So, we can't cancel the close, otherwise the application can get into a funny state.
					return;
                case System.Windows.Forms.CloseReason.TaskManagerClosing:
                case System.Windows.Forms.CloseReason.WindowsShutDown:
                    // windows is trying close the application, not just this window
                    // rather than propagate the request to close this window, we need
                    // to ask the entire desktop to quit
                    Application.Quit();
                    break;
                case System.Windows.Forms.CloseReason.UserClosing:
                case System.Windows.Forms.CloseReason.None:

                    // notify the model that a close request was made
                    RaiseCloseRequested();
                    break;
                default:
                    // other close reasons are not applicable
                    break;
            }

			// cancel the request - don't let winforms close the form
			e.Cancel = true;
        }

        private void FormDeactivateEventHandler(object sender, EventArgs e)
        {
            // do nothing
            // note: if we are showing a modal dialog, the form gets de-activated, but we are still the active desktop window
            // therefore, this event is not really useful to us
        }

        /// <summary>
        /// Handles the forms Activated event in order to track the currently active window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormActivatedEventHandler(object sender, EventArgs e)
        {
            // de-activate the previous window before activating the new one
            var lastActive = _desktopWindowActivationOrder.LastElement;
            if (lastActive != this)
            {
                if (lastActive != null)
                {
                    lastActive.SetActiveStatus(false);
                }

                this.SetActiveStatus(true);
                _desktopWindowActivationOrder.Add(this);
            }
        }

        /// <summary>
        /// Handles the forms visible event in order to track our visible status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormVisibleChangedEventHandler(object sender, EventArgs e)
        {
            this.SetVisibleStatus(_form.Visible);
        }

        #endregion

        #region Workspace Management

        internal void AddWorkspaceView(WorkspaceView workspaceView)
        {
            // When we add a new workspace, we need to
            HideShelvesOnWorkspaceOpen();

            _form.TabbedGroups.ActiveLeaf.TabPages.Add(workspaceView.TabPage);
            workspaceView.TabPage.Selected = true;

            _form.TabbedGroups.DisplayTabMode = DisplayTabModes.ShowAll;
        }

        internal void RemoveWorkspaceView(WorkspaceView workspaceView)
        {
            // remove this page from the activation order
            _workspaceActivationOrder.Remove(workspaceView);

            // if the page being removed is the selected page and there are other workspaces,
            // select the last active one before removing this one
            if (workspaceView.TabPage.Selected && _workspaceActivationOrder.Count > 0)
            {
                _workspaceActivationOrder.LastElement.TabPage.Selected = true;
            }

            // Remove the tab
            TabPageCollection tabPages;
            var found = FindTabPageCollection(_form.TabbedGroups.RootSequence, workspaceView.TabPage, out tabPages);
            if (found)
                tabPages.Remove(workspaceView.TabPage);

            // notify that we are no longer visible
            workspaceView.SetVisibleStatus(false);

            // When there are no tabs left, turn off the tab control strip.
            // Done purely for aesthetic reasons.
            if (_form.TabbedGroups.ActiveLeaf.TabPages.Count == 0)
                _form.TabbedGroups.DisplayTabMode = DisplayTabModes.HideAll;

            _form.DockingManager.Container.Focus();
        }

        private bool FindTabPageCollection(
                TabGroupSequence nodeGroup,
                Crownwood.DotNetMagic.Controls.TabPage tabPage,
                out TabPageCollection containingCollection)
        {
            for (var i = 0; i < nodeGroup.Count; i++)
            {
                var node = nodeGroup[i];

                if (node.IsSequence)
                {
                    var found = FindTabPageCollection(node as TabGroupSequence, tabPage, out containingCollection);

                    if (found)
                        return true;
                }

                if (node.IsLeaf)
                {
                    var leaf = (TabGroupLeaf)node;
                    if (leaf.TabPages.Contains(tabPage))
                    {
                        containingCollection = leaf.TabPages;
                        return true;
                    }
                }
            }

            containingCollection = null;
            return false;
        }

        private void TabbedGroupPageClosePressedEventHandler(TabbedGroups groups, TGCloseRequestEventArgs e)
        {
            var wv = (WorkspaceView)e.TabPage.Tag;

            // We cancel so that DotNetMagic doesn't remove the tab; we want
            // to do that programatically
            e.Cancel = true;

            // raise close requested event
            wv.RaiseCloseRequested();
        }

        private void TabbedGroupPageChangedEventHandler(TabbedGroups tg, Crownwood.DotNetMagic.Controls.TabPage tp)
        {
            // de-activate the previous workspace before activating the new one
            var lastActive = _workspaceActivationOrder.LastElement;
            if (lastActive != null)
            {
                lastActive.SetActiveStatus(false);
            }

            // important to check tp != null to account for the case where the last workspace closes
            var nowActive = (tp != null) ? (WorkspaceView)tp.Tag : null;
            if (nowActive != null)
            {
                nowActive.SetVisibleStatus(true);   // the very first time the page is selected, need to change its visible status
                nowActive.SetActiveStatus(true);
                _workspaceActivationOrder.Add(nowActive);
            }
        }

        #endregion

        #region Shelf Management

        internal Content AddShelfView(ShelfView shelfView, Control control, string title, ShelfDisplayHint hint, MemoryStream shelfRestoreStream)
        {
        	// Forcing this makes the control resize *before* adding it to the DotNetMagic control, 
        	// so the shelf will be the correct size.  This would be done automatically when the
			// control gets added - we're just doing it a bit prematurely in order to get the correct size.
        	control.Font = _form.DockingManager.TabControlFont;
        	var displaySize = control.Size;

			var content = _form.DockingManager.Contents.Add(control, title);
			content.Tag = shelfView;

			if (shelfRestoreStream != null)
			{
				content.LoadContentFromStream(shelfRestoreStream);

				// #4183 - the shelf restore stream includes the shelf title, which is supposed to be determined by the model/localization and not persisted
				content.Title = content.FullTitle = title;

				_form.DockingManager.ShowContent(content);
				if (content.IsAutoHidden && hint != ShelfDisplayHint.HideOnWorkspaceOpen)
					_form.DockingManager.BringAutoHideIntoView(content);

				return content;
			}

        	content.DisplaySize = displaySize;
        	content.AutoHideSize = displaySize;
        	content.FloatingSize = displaySize;

        	if ((hint & ShelfDisplayHint.DockAutoHide) != 0)
        		_form.DockingManager.Container.SuspendLayout();

        	// Dock the window on the correct edge
        	if ((hint & ShelfDisplayHint.DockTop) != 0)
        	{
        		_form.DockingManager.AddContentWithState(content, State.DockTop);
        	}
        	else if ((hint & ShelfDisplayHint.DockBottom) != 0)
        	{
        		_form.DockingManager.AddContentWithState(content, State.DockBottom);
        	}
        	else if ((hint & ShelfDisplayHint.DockLeft) != 0)
        	{
        		_form.DockingManager.AddContentWithState(content, State.DockLeft);
        	}
        	else if ((hint & ShelfDisplayHint.DockRight) != 0)
        	{
        		_form.DockingManager.AddContentWithState(content, State.DockRight);
        	}
        	else
        	{
				if ((hint & ShelfDisplayHint.ShowNearMouse) == ShelfDisplayHint.ShowNearMouse)
				{
					content.DisplayLocation = Control.MousePosition;
				}

        		_form.DockingManager.AddContentWithState(content, State.Floating);
        	}

		    if ((hint & ShelfDisplayHint.DockAutoHide) != 0)
            {
                _form.DockingManager.ToggleContentAutoHide(content);
                _form.DockingManager.Container.ResumeLayout();
                _form.DockingManager.BringAutoHideIntoView(content);
            }

            return content;
        }

        internal void ShowShelfView(ShelfView shelfView)
        {
			if (!shelfView.Content.Visible)
				shelfView.Content.BringToFront();

            if (shelfView.Content.IsDocked)
            {
                if (shelfView.Content.IsAutoHidden)   // auto-hide mode
                {
                    // show without activating
                    _form.DockingManager.BringAutoHideIntoView(shelfView.Content); // show it
                }
                else
                {
                    // content is pinned - therefore it should be already visible
                }
            }
            else
            {
                // floating
                _form.DockingManager.ShowContent(shelfView.Content);
            }
        }

        internal void HideShelfView(ShelfView shelfView)
        {
            if (shelfView.Content.IsDocked)
            {
                if (shelfView.Content.IsAutoHidden)   // auto-hide mode
                {
                    // only one auto-hide window can be showing at a given time, so calling this method should hide it
                    _form.DockingManager.RemoveShowingAutoHideWindows();
                }
                else
                {
                    // content is pinned - putting it in auto-hide mode should hide it
                    _form.DockingManager.ToggleContentAutoHide(shelfView.Content);

                    // the window seems to remain active even though it is not visible, which doesn't make much sense
                    // therefore, let's report it as inactive
                    shelfView.SetActiveStatus(false);
                    // since we don't seem to get a content-hiding message in this case, need to explicitly set this
                    shelfView.SetVisibleStatus(false);
                }
            }
            else
            {
                // floating
                _form.DockingManager.HideContent(shelfView.Content);

                // since we don't seem to get a content-hiding message in this case, need to explicitly set this
                shelfView.SetVisibleStatus(false);  
            }
        }

        internal void HideShelvesOnWorkspaceOpen()
        {
            // 1) Retract all visible autohide windows
            // 2) Put docked windows in autohide mode if the tool has specified so
            _form.DockingManager.RemoveShowingAutoHideWindows();

            for (int i = 0; i < _form.DockingManager.Contents.Count; i++)
            {
                var content = _form.DockingManager.Contents[i];

                var shelfView = (ShelfView)content.Tag;
                if ((shelfView.DisplayHint & ShelfDisplayHint.HideOnWorkspaceOpen) != 0)
                {
                    shelfView.Hide();
                }
            }
        }
        
        internal void ActivateShelfView(ShelfView shelfView)
        {
            if (shelfView.Content.IsAutoHidden)
            {
                // auto-hidden - bring into view
                _form.DockingManager.BringAutoHideIntoView(shelfView.Content);
            }
            else
            {
                // docked or floating - ensure we are in front
                shelfView.Content.BringToFront();
            }

            // set focus to the control - this is what actually activates the window
            shelfView.Content.Control.Focus();
        }

        internal void RemoveShelfView(ShelfView shelfView)
        {
			shelfView.SaveState();

            _form.DockingManager.Contents.Remove(shelfView.Content);
            shelfView.SetVisibleStatus(false);
        }

        private void DockingManagerContentHidingEventHandler(Content c, CancelEventArgs cea)
        {
            // this event is fired when the X on the shelf is clicked
            var shelfView = (ShelfView)c.Tag;

            // don't let dotnetmagic remove the shelf
            cea.Cancel = true;

            shelfView.RaiseCloseRequested();
        }

        private void DockingManagerContentShownEventHandler(Content c, EventArgs cea)
        {
        }

        private void DockingManagerContentAutoHideClosedEventHandler(Content c, EventArgs cea)
        {
            var shelfView = (ShelfView)c.Tag;

            shelfView.SetActiveStatus(false);   // force active to false, since the dotnetmagic events are not reliable
            shelfView.SetVisibleStatus(false);
        }

        private void DockingManagerContentAutoHideOpeningEventHandler(Content c, EventArgs cea)
        {
            var shelfView = (ShelfView)c.Tag;
            shelfView.SetVisibleStatus(true);
        }

        private void FormDockingManagerWindowDeactivatedEventHandler(DockingManager dm, Window wd)
        {
            var content = ((WindowContent) wd).CurrentContent;

            // seems that content may sometimes be null - not sure why
            // in this case, just ignore the event
            if (content != null)
            {
                var shelfView = (ShelfView)content.Tag;
                shelfView.SetActiveStatus(false);
            }
        }

        private void DockingManagerWindowActivatedEventHandler(DockingManager dm, Window wd)
        {
            var content = ((WindowContent) wd).CurrentContent;

            // seems that content may sometimes be null - not sure why
            // in this case, just ignore the event
            if (content != null)
            {
                var shelfView = (ShelfView)content.Tag;
                // when activated, report both visible and active status
                shelfView.SetVisibleStatus(true);
                shelfView.SetActiveStatus(true);
            }
        }

        #endregion

        #region IDesktopWindowView Members

        /// <summary>
        /// Creates a new view for the specified <see cref="Workspace"/>.
        /// </summary>
        /// <remarks>
        /// Override this method if you want to return a custom implementation of <see cref="IWorkspaceView"/>.
        /// In practice, it is preferable to subclass <see cref="WorkspaceView"/> rather than implement <see cref="IWorkspaceView"/>
        /// directly.
        /// </remarks>
        /// <param name="workspace"></param>
        /// <returns></returns>
        public virtual IWorkspaceView CreateWorkspaceView(Workspace workspace)
        {
            return new WorkspaceView(workspace, this);
        }

        /// <summary>
        /// Creates a new view for the specified <see cref="Shelf"/>.
        /// </summary>
        /// <remarks>
        /// Override this method if you want to return a custom implementation of <see cref="IShelfView"/>.
        /// In practice, it is preferable to subclass <see cref="ShelfView"/> rather than implement <see cref="IShelfView"/>
        /// directly.
        /// </remarks>
        /// <param name="shelf"></param>
        /// <returns></returns>
        public virtual IShelfView CreateShelfView(Shelf shelf)
        {
            return new ShelfView(shelf, this);
        }

        /// <summary>
        /// Creates a new view for the specified <see cref="DialogBox"/>.
        /// </summary>
        /// <remarks>
        /// Override this method if you want to return a custom implementation of <see cref="IDialogBoxView"/>.
        /// In practice, it is preferable to subclass <see cref="DialogBoxView"/> rather than implement <see cref="IDialogBoxView"/>
        /// directly.
        /// </remarks>
        /// <param name="dialogBox"></param>
        /// <returns></returns>
        public virtual IDialogBoxView CreateDialogBoxView(DialogBox dialogBox)
        {
            return new DialogBoxView(dialogBox, this);
        }

        /// <summary>
        /// Sets the menu model, causing the menu displayed on the screen to be updated.
        /// </summary>
        /// <remarks>
        /// The default implementation just sets the <see cref="DesktopForm.MenuModel"/> property.
        /// Override this method if you need to perform custom processing.
        /// </remarks>
        /// <param name="model"></param>
        public virtual void SetMenuModel(ActionModelNode model)
        {
            _form.MenuModel = model;
        }

        /// <summary>
        /// Sets the toolbar model, causing the toolbar displayed on the screen to be updated.
        /// </summary>
        /// <remarks>
        /// The default implementation just sets the <see cref="DesktopForm.ToolbarModel"/> property.
        /// Override this method if you need to perform custom processing.
        /// </remarks>
        /// <param name="model"></param>
        public virtual void SetToolbarModel(ActionModelNode model)
        {
            _form.ToolbarModel = model;
        }

    	/// <summary>
    	/// Displays a message box.
    	/// </summary>
    	/// <remarks>
    	/// Override this method if you need to customize the display of message boxes.
    	/// </remarks>
    	/// <param name="message"></param>
    	/// <param name="title"> </param>
    	/// <param name="buttons"></param>
    	/// <returns></returns>
    	public virtual DialogBoxAction ShowMessageBox(string message, string title, MessageBoxActions buttons)
        {
            var mb = new MessageBox();
            return mb.Show(message, title, buttons, _form);
        }

    	/// <summary>
    	/// Sets the alert context.
    	/// </summary>
    	/// <param name="alertContext"></param>
    	public void SetAlertContext(IDesktopAlertContext alertContext)
    	{
    		_alertContext = alertContext;
    	}

    	/// <summary>
    	/// Shows an alert notification in front of this window.
    	/// </summary>
    	public virtual void ShowAlert(AlertNotificationArgs args)
    	{
			var icon = _alertContext.GetIcon(args.Level).CreateIcon(IconSize.Large, new ResourceResolver(typeof(DesktopWindow).Assembly)); ;

			if (args.Level == AlertLevel.Info)
			{
				var dialog = _infoNotificationDialog;
				dialog.OpenLogLinkText = SR.LinkViewAllAlerts;
				dialog.AlertIcon = icon;
				dialog.Message = args.Message;
				dialog.LinkText = args.LinkText ?? "";
				dialog.LinkHandler = AlertLinkHandler(args.LinkAction);
				dialog.DismissOnLinkClicked = args.DismissOnLinkClicked;
				dialog.Popup(_errorNotificationDialog.Visible ? 1 : 0);
			}
			else
			{
				// both errors and warnings use the _errorNotificationDialog instance
				var dialog = _errorNotificationDialog;

				// update the un-acknowledged count
				var c = _alertContext.UnacknowledgedErrorWarningCount;
				dialog.OpenLogLinkText = c > 1 ? string.Format(SR.LinkMoreNewAlerts, c - 1) : SR.LinkViewAllAlerts;

				// if the dialog is already visible with an error, do not overwrite it with a warning (lower severity)
				if (args.Level == AlertLevel.Error || !_errorNotificationDialog.Visible)
				{
					dialog.AlertIcon = icon;
					dialog.Message = args.Message;
					dialog.LinkText = args.LinkText ?? "";
					dialog.LinkHandler = AlertLinkHandler(args.LinkAction);
					dialog.DismissOnLinkClicked = args.DismissOnLinkClicked;
					dialog.Popup(0);
				}
			}
		}

    	/// <summary>
    	/// Shows a 'Save file' dialog in front of this window.
    	/// </summary>
    	/// <param name="args"></param>
    	/// <returns></returns>
    	public virtual FileDialogResult ShowSaveFileDialogBox(FileDialogCreationArgs args)
    	{
			var dialog = new SaveFileDialog();
			PrepareFileDialog(dialog, args);
			dialog.OverwritePrompt = true;
    		dialog.FileOk += (sender, e) =>
    		                 	{
									if(!ValidateFileSavePath(dialog.FileName, args))
									{
										e.Cancel = true;
									}
    		                 	};

			var dr = dialog.ShowDialog(_form);
			if(dr == DialogResult.OK)
			{
				return new FileDialogResult(DialogBoxAction.Ok, dialog.FileName);
			}
    		return new FileDialogResult(DialogBoxAction.Cancel, (string)null);
    	}

    	/// <summary>
    	/// Shows a 'Open file' dialog in front of this window.
    	/// </summary>
    	/// <param name="args"></param>
    	/// <returns></returns>
    	public virtual FileDialogResult ShowOpenFileDialogBox(FileDialogCreationArgs args)
    	{
			var dialog = new OpenFileDialog();
			PrepareFileDialog(dialog, args);
    		dialog.CheckFileExists = true;
    		dialog.ShowReadOnly = false;
    		dialog.Multiselect = args.MultiSelect;

			var dr = dialog.ShowDialog(_form);
			if (dr == DialogResult.OK)
			{
				return new FileDialogResult(DialogBoxAction.Ok, dialog.FileNames);
			}
			return new FileDialogResult(DialogBoxAction.Cancel, (string)null);
    	}

    	/// <summary>
    	/// Shows a 'Select folder' dialog in front of this window.
    	/// </summary>
    	/// <param name="args"></param>
    	/// <returns></returns>
    	public FileDialogResult ShowSelectFolderDialogBox(SelectFolderDialogCreationArgs args)
    	{
    		var dialog = new FolderBrowserDialog();
    		dialog.SelectedPath = args.Path ?? "";
    		dialog.Description = args.Prompt ?? "";
    		dialog.ShowNewFolderButton = args.AllowCreateNewFolder;

    		var dr = dialog.ShowDialog(_form);
    		if (dr == DialogResult.OK)
    		{
    			return new FileDialogResult(DialogBoxAction.Ok, dialog.SelectedPath);
    		}
    		return new FileDialogResult(DialogBoxAction.Cancel, (string)null);
    	}

    	#endregion

        #region DesktopObjectView overrides

        /// <summary>
        /// Opens this view, showing the form on the screen.
        /// </summary>
        public override void Open()
        {
			try
			{
				LoadWindowSettings();
			}
			catch (Exception e)
			{
				// if the window settings can't be loaded for any reason,
				// just log it and move on
				Platform.Log(LogLevel.Error, e);
			}

            _form.Show();
        }

        /// <summary>
        /// Activates the view, activating the form on the screen.
        /// </summary>
        public override void Activate()
        {
            _form.Activate();
        }

        /// <summary>
        /// Shows the view, making the form visible on the screen.
        /// </summary>
        public override void Show()
        {
            _form.Show();
        }

        /// <summary>
        /// Hides the view, hiding the form on the screen.
        /// </summary>
        public override void Hide()
        {
            _form.Hide();
        }

        /// <summary>
        /// Sets the title that is displayed in the form's title bar.
        /// </summary>
        /// <remarks>
        /// Override this method if you need to customize the title that is displayed on the form.
        /// </remarks>
        /// <param name="title"></param>
        public override void SetTitle(string title)
        {
            _form.Text = title;
        }

        /// <summary>
        /// Disposes of this object, closing the form.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && _form != null)
            {
				if(_infoNotificationDialog != null)
				{
					_infoNotificationDialog.Dispose();
					_infoNotificationDialog = null;
				}
				if (_errorNotificationDialog != null)
				{
					_errorNotificationDialog.Dispose();
					_errorNotificationDialog = null;
				}


				_form.VisibleChanged -= FormVisibleChangedEventHandler;
				_form.Activated -= FormActivatedEventHandler;
				_form.Deactivate -= FormDeactivateEventHandler;
				_form.FormClosing -= FormFormClosingEventHandler;
				_form.TabbedGroups.PageCloseRequest -= TabbedGroupPageClosePressedEventHandler;
				_form.TabbedGroups.PageChanged -= TabbedGroupPageChangedEventHandler;
				_form.DockingManager.ContentHiding -= DockingManagerContentHidingEventHandler;
				_form.DockingManager.ContentShown -= DockingManagerContentShownEventHandler;
				_form.DockingManager.ContentAutoHideOpening -= DockingManagerContentAutoHideOpeningEventHandler;
				_form.DockingManager.ContentAutoHideClosed -= DockingManagerContentAutoHideClosedEventHandler;
				_form.DockingManager.WindowActivated -= DockingManagerWindowActivatedEventHandler;
				_form.DockingManager.WindowDeactivated -= FormDockingManagerWindowDeactivatedEventHandler;

                try
                {
                    SaveWindowSettings();
                }
                catch (Exception e)
                {
                    // if the window settings can't be saved for any reason,
                    // just log it and move on
                    Platform.Log(LogLevel.Error, e);
                }

                // bug #1171: if this window is the active window and there are other windows,
                // select the previously active one before destroying this one
                if (_desktopWindowActivationOrder.LastElement == this && _desktopWindowActivationOrder.Count > 1)
                {
                    _desktopWindowActivationOrder.SecondLastElement.Activate();
                }

                // remove this window from the activation order
                _desktopWindowActivationOrder.Remove(this);


                // now that we've cleaned up the activation,
                // we can destroy the form safely without worrying 
                // about the OS triggering activation events

                // this will close the form without firing any events
                _form.Dispose();
                _form = null;

                // notify that we are no longer visible
                SetVisibleStatus(false);
            }

            base.Dispose(disposing);
        }

        #endregion

        /// <summary>
        /// Called to create an instance of a <see cref="DesktopForm"/> for use by this view.
        /// </summary>
        /// <returns></returns>
        protected virtual DesktopForm CreateDesktopForm()
        {
            return new DesktopForm();
        }

        /// <summary>
        /// Gets the <see cref="DesktopForm"/> that is displayed on the screen.
        /// </summary>
        protected internal DesktopForm DesktopForm
        {
            get { return _form; }
        }

		private void LoadWindowSettings()
		{
			Rectangle screenRectangle;
			FormWindowState windowState;
			if (!DesktopViewSettings.Default.GetDesktopWindowState(_desktopWindow.Name, out screenRectangle, out windowState))
			{
				screenRectangle = WinFormsScreen.PrimaryScreen.Bounds;

				// Make the window size 75% of the primary screen
				const float scale = 0.75f;
				_form.Width = (int)(screenRectangle.Width * scale);
				_form.Height = (int)(screenRectangle.Height * scale);

				_form.StartPosition = FormStartPosition.CenterScreen;
			}
			else
			{
				_form.Location = screenRectangle.Location;
				_form.Size = screenRectangle.Size;
				_form.StartPosition = FormStartPosition.Manual;

				// If window was last closed when minimized, don't open it up minimized,
				// but rather just open it normally
				if (windowState == FormWindowState.Minimized)
					_form.WindowState = FormWindowState.Normal;
				else
					_form.WindowState = windowState;
			}
		}

		private void SaveWindowSettings()
		{
			Rectangle windowRectangle;
			// If the window state is normal, just save its location and size
			if (_form.WindowState == FormWindowState.Normal)
				windowRectangle = new Rectangle(_form.Location, _form.Size);
			// But, if it's minimized or maximized, save the restore bounds instead
			else
				windowRectangle = _form.RestoreBounds;

			FormWindowState windowState = _form.WindowState;

			DesktopViewSettings.Default.SaveDesktopWindowState(_desktopWindow.Name, windowRectangle, windowState);
		}

		private void PrepareFileDialog(FileDialog dialog, FileDialogCreationArgs args)
		{
			dialog.AddExtension = !string.IsNullOrEmpty(args.FileExtension);
			dialog.DefaultExt = args.FileExtension;
			dialog.FileName = args.FileName;
			dialog.InitialDirectory = args.Directory;
			dialog.RestoreDirectory = true;
			dialog.Title = args.Title;

			dialog.Filter = StringUtilities.Combine(args.Filters, "|", f => f.Description + "|" + f.Filter);
		}

		private bool ValidateFileSavePath(string filePath, FileDialogCreationArgs args)
		{
			if (args.PreventSaveToInstallPath && filePath.StartsWith(Platform.InstallDirectory))
			{
				ShowMessageBox(SR.ErrorSaveFileToInstallPath, null, MessageBoxActions.Ok);
				return false;
			}
			return true;
		}

		private System.Action AlertLinkHandler(Action<DesktopWindow> linkAction)
		{
			return delegate
			{
				try
				{
					if (linkAction != null)
					{
						linkAction(_desktopWindow);
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			};
		}

		private void ErrorDialogDismissed(object sender, AlertNotificationForm.DismissedEventArgs e)
		{
			// if the error dialog was manually dismissed by the user, consider that an acknowledgement of all alerts
			if(!e.AutoDismissed)
			{
				_alertContext.AcknowledgeAll();
			}
		}

		private void AlertDialogOpenLogClicked(object sender, EventArgs e)
		{
			_alertContext.ShowAlertViewer();
		}
	}
}

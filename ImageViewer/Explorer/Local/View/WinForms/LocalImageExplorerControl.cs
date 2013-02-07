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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Controls.WinForms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Local.View.WinForms
{
	public partial class LocalImageExplorerControl : UserControl
	{
		private LocalImageExplorerComponent _component;
		private DelayedEventPublisher _folderViewSelectionUpdatePublisher;
		private Pidl _homeLocation = null;

		public LocalImageExplorerControl(LocalImageExplorerComponent component)
		{
			_component = component;

			_folderViewSelectionUpdatePublisher = new DelayedEventPublisher(OnFolderViewSelectionUpdatePublished);

			InitializeComponent();
			InitializeHistoryMenu();
			InitializeIcons();
			InitializeFocusList();

			SetViewMode(Settings.Default.FolderViewMode, false);
			SetHomeLocation(Settings.Default.HomeLocation);

			_folderView.ExceptionRaised += OnFolderControlExceptionRaised;
			_folderTree.ExceptionRaised += OnFolderControlExceptionRaised;

			ResetFocus(); // reset focus must happen after explorer controls are initially populated

			// Initialize menus here
			ToolStripBuilder.BuildMenu(_folderViewContextMenu.Items, _component.ContextMenuModel.ChildNodes);
			ToolStripBuilder.BuildMenu(_folderTreeContextMenu.Items, _component.ContextMenuModel.ChildNodes);
		}

		private void PerformDispose(bool disposing)
		{
			if (disposing)
			{
				// this is a managed wrapper so it should only be disposed on disposing
				if (_homeLocation != null)
				{
					_homeLocation.Dispose();
					_homeLocation = null;
				}

				if (_folderViewSelectionUpdatePublisher != null)
				{
					_folderViewSelectionUpdatePublisher.Dispose();
					_folderViewSelectionUpdatePublisher = null;
				}
			}
		}

		private void OnFolderControlExceptionRaised(object sender, ItemEventArgs<Exception> e)
		{
			Exception ex = e.Item;
			Platform.Log(LogLevel.Debug, "FolderControl exception detected. Last Known Location: {0}", _txtAddress.Text);
			ExceptionHandler.Report(ex, _component.DesktopWindow);
		}

		private void UpdateFolderTreeSelection()
		{
			_component.Selection = new PathSelection(_folderTree.SelectedItem);
		}

		private void OnFolderViewSelectionUpdatePublished(object sender, EventArgs e)
		{
			_component.Selection = new PathSelection(CollectionUtils.Cast<FolderObject>(_folderView.SelectedItems));
		}

		private void OnItemOpened(object sender, EventArgs e)
		{
			if (_component.DefaultActionHandler != null)
			{
				_component.DefaultActionHandler();
			}
		}

		private void SetViewMode(System.Windows.Forms.View view, bool saveSetting)
		{
			_mnuIconsView.Checked = (view == System.Windows.Forms.View.LargeIcon);
			_mnuListView.Checked = (view == System.Windows.Forms.View.List);
			_mnuDetailsView.Checked = (view == System.Windows.Forms.View.Details);
			_mnuTilesView.Checked = (view == System.Windows.Forms.View.Tile);
			_folderView.View = view;

			if (saveSetting)
			{
				Settings settings = Settings.Default;
				settings.FolderViewMode = view;
				settings.Save();
			}
		}

		private void SetHomeLocation(string homeLocation)
		{
			if (!string.IsNullOrEmpty(homeLocation))
			{
				try
				{
					Environment.SpecialFolder specialFolder = (Environment.SpecialFolder) Enum.Parse(typeof (Environment.SpecialFolder), homeLocation);
					_homeLocation = new Pidl(specialFolder);
					return;
				}
				catch (ArgumentException) {}
				catch (NotSupportedException) {}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Debug, ex, "The special folder {0} isn't available.", homeLocation);
				}

				Pidl pidl;
				if (Pidl.TryParse(homeLocation, out pidl))
				{
					_homeLocation = pidl;
					return;
				}

				_homeLocation = null;
			}
		}

		public void BrowseToHome()
		{
			if (_homeLocation == null)
				_folderCoordinator.BrowseToHome();
			else
				_folderCoordinator.BrowseTo(_homeLocation);
		}

		#region Tab Order

		private delegate bool FocusDelegate(bool forward);

		private IList<KeyValuePair<Control, FocusDelegate>> _focusDelegates = null;

		private void InitializeFocusList()
		{
			// initialize control focus list
			List<KeyValuePair<Control, FocusDelegate>> focusDelegates = new List<KeyValuePair<Control, FocusDelegate>>(3);
			focusDelegates.Add(new KeyValuePair<Control, FocusDelegate>(_folderTree, f => _folderTree.SelectNextControl(_folderTree, f, false, true, false)));
			focusDelegates.Add(new KeyValuePair<Control, FocusDelegate>(_folderView, f => _folderView.SelectNextControl(_folderView, f, false, true, false)));
			focusDelegates.Add(new KeyValuePair<Control, FocusDelegate>(_addressStrip, f =>
			                                                                           	{
			                                                                           		_txtAddress.Focus();
			                                                                           		return _addressStrip.ContainsFocus;
			                                                                           	}));
			_focusDelegates = focusDelegates.AsReadOnly();
		}

		private void ResetFocus()
		{
			if (_focusDelegates.Count > 0)
				_focusDelegates[0].Value.Invoke(true);
		}

		protected override bool ProcessTabKey(bool forward)
		{
			// overrides the tab order using the focus delegates list
			int indexFocusedControl = 0;
			while (indexFocusedControl < _focusDelegates.Count)
			{
				// find the control that is currently focused
				if (_focusDelegates[indexFocusedControl].Key.ContainsFocus)
				{
					// try to focus the next control in sequence
					for (int offset = 1; offset < _focusDelegates.Count; offset++)
					{
						int index = (indexFocusedControl + (forward ? offset : _focusDelegates.Count - offset))%_focusDelegates.Count;
						if (_focusDelegates[index].Value.Invoke(forward))
							break; // end loop on first control that successfully focused
					}
					return true;
				}
				indexFocusedControl++;
			}
			return base.ProcessTabKey(forward);
		}

		#endregion

		#region Explorer Control

		private const int ShowHistoryCount = 10;
		private string _lastValidLocation = string.Empty;

		private static void InitializeImageList(ImageList imageList, string sizeString)
		{
			Type type = typeof (LocalImageExplorerControl);
			var resourceResolver = new ActionResourceResolver(type);

			string[] icons = {"Back", "Next", "Up", "Refresh", "Home", "ShowFolders", "View", "Go"};
			foreach (string iconName in icons)
			{
				var resourceName = string.Format("{0}.Icons.{1}Tool{2}.png", type.Namespace, iconName, sizeString);
				using (var ioStream = resourceResolver.OpenResource(resourceName))
				{
					if (ioStream == null)
						continue;
					imageList.Images.Add(iconName, Image.FromStream(ioStream));
				}
			}
		}

		private ImageList GetImageList(IconSize iconSize)
		{
			if (iconSize == IconSize.Small)
				return _smallIconImageList;

			if (iconSize == IconSize.Medium)
				return _mediumIconImageList;

			return _largeIconImageList;
		}

		private void InitializeIcons()
		{
			InitializeImageList(_largeIconImageList, "Large");
			InitializeImageList(_mediumIconImageList, "Medium");
			InitializeImageList(_smallIconImageList, "Small");

			_toolStrip.ImageList = GetImageList(Settings.Default.ToolbarIconSize);

			_btnBack.ImageKey = @"Back";
			_btnForward.ImageKey = @"Next";
			_btnUp.ImageKey = @"Up";
			_btnRefresh.ImageKey = @"Refresh";
			_btnHome.ImageKey = @"Home";
			_btnShowFolders.ImageKey = @"ShowFolders";
			_btnViews.ImageKey = @"View";

			_addressStrip.ImageList = _smallIconImageList;
			_btnGo.ImageKey = @"Go";
		}

		private void InitializeHistoryMenu()
		{
			for (int n = 0; n < ShowHistoryCount; n++)
			{
				ToolStripMenuItem menuBack = new ToolStripMenuItem();
				menuBack.Click += _mnuHistoryItem_Click;
				menuBack.Tag = -(n + 1);
				menuBack.Visible = false;
				_btnBack.DropDownItems.Add(menuBack);

				ToolStripMenuItem menuForward = new ToolStripMenuItem();
				menuForward.Click += _mnuHistoryItem_Click;
				menuForward.Tag = n + 1;
				menuForward.Visible = false;
				_btnForward.DropDownItems.Add(menuForward);
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			try
			{
				this.BrowseToHome();
			}
			catch (Exception ex)
			{
				OnFolderControlExceptionRaised(null, new ItemEventArgs<Exception>(ex));
			}

			base.OnLoad(e);
		}

		private void _folderCoordinator_CurrentPidlChanged(object sender, EventArgs e)
		{
			_btnUp.Enabled = _folderCoordinator.CanBrowseToParent;
			_btnBack.Enabled = _folderCoordinator.CanBrowseToPrevious;
			_btnForward.Enabled = _folderCoordinator.CanBrowseToNext;
			_lastValidLocation = _txtAddress.Text = _folderCoordinator.CurrentPath;
			this.Text = _folderCoordinator.CurrentDisplayName;
			this.UpdateBackButtonMenu();
			this.UpdateForwardButtonMenu();
		}

		private void UpdateBackButtonMenu()
		{
			int count = 0;
			foreach (Pidl pastPidl in _folderCoordinator.EnumeratePreviousLocations(false))
			{
				if (count >= ShowHistoryCount)
					break;
				_btnBack.DropDownItems[count].Text = pastPidl.DisplayName;
				_btnBack.DropDownItems[count].Visible = true;
				count++;
			}
			for (int n = count; n < ShowHistoryCount; n++)
				_btnBack.DropDownItems[n].Visible = false;
		}

		private void UpdateForwardButtonMenu()
		{
			int count = 0;
			foreach (Pidl futurePidl in _folderCoordinator.EnumerateNextLocations(false))
			{
				if (count >= ShowHistoryCount)
					break;
				_btnForward.DropDownItems[count].Text = futurePidl.DisplayName;
				_btnForward.DropDownItems[count].Visible = true;
				count++;
			}
			for (int n = count; n < ShowHistoryCount; n++)
				_btnForward.DropDownItems[n].Visible = false;
		}

		private void _btnUp_Click(object sender, EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			try
			{
				_folderCoordinator.BrowseToParent();
			}
			catch (Exception ex)
			{
				OnFolderControlExceptionRaised(null, new ItemEventArgs<Exception>(ex));
			}
			finally
			{
				this.ResetCursor();
			}
		}

		private void _btnBack_Click(object sender, EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			try
			{
				_folderCoordinator.BrowseToPrevious();
			}
			catch (Exception ex)
			{
				OnFolderControlExceptionRaised(null, new ItemEventArgs<Exception>(ex));
			}
			finally
			{
				this.ResetCursor();
			}
		}

		private void _btnForward_Click(object sender, EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			try
			{
				_folderCoordinator.BrowseToNext();
			}
			catch (Exception ex)
			{
				OnFolderControlExceptionRaised(null, new ItemEventArgs<Exception>(ex));
			}
			finally
			{
				this.ResetCursor();
			}
		}

		private void _btnHome_Click(object sender, EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			try
			{
				this.BrowseToHome();
			}
			catch (Exception ex)
			{
				OnFolderControlExceptionRaised(null, new ItemEventArgs<Exception>(ex));
			}
			finally
			{
				this.ResetCursor();
			}
		}

		private void _btnRefresh_Click(object sender, EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			try
			{
				_folderCoordinator.Refresh();
			}
			catch(Exception ex)
			{
				OnFolderControlExceptionRaised(null, new ItemEventArgs<Exception>(ex));
			}
			finally
			{
				this.ResetCursor();
			}
		}

		private void _btnGo_Click(object sender, EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			try
			{
				if (!string.IsNullOrEmpty(_txtAddress.Text))
					_folderCoordinator.BrowseTo(_txtAddress.Text);
			}
			catch (Exception ex)
			{
				this.OnFolderControlExceptionRaised(null, new ItemEventArgs<Exception>(ex));
				_txtAddress.Text = _lastValidLocation;
			}
			finally
			{
				this.ResetCursor();
			}
		}

		private void _txtAddress_KeyEnterPressed(object sender, EventArgs e)
		{
			_btnGo.PerformClick();
		}

		private void _mnuHistoryItem_Click(object sender, EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			try
			{
				_folderCoordinator.BrowseTo((int) ((ToolStripMenuItem) sender).Tag);
			}
			catch (Exception ex)
			{
				OnFolderControlExceptionRaised(null, new ItemEventArgs<Exception>(ex));
			}
			finally
			{
				this.ResetCursor();
			}
		}

		private void _btnShowFolders_Click(object sender, EventArgs e)
		{
			_btnShowFolders.Checked = !_btnShowFolders.Checked;
			_folderTree.Visible = _splitter.Visible = _btnShowFolders.Checked;
		}

		private void _mnuTilesView_Click(object sender, EventArgs e)
		{
			SetViewMode(System.Windows.Forms.View.Tile, true);
		}

		private void _mnuIconsView_Click(object sender, EventArgs e)
		{
			SetViewMode(System.Windows.Forms.View.LargeIcon, true);
		}

		private void _mnuListView_Click(object sender, EventArgs e)
		{
			SetViewMode(System.Windows.Forms.View.List, true);
		}

		private void _mnuDetailsView_Click(object sender, EventArgs e)
		{
			SetViewMode(System.Windows.Forms.View.Details, true);
		}

		private void _folderView_ItemDoubleClick(object sender, FolderViewItemEventArgs e)
		{
			if (!e.Item.IsFolder)
			{
				_folderViewSelectionUpdatePublisher.PublishNow(sender, e);
				OnItemOpened(sender, e);
				e.Handled = true;
			}
		}

		private void _folderView_SelectedItemsChanged(object sender, EventArgs e)
		{
			// listview-type controls fire the event for each item in the selection
			// (because each item selection change is conceptually separate in this type of GUI)
			// this can generate a lot of unecessary calls to update the component's selection
			// so we delay the event here until the selection settles down
			_folderViewSelectionUpdatePublisher.Publish(sender, e);
		}

		private void _folderTree_SelectedItemsChanged(object sender, EventArgs e)
		{
			UpdateFolderTreeSelection();
		}

		private void _folderControl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == (Keys.Control | Keys.A))
			{
				_folderView.SelectNextControl(_folderView, true, false, true, false);
				_folderView.SelectAll();
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		private void _folderControl_BeginBrowse(object sender, EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
		}

		private void _folderControl_EndBrowse(object sender, EventArgs e)
		{
			this.ResetCursor();
		}

		private void _folderViewContextMenu_Opening(object sender, CancelEventArgs e)
		{
			_folderViewSelectionUpdatePublisher.PublishNow(sender, e);
		}

		private void _folderTreeContextMenu_Opening(object sender, CancelEventArgs e)
		{
			UpdateFolderTreeSelection();
		}

		#endregion

		#region ExceptionPolicy Class

		[ExceptionPolicyFor(typeof (PathNotFoundException))]
		[ExceptionPolicyFor(typeof (PathAccessException))]
		[ExtensionOf(typeof (ExceptionPolicyExtensionPoint))]
		private class ExceptionPolicy : IExceptionPolicy
		{
			public void Handle(Exception ex, IExceptionHandlingContext exceptionHandlingContext)
			{
				if (ex is PathNotFoundException)
					Handle((PathNotFoundException) ex, exceptionHandlingContext);
				else if (ex is PathAccessException)
					Handle((PathAccessException) ex, exceptionHandlingContext);
			}

			private static void Handle(PathNotFoundException ex, IExceptionHandlingContext exceptionHandlingContext)
			{
				var sb = new StringBuilder();
				sb.AppendLine(SR.ErrorPathUnavailable);
				if (!string.IsNullOrEmpty(ex.Path))
					sb.AppendLine(string.Format(SR.FormatPath, ex.Path));
				exceptionHandlingContext.ShowMessageBox(sb.ToString());
			}

			private static void Handle(PathAccessException ex, IExceptionHandlingContext exceptionHandlingContext)
			{
				var sb = new StringBuilder();
				sb.AppendLine(SR.ErrorPathSecurity);
				if (!string.IsNullOrEmpty(ex.Path))
					sb.AppendLine(string.Format(SR.FormatPath, ex.Path));
				exceptionHandlingContext.ShowMessageBox(sb.ToString());
			}
		}

		#endregion

		#region PathSelection Class

		/// <summary>
		/// Custom <see cref="IPathSelection"/> implementation that allows for delayed shortcut resolution.
		/// </summary>
		/// <remarks>
		/// Resolving shortcuts can be expensive, so always call at the last possible moment (in conjunction with a user GUI action, preferably).
		/// </remarks>
		private class PathSelection : Selection<FolderObject>, IPathSelection
		{
			public PathSelection(FolderObject item) : base(item) {}
			public PathSelection(IEnumerable<FolderObject> folderObjects) : base(folderObjects) {}

			public string this[int index]
			{
				get { return Items[index].GetPath(true); }
			}

			public bool Contains(string path)
			{
				foreach (var item in Items)
					if (string.Equals(path, item.GetPath(true), StringComparison.InvariantCultureIgnoreCase))
						return true;
				return false;
			}

			public new IEnumerator<string> GetEnumerator()
			{
				foreach (var item in Items)
					yield return item.GetPath(true);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		#endregion
	}
}
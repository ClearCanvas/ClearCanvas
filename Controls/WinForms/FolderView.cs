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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ClearCanvas.Controls.WinForms.Native;

namespace ClearCanvas.Controls.WinForms
{
	public class FolderView : FolderControl
	{
		private event EventHandler _selectedItemsChanged;
		private event FolderViewItemEventHandler _itemDoubleClick;
		private event FolderViewItemEventHandler _itemKeyEnterPressed;
		private readonly FolderListView _folderListView;
		private IList<FolderViewItem> _selectedItems;
		private bool _suppressFolderListViewSelectedIndexChanged = false;

		public FolderView()
		{
			_folderListView = new FolderListView();
			_folderListView.BeforeBrowse += OnFolderListViewBeforeBrowse;
			_folderListView.AfterBrowse += OnFolderListViewAfterBrowse;
			_folderListView.SelectedIndexChanged += OnFolderListViewSelectedIndexChanged;
			_folderListView.KeyDown += OnFolderListViewKeyDown;
			_folderListView.KeyPress += OnFolderListViewKeyPress;
			_folderListView.KeyUp += OnFolderListViewKeyUp;
			_folderListView.Dock = DockStyle.Fill;
			_folderListView.View = View.LargeIcon;

			_selectedItems = new List<FolderViewItem>(0).AsReadOnly();

			base.SuspendLayout();
			base.Controls.Add(_folderListView);
			base.ResumeLayout(false);
		}

		#region Designer Properties

		#region AutoArrange

		[DefaultValue(true)]
		public bool AutoArrange
		{
			get { return _folderListView.AutoArrange; }
			set
			{
				if (_folderListView.AutoArrange != value)
				{
					_folderListView.SuspendLayout();
					_folderListView.AutoArrange = value;
					_folderListView.ResumeLayout(true);
					this.OnPropertyChanged(new PropertyChangedEventArgs("AutoArrange"));
				}
			}
		}

		private void ResetAutoArrange()
		{
			this.AutoArrange = true;
		}

		#endregion

		#region AutoDrillDown

		[DefaultValue(true)]
		public bool AutoDrillDown
		{
			get { return _folderListView.AutoDrillDown; }
			set
			{
				if (_folderListView.AutoDrillDown != value)
				{
					_folderListView.AutoDrillDown = value;
					this.OnPropertyChanged(new PropertyChangedEventArgs("AutoDrillDown"));
				}
			}
		}

		private void ResetAutoDrillDown()
		{
			this.AutoDrillDown = true;
		}

		#endregion

		#region AutoWaitCursor

		[DefaultValue(true)]
		public bool AutoWaitCursor
		{
			get { return _folderListView.AutoWaitCursor; }
			set
			{
				if (_folderListView.AutoWaitCursor != value)
				{
					_folderListView.AutoWaitCursor = value;
					this.OnPropertyChanged(new PropertyChangedEventArgs("AutoWaitCursor"));
				}
			}
		}

		private void ResetAutoWaitCursor()
		{
			this.AutoWaitCursor = true;
		}

		#endregion

		#region DereferenceLinks

		[DefaultValue(true)]
		public bool DereferenceLinks
		{
			get { return _folderListView.DereferenceLinks; }
			set
			{
				if (_folderListView.DereferenceLinks != value)
				{
					_folderListView.DereferenceLinks = value;
					OnPropertyChanged(new PropertyChangedEventArgs("DereferenceLinks"));
				}
			}
		}

		private void ResetDereferenceLinks()
		{
			DereferenceLinks = true;
		}

		#endregion

		#region FileSizeFormat

		[DefaultValue(FileSizeFormat.BinaryOctets)]
		[Localizable(true)]
		public FileSizeFormat FileSizeFormat
		{
			get { return _folderListView.FileSizeFormat; }
			set
			{
				if (_folderListView.FileSizeFormat != value)
				{
					_folderListView.FileSizeFormat = value;
					this.OnPropertyChanged(new PropertyChangedEventArgs("FileSizeFormat"));
				}
			}
		}

		private void ResetFileSizeFormat()
		{
			this.FileSizeFormat = FileSizeFormat.BinaryOctets;
		}

		#endregion

		#region MultiSelect

		[DefaultValue(true)]
		public bool MultiSelect
		{
			get { return _folderListView.MultiSelect; }
			set
			{
				if (_folderListView.MultiSelect != value)
				{
					_folderListView.MultiSelect = value;
					this.OnPropertyChanged(new PropertyChangedEventArgs("MultiSelect"));
				}
			}
		}

		private void ResetMultiSelect()
		{
			this.MultiSelect = true;
		}

		#endregion

		#region View

		public View View
		{
			get { return _folderListView.View; }
			set
			{
				if (_folderListView.View != value)
				{
					_folderListView.SuspendLayout();
					_folderListView.View = value;
					_folderListView.ResumeLayout(true);
					this.OnPropertyChanged(new PropertyChangedEventArgs("View"));
				}
			}
		}

		private void ResetView()
		{
			this.View = View.LargeIcon;
		}

		private bool ShouldSerializeView()
		{
			return this.View != View.LargeIcon;
		}

		#endregion

		#endregion

		#region Sorting

		public void SortBy(SortKey sortKey)
		{
			_folderListView.Sort(sortKey);
		}

		#endregion

		#region SelectedItems

		public IList<FolderViewItem> SelectedItems
		{
			get
			{
				if (_selectedItems == null)
				{
					var baseName = _folderListView.CurrentPidl.DisplayName + System.IO.Path.DirectorySeparatorChar;
					var selection = new List<FolderViewItem>();
					if (_folderListView.SelectedItems != null)
					{
						foreach (FolderListViewItem item in _folderListView.SelectedItems)
							selection.Add(new FolderViewItem(item.Pidl.Path, baseName + item.DisplayName, item.DisplayName, item.IsFolder, false));
					}
					_selectedItems = selection.AsReadOnly();
				}
				return _selectedItems;
			}
		}

		public event EventHandler SelectedItemsChanged
		{
			add { _selectedItemsChanged += value; }
			remove { _selectedItemsChanged -= value; }
		}

		protected virtual void OnSelectedItemsChanged(EventArgs e)
		{
			if (_selectedItemsChanged != null)
				_selectedItemsChanged.Invoke(this, e);
		}

		public void SelectAll()
		{
			_suppressFolderListViewSelectedIndexChanged = true;
			foreach (ListViewItem item in _folderListView.Items)
				item.Selected = true;
			_suppressFolderListViewSelectedIndexChanged = false;
			this.OnFolderListViewSelectedIndexChanged(_folderListView, EventArgs.Empty);
		}

		public void SelectNone()
		{
			_suppressFolderListViewSelectedIndexChanged = true;
			foreach (ListViewItem item in _folderListView.Items)
				item.Selected = false;
			_suppressFolderListViewSelectedIndexChanged = false;
			this.OnFolderListViewSelectedIndexChanged(_folderListView, EventArgs.Empty);
		}

		public void SelectInverted()
		{
			_suppressFolderListViewSelectedIndexChanged = true;
			foreach (ListViewItem item in _folderListView.Items)
				item.Selected = !item.Selected;
			_suppressFolderListViewSelectedIndexChanged = false;
			this.OnFolderListViewSelectedIndexChanged(_folderListView, EventArgs.Empty);
		}

		#endregion

		#region ItemDoubleClick

		public event FolderViewItemEventHandler ItemDoubleClick
		{
			add { _itemDoubleClick += value; }
			remove { _itemDoubleClick -= value; }
		}

		protected virtual void OnItemDoubleClick(FolderViewItemEventArgs e)
		{
			if (_itemDoubleClick != null)
				_itemDoubleClick.Invoke(this, e);
		}

		#endregion

		#region ItemKeyEnterPressed

		public event FolderViewItemEventHandler ItemKeyEnterPressed
		{
			add { _itemKeyEnterPressed += value; }
			remove { _itemKeyEnterPressed -= value; }
		}

		protected virtual void OnItemKeyEnterPressed(FolderViewItemEventArgs e)
		{
			if (_itemKeyEnterPressed != null)
				_itemKeyEnterPressed.Invoke(this, e);
		}

		#endregion

		protected override Pidl CurrentPidlCore
		{
			get { return _folderListView.CurrentPidl; }
		}

		protected override void BrowseToCore(Pidl pidl)
		{
			_folderListView.BrowseToPidl(pidl);
		}

		public override void Reload()
		{
			try
			{
				_folderListView.BrowseToPidl(_folderListView.CurrentPidl);
			}
			catch (Exception ex)
			{
				HandleBrowseException(ex);
			}
		}

		private void OnFolderListViewBeforeBrowse(object sender, CancelEventArgs e)
		{
			e.Cancel |= this.NotifyCoordinatorPidlChanging();
			if (!e.Cancel)
			{
				this.OnBeginBrowse(EventArgs.Empty);
			}
		}

		private void OnFolderListViewAfterBrowse(object sender, EventArgs e)
		{
			this.OnCurrentPidlChanged(EventArgs.Empty);
			this.NotifyCoordinatorPidlChanged();
			this.OnEndBrowse(EventArgs.Empty);
		}

		private void OnFolderListViewSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_suppressFolderListViewSelectedIndexChanged)
			{
				// reset the selected items list and notify listeners of change
				_selectedItems = null;
				this.OnSelectedItemsChanged(EventArgs.Empty);
				this.OnPropertyChanged(new PropertyChangedEventArgs("SelectedItems"));
			}
		}

		private void OnFolderListViewKeyDown(object sender, KeyEventArgs e)
		{
			this.OnKeyDown(e);
		}

		private void OnFolderListViewKeyPress(object sender, KeyPressEventArgs e)
		{
			this.OnKeyPress(e);
		}

		private void OnFolderListViewKeyUp(object sender, KeyEventArgs e)
		{
			this.OnKeyUp(e);
		}

		#region FolderViewItem Class

		public class FolderViewItem : FolderObject
		{
			internal FolderViewItem(Pidl pidl)
				: base(pidl.Path, pidl.VirtualPath, pidl.DisplayName, pidl.IsFolder, pidl.IsLink) {}

			internal FolderViewItem(string path, string virtualPath, string displayName, bool isFolder, bool isLink)
				: base(path, virtualPath, displayName, isFolder, isLink) {}
		}

		#endregion

		#region FolderListViewItem Class

		private class FolderListViewItem : ListViewItem, IDisposable
		{
			private Pidl _pidl;
			private readonly string _fullPath;
			private readonly int _iconIndex;
			private readonly bool _lastModifiedValid;

			public readonly string DisplayName;
			public readonly string TypeName;
			public readonly byte ItemClass;
			public readonly long FileSize;
			public readonly bool IsFolder;
			public readonly DateTime LastModified;

			public FolderListViewItem(Pidl absolutePidl, Pidl myDocumentsReferencePidl, FileSizeFormat fileSizeFormat)
			{
				_pidl = absolutePidl;
				_fullPath = absolutePidl.Path;

				// request shell item info now - it's faster than binding to ShellItem directly, and we need it for sorting purposes anyway
				var uFlags = SHGFI.SHGFI_PIDL | SHGFI.SHGFI_TYPENAME | SHGFI.SHGFI_DISPLAYNAME | SHGFI.SHGFI_SYSICONINDEX;

				// bug #9975: asking for SHGFI_ATTRIBUTES can be a very slow operation, so we don't want to ask for them unless we really need them.
				// if we're dealing with a regular file or directory, then getting the attributes gives us no additional information, so we can avoid it
				var isFile = !string.IsNullOrEmpty(_fullPath) && File.Exists(_fullPath);
				var isDirectory = !string.IsNullOrEmpty(_fullPath) && Directory.Exists(_fullPath);
				if (!isFile && !isDirectory)
				{
					uFlags = uFlags | SHGFI.SHGFI_ATTRIBUTES;
				}

				var shInfo = new SHFILEINFO();
				Shell32.SHGetFileInfo((IntPtr) _pidl, 0, out shInfo, (uint)Marshal.SizeOf(shInfo), uFlags);

				// check if item is purely virtual or is a physical file system object
				var isPureVirtual = !isFile && !isDirectory && ((SFGAO)shInfo.dwAttributes & SFGAO.SFGAO_FILESYSTEM) == 0;

				// check if item is actually a folder or potentially has subfolders; exclude real files that provide subfolders via shell namespace extensions (e.g. ZIP files in WinXP)
				var isFolder = isDirectory || (((SFGAO) shInfo.dwAttributes & (SFGAO.SFGAO_FOLDER | SFGAO.SFGAO_HASSUBFOLDER)) != 0 && !isFile);

				byte itemClass;
				if (_pidl == myDocumentsReferencePidl)
					itemClass = 0;
				else if (isPureVirtual && isFolder)
					itemClass = 64;
				else if (isFolder)
					itemClass = (byte) (128 + GetRootVolumeIndex(_fullPath));
				else
					itemClass = byte.MaxValue;

				this._iconIndex = shInfo.iIcon;
				this._lastModifiedValid = false;
				this.DisplayName = shInfo.szDisplayName;
				this.TypeName = shInfo.szTypeName;
				this.ItemClass = itemClass;
				this.LastModified = DateTime.MinValue;
				this.IsFolder = isFolder;
				this.FileSize = -1;

				if (File.Exists(_fullPath))
				{
					FileInfo fileInfo = new FileInfo(this._fullPath);
					this._lastModifiedValid = true;
					this.LastModified = fileInfo.LastWriteTime;
					this.FileSize = fileInfo.Length;
				}
				else if (Directory.Exists(_fullPath))
				{
					this._lastModifiedValid = true;
					this.LastModified = Directory.GetLastWriteTime(_fullPath);
				}

				this.Text = this.DisplayName;
				this.ImageIndex = this._iconIndex;
				this.SubItems.Add(CreateSubItem(this, FormatFileSize(this.FileSize, fileSizeFormat)));
				this.SubItems.Add(CreateSubItem(this, this.TypeName));
				this.SubItems.Add(CreateSubItem(this, FormatLastModified(this.LastModified, this._lastModifiedValid)));
			}

			public void Dispose()
			{
				if (_pidl != null)
				{
					_pidl.Dispose();
					_pidl = null;
				}
			}

			public Pidl Pidl
			{
				get { return _pidl; }
			}

			private static string FormatFileSize(long fileSize, FileSizeFormat fileSizeFormat)
			{
				if (fileSize < 0) // file doesn't exist!
					return string.Empty;

				switch (fileSizeFormat)
				{
					case FileSizeFormat.MetricOctets:
						if (fileSize < 900) // less than 900 bytes
							return string.Format(SR.FormatFileSizeBytes, fileSize);
						if (fileSize < 900000) // between 900 bytes and 900 KB
							return string.Format(SR.FormatFileSizeKB, fileSize/1000.0);
						else if (fileSize < 900000000) // between 900 KB and 900 MB
							return string.Format(SR.FormatFileSizeMB, fileSize/1000000.0);
						else if (fileSize < 900000000000) // between 900 MB and 900 GB
							return string.Format(SR.FormatFileSizeGB, fileSize/1000000000.0);
						else // greater than 900 GB...
							return string.Format(SR.FormatFileSizeTB, fileSize/1000000000000.0);
					case FileSizeFormat.LegacyOctets:
						if (fileSize < 896) // less than 896 bytes
							return string.Format(SR.FormatFileSizeBytes, fileSize);
						if (fileSize < 917504) // between 896 bytes and 896 KiB
							return string.Format(SR.FormatFileSizeKB, fileSize/1024.0);
						else if (fileSize < 939524096) // between 896 KiB and 896 MiB
							return string.Format(SR.FormatFileSizeMB, fileSize/1048576.0);
						else if (fileSize < 841813590016) // between 896 MiB and 896 GiB
							return string.Format(SR.FormatFileSizeGB, fileSize/1073741824.0);
						else // greater than 896 GiB...
							return string.Format(SR.FormatFileSizeTB, fileSize/1099511627776.0);
					case FileSizeFormat.BinaryOctets:
					default:
						if (fileSize < 896) // less than 896 bytes
							return string.Format(SR.FormatFileSizeBytes, fileSize);
						if (fileSize < 917504) // between 896 bytes and 896 KiB
							return string.Format(SR.FormatFileSizeKiB, fileSize/1024.0);
						else if (fileSize < 939524096) // between 896 KiB and 896 MiB
							return string.Format(SR.FormatFileSizeMiB, fileSize/1048576.0);
						else if (fileSize < 841813590016) // between 896 MiB and 896 GiB
							return string.Format(SR.FormatFileSizeGiB, fileSize/1073741824.0);
						else // greater than 896 GiB...
							return string.Format(SR.FormatFileSizeTiB, fileSize/1099511627776.0);
				}
			}

			private static string GetDirectoryRootVolume(string fullPath)
			{
				try
				{
					return Directory.GetDirectoryRoot(fullPath);
				}
				catch (Exception)
				{
					// not a standard rooted Windows path
					return string.Empty;
				}
			}

			private static byte GetRootVolumeIndex(string fullPath)
			{
				if (string.IsNullOrEmpty(fullPath))
					return 26;
				string rootVolume = GetDirectoryRootVolume(fullPath);
				if (string.IsNullOrEmpty(rootVolume) || !string.Equals(rootVolume, fullPath, StringComparison.InvariantCultureIgnoreCase))
					return 26;
				char driveLetter = rootVolume.ToLowerInvariant()[0];
				if (driveLetter < 'a' || driveLetter > 'z')
					return 26;
				return (byte) (driveLetter - 'a');
			}

			private static string FormatLastModified(DateTime lastModified, bool valid)
			{
				if (!valid)
					return string.Empty;
				return lastModified.ToString();
			}

			private static ListViewSubItem CreateSubItem(ListViewItem parent, string value)
			{
				return new ListViewSubItem(parent, value, SystemColors.GrayText, parent.BackColor, parent.Font);
			}
		}

		#endregion

		#region FolderListView Class

		private class FolderListView : ListView
		{
			public event CancelEventHandler BeforeBrowse;
			public event EventHandler AfterBrowse;
			private bool _suppressBrowseEvents = false;

			private const string COLUMN_NAME = "Name";
			private const string COLUMN_SIZE = "Size";
			private const string COLUMN_TYPE = "Type";
			private const string COLUMN_DATEMODIFIED = "DateModified";

			private Pidl _myDocumentsReferencePidl;
			private ShellItem _rootShellItem = new ShellItem();
			private ShellItem _currentShellItem = null;

			private bool _autoDrillDown = true;
			private bool _autoWaitCursor = true;
			private Cursor _oldCursor = null;
			private FileSizeFormat _fileSizeFormat = FileSizeFormat.BinaryOctets;

			public FolderListView() : base()
			{
				_currentShellItem = _rootShellItem.Clone();
				_myDocumentsReferencePidl = new Pidl(Environment.SpecialFolder.MyDocuments);

				this.Alignment = ListViewAlignment.SnapToGrid;
				this.AllowColumnReorder = false;
				this.AutoArrange = true;
				this.CheckBoxes = false;
				this.DereferenceLinks = true;
				this.HeaderStyle = ColumnHeaderStyle.Clickable;
				this.HideSelection = false;
				this.LabelEdit = false;
				this.LabelWrap = true;
				this.ListViewItemSorter = new FolderListViewItemComparer();
				this.MultiSelect = true;
				this.Scrollable = true;
				this.Sorting = SortOrder.Ascending;
				this.TileSize = new Size(12*(this.FontHeight + 4), 3*this.FontHeight + 4);
				this.View = View.LargeIcon;
				this.PopulateItems();
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					DisposeEach(this.Items);

					if (_currentShellItem != null)
					{
						_currentShellItem.Dispose();
						_currentShellItem = null;
					}

					if (_rootShellItem != null)
					{
						_rootShellItem.Dispose();
						_rootShellItem = null;
					}

					if (_myDocumentsReferencePidl != null)
					{
						_myDocumentsReferencePidl.Dispose();
						_myDocumentsReferencePidl = null;
					}
				}
				base.Dispose(disposing);
			}

			protected override void OnHandleCreated(EventArgs e)
			{
				base.OnHandleCreated(e);

				try
				{
					int hRes = User32.SendMessage(Handle, LV.LVM_SETIMAGELIST, LV.LVSIL_SMALL, SystemImageList.SmallIcons);
					if (hRes != 0)
						Marshal.ThrowExceptionForHR(hRes);

					hRes = User32.SendMessage(Handle, LV.LVM_SETIMAGELIST, LV.LVSIL_NORMAL, SystemImageList.LargeIcons);
					if (hRes != 0)
						Marshal.ThrowExceptionForHR(hRes);
				}
				catch (Exception ex)
				{
					((FolderView) this.Parent).HandleInitializationException(ex);
				}
			}

			protected override CreateParams CreateParams
			{
				get
				{
					CreateParams cp = base.CreateParams;
					cp.Style = cp.Style | LV.LVS_SHAREIMAGELISTS;
					return cp;
				}
			}

			protected override void OnColumnClick(ColumnClickEventArgs e)
			{
				base.OnColumnClick(e);
				switch (this.Columns[e.Column].Name)
				{
					case COLUMN_NAME:
						this.Sort(SortKey.Name);
						break;
					case COLUMN_SIZE:
						this.Sort(SortKey.Size);
						break;
					case COLUMN_TYPE:
						this.Sort(SortKey.Type);
						break;
					case COLUMN_DATEMODIFIED:
						this.Sort(SortKey.Date);
						break;
				}
			}

			protected override void OnDoubleClick(EventArgs e)
			{
				if (this.SelectedItems != null && this.SelectedItems.Count > 0)
				{
					try
					{
						Point point = this.PointToClient(Cursor.Position);
						FolderListViewItem item = (FolderListViewItem) this.GetItemAt(point.X, point.Y);
						if (item != null)
						{
							var pidl = item.Pidl.Clone();
							try
							{
								if (DereferenceLinks && pidl.IsLink)
								{
									// attempt to resolve links first if necessary
									string resolvedPath;
									Pidl resolvedPidl;
									if (ShellItem.TryResolveLink(Handle, pidl.Path, out resolvedPath) && Pidl.TryParse(resolvedPath, out resolvedPidl))
									{
										pidl.Dispose();
										pidl = resolvedPidl;
									}
								}

								var handled = false;

								var control = Parent as FolderView;
								if (control != null)
								{
									var args = new FolderViewItemEventArgs(new FolderViewItem(pidl));
									control.OnItemDoubleClick(args);
									handled = args.Handled;
								}

								if (AutoDrillDown && pidl.IsFolder && !handled)
								{
									// if the user double clicked on a folder item, perform a drill down
									Browse(new ShellItem(pidl, _rootShellItem, false));
								}
							}
							finally
							{
								pidl.Dispose();
							}
						}
					}
					catch (Exception ex)
					{
						HandleBrowseException(ex);
					}
				}
				base.OnDoubleClick(e);
			}

			protected override void OnKeyDown(KeyEventArgs e)
			{
				if (e.KeyCode == Keys.Enter)
				{
					if (this.SelectedItems != null && this.SelectedItems.Count > 0)
					{
						try
						{
							FolderListViewItem item = (FolderListViewItem) this.FocusedItem;
							if (item != null)
							{
								var pidl = item.Pidl.Clone();
								try
								{
									if (DereferenceLinks && pidl.IsLink)
									{
										// attempt to resolve links first if necessary
										string resolvedPath;
										Pidl resolvedPidl;
										if (ShellItem.TryResolveLink(Handle, pidl.Path, out resolvedPath) && Pidl.TryParse(resolvedPath, out resolvedPidl))
										{
											pidl.Dispose();
											pidl = resolvedPidl;
										}
									}

									var handled = false;

									var control = Parent as FolderView;
									if (control != null)
									{
										var args = new FolderViewItemEventArgs(new FolderViewItem(pidl));
										control.OnItemKeyEnterPressed(args);
										handled = args.Handled;
									}

									if (AutoDrillDown && pidl.IsFolder && !handled)
									{
										// if the user pressed ENTER on a folder item, perform a drill down
										Browse(new ShellItem(pidl, _rootShellItem, false));
									}
								}
								finally
								{
									pidl.Dispose();
								}
							}
						}
						catch (Exception ex)
						{
							HandleBrowseException(ex);
						}
					}

					e.Handled = true;
					e.SuppressKeyPress = true;
				}
				base.OnKeyDown(e);
			}

			protected override bool IsInputKey(Keys keyData)
			{
				if (keyData == Keys.Enter)
					return true;
				return base.IsInputKey(keyData);
			}

			public Pidl CurrentPidl
			{
				get
				{
					if (_currentShellItem == null)
						return null;
					return _currentShellItem.Pidl;
				}
			}

			public bool AutoDrillDown
			{
				get { return _autoDrillDown; }
				set { _autoDrillDown = value; }
			}

			public bool AutoWaitCursor
			{
				get { return _autoWaitCursor; }
				set { _autoWaitCursor = value; }
			}

			public bool DereferenceLinks { get; set; }

			public FileSizeFormat FileSizeFormat
			{
				get { return _fileSizeFormat; }
				set { _fileSizeFormat = value; }
			}

			public new View View
			{
				get { return base.View; }
				set
				{
					if (base.View != value)
					{
						this.SuspendLayout();
						try
						{
							if (base.View == View.Details)
								base.Columns.Clear();

							base.View = value;

							if (value == View.Details)
								base.Columns.AddRange(CreateDetailsViewColumns());
						}
						finally
						{
							this.ResumeLayout(true);
						}
					}
				}
			}

			private void BeginWaitCursor()
			{
				if (_autoWaitCursor)
				{
					_oldCursor = this.Cursor;
					this.Cursor = Cursors.WaitCursor;
				}
			}

			private void EndWaitCursor()
			{
				if (_oldCursor != null)
				{
					this.Cursor = _oldCursor;
					_oldCursor = null;
				}
			}

			public void Sort(SortKey sortKey)
			{
				((FolderListViewItemComparer) this.ListViewItemSorter).SortOn(sortKey);
				this.Sort();
			}

			public void BrowseToPidl(Pidl pidl)
			{
				_suppressBrowseEvents = true;
				try
				{
					this.Browse(new ShellItem(pidl, _rootShellItem, false));
				}
				catch (Exception ex)
				{
					HandleBrowseException(ex);
				}
				finally
				{
					_suppressBrowseEvents = false;
				}
			}

			private void Browse(ShellItem destination)
			{
				if (_currentShellItem != destination)
				{
					CancelEventArgs e = new CancelEventArgs();
					if (!_suppressBrowseEvents && this.BeforeBrowse != null)
						this.BeforeBrowse.Invoke(this, e);
					if (e.Cancel)
						return;

					this.SuspendLayout();
					try
					{
						DisposeEach(this.Items);
						this.Items.Clear();

						if (_currentShellItem != null)
							_currentShellItem.Dispose();

						_currentShellItem = destination;

						if (_currentShellItem != null)
							this.PopulateItems();
					}
					catch (PathNotFoundException ex)
					{
						HandleBrowseException(ex);
					}
					catch (Exception ex)
					{
						HandleBrowseException(new IOException("The specified path is inaccessible.", ex));
					}
					finally
					{
						this.ResumeLayout(true);
					}

					if (!_suppressBrowseEvents && this.AfterBrowse != null)
						this.AfterBrowse.Invoke(this, EventArgs.Empty);
				}
			}

			private void PopulateItems()
			{
				this.BeginWaitCursor();
				this.BeginUpdate();
				try
				{
					List<FolderListViewItem> items = new List<FolderListViewItem>();
					foreach (Pidl pidl in _currentShellItem.EnumerateChildPidls())
					{
						items.Add(new FolderListViewItem(new Pidl(_currentShellItem.Pidl, pidl), _myDocumentsReferencePidl, _fileSizeFormat));
						pidl.Dispose(); // the enumerator makes relative PIDLs, but the FolderListViewItem needs an absolute one
					}
					this.Items.AddRange(items.ToArray());
				}
				catch (Exception ex)
				{
					HandleBrowseException(ex);
				}
				finally
				{
					this.EndUpdate();
					this.EndWaitCursor();
				}
			}

			private void HandleBrowseException(Exception exception)
			{
				FolderView control = base.Parent as FolderView;
				if (control != null)
					control.HandleBrowseException(exception);
			}

			private static ColumnHeader[] CreateDetailsViewColumns()
			{
				ColumnHeader[] columns = new ColumnHeader[4];

				columns[0] = new ColumnHeader();
				columns[0].Name = COLUMN_NAME;
				columns[0].Text = SR.Name;
				columns[0].Width = -1;

				columns[1] = new ColumnHeader();
				columns[1].Name = COLUMN_SIZE;
				columns[1].Text = SR.Size;
				columns[1].Width = 100;

				columns[2] = new ColumnHeader();
				columns[2].Name = COLUMN_TYPE;
				columns[2].Text = SR.Type;
				columns[2].Width = 100;

				columns[3] = new ColumnHeader();
				columns[3].Name = COLUMN_DATEMODIFIED;
				columns[3].Text = SR.DateModified;
				columns[3].Width = 100;

				return columns;
			}

			private class FolderListViewItemComparer : IComparer
			{
				private readonly List<SortKey> _sortKeys = new List<SortKey>(4);
				private readonly Dictionary<SortKey, bool> _invertOrder = new Dictionary<SortKey, bool>();
				private bool _invertClassOrder = false;

				public FolderListViewItemComparer()
				{
					_sortKeys.Add(SortKey.Name);
					_invertOrder[SortKey.Name] = false;
					_invertOrder[SortKey.Date] = false;
					_invertOrder[SortKey.Size] = false;
					_invertOrder[SortKey.Type] = false;
				}

				public void SortOn(SortKey key)
				{
					if (_sortKeys.Remove(key))
						_invertOrder[key] = _invertClassOrder = !_invertOrder[key];
					_sortKeys.Insert(0, key);
				}

				public int Compare(object x, object y)
				{
					FolderListViewItem itemX = (FolderListViewItem) x;
					FolderListViewItem itemY = (FolderListViewItem) y;

					int result = itemX.ItemClass.CompareTo(itemY.ItemClass);
					if (result != 0)
						return result*(_invertClassOrder ? -1 : 1);

					foreach (SortKey key in _sortKeys)
					{
						result = key.Compare(itemX, itemY);
						if (result != 0)
							return result*(_invertOrder[key] ? -1 : 1);
					}
					return 0;
				}
			}
		}

		#endregion

		#region SortKey

		public sealed class SortKey
		{
			public static readonly SortKey Name = new SortKey((x, y) => string.Compare(x.DisplayName, y.DisplayName, StringComparison.InvariantCultureIgnoreCase));
			public static readonly SortKey Type = new SortKey((x, y) => string.Compare(x.TypeName, y.TypeName, StringComparison.InvariantCultureIgnoreCase));
			public static readonly SortKey Date = new SortKey((x, y) => DateTime.Compare(x.LastModified, y.LastModified));
			public static readonly SortKey Size = new SortKey((x, y) => x.FileSize.CompareTo(y.FileSize));

			private readonly Comparison<FolderListViewItem> _comparison;

			private SortKey(Comparison<FolderListViewItem> comparison)
			{
				_comparison = comparison;
			}

			internal int Compare(ListViewItem x, ListViewItem y)
			{
				return _comparison((FolderListViewItem) x, (FolderListViewItem) y);
			}
		}

		#endregion
	}

	public delegate void FolderViewItemEventHandler(object sender, FolderViewItemEventArgs e);

	public class FolderViewItemEventArgs : EventArgs
	{
		private readonly FolderView.FolderViewItem _item;

		public FolderViewItemEventArgs(FolderView.FolderViewItem item)
		{
			_item = item;
		}

		public FolderView.FolderViewItem Item
		{
			get { return _item; }
		}

		public bool Handled { get; set; }
	}
}
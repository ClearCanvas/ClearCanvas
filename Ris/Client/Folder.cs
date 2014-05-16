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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Specifies a folder's default path within its folder system.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class FolderPathAttribute : Attribute
	{
		private readonly string _path;
		private readonly bool _startExpanded;

		public FolderPathAttribute(string path)
		{
			_path = path;
		}

		public FolderPathAttribute(string path, bool startExpanded)
		{
			_path = path;
			_startExpanded = startExpanded;
		}

		public string Path
		{
			get { return _path; }
		}

		public bool StartExpanded
		{
			get { return _startExpanded; }
		}
	}

	/// <summary>
	/// Specifies a folder's default description.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class FolderDescriptionAttribute : Attribute
	{
		private readonly string _description;

		public FolderDescriptionAttribute(string description)
		{
			_description = description;
		}

		/// <summary>
		/// Gets the description, which may be a resource key.
		/// </summary>
		public string Description
		{
			get { return _description; }
		}
	}

	/// <summary>
	/// Abstract base implementation of <see cref="IFolder"/>.
	/// </summary>
	public abstract class Folder : IFolder
	{
		private event EventHandler _textChanged;
		private event EventHandler _iconChanged;
		private event EventHandler _tooltipChanged;
		private event EventHandler _itemsTableChanging;
		private event EventHandler _itemsTableChanged;
		private event EventHandler _totalItemCountChanged;
		private event EventHandler _updating;
		private event EventHandler _updated;

		private IResourceResolver _resourceResolver;
		private bool _isOpen;

		private IFolderSystem _folderSystem;
		private Path _folderPath;
		private readonly bool _startExpanded;
		private bool _isStatic = true;
		private bool _visible = true;

		private static readonly IconSet _closedIconSet = new IconSet("FolderClosedSmall.png", "FolderClosedMedium.png", "FolderClosedMedium.png");
		private static readonly IconSet _openIconSet = new IconSet("FolderOpenSmall.png", "FolderOpenMedium.png", "FolderOpenMedium.png");
		private static readonly IconSet _closedErrorIconSet = new IconSet("FolderClosedErrorSmall.png", "FolderClosedErrorMedium.png", "FolderClosedErrorMedium.png");
		private static readonly IconSet _openErrorIconSet = new IconSet("FolderOpenErrorSmall.png", "FolderOpenErrorMedium.png", "FolderOpenErrorMedium.png");
		private string _folderTooltip;
		private int _totalItemCount = -1;
		private int _currentPageNumber;

		private TimeSpan _autoInvalidateInterval = TimeSpan.Zero;
		private DateTime _lastUpdateTime = DateTime.MinValue;	// basically "never"
		private bool _isUpdating;

		private bool _inErrorState;

		/// <summary>
		/// Constructor
		/// </summary>
		protected Folder()
		{
			_resourceResolver = new ResourceResolver(this.GetType(), true);
			
			// Initialize folder Path
			var pathAttrib = AttributeUtils.GetAttribute<FolderPathAttribute>(this.GetType());
			if (pathAttrib != null)
			{
				_folderPath = new Path(pathAttrib.Path, _resourceResolver);
				_startExpanded = pathAttrib.StartExpanded;
			}

			// Initialize tooltip
			var attrib = AttributeUtils.GetAttribute<FolderDescriptionAttribute>(this.GetType());
			if (attrib != null)
			{
				this.Tooltip = _resourceResolver.LocalizeString(attrib.Description);
			}
		}

		/// <summary>
		/// Constructor.  Values passed to this constructor will override any <see cref="FolderPathAttribute"/> declaration.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="startExpanded"></param>
		protected Folder(Path path, bool startExpanded)
		{
			_resourceResolver = new ResourceResolver(this.GetType(), true);
			_folderPath = path;
			_startExpanded = startExpanded;
		}

		#region IFolder Members

		/// <summary>
		/// Gets the ID that identifies the folder, which by default is the short-name of the class.
		/// </summary>
		public virtual string Id
		{
			get { return this.GetType().Name; }
		}

		/// <summary>
		/// Gets the text that should be displayed for the folder.
		/// </summary>
		/// <remarks>
		/// The default implementation of this property returns the <see cref="Name"/> of the folder,
		/// followed by the <see cref="TotalItemCount"/>, if <see cref="IsItemCountKnown"/> returns true.
		/// </remarks>
		public virtual string Text
		{
			get
			{
				return this.IsItemCountKnown && this.DisplayCount ?
					string.Format("{0} ({1})", this.Name, this.TotalItemCount) : this.Name;
			}
		}

		/// <summary>
		/// Gets the folder system that owns this folder.
		/// </summary>
		public IFolderSystem FolderSystem
		{
			get { return _folderSystem; }
		}

		/// <summary>
		/// Gets the folder name, which is the last part of the <see cref="FolderPath"/> value.
		/// </summary>
		public string Name
		{
			get { return _folderPath != null ? _folderPath.LastSegment.LocalizedText : string.Empty; }
		}

		/// <summary>
		/// Allows the folder to notify that it's text has changed
		/// </summary>
		public event EventHandler TextChanged
		{
			add { _textChanged += value; }
			remove { _textChanged -= value; }
		}

		/// <summary>
		/// Gets or sets the auto-invalidate time interval for this folder.
		/// A value of <see cref="TimeSpan.Zero"/> means auto-invalidation is effectively disabled.
		/// </summary>
		public TimeSpan AutoInvalidateInterval
		{
			get { return _autoInvalidateInterval; }
			protected set { _autoInvalidateInterval = value; }
		}

		public DateTime LastUpdateTime
		{
			get { return _lastUpdateTime; }
		}

		public void Update()
		{
			if (UpdateCore())
			{
				_lastUpdateTime = Platform.Time;
			}
		}

		public bool IsUpdating
		{
			get { return _isUpdating; }
		}

		public void Invalidate()
		{
			InvalidateCore();
		}

		public void Invalidate(bool resetPage)
		{
			if (resetPage)
				_currentPageNumber = 0;

			InvalidateCore();
		}

		/// <summary>
		/// Opens the folder (i.e. instructs the folder to show its "open" state icon).
		/// </summary>
		public virtual void OpenFolder()
		{
			_isOpen = true;
			NotifyIconChanged();
		}

		/// <summary>
		/// Closes the folder (i.e. instructs the folder to show its "closed" state icon).
		/// </summary>
		public virtual void CloseFolder()
		{
			_isOpen = false;
			NotifyIconChanged();
		}

		/// <summary>
		/// Indicates if the folder should be initially expanded.
		/// </summary>
		public virtual bool StartExpanded
		{
			get { return _startExpanded; }
		}

		/// <summary>
		/// Gets the iconset that should be displayed for the folder.
		/// </summary>
		public IconSet IconSet
		{
			get {
				return InErrorState
				       	? (_isOpen ? this.OpenErrorIconSet : this.ClosedErrorIconSet)
				       	: (_isOpen ? this.OpenIconSet : this.ClosedIconSet);
			}
		}

		/// <summary>
		/// Allows the folder to nofity that its icon has changed.
		/// </summary>
		public event EventHandler IconChanged
		{
			add { _iconChanged += value; }
			remove { _iconChanged -= value; }
		}

		/// <summary>
		/// Gets the resource resolver associated with this folder.
		/// </summary>
		public IResourceResolver ResourceResolver
		{
			get { return _resourceResolver; }
			protected set { _resourceResolver = value; }
		}

		/// <summary>
		/// Gets the tooltip that should be displayed for the folder.
		/// </summary>
		public virtual string Tooltip
		{
			get { return _folderTooltip; }
			protected set
			{
				_folderTooltip = value;
				EventsHelper.Fire(_tooltipChanged, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Allows the folder to notify that its tooltip has changed.
		/// </summary>
		public event EventHandler TooltipChanged
		{
			add { _tooltipChanged += value; }
			remove { _tooltipChanged -= value; }
		}

		/// <summary>
		/// Occurs when refresh is about to begin.
		/// </summary>
		public event EventHandler ItemsTableChanging
		{
			add { _itemsTableChanging += value; }
			remove { _itemsTableChanging -= value; }
		}

		/// <summary>
		/// Occurs when refresh is about to finish.
		/// </summary>
		public event EventHandler ItemsTableChanged
		{
			add { _itemsTableChanged += value; }
			remove { _itemsTableChanged -= value; }
		}

		/// <summary>
		/// Occurs before the folder is about to update.
		/// </summary>
		public event EventHandler Updating
		{
			add { _updating += value; }
			remove { _updating -= value; }
		}

		/// <summary>
		/// Occurs after the folder has finished updating.
		/// </summary>
		public event EventHandler Updated
		{
			add { _updated += value; }
			remove { _updated -= value; }
		}

		/// <summary>
		/// Gets the open/close state of the current folder
		/// </summary>
		public bool IsOpen
		{
			get { return _isOpen; }
			protected set { _isOpen = value; }
		}

		/// <summary>
		/// Asks the folder if it can accept a drop of the specified items
		/// </summary>
		/// <param name="items"></param>
		/// <param name="kind"></param>
		/// <returns></returns>
		public virtual DragDropKind CanAcceptDrop(object[] items, DragDropKind kind)
		{
			return DragDropKind.None;
		}

		/// <summary>
		/// Instructs the folder to accept the specified items
		/// </summary>
		/// <param name="items"></param>
		/// <param name="kind"></param>
		public virtual DragDropKind AcceptDrop(object[] items, DragDropKind kind)
		{
			return DragDropKind.None;
		}

		/// <summary>
		/// Informs the folder that the specified items were dragged from it.  It is up to the implementation
		/// of the folder to determine the appropriate response (e.g. whether the items should be removed or not).
		/// </summary>
		/// <param name="items"></param>
		/// <param name="result">The result of the drag drop operation</param>
		public virtual void DragComplete(object[] items, DragDropKind result)
		{
		}

		/// <summary>
		/// Gets a table of the items that are contained in this folder
		/// </summary>
		public abstract ITable ItemsTable
		{
			get;
		}

		/// <summary>
		/// Gets whether or not this folder will display count.
		/// </summary>
		public virtual bool DisplayCount
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the total number of items "contained" in this folder, which may be the same
		/// as the number of items displayed in the <see cref="IFolder.ItemsTable"/>, or may be larger
		/// in the event the table is only showing a subset of the total number of items.
		/// </summary>
		public int TotalItemCount
		{
			get { return _totalItemCount; }
			protected set
			{
				if (value != _totalItemCount)
				{
					_totalItemCount = value;
					NotifyTotalItemCountChanged();
					NotifyTextChanged();
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the count of items in the folder is currently known.
		/// </summary>
		/// <remarks>
		/// The default implementation returns true if <see cref="TotalItemCount"/> returns a value
		/// larger than -1.
		/// </remarks>
		protected virtual bool IsItemCountKnown
		{
			get { return _totalItemCount > -1; }
		}


		/// <summary>
		/// Occurs when the value of the <see cref="IFolder.TotalItemCount"/> property changes.
		/// </summary>
		public event EventHandler TotalItemCountChanged
		{
			add { _totalItemCountChanged += value; }
			remove { _totalItemCountChanged -= value; }
		}

		/// <summary>
		/// Gets or sets the folder path which sets up the tree structure
		/// </summary>
		public Path FolderPath
		{
			get { return _folderPath; }
			set { _folderPath = value; }
		}

		/// <summary>
		/// Gets a value indicating whether or not the folder is 'static'.
		/// </summary>
		/// <remarks>
		/// In the context of workflow, folders created via the normal constructor (new Folder(...)) are considered static and are
		/// otherwise they are considered generated if created by Activator.CreateInstance.
		/// </remarks>
		public bool IsStatic
		{
			get { return _isStatic; }
			protected set { _isStatic = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating if the folder should be visible.
		/// </summary>
		public bool Visible
		{
			get { return _visible; }
			set { _visible = value; }
		}

		#endregion

		#region Paging members

		/// <summary>
		/// Gets a value indicating whether the folder supports paging.
		/// </summary>
		public virtual bool SupportsPaging
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the number of items per page.
		/// </summary>
		public virtual int PageSize
		{
			get { return 50; }
		}

		/// <summary>
		/// Gets the current requested page number.
		/// </summary>
		/// <remarks>
		/// It is possible for the page number to be out-of-sync with the actual page being displayed.
		/// In which case, the folder would be marked invalidated.  The folder will not contain updated items 
		/// until the next call to <see cref="Update"/>.
		/// </remarks>
		public int PageNumber
		{
			get { return this.SupportsPaging ? _currentPageNumber : 0; }
		}

		/// <summary>
		/// Gets a value indicating whether there is at least a page after the <see cref="PageNumber"/>.
		/// </summary>
		public bool HasNext
		{
			get
			{
				if (this.PageSize <= 0)
					return false;

				return this.SupportsPaging ? (_currentPageNumber + 1) * this.PageSize < _totalItemCount : false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether there is at least a page before the <see cref="PageNumber"/>
		/// </summary>
		public bool HasPrevious
		{
			get
			{
				if (this.PageSize <= 0)
					return false;

				return this.SupportsPaging ? _currentPageNumber > 0 : false;
			}
		}

		/// <summary>
		/// Make a request to move to the next page.  The folder is then makred as invalidated.
		/// The folder will not contain items of the next page until the next call to <see cref="Update"/>.
		/// </summary>
		public void MoveNextPage()
		{
			if (!this.HasNext)
				return;

			_currentPageNumber++;
			Invalidate();
		}

		/// <summary>
		/// Make a request to move to the previous page.  The folder is then makred as invalidated.
		/// The folder will not contain items of the previous page until the next call to <see cref="Update"/>.
		/// </summary>
		public void MovePreviousPage()
		{
			if (!this.HasPrevious)
				return;

			_currentPageNumber--;
			Invalidate();
		}

		#endregion

		#region Overridable members

		protected abstract void InvalidateCore();

		protected abstract bool UpdateCore();

		/// <summary>
		/// Gets the closed-state <see cref="IconSet"/>.
		/// </summary>
		/// <remarks>
		/// Override this property to provide a custom closed-state icon.
		/// </remarks>
		protected virtual IconSet ClosedIconSet
		{
			get { return _closedIconSet; }
		}

		/// <summary>
		/// Gets the open-state <see cref="IconSet"/>.
		/// </summary>
		/// <remarks>
		/// Override this property to provide a custom closed-state icon.
		/// </remarks>
		protected virtual IconSet OpenIconSet
		{
			get { return _openIconSet; }
		}

		/// <summary>
		/// Gets the closed-error-state <see cref="IconSet"/>.
		/// </summary>
		/// <remarks>
		/// Override this property to provide a custom closed-error-state  icon.
		/// </remarks>
		protected virtual IconSet ClosedErrorIconSet
		{
			get { return _closedErrorIconSet; }
		}

		/// <summary>
		/// Gets the open-error-state <see cref="IconSet"/>.
		/// </summary>
		/// <remarks>
		/// Override this property to provide a custom open-error-state icon.
		/// </remarks>
		protected virtual IconSet OpenErrorIconSet
		{
			get { return _openErrorIconSet; }
		}

		#endregion

		#region Internal API

		internal void SetFolderSystem(IFolderSystem folderSystem)
		{
			_folderSystem = folderSystem;
		}

		#endregion


		#region Protected members

		protected bool InErrorState
		{
			get { return _inErrorState; }
			set
			{
				_inErrorState = value;
				NotifyIconChanged();
			}
		}

		protected void NotifyTextChanged()
		{
			EventsHelper.Fire(_textChanged, this, EventArgs.Empty);
		}

		protected void NotifyIconChanged()
		{
			EventsHelper.Fire(_iconChanged, this, EventArgs.Empty);
		}

		protected void NotifyItemsTableChanging()
		{
			EventsHelper.Fire(_itemsTableChanging, this, EventArgs.Empty);
		}

		protected void NotifyItemsTableChanged()
		{
			EventsHelper.Fire(_itemsTableChanged, this, EventArgs.Empty);
		}

		protected void NotifyTotalItemCountChanged()
		{
			EventsHelper.Fire(_totalItemCountChanged, this, EventArgs.Empty);
		}

		/// <summary>
		/// Sets the <see cref="IsUpdating"/> property to true and raises the <see cref="Updating"/> event.
		/// </summary>
		/// <remarks>
		/// The subclass is responsible for calling this method, typically from within the <see cref="UpdateCore"/>
		/// implementation.
		/// </remarks>
		protected void BeginUpdate()
		{
			_isUpdating = true;
			EventsHelper.Fire(_updating, this, EventArgs.Empty);
		}

		/// <summary>
		/// Sets the <see cref="IsUpdating"/> property to false and raises the <see cref="Updated"/> event.
		/// </summary>
		/// <remarks>
		/// The subclass is responsible for calling this method when an update is complete.
		/// </remarks>
		protected void EndUpdate()
		{
			_isUpdating = false;
			EventsHelper.Fire(_updated, this, EventArgs.Empty);
		}

		#endregion


		/// <summary>
		/// Disposes the <see cref="Folder"/>.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
		}

		#region IDisposable Members

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion
	}
}

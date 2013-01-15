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
using System.ComponentModel;
using System.IO;

namespace ClearCanvas.Controls.WinForms
{
	public sealed class FolderCoordinator : Component, INotifyPropertyChanged
	{
		private event PropertyChangedEventHandler _propertyChanged;
		private event CancelEventHandler _currentPidlChanging;
		private event EventHandler _currentPidlChanged;

		private ShellItem _rootShellItem = new ShellItem();
		private DefaultSearchLocations _defaultSearchLocations = new DefaultSearchLocations();
		private BrowseHistory<Pidl> _browseHistory = new BrowseHistory<Pidl>();
		private List<IFolderCoordinatee> _list = new List<IFolderCoordinatee>();
		private string _currentPhysicalPath = string.Empty;
		private string _currentVirtualPath = string.Empty;
		private string _currentPath = string.Empty;
		private string _currentDisplayName = string.Empty;
		private bool _canBrowseToParent = false;
		private bool _canBrowseToPrevious = false;
		private bool _canBrowseToNext = false;
		private int _countPreviousLocations = 0;
		private int _countNextLocations = 0;

		public FolderCoordinator() : base() {}

		public FolderCoordinator(IContainer container) : this()
		{
			container.Add(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_browseHistory != null)
				{
					_browseHistory.Dispose();
					_browseHistory = null;
				}

				if (_defaultSearchLocations != null)
				{
					_defaultSearchLocations.Dispose();
					_defaultSearchLocations = null;
				}

				if (_list != null)
				{
					// we don't own the coordinatees, so don't dispose them
					_list.Clear();
					_list = null;
				}

				if (_rootShellItem != null)
				{
					_rootShellItem.Dispose();
					_rootShellItem = null;
				}
			}
			base.Dispose(disposing);
		}

		#region Properties

		#region CurrentDisplayName

		public string CurrentDisplayName
		{
			get { return _currentDisplayName; }
		}

		private string UpdateCurrentDisplayName()
		{
			string currentDisplayName = string.Empty;
			if (_browseHistory.Current != null)
			{
				currentDisplayName = _browseHistory.Current.DisplayName;
			}
			if (_currentDisplayName != currentDisplayName)
			{
				_currentDisplayName = currentDisplayName;
				this.OnPropertyChanged(new PropertyChangedEventArgs("CurrentDisplayName"));
			}
			return currentDisplayName;
		}

		#endregion

		#region CurrentPhysicalPath

		public string CurrentPhysicalPath
		{
			get { return _currentPhysicalPath; }
		}

		private string UpdateCurrentPhysicalPath()
		{
			string currentPhysicalPath = string.Empty;
			if (_browseHistory.Current != null)
			{
				currentPhysicalPath = _browseHistory.Current.Path;
			}
			if (_currentPhysicalPath != currentPhysicalPath)
			{
				_currentPhysicalPath = currentPhysicalPath;
				this.OnPropertyChanged(new PropertyChangedEventArgs("CurrentPhysicalPath"));
			}
			return currentPhysicalPath;
		}

		#endregion

		#region CurrentVirtualPath

		public string CurrentVirtualPath
		{
			get { return _currentVirtualPath; }
		}

		private string UpdateCurrentVirtualPath()
		{
			string currentVirtualPath = string.Empty;
			if (_browseHistory.Current != null)
			{
				currentVirtualPath = _browseHistory.Current.VirtualPath;
			}
			if (_currentVirtualPath != currentVirtualPath)
			{
				_currentVirtualPath = currentVirtualPath;
				this.OnPropertyChanged(new PropertyChangedEventArgs("CurrentVirtualPath"));
			}
			return currentVirtualPath;
		}

		#endregion

		#region CurrentPath

		public string CurrentPath
		{
			get { return _currentPath; }
		}

		private string UpdateCurrentPath()
		{
			string currentPath = string.Empty;
			if (_browseHistory.Current != null)
			{
				currentPath = _browseHistory.Current.Path;
				if (string.IsNullOrEmpty(currentPath))
					currentPath = _browseHistory.Current.DisplayName;
			}
			if (_currentPath != currentPath)
			{
				_currentPath = currentPath;
				this.OnPropertyChanged(new PropertyChangedEventArgs("CurrentPath"));
			}
			return currentPath;
		}

		#endregion

		#region CanBrowseToParent

		[Browsable(false)]
		public bool CanBrowseToParent
		{
			get { return _canBrowseToParent; }
		}

		private bool UpdateCanBrowseToParent()
		{
			Pidl current = _browseHistory.Current;
			bool canBrowseToParent = (current != null && !current.IsRoot);
			if (_canBrowseToParent != canBrowseToParent)
			{
				_canBrowseToParent = canBrowseToParent;
				this.OnPropertyChanged(new PropertyChangedEventArgs("CanBrowseToParent"));
			}
			return canBrowseToParent;
		}

		#endregion

		#region CanBrowseToPrevious

		[Browsable(false)]
		public bool CanBrowseToPrevious
		{
			get { return _canBrowseToPrevious; }
		}

		private bool UpdateCanBrowseToPrevious()
		{
			bool value = !_browseHistory.AtBeginning;
			if (_canBrowseToPrevious != value)
			{
				_canBrowseToPrevious = value;
				this.OnPropertyChanged(new PropertyChangedEventArgs("CanBrowseToPrevious"));
			}
			return value;
		}

		#endregion

		#region CanBrowseToNext

		[Browsable(false)]
		public bool CanBrowseToNext
		{
			get { return _canBrowseToNext; }
		}

		private bool UpdateCanBrowseToNext()
		{
			bool value = !_browseHistory.AtEnd;
			if (_canBrowseToNext != value)
			{
				_canBrowseToNext = value;
				this.OnPropertyChanged(new PropertyChangedEventArgs("CanBrowseToNext"));
			}
			return value;
		}

		#endregion

		#region CountPreviousLocations

		[Browsable(false)]
		public int CountPreviousLocations
		{
			get { return _countPreviousLocations; }
		}

		private int UpdateCountPreviousLocations()
		{
			int value = _browseHistory.PastHistoryCount;
			if (_countPreviousLocations != value)
			{
				_countPreviousLocations = value;
				this.OnPropertyChanged(new PropertyChangedEventArgs("CountPreviousLocations"));
			}
			return value;
		}

		#endregion

		#region CountNextLocations

		[Browsable(false)]
		public int CountNextLocations
		{
			get { return _countNextLocations; }
		}

		private int UpdateCountNextLocations()
		{
			int value = _browseHistory.FutureHistoryCount;
			if (_countNextLocations != value)
			{
				_countNextLocations = value;
				this.OnPropertyChanged(new PropertyChangedEventArgs("CountNextLocations"));
			}
			return value;
		}

		#endregion

		[Browsable(false)]
		public Pidl CurrentPidl
		{
			get { return _browseHistory.Current; }
		}

		#endregion

		#region Events

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		private void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (_propertyChanged != null && base.CanRaiseEvents)
				_propertyChanged.Invoke(this, e);
		}

		public event CancelEventHandler CurrentPidlChanging
		{
			add { _currentPidlChanging += value; }
			remove { _currentPidlChanging -= value; }
		}

		private void OnCurrentPidlChanging(CancelEventArgs e)
		{
			if (_currentPidlChanging != null && base.CanRaiseEvents)
				_currentPidlChanging(this, e);
		}

		public event EventHandler CurrentPidlChanged
		{
			add { _currentPidlChanged += value; }
			remove { _currentPidlChanged -= value; }
		}

		private void OnCurrentPidlChanged(EventArgs e)
		{
			if (_currentPidlChanged != null && base.CanRaiseEvents)
				_currentPidlChanged(this, e);
		}

		#endregion

		#region Public Methods

		public void Refresh()
		{
			foreach (IFolderCoordinatee coordinatee in _list)
			{
				coordinatee.Reload();
			}
		}

		public IEnumerable<Pidl> EnumeratePreviousLocations(bool fromEarliest)
		{
			return _browseHistory.EnumeratePastHistory(fromEarliest);
		}

		public IEnumerable<Pidl> EnumerateNextLocations(bool fromFarthest)
		{
			return _browseHistory.EnumerateFutureHistory(fromFarthest);
		}

		public void BrowseTo(Pidl pidl)
		{
			if (pidl == null)
				throw new ArgumentNullException("pidl");

			CancelEventArgs e = new CancelEventArgs();
			this.OnCurrentPidlChanging(e);
			if (e.Cancel)
				return;

			Pidl current = this.BrowseToCore(pidl);
			foreach (IFolderCoordinatee coordinatee in _list)
			{
				coordinatee.BrowseTo(current);
			}
		}

		public void BrowseTo(string path)
		{
			if (string.IsNullOrEmpty(path))
				throw new ArgumentNullException("path");

			if (!this.TryBrowseToPhysicalLocation(path))
			{
				if (!this.TryBrowseToVirtualLocation(path))
				{
					throw new PathNotFoundException(path);
				}
			}
		}

		public void BrowseTo(Environment.SpecialFolder specialFolder)
		{
			using (Pidl pidl = new Pidl(specialFolder))
			{
				this.BrowseTo(pidl);
			}
		}

		public void BrowseTo(int stepsForwardInHistory)
		{
			Pidl current = this.BrowseHistoryCore(stepsForwardInHistory);
			foreach (IFolderCoordinatee coordinatee in _list)
			{
				coordinatee.BrowseTo(current);
			}
		}

		public void BrowseToHome()
		{
			this.BrowseTo(Environment.SpecialFolder.Desktop);
		}

		public void BrowseToPrevious()
		{
			this.BrowseTo(-1);
		}

		public void BrowseToNext()
		{
			this.BrowseTo(1);
		}

		public void BrowseToParent()
		{
			if (_browseHistory.Current != null)
			{
				Pidl parent = _browseHistory.Current.GetParent();
				if (parent != null)
				{
					try
					{
						this.BrowseTo(parent);
					}
					finally
					{
						parent.Dispose();
					}
				}
			}
		}

		public void BrowseToPhysicalLocation(string path)
		{
			if (!string.IsNullOrEmpty(path))
			{
				if (this.TryBrowseToPhysicalLocation(path))
					return;
			}
			throw new PathNotFoundException(path);
		}

		public bool TryBrowseToPhysicalLocation(string path)
		{
			Pidl pidl;
			if (Pidl.TryParse(path, out pidl))
			{
				this.BrowseTo(pidl);
				pidl.Dispose();
				return true;
			}
			return false;
		}

		public void BrowseToVirtualLocation(string path)
		{
			if (!string.IsNullOrEmpty(path))
			{
				if (this.TryBrowseToVirtualLocation(path))
					return;
			}
			throw new PathNotFoundException(path);
		}

		public bool TryBrowseToVirtualLocation(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				this.BrowseTo((Pidl) null);
				return true;
			}

			string vRelativePath;

			// try to figure how the virtual path is rooted - if there is no match, assume desktop as root
			Pidl vPathRootPidl = _defaultSearchLocations.FindRoot(path, out vRelativePath) ?? _rootShellItem.Pidl;
			if (string.IsNullOrEmpty(vRelativePath))
			{
				// no umatched path relative to the root, so we can browse there directly
				// this PIDL is either owned by _rootShellItem or _defaultSearchLocations, so we don't need to dispose of it
				this.BrowseTo(vPathRootPidl);
				return true;
			}

			string vChildComponent;

			// try to find the first matching child of the discovered special path root
			string vFirstComponent = GetPathRoot(vRelativePath, out vChildComponent);
			using (ShellItem vPathRootShellItem = new ShellItem(vPathRootPidl, _rootShellItem, false))
			{
				List<ShellItem> children = new List<ShellItem>(vPathRootShellItem.EnumerateChildren());
				try
				{
					// find the first child whose display name matches the first component of the desired path
					foreach (ShellItem child in children)
					{
						if (string.Equals(child.DisplayName, vFirstComponent, StringComparison.InvariantCultureIgnoreCase))
						{
							// if we have an additional child path component, and this item has a physical path, combine them!
							if (!string.IsNullOrEmpty(vChildComponent) && !string.IsNullOrEmpty(child.Path))
							{
								string fullyQualifiedPath = Path.Combine(child.Path, vChildComponent);
								using (Pidl pidl = Pidl.Parse(fullyQualifiedPath))
								{
									if (pidl == null)
										return false;
									this.BrowseTo(pidl);
									return true;
								}
							}
							this.BrowseTo(child.Pidl);
							return true;
						}
					}
				}
				finally
				{
					// the call to EnumerateChildren() created this items - we must dispose them!
					foreach (ShellItem child in children)
						child.Dispose();
				}
			}

			return false;
		}

		#endregion

		#region Core BrowseTo Method

		/// <remarks>
		/// This method will not update any registered components.
		/// It is for updating the coordinator's own state.
		/// </remarks>
		private Pidl BrowseToCore(Pidl pidl)
		{
			if (pidl != null && !pidl.IsFolder)
			{
				// if for some reason we're trying to browse to a file, use the file's location instead
				using (Pidl parent = pidl.GetParent())
				{
					return this.BrowseToCore(parent);
				}
			}

			if (_browseHistory.Current != pidl)
			{
				Pidl clonePidl = null;
				if (pidl != null)
					clonePidl = pidl.Clone();

				_browseHistory.Go(clonePidl);

				this.UpdateProperties();
				this.OnCurrentPidlChanged(EventArgs.Empty);
			}
			return _browseHistory.Current;
		}

		private Pidl BrowseHistoryCore(int countForward)
		{
			CancelEventArgs e = new CancelEventArgs();
			this.OnCurrentPidlChanging(e);

			// try to go forward or back
			if (!e.Cancel && _browseHistory.Go(countForward))
			{
				this.UpdateProperties();
				this.OnCurrentPidlChanged(EventArgs.Empty);
			}
			return _browseHistory.Current;
		}

		private void UpdateProperties()
		{
			this.UpdateCanBrowseToParent();
			this.UpdateCanBrowseToPrevious();
			this.UpdateCanBrowseToNext();
			this.UpdateCountPreviousLocations();
			this.UpdateCountNextLocations();
			this.UpdateCurrentDisplayName();
			this.UpdateCurrentPhysicalPath();
			this.UpdateCurrentVirtualPath();
			this.UpdateCurrentPath();
		}

		#endregion

		#region IFolderCoordinatee Event Handlers

		private void OnFolderCoordinateePidlChanging(object sender, CancelEventArgs e)
		{
			CancelEventArgs ce = new CancelEventArgs();
			this.OnCurrentPidlChanging(ce);
			e.Cancel |= ce.Cancel;
		}

		private void OnFolderCoordinateePidlChanged(object sender, EventArgs e)
		{
			Pidl pidl = ((IFolderCoordinatee) sender).Pidl;
			this.BrowseToCore(pidl);
			foreach (IFolderCoordinatee coordinatee in _list)
			{
				if (coordinatee != sender)
					coordinatee.BrowseTo(pidl);
			}
		}

		#endregion

		private static string GetPathRoot(string path, out string relativePath)
		{
			relativePath = string.Empty;
			string[] pathParts = path.Split(new char[] {Path.DirectorySeparatorChar}, 2, StringSplitOptions.RemoveEmptyEntries);
			if (pathParts.Length == 0)
				return string.Empty;
			if (pathParts.Length > 1)
				relativePath = pathParts[1];
			return pathParts[0];
		}

		#region IFolderCoordinatee Registration

		internal bool RegisterCoordinatee(IFolderCoordinatee coordinatee)
		{
			bool result = !_list.Contains(coordinatee);
			if (result)
			{
				coordinatee.PidlChanging += OnFolderCoordinateePidlChanging;
				coordinatee.PidlChanged += OnFolderCoordinateePidlChanged;
				_list.Add(coordinatee);
			}
			return result;
		}

		internal bool UnregisterCoordinatee(IFolderCoordinatee coordinatee)
		{
			bool result = _list.Remove(coordinatee);
			if (result)
			{
				coordinatee.PidlChanged -= OnFolderCoordinateePidlChanged;
				coordinatee.PidlChanging -= OnFolderCoordinateePidlChanging;
			}
			return result;
		}

		#endregion

		#region DefaultSearchLocations Class

		private class DefaultSearchLocations : IDisposable
		{
			private Dictionary<string, Pidl> _dictionary;

			public DefaultSearchLocations()
			{
				_dictionary = new Dictionary<string, Pidl>(5);
				foreach (Environment.SpecialFolder folder in EnumerateLocations())
				{
					Pidl pidl = null;
					try
					{
						pidl = new Pidl(folder);
						string displayName = pidl.DisplayName;
						if (!string.IsNullOrEmpty(displayName))
						{
							_dictionary.Add(displayName.ToLowerInvariant(), pidl);
							continue;
						}
						pidl.Dispose();
					}
					catch (Exception)
					{
						if (pidl != null)
							pidl.Dispose();
					}
				}
			}

			public void Dispose()
			{
				if (_dictionary != null)
				{
					foreach (Pidl pidl in _dictionary.Values)
						pidl.Dispose();
					_dictionary.Clear();
					_dictionary = null;
				}
			}

			public Pidl this[string displayName]
			{
				get
				{
					displayName = displayName.ToLowerInvariant();
					if (!_dictionary.ContainsKey(displayName))
						return null;
					return _dictionary[displayName];
				}
			}

			public bool Contains(string displayName)
			{
				displayName = displayName.ToLowerInvariant();
				return _dictionary.ContainsKey(displayName);
			}

			public Pidl FindRoot(string virtualPath, out string relativePath)
			{
				string pathRoot = GetPathRoot(virtualPath, out relativePath);
				if (!this.Contains(pathRoot))
				{
					relativePath = virtualPath;
					return null;
				}
				return this[pathRoot];
			}

			private static IEnumerable<Environment.SpecialFolder> EnumerateLocations()
			{
				yield return Environment.SpecialFolder.Desktop;
				yield return Environment.SpecialFolder.MyComputer;
				yield return Environment.SpecialFolder.MyDocuments;
				yield return Environment.SpecialFolder.MyMusic;
				yield return Environment.SpecialFolder.MyPictures;
			}
		}

		#endregion

		#region BrowseHistory Class

		private class BrowseHistory<T> : IDisposable where T : class, IDisposable
		{
			private const int MaxHistorySize = 100;
			private LinkedList<T> _futureHistory = new LinkedList<T>();
			private LinkedList<T> _pastHistory = new LinkedList<T>();
			private LinkedListNode<T> _current = null;

			public void Dispose()
			{
				if (_futureHistory != null)
				{
					Purge(_futureHistory);
					_futureHistory = null;
				}

				if (_pastHistory != null)
				{
					Purge(_pastHistory);
					_pastHistory = null;
				}

				if (_current != null)
				{
					if (_current.Value != null)
						_current.Value.Dispose();
					_current = null;
				}
			}

			public int PastHistoryCount
			{
				get { return _pastHistory.Count; }
			}

			public int FutureHistoryCount
			{
				get { return _futureHistory.Count; }
			}

			public bool AtBeginning
			{
				get { return _pastHistory.Count == 0; }
			}

			public bool AtEnd
			{
				get { return _futureHistory.Count == 0; }
			}

			public T Current
			{
				get
				{
					if (_current == null)
						return null;
					return _current.Value;
				}
			}

			public void Go(T item)
			{
				if (_current != null)
					_pastHistory.AddFirst(_current);

				_current = new LinkedListNode<T>(item);

				// purge the entire future history
				Purge(_futureHistory);

				// trim the past history to be under the MaxHistorySize
				while (_pastHistory.Count > MaxHistorySize)
				{
					LinkedListNode<T> last = _pastHistory.Last;
					_pastHistory.RemoveLast();
					if (last.Value != null)
						last.Value.Dispose();
				}
			}

			public bool Go(int countForward)
			{
				if (countForward < 0 && _pastHistory.Count >= -countForward)
				{
					for (int n = 0; n > countForward; --n)
					{
						_futureHistory.AddFirst(_current);
						_current = _pastHistory.First;
						_pastHistory.RemoveFirst();
					}
					return true;
				}
				else if (countForward > 0 && _futureHistory.Count >= countForward)
				{
					for (int n = 0; n < countForward; n++)
					{
						_pastHistory.AddFirst(_current);
						_current = _futureHistory.First;
						_futureHistory.RemoveFirst();
					}
					return true;
				}
				return false;
			}

			public IEnumerable<T> EnumeratePastHistory(bool fromEarliest)
			{
				return EnumerateLinkedList(_pastHistory, fromEarliest);
			}

			public IEnumerable<T> EnumerateFutureHistory(bool fromFarthest)
			{
				return EnumerateLinkedList(_futureHistory, fromFarthest);
			}

			private static IEnumerable<T> EnumerateLinkedList(LinkedList<T> linkedList, bool fromLast)
			{
				if (fromLast)
				{
					LinkedListNode<T> node = linkedList.Last;
					while (node != null)
					{
						yield return node.Value;
						node = node.Previous;
					}
				}
				else
				{
					LinkedListNode<T> node = linkedList.First;
					while (node != null)
					{
						yield return node.Value;
						node = node.Next;
					}
				}
			}

			private static void Purge(ICollection<T> list)
			{
				foreach (T item in list)
					if (item != null)
						item.Dispose();
				list.Clear();
			}
		}

		#endregion
	}
}
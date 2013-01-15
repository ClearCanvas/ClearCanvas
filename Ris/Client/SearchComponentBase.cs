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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	public abstract class SearchParams
	{
		/// <summary>
		/// Constructor for text-based search.
		/// </summary>
		public SearchParams(string textSearch)
		{
			this.TextSearch = textSearch;
		}

		/// <summary>
		/// Specifies the query text.
		/// </summary>
		public string TextSearch;

		/// <summary>
		/// Specifies that "advanced" mode should be used, in which case the text query is ignored
		/// and the search is based on the content of the <see cref="SearchFields"/> member.
		/// </summary>
		public bool UseAdvancedSearch;
	}

	public abstract class SearchParams<TAdvancedSearchFields> : SearchParams
		where TAdvancedSearchFields : DataContractBase
	{
		protected SearchParams(string textSearch)
			: base(textSearch)
		{
		}

		protected SearchParams(TAdvancedSearchFields searchFields)
			: base(null)
		{
			this.UseAdvancedSearch = true;
			this.SearchFields = searchFields;
		}

		public TAdvancedSearchFields SearchFields;
	}

	/// <summary>
	/// Defines an interface for handling the Search Data
	/// </summary>
	public interface ISearchDataHandler
	{
		bool SearchEnabled { get; }
		SearchParams SearchParams { set; }
		event EventHandler SearchEnabledChanged;
	}

	public class SearchComponentManager
	{
		public class ActiveSearchComponentContext
		{
			public static ActiveSearchComponentContext None = new ActiveSearchComponentContext(null, null);

			private readonly Shelf _shelf;
			private readonly Type _type;

			public ActiveSearchComponentContext(Shelf searchComponentShelf, Type searchComponentType)
			{
				_shelf = searchComponentShelf;
				_type = searchComponentType;
			}

			public Type Type
			{
				get { return _type; }
			}

			public void Close()
			{
				_shelf.Close();
			}
		}

		public static ActiveSearchComponentContext ActiveSearchComponent = ActiveSearchComponentContext.None;

		public static void EnsureProperSearchComponent(IFolderSystem folderSystem)
		{
			Platform.CheckForNullReference(folderSystem, "folderSystem");

			// do nothing if there is no search component open
			if (ActiveSearchComponent == ActiveSearchComponentContext.None)
				return;

			// if the folder system does not support searching, do nothing (the search component shelf will be disabled)
			if (!folderSystem.AdvancedSearchEnabled)
				return;

			// close the open shelf and open the folder system's search component
			if (folderSystem.SearchComponentType != ActiveSearchComponent.Type)
			{
				ActiveSearchComponent.Close();
				folderSystem.LaunchSearchComponent();
			}
		}
	}

	public abstract class SearchComponentBase : ApplicationComponent
	{
		protected class SearchComponentManager<TSearchComponent>
			where TSearchComponent : SearchComponentBase, new()
		{
			private SearchComponentBase _instance;
			private Shelf _searchComponentShelf;
			private ISearchDataHandler _activeSearchHandler;

			public SearchComponentManager()
			{
				_activeSearchHandler = GetActiveWorkspaceSearchHandler();
			}

			public Shelf Launch(IDesktopWindow desktopWindow)
			{
				try
				{
					if (_searchComponentShelf != null)
					{
						_searchComponentShelf.Activate();
					}
					else
					{
						desktopWindow.Workspaces.ItemActivationChanged += Workspaces_ItemActivationChanged;

						_searchComponentShelf = LaunchAsShelf(
							desktopWindow,
							this.Instance,
							SR.TitleSearch,
							SR.TitleSearch,
							ShelfDisplayHint.DockFloat);

						_searchComponentShelf.Closed += delegate
						{
							desktopWindow.Workspaces.ItemActivationChanged -= Workspaces_ItemActivationChanged;
							_searchComponentShelf = null;
							SearchComponentManager.ActiveSearchComponent = SearchComponentManager.ActiveSearchComponentContext.None;
						};

						UpdateDisplay();
					}
				}
				catch (Exception e)
				{
					// cannot start component
					ExceptionHandler.Report(e, desktopWindow);
				}

				SearchComponentManager.ActiveSearchComponent = new SearchComponentManager.ActiveSearchComponentContext(_searchComponentShelf, typeof(TSearchComponent));

				return _searchComponentShelf;
			}

			public SearchComponentBase Instance
			{
				get
				{
					if (_instance == null)
						_instance = new TSearchComponent();

					return _instance;
				}
			}

			public ISearchDataHandler ActiveSearchHandler
			{
				get { return _activeSearchHandler; }
			}

			private void Workspaces_ItemActivationChanged(object sender, ItemEventArgs<Workspace> e)
			{
				UpdateDisplay();
			}

			private ISearchDataHandler GetActiveWorkspaceSearchHandler()
			{
				if (_searchComponentShelf == null)
					return null;

				Workspace activeWorkspace = _searchComponentShelf.DesktopWindow.ActiveWorkspace;
				if (activeWorkspace != null && activeWorkspace.Component is ISearchDataHandler)
					return (ISearchDataHandler)(activeWorkspace.Component);

				return null;
			}

			private void UpdateDisplay()
			{
				if (_activeSearchHandler != null)
					_activeSearchHandler.SearchEnabledChanged -= OnSearchEnabledChanged;

				_activeSearchHandler = GetActiveWorkspaceSearchHandler();
				if (_activeSearchHandler != null)
					_activeSearchHandler.SearchEnabledChanged += OnSearchEnabledChanged;

				Instance.NotifyPropertyChanged("ComponentEnabled");
				Instance.NotifyPropertyChanged("SearchEnabled");
			}

			private void OnSearchEnabledChanged(object sender, EventArgs e)
			{
				Instance.NotifyPropertyChanged("ComponentEnabled");
				Instance.NotifyPropertyChanged("SearchEnabled");
			}
		}

		protected SearchParams _searchParams;
		private bool _keepOpen;

		protected SearchComponentBase(SearchParams searchParams)
		{
			_searchParams = searchParams;
		}

        public override void Start()
        {
            _keepOpen = SearchComponentSettings.Default.KeepSearchComponentOpen;

            base.Start();
        }

		public bool KeepOpen
		{
			get { return _keepOpen; }
			set
			{
			    _keepOpen = value;
                SearchComponentSettings.Default.KeepSearchComponentOpen = _keepOpen;
                SearchComponentSettings.Default.Save();
			}
		}

		public bool SearchEnabled
		{
			get { return this.ComponentEnabled && this.HasNonEmptyFields; }
		}

		public abstract bool HasNonEmptyFields { get; }

		public bool ComponentEnabled
		{
			get { return this.ActiveSearchHandler == null ? false : this.ActiveSearchHandler.SearchEnabled; }
		}

		protected abstract ISearchDataHandler ActiveSearchHandler { get; }

		public void Search()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			if (this.ActiveSearchHandler != null)
				this.ActiveSearchHandler.SearchParams = _searchParams; ;

			// always turn the validation errors off after a successful search
			this.ShowValidation(false);

			if (!_keepOpen)
			{
				this.Host.Exit();
			}
		}

		public abstract void Clear();

	}
}
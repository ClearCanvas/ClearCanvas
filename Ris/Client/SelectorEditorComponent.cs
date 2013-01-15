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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="SelectorEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class SelectorEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	public abstract class SelectorEditorComponent : ApplicationComponent
	{
		private readonly bool _isReadOnly;
		private event EventHandler _itemsAdded;
		private event EventHandler _itemsRemoved;

		protected SelectorEditorComponent(bool isReadOnly)
		{
			_isReadOnly = isReadOnly;
		}

		public bool IsReadOnly
		{
			get { return _isReadOnly; }
		}

		public abstract ITable AvailableItemsTable { get; }
		public abstract ITable SelectedItemsTable { get; }

		public event EventHandler ItemsAdded
		{
			add { _itemsAdded += value; }
			remove { _itemsAdded -= value; }
		}

		public event EventHandler ItemsRemoved
		{
			add { _itemsRemoved += value; }
			remove { _itemsRemoved -= value; }
		}

		#region Presentation Model

		public void NotifyItemsAdded()
		{
			this.Modified = true;
			EventsHelper.Fire(_itemsAdded, this, EventArgs.Empty);
		}

		public void NotifyItemsRemoved()
		{
			this.Modified = true;
			EventsHelper.Fire(_itemsRemoved, this, EventArgs.Empty);
		}

		#endregion
	}

	/// <summary>
	/// SelectorEditorComponent class
	/// </summary>
	[AssociateView(typeof(SelectorEditorComponentViewExtensionPoint))]
	public class SelectorEditorComponent<TSummary, TTable> : SelectorEditorComponent
		where TSummary : DataContractBase
		where TTable : Table<TSummary>, new()
	{
		private readonly TTable _available;
		private readonly TTable _selected;

		/// <summary>
		/// Constructor
		/// </summary>
		public SelectorEditorComponent(IEnumerable<TSummary> allItems, IEnumerable<TSummary> selectedItems, Converter<TSummary, EntityRef> identityProvider, bool isReadOnly)
			:base(isReadOnly)
		{
			_available = new TTable();
			_selected = new TTable();

			_selected.Items.AddRange(selectedItems);
			_available.Items.AddRange(Subtract(selectedItems, allItems, identityProvider));
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="allItems"></param>
		/// <param name="selectedItems"></param>
		/// <param name="identityProvider"></param>
		public SelectorEditorComponent(IEnumerable<TSummary> allItems, IEnumerable<TSummary> selectedItems, Converter<TSummary, EntityRef> identityProvider)
			:this(allItems, selectedItems, identityProvider, false)
		{
		}

		/// <summary>
		/// Gets or sets the list of all possible items.
		/// </summary>
		public List<TSummary> AllItems
		{
			get
			{
				var list = new List<TSummary>(_available.Items);
				list.AddRange(_selected.Items);
				return list;
			}
			set
			{
				_available.Items.Clear();
				
				if(value != null)
					_available.Items.AddRange(value);

				// remove any selected items that are no longer valid choices,
				// and any available items that are already selected
				var selectedItems = new List<TSummary>(_selected.Items);
				foreach (var item in selectedItems)
				{
					if (_available.Items.Contains(item))
						_available.Items.Remove(item);
					else
						_selected.Items.Remove(item);
				}
			}
		}

		/// <summary>
		/// Gets the list of selected items.
		/// </summary>
		public virtual IList<TSummary> SelectedItems
		{
			get { return _selected.Items; }
		}

		#region Presentation Model

		public override ITable AvailableItemsTable
		{
			get { return _available; }
		}

		public override ITable SelectedItemsTable
		{
			get { return _selected; }
		}

		#endregion

		private static List<T> Subtract<T>(IEnumerable<T> some, IEnumerable<T> all, Converter<T, EntityRef> identityProvider)
		{
			return CollectionUtils.Reject(all,
				x => CollectionUtils.Contains(some, y => identityProvider(x).Equals(identityProvider(y), true)));
		}
	}
}

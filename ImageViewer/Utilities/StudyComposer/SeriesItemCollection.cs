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

using System.Collections.Generic;
using System.ComponentModel;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.StudyBuilder;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	public sealed class SeriesItemCollection : BindingList<SeriesItem>
	{
		// mappings between nodes and items
		private readonly Dictionary<SeriesNode, SeriesItem> _map = new Dictionary<SeriesNode, SeriesItem>();

		// reference to the underlying list
		private readonly SeriesNodeCollection _list;

		internal SeriesItemCollection(SeriesNodeCollection list)
		{
			_list = list;
			for (int n = 0; n < list.Count; n++)
			{
				base.Insert(n, new SeriesItem(list[n]));
			}
		}

		internal SeriesItem GetByUid(DicomAttributeCollection dataSet)
		{
			SeriesNode node = _list.GetSeriesByUid(dataSet);
			if (!_map.ContainsKey(node))
				base.Insert(_list.IndexOf(node), new SeriesItem(node));
			return _map[node];
		}

		private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			int id = base.IndexOf(sender as SeriesItem);
			if (id >= 0)
				base.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, id));
		}

		protected override object AddNewCore()
		{
			return new SeriesItem(new SeriesNode());
		}

		protected override void ClearItems()
		{
			foreach (SeriesItem item in _map.Values)
			{
				item.PropertyChanged -= Item_PropertyChanged;
			}
			base.ClearItems();
			_map.Clear();
			_list.Clear();
		}

		protected override void InsertItem(int index, SeriesItem item)
		{
			SeriesNode node = item.Node;
			_map.Add(node, item);
			if (!_list.Contains(node)) // this method is also called when initializing the list from the list, so we need to check this to avoid re-adding
				_list.Insert(index, node);

			base.InsertItem(index, item);

			item.PropertyChanged += Item_PropertyChanged;
		}

		protected override void RemoveItem(int index)
		{
			SeriesNode node = base[index].Node;

			_map[node].PropertyChanged -= Item_PropertyChanged;
			_map.Remove(node);
			_list.Remove(node);

			base.RemoveItem(index);
		}

		protected override void SetItem(int index, SeriesItem item)
		{
			SeriesNode oldNode = base[index].Node;
			SeriesNode newNode = item.Node;
			_map.Add(newNode, item);
			_map[oldNode].PropertyChanged -= Item_PropertyChanged;
			_map.Remove(oldNode);
			_list.Remove(oldNode);
			_list.Insert(index, newNode);
			item.PropertyChanged += Item_PropertyChanged;

			base.SetItem(index, item);
		}
	}
}
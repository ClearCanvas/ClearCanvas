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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.StudyBuilder;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	public sealed class ImageItemCollection : BindingList<ImageItem>
	{
		// mappings between nodes and items
		private readonly Dictionary<SopInstanceNode, ImageItem> _map = new Dictionary<SopInstanceNode, ImageItem>();

		// reference to the underlying list
		private readonly SopInstanceNodeCollection _list;

		internal ImageItemCollection(SopInstanceNodeCollection list)
		{
			_list = list;
			for (int n = 0; n < list.Count; n++)
			{
				//TODO: implement some way to render an icon from the dicom attributes instead of needing an ipresentationimage
				//so that we can stick the same UI mode on top of a studybuilder session created programmatically
				//so when we implement that, we can uncomment this line below and allow preinitialization of the collection
				//base.Insert(n, new ImageItem(list[n], null));
			}
		}

		internal ImageItem GetByUid(DicomFile dicomFile, IPresentationImage image)
		{
			SopInstanceNode node = _list.GetImageByUid(dicomFile);
			if (!_map.ContainsKey(node))
				base.Insert(_list.IndexOf(node), new ImageItem(node, image));
			return _map[node];
		}

		private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			int id = base.IndexOf(sender as ImageItem);
			if (id >= 0)
				base.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, id));
		}

		protected override object AddNewCore()
		{
			//TODO: see notes in constructor (and this is why we disable Add New functionality at the image level)
			//return new ImageItem(new SopInstanceNode(), null);
			throw new NotSupportedException();
		}

		protected override void ClearItems()
		{
			foreach (ImageItem item in _map.Values)
			{
				item.PropertyChanged -= Item_PropertyChanged;
			}
			base.ClearItems();
			_map.Clear();
			_list.Clear();
		}

		protected override void InsertItem(int index, ImageItem item)
		{
			SopInstanceNode node = item.Node;
			_map.Add(node, item);
			if (!_list.Contains(node)) // this method is also called when initializing the list from the list, so we need to check this to avoid re-adding
				_list.Insert(index, node);

			base.InsertItem(index, item);

			item.PropertyChanged += Item_PropertyChanged;
		}

		protected override void RemoveItem(int index)
		{
			SopInstanceNode node = base[index].Node;

			_map[node].PropertyChanged -= Item_PropertyChanged;
			_map.Remove(node);
			_list.Remove(node);

			base.RemoveItem(index);
		}

		protected override void SetItem(int index, ImageItem item)
		{
			SopInstanceNode oldNode = base[index].Node;
			SopInstanceNode newNode = item.Node;
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
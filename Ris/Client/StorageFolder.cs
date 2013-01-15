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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
    public class StorageFolder<TItem> : Folder
    {
        private string _folderName;

        private Table<TItem> _itemsTable;

        public StorageFolder(string folderName, Table<TItem> itemsTable)
        {
            _folderName = folderName;
            _itemsTable = itemsTable;

            _itemsTable.Items.ItemsChanged += delegate
                {
                	this.TotalItemCount = _itemsTable.Items.Count;
                };
        }

        protected ItemCollection<TItem> Items
        {
            get { return _itemsTable.Items; }
        }

		protected override bool IsItemCountKnown
		{
			get { return true; }
		}

		protected override bool UpdateCore()
		{
			// do nothing
			return false;
		}

		protected override void InvalidateCore()
		{
			// do nothing
		}

		public override ITable ItemsTable
        {
            get { return _itemsTable; }
        }

        public override DragDropKind CanAcceptDrop(object[] items, DragDropKind kind)
        {
            // return the requested kind if all items are of type TItem, otherwise None
            return CollectionUtils.TrueForAll(items, delegate(object obj) { return obj is TItem; }) ? kind : DragDropKind.None;
        }

        public override DragDropKind AcceptDrop(object[] items, DragDropKind kind)
        {
            if (kind != DragDropKind.None)
            {
                // store any items that are not already in this folder
                foreach (TItem item in items)
                {
                    if (!CollectionUtils.Contains<TItem>(this.Items, delegate(TItem x) { return x.Equals(item); }))
                    {
                        this.Items.Add(item);
                    }
                }
            }

            return kind;
        }

        public override void DragComplete(object[] items, DragDropKind kind)
        {
            // if the operation was a Move, then we should remove the items from this folder
            if (kind == DragDropKind.Move)
            {
                foreach (TItem item in items)
                {
                    this.Items.Remove(item);
                }
            }
        }
    }
}

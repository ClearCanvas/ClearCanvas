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
using System.Web.UI.WebControls;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Web.Common
{
    /// <summary>
    /// Abstract base for a generic collection of object which can be indexed based on a key.
    /// </summary>
    public abstract class KeyedCollectionBase<TData, TKey> : Dictionary<TKey, TData>
    {
        public KeyedCollectionBase()
        {
        }


        public KeyedCollectionBase(IList<TData> list)
        {
            foreach (TData item in list)
            {
                Add(GetKey(item), item);
            }
        }


        protected abstract TKey GetKey(TData item);

        public TData this[int index]
        {
            get
            {
                List<TData> list = new List<TData>(this.Values);
                return list[index];
            }
        }

        public void Add(IList<TData> items)
        {
            foreach (TData item in items)
                Add(item);
        }

        public void Add(TData item)
        {
            TKey key = GetKey(item);

            if (IndexOf(key) > 0)
            {
                Exception e = new Exception(string.Format("Key {0} already exists in list.\n\nItem: {1}\n\nList: {2}", key, item, this));
                Platform.Log(LogLevel.Error, e.Message);
                throw e;
            }
            else
            {
                Add(key, item);
            }
        }

        public int IndexOf(TKey key)
        {
            List<TKey> list = new List<TKey>(Keys);
            return list.IndexOf(key);
        }

        public int IndexOf(TData item)
        {
            List<TData> list = new List<TData>(this.Values);
            return list.IndexOf(item);
        }

        /// <summary>
        /// Return the row index of the item if rendered in the specified grid
        /// </summary>
        /// <param name="key"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public int RowIndexOf(TKey key, GridView grid)
        {
            int index = IndexOf(key);

            if (!grid.AllowPaging)
            {
                return index;
            }
            else
            {
                int curPageMinIndex = grid.PageSize * grid.PageIndex;
                int curPageMaxIndex = curPageMinIndex + grid.PageSize;
                if (index < curPageMinIndex || index > curPageMaxIndex)
                    return -1;
                else
                    return index % grid.PageSize;
            }

        }

        /// <summary>
        /// Return the row index of the item if rendered in the specified grid
        /// </summary>
        /// <param name="item"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public int RowIndexOf(TData item, GridView grid)
        {
            return RowIndexOf(GetKey(item), grid);
        }
    }
}
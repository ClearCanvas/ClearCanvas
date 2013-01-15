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

namespace ClearCanvas.ImageServer.Utilities.CleanupReconcile
{
    public class SortableResultList<T> : BindingList<T>
    {
        private bool _isSorted;
        private ListSortDirection _sortDirection;
        private PropertyDescriptor _sortProperty;

        public SortableResultList(List<T> list)
            :base(list)
        {
            
        }

        protected override bool SupportsSortingCore
        {
            get
            {
                return true;
            }
        }

        protected override bool SupportsSearchingCore
        {
            get { return true; }
        }

        protected override bool IsSortedCore
        {
            get
            {
                return _isSorted;
            }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get
            {
                return _sortDirection;
            }
        }
        protected override PropertyDescriptor SortPropertyCore
        {
            get
            {
                return _sortProperty;
            }
        }

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            _sortDirection = direction;
            _sortProperty = prop;
            
            var list = Items as List<T>;
            list.Sort(delegate(T entry1, T entry2)
                          {
                              object value1 = prop.GetValue(entry1);
                              object value2 = prop.GetValue(entry2);
                              return value1.ToString().CompareTo(value2.ToString());
                          });

            _isSorted = true;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

    }
}
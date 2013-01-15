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
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare
{
    internal static class MinMaxHelper
    {
        internal static TValue MinValue<TItem, TValue>(IEnumerable<TItem> items, Predicate<TItem> itemFilter, Converter<TItem, TValue> valueGetter, TValue nullValue)
        {
            return CollectionUtils.Min(GetSourceValues(items, itemFilter, valueGetter, nullValue), nullValue);
        }

        internal static TValue MaxValue<TItem, TValue>(IEnumerable<TItem> items, Predicate<TItem> itemFilter, Converter<TItem, TValue> valueGetter, TValue nullValue)
        {
            return CollectionUtils.Max(GetSourceValues(items, itemFilter, valueGetter, nullValue), nullValue);
        }


        private static IEnumerable<TValue> GetSourceValues<TItem, TValue>(IEnumerable<TItem> items, Predicate<TItem> itemFilter, Converter<TItem, TValue> valueGetter, TValue nullValue)
        {
            List<TItem> sources = CollectionUtils.Select(items, itemFilter);

            return CollectionUtils.Select(
                CollectionUtils.Map(sources, valueGetter),
                    delegate(TValue value) { return !Equals(value, nullValue); });
        }
    }
}

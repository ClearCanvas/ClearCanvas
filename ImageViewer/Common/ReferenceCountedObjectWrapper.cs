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

using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common
{
	public class ReferenceCountedObjectWrapper
	{
		private readonly object _item;
		private int _referenceCount;

		public ReferenceCountedObjectWrapper(object item)
		{
			Platform.CheckForNullReference(item, "item");
			_item = item;
		}

		public object Item
		{
			get { return _item; }
		}

		public void IncrementReferenceCount()
		{
			Interlocked.Increment(ref _referenceCount);
		}

		public void DecrementReferenceCount()
		{
			Interlocked.Decrement(ref _referenceCount);
		}

		public bool IsReferenceCountAboveZero()
		{
			return 0 < Thread.VolatileRead(ref _referenceCount);
		}

		public int ReferenceCount
		{
			get { return Thread.VolatileRead(ref _referenceCount); }
		}
	}
	
	public class ReferenceCountedObjectWrapper<T> : ReferenceCountedObjectWrapper
	{
		public ReferenceCountedObjectWrapper(T item)
			: base(item)
		{
		}

		public new T Item
		{
			get { return (T)base.Item; }
		}
	}
}
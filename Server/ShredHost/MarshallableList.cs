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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Server.ShredHost
{
    public abstract class MarshallableList<TContainedType> : MarshalByRefObject, IEnumerable<TContainedType>
    {
        public MarshallableList()
        {
            _internalList = new List<TContainedType>();
        }

        protected ReadOnlyCollection<TContainedType> ContainedObjects
        {
            get { return _internalList.AsReadOnly(); }
        }

        public void Add(TContainedType objectToContain)
        {
            _internalList.Add(objectToContain);
        }

        public override object InitializeLifetimeService()
        {
            // I can't find any documentation yet, that says that returning null 
            // means that the lifetime of the object should not expire after a timeout
            // but the initial solution comes from this page: http://www.dotnet247.com/247reference/msgs/13/66416.aspx
            return null;
        }

        public void Clear()
        {
            _internalList.Clear();
        }

        #region Properties
        public int Count
        {
            get { return _internalList.Count; }
        }
	
        #endregion

        #region Private Members
        List<TContainedType> _internalList;
        #endregion

        #region IEnumerable<TContainedType> Members

        public IEnumerator<TContainedType> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        #endregion

    }
}

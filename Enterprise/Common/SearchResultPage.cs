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
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Provides a mechanism for requesting a "page" of search results from a persistent store.
    /// </summary>
    [DataContract]
    public class SearchResultPage
    {
        private int _firstRow;
        private int _maxRows;

        /// <summary>
        /// Constructor
        /// </summary>
        public SearchResultPage()
        {
            _firstRow = -1;  // ignore
            _maxRows = -1; // ignore
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="firstRow">The first row to return</param>
        /// <param name="maxRows">The maximum number of rows to return</param>
        public SearchResultPage(int firstRow, int maxRows)
        {
            _firstRow = firstRow;
            _maxRows = maxRows;
        }

        /// <summary>
        /// The first row to return.
        /// </summary>
        [DataMember]
        public int FirstRow
        {
            get { return _firstRow; }
            set { _firstRow = value; }
        }

        /// <summary>
        /// The maximum number of rows to return.  A value of -1 can be used to indicate that all rows should
        /// be returned.  This feature should be used with caution however.
        /// </summary>
        [DataMember]
        public int MaxRows
        {
            get { return _maxRows; }
            set { _maxRows = value; }
        }
    }
}

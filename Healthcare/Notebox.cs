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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Abstract base implementation of <see cref="INotebox"/>.
    /// </summary>
    public abstract class Notebox : INotebox
    {
        /// <summary>
        /// Queries the notebox for its contents.
        /// </summary>
        /// <param name="nqc"></param>
        /// <returns></returns>
        public abstract IList GetItems(INoteboxQueryContext nqc);

        /// <summary>
        /// Queries the notebox for a count of its contents.
        /// </summary>
        /// <param name="nqc"></param>
        /// <returns></returns>
        public abstract int GetItemCount(INoteboxQueryContext nqc);

        /// <summary>
        /// Gets the invariant criteria for this class of notebox.
        /// </summary>
        /// <param name="wqc"></param>
        /// <returns></returns>
        public abstract NoteboxItemSearchCriteria[] GetInvariantCriteria(INoteboxQueryContext wqc);
    }
}

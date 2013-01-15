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

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Defines an interface for handling drag and drop operations
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface IDropHandler<TItem>
    {
        /// <summary>
        /// Asks the handler if it can accept the specified items.  This value is used to provide visual feedback
        /// to the user to indicate that a drop is possible.
        /// </summary>
        bool CanAcceptDrop(ICollection<TItem> items);

        /// <summary>
        /// Asks the handler to process the specified items, and returns true if the items were successfully processed.
        /// </summary>
        bool ProcessDrop(ICollection<TItem> items);
    }
}

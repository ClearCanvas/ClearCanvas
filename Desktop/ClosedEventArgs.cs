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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Provides data for Closed events.
    /// </summary>
    public class ClosedEventArgs : EventArgs
    {
        private CloseReason _reason;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal ClosedEventArgs(CloseReason reason)
        {
            _reason = reason;
        }

        /// <summary>
        /// Gets the reason that the object was closed.
        /// </summary>
        public CloseReason Reason
        {
            get { return _reason; }
        }
    }

    /// <summary>
    /// Provides data for ItemClosed events.
    /// </summary>
    /// <typeparam name="TItem">Type of the item that was closed.</typeparam>
    public class ClosedItemEventArgs<TItem> : ItemEventArgs<TItem>
    {
        private CloseReason _reason;

        /// <summary>
        /// Constructor.
        /// </summary>
		internal ClosedItemEventArgs(TItem item, CloseReason reason)
            :base(item)
        {
            _reason = reason;
        }

        /// <summary>
        /// Gets the reason that the item was closed.
        /// </summary>
        public CloseReason Reason
        {
            get { return _reason; }
        }
    }
}

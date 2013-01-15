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
using System.Collections;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Abstract base class for order-noteboxes.
    /// </summary>
    public abstract class OrderNotebox : Notebox
    {
        /// <summary>
        /// Helper method to get a broker.
        /// </summary>
        /// <param name="nqc"></param>
        /// <returns></returns>
        protected IOrderNoteboxItemBroker GetBroker(INoteboxQueryContext nqc)
        {
            return nqc.GetBroker<IOrderNoteboxItemBroker>();
        }
    }

    /// <summary>
    /// Defines the order-note "Inbox".
    /// </summary>
    [ExtensionOf(typeof(NoteboxExtensionPoint))]
    public class OrderNotePersonalInbox : OrderNotebox
    {
        public override NoteboxItemSearchCriteria[] GetInvariantCriteria(INoteboxQueryContext wqc)
        {
            NoteboxItemSearchCriteria where = new NoteboxItemSearchCriteria();
            where.IsAcknowledged = false;
            where.SentToMe = true;

            return new NoteboxItemSearchCriteria[]{ where };
        }

        public override IList GetItems(INoteboxQueryContext nqc)
        {
            return GetBroker(nqc).GetInboxItems(this, nqc);
        }

        public override int GetItemCount(INoteboxQueryContext nqc)
        {
            return GetBroker(nqc).CountInboxItems(this, nqc);
        }
    }

	/// <summary>
	/// Defines the order-note "Inbox".
	/// </summary>
	[ExtensionOf(typeof(NoteboxExtensionPoint))]
	public class OrderNoteGroupInbox : OrderNotebox
	{
		public override NoteboxItemSearchCriteria[] GetInvariantCriteria(INoteboxQueryContext wqc)
		{
			NoteboxItemSearchCriteria where = new NoteboxItemSearchCriteria();
			where.IsAcknowledged = false;
			where.SentToGroupIncludingMe = true;

			return new NoteboxItemSearchCriteria[] { where };
		}

		public override IList GetItems(INoteboxQueryContext nqc)
		{
			return GetBroker(nqc).GetInboxItems(this, nqc);
		}

		public override int GetItemCount(INoteboxQueryContext nqc)
		{
			return GetBroker(nqc).CountInboxItems(this, nqc);
		}
	}

    /// <summary>
    /// Defines the order-note "Sent Items" box.
    /// </summary>
    [ExtensionOf(typeof(NoteboxExtensionPoint))]
    public class OrderNoteSentItems : OrderNotebox
    {
        public override NoteboxItemSearchCriteria[] GetInvariantCriteria(INoteboxQueryContext wqc)
        {
            NoteboxItemSearchCriteria where = new NoteboxItemSearchCriteria();
            where.IsAcknowledged = false;
            where.SentByMe = true;

            return new NoteboxItemSearchCriteria[] { where };
        }

        public override IList GetItems(INoteboxQueryContext nqc)
        {
            return GetBroker(nqc).GetSentItems(this, nqc);
        }

        public override int GetItemCount(INoteboxQueryContext nqc)
        {
            return GetBroker(nqc).CountSentItems(this, nqc);
        }
    }
}

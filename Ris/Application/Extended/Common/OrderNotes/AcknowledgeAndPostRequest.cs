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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Extended.Common.OrderNotes
{
    [DataContract]
    public class AcknowledgeAndPostRequest : DataContractBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderRef"></param>
        /// <param name="orderNotesToAcknowledge"></param>
        /// <param name="orderNote"></param>
        public AcknowledgeAndPostRequest(EntityRef orderRef, List<EntityRef> orderNotesToAcknowledge, OrderNoteDetail orderNote)
        {
            OrderRef = orderRef;
            OrderNotesToAcknowledge = orderNotesToAcknowledge;
            OrderNote = orderNote;
        }

        /// <summary>
        /// Specifies the order to which this request pertains.
        /// </summary>
        [DataMember]
        public EntityRef OrderRef;

        /// <summary>
        /// Specifies a list of order notes to acknowledge prior replying. Optional.
        /// </summary>
        [DataMember]
        public List<EntityRef> OrderNotesToAcknowledge;

        /// <summary>
        /// Specifies a reply order note. Optional.
        /// </summary>
        [DataMember]
        public OrderNoteDetail OrderNote;

    }
}

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
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// 
    /// </summary>
    // Because the notebox system is so simple, there is no point in
    // using the standard ISearchCondition/SearchCriteria pattern here...
    // A few boolean flags will suffice for all current use cases.  As the
    // system evolves, it is expected that this may need to be refactored
    // to be more similar to the typical SearchCriteria classes.
    public class NoteboxItemSearchCriteria : SearchCriteria
    {
        private bool _sentToMe;
        private bool _sentToGroupIncludingMe;
        private bool _sentByMe;
        private bool _isAcknowledged;

        /// <summary>
        /// Constructor.
        /// </summary>
        public NoteboxItemSearchCriteria()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected NoteboxItemSearchCriteria(NoteboxItemSearchCriteria other)
            : base(other)
        {
        }

        public override object Clone()
        {
            return new NoteboxItemSearchCriteria(this);
        }

        public bool SentToMe
        {
            get { return _sentToMe; }
            set { _sentToMe = value; }
        }

        public bool SentToGroupIncludingMe
        {
            get { return _sentToGroupIncludingMe; }
            set { _sentToGroupIncludingMe = value; }
        }

        public bool SentByMe
        {
            get { return _sentByMe; }
            set { _sentByMe = value; }
        }

        public bool IsAcknowledged
        {
            get { return _isAcknowledged; }
            set { _isAcknowledged = value; }
        }
    }
}

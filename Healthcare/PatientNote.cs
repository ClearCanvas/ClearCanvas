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
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// PatientNote
    /// </summary>
    public partial class PatientNote
	{
        /// <summary>
        /// Constructor for creating a new patient note.
        /// </summary>
        /// <param name="author"></param>
        /// <param name="category"></param>
        /// <param name="comment"></param>
        public PatientNote(Staff author, PatientNoteCategory category, string comment)
        {
            _author = author;
            _category = category;
            _comment = comment;

            // valid from now indefinitely
            _creationTime = Platform.Time;
            _validRange = new DateTimeRange(_creationTime, null);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PatientNote"/> is currently valid.
        /// </summary>
        public bool IsCurrent
        {
            get { return this.ValidRange == null || this.ValidRange.Includes(Platform.Time); }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PatientNote"/> has expired.
        /// </summary>
        public bool IsExpired
        {
            get { return _validRange != null && _validRange.Until < Platform.Time; }
        }

        private void CustomInitialize()
        {
        }
    }
}
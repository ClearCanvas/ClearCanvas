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

namespace ClearCanvas.ImageServer.Enterprise
{
    /// <summary>
    /// Generic base class for update parameter classes used in a non-procedural update broker implementing the <see cref="IUpdateBroker"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of the field to be updated</typeparam>
    public class EntityUpdateColumn<T> : EntityColumnBase
    {      
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fieldName">The update column name.</param>
        /// <param name="value">The value to update.</param>
        public EntityUpdateColumn(String fieldName, T value)
            : base(fieldName)
        {
            _value = value;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// The value of the column to update.
        /// </summary>
        public new T Value
        {
            get { return (T) _value; }
        }

        #endregion Public properties
    }
}

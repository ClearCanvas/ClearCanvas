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
    /// Abstract base class for doing inserts and updates with the
    /// <see cref="IEntityBroker{TServerEntity,TSelectCriteria,TUpdateColumns}"/> interface.
    /// </summary>
    public abstract class EntityColumnBase
    {
        #region Protected Members

        protected string _fieldName;
        protected object _value;

        #endregion  Protected Members

        #region Public Properties

        /// <summary>
        /// Gets the key corresponding to the parameter/field to be updated.
        /// </summary>
        public String FieldName
        {
            get { return _fieldName; }
        }

        /// <summary>
        /// Gets the value of the parameter/field.
        /// </summary>
        public object Value
        {
            get { return _value; }
        }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fieldName">The column name.</param>
        protected EntityColumnBase(String fieldName)
        {
            _fieldName = fieldName;
        }

        #endregion Constructors
    }
}

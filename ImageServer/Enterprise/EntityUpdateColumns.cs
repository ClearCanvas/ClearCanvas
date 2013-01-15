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

namespace ClearCanvas.ImageServer.Enterprise
{
    /// <summary>
    /// Abstract base class to store a collection of update columns to be used in in a 
    /// non-procedural broker implementing the 
    /// <see cref="IEntityBroker{TServerEntity,TSelectCriteria,TUpdateColumns}"/> interface.
    /// </summary>
    /// <remark>
    /// Each updatable field in the parameter collection is an element of <see cref="SubParameters"/>.
    /// </remark>
    abstract public class EntityUpdateColumns
    {
        #region Private Members

        private readonly string _entityName;       
        private readonly Dictionary<string, EntityColumnBase> _subParameters = new Dictionary<string, EntityColumnBase>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entityName">The name of the <see cref="ServerEntity"/>.</param>
        public EntityUpdateColumns(string entityName)
        {
            _entityName = entityName;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Returns the list of sub-parameters
        /// </summary>
        public IDictionary<string, EntityColumnBase> SubParameters
        {
            get { return _subParameters; }
        }
        
        public virtual bool IsEmpty
        {
            get
            {
                if (_subParameters.Values.Count > 0)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Gets the key corresponding to the parameter/field to be updated.
        /// </summary>
        public String FieldName
        {
            get { return _entityName; }
        }

        #endregion
    }
}

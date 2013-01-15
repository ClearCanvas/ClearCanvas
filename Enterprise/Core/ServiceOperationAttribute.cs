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

namespace ClearCanvas.Enterprise.Core
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class ServiceOperationAttribute : Attribute
    {
        private PersistenceScopeOption _scopeOption;
        private bool _changeSetAuditable = true;
        
        public ServiceOperationAttribute()
        {
            // a persistence context is required, by default
            _scopeOption = PersistenceScopeOption.Required;
        }

		/// <summary>
		/// Gets or sets a value indicating whether change-set auditing is applied to this operation.
		/// Does not affect other levels of auditing.
		/// </summary>
        public bool ChangeSetAuditable
        {
            get { return _changeSetAuditable; }
            set { _changeSetAuditable = value; }
        }

		/// <summary>
		/// Gets or sets the <see cref="PersistenceScopeOption"/> to apply to this operation.
		/// </summary>
        public PersistenceScopeOption PersistenceScopeOption
        {
            get { return _scopeOption; }
            set { _scopeOption = value; }
        }

        public abstract PersistenceScope CreatePersistenceScope();
   }
}

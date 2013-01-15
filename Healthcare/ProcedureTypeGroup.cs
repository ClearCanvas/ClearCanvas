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

using ClearCanvas.Enterprise.Core;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// ProcedureTypeGroup entity
    /// </summary>
	public partial class ProcedureTypeGroup : ClearCanvas.Enterprise.Core.Entity
	{
        /// <summary>
        /// Returns all concrete subclasses of this class.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IList<Type> ListSubClasses(IPersistenceContext context)
        {
            return CollectionUtils.Select(context.GetBroker<IMetadataBroker>().ListEntityClasses(),
                delegate(Type t) { return !t.IsAbstract && t.IsSubclassOf(typeof(ProcedureTypeGroup)); });
        }

        /// <summary>
        /// Gets the concrete subclass matching the specified name, which need not be fully qualified.
        /// </summary>
        /// <param name="subclassName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Type GetSubClass(string subclassName, IPersistenceContext context)
        {
            return CollectionUtils.SelectFirst(ListSubClasses(context),
				delegate(Type t) { return t.FullName.EndsWith(subclassName); });
        }
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}
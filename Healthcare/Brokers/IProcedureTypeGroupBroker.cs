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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Brokers
{
    /// <summary>
    /// Defines the interface for a <see cref="ProcedureTypeGroup"/> broker
    /// </summary>
    public partial interface IProcedureTypeGroupBroker
    {
        /// <summary>
        /// Finds all <see cref="ProcedureTypeGroup"/> instances of the specified sub-class.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="subClass"></param>
        /// <returns></returns>
        IList<ProcedureTypeGroup> Find(ProcedureTypeGroupSearchCriteria criteria, Type subClass);

		/// <summary>
		/// Finds all <see cref="ProcedureTypeGroup"/> instances of the specified sub-class.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="subClass"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		IList<ProcedureTypeGroup> Find(ProcedureTypeGroupSearchCriteria criteria, Type subClass, SearchResultPage page);
		
		/// <summary>
        /// Finds one instance of the specified subclass matching the specified criteria.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="subClass"></param>
        /// <returns></returns>
        ProcedureTypeGroup FindOne(ProcedureTypeGroupSearchCriteria criteria, Type subClass);
    }
}

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

using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Brokers
{
	public interface IWorklistBroker : IEntityBroker<Worklist, WorklistSearchCriteria>
	{
		/// <summary>
		/// Count the number of worklists owned by the specified owner.
		/// </summary>
		/// <param name="owner"></param>
		/// <returns></returns>
		int Count(WorklistOwner owner);

		/// <summary>
		/// Finds worklists assigned to specified staff group.
		/// </summary>
		/// <param name="staffGroup"></param>
		/// <returns></returns>
		IList<Worklist> Find(StaffGroup staffGroup);

		/// <summary>
		/// Finds worklists matching specified class names and assigned to specified staff.
		/// </summary>
		/// <param name="staff"></param>
		/// <param name="worklistClassNames"></param>
		/// <returns></returns>
		IList<Worklist> Find(Staff staff, IEnumerable<string> worklistClassNames);

		/// <summary>
		/// Finds worklists matching the specified name (which may contain wildcards) and class names.
		/// </summary>
		/// <param name="name">If empty, no name criteria is applied.</param>
		/// <param name="includeUserDefinedWorklists"></param>
		/// <param name="worklistClassNames"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		IList<Worklist> Find(string name, bool includeUserDefinedWorklists, IEnumerable<string> worklistClassNames, SearchResultPage page);

		/// <summary>
		/// Finds one worklist with the specified name and class name.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="worklistClassName"></param>
		/// <returns></returns>
		Worklist FindOne(string name, string worklistClassName);
	}
}

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
using System.Data.Linq;
using System.Linq;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{
	public class WorkItemUidBroker : Broker
	{
		internal WorkItemUidBroker(DicomStoreDataContext context)
			: base(context) {}

		/// <summary>
		/// Gets the specified number of pending work items.
		/// </summary>
		/// <param name="workItemOid"></param>
		/// <returns></returns>
		public IList<WorkItemUid> GetWorkItemUidsForWorkItem(long workItemOid)
		{
			return (from w in Context.WorkItemUids
			        where w.WorkItemOid == workItemOid
			        select w).ToList();
		}

		/// <summary>
		/// Gets the specified number of pending work items.
		/// </summary>
		/// <param name="workItemOid"></param>
		/// <param name="seriesInstanceUid"></param>
		/// <param name="sopInstanceUid"> </param>
		/// <returns></returns>
		public bool HasWorkItemUidForSop(long workItemOid, string seriesInstanceUid, string sopInstanceUid)
		{
			return (from w in Context.WorkItemUids
			        where w.WorkItemOid == workItemOid
			              && w.SeriesInstanceUid == seriesInstanceUid
			              && w.SopInstanceUid == sopInstanceUid
			        select w).Count() > 1;
		}

		/// <summary>
		/// Get a specific WorkItemUid
		/// </summary>
		/// <param name="oid"></param>
		/// <returns></returns>
		public WorkItemUid GetWorkItemUid(long oid)
		{
			return _getWorkItemUidByOid(Context, oid).FirstOrDefault();
		}

		/// <summary>
		/// Insert a WorkItemUid
		/// </summary>
		/// <param name="entity"></param>
		public void AddWorkItemUid(WorkItemUid entity)
		{
			Context.WorkItemUids.InsertOnSubmit(entity);
		}

		/// <summary>
		/// Delete WorkItemUid entity.
		/// </summary>
		/// <param name="entity"></param>
		public void Delete(WorkItemUid entity)
		{
			Context.WorkItemUids.DeleteOnSubmit(entity);
		}

		internal void DeleteAll()
		{
			Context.WorkItemUids.DeleteAllOnSubmit(Context.WorkItemUids);
		}

		#region Compiled Queries

		private static readonly Func<DicomStoreDataContext, long, IQueryable<WorkItemUid>> _getWorkItemUidByOid =
			CompiledQuery.Compile<DicomStoreDataContext, long, IQueryable<WorkItemUid>>((context, oid) => (from w in context.WorkItemUids
			                                                                                               where w.Oid == oid
			                                                                                               select w).Take(1));

		#endregion
	}
}
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
	public class StudyBroker : Broker
	{
		internal StudyBroker(DicomStoreDataContext context)
			: base(context) {}

		public void AddStudy(Study study)
		{
			Context.Studies.InsertOnSubmit(study);
		}

		public Study GetStudy(long oid)
		{
			return _getStudyByOid(Context, oid).FirstOrDefault();
		}

		public Study GetStudy(string studyInstanceUid)
		{
			return _getStudyByStudyInstanceUid(Context, studyInstanceUid).FirstOrDefault();
		}

		public long GetStudyCount()
		{
			return _getStudyCount(Context);
		}

		public List<long> GetStudyOids()
		{
			return _getAllStudyOids(Context).ToList();
		}

		public List<Study> GetStudies()
		{
			return _getAllStudies(Context).ToList();
		}

		public List<Study> GetReindexStudies()
		{
			return _getReindexStudies(Context).ToList();
		}

		/// <summary>
		/// Get studies that are eligible for deletion as of the specified time.
		/// </summary>
		/// <param name="now"></param>
		/// <param name="batchSize"></param>
		/// <returns></returns>
		public List<Study> GetStudiesForDeletion(DateTime now, int batchSize)
		{
			return _getStudiesForDeletion(Context, now, batchSize).ToList();
		}

		/// <summary>
		/// Delete Study entity.
		/// </summary>
		/// <param name="entity"></param>
		public void Delete(Study entity)
		{
			Context.Studies.DeleteOnSubmit(entity);
		}

		/// <summary>
		/// Delete Study entity.
		/// </summary>
		public void DeleteStudy(string studyInstanceUid)
		{
			Context.Studies.DeleteOnSubmit(GetStudy(studyInstanceUid));
		}

		internal void DeleteAll()
		{
			Context.Studies.DeleteAllOnSubmit(from s in Context.Studies select s);
		}

		#region Compiled Queries

		private static readonly Func<DicomStoreDataContext, long, IQueryable<Study>> _getStudyByOid =
			CompiledQuery.Compile<DicomStoreDataContext, long, IQueryable<Study>>((context, oid) => (from s in context.Studies
			                                                                                         where s.Oid == oid
			                                                                                         select s).Take(1));

		private static readonly Func<DicomStoreDataContext, string, IQueryable<Study>> _getStudyByStudyInstanceUid =
			CompiledQuery.Compile<DicomStoreDataContext, string, IQueryable<Study>>((context, studyInstanceUid) => (from s in context.Studies
			                                                                                                        where s.StudyInstanceUid == studyInstanceUid
			                                                                                                        select s).Take(1));

		private static readonly Func<DicomStoreDataContext, int> _getStudyCount =
			CompiledQuery.Compile<DicomStoreDataContext, int>(context => (from s in context.Studies
			                                                              select s.Oid).Count());

		private static readonly Func<DicomStoreDataContext, IQueryable<long>> _getAllStudyOids =
			CompiledQuery.Compile<DicomStoreDataContext, IQueryable<long>>(context => (from s in context.Studies
			                                                                           select s.Oid));

		private static readonly Func<DicomStoreDataContext, IQueryable<Study>> _getAllStudies =
			CompiledQuery.Compile<DicomStoreDataContext, IQueryable<Study>>(context => (from s in context.Studies
			                                                                            select s));

		private static readonly Func<DicomStoreDataContext, IQueryable<Study>> _getReindexStudies =
			CompiledQuery.Compile<DicomStoreDataContext, IQueryable<Study>>(context => (from s in context.Studies
			                                                                            where s.Reindex
			                                                                            select s));

		private static readonly Func<DicomStoreDataContext, DateTime, int, IQueryable<Study>> _getStudiesForDeletion =
			CompiledQuery.Compile<DicomStoreDataContext, DateTime, int, IQueryable<Study>>((context, now, batchSize) => (from s in context.Studies
			                                                                                                             where !s.Deleted && !s.Reindex && s.DeleteTime != null && s.DeleteTime < now
			                                                                                                             orderby s.DeleteTime ascending
			                                                                                                             select s).Take(batchSize));

		#endregion
	}
}
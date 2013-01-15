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
using System.Linq;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{
	public class StudyBroker : Broker
	{
		internal StudyBroker(DicomStoreDataContext context)
			: base(context)
		{
		}

		public void AddStudy(Study study)
		{
			this.Context.Studies.InsertOnSubmit(study);
		}

		public Study GetStudy(long oid)
		{	
            var list = (from s in this.Context.Studies
                        where s.Oid == oid
                        select s).ToList();

            if (!list.Any()) return null;

            return list.First();
		}

		public Study GetStudy(string studyInstanceUid)
		{
            var list = (from s in this.Context.Studies
                        where s.StudyInstanceUid == studyInstanceUid
                        select s).ToList();

            if (!list.Any()) return null;

            return list.First();
		}

        public long GetStudyCount()
        {
            var count = (from s in Context.Studies
                        select s).Count();
            return count;
        }

        public List<long> GetStudyOids()
        {
            return GetSingleColumn<Study, long>(null, s => s.Oid);
        }

        public List<Study> GetStudies()
        {
            return (from s in Context.Studies                    
                    select s).ToList();
        }

        public List<Study> GetReindexStudies()
        {
            return (from s in Context.Studies
                    where s.Reindex
                    select s).ToList();
        }

		/// <summary>
		/// Get studies that are eligible for deletion as of the specified time.
		/// </summary>
		/// <param name="now"></param>
		/// <param name="batchSize"></param>
		/// <returns></returns>
		public List<Study> GetStudiesForDeletion(DateTime now, int batchSize)
		{
			return (from s in Context.Studies
					where !s.Deleted && !s.Reindex && s.DeleteTime != null && s.DeleteTime < now
					orderby s.DeleteTime ascending 
					select s)
					.Take(batchSize)
					.ToList();
		}		 

        /// <summary>
        /// Delete Study entity.
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(Study entity)
        {
            this.Context.Studies.DeleteOnSubmit(entity);
        }

        /// <summary>
        /// Delete Study entity.
        /// </summary>
	    public void DeleteStudy(string studyInstanceUid)
        {
            this.Context.Studies.DeleteOnSubmit(GetStudy(studyInstanceUid));
	    }

	    internal void DeleteAll()
	    {
            Context.Studies.DeleteAllOnSubmit(from s in Context.Studies select s);
	    }
	}
}

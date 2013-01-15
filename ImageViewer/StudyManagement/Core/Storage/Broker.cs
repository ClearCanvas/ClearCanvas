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
using System.Linq.Expressions;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{
	public abstract class Broker
	{
		protected Broker(DicomStoreDataContext context)
		{
			Context = context;	
		}

		protected DicomStoreDataContext Context { get; private set; }

        // taken from here: http://stackoverflow.com/questions/2910471/querying-a-single-column-with-linq
        public List<TResult> GetSingleColumn<T, TResult>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TResult>> select) 
            where T : class
        {
                var q = Context.GetTable<T>().AsQueryable();
                if (predicate != null)
                    q = q.Where(predicate).AsQueryable();
                return q.Select(select).ToList();
            
        }
	}
}

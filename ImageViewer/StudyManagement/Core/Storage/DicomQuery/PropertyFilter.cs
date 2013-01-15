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
using System.Linq;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery
{
    /// <summary>
    /// Base class for <see cref="PropertyFilter{TDatabaseObject}"/>s.
    /// </summary>
    /// <remarks><see cref="DicomPropertyFilter{TDatabaseObject}"/>s use the template and rule
    /// design patterns to allow subclasses to implement only what they need to, and not have
    /// to worry about providing any logic. Subclasses should only have to filter SQL queries
    /// and return property values for post-filtering.</remarks>
    internal interface IPropertyFilter<TDatabaseObject> 
        where TDatabaseObject : class 
    {
        IQueryable<TDatabaseObject> AddToQuery(IQueryable<TDatabaseObject> query);
        IEnumerable<TDatabaseObject> FilterResults(IEnumerable<TDatabaseObject> results);
        void SetAttributeValue(TDatabaseObject item, DicomAttributeCollection result);
    }

    internal abstract class PropertyFilter<TDatabaseObject> : IPropertyFilter<TDatabaseObject>
        where TDatabaseObject : class
    {
        protected internal bool AddToQueryEnabled { get; set; }
        protected internal bool FilterResultsEnabled { get; set; }

        protected internal abstract bool ShouldAddToQuery { get; }
        protected internal virtual bool ShouldAddToResult { get; set; }

        protected virtual IQueryable<TDatabaseObject> AddToQuery(IQueryable<TDatabaseObject> query)
        {
            return query;
        }

        protected virtual IEnumerable<TDatabaseObject> FilterResults(IEnumerable<TDatabaseObject> results)
        {
            return results;
        }

        protected abstract void SetAttributeValue(TDatabaseObject item, DicomAttributeCollection result);

        #region IPropertyFilter<TDatabaseObject> Members

        IQueryable<TDatabaseObject> IPropertyFilter<TDatabaseObject>.AddToQuery(IQueryable<TDatabaseObject> query)
        {
            if (!AddToQueryEnabled || !ShouldAddToQuery)
                return query;

            return AddToQuery(query);
        }

        IEnumerable<TDatabaseObject> IPropertyFilter<TDatabaseObject>.FilterResults(IEnumerable<TDatabaseObject> results)
        {
            if (!FilterResultsEnabled || !ShouldAddToQuery)
                return results;

            return FilterResults(results);
        }

        void IPropertyFilter<TDatabaseObject>.SetAttributeValue(TDatabaseObject item, DicomAttributeCollection result)
        {
            if (ShouldAddToResult)
                SetAttributeValue(item, result);
        }

        #endregion
    }
}
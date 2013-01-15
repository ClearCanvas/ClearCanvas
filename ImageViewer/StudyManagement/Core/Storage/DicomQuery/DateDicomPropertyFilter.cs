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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery
{
    internal abstract class DateDicomPropertyFilter<TDatabaseObject> : DicomPropertyFilter<TDatabaseObject>
        where TDatabaseObject : class
    {
        private bool _isRange;
        private DateTime? _date1;
        private DateTime? _date2;
        private bool _parsedCriterion;

        protected DateDicomPropertyFilter(DicomTagPath path, DicomAttributeCollection criteria)
            : base(path, criteria)
        {
            Platform.CheckTrue(path.ValueRepresentation.Name == "DA", "Path is not VR=DA");
            if (Criterion != null)
                Platform.CheckTrue(Criterion.Tag.VR.Name == "DA", "Criteria is not VR=DA");
        }

        public DateTime? Date1
        {
            get
            {
                if (!_parsedCriterion)
                    ParseCriterion();
                
                return _date1;
            }
        }

        public DateTime? Date2
        {
            get
            {
                if (!_parsedCriterion)
                    ParseCriterion();

                return _date2;
            }
        }

        private void ParseCriterion()
        {
            _parsedCriterion = true;
            DateRangeHelper.Parse(Criterion.GetString(0, ""), out _date1, out _date2, out _isRange);
        }

        protected virtual IQueryable<TDatabaseObject> AddEqualsToQuery(IQueryable<TDatabaseObject> query, DateTime date)
        {
            throw new NotImplementedException("If AddToQueryEnabled is true, AddEqualsToQuery must be implemented.");
        }

        protected virtual IQueryable<TDatabaseObject> AddLessOrEqualToQuery(IQueryable<TDatabaseObject> query, DateTime date)
        {
            throw new NotImplementedException("If AddToQueryEnabled is true, AddLessOrEqualToQuery must be implemented.");
        }

        protected virtual IQueryable<TDatabaseObject> AddGreaterOrEqualToQuery(IQueryable<TDatabaseObject> query, DateTime date)
        {
            throw new NotImplementedException("If AddToQueryEnabled is true, AddGreaterOrEqualToQuery must be implemented.");
        }

        protected virtual IQueryable<TDatabaseObject> AddBetweenDatesToQuery(IQueryable<TDatabaseObject> query, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException("If AddToQueryEnabled is true, AddBetweenDatesToQuery must be implemented.");
        }

        protected sealed override IQueryable<TDatabaseObject> AddToQuery(IQueryable<TDatabaseObject> query)
        {
            if (Date1 != null && Date2 != null)
                return AddBetweenDatesToQuery(query, Date1.Value, Date2.Value);
            
            if (Date1 != null)
            {
                if (_isRange)
                    return AddGreaterOrEqualToQuery(query, Date1.Value);

                return AddEqualsToQuery(query, Date1.Value);
            }

            if (Date2 != null)
                return AddLessOrEqualToQuery(query, Date2.Value);

            return base.AddToQuery(query);
        }

        protected sealed override System.Collections.Generic.IEnumerable<TDatabaseObject> FilterResults(System.Collections.Generic.IEnumerable<TDatabaseObject> results)
        {
            throw new NotSupportedException("Any date filtering is done in the database only.");
        }
    }
}

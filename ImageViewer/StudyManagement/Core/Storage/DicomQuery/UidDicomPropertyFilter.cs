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
    internal abstract class UidPropertyFilter<TDatabaseObject> : DicomPropertyFilter<TDatabaseObject>
        where TDatabaseObject : class
    {
        private string[] _criterionValues;

        protected UidPropertyFilter(DicomTagPath path, DicomAttributeCollection criteria)
            : base(path, criteria)
        {
            Platform.CheckTrue(path.ValueRepresentation.Name == "UI", "Path is not VR=UI");
            if (Criterion != null)
                Platform.CheckTrue(Criterion.Tag.VR.Name == "UI", "Criteria is not VR=UI");
        }

        protected string[] CriterionValues
        {
            get
            {
                if (_criterionValues != null)
                    return _criterionValues;

                _criterionValues = DicomStringHelper.GetStringArray(Criterion.ToString()) ?? new string[0];
                return _criterionValues;
            }    
        }

        protected virtual IQueryable<TDatabaseObject> AddUidToQuery(IQueryable<TDatabaseObject> query, string uid)
        {
            throw new NotImplementedException("If AddToQueryEnabled=true, AddUidToQuery must be implemented.");
        }

        protected virtual IQueryable<TDatabaseObject> AddUidsToQuery(IQueryable<TDatabaseObject> query, string[] uids)
        {
            throw new NotImplementedException("If AddToQueryEnabled=true, AddUidsToQuery must be implemented.");
        }

        protected override IQueryable<TDatabaseObject> AddToQuery(IQueryable<TDatabaseObject> query)
        {
            if (CriterionValues.Length == 0)
                return base.AddToQuery(query);

            if (CriterionValues.Length == 1)
                return AddUidToQuery(query, CriterionValues[0]);

            return AddUidsToQuery(query, CriterionValues);
        }
    }
}

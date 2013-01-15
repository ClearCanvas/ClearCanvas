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

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery
{
    /// <summary>
    /// Base class for <see cref="PropertyFilter{TDatabaseObject}"/>s that are 1:1 with a DICOM Attribute
    /// that can be queried and returned according to Part 4 of the DICOM standard.
    /// </summary>
    /// <remarks><see cref="DicomPropertyFilter{TDatabaseObject}"/>s use the template and rule
    /// design patterns to allow subclasses to implement only what they need to, and not have
    /// to worry about providing any logic. Subclasses should only have to filter SQL queries
    /// and return property values for post-filtering.</remarks>
    internal abstract class DicomPropertyFilter<TDatabaseObject> : PropertyFilter<TDatabaseObject>
        where TDatabaseObject : class
    {
        protected DicomPropertyFilter(DicomTagPath path, DicomAttributeCollection criteria)
        {
            Path = path;
            Criterion = Path.GetAttribute(criteria);
            IsReturnValueRequired = false;
            AddToQueryEnabled = true;
            FilterResultsEnabled = false;
        }

        protected internal bool IsReturnValueRequired { get; set; }

        public DicomTagPath Path { get; private set; }
        public DicomAttribute Criterion { get; private set; }

        protected internal bool IsCriterionEmpty
        {
            get { return Criterion == null || Criterion.IsEmpty; }
        }

        protected internal bool IsCriterionNull
        {
            get { return Criterion != null && Criterion.IsNull; }
        }

        protected internal override bool ShouldAddToQuery
        {
            get { return !IsCriterionEmpty && !IsCriterionNull; }
        }

        protected internal override bool ShouldAddToResult
        {
            get { return IsReturnValueRequired || !IsCriterionEmpty; }
        }

        protected virtual void AddValueToResult(TDatabaseObject item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetNullValue();
        }

        protected sealed override void SetAttributeValue(TDatabaseObject item, DicomAttributeCollection result)
        {
            var resultAttribute = Path.GetAttribute(result, true);
            AddValueToResult(item, resultAttribute);
        }

        public override string ToString()
        {
            return Path.ToString();
        }
    }
}
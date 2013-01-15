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

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Provides a basic implementation of <see cref="IRelatedEntityCondition{T}"/>.  See <see cref="SearchCriteria"/> for 
    /// usage of this class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RelatedEntityCondition<T> : SearchConditionBase, IRelatedEntityCondition<T>
        where T: SearchCriteria
    {
        #region Private Members
        private readonly string _baseTableColumn;
        private readonly string _relatedTableColumn;
        #endregion

        #region Public Properties
        public string BaseTableColumn
        {
            get { return _baseTableColumn; }
        }
        public string RelatedTableColumn
        {
            get { return _relatedTableColumn; }
        }
        #endregion

        #region Constructors
        public RelatedEntityCondition()
        {
        }

        public RelatedEntityCondition(string name, string baseTableColumn, string relatedTableColumn)
            : base(name)
        {
            _baseTableColumn = baseTableColumn;
            _relatedTableColumn = relatedTableColumn;
        }

        protected RelatedEntityCondition(RelatedEntityCondition<T> other)
            :base(other)
        {
            _baseTableColumn = other._baseTableColumn;
            _relatedTableColumn = other._relatedTableColumn;
        }
        #endregion

        #region IRelatedEntityCondition<T> Members

        public void Exists(T val)
        {
            SetCondition(SearchConditionTest.Exists, val);
        }

        public void NotExists(T val)
        {
            SetCondition(SearchConditionTest.NotExists, val);
        }

        #endregion

        public override object Clone()
        {
            return new RelatedEntityCondition<T>(this);
        }
    }
}

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

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery
{
    /// <summary>
    /// Interface for <see cref="PropertyFilter{TDatabaseObject}"/>s where its value(s) in the database
    /// may be multi-valued.
    /// </summary>
    /// <remarks>Multi-valued properties are stored in the database as DICOM multi-valued strings (separated by a backslash '\').
    /// 
    /// This interface can be implemented by any <see cref="PropertyFilter{TDatabaseObject}"/> whether
    /// its property is single-valued, or multi-valued, and it will work fine. However, in the case where
    /// a property is known to potentially be multi-valued, that must be taken into account in the
    /// <see cref="PropertyFilter{TDatabaseObject}"/>'s AddToQuery overrides.</remarks>
    internal interface IMultiValuedPropertyFilter<TDatabaseObject> : IPropertyFilter<TDatabaseObject> where TDatabaseObject : class
    {
        string[] CriterionValues { get; }
        bool IsWildcardCriterionAllowed { get; }
        bool IsWildcardCriterion(string criterion);

        string[] GetPropertyValues(TDatabaseObject item);

        IQueryable<TDatabaseObject> AddEqualsToQuery(IQueryable<TDatabaseObject> query, string criterionValue);
        IQueryable<TDatabaseObject> AddLikeToQuery(IQueryable<TDatabaseObject> query, string criterion);
    }

    /// <summary>
    /// Rule/logic for querying/filtering on a property that may be multi-valued.
    /// </summary>
    /// <remarks>The logic is separated out here because it is reasonably complicated and can be reused.</remarks>
    internal class MultiValuedPropertyRule<TDatabaseObject> where TDatabaseObject : class 
    {
        private readonly IMultiValuedPropertyFilter<TDatabaseObject> _propertyFilter;

        public MultiValuedPropertyRule(IMultiValuedPropertyFilter<TDatabaseObject> propertyFilter)
        {
            _propertyFilter = propertyFilter;
        }

        public IQueryable<TDatabaseObject> AddToQuery(IQueryable<TDatabaseObject> query)
        {
            if (_propertyFilter.CriterionValues.Length == 0)
            {
                //We should never actually get here because the empty criteria case
                //is covered elsewhere, but it doesn't hurt.
                return query;
            }

            if (_propertyFilter.CriterionValues.Length > 1)
                return AddToQuery(query, _propertyFilter.CriterionValues);

            return AddToQuery(query, _propertyFilter.CriterionValues[0]);
        }

        public IEnumerable<TDatabaseObject> FilterResults(IEnumerable<TDatabaseObject> results)
        {
            if (_propertyFilter.CriterionValues.Length == 0)
            {
                //We should never actually get here because the empty criteria case
                //is covered elsewhere, but it doesn't hurt.
                return results;
            }

            if (_propertyFilter.CriterionValues.Length > 1)
                return FilterResults(results, _propertyFilter.CriterionValues);

            return FilterResults(results, _propertyFilter.CriterionValues[0]);
        }

        private IQueryable<TDatabaseObject> AddToQuery(IQueryable<TDatabaseObject> query, string criterionValue)
        {
            if (!_propertyFilter.IsWildcardCriterion(criterionValue))
                return _propertyFilter.AddEqualsToQuery(query, criterionValue);

            var sqlCriterion = criterionValue.Replace("*", "%").Replace("?", "_");
            var returnQuery = _propertyFilter.AddLikeToQuery(query, sqlCriterion);
            return returnQuery;
        }

        private IQueryable<TDatabaseObject> AddToQuery(IQueryable<TDatabaseObject> inputQuery, IEnumerable<string> criterionValues)
        {
            IQueryable<TDatabaseObject> unionedQuery = null;
            foreach (var criterionValue in criterionValues.Where(value => !String.IsNullOrEmpty(value)))
            {
                var criterionQuery = AddToQuery(inputQuery, criterionValue);
                unionedQuery = unionedQuery == null ? criterionQuery : unionedQuery.Union(criterionQuery);
            }

            return unionedQuery ?? inputQuery;
        }

        private bool IsMatch(TDatabaseObject result, string criterion)
        {
            var propertyValues = _propertyFilter.GetPropertyValues(result);
            if (propertyValues.Length == 0)
            {
                //DICOM says if we maintain an object with an empty value, it's a match for any criteria,
                //but we don't do it because it's weird.
                return false;
            }

            if (!_propertyFilter.IsWildcardCriterion(criterion))
                return propertyValues.Any(value => QueryUtilities.AreEqual(value, criterion));

            return propertyValues.Any(value => QueryUtilities.IsLike(value, criterion));
        }

        private IEnumerable<TDatabaseObject> FilterResults(IEnumerable<TDatabaseObject> results, IEnumerable<string> criterionValues)
        {
            var resultsList = new List<TDatabaseObject>(results);
            IEnumerable<TDatabaseObject> unionedResults = null;
            foreach (var criterionValue in criterionValues.Where(value => !String.IsNullOrEmpty(value)))
            {
                var criterion = criterionValue;
                var criterionResults = resultsList.Where(result => IsMatch(result, criterion));
                unionedResults = unionedResults == null ? criterionResults : unionedResults.Union(criterionResults);
            }

            return unionedResults ?? results;
        }

        private IEnumerable<TDatabaseObject> FilterResults(IEnumerable<TDatabaseObject> results, string criterionValue)
        {
            //Empty criterion means just return the value.
            if (string.IsNullOrEmpty(criterionValue))
                return results;

            return results.Where(result => IsMatch(result, criterionValue));
        }
    }
}
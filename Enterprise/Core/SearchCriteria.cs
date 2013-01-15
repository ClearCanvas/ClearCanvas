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
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Base interface for objects that behave as search criteria.
	/// </summary>
	public interface ISearchCriteria : ICloneable
	{
		/// <summary>
		/// Gets the key, or null if this is a top-level criteria.
		/// </summary>
		/// <returns></returns>
		string GetKey();

		/// <summary>
		/// Gets a value indicating if this criteria instance is empty.
		/// </summary>
		bool IsEmpty { get; }

		/// <summary>
		/// Creates a new object that is a copy of the current instance, including only the sub-criteria
		/// that are included by the specified filter.  The filter is optionally applied recursively to sub-criteria.
		/// </summary>
		/// <param name="subCriteriaFilter"></param>
		/// <param name="recursive"></param>
		/// <returns></returns>
		ISearchCriteria Clone(Predicate<ISearchCriteria> subCriteriaFilter, bool recursive);

		/// <summary>
		/// Gets a predicate representing this criteria that can be used to test an object to see if it satisfies the criteria.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		Predicate<T> AsPredicate<T>();
	}


	/// <summary>
	/// Abstract base class for all search criteria classes.
	/// </summary>
	public abstract class SearchCriteria : ISearchCriteria
	{
		private readonly string _key;
		private readonly Dictionary<string, SearchCriteria> _subCriteria;

		#region Constructors

		/// <summary>
		/// Constructs a search-criteria with a key.
		/// </summary>
		/// <param name="key"></param>
		protected SearchCriteria(string key)
		{
			_key = key;
			_subCriteria = new Dictionary<string, SearchCriteria>();
		}

		/// <summary>
		/// Constructs a top-level search criteria.
		/// </summary>
		protected SearchCriteria()
			: this((string)null)
		{
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="other"></param>
		protected SearchCriteria(SearchCriteria other)
		{
			_key = other._key;
			_subCriteria = new Dictionary<string, SearchCriteria>();

			foreach (var kvp in other._subCriteria)
			{
				_subCriteria.Add(kvp.Key, (SearchCriteria)kvp.Value.Clone());
			}
		}

		#endregion

		#region Public API

		public void SetSubCriteria(SearchCriteria subCriteria)
		{
			var key = subCriteria.GetKey();
			if(string.IsNullOrEmpty(key))
				throw new ArgumentException("Sub-criteria must have a key defined.");

			this.SubCriteria[key] = subCriteria;
		}

		public IEnumerable<SearchCriteria> EnumerateSubCriteria()
		{
			return SubCriteria.Values;
		}

		/// <summary>
		/// Gets a predicate representing this criteria that can be used to test an object to see if it satisfies the criteria.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public Predicate<T> AsPredicate<T>()
		{
			return value => IsSatisfiedBy(value);
		}

		/// <summary>
		/// Gets a value indicating if this criteria instance is empty, that is,
		/// it does not specify any conditions.
		/// </summary>
		public virtual bool IsEmpty
		{
			get
			{
				foreach (var criteria in _subCriteria.Values)
				{
					if (!criteria.IsEmpty)
						return false;
				}
				return true;
			}
		}

		/// <summary>
		/// Gets a text dump of this criteria object that is useful for debugging purposes.
		/// </summary>
		/// <remarks>
		/// This property is provided because the criteria objects are particularly difficult to work
		/// with in the debugger when there are multiple levels of nested criteria.
		/// </remarks>
		public string[] __Dump
		{
			get
			{
				return Dump(GetKey());
			}
		}

		/// <summary>
		/// Gets the key, or null if this is a top-level criteria.
		/// </summary>
		/// <remarks>
		/// This is intentionally implemented as a method, rather than a property, so that there is no chance that a sub-class
		/// property will conflict.
		/// </remarks>
		/// <returns></returns>
		public string GetKey()
		{
			return _key;
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance, including only the sub-criteria
		/// that are included by the specified filter.  The filter is optionally applied recursively to sub-criteria.
		/// </summary>
		/// <param name="subCriteriaFilter"></param>
		/// <param name="recursive"></param>
		/// <returns></returns>
		public ISearchCriteria Clone(Predicate<ISearchCriteria> subCriteriaFilter, bool recursive)
		{
			// this implementation is not particularly efficient, but it was the simplest 
			// way to do it given the default Clone() overload
			// we clone the entire criteria object, then remove any sub-criteria
			// that don't satisfy the filter
			var copy = (SearchCriteria)this.Clone();
			copy.FilterSubCriteria(subCriteriaFilter, recursive);
			return copy;
		}

		///<summary>
		///Creates a new object that is a copy of the current instance.
		///</summary>
		///
		///<returns>
		///A new object that is a copy of this instance.
		///</returns>
		///<filterpriority>2</filterpriority>
		public abstract object Clone();

		#endregion

		#region Protected API

		/// <summary>
		/// Tests whether the specified value satisfies this criteria.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		protected internal virtual bool IsSatisfiedBy(object obj)
		{
			foreach (var kvp in SubCriteria)
			{
				object x;
				if (!GetPropertyValue(obj, kvp.Key, out x))
				{
					// TODO: if the property doesn't exist, what do we do??  
					// for now, return false, but we might want to have a flag indicating how to handle missing properties
					return false;
				}

				if (!kvp.Value.IsSatisfiedBy(x))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Gets the sub-criteria dictionary.
		/// </summary>
		protected internal IDictionary<string, SearchCriteria> SubCriteria
		{
			get { return _subCriteria; }
		}

		/// <summary>
		/// Gets an array of strings where each string is a text representation of a search condition
		/// that is defined by this criteria instance.
		/// </summary>
		/// <param name="prefix"></param>
		/// <returns></returns>
		protected virtual string[] Dump(string prefix)
		{
			var lines = new List<string>();
			foreach (var pair in _subCriteria)
			{
				if (!pair.Value.IsEmpty)
				{
					var p = prefix == null ? "" : prefix + ".";
					lines.AddRange(pair.Value.Dump(p + pair.Key));
				}
			}
			return lines.ToArray();
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Recursively removes sub-criteria from this instance that do not satisfy the filter condition.
		/// </summary>
		/// <param name="subCriteriaFilter"></param>
		/// <param name="recursive"></param>
		private void FilterSubCriteria(Predicate<ISearchCriteria> subCriteriaFilter, bool recursive)
		{
			var keys = new List<string>(_subCriteria.Keys);
			foreach (var key in keys)
			{
				var subCriteria = _subCriteria[key];
				if (!subCriteriaFilter(subCriteria))
				{
					// remove sub-criteria
					_subCriteria.Remove(key);
				}
				else
				{
					// retain immediate sub-criteria, but optionally apply filter recursively
					if (recursive)
						subCriteria.FilterSubCriteria(subCriteriaFilter, recursive);
				}
			}
		}

		/// <summary>
		/// Gets the value of the specified property, or returns false if the property does not exist.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private static bool GetPropertyValue(object obj, string property, out object value)
		{
			value = null;

			// if obj is null, leave value = null (null propagation)
			if (ReferenceEquals(obj, null))
				return true;

			// get the property
			var prop = obj.GetType().GetProperty(property);

			// if the property doesn't exist, return false
			if (prop == null)
				return false;

			// evaluate the property
			value = prop.GetValue(obj, null);
			return true;
		}

		#endregion
	}
}

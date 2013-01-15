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
using System.Text;
using ClearCanvas.Enterprise.Core;
using System.Collections;

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
	/// <summary>
	/// Provides support for building HQL queries dynamically from <see cref="SearchCriteria"/> objects.
	/// </summary>
	/// <seealso cref="HqlQuery"/>
	public class HqlCondition : HqlElement
	{
		/// <summary>
		/// Extracts a list of <see cref="HqlCondition"/> objects from the specified <see cref="SearchCriteria"/>
		/// </summary>
		/// <param name="qualifier">The HQL qualifier to prepend to the criteria variables</param>
		/// <param name="criteria">The search criteria object</param>
		/// <returns>A list of HQL conditions that are equivalent to the search criteria</returns>
		public static HqlCondition[] FromSearchCriteria(string qualifier, SearchCriteria criteria)
		{
			return FromSearchCriteria(qualifier, criteria, a => a);
		}

		/// <summary>
		/// Extracts a list of <see cref="HqlCondition"/> objects from the specified <see cref="SearchCriteria"/>
		/// </summary>
		/// <param name="qualifier">The HQL qualifier to prepend to the criteria variables</param>
		/// <param name="criteria">The search criteria object</param>
		/// <param name="remapHqlExprFunc"></param>
		/// <returns>A list of HQL conditions that are equivalent to the search criteria</returns>
		public static HqlCondition[] FromSearchCriteria(string qualifier, SearchCriteria criteria, Converter<string, string> remapHqlExprFunc)
		{
			var hqlConditions = new List<HqlCondition>();
			if (criteria is SearchConditionBase)
			{
				var sc = (SearchConditionBase)criteria;
				if (sc.Test != SearchConditionTest.None)
				{
					hqlConditions.Add(GetCondition(remapHqlExprFunc(qualifier), sc.Test, sc.Values));
				}
			}
			else
			{
				// recur on subCriteria
				foreach (var subCriteria in criteria.EnumerateSubCriteria())
				{
					// use a different syntax for "extended properties" than regular properties
					var subQualifier = criteria is ExtendedPropertiesSearchCriteria ?
						string.Format("{0}['{1}']", qualifier, subCriteria.GetKey()) :
						string.Format("{0}.{1}", qualifier, subCriteria.GetKey());

					hqlConditions.AddRange(FromSearchCriteria(subQualifier, subCriteria, remapHqlExprFunc));
				}
			}

			return hqlConditions.ToArray();
		}

		public static HqlCondition EqualTo(string variable, object value)
		{
			return MakeCondition(variable, "= ?", value);
		}

		public static HqlCondition NotEqualTo(string variable, object value)
		{
			return MakeCondition(variable, "<> ?", value);
		}

		public static HqlCondition Like(string variable, string value)
		{
			return MakeCondition(variable, "like ?", value);
		}

		public static HqlCondition NotLike(string variable, string value)
		{
			return MakeCondition(variable, "not like ?", value);
		}

		public static HqlCondition Between(string variable, object lower, object upper)
		{
			return MakeCondition(variable, "between ? and ?", lower, upper);
		}

		public static HqlCondition In(string variable, params object[] values)
		{
			return InHelper(variable, values, false);
		}

		public static HqlCondition In(string variable, IEnumerable values)
		{
			return InHelper(variable, values, false);
		}

		public static HqlCondition NotIn(string variable, params object[] values)
		{
			return InHelper(variable, values, true);
		}

		public static HqlCondition NotIn(string variable, IEnumerable values)
		{
			return InHelper(variable, values, true);
		}

		private static HqlCondition InHelper(string variable, IEnumerable values, bool invert)
		{
			var valueList = new List<object>();
			var placeHolderList = new List<string>();
			foreach (var o in values)
			{
				valueList.Add(o);
				placeHolderList.Add("?");
			}

			var sb = invert ? new StringBuilder("not in (") : new StringBuilder("in (");
			sb.Append(string.Join(",", placeHolderList.ToArray()));
			sb.Append(")");
			return MakeCondition(variable, sb.ToString(), valueList.ToArray());
		}


		public static HqlCondition LessThan(string variable, object value)
		{
			return MakeCondition(variable, "< ?", value);
		}

		public static HqlCondition LessThanOrEqual(string variable, object value)
		{
			return MakeCondition(variable, "<= ?", value);
		}

		public static HqlCondition MoreThan(string variable, object value)
		{
			return MakeCondition(variable, "> ?", value);
		}

		public static HqlCondition MoreThanOrEqual(string variable, object value)
		{
			return MakeCondition(variable, ">= ?", value);
		}

		public static HqlCondition IsNull(string variable)
		{
			return MakeCondition(variable, "is null");
		}

		public static HqlCondition IsNotNull(string variable)
		{
			return MakeCondition(variable, "is not null");
		}

		public static HqlCondition IsOfClass(string variable, Type[] classes)
		{
			var or = new HqlOr();
			foreach (var clazz in classes)
			{
				or.Conditions.Add(new HqlCondition(string.Format("{0}.class = {1}", variable, clazz.FullName)));
			}
			return or;
		}

		private static HqlCondition MakeCondition(string variable, string hql, params object[] values)
		{
			return new HqlCondition(string.Format("{0} {1}", variable, hql), values);
		}

		private readonly string _hql;
		private readonly object[] _parameters;

		/// <summary>
		/// Constructs an <see cref="HqlCondition"/> from the specified HQL string and parameters.
		/// </summary>
		/// <param name="hql">The HQL string containing conditional parameter placeholders</param>
		/// <param name="parameters">Set of parameters to substitute</param>
		public HqlCondition(string hql, params object[] parameters)
		{
			_hql = hql;
			_parameters = parameters;
		}

		/// <summary>
		/// The HQL for this condition.
		/// </summary>
		public override string Hql
		{
			get { return _hql; }
		}

		/// <summary>
		/// The set of parameters to be substituted into this condition.
		/// </summary>
		public virtual object[] Parameters
		{
			get { return _parameters; }
		}

		internal static HqlCondition GetCondition(string variable, SearchConditionTest test, object[] values)
		{
			switch (test)
			{
				case SearchConditionTest.Equal:
					return EqualTo(variable, values[0]);
				case SearchConditionTest.NotEqual:
					return NotEqualTo(variable, values[0]);
				case SearchConditionTest.Like:
					return Like(variable, (string)values[0]);
				case SearchConditionTest.NotLike:
					return NotLike(variable, (string)values[0]);
				case SearchConditionTest.Between:
					return Between(variable, values[0], values[1]);
				case SearchConditionTest.In:
					return In(variable, values);
				case SearchConditionTest.NotIn:
					return NotIn(variable, values);
				case SearchConditionTest.LessThan:
					return LessThan(variable, values[0]);
				case SearchConditionTest.LessThanOrEqual:
					return LessThanOrEqual(variable, values[0]);
				case SearchConditionTest.MoreThan:
					return MoreThan(variable, values[0]);
				case SearchConditionTest.MoreThanOrEqual:
					return MoreThanOrEqual(variable, values[0]);
				case SearchConditionTest.NotNull:
					return IsNotNull(variable);
				case SearchConditionTest.Null:
					return IsNull(variable);
				default:
					throw new Exception();  // invalid
			}
		}

	}
}

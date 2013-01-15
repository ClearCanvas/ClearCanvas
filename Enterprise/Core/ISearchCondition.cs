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
using System.Collections;

namespace ClearCanvas.Enterprise.Core
{
	public interface ISearchCondition : ISearchCriteria
    {
        /// <summary>
        /// Specifies that the condition variable be equal to the specified value.
        /// </summary>
        /// <param name="val"></param>
        void EqualTo(object val);

        /// <summary>
        /// Specifies that the condition variable not be equal to the specified value.
        /// </summary>
        /// <param name="val"></param>
        void NotEqualTo(object val);

        /// <summary>
        /// Specifies that the condition variable be "like" the specified value.
        /// Note that this test is only valid when T is a string.  The specified value must
        /// contain at least one % character to act as a wildcard.
        /// </summary>
        /// <param name="val"></param>
        void Like(object val);

        /// <summary>
        /// Specifies that the condition variable be "not like" the specified value.
        /// Note that this test is only valid when T is a string.  The specified value must
        /// contain at least one % character to act as a wildcard.
        /// </summary>
        /// <param name="val"></param>
        void NotLike(object val);

        /// <summary>
        /// Specifies that the condition variable starts with the specified value.  This test makes
        /// sense only when T is a string.  The specified value should not contain any % characters.
        /// Calling this method is identical to calling <code>Like(val + "%")</code>
        /// </summary>
        /// <param name="val"></param>
        void StartsWith(object val);

        /// <summary>
        /// Specifies that the condition variable be between the specified values.
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        void Between(object lower, object upper);

        /// <summary>
        /// Specifies that the condition variable be contained in the specified set of values.
        /// </summary>
        /// <param name="values"></param>
        void In(IEnumerable values);

		/// <summary>
		/// Specifies that the condition variable not be contained in the specified set of values.
		/// </summary>
		/// <param name="values"></param>
		void NotIn(IEnumerable values);

        /// <summary>
        /// Specifies that the condition variable be less than the specified value.
        /// </summary>
        /// <param name="val"></param>
        void LessThan(object val);

        /// <summary>
        /// Specifies that the condition variable be less than or equal to the specified value.
        /// </summary>
        /// <param name="val"></param>
        void LessThanOrEqualTo(object val);

        /// <summary>
        /// Specifies that the condition variable be more than the specified value.
        /// </summary>
        /// <param name="val"></param>
        void MoreThan(object val);

        /// <summary>
        /// Specifies that the condition variable be more than or equal to the specified value.
        /// </summary>
        /// <param name="val"></param>
        void MoreThanOrEqualTo(object val);

        /// <summary>
        /// Specifies that the condition variable be null, assuming T is a type that supports the notion of null.
        /// </summary>
        void IsNull();

        /// <summary>
        /// Specifies that the condition variable be non-null, assuming T is a type that supports the notion of null.
        /// </summary>
        void IsNotNull();

        /// <summary>
        /// Specifies that the condition variable be used to sort the results in ascending order.
        /// </summary>
        /// <param name="position">Specifies the priority of this sort constraint relative to other sort constraints</param>
        void SortAsc(int position);

        /// <summary>
        /// Specifies that the condition variable be used to sort the results in descending order.
        /// </summary>
        /// <param name="position">Specifies the priority of this sort constraint relative to other sort constraints</param>
        void SortDesc(int position);
    }


    /// <summary>
    /// Defines the public inteface for a search condition.  Note that not all methods
    /// will make sense for every possible type T.  All methods
    /// will throw an exception if a null argument is passed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISearchCondition<T> : ISearchCriteria
    {
        /// <summary>
        /// Specifies that the condition variable be equal to the specified value.
        /// </summary>
        /// <param name="val"></param>
        void EqualTo(T val);

        /// <summary>
        /// Specifies that the condition variable not be equal to the specified value.
        /// </summary>
        /// <param name="val"></param>
        void NotEqualTo(T val);

        /// <summary>
        /// Specifies that the condition variable be "like" the specified value.
        /// Note that this test is only valid when T is a string.  The specified value must
        /// contain at least one % character to act as a wildcard.
        /// </summary>
        /// <param name="val"></param>
        void Like(T val);

        /// <summary>
        /// Specifies that the condition variable be "not like" the specified value.
        /// Note that this test is only valid when T is a string.  The specified value must
        /// contain at least one % character to act as a wildcard.
        /// </summary>
        /// <param name="val"></param>
        void NotLike(T val);

        /// <summary>
        /// Specifies that the condition variable starts with the specified value.  This test makes
        /// sense only when T is a string.  The specified value should not contain any % characters.
        /// Calling this method is identical to calling <code>Like(val + "%")</code>
        /// </summary>
        /// <param name="val"></param>
        void StartsWith(T val);

        /// <summary>
        /// Specifies that the condition variable be between the specified values.
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        void Between(T lower, T upper);

        /// <summary>
        /// Specifies that the condition variable be contained in the specified set of values.
        /// </summary>
        /// <param name="values"></param>
        void In(IEnumerable<T> values);

		/// <summary>
		/// Specifies that the condition variable not be contained in the specified set of values.
		/// </summary>
		/// <param name="values"></param>
		void NotIn(IEnumerable<T> values);

        /// <summary>
        /// Specifies that the condition variable be less than the specified value.
        /// </summary>
        /// <param name="val"></param>
        void LessThan(T val);

        /// <summary>
        /// Specifies that the condition variable be less than or equal to the specified value.
        /// </summary>
        /// <param name="val"></param>
        void LessThanOrEqualTo(T val);

        /// <summary>
        /// Specifies that the condition variable be more than the specified value.
        /// </summary>
        /// <param name="val"></param>
        void MoreThan(T val);

        /// <summary>
        /// Specifies that the condition variable be more than or equal to the specified value.
        /// </summary>
        /// <param name="val"></param>
        void MoreThanOrEqualTo(T val);

        /// <summary>
        /// Specifies that the condition variable be null, assuming T is a type that supports the notion of null.
        /// </summary>
        void IsNull();

        /// <summary>
        /// Specifies that the condition variable be non-null, assuming T is a type that supports the notion of null.
        /// </summary>
        void IsNotNull();

        /// <summary>
        /// Specifies that the condition variable be used to sort the results in ascending order.
        /// </summary>
        /// <param name="position">Specifies the priority of this sort constraint relative to other sort constraints</param>
        void SortAsc(int position);

        /// <summary>
        /// Specifies that the condition variable be used to sort the results in descending order.
        /// </summary>
        /// <param name="position">Specifies the priority of this sort constraint relative to other sort constraints</param>
        void SortDesc(int position);
    }
}

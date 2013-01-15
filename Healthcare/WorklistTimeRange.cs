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
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Represents a time-range by which a worklist may be filtered.
    /// </summary>
    public class WorklistTimeRange : ValueObject, IEquatable<WorklistTimeRange>
    {
        /// <summary>
        /// Defines a time-range that originates at midnight this morning and ends at midnight tonight.
        /// </summary>
        public static readonly WorklistTimeRange Today = new WorklistTimeRange(WorklistTimePoint.Today, WorklistTimePoint.Today);

        private WorklistTimePoint _start;
        private WorklistTimePoint _end;


        /// <summary>
        /// Constructor.
        /// </summary>
        public WorklistTimeRange()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
		public WorklistTimeRange(WorklistTimePoint start, WorklistTimePoint end)
		{
            _start = start;
            _end = end;
        }

        #region Public members

        /// <summary>
        /// Gets or sets the beginning of the time range.
        /// </summary>
        public WorklistTimePoint Start
        {
            get { return _start; }
            set { _start = value; }
        }

        /// <summary>
        /// Gets or sets the end of the time range.
        /// </summary>
        public WorklistTimePoint End
        {
            get { return _end; }
            set { _end = value; }
        }

		/// <summary>
		/// Gets a value indicating whether this range has a start value.
		/// </summary>
    	public bool HasStart
    	{
			get { return _start != null; }
    	}

		/// <summary>
		/// Gets a value indicating whether this range has an end value.
		/// </summary>
    	public bool HasEnd
    	{
    		get { return _end != null; }
    	}

		/// <summary>
		/// Gets a value indicating whether this range is closed (has both a start and an end specified).
		/// </summary>
    	public bool IsClosed
    	{
			get { return HasStart && HasEnd; }
    	}

		/// <summary>
		/// Gets a value indicating whether this range is unbounded at one or both ends.
		/// </summary>
    	public bool IsOpen
    	{
			get { return !IsClosed; }
    	}

		/// <summary>
		/// Checks if this range is guaranteed to always be within the specified minimum and maximum time span.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public bool IsConstrained(TimeSpan min, TimeSpan max)
		{
			if (min < TimeSpan.Zero)
				throw new ArgumentOutOfRangeException("min", "must be greater than or equal to zero.");
			if (max < TimeSpan.Zero)
				throw new ArgumentOutOfRangeException("max", "must be greater than or equal to zero.");

			// an open range is false by definition
			if (IsOpen)
				return false;

			// a range consisting of one fixed and one relative point can't be guaranteed
			// always satisfy the condition
			if (Start.IsFixed ^ End.IsFixed)
				return false;

			// get the span by resolving the range at an arbitrary point in time (use DateTime.Now for example)
			DateTime startTime, endTime;
			Resolve(DateTime.Now, out startTime, out endTime);

			var span = endTime - startTime;

			// ensure span satisfies condition
			return span >= min && span <= max;
		}

        /// <summary>
        /// Applies this time range to the specified <see cref="ISearchCondition{T}"/>, using the specified current time.
        /// </summary>
        /// <remarks>
        /// T must be either a <see cref="DateTime"/> or a nullable <see cref="DateTime"/>.
        /// </remarks>
        /// <param name="condition"></param>
        /// <param name="currentTime"></param>
        public void Apply(ISearchCondition condition, DateTime currentTime)
        {
            DateTime startTime, endTime;
            Resolve(currentTime, out startTime, out endTime);

            ApplyRange(condition, _start != null, startTime, _end != null, endTime);

        }


        #endregion

        #region Helpers

        private void Resolve(DateTime currentTime, out DateTime startTime, out DateTime endTime)
        {
            startTime = _start != null ? _start.ResolveDown(currentTime) : DateTime.MinValue;
            endTime = _end != null ? _end.ResolveUp(currentTime) : DateTime.MaxValue;
        }

        private static void ApplyRange(ISearchCondition condition, bool hasLower, DateTime lower, bool hasUpper, DateTime upper)
        {
			// if both upper and lower bounded, use the between operator, otherwise use the >= and < operators
			// note that in SQL server, BETWEEN a AND b means a <= x < b (it is asymmetrical), however
			// this is not necessarily the case in other database servers... not much we can do about this.
			if (hasLower && hasUpper)
            {
                condition.Between(lower, upper);
            }
            else if (hasLower)
            {
                condition.MoreThanOrEqualTo(lower);
            }
            else if (hasUpper)
            {
                condition.LessThan(upper);
            }
        }

        #endregion

        #region overrides

        public override object Clone()
        {
            return new WorklistTimeRange(
                _start != null ? (WorklistTimePoint) _start.Clone() : null,
                _end != null ? (WorklistTimePoint)_end.Clone() : null);
        }

        public bool Equals(WorklistTimeRange worklistTimeRange)
        {
            if (worklistTimeRange == null) return false;
            return Equals(_start, worklistTimeRange._start) && Equals(_end, worklistTimeRange._end);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as WorklistTimeRange);
        }

        public override int GetHashCode()
        {
            return (_start != null ? _start.GetHashCode() : 0) + 29*(_end != null ? _end.GetHashCode() : 0);
        }

        #endregion
    }
}

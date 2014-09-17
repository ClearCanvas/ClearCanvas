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

namespace ClearCanvas.Ris.Client
{
    public abstract class RelativeTime : IComparable<RelativeTime>, IComparable
    {
        private int _value;

        protected RelativeTime(int value)
        {
            _value = value;
        }

        public int Value
        {
            get { return _value; }
        }

        public override bool Equals(object obj)
        {
            RelativeTime other = obj as RelativeTime;
            if (other == null)
                return false;
            
            return this.GetType() == other.GetType() && _value == other._value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        #region IComparable<RelativeTime> Members

        public int CompareTo(RelativeTime other)
        {
            if (this.GetType() != other.GetType())
                throw new InvalidOperationException("Only instances of the same class can be compared");
            return _value.CompareTo(other._value);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            RelativeTime that = obj as RelativeTime;
            if(that == null)
                throw new InvalidOperationException("Cannot compare to null.");
            return CompareTo(that);
        }

        #endregion
    }

    public class RelativeTimeInDays : RelativeTime
    {
        public RelativeTimeInDays(int days)
            :base(days)
        {
        }

        public override string ToString()
        {
            if (this.Value == 0)
                return SR.RelativeTimeInDays_Today;
            if (this.Value == 1)
                return SR.RelativeTimeInDays_Tomorrow;
            if (this.Value == -1)
                return SR.RelativeTimeInDays_Yesterday;

			if (this.Value > 0)
				return string.Format(SR.RelativeTimeInDays_DaysFromNow, Math.Abs(this.Value));
			else
				return string.Format(SR.RelativeTimeInDays_DaysAgo, Math.Abs(this.Value));
		}
    }

    public class RelativeTimeInHours : RelativeTime
    {
        public RelativeTimeInHours(int hours)
            : base(hours)
        {
        }

        public override string ToString()
        {
            if (this.Value == 0)
                return SR.RelativeTimeInHours_Now;
			if (this.Value == 1)
				return SR.RelativeTimeInHours_OneHourFromNow;
			if (this.Value == -1)
				return SR.RelativeTimeInHours_OneHourAgo;

			if (this.Value > 0)
				return string.Format(SR.RelativeTimeInHours_HoursFromNow, Math.Abs(this.Value));
			else
				return string.Format(SR.RelativeTimeInHours_HoursAgo, Math.Abs(this.Value));
        }
    }
}

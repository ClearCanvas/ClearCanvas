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

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// <see cref="TimeSpan"/> formatter class.
    /// </summary>
    public static class TimeSpanFormatter
    {
        #region Constants

        private const double TICKSPERHOUR = TICKSPERMINUTE*60;
        private const double TICKSPERMICROECONDS = 10;
        private const double TICKSPERMILISECONDS = TICKSPERMICROECONDS*1000;
        private const double TICKSPERMINUTE = TICKSPERSECONDS*60;
        private const double TICKSPERNANOSECONDS = 1/100.0;
        private const double TICKSPERSECONDS = TICKSPERMILISECONDS*1000;

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Formats a <see cref="TimeSpan"/> in appropriate units, with option to round up.
        /// </summary>
        /// <param name="duration">The duration to be formatted</param>
        /// <param name="roundUp">Indicates whether the duration should be rounded up (eg, '3 sec' instead of '3.232 sec')</param>
        /// <returns>A formatted string representation of the duration</returns>
        public static string Format(TimeSpan duration, bool roundUp)
        {
            if (roundUp)
            {
                if (duration == TimeSpan.Zero)
                    return "N/A";
                else if (duration.Ticks > TICKSPERHOUR)
                    return String.Format("{0} hr {1} min", duration.Hours, duration.Minutes);
                if (duration.Ticks > TICKSPERMINUTE)
                    return String.Format("{0:0} min", duration.TotalMinutes);
                if (duration.Ticks > TICKSPERSECONDS)
                    return String.Format("{0:0} sec", duration.TotalSeconds);
                if (duration.Ticks > TICKSPERMILISECONDS)
                    return String.Format("{0:0} ms", duration.TotalMilliseconds);
                if (duration.Ticks > TICKSPERMICROECONDS)
                    return String.Format("{0:0} µs", duration.Ticks / TICKSPERMICROECONDS);
                else
                    return String.Format("{0:0} ns", duration.Ticks / TICKSPERNANOSECONDS);
            }
            else
            {
                if (duration == TimeSpan.Zero)
                    return "N/A";
                else if (duration.Ticks > TICKSPERHOUR)
                    return String.Format("{0} hr {1} min", duration.Hours, duration.Minutes);
                if (duration.Ticks > TICKSPERMINUTE)
                    return String.Format("{0:0.00} min", duration.TotalMinutes);
                if (duration.Ticks > TICKSPERSECONDS)
                    return String.Format("{0:0.00} sec", duration.TotalSeconds);
                if (duration.Ticks > TICKSPERMILISECONDS)
                    return String.Format("{0:0.00} ms", duration.TotalMilliseconds);
                if (duration.Ticks > TICKSPERMICROECONDS)
                    return String.Format("{0:0.00} µs", duration.Ticks / TICKSPERMICROECONDS);
                else
                    return String.Format("{0:0.00} ns", duration.Ticks / TICKSPERNANOSECONDS);
                
            }
        }

        /// <summary>
        /// Formats a <see cref="TimeSpan"/> in appropriate units.
        /// </summary>
        /// <param name="duration">The duration to be formatted</param>
        /// <returns>A formatted string representation of the duration</returns>
        public static string Format(TimeSpan duration)
        {
            return Format(duration, false);
        }

        #endregion
    }
}
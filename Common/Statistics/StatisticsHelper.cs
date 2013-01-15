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
using System.Diagnostics;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Helper class
    /// </summary>
    public static class StatisticsHelper
    {
        /// <summary>
        /// Resolves the ID of a statistics based on its context and name
        /// </summary>
        /// <param name="stat"></param>
        /// <returns></returns>
        public static object ResolveID(IStatistics stat)
        {
            object key = null;
            if (stat.Context != null)
            {
                key = String.Format("{0}.{1}", stat.Context.ID, stat.Name);
            }
            else
            {
                key = String.Format("{0}", stat.Name);
            }

            Debug.Assert(key != null);
            return key;
        }
    }
}
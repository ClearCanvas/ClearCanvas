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

namespace ClearCanvas.ImageServer.Common.Utilities
{
    public static class StringUtils
    {
        /// <summary>
        /// Compares two strings, treat Null and Empty being the same.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool AreEqual(string x, string y, StringComparison options)
        {
            if (String.IsNullOrEmpty(x))
            {
                return String.IsNullOrEmpty(y);   
            }
            else
                return x.Equals(y, options);
        }

        /// <summary>
        /// Returns the last part of the string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <param name="prepend"></param>
        /// <returns></returns>
        public static String Last(String value, int length, String prepend)
        {
            if (String.IsNullOrEmpty(value))
                return value;

            if (value.Length > length)
            {
                String last = value.Substring(value.Length - length);
                if (String.IsNullOrEmpty(prepend))
                    return last;
                else
                    return prepend + last;
            }
            else
                return value;
                
        }
    }
}

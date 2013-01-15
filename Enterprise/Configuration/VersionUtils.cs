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
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Configuration
{
    /// <summary>
    /// Utilities related to the <see cref="System.Version"/> class.
    /// </summary>
    static class VersionUtils
    {
        /// <summary>
        /// Converts the specified version to a padded version string, which always has the form
        /// xxxxx.xxxxx.xxxxx.xxxxx - this format allows version strings to be compared.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static string ToPaddedVersionString(Version v)
        {
        	return ToPaddedVersionString(v, true, true);
        }

		/// <summary>
		/// Converts the specified version to a padded version string, which always has the form
		/// xxxxx.xxxxx.xxxxx.xxxxx, optionally including the build and revision parts.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="includeBuildPart"></param>
		/// <param name="includeRevisionPart"></param>
		/// <returns></returns>
        public static string ToPaddedVersionString(Version v, bool includeBuildPart, bool includeRevisionPart)
        {
			Platform.CheckForNullReference(v, "value");

			//major.minor.build.revision
			StringBuilder sb = new StringBuilder();
			sb.Append(v.Major.ToString("d5"));
			sb.Append(".");
			sb.Append(v.Minor.ToString("d5"));

			if(includeBuildPart)
			{
				sb.Append(".");
				sb.Append(v.Build.ToString("d5"));
				if (includeRevisionPart)
				{
					sb.Append(".");
					sb.Append(v.Revision.ToString("d5"));
				}
			}


			return sb.ToString();
		}

        /// <summary>
        /// Converts a padded version string to a <see cref="System.Version"/>
        /// </summary>
        /// <param name="pvs"></param>
        /// <returns></returns>
        public static Version FromPaddedVersionString(string pvs)
        {
            return new Version(pvs);
        }
    }
}

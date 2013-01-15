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
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    /// <summary>
    /// Requests a list of worklist, according to the specified filters.
    /// </summary>
    [DataContract]
    public class ListWorklistsRequest : ListRequestBase
    {
        /// <summary>
        /// Filters the results by the specified class names.
        /// </summary>
        [DataMember]
        public List<string> ClassNames;

        /// <summary>
        /// Filters the results by the specified categories.
        /// </summary>
        [DataMember]
        public List<string> Categories;

        /// <summary>
        /// Specifies whether static worklists should be returned in the results.
        /// </summary>
        [DataMember]
        public bool IncludeStatic;

        /// <summary>
        /// Specifies whether user and group owned worklists should be returned in the results.
        /// </summary>
        [DataMember]
        public bool IncludeUserDefinedWorklists;

        /// <summary>
        /// Filters the results by the specified name.
        /// </summary>
        [DataMember]
        public string WorklistName;
    }
}
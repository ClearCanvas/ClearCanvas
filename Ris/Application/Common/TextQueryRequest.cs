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

using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
	public class TextQueryRequest : ListRequestBase
    {
		/// <summary>
		/// Constructor
		/// </summary>
		public TextQueryRequest()
		{
				
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="textQuery"></param>
		/// <param name="specificityThreshold"></param>
		public TextQueryRequest(string textQuery, int specificityThreshold)
			:this(textQuery, specificityThreshold, false)
		{
		}

    	/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="textQuery"></param>
		/// <param name="specificityThreshold"></param>
    	/// <param name="includeDeactivated"></param>
    	public TextQueryRequest(string textQuery, int specificityThreshold, bool includeDeactivated)
    	{
    		TextQuery = textQuery;
    		SpecificityThreshold = specificityThreshold;
			IncludeDeactivated = includeDeactivated;
    	}

    	/// <summary>
        /// The query text.
        /// </summary>
        [DataMember]
        public string TextQuery;

        /// <summary>
        /// The maximum number of allowed matches for which results should be returned.  If the query results in more
        /// matches, it is considered to be not specific enough, and no results are returned. If this value is 0,
        /// it is ignored.
        /// </summary>
        [DataMember]
        public int SpecificityThreshold;
    }
}

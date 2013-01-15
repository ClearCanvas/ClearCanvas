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
using System.Xml.Serialization;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Represents the result of the comparison when two sets of attributes are compared using <see cref="DicomAttributeCollection.Equals()"/>.
    /// </summary>
    public class DicomAttributeComparisonResult
    {
        #region Public Overrides
		public override string  ToString()
		{
			return Details;
		}
    	#endregion

        #region Public Properties

    	/// <summary>
    	/// Type of differences.
    	/// </summary>
    	[XmlAttribute]
    	public ComparisonResultType ResultType { get; set; }

    	/// <summary>
    	/// The name of the offending tag. This can be null if the difference is not tag specific.
    	/// </summary>
    	[XmlAttribute]
    	public String TagName { get; set; }

    	/// <summary>
    	/// Detailed text describing the problem.
    	/// </summary>
    	public string Details { get; set; }

    	#endregion

    }
}
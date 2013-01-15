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

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// IccProfile Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.11.15 (Table C.11.15-1)</remarks>
	public class IccProfileModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IccProfileModuleIod"/> class.
		/// </summary>	
		public IccProfileModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="IccProfileModuleIod"/> class.
		/// </summary>
		/// <param name="IDicomAttributeProvider">The dicom attribute provider.</param>
		public IccProfileModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) {}

		/// <summary>
		/// NOT IMPLEMENTED. Gets or sets the value of IccProfile in the underlying collection. Type 1.
		/// </summary> 		
		public object IccProfile
		{
			// TODO - Implement this.
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.IccProfile; }
		}
	}
}
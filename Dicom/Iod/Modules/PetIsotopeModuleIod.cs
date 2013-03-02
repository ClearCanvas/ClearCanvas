#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// PetIsotope Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.9.2 (Table C.8-61)</remarks>
	public class PetIsotopeModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PetIsotopeModuleIod"/> class.
		/// </summary>	
		public PetIsotopeModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PetIsotopeModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute collection.</param>
		public PetIsotopeModuleIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.RadiopharmaceuticalInformationSequence;
				yield return DicomTags.InterventionDrugInformationSequence;
			}
		}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			RadiopharmaceuticalInformationSequence = null;
			InterventionDrugInformationSequence = null;
		}

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public bool HasValues()
		{
			return !(IsNullOrEmpty(RadiopharmaceuticalInformationSequence)
			         && IsNullOrEmpty(InterventionDrugInformationSequence));
		}

		/// <summary>
		/// NOT IMPLEMENTED. Gets or sets the value of RadiopharmaceuticalInformationSequence in the underlying collection. Type 2.
		/// </summary> 		
		public object RadiopharmaceuticalInformationSequence
		{
			// TODO - Implement this.
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// NOT IMPLEMENTED. Gets or sets the value of InterventionDrugInformationSequence in the underlying collection. Type 3.
		/// </summary> 		
		public object InterventionDrugInformationSequence
		{
			// TODO - Implement this.
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
	}
}
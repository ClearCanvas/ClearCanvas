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
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.Sequences
{
	/// <summary>
	/// PatientBreed Code Sequence
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.1.1 (Table C.7-1)</remarks>
	[Obsolete("Use ContextGroups.Breed instead.")]
	public class PatientBreedCodeSequence : CodeSequenceMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PatientBreedCodeSequence"/> class.
		/// </summary>
		public PatientBreedCodeSequence() : base()
		{
			base.ContextIdentifier = "7480";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PatientBreedCodeSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public PatientBreedCodeSequence(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem)
		{
			base.ContextIdentifier = "7480";
		}

		/// <summary>
		/// Converts a <see cref="PatientBreedCodeSequence"/> to a <see cref="Breed"/>.
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public static implicit operator Breed(PatientBreedCodeSequence code)
		{
			return new Breed(code.CodingSchemeDesignator, code.CodingSchemeVersion, code.CodeValue, code.CodeMeaning);
		}

		/// <summary>
		/// Converts a <see cref="Breed"/> to a <see cref="PatientBreedCodeSequence"/>.
		/// </summary>
		/// <param name="breed"></param>
		/// <returns></returns>
		public static implicit operator PatientBreedCodeSequence(Breed breed)
		{
			var codeSequence = new PatientBreedCodeSequence();
			breed.WriteToCodeSequence(codeSequence);
			return codeSequence;
		}
	}
}
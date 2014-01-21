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
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.ContextGroups
{
	/// <summary>
	/// Source Image Purposes of Reference Context Group
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 16, Annex B, CID 7202</remarks>
	public sealed class SourceImagePurposesOfReferenceContextGroup : ContextGroupBase<SourceImagePurposesOfReference>
	{
		private SourceImagePurposesOfReferenceContextGroup()
			: base(7202, "Source Image Purposes of Reference", true, new DateTime(2011, 06, 09)) {}

		#region Static Instances

		public static readonly SourceImagePurposesOfReference UncompressedPredecessor = new SourceImagePurposesOfReference("121320", "Uncompressed predecessor");
		public static readonly SourceImagePurposesOfReference MaskImageForImageProcessingOperation = new SourceImagePurposesOfReference("121321", "Mask image for image processing operation");
		public static readonly SourceImagePurposesOfReference SourceImageForImageProcessingOperation = new SourceImagePurposesOfReference("121322", "Source image for image processing operation");
		public static readonly SourceImagePurposesOfReference SourceImageForMontage = new SourceImagePurposesOfReference("121329", "Source image for montage");
		public static readonly SourceImagePurposesOfReference LossyCompressedPredecessor = new SourceImagePurposesOfReference("121330", "Lossy compressed predecessor");
		public static readonly SourceImagePurposesOfReference ForProcessingPredecessor = new SourceImagePurposesOfReference("121358", "For Processing predecessor");

		#endregion

		#region Singleton Instancing

		private static readonly SourceImagePurposesOfReferenceContextGroup _contextGroup = new SourceImagePurposesOfReferenceContextGroup();

		public static SourceImagePurposesOfReferenceContextGroup Instance
		{
			get { return _contextGroup; }
		}

		#endregion

		#region Static Enumeration of Values

		public static IEnumerable<SourceImagePurposesOfReference> Values
		{
			get
			{
				yield return UncompressedPredecessor;
				yield return MaskImageForImageProcessingOperation;
				yield return SourceImageForImageProcessingOperation;
				yield return SourceImageForMontage;
				yield return LossyCompressedPredecessor;
				yield return ForProcessingPredecessor;
			}
		}

		/// <summary>
		/// Gets an enumerator that iterates through the defined codes.
		/// </summary>
		/// <returns>A <see cref="IEnumerator{T}"/> object that can be used to iterate through the defined codes.</returns>
		public override IEnumerator<SourceImagePurposesOfReference> GetEnumerator()
		{
			return Values.GetEnumerator();
		}

		public static SourceImagePurposesOfReference LookupCode(CodeSequenceMacro codeSequence)
		{
			return Instance.Lookup(codeSequence);
		}

		#endregion
	}

	/// <summary>
	/// Represents a Source Image Purposes of Reference code.
	/// </summary>
	public class SourceImagePurposesOfReference : ContextGroupBase<SourceImagePurposesOfReference>.ContextGroupItemBase
	{
		/// <summary>
		/// Constructor for codes defined in DICOM 2011, Part 16, Annex B, CID 7202.
		/// </summary>
		internal SourceImagePurposesOfReference(string codeValue, string codeMeaning) : base("DCM", codeValue, codeMeaning) {}

		/// <summary>
		/// Constructs a new code.
		/// </summary>
		/// <param name="codingSchemeDesignator">The designator of the coding scheme in which this code is defined.</param>
		/// <param name="codeValue">The value of this code.</param>
		/// <param name="codeMeaning">The Human-readable meaning of this code.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codingSchemeDesignator"/> or <paramref name="codeValue"/> are <code>null</code> or empty.</exception>
		public SourceImagePurposesOfReference(string codingSchemeDesignator, string codeValue, string codeMeaning)
			: base(codingSchemeDesignator, codeValue, codeMeaning) {}

		/// <summary>
		/// Constructs a new code.
		/// </summary>
		/// <param name="codingSchemeDesignator">The designator of the coding scheme in which this code is defined.</param>
		/// <param name="codingSchemeVersion">The version of the coding scheme in which this code is defined, if known. Should be <code>null</code> if not explicitly specified.</param>
		/// <param name="codeValue">The value of this code.</param>
		/// <param name="codeMeaning">The Human-readable meaning of this code.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codingSchemeDesignator"/> or <paramref name="codeValue"/> are <code>null</code> or empty.</exception>
		public SourceImagePurposesOfReference(string codingSchemeDesignator, string codingSchemeVersion, string codeValue, string codeMeaning)
			: base(codingSchemeDesignator, codingSchemeVersion, codeValue, codeMeaning) {}

		/// <summary>
		/// Constructs a new code.
		/// </summary>
		/// <param name="codeSequence">The code sequence attributes macro.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codeSequence.CodingSchemeDesignator"/> or <paramref name="codeSequence.CodeValue"/> are <code>null</code> or empty.</exception>
		public SourceImagePurposesOfReference(CodeSequenceMacro codeSequence)
			: base(codeSequence.CodingSchemeDesignator, codeSequence.CodingSchemeVersion, codeSequence.CodeValue, codeSequence.CodeMeaning) {}
	}
}
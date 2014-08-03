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
	/// Multi-Frame Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.6 (Table C.7-14)</remarks>
	public class MultiFrameModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MultiFrameModuleIod"/> class.
		/// </summary>	
		public MultiFrameModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MultiFrameModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute collection.</param>
		public MultiFrameModuleIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.NumberOfFrames;
				yield return DicomTags.FrameIncrementPointer;
			}
		}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes() {}

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public bool HasValues()
		{
			return !IsNullOrEmpty(FrameIncrementPointer);
		}

		/// <summary>
		/// Gets or sets the value of NumberOfFrames in the underlying collection. Type 1.
		/// </summary>
		public int NumberOfFrames
		{
			get { return DicomAttributeProvider[DicomTags.NumberOfFrames].GetInt32(0, 0); }
			set { DicomAttributeProvider[DicomTags.NumberOfFrames].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of FrameIncrementPointer in the underlying collection. Type 1.
		/// </summary>
		public uint[] FrameIncrementPointer
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FrameIncrementPointer];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var values = new uint[dicomAttribute.Count];
				for (int n = 0; n < values.Length; n++)
					values[n] = dicomAttribute.GetUInt32(n, 0);
				return values;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "FrameIncrementPointer is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.FrameIncrementPointer];
				for (int n = 0; n < value.Length; n++)
					dicomAttribute.SetUInt32(n, value[n]);
			}
		}
	}
}
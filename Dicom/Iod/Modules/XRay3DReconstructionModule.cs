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
	/// X-Ray 3D Reconstruction Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.21.4 (Table C.8.21.4-1)</remarks>
	public class XRay3DReconstructionModule : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XRay3DReconstructionModule"/> class.
		/// </summary>	
		public XRay3DReconstructionModule() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="XRay3DReconstructionModule"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute collection.</param>
		public XRay3DReconstructionModule(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.XRay3dReconstructionSequence; }
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
			return !(IsNullOrEmpty(XRay3DReconstructionSequence));
		}

		/// <summary>
		/// Gets or sets the value of XRay3dReconstructionSequence in the underlying collection. Type 1.
		/// </summary>
		public XRay3DReconstructionSequenceItem[] XRay3DReconstructionSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.XRay3dReconstructionSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new XRay3DReconstructionSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new XRay3DReconstructionSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "XRay3dReconstructionSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.XRay3dReconstructionSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a XRay3dReconstructionSequence item. Does not modify the XRay3dReconstructionSequence in the underlying collection.
		/// </summary>
		public XRay3DReconstructionSequenceItem CreateXRay3DReconstructionSequenceItem()
		{
			var iodBase = new XRay3DReconstructionSequenceItem(new DicomSequenceItem());
			return iodBase;
		}
	}

	/// <summary>
	/// X-Ray 3D Reconstruction Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.21.4 (Table C.8.21.4-1)</remarks>
	public class XRay3DReconstructionSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XRay3DReconstructionSequenceItem"/> class.
		/// </summary>
		public XRay3DReconstructionSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="XRay3DReconstructionSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public XRay3DReconstructionSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of ReconstructionDescription in the underlying collection. Type 3.
		/// </summary>
		public string ReconstructionDescription
		{
			get { return DicomAttributeProvider[DicomTags.ReconstructionDescription].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.ReconstructionDescription] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.ReconstructionDescription].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ApplicationName in the underlying collection. Type 1.
		/// </summary>
		public string ApplicationName
		{
			get { return DicomAttributeProvider[DicomTags.ApplicationName].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "ApplicationName is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.ApplicationName].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ApplicationVersion in the underlying collection. Type 1.
		/// </summary>
		public string ApplicationVersion
		{
			get { return DicomAttributeProvider[DicomTags.ApplicationVersion].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "ApplicationVersion is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.ApplicationVersion].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ApplicationManufacturer in the underlying collection. Type 1.
		/// </summary>
		public string ApplicationManufacturer
		{
			get { return DicomAttributeProvider[DicomTags.ApplicationManufacturer].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "ApplicationManufacturer is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.ApplicationManufacturer].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of AlgorithmType in the underlying collection. Type 1.
		/// </summary>
		public string AlgorithmType
		{
			get { return DicomAttributeProvider[DicomTags.AlgorithmType].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "AlgorithmType is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.AlgorithmType].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of AlgorithmDescription in the underlying collection. Type 3.
		/// </summary>
		public string AlgorithmDescription
		{
			get { return DicomAttributeProvider[DicomTags.AlgorithmDescription].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.AlgorithmDescription] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.AlgorithmDescription].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of AcquisitionIndex in the underlying collection. Type 1.
		/// </summary>
		public int[] AcquisitionIndex
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.AcquisitionIndex];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var values = new int[dicomAttribute.Count];
				for (int n = 0; n < values.Length; n++)
					values[n] = dicomAttribute.GetInt32(n, 0);
				return values;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "AcquisitionIndex is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.AcquisitionIndex];
				for (int n = 0; n < value.Length; n++)
					dicomAttribute.SetInt32(n, value[n]);
			}
		}
	}
}
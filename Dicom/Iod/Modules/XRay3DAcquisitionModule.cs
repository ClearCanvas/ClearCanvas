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

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// X-Ray 3D Acquisition Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.21.3</remarks>
	public class XRay3DAcquisitionModule : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XRay3DAcquisitionModule"/> class.
		/// </summary>	
		public XRay3DAcquisitionModule() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="XRay3DAcquisitionModule"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute collection.</param>
		public XRay3DAcquisitionModule(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.XRay3dAcquisitionSequence; }
		}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public virtual void InitializeAttributes() {}

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public virtual bool HasValues()
		{
			return !(IsNullOrEmpty(XRay3DAcquisitionSequence));
		}

		/// <summary>
		/// Gets or sets the value of XRay3dAcquisitionSequence in the underlying collection. Type 1.
		/// </summary>
		public IXRay3DAcquisitionSequenceItem[] XRay3DAcquisitionSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.XRay3dAcquisitionSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new IXRay3DAcquisitionSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = CreateXRay3DAcquisitionSequenceItemCore(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "XRay3dAcquisitionSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.XRay3dAcquisitionSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a XRay3dAcquisitionSequence item. Does not modify the XRay3dAcquisitionSequence in the underlying collection.
		/// </summary>
		public IXRay3DAcquisitionSequenceItem CreateXRay3DAcquisitionSequenceItem()
		{
			var iodBase = CreateXRay3DAcquisitionSequenceItemCore(new DicomSequenceItem());
			iodBase.InitializeAttributes();
			return iodBase;
		}

		protected virtual IXRay3DAcquisitionSequenceItem CreateXRay3DAcquisitionSequenceItemCore(DicomSequenceItem sequenceItem)
		{
			return new XRay3DAcquisitionSequenceItem(sequenceItem);
		}

		/// <summary>
		/// Barebones default implementation
		/// </summary>
		private class XRay3DAcquisitionSequenceItem : SequenceIodBase, IXRay3DAcquisitionSequenceItem
		{
			public XRay3DAcquisitionSequenceItem(DicomSequenceItem dicomSequenceItem)
				: base(dicomSequenceItem) {}

			public void InitializeAttributes() {}
		}
	}

	/// <summary>
	/// X-Ray 3D Acquisition Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.21.3</remarks>
	public interface IXRay3DAcquisitionSequenceItem : IIodMacro {}
}
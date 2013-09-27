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

namespace ClearCanvas.Dicom.Iod.FunctionalGroups
{
	/// <summary>
	/// X-Ray Collimator Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.19.6.12 (Table C.8.19.6-12)</remarks>
	public class XRayCollimatorFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XRayCollimatorFunctionalGroup"/> class.
		/// </summary>
		public XRayCollimatorFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="XRayCollimatorFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public XRayCollimatorFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.CollimatorShapeSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.CollimatorShape;
				yield return DicomTags.CollimatorLeftVerticalEdge;
				yield return DicomTags.CollimatorRightVerticalEdge;
				yield return DicomTags.CollimatorUpperHorizontalEdge;
				yield return DicomTags.CollimatorLowerHorizontalEdge;
				yield return DicomTags.CenterOfCircularCollimator;
				yield return DicomTags.RadiusOfCircularCollimator;
				yield return DicomTags.VerticesOfThePolygonalCollimator;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of CollimatorShapeSequence in the underlying collection. Type 1.
		/// </summary>
		public CollimatorShapeSequenceItem CollimatorShapeSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CollimatorShapeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new CollimatorShapeSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CollimatorShapeSequence];
				if (value == null)
				{
					const string msg = "CollimatorShapeSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the CollimatorShapeSequence in the underlying collection. Type 1.
		/// </summary>
		public CollimatorShapeSequenceItem CreateCollimatorShapeSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.CollimatorShapeSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new CollimatorShapeSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new CollimatorShapeSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Collimator Shape Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.19.6.12 (Table C.8.19.6-12)</remarks>
	public class CollimatorShapeSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CollimatorShapeSequenceItem"/> class.
		/// </summary>
		public CollimatorShapeSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CollimatorShapeSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CollimatorShapeSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of CollimatorShape in the underlying collection. Type 1.
		/// </summary>
		public string CollimatorShape
		{
			get { return DicomAttributeProvider[DicomTags.CollimatorShape].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "CollimatorShape is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.CollimatorShape].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CollimatorLeftVerticalEdge in the underlying collection. Type 1C.
		/// </summary>
		public int? CollimatorLeftVerticalEdge
		{
			get
			{
				int result;
				if (DicomAttributeProvider[DicomTags.CollimatorLeftVerticalEdge].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.CollimatorLeftVerticalEdge] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.CollimatorLeftVerticalEdge].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CollimatorRightVerticalEdge in the underlying collection. Type 1C.
		/// </summary>
		public int? CollimatorRightVerticalEdge
		{
			get
			{
				int result;
				if (DicomAttributeProvider[DicomTags.CollimatorRightVerticalEdge].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.CollimatorRightVerticalEdge] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.CollimatorRightVerticalEdge].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CollimatorUpperHorizontalEdge in the underlying collection. Type 1C.
		/// </summary>
		public int? CollimatorUpperHorizontalEdge
		{
			get
			{
				int result;
				if (DicomAttributeProvider[DicomTags.CollimatorUpperHorizontalEdge].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.CollimatorUpperHorizontalEdge] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.CollimatorUpperHorizontalEdge].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CollimatorLowerHorizontalEdge in the underlying collection. Type 1C.
		/// </summary>
		public int? CollimatorLowerHorizontalEdge
		{
			get
			{
				int result;
				if (DicomAttributeProvider[DicomTags.CollimatorLowerHorizontalEdge].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.CollimatorLowerHorizontalEdge] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.CollimatorLowerHorizontalEdge].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CenterOfCircularCollimator in the underlying collection. Type 1C.
		/// </summary>
		public int[] CenterOfCircularCollimator
		{
			get
			{
				var result = new int[2];
				if (DicomAttributeProvider[DicomTags.CenterOfCircularCollimator].TryGetInt32(0, out result[0])
				    && DicomAttributeProvider[DicomTags.CenterOfCircularCollimator].TryGetInt32(1, out result[1]))
					return result;
				return null;
			}
			set
			{
				if (value == null || value.Length != 2)
				{
					DicomAttributeProvider[DicomTags.CenterOfCircularCollimator] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.CenterOfCircularCollimator].SetInt32(0, value[0]);
				DicomAttributeProvider[DicomTags.CenterOfCircularCollimator].SetInt32(1, value[1]);
			}
		}

		/// <summary>
		/// Gets or sets the value of RadiusOfCircularCollimator in the underlying collection. Type 1C.
		/// </summary>
		public int? RadiusOfCircularCollimator
		{
			get
			{
				int result;
				if (DicomAttributeProvider[DicomTags.RadiusOfCircularCollimator].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.RadiusOfCircularCollimator] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.RadiusOfCircularCollimator].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of VerticesOfThePolygonalCollimator in the underlying collection. Type 1C.
		/// </summary>
		public int[] VerticesOfThePolygonalCollimator
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.VerticesOfThePolygonalCollimator];
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
					DicomAttributeProvider[DicomTags.VerticesOfThePolygonalCollimator] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.VerticesOfThePolygonalCollimator];
				for (int n = 0; n < value.Length; n++)
					dicomAttribute.SetInt32(n, value[n]);
			}
		}
	}
}
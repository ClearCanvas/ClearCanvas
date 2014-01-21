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
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// Breast Tomosynthesis Acquisition Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.21.3.4 (Table C.8.21.3.4-1)</remarks>
	public class BreastTomosynthesisAcquisitionModuleIod : XRay3DAcquisitionModule
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BreastTomosynthesisAcquisitionModuleIod"/> class.
		/// </summary>	
		public BreastTomosynthesisAcquisitionModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="BreastTomosynthesisAcquisitionModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute collection.</param>
		public BreastTomosynthesisAcquisitionModuleIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets or sets the value of XRay3DAcquisitionSequence in the underlying collection. Type 1.
		/// </summary>
		public new BreastTomosynthesisAcquisitionSequenceItem[] XRay3DAcquisitionSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.XRay3dAcquisitionSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new BreastTomosynthesisAcquisitionSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new BreastTomosynthesisAcquisitionSequenceItem(items[n]);

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
		public new BreastTomosynthesisAcquisitionSequenceItem CreateXRay3DAcquisitionSequenceItem()
		{
			return (BreastTomosynthesisAcquisitionSequenceItem) base.CreateXRay3DAcquisitionSequenceItem();
		}

		protected override IXRay3DAcquisitionSequenceItem CreateXRay3DAcquisitionSequenceItemCore(DicomSequenceItem sequenceItem)
		{
			return new BreastTomosynthesisAcquisitionSequenceItem(sequenceItem);
		}
	}

	/// <summary>
	/// Breast Tomosynthesis Acquisition Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.21.3.4 (Table C.8.21.3.4-1)</remarks>
	public class BreastTomosynthesisAcquisitionSequenceItem : SequenceIodBase, IXRay3DAcquisitionSequenceItem, IXRay3DGeneralSharedAcquisitionMacro, IXRay3DGeneralPositionerMovementMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BreastTomosynthesisAcquisitionSequenceItem"/> class.
		/// </summary>
		public BreastTomosynthesisAcquisitionSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="BreastTomosynthesisAcquisitionSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public BreastTomosynthesisAcquisitionSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		void IIodMacro.InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of FieldOfViewShape in the underlying collection. Type 1.
		/// </summary>
		public string FieldOfViewShape
		{
			get { return DicomAttributeProvider[DicomTags.FieldOfViewShape].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "FieldOfViewShape is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.FieldOfViewShape].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SourceImageSequence in the underlying collection. Type 1C.
		/// </summary>
		public ImageSopInstanceReferenceMacro[] SourceImageSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.SourceImageSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
				{
					return null;
				}

				var result = new ImageSopInstanceReferenceMacro[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new ImageSopInstanceReferenceMacro(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.SourceImageSequence] = null;
					return;
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.SourceImageSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a SourceImageSequence item. Does not modify the SourceImageSequence in the underlying collection.
		/// </summary>
		public ImageSopInstanceReferenceMacro CreateSourceImageSequenceItem()
		{
			var iodBase = new ImageSopInstanceReferenceMacro(new DicomSequenceItem());
			return iodBase;
		}

		/// <summary>
		/// Gets or sets the value of FieldOfViewDimensionsInFloat in the underlying collection. Type 1C.
		/// </summary>
		public double[] FieldOfViewDimensionsInFloat
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FieldOfViewDimensionsInFloat];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var values = new double[dicomAttribute.Count];
				for (var n = 0; n < values.Length; n++)
					values[n] = dicomAttribute.GetFloat64(n, 0);
				return values;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.FieldOfViewDimensionsInFloat] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.FieldOfViewDimensionsInFloat];
				for (var n = 0; n < value.Length; n++)
					dicomAttribute.SetFloat64(n, value[n]);
			}
		}

		/// <summary>
		/// Gets or sets the value of FieldOfViewOrigin in the underlying collection. Type 1C.
		/// </summary>
		public double[] FieldOfViewOrigin
		{
			get
			{
				var result = new double[2];
				if (DicomAttributeProvider[DicomTags.FieldOfViewOrigin].TryGetFloat64(0, out result[0])
				    && DicomAttributeProvider[DicomTags.FieldOfViewOrigin].TryGetFloat64(1, out result[1]))
					return result;
				return null;
			}
			set
			{
				if (value == null || value.Length != 2)
				{
					DicomAttributeProvider[DicomTags.FieldOfViewOrigin] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.FieldOfViewOrigin].SetFloat64(0, value[0]);
				DicomAttributeProvider[DicomTags.FieldOfViewOrigin].SetFloat64(1, value[1]);
			}
		}

		/// <summary>
		/// Gets or sets the value of FieldOfViewRotation in the underlying collection. Type 1C.
		/// </summary>
		public double? FieldOfViewRotation
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.FieldOfViewRotation].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.FieldOfViewRotation] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.FieldOfViewRotation].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of FieldOfViewHorizontalFlip in the underlying collection. Type 1C.
		/// </summary>
		public string FieldOfViewHorizontalFlip
		{
			get { return DicomAttributeProvider[DicomTags.FieldOfViewHorizontalFlip].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.FieldOfViewHorizontalFlip] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.FieldOfViewHorizontalFlip].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of Grid in the underlying collection. Type 1C.
		/// </summary>
		public string Grid
		{
			get { return DicomAttributeProvider[DicomTags.Grid].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.Grid] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.Grid].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of GridAbsorbingMaterial in the underlying collection. Type 3.
		/// </summary>
		public string GridAbsorbingMaterial
		{
			get { return DicomAttributeProvider[DicomTags.GridAbsorbingMaterial].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.GridAbsorbingMaterial] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.GridAbsorbingMaterial].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of GridSpacingMaterial in the underlying collection. Type 3.
		/// </summary>
		public string GridSpacingMaterial
		{
			get { return DicomAttributeProvider[DicomTags.GridSpacingMaterial].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.GridSpacingMaterial] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.GridSpacingMaterial].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of GridThickness in the underlying collection. Type 3.
		/// </summary>
		public double? GridThickness
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.GridThickness].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.GridThickness] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.GridThickness].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of GridPitch in the underlying collection. Type 3.
		/// </summary>
		public double? GridPitch
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.GridPitch].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.GridPitch] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.GridPitch].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of GridAspectRatio in the underlying collection. Type 3.
		/// </summary>
		public int[] GridAspectRatio
		{
			get
			{
				var result = new int[2];
				if (DicomAttributeProvider[DicomTags.GridAspectRatio].TryGetInt32(0, out result[0])
				    && DicomAttributeProvider[DicomTags.GridAspectRatio].TryGetInt32(1, out result[1]))
					return result;
				return null;
			}
			set
			{
				if (value == null || value.Length != 2)
				{
					DicomAttributeProvider[DicomTags.GridAspectRatio] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.GridAspectRatio].SetInt32(0, value[0]);
				DicomAttributeProvider[DicomTags.GridAspectRatio].SetInt32(1, value[1]);
			}
		}

		/// <summary>
		/// Gets or sets the value of GridPeriod in the underlying collection. Type 3.
		/// </summary>
		public double? GridPeriod
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.GridPeriod].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.GridPeriod] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.GridPeriod].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of GridFocalDistance in the underlying collection. Type 3.
		/// </summary>
		public double? GridFocalDistance
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.GridFocalDistance].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.GridFocalDistance] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.GridFocalDistance].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of GridId in the underlying collection. Type 3.
		/// </summary>
		public string GridId
		{
			get { return DicomAttributeProvider[DicomTags.GridId].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.GridId] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.GridId].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of Kvp in the underlying collection. Type 1C.
		/// </summary>
		public double? Kvp
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.Kvp].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.Kvp] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.Kvp].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of XRayTubeCurrentInMa in the underlying collection. Type 1C.
		/// </summary>
		public double? XRayTubeCurrentInMa
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.XRayTubeCurrentInMa].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.XRayTubeCurrentInMa] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.XRayTubeCurrentInMa].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ExposureTimeInMs in the underlying collection. Type 1C.
		/// </summary>
		public double? ExposureTimeInMs
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.ExposureTimeInMs].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.ExposureTimeInMs] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.ExposureTimeInMs].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ExposureInMas in the underlying collection. Type 1C.
		/// </summary>
		public double? ExposureInMas
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.ExposureInMas].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.ExposureInMas] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.ExposureInMas].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ContrastBolusAgent in the underlying collection. Type 1C.
		/// </summary>
		public string ContrastBolusAgent
		{
			get { return DicomAttributeProvider[DicomTags.ContrastBolusAgent].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.ContrastBolusAgent] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.ContrastBolusAgent].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ContrastBolusAgentSequence in the underlying collection. Type 1C.
		/// </summary>
		public CodeSequenceMacro[] ContrastBolusAgentSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.ContrastBolusAgentSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
				{
					return null;
				}

				var result = new CodeSequenceMacro[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new CodeSequenceMacro(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.ContrastBolusAgentSequence] = null;
					return;
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.ContrastBolusAgentSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a ContrastBolusAgentSequence item. Does not modify the ContrastBolusAgentSequence in the underlying collection.
		/// </summary>
		public CodeSequenceMacro CreateContrastBolusAgentSequenceItem()
		{
			var iodBase = new CodeSequenceMacro(new DicomSequenceItem());
			return iodBase;
		}

		/// <summary>
		/// Gets or sets the value of StartAcquisitionDateTime in the underlying collection.  Type 1C.
		/// </summary>
		public DateTime? StartAcquisitionDateTime
		{
			get
			{
				var datetime = DicomAttributeProvider[DicomTags.StartAcquisitionDatetime].GetString(0, string.Empty);
				return DateTimeParser.Parse(datetime);
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.StartAcquisitionDatetime] = null;
					return;
				}
				var datetime = DicomAttributeProvider[DicomTags.StartAcquisitionDatetime];
				datetime.SetStringValue(DateTimeParser.ToDicomString(value.Value, false));
			}
		}

		/// <summary>
		/// Gets or sets the value of EndAcquisitionDatetime in the underlying collection.  Type 1C.
		/// </summary>
		public DateTime? EndAcquisitionDateTime
		{
			get
			{
				var datetime = DicomAttributeProvider[DicomTags.EndAcquisitionDatetime].GetString(0, string.Empty);
				return DateTimeParser.Parse(datetime);
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.EndAcquisitionDatetime] = null;
					return;
				}
				var datetime = DicomAttributeProvider[DicomTags.EndAcquisitionDatetime];
				datetime.SetStringValue(DateTimeParser.ToDicomString(value.Value, false));
			}
		}

		/// <summary>
		/// Gets or sets the value of PrimaryPositionerScanArc in the underlying collection. Type 1C.
		/// </summary>
		public double? PrimaryPositionerScanArc
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.PrimaryPositionerScanArc].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.PrimaryPositionerScanArc] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.PrimaryPositionerScanArc].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PrimaryPositionerScanStartAngle in the underlying collection. Type 1C.
		/// </summary>
		public double? PrimaryPositionerScanStartAngle
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.PrimaryPositionerScanStartAngle].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.PrimaryPositionerScanStartAngle] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.PrimaryPositionerScanStartAngle].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PrimaryPositionerIncrement in the underlying collection. Type 1C.
		/// </summary>
		public double? PrimaryPositionerIncrement
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.PrimaryPositionerIncrement].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.PrimaryPositionerIncrement] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.PrimaryPositionerIncrement].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SecondaryPositionerScanArc in the underlying collection. Type 1C.
		/// </summary>
		public double? SecondaryPositionerScanArc
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.SecondaryPositionerScanArc].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.SecondaryPositionerScanArc] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.SecondaryPositionerScanArc].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SecondaryPositionerScanStartAngle in the underlying collection. Type 1C.
		/// </summary>
		public double? SecondaryPositionerScanStartAngle
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.SecondaryPositionerScanStartAngle].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.SecondaryPositionerScanStartAngle] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.SecondaryPositionerScanStartAngle].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SecondaryPositionerIncrement in the underlying collection. Type 1C.
		/// </summary>
		public double? SecondaryPositionerIncrement
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.SecondaryPositionerIncrement].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.SecondaryPositionerIncrement] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.SecondaryPositionerIncrement].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DistanceSourceToDetector in the underlying collection. Type 1.
		/// </summary>
		public double DistanceSourceToDetector
		{
			get { return DicomAttributeProvider[DicomTags.DistanceSourceToDetector].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.DistanceSourceToDetector].SetFloat64(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of DistanceSourceToPatient in the underlying collection. Type 1.
		/// </summary>
		public double DistanceSourceToPatient
		{
			get { return DicomAttributeProvider[DicomTags.DistanceSourceToPatient].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.DistanceSourceToPatient].SetFloat64(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of EstimatedRadiographicMagnificationFactor in the underlying collection. Type 1.
		/// </summary>
		public double EstimatedRadiographicMagnificationFactor
		{
			get { return DicomAttributeProvider[DicomTags.EstimatedRadiographicMagnificationFactor].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.EstimatedRadiographicMagnificationFactor].SetFloat64(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of AnodeTargetMaterial in the underlying collection. Type 1.
		/// </summary>
		public string AnodeTargetMaterial
		{
			get { return DicomAttributeProvider[DicomTags.AnodeTargetMaterial].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "AnodeTargetMaterial is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.AnodeTargetMaterial].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of BodyPartThickness in the underlying collection. Type 1.
		/// </summary>
		public double BodyPartThickness
		{
			get { return DicomAttributeProvider[DicomTags.BodyPartThickness].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.BodyPartThickness].SetFloat64(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of ExposureControlMode in the underlying collection. Type 1.
		/// </summary>
		public string ExposureControlMode
		{
			get { return DicomAttributeProvider[DicomTags.ExposureControlMode].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "ExposureControlMode is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.ExposureControlMode].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ExposureControlModeDescription in the underlying collection. Type 1.
		/// </summary>
		public string ExposureControlModeDescription
		{
			get { return DicomAttributeProvider[DicomTags.ExposureControlModeDescription].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "ExposureControlModeDescription is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.ExposureControlModeDescription].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of HalfValueLayer in the underlying collection. Type 1.
		/// </summary>
		public double HalfValueLayer
		{
			get { return DicomAttributeProvider[DicomTags.HalfValueLayer].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.HalfValueLayer].SetFloat64(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of FocalSpots in the underlying collection. Type 1.
		/// </summary>
		public double FocalSpots
		{
			get { return DicomAttributeProvider[DicomTags.FocalSpots].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.FocalSpots].SetFloat64(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of DetectorBinning in the underlying collection. Type 1C.
		/// </summary>
		public double[] DetectorBinning
		{
			get
			{
				var result = new double[2];
				if (DicomAttributeProvider[DicomTags.DetectorBinning].TryGetFloat64(0, out result[0])
				    && DicomAttributeProvider[DicomTags.DetectorBinning].TryGetFloat64(1, out result[1]))
					return result;
				return null;
			}
			set
			{
				if (value == null || value.Length != 2)
				{
					DicomAttributeProvider[DicomTags.DetectorBinning] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.DetectorBinning].SetFloat64(0, value[0]);
				DicomAttributeProvider[DicomTags.DetectorBinning].SetFloat64(1, value[1]);
			}
		}

		/// <summary>
		/// Gets or sets the value of DetectorTemperature in the underlying collection. Type 1.
		/// </summary>
		public double DetectorTemperature
		{
			get { return DicomAttributeProvider[DicomTags.DetectorTemperature].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.DetectorTemperature].SetFloat64(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of FilterType in the underlying collection. Type 1.
		/// </summary>
		public string FilterType
		{
			get { return DicomAttributeProvider[DicomTags.FilterType].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "FilterType is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.FilterType].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of FilterMaterial in the underlying collection. Type 1.
		/// </summary>
		public string FilterMaterial
		{
			get { return DicomAttributeProvider[DicomTags.FilterMaterial].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "FilterMaterial is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.FilterMaterial].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of FilterThicknessMinimum in the underlying collection. Type 3.
		/// </summary>
		public double[] FilterThicknessMinimum
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FilterThicknessMinimum];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var values = new double[dicomAttribute.Count];
				for (var n = 0; n < values.Length; n++)
					values[n] = dicomAttribute.GetFloat64(n, 0);
				return values;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.FilterThicknessMinimum] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.FilterThicknessMinimum];
				for (var n = 0; n < value.Length; n++)
					dicomAttribute.SetFloat64(n, value[n]);
			}
		}

		/// <summary>
		/// Gets or sets the value of FilterThicknessMaximum in the underlying collection. Type 3.
		/// </summary>
		public double[] FilterThicknessMaximum
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FilterThicknessMaximum];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var values = new double[dicomAttribute.Count];
				for (var n = 0; n < values.Length; n++)
					values[n] = dicomAttribute.GetFloat64(n, 0);
				return values;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.FilterThicknessMaximum] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.FilterThicknessMaximum];
				for (var n = 0; n < value.Length; n++)
					dicomAttribute.SetFloat64(n, value[n]);
			}
		}

		/// <summary>
		/// Gets or sets the value of FilterBeamPathLengthMinimum in the underlying collection. Type 3.
		/// </summary>
		public double[] FilterBeamPathLengthMinimum
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FilterBeamPathLengthMinimum];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var values = new double[dicomAttribute.Count];
				for (var n = 0; n < values.Length; n++)
					values[n] = dicomAttribute.GetFloat64(n, 0);
				return values;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.FilterBeamPathLengthMinimum] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.FilterBeamPathLengthMinimum];
				for (var n = 0; n < value.Length; n++)
					dicomAttribute.SetFloat64(n, value[n]);
			}
		}

		/// <summary>
		/// Gets or sets the value of FilterBeamPathLengthMaximum in the underlying collection. Type 3.
		/// </summary>
		public double[] FilterBeamPathLengthMaximum
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FilterBeamPathLengthMaximum];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var values = new double[dicomAttribute.Count];
				for (var n = 0; n < values.Length; n++)
					values[n] = dicomAttribute.GetFloat64(n, 0);
				return values;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.FilterBeamPathLengthMaximum] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.FilterBeamPathLengthMaximum];
				for (var n = 0; n < value.Length; n++)
					dicomAttribute.SetFloat64(n, value[n]);
			}
		}
	}

	/// <summary>
	/// Breast Tomosynthesis Per Projection Acquisition Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.21.3.4 (Table C.8.21.3.4-1)</remarks>
	public class BreastTomosynthesisPerProjectionAcquisitionSequenceItem : SequenceIodBase, IXRay3DGeneralPerProjectionAcquisitionMacro, IExposureIndexMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BreastTomosynthesisPerProjectionAcquisitionSequenceItem"/> class.
		/// </summary>
		public BreastTomosynthesisPerProjectionAcquisitionSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="BreastTomosynthesisPerProjectionAcquisitionSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public BreastTomosynthesisPerProjectionAcquisitionSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		void IIodMacro.InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of Kvp in the underlying collection. Type 1C.
		/// </summary>
		public double? Kvp
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.Kvp].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.Kvp] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.Kvp].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of XRayTubeCurrentInMa in the underlying collection. Type 1C.
		/// </summary>
		public double? XRayTubeCurrentInMa
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.XRayTubeCurrentInMa].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.XRayTubeCurrentInMa] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.XRayTubeCurrentInMa].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of FrameAcquisitionDuration in the underlying collection. Type 1C.
		/// </summary>
		public double? FrameAcquisitionDuration
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.FrameAcquisitionDuration].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.FrameAcquisitionDuration] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.FrameAcquisitionDuration].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CollimatorShape in the underlying collection. Type 1C.
		/// </summary>
		public string CollimatorShape
		{
			get { return DicomAttributeProvider[DicomTags.CollimatorShape].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.CollimatorShape] = null;
					return;
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

		/// <summary>
		/// Gets or sets the value of PositionerPrimaryAngle in the underlying collection. Type 1.
		/// </summary>
		public double PositionerPrimaryAngle
		{
			get { return DicomAttributeProvider[DicomTags.PositionerPrimaryAngle].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.PositionerPrimaryAngle].SetFloat64(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of PositionerSecondaryAngle in the underlying collection. Type 1C.
		/// </summary>
		public double? PositionerSecondaryAngle
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.PositionerSecondaryAngle].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.PositionerSecondaryAngle] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.PositionerSecondaryAngle].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ExposureTimeInMs in the underlying collection. Type 1.
		/// </summary>
		public double ExposureTimeInMs
		{
			get { return DicomAttributeProvider[DicomTags.ExposureTimeInMs].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.ExposureTimeInMs].SetFloat64(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of ExposureInMas in the underlying collection. Type 1.
		/// </summary>
		public double ExposureInMas
		{
			get { return DicomAttributeProvider[DicomTags.ExposureInMas].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.ExposureInMas].SetFloat64(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of RelativeXRayExposure in the underlying collection. Type 1.
		/// </summary>
		public int RelativeXRayExposure
		{
			get { return DicomAttributeProvider[DicomTags.RelativeXRayExposure].GetInt32(0, 0); }
			set { DicomAttributeProvider[DicomTags.RelativeXRayExposure].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of OrganDose in the underlying collection. Type 3.
		/// </summary>
		public double? OrganDose
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.OrganDose].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.OrganDose] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.OrganDose].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of EntranceDoseInMgy in the underlying collection. Type 3.
		/// </summary>
		public double? EntranceDoseInMgy
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.EntranceDoseInMgy].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.EntranceDoseInMgy] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.EntranceDoseInMgy].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ExposureIndex in the underlying collection. Type 3.
		/// </summary>
		public double? ExposureIndex
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.ExposureIndex].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.ExposureIndex] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.ExposureIndex].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of TargetExposureIndex in the underlying collection. Type 3.
		/// </summary>
		public double? TargetExposureIndex
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.TargetExposureIndex].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.TargetExposureIndex] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.TargetExposureIndex].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DeviationIndex in the underlying collection. Type 3.
		/// </summary>
		public double? DeviationIndex
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.DeviationIndex].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.DeviationIndex] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.DeviationIndex].SetFloat64(0, value.Value);
			}
		}
	}
}
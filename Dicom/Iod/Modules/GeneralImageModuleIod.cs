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
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// GeneralImage Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.1 (Table C.7-9)</remarks>
	public class GeneralImageModuleIod : IodBase
	{
		private const string _enumYes = "YES";
		private const string _enumNo = "NO";
		private const string _enumLossy = "01";
		private const string _enumLossless = "00";

		/// <summary>
		/// Initializes a new instance of the <see cref="GeneralImageModuleIod"/> class.
		/// </summary>	
		public GeneralImageModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="GeneralImageModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute collection.</param>
		public GeneralImageModuleIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.InstanceNumber;
				yield return DicomTags.PatientOrientation;
				yield return DicomTags.ContentDate;
				yield return DicomTags.ContentTime;
				yield return DicomTags.ImageType;
				yield return DicomTags.AcquisitionNumber;
				yield return DicomTags.AcquisitionDate;
				yield return DicomTags.AcquisitionTime;
				yield return DicomTags.AcquisitionDatetime;
				yield return DicomTags.ReferencedImageSequence;
				yield return DicomTags.DerivationDescription;
				yield return DicomTags.DerivationCodeSequence;
				yield return DicomTags.SourceImageSequence;
				yield return DicomTags.ReferencedInstanceSequence;
				yield return DicomTags.ImagesInAcquisition;
				yield return DicomTags.ImageComments;
				yield return DicomTags.QualityControlImage;
				yield return DicomTags.BurnedInAnnotation;
				yield return DicomTags.RecognizableVisualFeatures;
				yield return DicomTags.LossyImageCompression;
				yield return DicomTags.LossyImageCompressionRatio;
				yield return DicomTags.LossyImageCompressionMethod;
				yield return DicomTags.IconImageSequence;
				yield return DicomTags.PresentationLutShape;
				yield return DicomTags.IrradiationEventUid;
			}
		}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			InstanceNumber = 1;
		}

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public bool HasValues()
		{
			return !(IsNullOrEmpty(InstanceNumber)
			         && IsNullOrEmpty(PatientOrientation)
			         && IsNullOrEmpty(ContentDateTime)
			         && IsNullOrEmpty(ImageType)
			         && IsNullOrEmpty(AcquisitionNumber)
			         && IsNullOrEmpty(AcquisitionDateTime)
			         && IsNullOrEmpty(ReferencedImageSequence)
			         && IsNullOrEmpty(DerivationDescription)
			         && IsNullOrEmpty(DerivationCodeSequence)
			         && IsNullOrEmpty(SourceImageSequence)
			         && IsNullOrEmpty(ReferencedInstanceSequence)
			         && IsNullOrEmpty(ImagesInAcquisition)
			         && IsNullOrEmpty(ImageComments)
			         && IsNullOrEmpty(QualityControlImage)
			         && IsNullOrEmpty(BurnedInAnnotation)
			         && IsNullOrEmpty(RecognizableVisualFeatures)
			         && IsNullOrEmpty(LossyImageCompression)
			         && IsNullOrEmpty(LossyImageCompressionRatio)
			         && IsNullOrEmpty(LossyImageCompressionMethod)
			         && IsNullOrEmpty(IconImageSequence)
			         && IsNullOrEmpty(PresentationLutShape)
			         && IsNullOrEmpty(IrradiationEventUid));
		}

		/// <summary>
		/// Gets or sets the value of InstanceNumber in the underlying collection. Type 2.
		/// </summary>
		public int? InstanceNumber
		{
			get
			{
				int result;
				if (DicomAttributeProvider[DicomTags.InstanceNumber].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.InstanceNumber].SetNullValue();
					return;
				}
				DicomAttributeProvider[DicomTags.InstanceNumber].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PatientOrientation in the underlying collection. Type 2C.
		/// </summary>
		public string PatientOrientation
		{
			// TODO: make it easier to specify values
			get { return DicomAttributeProvider[DicomTags.PatientOrientation].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.PatientOrientation] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.PatientOrientation].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ContentDate and ContentTime in the underlying collection.  Type 2C.
		/// </summary>
		public DateTime? ContentDateTime
		{
			get
			{
				var date = DicomAttributeProvider[DicomTags.ContentDate].GetString(0, string.Empty);
				var time = DicomAttributeProvider[DicomTags.ContentTime].GetString(0, string.Empty);
				return DateTimeParser.ParseDateAndTime(string.Empty, date, time);
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.ContentDate] = null;
					DicomAttributeProvider[DicomTags.ContentTime] = null;
					return;
				}
				var date = DicomAttributeProvider[DicomTags.ContentDate];
				var time = DicomAttributeProvider[DicomTags.ContentTime];
				DateTimeParser.SetDateTimeAttributeValues(value, date, time);
			}
		}

		/// <summary>
		/// Gets or sets the value of ImageType in the underlying collection. Type 3.
		/// </summary>
		public string ImageType
		{
			// TODO: make it easier to specify values
			get { return DicomAttributeProvider[DicomTags.ImageType].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.ImageType] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.ImageType].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of AcquisitionNumber in the underlying collection. Type 3.
		/// </summary>
		public int? AcquisitionNumber
		{
			get
			{
				int result;
				if (DicomAttributeProvider[DicomTags.AcquisitionNumber].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.AcquisitionNumber] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.AcquisitionNumber].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of AcquisitionDate, AcquisitionTime and/or AcquisitionDatetime in the underlying collection.  Type 3.
		/// </summary>
		public DateTime? AcquisitionDateTime
		{
			get
			{
				var date = DicomAttributeProvider[DicomTags.AcquisitionDate].GetString(0, string.Empty);
				var time = DicomAttributeProvider[DicomTags.AcquisitionTime].GetString(0, string.Empty);
				var datm = DicomAttributeProvider[DicomTags.AcquisitionDatetime].GetString(0, string.Empty);
				return DateTimeParser.ParseDateAndTime(datm, date, time);
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.AcquisitionDate] = null;
					DicomAttributeProvider[DicomTags.AcquisitionTime] = null;
					DicomAttributeProvider[DicomTags.AcquisitionDatetime] = null;
					return;
				}
				var date = DicomAttributeProvider[DicomTags.AcquisitionDate];
				var time = DicomAttributeProvider[DicomTags.AcquisitionTime];
				var datm = DicomAttributeProvider[DicomTags.AcquisitionDatetime];
				DateTimeParser.SetDateTimeAttributeValues(value, datm, date, time);
			}
		}

		/// <summary>
		/// Gets or sets the value of ReferencedImageSequence in the underlying collection. Type 3.
		/// </summary>
		public ReferencedImageSequence[] ReferencedImageSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.ReferencedImageSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
				{
					return null;
				}

				var result = new ReferencedImageSequence[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new ReferencedImageSequence(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.ReferencedImageSequence] = null;
					return;
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.ReferencedImageSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a ReferencedImageSequence item. Does not modify the ReferencedImageSequence in the underlying collection.
		/// </summary>
		public ReferencedImageSequence CreateReferencedImageSequence()
		{
			var iodBase = new ReferencedImageSequence(new DicomSequenceItem());
			return iodBase;
		}

		/// <summary>
		/// Gets or sets the value of DerivationDescription in the underlying collection. Type 3.
		/// </summary>
		public string DerivationDescription
		{
			get { return DicomAttributeProvider[DicomTags.DerivationDescription].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.DerivationDescription] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.DerivationDescription].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DerivationCodeSequence in the underlying collection. Type 3.
		/// </summary>
		public CodeSequenceMacro[] DerivationCodeSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.DerivationCodeSequence];
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
					DicomAttributeProvider[DicomTags.DerivationCodeSequence] = null;
					return;
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.DerivationCodeSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a DerivationCodeSequence item. Does not modify the DerivationCodeSequence in the underlying collection.
		/// </summary>
		public CodeSequenceMacro CreateDerivationCodeSequence()
		{
			var iodBase = new CodeSequenceMacro(new DicomSequenceItem());
			return iodBase;
		}

		/// <summary>
		/// Gets or sets the value of SourceImageSequence in the underlying collection. Type 3.
		/// </summary>
		public SourceImageSequence[] SourceImageSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.SourceImageSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
				{
					return null;
				}

				var result = new SourceImageSequence[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new SourceImageSequence(items[n]);

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
		public SourceImageSequence CreateSourceImageSequence()
		{
			var iodBase = new SourceImageSequence(new DicomSequenceItem());
			return iodBase;
		}

		/// <summary>
		/// Gets or sets the value of ReferencedInstanceSequence in the underlying collection. Type 3.
		/// </summary>
		public ReferencedInstanceSequenceIod[] ReferencedInstanceSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.ReferencedInstanceSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
				{
					return null;
				}

				var result = new ReferencedInstanceSequenceIod[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new ReferencedInstanceSequenceIod(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.ReferencedInstanceSequence] = null;
					return;
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.ReferencedInstanceSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a ReferencedInstanceSequence item. Does not modify the ReferencedInstanceSequence in the underlying collection.
		/// </summary>
		public ReferencedInstanceSequenceIod CreateReferencedInstanceSequence()
		{
			var iodBase = new ReferencedInstanceSequenceIod(new DicomSequenceItem());
			return iodBase;
		}

		/// <summary>
		/// Gets or sets the value of ImagesInAcquisition in the underlying collection. Type 3.
		/// </summary>
		public int? ImagesInAcquisition
		{
			get
			{
				int result;
				if (DicomAttributeProvider[DicomTags.ImagesInAcquisition].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.ImagesInAcquisition] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.ImagesInAcquisition].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ImageComments in the underlying collection. Type 3.
		/// </summary>
		public string ImageComments
		{
			get { return DicomAttributeProvider[DicomTags.ImageComments].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.ImageComments] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.ImageComments].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of QualityControlImage in the underlying collection. Type 3.
		/// </summary>
		public bool? QualityControlImage
		{
			get { return ParseBool(DicomAttributeProvider[DicomTags.QualityControlImage].GetString(0, string.Empty), _enumYes, _enumNo); }
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.QualityControlImage] = null;
					return;
				}
				SetAttributeFromBool(DicomAttributeProvider[DicomTags.QualityControlImage], value, _enumYes, _enumNo);
			}
		}

		/// <summary>
		/// Gets or sets the value of BurnedInAnnotation in the underlying collection. Type 3.
		/// </summary>
		public bool? BurnedInAnnotation
		{
			get { return ParseBool(DicomAttributeProvider[DicomTags.BurnedInAnnotation].GetString(0, string.Empty), _enumYes, _enumNo); }
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.BurnedInAnnotation] = null;
					return;
				}
				SetAttributeFromBool(DicomAttributeProvider[DicomTags.BurnedInAnnotation], value, _enumYes, _enumNo);
			}
		}

		/// <summary>
		/// Gets or sets the value of RecognizableVisualFeatures in the underlying collection. Type 3.
		/// </summary>
		public bool? RecognizableVisualFeatures
		{
			get { return ParseBool(DicomAttributeProvider[DicomTags.RecognizableVisualFeatures].GetString(0, string.Empty), _enumYes, _enumNo); }
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.RecognizableVisualFeatures] = null;
					return;
				}
				SetAttributeFromBool(DicomAttributeProvider[DicomTags.RecognizableVisualFeatures], value, _enumYes, _enumNo);
			}
		}

		/// <summary>
		/// Gets or sets the value of LossyImageCompression in the underlying collection. Type 3.
		/// </summary>
		public bool? LossyImageCompression
		{
			get { return ParseBool(DicomAttributeProvider[DicomTags.LossyImageCompression].GetString(0, string.Empty), _enumLossy, _enumLossless); }
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.LossyImageCompression] = null;
					return;
				}
				SetAttributeFromBool(DicomAttributeProvider[DicomTags.LossyImageCompression], value, _enumLossy, _enumLossless);
			}
		}

		/// <summary>
		/// Gets or sets the value of LossyImageCompressionRatio in the underlying collection. Type 3.
		/// </summary>
		public double[] LossyImageCompressionRatio
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.LossyImageCompressionRatio];
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
					DicomAttributeProvider[DicomTags.LossyImageCompressionRatio] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.LossyImageCompressionRatio];
				for (var n = 0; n < value.Length; n++)
					dicomAttribute.SetFloat64(n, value[n]);
			}
		}

		/// <summary>
		/// Gets or sets the value of LossyImageCompressionMethod in the underlying collection. Type 3.
		/// </summary>
		public string LossyImageCompressionMethod
		{
			get { return DicomAttributeProvider[DicomTags.LossyImageCompressionMethod].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.LossyImageCompressionMethod] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.LossyImageCompressionMethod].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PresentationLutShape in the underlying collection. Type 3.
		/// </summary>
		public PresentationLutShape PresentationLutShape
		{
			get { return ParseEnum(DicomAttributeProvider[DicomTags.PresentationLutShape].GetString(0, string.Empty), PresentationLutShape.None); }
			set
			{
				if (value == PresentationLutShape.None)
				{
					DicomAttributeProvider[DicomTags.PresentationLutShape] = null;
					return;
				}
				SetAttributeFromEnum(DicomAttributeProvider[DicomTags.PresentationLutShape], value);
			}
		}

		/// <summary>
		/// Gets or sets the value of IconImageSequence in the underlying collection. Type 3.
		/// </summary>
		public IconImageSequence IconImageSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.IconImageSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
				{
					return null;
				}
				return new IconImageSequence(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.IconImageSequence];
				if (value == null)
				{
					DicomAttributeProvider[DicomTags.IconImageSequence] = null;
					return;
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the IconImageSequence in the underlying collection. Type 3.
		/// </summary>
		public IconImageSequence CreateIconImageSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.IconImageSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new IconImageSequence(dicomSequenceItem);
				return sequenceType;
			}
			return new IconImageSequence(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}

		/// <summary>
		/// Gets or sets the value of IrradiationEventUid in the underlying collection. Type 3.
		/// </summary>
		public string IrradiationEventUid
		{
			get { return DicomAttributeProvider[DicomTags.IrradiationEventUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.IrradiationEventUid] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.IrradiationEventUid].SetString(0, value);
			}
		}
	}

	#region PresentationLutShape Enum

	/// <summary>
	/// When present, specifies an identity transformation for the Presentation LUT such that the 
	/// output of all grayscale transformations, if any, are defined to be in P-Values.
	/// <para>
	/// When this attribute is used with a color photometric interpretation then the
	/// luminance component is in P-Values.</para>
	/// </summary>
	public enum PresentationLutShape
	{
		/// <summary>
		/// 
		/// </summary>
		None,

		/// <summary>
		/// output is in P-Values - shall be used if Photometric Interpretation 
		/// (0028,0004) is MONOCHROME2 or any color photometric interpretation.
		/// </summary>
		Identity,

		/// <summary>
		/// output after inversion is in PValues - shall be used if Photometric 
		/// Interpretation (0028,0004) is MONOCHROME1.
		/// </summary>
		Inverse
	}

	#endregion
}
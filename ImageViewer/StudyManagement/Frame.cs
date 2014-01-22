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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom.Validation;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Represents the DICOM concept of a frame.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The properties of this class, as well as its implementation of <see cref="IDicomAttributeProvider"/>,
	/// represent a frame-normalized view of the attributes of the parent <see cref="ImageSop"/>. When the SOP
	/// instance represents a multi-frame image, DICOM tags which have a singular invocation in the Multi-Frame
	/// Functional Groups Module will be mapped to the appropriate functional group instead of the root of the data set.
	/// </para>
	/// <para>
	/// Additionally, some properties (such as <see cref="ImageType"/>) will try to map to a different tag that represents the
	/// frame-level version of the same concept if possible (e.g. <see cref="ImageType"/> maps to <see cref="DicomTags.FrameType"/>
	/// in the functional groups first and, if that is unavailable, falls back to <see cref="DicomTags.ImageType"/>
	/// at the root level. This behaviour does not occur when accessing attributes directly via the indexers or the
	/// <see cref="IDicomAttributeProvider"/> implementation - <see cref="DicomTags.FrameType"/> will only map to the
	/// functional group, and <see cref="DicomTags.ImageType"/> will only map to the root of the data set.
	/// </para>
	/// </remarks>
	public partial class Frame : IDicomAttributeProvider, IDisposable
	{
		#region Private fields

		private readonly ImageSop _parentImageSop;
		private readonly int _frameNumber;

		private readonly object _syncLock = new object();
		private volatile NormalizedPixelSpacing _normalizedPixelSpacing;
		private volatile ImagePlaneHelper _imagePlaneHelper;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="Frame"/> with the
		/// specified parameters.
		/// </summary>
		/// <param name="parentImageSop">The parent <see cref="ImageSop"/>.</param>
		/// <param name="frameNumber">The first frame is frame 1.</param>
		protected internal Frame(ImageSop parentImageSop, int frameNumber)
		{
			Platform.CheckForNullReference(parentImageSop, "parentImageSop");
			Platform.CheckPositive(frameNumber, "frameNumber");
			_parentImageSop = parentImageSop;
			_frameNumber = frameNumber;
		}

		/// <summary>
		/// Gets the parent <see cref="ImageSop"/>.
		/// </summary>
		public ImageSop ParentImageSop
		{
			get { return _parentImageSop; }
		}

		/// <summary>
		/// Gets the Study Instance UID.
		/// </summary>
		public string StudyInstanceUid
		{
			get { return ParentImageSop.StudyInstanceUid; }
		}

		/// <summary>
		/// Gets the Series Instance UID.
		/// </summary>
		public string SeriesInstanceUid
		{
			get { return ParentImageSop.SeriesInstanceUid; }
		}

		/// <summary>
		/// Gets the SOP Instance UID.
		/// </summary>
		public string SopInstanceUid
		{
			get { return ParentImageSop.SopInstanceUid; }
		}

		/// <summary>
		/// Gets the Frame Number.
		/// </summary>
		public int FrameNumber
		{
			get { return _frameNumber; }
		}

		#region General Image Module

		/// <summary>
		/// Gets the patient orientation in the frame.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If the <see cref="DicomTags.PatientOrientation"/> is neither included in a functional group nor the root data set,
		/// an equivalent patient orientation will be inferred from <see cref="DicomTags.ImageOrientationPatient"/> where available.
		/// </para>
		/// <para>
		/// A <see cref="Dicom.Iod.PatientOrientation"/> instance is returned even when no data is available;
		/// the <see cref="Dicom.Iod.PatientOrientation.IsEmpty"/> property will be true.
		/// </para>
		/// </remarks>
		public virtual PatientOrientation PatientOrientation
		{
			get
			{
				var patientOrientation = _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.PatientOrientation).ToString();
				return PatientOrientation.FromString(patientOrientation, _parentImageSop.AnatomicalOrientationType) ?? ImageOrientationPatient.ToPatientOrientation();
			}
		}

		/// <summary>
		/// Gets the frame type as a DICOM multi-valued string.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This property is named ImageType for historical reasons; the value of the <see cref="DicomTags.FrameType"/>
		/// attribute in the functional groups will be returned if available.
		/// </para>
		/// </remarks>
		public virtual string ImageType
		{
			get
			{
				DicomAttribute dicomAttribute;
				if (_parentImageSop.TryGetFrameAttribute(_frameNumber, DicomTags.FrameType, out dicomAttribute) && !dicomAttribute.IsEmpty)
					return dicomAttribute.ToString();
				return _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.ImageType).ToString();
			}
		}

		/// <summary>
		/// Gets the acquisition number for the frame.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This property will return the value of the <see cref="DicomTags.FrameAcquisitionNumber"/>
		/// attribute in the functional groups if available.
		/// </para>
		/// </remarks>
		public virtual int AcquisitionNumber
		{
			get
			{
				DicomAttribute dicomAttribute;
				if (_parentImageSop.TryGetFrameAttribute(_frameNumber, DicomTags.FrameAcquisitionNumber, out dicomAttribute) && !dicomAttribute.IsEmpty)
					return dicomAttribute.GetInt32(0, 0);
				return _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.AcquisitionNumber).GetInt32(0, 0);
			}
		}

		/// <summary>
		/// Gets the acquisiton date for the frame.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This property will return the date component of the <see cref="DicomTags.FrameAcquisitionDatetime"/>
		/// attribute in the functional groups if available, or the date component of <see cref="DicomTags.AcquisitionDatetime"/>,
		/// or the value of the <see cref="DicomTags.AcquisitionDate"/> attribute.
		/// </para>
		/// </remarks>
		public virtual string AcquisitionDate
		{
			get
			{
				DicomAttribute dicomAttribute;
				if (_parentImageSop.TryGetFrameAttribute(_frameNumber, DicomTags.FrameAcquisitionDatetime, out dicomAttribute) && !dicomAttribute.IsEmpty)
					return DateTimeParser.GetDateAttributeValues(dicomAttribute.GetString(0, string.Empty));
				return DateTimeParser.GetDateAttributeValues(this, DicomTags.AcquisitionDatetime, DicomTags.AcquisitionDate);
			}
		}

		/// <summary>
		/// Gets the acquisition time for the frame.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This property will return the time component of the <see cref="DicomTags.FrameAcquisitionDatetime"/>
		/// attribute in the functional groups if available, or the time component of <see cref="DicomTags.AcquisitionDatetime"/>,
		/// or the value of the <see cref="DicomTags.AcquisitionTime"/> attribute.
		/// </para>
		/// </remarks>
		public virtual string AcquisitionTime
		{
			get
			{
				DicomAttribute dicomAttribute;
				if (_parentImageSop.TryGetFrameAttribute(_frameNumber, DicomTags.FrameAcquisitionDatetime, out dicomAttribute) && !dicomAttribute.IsEmpty)
					return DateTimeParser.GetTimeAttributeValues(dicomAttribute.GetString(0, string.Empty));
				return DateTimeParser.GetTimeAttributeValues(this, DicomTags.AcquisitionDatetime, DicomTags.AcquisitionTime);
			}
		}

		/// <summary>
		/// Gets the acquisition date/time for the frame.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This property will return the value of the <see cref="DicomTags.FrameAcquisitionDatetime"/>
		/// attribute in the functional groups if available, or the <see cref="DicomTags.AcquisitionDatetime"/>,
		/// or a concatenation of the <see cref="DicomTags.AcquisitionDate"/> and <see cref="DicomTags.AcquisitionTime"/>
		/// attributes.
		/// </para>
		/// </remarks>
		public virtual string AcquisitionDateTime
		{
			get
			{
				DicomAttribute dicomAttribute;
				if (_parentImageSop.TryGetFrameAttribute(_frameNumber, DicomTags.FrameAcquisitionDatetime, out dicomAttribute) && !dicomAttribute.IsEmpty)
					return dicomAttribute.GetString(0, string.Empty);
				return DateTimeParser.GetDateTimeAttributeValues(this, DicomTags.AcquisitionDatetime, DicomTags.AcquisitionDate, DicomTags.AcquisitionTime);
			}
		}

		/// <summary>
		/// Gets the number of images in the acquisition.
		/// </summary>
		public virtual int ImagesInAcquisition
		{
			get { return _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.ImagesInAcquisition).GetInt32(0, 0); }
		}

		/// <summary>
		/// Gets the comments for the frame.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This property will return the value of the <see cref="DicomTags.FrameComments"/>
		/// attribute in the functional groups if available.
		/// </para>
		/// </remarks>
		public virtual string ImageComments
		{
			get
			{
				DicomAttribute dicomAttribute;
				if (_parentImageSop.TryGetFrameAttribute(_frameNumber, DicomTags.FrameComments, out dicomAttribute) && !dicomAttribute.IsEmpty)
					return dicomAttribute.ToString();
				return _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.ImageComments).ToString();
			}
		}

		/// <summary>
		/// Gets the the lossy image compression code value.
		/// </summary>
		public virtual string LossyImageCompression
		{
			get { return _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.LossyImageCompression).GetString(0, null) ?? string.Empty; }
		}

		/// <summary>
		/// Gets the lossy image compression ratio.
		/// </summary>
		/// <remarks>
		/// Will return as many parsable values as possible up to the first non-parsable value.  For example, if there are 3 values, but the last one is poorly encoded, 2 values will be returned.
		/// </remarks>
		public virtual double[] LossyImageCompressionRatio
		{
			get
			{
				var lossyImageCompressionRatios = _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.LossyImageCompressionRatio).ToString();

				double[] values;
				DicomStringHelper.TryGetDoubleArray(lossyImageCompressionRatios, out values);
				return values ?? new double[0];
			}
		}

		#endregion

		#region Image Plane Module

		/// <summary>
		/// Gets the pixel spacing.
		/// </summary>
		/// <remarks>
		/// It is generally recommended that clients use <see cref="NormalizedPixelSpacing"/> when
		/// performing calculations involving pixel spacing, as this property does not account for
		/// alternate sources of pixel spacing information.
		/// </remarks>
		public virtual PixelSpacing PixelSpacing
		{
			get
			{
				var pixelSpacing = _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.PixelSpacing).ToString();
				return PixelSpacing.FromString(pixelSpacing) ?? PixelSpacing.Zero;
			}
		}

		/// <summary>
		/// Gets the image orientation patient.
		/// </summary>
		/// <remarks>
		/// Returns an <see cref="Dicom.Iod.ImageOrientationPatient"/> object with zero for all its values
		/// when no data is available or the existing data is bad/incorrect;  <see cref="Dicom.Iod.ImageOrientationPatient.IsNull"/>
		/// will be true.
		/// </remarks>
		public virtual ImageOrientationPatient ImageOrientationPatient
		{
			get
			{
				var imageOrientationPatient = _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.ImageOrientationPatient).ToString();
				return ImageOrientationPatient.FromString(imageOrientationPatient) ?? new ImageOrientationPatient(0, 0, 0, 0, 0, 0);
			}
		}

		/// <summary>
		/// Gets the image position patient.
		/// </summary>
		/// <remarks>
		/// Returns an <see cref="Dicom.Iod.ImagePositionPatient"/> object with zero for all its values when no data is
		/// available or the existing data is bad/incorrect; <see cref="Dicom.Iod.ImagePositionPatient.IsNull"/> will be true.
		/// </remarks>
		public virtual ImagePositionPatient ImagePositionPatient
		{
			get
			{
				var imagePositionPatient = _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.ImagePositionPatient).ToString();
				return ImagePositionPatient.FromString(imagePositionPatient) ?? new ImagePositionPatient(0, 0, 0);
			}
		}

		/// <summary>
		/// Gets the slice thickness of the frame.
		/// </summary>
		public virtual double SliceThickness
		{
			get { return _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.SliceThickness).GetFloat64(0, 0); }
		}

		/// <summary>
		/// Gets the slice location of the frame.
		/// </summary>
		public virtual double SliceLocation
		{
			get { return _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.SliceLocation).GetFloat64(0, 0); }
		}

		#endregion

		#region X-ray Acquisition Module

		/// <summary>
		/// Gets the imager pixel spacing.
		/// </summary>
		/// <remarks>
		/// It is generally recommended that clients use <see cref="NormalizedPixelSpacing"/> when
		/// performing calculations involving pixel spacing, as this property does not account for
		/// alternate sources of pixel spacing information.
		/// </remarks>
		public virtual PixelSpacing ImagerPixelSpacing
		{
			get
			{
				var imagerPixelSpacing = _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.ImagerPixelSpacing).ToString();
				return PixelSpacing.FromString(imagerPixelSpacing) ?? PixelSpacing.Zero;
			}
		}

		#endregion

		#region Image Pixel Module

		/// <summary>
		/// Gets the number of samples per pixel.
		/// </summary>
		public virtual int SamplesPerPixel
		{
			get { return _parentImageSop[DicomTags.SamplesPerPixel].GetUInt16(0, 0); }
		}

		/// <summary>
		/// Gets the photometric interpretation.
		/// </summary>
		public virtual PhotometricInterpretation PhotometricInterpretation
		{
			get { return PhotometricInterpretation.FromCodeString(_parentImageSop[DicomTags.PhotometricInterpretation].GetString(0, null)); }
		}

		/// <summary>
		/// Gets the number of rows.
		/// </summary>
		public virtual int Rows
		{
			get { return _parentImageSop[DicomTags.Rows].GetUInt16(0, 0); }
		}

		/// <summary>
		/// Gets the number of columns.
		/// </summary>
		public virtual int Columns
		{
			get { return _parentImageSop[DicomTags.Columns].GetUInt16(0, 0); }
		}

		/// <summary>
		/// Gets the number of bits allocated.
		/// </summary>
		public virtual int BitsAllocated
		{
			get { return _parentImageSop[DicomTags.BitsAllocated].GetUInt16(0, 0); }
		}

		/// <summary>
		/// Gets the number of bits stored.
		/// </summary>
		public virtual int BitsStored
		{
			get { return _parentImageSop[DicomTags.BitsStored].GetUInt16(0, 0); }
		}

		/// <summary>
		/// Gets the index of the high bit.
		/// </summary>
		public virtual int HighBit
		{
			get { return _parentImageSop[DicomTags.HighBit].GetUInt16(0, 0); }
		}

		/// <summary>
		/// Gets the pixel representation.
		/// </summary>
		public virtual int PixelRepresentation
		{
			get { return _parentImageSop[DicomTags.PixelRepresentation].GetUInt16(0, 0); }
		}

		/// <summary>
		/// Gets the planar configuration.
		/// </summary>
		public virtual int PlanarConfiguration
		{
			get { return _parentImageSop[DicomTags.PlanarConfiguration].GetUInt16(0, 0); }
		}

		/// <summary>
		/// Gets the pixel aspect ratio.
		/// </summary>
		/// <remarks>
		/// If no value exists in the image header, or the value is invalid, a <see cref="ClearCanvas.Dicom.Iod.PixelAspectRatio"/>
		/// is returned whose <see cref="ClearCanvas.Dicom.Iod.PixelAspectRatio.IsNull"/> property evaluates to true.
		/// </remarks>
		public virtual PixelAspectRatio PixelAspectRatio
		{
			get
			{
				var pixelAspectRatio = _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.PixelAspectRatio).ToString();
				return PixelAspectRatio.FromString(pixelAspectRatio) ?? new PixelAspectRatio(0, 0);
			}
		}

		#endregion

		#region Modality LUT Module

		/// <summary>
		/// Gets the rescale intercept for the frame.
		/// </summary>
		public virtual double RescaleIntercept
		{
			get { return _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.RescaleIntercept).GetFloat64(0, 0); }
		}

		/// <summary>
		/// Gets the rescale slope for the frame.
		/// </summary>
		/// <remarks>
		/// If the frame does not specify a slope, the default slope of 1.0 will be returned.
		/// </remarks>
		public virtual double RescaleSlope
		{
			get
			{
				// ReSharper disable CompareOfFloatsByEqualityOperator
				var rescaleSlope = _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.RescaleSlope).GetFloat64(0, 1);
				return rescaleSlope == 0.0 ? 1.0 : rescaleSlope;
				// ReSharper restore CompareOfFloatsByEqualityOperator
			}
		}

		/// <summary>
		/// Gets the rescale type for the frame.
		/// </summary>
		public virtual string RescaleType
		{
			get { return _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.RescaleType).GetString(0, null) ?? string.Empty; }
		}

		/// <summary>
		/// Gets the units of the rescale function output.
		/// </summary>
		public RescaleUnits RescaleUnits
		{
			get { return RescaleUnits.GetRescaleUnits(this); }
		}

		/// <summary>
		/// Gets a value indicating whether or not the rescale function is subnormal (i.e. output of the function is too small to be represented as distinct values).
		/// </summary>
		public bool IsSubnormalRescale
		{
			// function is subnormal if slope is such that 2**BS distinct values all map to a single distinct integer
			get { return RescaleSlope < 1.0/(1 << BitsStored); }
		}

		#endregion

		#region VOI LUT Module

		/// <summary>
		/// Gets the VOI data LUTs applicable to this frame.
		/// </summary>
		public virtual IList<VoiDataLut> VoiDataLuts
		{
			get { return _parentImageSop.GetFrameVoiDataLuts(_frameNumber); }
		}

		/// <summary>
		/// Gets the VOI window width/center values, and their respective explanations, applicable to this frame.
		/// </summary>
		/// <remarks>
		/// Will return as many parsable values as possible up to the first non-parsable value.
		/// For example, if there are 3 values, but the last one is poorly encoded, 2 values will be returned.
		/// </remarks>
		public virtual IList<VoiWindow> VoiWindows
		{
			get { return VoiWindow.GetWindows(this).ToList(); }
		}

		/// <summary>
		/// Gets an array of the VOI window width/center values, and their respective explanations, applicable to this frame.
		/// </summary>
		/// <remarks>
		/// Will return as many parsable values as possible up to the first non-parsable value.
		/// For example, if there are 3 values, but the last one is poorly encoded, 2 values will be returned.
		/// </remarks>
		/// <seealso cref="VoiWindows"/>
		public virtual Window[] WindowCenterAndWidth
		{
			// The equivalent parse method in Window throws an excpetion if there are any mismatched width/center pairs
			get { return VoiWindow.GetWindows(this).Select(w => (Window) w).ToArray(); }
		}

		/// <summary>
		/// Gets the window width and center explanation(s).
		/// </summary>
		/// <seealso cref="VoiWindows"/>
		public virtual string[] WindowCenterAndWidthExplanation
		{
			get { return DicomStringHelper.GetStringArray(_parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.WindowCenterWidthExplanation).ToString()); }
		}

		#endregion

		#region Frame of Reference Module

		/// <summary>
		/// Gets the frame of reference UID of the frame.
		/// </summary>
		public virtual string FrameOfReferenceUid
		{
			get { return _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.FrameOfReferenceUid).GetString(0, null) ?? string.Empty; }
		}

		#endregion

		#region Other Modules

		/// <summary>
		/// Gets the laterality of the anatomy depicted in this frame.
		/// </summary>
		public virtual string Laterality
		{
			get
			{
				string laterality;
				DicomAttribute dicomAttribute;
				if (_parentImageSop.TryGetFrameAttribute(_frameNumber, DicomTags.FrameLaterality, out dicomAttribute) && dicomAttribute.TryGetString(0, out laterality))
					return laterality;
				if (_parentImageSop.TryGetFrameAttribute(0, DicomTags.ImageLaterality, out dicomAttribute) && dicomAttribute.TryGetString(0, out laterality))
					return laterality;
				if (_parentImageSop.TryGetFrameAttribute(0, DicomTags.Laterality, out dicomAttribute) && dicomAttribute.TryGetString(0, out laterality))
					return laterality;
				return string.Empty;
			}
		}

		/// <summary>
		/// Gets the view position of the frame.
		/// </summary>
		public virtual string ViewPosition
		{
			get { return _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.ViewPosition).GetString(0, null) ?? string.Empty; }
		}

		/// <summary>
		/// Gets the spacing between the slices.
		/// </summary>
		public virtual double SpacingBetweenSlices
		{
			get { return _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.SpacingBetweenSlices).GetFloat64(0, 0); }
		}

		/// <summary>
		/// Gets the MR echo number of the frame.
		/// </summary>
		/// <remarks>
		/// For single-frame MR images, this is the value of the <see cref="DicomTags.EchoNumbers"/> attribute.
		/// For multi-frame MR images, this is the index value in the dimension identified by the <see cref="DicomTags.EffectiveEchoTime"/> tag.
		/// For all other images, the value of this property is not defined.
		/// </remarks>
		public virtual int EchoNumber
		{
			get
			{
				return _parentImageSop.GetFrameDimensionIndexValue(_frameNumber, DicomTags.EffectiveEchoTime, DicomTags.MrEchoSequence)
				       ?? _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.EchoNumbers).GetInt32(0, 0);
			}
		}

		/// <summary>
		/// Gets the stack number of the frame.
		/// </summary>
		/// <remarks>
		/// For multi-frame images, this is the index value in the dimension identified by the <see cref="DicomTags.StackId"/> tag.
		/// For all other images, the value of this property is not defined.
		/// </remarks>
		public virtual int StackNumber
		{
			get { return _parentImageSop.GetFrameDimensionIndexValue(_frameNumber, DicomTags.StackId, DicomTags.FrameContentSequence).GetValueOrDefault(0); }
		}

		/// <summary>
		/// Gets the stack ID of the frame.
		/// </summary>
		/// <remarks>
		/// For multi-frame images, this is the label of the stack as identified by the <see cref="DicomTags.StackId"/> tag.
		/// For all other images, the value of this property is not defined.
		/// </remarks>
		public virtual string StackId
		{
			get { return _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.StackId).GetString(0, null) ?? string.Empty; }
		}

		/// <summary>
		/// Gets the position of the frame within its stack.
		/// </summary>
		/// <remarks>
		/// For multi-frame images organized by stacks, this is the position of this frame within its stack as identified by the <see cref="DicomTags.InStackPositionNumber"/> tag.
		/// For all other images, the value of this property is not defined.
		/// </remarks>
		public virtual int InStackPositionNumber
		{
			get { return _parentImageSop.GetFrameAttribute(_frameNumber, DicomTags.InStackPositionNumber).GetInt32(0, 0); }
		}

		#endregion

		/// <summary>
		/// Gets a specific DICOM attribute for this frame.
		/// </summary>
		/// <remarks>
		/// <see cref="DicomAttribute"/>s returned by this method should be considered
		/// read-only and should not be modified in any way.
		/// </remarks>
		/// <param name="dicomTag">The DICOM tag to retrieve.</param>
		/// <returns>Returns the requested <see cref="DicomAttribute"/>.</returns>
		public DicomAttribute this[DicomTag dicomTag]
		{
			get { return _parentImageSop.GetFrameAttribute(_frameNumber, dicomTag); }
		}

		/// <summary>
		/// Gets a specific DICOM attribute for this frame.
		/// </summary>
		/// <remarks>
		/// <see cref="DicomAttribute"/>s returned by this method should be considered
		/// read-only and should not be modified in any way.
		/// </remarks>
		/// <param name="dicomTag">The DICOM tag to retrieve.</param>
		/// <returns>Returns the requested <see cref="DicomAttribute"/>.</returns>
		public DicomAttribute this[uint dicomTag]
		{
			get { return _parentImageSop.GetFrameAttribute(_frameNumber, dicomTag); }
		}

		/// <summary>
		/// Gets a value indicating whether the image is colour.
		/// </summary>
		/// <returns>
		/// <b>true</b> if <see cref="PhotometricInterpretation"/> is anything other than
		/// MONOCHROME1 or MONOCHROME2.
		/// </returns>
		public bool IsColor
		{
			get { return PhotometricInterpretation.IsColor; }
		}

		/// <summary>
		/// Gets the <see cref="ImagePlaneHelper"/> for this <see cref="ImageSop"/>.
		/// </summary>
		public ImagePlaneHelper ImagePlaneHelper
		{
			get
			{
				if (_imagePlaneHelper == null)
				{
					lock (_syncLock)
					{
						if (_imagePlaneHelper == null)
							_imagePlaneHelper = new ImagePlaneHelper(this);
					}
				}

				return _imagePlaneHelper;
			}
		}

		/// <summary>
		/// Invalidates the cached <see cref="ImagePlaneHelper"/>, forcing it to be recalculated the next time it is accessed.
		/// </summary>
		protected void InvalidateImagePlaneHelper()
		{
			lock (_syncLock)
			{
				_imagePlaneHelper = null;
			}
		}

		/// <summary>
		/// Gets the pixel spacing appropriate to the modality.
		/// </summary>
		/// <remarks>
		/// See the remarks for <see cref="StudyManagement.NormalizedPixelSpacing"/>.
		/// Clients who require the pixel spacing should use this property as opposed to 
		/// the raw DICOM pixel spacing property in <see cref="PixelSpacing"/>.
		/// </remarks>
		public NormalizedPixelSpacing NormalizedPixelSpacing
		{
			get
			{
				if (_normalizedPixelSpacing == null)
				{
					lock (_syncLock)
					{
						if (_normalizedPixelSpacing == null)
							_normalizedPixelSpacing = new NormalizedPixelSpacing(this);
					}
				}

				return _normalizedPixelSpacing;
			}
		}

		/// <summary>
		/// Gets pixel data in normalized form.
		/// </summary>
		/// <remarks>
		/// See <see cref="ISopFrameData.GetNormalizedPixelData"/> for a detailed explanation.
		/// </remarks>
		public virtual byte[] GetNormalizedPixelData()
		{
			return _parentImageSop.DataSource.GetFrameData(FrameNumber).GetNormalizedPixelData();
		}

		/// <summary>
		/// Unloads the pixel data.
		/// </summary>
		/// <remarks>
		/// <see cref="ISopFrameData.Unload"/> for a detailed explanation.
		/// </remarks>
		public virtual void UnloadPixelData()
		{
			_parentImageSop.DataSource.GetFrameData(FrameNumber).Unload();
		}

		/// <summary>
		/// Creates a <see cref="IFrameReference">transient reference</see> to this <see cref="Frame"/>.
		/// </summary>
		/// <remarks>
		/// See <see cref="ISopReference"/> for a detailed explanation of transient references.
		/// </remarks>
		public IFrameReference CreateTransientReference()
		{
			return new FrameReference(this);
		}

		/// <summary>
		/// Validates the <see cref="ImageSop"/> object.
		/// </summary>
		/// <remarks>
		/// Derived classes should call the base class implementation first, and then do further validation.
		/// The <see cref="ImageSop"/> class validates properties deemed vital to usage of the object.
		/// </remarks>
		/// <exception cref="SopValidationException">Thrown when validation fails.</exception>
		internal void Validate()
		{
			DicomValidator.ValidateRows(Rows);
			DicomValidator.ValidateColumns(Columns);
			DicomValidator.ValidateBitsAllocated(BitsAllocated);
			DicomValidator.ValidateBitsStored(BitsStored);
			DicomValidator.ValidateHighBit(HighBit);
			DicomValidator.ValidateSamplesPerPixel(SamplesPerPixel);
			DicomValidator.ValidatePixelRepresentation(PixelRepresentation);
			DicomValidator.ValidatePhotometricInterpretation(PhotometricInterpretation);

			DicomValidator.ValidateImagePropertyRelationships
				(
					BitsStored,
					BitsAllocated,
					HighBit,
					PhotometricInterpretation,
					PlanarConfiguration,
					SamplesPerPixel
				);
		}

		/// <summary>
		/// Inheritors should override this method to do additional cleanup.
		/// </summary>
		/// <remarks>
		/// Frames should never be disposed by client code; they are disposed by
		/// the parent <see cref="ImageSop"/>.
		/// </remarks>
		protected virtual void Dispose(bool disposing) {}

		#region IDicomAttributeProvider Members

		DicomAttribute IDicomAttributeProvider.this[DicomTag dicomTag]
		{
			get { return this[dicomTag]; }
			set { throw new InvalidOperationException("The DICOM attributes of the Frame class should be considered read-only."); }
		}

		DicomAttribute IDicomAttributeProvider.this[uint dicomTag]
		{
			get { return this[dicomTag]; }
			set { throw new InvalidOperationException("The DICOM attributes of the Frame class should be considered read-only."); }
		}

		bool IDicomAttributeProvider.TryGetAttribute(DicomTag dicomTag, out DicomAttribute dicomAttribute)
		{
			return _parentImageSop.TryGetFrameAttribute(_frameNumber, dicomTag, out dicomAttribute);
		}

		bool IDicomAttributeProvider.TryGetAttribute(uint dicomTag, out DicomAttribute dicomAttribute)
		{
			return _parentImageSop.TryGetFrameAttribute(_frameNumber, dicomTag, out dicomAttribute);
		}

		#endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion
	}
}
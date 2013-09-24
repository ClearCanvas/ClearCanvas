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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom.Validation;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Represents the DICOM concept of a frame.
	/// </summary>
	/// <remarks>
	/// Note that there should no longer be any need to derive directly from this class.
	/// See <see cref="ISopDataSource"/> and/or <see cref="Sop"/> for more information.
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
		/// Gets the frame number.
		/// </summary>
		public int FrameNumber
		{
			get { return _frameNumber; }
		}

		#region General Image Module

		/// <summary>
		/// Gets the patient orientation.
		/// </summary>
		/// <remarks>
		/// A <see cref="Dicom.Iod.PatientOrientation"/> is returned even when no data is available; 
		/// the <see cref="Dicom.Iod.PatientOrientation.IsEmpty"/> property will be true.
		/// </remarks>
		public virtual PatientOrientation PatientOrientation
		{
			get
			{
				var patientOrientation = _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.PatientOrientation).ToString();
				return PatientOrientation.FromString(patientOrientation, _parentImageSop.AnatomicalOrientationType) ?? PatientOrientation.Empty;
			}
		}

		/// <summary>
		/// Gets the image type as a DICOM multi-valued string.
		/// </summary>
		public virtual string ImageType
		{
			get { return _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.ImageType).ToString(); }
		}

		/// <summary>
		/// Gets the acquisition number.
		/// </summary>
		public virtual int AcquisitionNumber
		{
			get { return _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.AcquisitionNumber).GetInt32(0, 0); }
		}

		/// <summary>
		/// Gets the acquisiton date.
		/// </summary>
		public virtual string AcquisitionDate
		{
			get { return _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.AcquisitionDate).GetString(0, null) ?? string.Empty; }
		}

		/// <summary>
		/// Gets the acquisition time.
		/// </summary>
		public virtual string AcquisitionTime
		{
			get { return _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.AcquisitionTime).GetString(0, null) ?? string.Empty; }
		}

		/// <summary>
		/// Gets the acquisition date/time.
		/// </summary>
		public virtual string AcquisitionDateTime
		{
			get { return _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.AcquisitionDatetime).GetString(0, null) ?? string.Empty; }
		}

		/// <summary>
		/// Gets the number of images in the acquisition.
		/// </summary>
		public virtual int ImagesInAcquisition
		{
			get { return _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.ImagesInAcquisition).GetInt32(0, 0); }
		}

		/// <summary>
		/// Gets the image comments.
		/// </summary>
		public virtual string ImageComments
		{
			get { return _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.ImageComments).GetString(0, null) ?? string.Empty; }
		}

		/// <summary>
		/// Gets the lossy image compression.
		/// </summary>
		public virtual string LossyImageCompression
		{
			get { return _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.LossyImageCompression).GetString(0, null) ?? string.Empty; }
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
				var lossyImageCompressionRatios = _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.LossyImageCompressionRatio).ToString();

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
		/// in calculations that require the pixel spacing.
		/// </remarks>
		public virtual PixelSpacing PixelSpacing
		{
			get
			{
				var pixelSpacing = _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.PixelSpacing).ToString();
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
				var imageOrientationPatient = _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.ImageOrientationPatient).ToString();
				if (!string.IsNullOrEmpty(imageOrientationPatient))
				{
					double[] values;
					if (DicomStringHelper.TryGetDoubleArray(imageOrientationPatient, out values) && values.Length == 6)
						return new ImageOrientationPatient(values[0], values[1], values[2], values[3], values[4], values[5]);
				}

				return new ImageOrientationPatient(0, 0, 0, 0, 0, 0);
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
				var imagePositionPatient = _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.ImagePositionPatient).ToString();
				if (!string.IsNullOrEmpty(imagePositionPatient))
				{
					double[] values;
					if (DicomStringHelper.TryGetDoubleArray(imagePositionPatient, out values) && values.Length == 3)
						return new ImagePositionPatient(values[0], values[1], values[2]);
				}

				return new ImagePositionPatient(0, 0, 0);
			}
		}

		/// <summary>
		/// Gets the slice thickness.
		/// </summary>
		public virtual double SliceThickness
		{
			get { return _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.SliceThickness).GetFloat64(0, 0); }
		}

		/// <summary>
		/// Gets the slice location.
		/// </summary>
		public virtual double SliceLocation
		{
			get { return _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.SliceLocation).GetFloat64(0, 0); }
		}

		#endregion

		#region X-ray Acquisition Module

		/// <summary>
		/// Gets the imager pixel spacing.
		/// </summary>
		/// <remarks>
		/// It is generally recommended that clients use <see cref="NormalizedPixelSpacing"/> when
		/// in calculations that require the imager pixel spacing.
		/// </remarks>
		public virtual PixelSpacing ImagerPixelSpacing
		{
			get
			{
				var imagerPixelSpacing = _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.ImagerPixelSpacing).ToString();
				return PixelSpacing.FromString(imagerPixelSpacing) ?? PixelSpacing.Zero;
			}
		}

		#endregion

		#region Image Pixel Module

		/// <summary>
		/// Gets the samples per pixel.
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
		/// Gets the high bit.
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
				var pixelAspectRatio = _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.PixelAspectRatio).ToString();
				if (!string.IsNullOrEmpty(pixelAspectRatio))
				{
					int[] values;
					if (DicomStringHelper.TryGetIntArray(pixelAspectRatio, out values) && values.Length == 2)
						return new PixelAspectRatio(values[0], values[1]);
				}
				return new PixelAspectRatio(0, 0);
			}
		}

		#endregion

		#region Modality LUT Module

		/// <summary>
		/// Gets the rescale intercept.
		/// </summary>
		public virtual double RescaleIntercept
		{
			get { return _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.RescaleIntercept).GetFloat64(0, 0); }
		}

		/// <summary>
		/// Gets the rescale slope.
		/// </summary>
		/// <remarks>
		/// 1.0 is returned if no data is available.
		/// </remarks>
		public virtual double RescaleSlope
		{
			get
			{
				// ReSharper disable CompareOfFloatsByEqualityOperator
				var rescaleSlope = _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.RescaleSlope).GetFloat64(0, 1);
				return rescaleSlope == 0.0 ? 1.0 : rescaleSlope;
				// ReSharper restore CompareOfFloatsByEqualityOperator
			}
		}

		/// <summary>
		/// Gets the rescale type.
		/// </summary>
		public virtual string RescaleType
		{
			get { return _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.RescaleType).GetString(0, null) ?? string.Empty; }
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
		/// Gets the window width and center.
		/// </summary>
		/// <remarks>
		/// Will return as many parsable values as possible up to the first non-parsable value.
		/// For example, if there are 3 values, but the last one is poorly encoded, 2 values will be returned.
		/// </remarks>
		public virtual Window[] WindowCenterAndWidth
		{
			get
			{
				var windowCenterValues = _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.WindowCenter).ToString();
				if (!string.IsNullOrEmpty(windowCenterValues))
				{
					var windowWidthValues = _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.WindowWidth).ToString();
					if (!string.IsNullOrEmpty(windowWidthValues))
					{
						if (!String.IsNullOrEmpty(windowCenterValues) && !String.IsNullOrEmpty(windowWidthValues))
						{
							double[] windowCenters;
							double[] windowWidths;
							DicomStringHelper.TryGetDoubleArray(windowCenterValues, out windowCenters);
							DicomStringHelper.TryGetDoubleArray(windowWidthValues, out windowWidths);

							if (windowCenters.Length > 0 && windowCenters.Length == windowWidths.Length)
								return Enumerable.Range(0, windowWidths.Length).Select(i => new Window(windowWidths[i], windowCenters[i])).ToArray();
						}
					}
				}

				return new Window[0];
			}
		}

		/// <summary>
		/// Gets the window width and center explanation(s).
		/// </summary>
		public virtual string[] WindowCenterAndWidthExplanation
		{
			get
			{
				var windowCenterAndWidthExplanations = _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.WindowCenterWidthExplanation).ToString();
				return DicomStringHelper.GetStringArray(windowCenterAndWidthExplanations);
			}
		}

		#endregion

		#region Frame of Reference Module

		/// <summary>
		/// Gets the frame of reference uid for the image.
		/// </summary>
		public virtual string FrameOfReferenceUid
		{
			get { return _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.FrameOfReferenceUid).GetString(0, null) ?? string.Empty; }
		}

		#endregion

		#region Modality-Specific Modules

		/// <summary>
		/// Gets the spacing between the slices.
		/// </summary>
		public virtual double SpacingBetweenSlices
		{
			get { return _parentImageSop.GetDicomAttribute(_frameNumber, DicomTags.SpacingBetweenSlices).GetFloat64(0, 0); }
		}

		#endregion

		public DicomAttribute this[DicomTag dicomTag]
		{
			get { return _parentImageSop.GetDicomAttribute(_frameNumber, dicomTag); }
		}

		public DicomAttribute this[uint dicomTag]
		{
			get { return _parentImageSop.GetDicomAttribute(_frameNumber, dicomTag); }
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
			return _parentImageSop.TryGetDicomAttribute(_frameNumber, dicomTag, out dicomAttribute);
		}

		bool IDicomAttributeProvider.TryGetAttribute(uint dicomTag, out DicomAttribute dicomAttribute)
		{
			return _parentImageSop.TryGetDicomAttribute(_frameNumber, dicomTag, out dicomAttribute);
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
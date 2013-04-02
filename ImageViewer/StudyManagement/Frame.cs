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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom.Validation;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Represents the DICOM concept of a frame.
	/// </summary>
	/// <remarks>
	/// Note that there should no longer be any need to derive directly from this class.
	/// See <see cref="ISopDataSource"/> and/or <see cref="Sop"/> for more information.
	/// </remarks>
	public partial class Frame : IDisposable
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
			get { return ParentImageSop.StudyInstanceUid;  }
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
				string patientOrientation;
				patientOrientation = _parentImageSop[DicomTags.PatientOrientation].ToString();
				if (!string.IsNullOrEmpty(patientOrientation))
				{
					string[] values = DicomStringHelper.GetStringArray(patientOrientation);
					if (values.Length == 2)
						return new PatientOrientation(values[0], values[1], _parentImageSop.AnatomicalOrientationType);
				}

				return new PatientOrientation("", "");
			}
		}

		/// <summary>
		/// Gets the image type.  The entire Image Type value should be returned as a Dicom string array.
		/// </summary>
		public virtual string ImageType
		{
			get
			{
				string imageType;
				imageType = _parentImageSop[DicomTags.ImageType].ToString();
				return imageType ?? "";
			}
		}

		/// <summary>
		/// Gets the acquisition number.
		/// </summary>
		public virtual int AcquisitionNumber
		{
			get
			{
				int acquisitionNumber;
				acquisitionNumber = _parentImageSop[DicomTags.AcquisitionNumber].GetInt32(0, 0);
				return acquisitionNumber;
			}
		}

		/// <summary>
		/// Gets the acquisiton date.
		/// </summary>
		public virtual string AcquisitionDate
		{
			get
			{
				string acquisitionDate;
				acquisitionDate = _parentImageSop[DicomTags.AcquisitionDate].GetString(0, null);
				return acquisitionDate ?? "";
			}
		}

		/// <summary>
		/// Gets the acquisition time.
		/// </summary>
		public virtual string AcquisitionTime
		{
			get
			{
				string acquisitionTime;
				acquisitionTime = _parentImageSop[DicomTags.AcquisitionTime].GetString(0, null);
				return acquisitionTime ?? "";
			}
		}

		/// <summary>
		/// Gets the acquisition date/time.
		/// </summary>
		public virtual string AcquisitionDateTime
		{
			get
			{
				string acquisitionDateTime;
				acquisitionDateTime = _parentImageSop[DicomTags.AcquisitionDatetime].GetString(0, null);
				return acquisitionDateTime ?? "";
			}
		}

		/// <summary>
		/// Gets the number of images in the acquisition.
		/// </summary>
		public virtual int ImagesInAcquisition
		{
			get
			{
				int imagesInAcquisition;
				imagesInAcquisition = _parentImageSop[DicomTags.ImagesInAcquisition].GetInt32(0, 0);
				return imagesInAcquisition;
			}
		}

		/// <summary>
		/// Gets the image comments.
		/// </summary>
		public virtual string ImageComments
		{
			get
			{
				string imageComments;
				imageComments = _parentImageSop[DicomTags.ImageComments].GetString(0, null);
				return imageComments ?? "";
			}
		}

		/// <summary>
		/// Gets the lossy image compression.
		/// </summary>
		public virtual string LossyImageCompression
		{
			get
			{
				string lossyImageCompression;
				lossyImageCompression = _parentImageSop[DicomTags.LossyImageCompression].GetString(0, null);
				return lossyImageCompression ?? "";
			}
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
				string lossyImageCompressionRatios;
				lossyImageCompressionRatios = _parentImageSop[DicomTags.LossyImageCompressionRatio].ToString();

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
				string pixelSpacing;
				pixelSpacing = _parentImageSop[DicomTags.PixelSpacing].ToString();
				if (!string.IsNullOrEmpty(pixelSpacing))
				{
					double[] values;
					if (DicomStringHelper.TryGetDoubleArray(pixelSpacing, out values) && values.Length == 2)
						return new PixelSpacing(values[0], values[1]);
				}

				return new PixelSpacing(0, 0);
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
				string imageOrientationPatient;
				imageOrientationPatient = _parentImageSop[DicomTags.ImageOrientationPatient].ToString();
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
				string imagePositionPatient;
				imagePositionPatient = _parentImageSop[DicomTags.ImagePositionPatient].ToString();
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
			get
			{
				double sliceThickness;
				sliceThickness = _parentImageSop[DicomTags.SliceThickness].GetFloat64(0, 0);
				return sliceThickness;
			}
		}

		/// <summary>
		/// Gets the slice location.
		/// </summary>
		public virtual double SliceLocation
		{
			get
			{
				double sliceLocation;
				sliceLocation = _parentImageSop[DicomTags.SliceLocation].GetFloat64(0, 0);
				return sliceLocation;
			}
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
				string imagerPixelSpacing;
				imagerPixelSpacing = _parentImageSop[DicomTags.ImagerPixelSpacing].ToString();
				if (!string.IsNullOrEmpty(imagerPixelSpacing))
				{
					double[] values;
					if (DicomStringHelper.TryGetDoubleArray(imagerPixelSpacing, out values) && values.Length == 2)
						return new PixelSpacing(values[0], values[1]);
				}

				return new PixelSpacing(0, 0);
			}
		}

		#endregion

		#region Image Pixel Module

		#region Type 1

		/// <summary>
		/// Gets the samples per pixel.
		/// </summary>
		public virtual int SamplesPerPixel
		{
			get
			{
				ushort samplesPerPixel;
				samplesPerPixel = _parentImageSop[DicomTags.SamplesPerPixel].GetUInt16(0, 0);
				return samplesPerPixel;
			}
		}

		/// <summary>
		/// Gets the photometric interpretation.
		/// </summary>
		public virtual PhotometricInterpretation PhotometricInterpretation
		{
			get
			{
				string photometricInterpretation;
				photometricInterpretation = _parentImageSop[DicomTags.PhotometricInterpretation].GetString(0, null);
				return PhotometricInterpretation.FromCodeString(photometricInterpretation);
			}
		}

		/// <summary>
		/// Gets the number of rows.
		/// </summary>
		public virtual int Rows
		{
			get
			{
				ushort rows;
				rows = _parentImageSop[DicomTags.Rows].GetUInt16(0, 0);
				return rows;
			}
		}

		/// <summary>
		/// Gets the number of columns.
		/// </summary>
		public virtual int Columns
		{
			get
			{
				ushort columns;
				columns = _parentImageSop[DicomTags.Columns].GetUInt16(0, 0);
				return columns;
			}
		}

		/// <summary>
		/// Gets the number of bits allocated.
		/// </summary>
		public virtual int BitsAllocated
		{
			get
			{
				ushort bitsAllocated;
				bitsAllocated = _parentImageSop[DicomTags.BitsAllocated].GetUInt16(0, 0);
				return bitsAllocated;
			}
		}

		/// <summary>
		/// Gets the number of bits stored.
		/// </summary>
		public virtual int BitsStored
		{
			get
			{
				ushort bitsStored;
				bitsStored = _parentImageSop[DicomTags.BitsStored].GetUInt16(0, 0);
				return bitsStored;
			}
		}

		/// <summary>
		/// Gets the high bit.
		/// </summary>
		public virtual int HighBit
		{
			get
			{
				ushort highBit;
				highBit = _parentImageSop[DicomTags.HighBit].GetUInt16(0, 0);
				return highBit;
			}
		}

		/// <summary>
		/// Gets the pixel representation.
		/// </summary>
		public virtual int PixelRepresentation
		{
			get
			{
				ushort pixelRepresentation;
				pixelRepresentation = _parentImageSop[DicomTags.PixelRepresentation].GetUInt16(0, 0);
				return pixelRepresentation;
			}
		}

		#endregion
		#region Type 1C

		/// <summary>
		/// Gets the planar configuration.
		/// </summary>
		public virtual int PlanarConfiguration
		{
			get
			{
				ushort planarConfiguration;
				planarConfiguration = _parentImageSop[DicomTags.PlanarConfiguration].GetUInt16(0, 0);
				return planarConfiguration;
			}
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
				string pixelAspectRatio;
				pixelAspectRatio = _parentImageSop[DicomTags.PixelAspectRatio].ToString();
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
		#endregion

		#region Modality LUT Module

		/// <summary>
		/// Gets the rescale intercept.
		/// </summary>
		public virtual double RescaleIntercept
		{
			get
			{
				double rescaleIntercept;
				rescaleIntercept = _parentImageSop[DicomTags.RescaleIntercept].GetFloat64(0, 0);
				return rescaleIntercept;
			}
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
				double rescaleSlope;
				rescaleSlope = _parentImageSop[DicomTags.RescaleSlope].GetFloat64(0, 0);
				if (rescaleSlope == 0.0)
					return 1.0;

				return rescaleSlope;
			}
		}

		/// <summary>
		/// Gets the rescale type.
		/// </summary>
		public virtual string RescaleType
		{
			get
			{
				string rescaleType;
				rescaleType = _parentImageSop[DicomTags.RescaleType].GetString(0, null);
				return rescaleType ?? "";
			}
		}

		/// <summary>
		/// Gets the units of the rescale function output.
		/// </summary>
		/// <seealso cref="RescaleSlope"/>
		/// <seealso cref="RescaleIntercept"/>
		/// <seealso cref="RescaleType"/>
		public RescaleUnits RescaleUnits
		{
			get { return RescaleUnits.GetRescaleUnits(_parentImageSop.DataSource); }
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
				string windowCenterValues;
				windowCenterValues = _parentImageSop[DicomTags.WindowCenter].ToString();
				if (!string.IsNullOrEmpty(windowCenterValues))
				{
					string windowWidthValues;
					windowWidthValues = _parentImageSop[DicomTags.WindowWidth].ToString();
					if (!string.IsNullOrEmpty(windowWidthValues))
					{
						if (!String.IsNullOrEmpty(windowCenterValues) && !String.IsNullOrEmpty(windowWidthValues))
						{
							List<Window> windows = new List<Window>();

							double[] windowCenters;
							double[] windowWidths;
							DicomStringHelper.TryGetDoubleArray(windowCenterValues, out windowCenters);
							DicomStringHelper.TryGetDoubleArray(windowWidthValues, out windowWidths);

							if (windowCenters.Length > 0 && windowCenters.Length == windowWidths.Length)
							{
								for (int i = 0; i < windowWidths.Length; ++i)
									windows.Add(new Window(windowWidths[i], windowCenters[i]));

								return windows.ToArray();
							}
						}
					}
				}

				return new Window[] { };
			}
		}

		/// <summary>
		/// Gets the window width and center explanation(s).
		/// </summary>
		public virtual string[] WindowCenterAndWidthExplanation
		{
			get
			{
				string windowCenterAndWidthExplanations;
				windowCenterAndWidthExplanations = _parentImageSop[DicomTags.WindowCenterWidthExplanation].ToString();
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
			get
			{
				string frameOfReferenceUid;
				frameOfReferenceUid = _parentImageSop[DicomTags.FrameOfReferenceUid].GetString(0, null);
				return frameOfReferenceUid ?? "";
			}
		}

		#endregion

		#region MR Image Module

		/// <summary>
		/// Gets the spacing between the slices.
		/// </summary>
		public virtual double SpacingBetweenSlices
		{
			get
			{
				double spacingBetweenSlices;
				spacingBetweenSlices = _parentImageSop[DicomTags.SpacingBetweenSlices].GetFloat64(0, 0);
				return spacingBetweenSlices;
			}
		}

		#endregion

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
			DicomValidator.ValidateRows(this.Rows);
			DicomValidator.ValidateColumns(this.Columns);
			DicomValidator.ValidateBitsAllocated(this.BitsAllocated);
			DicomValidator.ValidateBitsStored(this.BitsStored);
			DicomValidator.ValidateHighBit(this.HighBit);
			DicomValidator.ValidateSamplesPerPixel(this.SamplesPerPixel);
			DicomValidator.ValidatePixelRepresentation(this.PixelRepresentation);
			DicomValidator.ValidatePhotometricInterpretation(this.PhotometricInterpretation);

			DicomValidator.ValidateImagePropertyRelationships
				(
					this.BitsStored, 
					this.BitsAllocated, 
					this.HighBit, 
					this.PhotometricInterpretation, 
					this.PlanarConfiguration, 
					this.SamplesPerPixel
				);
		}

		/// <summary>
		/// Inheritors should override this method to do additional cleanup.
		/// </summary>
		/// <remarks>
		/// Frames should never be disposed by client code; they are disposed by
		/// the parent <see cref="ImageSop"/>.
		/// </remarks>
		protected virtual void Dispose(bool disposing)
		{
		}

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion
	}
}

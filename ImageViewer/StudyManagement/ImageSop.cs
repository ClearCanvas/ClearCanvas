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
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM Image SOP Instance.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Note that there should no longer be any need to derive from this class; the <see cref="Sop"/>, <see cref="ImageSop"/>
	/// and <see cref="Frame"/> classes are now just simple Bridge classes (see Bridge Design Pattern)
	/// to <see cref="ISopDataSource"/> and <see cref="ISopFrameData"/>.  See the
	/// remarks for <see cref="ISopDataSource"/> for more information.
	/// </para>
	/// <para>Also, for more information on 'transient references' and the lifetime of <see cref="Sop"/>s,
	/// see <see cref="ISopReference"/>.
	/// </para>
	/// </remarks>
	public partial class ImageSop : Sop
	{
		private readonly object _syncLock = new object();
		private volatile FrameCollection _frames;

		/// <summary>
		/// Constructs a new instance of <see cref="ImageSop"/> from a local file.
		/// </summary>
		/// <param name="filename">The path to a local DICOM Part 10 file.</param>
		public ImageSop(string filename)
			: base(filename)
		{
			_functionalGroups = GetFunctionalGroupMap(DataSource);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ImageSop"/>.
		/// </summary>
		public ImageSop(ISopDataSource dataSource)
			: base(dataSource)
		{
			_functionalGroups = GetFunctionalGroupMap(DataSource);
		}

		/// <summary>
		/// A collection of <see cref="Frame"/> objects.
		/// </summary>
		/// <remarks>
		/// DICOM distinguishes between regular image SOPs and multiframe image SOPs.
		/// ClearCanvas, however, does not make this distinction, as it requires that 
		/// two sets of client code be written.  Instead, all image SOPs are considered
		/// to be multiframe, with regular images being a special case of a multiframe
		/// image with one frame. It can be assumed that all images contain at least
		/// one frame.
		/// </remarks>
		/// <seealso cref="NumberOfFrames"/>
		public FrameCollection Frames
		{
			get
			{
				if (_frames == null)
				{
					lock (_syncLock)
					{
						if (_frames == null)
						{
							var frames = new FrameCollection();
							for (int i = 1; i <= NumberOfFrames; i++)
								frames.Add(CreateFrame(i));

							_frames = frames;
						}
					}
				}

				return _frames;
			}
		}

		#region General Series Module

		/// <summary>
		/// Gets the Anatomical Orientation Type.
		/// </summary>
		public virtual string AnatomicalOrientationType
		{
			get { return this[DicomTags.AnatomicalOrientationType].ToString(); }
		}

		#endregion

		#region DX Series Module

		/// <summary>
		/// Gets the Presentation Intent Type.
		/// </summary>
		public virtual string PresentationIntentType
		{
			get { return this[DicomTags.PresentationIntentType].ToString(); }
		}

		#endregion

		#region General Image Module

		/// <summary>
		/// Gets the patient orientation.
		/// </summary>
		/// <remarks>
		/// A <see cref="Dicom.Iod.PatientOrientation"/> is returned even when no data is available; 
		/// the <see cref="Dicom.Iod.PatientOrientation.IsEmpty"/> property will be true.
		/// </remarks>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual PatientOrientation PatientOrientation
		{
			get { return Frames[1].PatientOrientation; }
		}

		/// <summary>
		/// Gets the image type.  The entire Image Type value should be returned as a Dicom string array.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual string ImageType
		{
			get { return Frames[1].ImageType; }
		}

		/// <summary>
		/// Gets the acquisition number.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int AcquisitionNumber
		{
			get { return Frames[1].AcquisitionNumber; }
		}

		/// <summary>
		/// Gets the acquisiton date.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual string AcquisitionDate
		{
			get { return Frames[1].AcquisitionDate; }
		}

		/// <summary>
		/// Gets the acquisition time.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual string AcquisitionTime
		{
			get { return Frames[1].AcquisitionTime; }
		}

		/// <summary>
		/// Gets the acquisition date/time.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual string AcquisitionDateTime
		{
			get { return Frames[1].AcquisitionDateTime; }
		}

		/// <summary>
		/// Gets the number of images in the acquisition.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int ImagesInAcquisition
		{
			get { return Frames[1].ImagesInAcquisition; }
		}

		/// <summary>
		/// Gets the image comments.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual string ImageComments
		{
			get { return Frames[1].ImageComments; }
		}

		/// <summary>
		/// Gets the lossy image compression.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual string LossyImageCompression
		{
			get { return Frames[1].LossyImageCompression; }
		}

		/// <summary>
		/// Gets the lossy image compression ratio.
		/// </summary>
		/// <remarks>
		/// Will return as many parsable values as possible up to the first non-parsable value.  For example, if there are 3 values, but the last one is poorly encoded, 2 values will be returned.
		/// </remarks>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual double[] LossyImageCompressionRatio
		{
			get { return Frames[1].LossyImageCompressionRatio; }
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
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual PixelSpacing PixelSpacing
		{
			get { return Frames[1].PixelSpacing; }
		}

		/// <summary>
		/// Gets the image orientation patient.
		/// </summary>
		/// <remarks>
		/// Returns an <see cref="Dicom.Iod.ImageOrientationPatient"/> object with zero for all its values
		/// when no data is available or the existing data is bad/incorrect;  <see cref="Dicom.Iod.ImageOrientationPatient.IsNull"/>
		/// will be true.
		/// </remarks>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual ImageOrientationPatient ImageOrientationPatient
		{
			get { return Frames[1].ImageOrientationPatient; }
		}

		/// <summary>
		/// Gets the image position patient.
		/// </summary>
		/// <remarks>
		/// Returns an <see cref="Dicom.Iod.ImagePositionPatient"/> object with zero for all its values when no data is
		/// available or the existing data is bad/incorrect; <see cref="Dicom.Iod.ImagePositionPatient.IsNull"/> will be true.
		/// </remarks>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual ImagePositionPatient ImagePositionPatient
		{
			get { return Frames[1].ImagePositionPatient; }
		}

		/// <summary>
		/// Gets the slice thickness.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual double SliceThickness
		{
			get { return Frames[1].SliceThickness; }
		}

		/// <summary>
		/// Gets the slice location.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual double SliceLocation
		{
			get { return Frames[1].SliceLocation; }
		}

		#endregion

		#region Image Pixel Module

		#region Type 1

		/// <summary>
		/// Gets the samples per pixel.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int SamplesPerPixel
		{
			get { return Frames[1].SamplesPerPixel; }
		}

		/// <summary>
		/// Gets the photometric interpretation.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual PhotometricInterpretation PhotometricInterpretation
		{
			get { return Frames[1].PhotometricInterpretation; }
		}

		/// <summary>
		/// Gets the number of rows.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int Rows
		{
			get { return Frames[1].Rows; }
		}

		/// <summary>
		/// Gets the number of columns.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int Columns
		{
			get { return Frames[1].Columns; }
		}

		/// <summary>
		/// Gets the number of bits allocated.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int BitsAllocated
		{
			get { return Frames[1].BitsAllocated; }
		}

		/// <summary>
		/// Gets the number of bits stored.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int BitsStored
		{
			get { return Frames[1].BitsStored; }
		}

		/// <summary>
		/// Gets the high bit.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int HighBit
		{
			get { return Frames[1].HighBit; }
		}

		/// <summary>
		/// Gets the pixel representation.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int PixelRepresentation
		{
			get { return Frames[1].PixelRepresentation; }
		}

		#endregion

		#region Type 1C

		/// <summary>
		/// Gets the planar configuration.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int PlanarConfiguration
		{
			get { return Frames[1].PlanarConfiguration; }
		}

		/// <summary>
		/// Gets the pixel aspect ratio.
		/// </summary>
		/// <remarks>
		/// If no value exists in the image header, or the value is invalid, a <see cref="ClearCanvas.Dicom.Iod.PixelAspectRatio"/>
		/// is returned whose <see cref="ClearCanvas.Dicom.Iod.PixelAspectRatio.IsNull"/> property evaluates to true.
		/// </remarks>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual PixelAspectRatio PixelAspectRatio
		{
			get { return Frames[1].PixelAspectRatio; }
		}

		#endregion

		#endregion

		#region Modality LUT Module

		/// <summary>
		/// Gets the rescale intercept.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual double RescaleIntercept
		{
			get { return Frames[1].RescaleIntercept; }
		}

		/// <summary>
		/// Gets the rescale slope.
		/// </summary>
		/// <remarks>
		/// 1.0 is returned if no data is available.
		/// </remarks>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual double RescaleSlope
		{
			get { return Frames[1].RescaleSlope; }
		}

		/// <summary>
		/// Gets the rescale type.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual string RescaleType
		{
			get { return Frames[1].RescaleType; }
		}

		#endregion

		#region VOI LUT Module

		/// <summary>
		/// Gets the <see cref="VoiDataLut"/>s from the image header.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual IList<VoiDataLut> VoiDataLuts
		{
			get { return Frames[1].VoiDataLuts; }
		}

		/// <summary>
		/// Gets the window width and center.
		/// </summary>
		/// <remarks>
		/// Will return as many parsable values as possible up to the first non-parsable value.  For example, if there are 3 values, but the last one is poorly encoded, 2 values will be returned.
		/// </remarks>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual Window[] WindowCenterAndWidth
		{
			get { return Frames[1].WindowCenterAndWidth; }
		}

		/// <summary>
		/// Gets the window width and center explanation.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual string[] WindowCenterAndWidthExplanation
		{
			get { return Frames[1].WindowCenterAndWidthExplanation; }
		}

		#endregion

		#region Multi-Frame Module

		/// <summary>
		/// Gets the number of frames in the image SOP.
		/// </summary>
		/// <remarks>
		/// Regular, non-multiframe DICOM images do not have this tag. However, because 
		/// such images are treated as multiframes with a single frame, 
		/// <see cref="NumberOfFrames"/> returns 1 in that case.
		/// </remarks>
		public virtual int NumberOfFrames
		{
			get { return Math.Max(this[DicomTags.NumberOfFrames].GetInt32(0, 1), 1); }
		}

		#endregion

		#region Mammography Image Module / DX Anatomy Imaged Module / Intra-Oral Image Module / Ocular Region Imaged Module

		/// <summary>
		/// Gets the Image Laterality.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual string ImageLaterality
		{
			get { return this[DicomTags.ImageLaterality].GetString(0, null) ?? string.Empty; }
		}

		#endregion

		#region CR Series Module / DX Positioning Module

		/// <summary>
		/// Gets the View Position.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual string ViewPosition
		{
			get { return this[DicomTags.ViewPosition].GetString(0, null) ?? string.Empty; }
		}

		#endregion

		/// <summary>
		/// Gets pixel data in normalized form.
		/// </summary>
		/// <remarks>
		/// See the comments for <see cref="Frame"/> for an explanation of 'normalized' pixel data.
		/// </remarks>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual byte[] GetNormalizedPixelData()
		{
			return Frames[1].GetNormalizedPixelData();
		}

		protected override IEnumerable<TransferSyntax> GetAllowableTransferSyntaxes()
		{
			var list = new List<TransferSyntax>(base.GetAllowableTransferSyntaxes());
			list.AddRange(DicomCodecRegistry.GetCodecTransferSyntaxes());
			return list;
		}

		/// <summary>
		/// Factory method to create the frame with the specified frame number.
		/// </summary>
		/// <param name="frameNumber">The numeric identifier of the <see cref="Frame"/> to create; frame numbers are <b>one-based</b>.</param>
		protected virtual Frame CreateFrame(int frameNumber)
		{
			return new Frame(this, frameNumber);
		}

		/// <summary>
		/// Validates the <see cref="ImageSop"/> object.
		/// </summary>
		/// <remarks>
		/// Derived classes should call the base class implementation first, and then do further validation.
		/// The <see cref="ImageSop"/> class validates properties deemed vital to usage of the object.
		/// </remarks>
		/// <exception cref="SopValidationException">Thrown when validation fails.</exception>
		protected override void ValidateInternal()
		{
			base.ValidateInternal();

			ValidateAllowableTransferSyntax();

			foreach (Frame frame in Frames)
				frame.Validate();
		}

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		/// <param name="disposing">True if disposing, false if finalizing.</param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				lock (_syncLock)
				{
					if (_frames != null)
					{
						foreach (Frame frame in _frames)
							(frame as IDisposable).Dispose();

						_frames = null;
					}
				}
			}
		}

		/// <summary>
		/// Gets whether or not this Image SOP instance is a multi-frame image.
		/// </summary>
		public bool IsMultiframe
		{
			// we include a check for the functional groups since an "enhanced" image could have only one frame, but have important data encoded in functional groups anyway
			get { return NumberOfFrames > 1 || new MultiFrameFunctionalGroupsModuleIod(DataSource).HasValues(); }
		}

		/// <summary>
		/// Checks whether or not the specified SOP Class indicates a supported image type.
		/// </summary>
		/// <param name="sopClass">The SOP Class UID to be checked.</param>
		/// <returns>True if the SOP Class is a supported image type; False otherwise.</returns>
		public static bool IsSupportedSopClass(string sopClass)
		{
			return IsImageSop(sopClass);
		}

		/// <summary>
		/// Checks whether or not the specified SOP Class indicates a supported image type.
		/// </summary>
		/// <param name="sopClass">The SOP Class to be checked.</param>
		/// <returns>True if the SOP Class is a supported image type; False otherwise.</returns>
		public static bool IsSupportedSopClass(SopClass sopClass)
		{
			return IsImageSop(sopClass);
		}
	}
}
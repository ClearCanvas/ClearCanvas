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
using System.Globalization;
using System.IO;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom
{
	/// <summary>
	/// Base class representing pixel data.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class is used to represent pixel data within the DICOM library.  It contains
	/// all the basic tags required to work with pixel data.
	/// </para>
	/// </remarks>
	public abstract class DicomPixelData
	{
		#region Private Members

		private int _frames = 1;

		#region Image Pixel Macro

		private ushort _samplesPerPixel = 1;
		private ushort _pixelRepresentation;
		private ushort _planarConfiguration;
		private string _photometricInterpretation;

		#endregion

		private TransferSyntax _transferSyntax = TransferSyntax.ExplicitVrLittleEndian;
		private SopClass _sopClass;

		#region General Image Module

		private string _lossyImageCompression = "";
		private string _lossyImageCompressionMethod = "";
		private string _derivationDescription = "";

		#endregion

		#region Modality Lut

		private Decimal _rescaleSlope = 1;
		private Decimal _rescapeIntercept = 0;

		private string _rescaleSlopeString = "1";
		private string _rescaleInterceptString = "0";

		private readonly bool _hasDataModalityLut = false;

		#endregion

		#region VOI LUT

		private List<Window> _linearVoiLuts = new List<Window>();
		private readonly bool _hasDataVoiLuts = false;

		#endregion

		#region Palette Color LUT

		private bool _hasPaletteColorLut = false;
		private readonly PaletteColorLut _paletteColorLut;

		#endregion

		#endregion

		#region Public Static Methods

		/// <summary>
		/// Creates an instance of <see cref="DicomPixelData"/> from specified image path
		/// </summary>
		/// <param name="path"></param>
		/// <returns>
		/// </returns>
		public static DicomPixelData CreateFrom(string path)
		{
			DicomFile file = new DicomFile(path);
			file.Load();
			return CreateFrom(file);
		}

		/// <summary>
		/// Creates an instance of <see cref="DicomPixelData"/> from specified stream
		/// </summary>
		/// <param name="stream"></param>
		/// <returns>
		/// </returns>
		public static DicomPixelData CreateFrom(Stream stream)
		{
			DicomFile file = new DicomFile();
			file.Load(stream);
			return CreateFrom(file);
		}

		/// <summary>
		/// Creates an instance of <see cref="DicomPixelData"/> from specified dicom message
		/// </summary>
		/// <param name="message"></param>
		/// <returns>
		/// </returns>
		public static DicomPixelData CreateFrom(DicomMessageBase message)
		{
			if (message.TransferSyntax.LosslessCompressed || message.TransferSyntax.LossyCompressed)
				return new DicomCompressedPixelData(message);
			return new DicomUncompressedPixelData(message);
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message"></param>
		protected DicomPixelData(DicomMessageBase message)
			: this(message.DataSet)
		{
			_transferSyntax = message.TransferSyntax;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="collection"></param>
		protected DicomPixelData(DicomAttributeCollection collection)
		{
			collection.LoadDicomFields(this);

			SopClass = SopClass.GetSopClass(collection[DicomTags.SopClassUid].GetString(0, string.Empty));

			if (collection.Contains(DicomTags.NumberOfFrames))
				NumberOfFrames = collection[DicomTags.NumberOfFrames].GetInt32(0, 1);
			if (collection.Contains(DicomTags.PlanarConfiguration))
				PlanarConfiguration = collection[DicomTags.PlanarConfiguration].GetUInt16(0, 1);
			if (collection.Contains(DicomTags.LossyImageCompression))
				LossyImageCompression = collection[DicomTags.LossyImageCompression].GetString(0, string.Empty);
			if (collection.Contains(DicomTags.LossyImageCompressionRatio))
				LossyImageCompressionRatio = collection[DicomTags.LossyImageCompressionRatio].GetFloat32(0, 1.0f);
			if (collection.Contains(DicomTags.LossyImageCompressionMethod))
				LossyImageCompressionMethod = collection[DicomTags.LossyImageCompressionMethod].GetString(0, string.Empty);
			if (collection.Contains(DicomTags.DerivationDescription))
				DerivationDescription = collection[DicomTags.DerivationDescription].GetString(0, string.Empty);
			if (collection.Contains(DicomTags.RescaleSlope))
				RescaleSlope = collection[DicomTags.RescaleSlope].ToString();
			if (collection.Contains(DicomTags.RescaleIntercept))
				RescaleIntercept = collection[DicomTags.RescaleIntercept].ToString();
			if (collection.Contains(DicomTags.ModalityLutSequence))
			{
				DicomAttribute attrib = collection[DicomTags.ModalityLutSequence];
				_hasDataModalityLut = !attrib.IsEmpty && !attrib.IsNull;
			}

			_linearVoiLuts = Window.GetWindowCenterAndWidth(collection);
			if (collection.Contains(DicomTags.VoiLutSequence))
			{
				DicomAttribute attrib = collection[DicomTags.VoiLutSequence];
				_hasDataVoiLuts = !attrib.IsEmpty && !attrib.IsNull;
			}

			if (PhotometricInterpretation.Equals(Iod.PhotometricInterpretation.PaletteColor.Code) && collection.Contains(DicomTags.RedPaletteColorLookupTableDescriptor))
			{
				_paletteColorLut = PaletteColorLut.Create(collection);
				_hasPaletteColorLut = true;
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="attrib"></param>
		internal DicomPixelData(DicomPixelData attrib)
		{
			SopClass = attrib.SopClass;

			NumberOfFrames = attrib.NumberOfFrames;
			ImageWidth = attrib.ImageWidth;
			ImageHeight = attrib.ImageHeight;
			HighBit = attrib.HighBit;
			BitsStored = attrib.BitsStored;
			BitsAllocated = attrib.BitsAllocated;
			SamplesPerPixel = attrib.SamplesPerPixel;
			PixelRepresentation = attrib.PixelRepresentation;
			PlanarConfiguration = attrib.PlanarConfiguration;
			PhotometricInterpretation = attrib.PhotometricInterpretation;
			LossyImageCompression = attrib.LossyImageCompression;
			DerivationDescription = attrib.DerivationDescription;
			LossyImageCompressionRatio = attrib.LossyImageCompressionRatio;
			LossyImageCompressionMethod = attrib.LossyImageCompressionMethod;
			RescaleSlope = attrib.RescaleSlope;
			RescaleIntercept = attrib.RescaleIntercept;

			_hasDataModalityLut = attrib.HasDataModalityLut;
			_hasDataVoiLuts = attrib.HasDataVoiLuts;
			_hasPaletteColorLut = attrib.HasPaletteColorLut;
			_paletteColorLut = attrib.PaletteColorLut;

			foreach (Window window in attrib.LinearVoiLuts)
				_linearVoiLuts.Add(window);
		}

		#endregion

		#region GetFrame

		/// <summary>
		/// Get a specific frame's data in uncompressed format.
		/// </summary>
		/// <param name="frame">The zero offset frame to get.</param>
		/// <returns>A byte array containing the pixel data.</returns>
		public byte[] GetFrame(int frame)
		{
			string photometricInterpretation;

			return GetFrame(frame, out photometricInterpretation);
		}

		/// <summary>
		/// Get a specific uncompressed frame with the photometric interpretation.
		/// </summary>
		/// <param name="frame">A zero offset frame number.</param>
		/// <param name="photometricInterpretation">The photometric interpretation of the pixel data.</param>
		/// <returns>A byte array containing the uncompressed pixel data.</returns>
		public abstract byte[] GetFrame(int frame, out string photometricInterpretation);

		#endregion

		/// <summary>
		/// Update the tags in an attribute collection.
		/// </summary>
		/// <param name="dataset">The attribute collection to update.</param>
		public abstract void UpdateAttributeCollection(DicomAttributeCollection dataset);

		/// <summary>
		/// Updat ethe pixel data related tags in a DICOM message.
		/// </summary>
		/// <param name="message"></param>
		public abstract void UpdateMessage(DicomMessageBase message);

		#region Public Properties

		/// <summary>
		/// The number of frames in the pixel data.
		/// </summary>
		public int NumberOfFrames
		{
			get { return _frames; }
			set { _frames = value; }
		}

		#region Image Pixel Macro

		[DicomField(DicomTags.Columns, DefaultValue = DicomFieldDefault.Default)]
		public ushort ImageWidth { get; set; }

		[DicomField(DicomTags.Rows, DefaultValue = DicomFieldDefault.Default)]
		public ushort ImageHeight { get; set; }

		[DicomField(DicomTags.HighBit, DefaultValue = DicomFieldDefault.Default)]
		public ushort HighBit { get; set; }

		public ushort LowBit
		{
			get { return GetLowBit(BitsStored, HighBit); }
		}

		[DicomField(DicomTags.BitsStored, DefaultValue = DicomFieldDefault.Default)]
		public ushort BitsStored { get; set; }

		[DicomField(DicomTags.BitsAllocated, DefaultValue = DicomFieldDefault.Default)]
		public ushort BitsAllocated { get; set; }

		public int BytesAllocated
		{
			get
			{
				int bytes = BitsAllocated/8;
				if ((BitsAllocated%8) > 0)
					bytes++;
				return bytes;
			}
		}

		[DicomField(DicomTags.SamplesPerPixel, DefaultValue = DicomFieldDefault.Default)]
		public ushort SamplesPerPixel
		{
			get { return _samplesPerPixel; }
			set { _samplesPerPixel = value; }
		}

		[DicomField(DicomTags.PixelRepresentation, DefaultValue = DicomFieldDefault.Default)]
		public ushort PixelRepresentation
		{
			get { return _pixelRepresentation; }
			set { _pixelRepresentation = value; }
		}

		public bool IsSigned
		{
			get { return _pixelRepresentation != 0; }
		}

		// Not always in images, so don't make it an attribute and manually update it if needed
		public ushort PlanarConfiguration
		{
			get { return _planarConfiguration; }
			set { _planarConfiguration = value; }
		}

		/// <summary>
		/// Flag telling if the pixel data is encoded in planes.
		/// </summary>
		public bool IsPlanar
		{
			get { return _planarConfiguration != 0; }
		}

		/// <summary>
		/// Photometric Interpretation (0028,0004)
		/// </summary>
		[DicomField(DicomTags.PhotometricInterpretation, DefaultValue = DicomFieldDefault.Null)]
		public string PhotometricInterpretation
		{
			get { return _photometricInterpretation; }
			set { _photometricInterpretation = value; }
		}

		#endregion

		#region Modality Lut

		/// <summary>
		/// The rescale slope as a decimal string.
		/// </summary>
		public string RescaleSlope
		{
			get { return _rescaleSlopeString; }
			set
			{
				decimal rescaleSlope;
				if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out rescaleSlope))
				{
					_rescaleSlopeString = value;
					_rescaleSlope = rescaleSlope;
				}
			}
		}

		/// <summary>
		/// The rescale intercept as a decimal string.
		/// </summary>
		public string RescaleIntercept
		{
			get { return _rescaleInterceptString; }
			set
			{
				decimal rescapeIntercept;
				if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out rescapeIntercept))
				{
					_rescaleInterceptString = value;
					_rescapeIntercept = rescapeIntercept;
				}
			}
		}

		/// <summary>
		/// Decimal representation of the <see cref="RescaleSlope"/>.
		/// </summary>
		public decimal DecimalRescaleSlope
		{
			get { return _rescaleSlope; }
			set
			{
				_rescaleSlope = value;
				//G12 everywhere else.
				_rescaleSlopeString = _rescaleSlope.ToString("G10");
			}
		}

		/// <summary>
		/// Decimal representation of the <see cref="RescaleIntercept"/>.
		/// </summary>
		public decimal DecimalRescaleIntercept
		{
			get { return _rescapeIntercept; }
			set
			{
				_rescapeIntercept = value;
				//G12 everywhere else.
				_rescaleInterceptString = _rescapeIntercept.ToString("G10");
			}
		}

		/// <summary>
		/// Flag telling if the pixel data has a data modality LUT.
		/// </summary>
		public bool HasDataModalityLut
		{
			get { return _hasDataModalityLut; }
		}

		#endregion

		#region Voi Lut

		public List<Window> LinearVoiLuts
		{
			get { return _linearVoiLuts; }
			set { _linearVoiLuts = value ?? new List<Window>(); }
		}

		public bool HasLinearVoiLuts
		{
			get { return _linearVoiLuts.Count > 0; }
		}

		public bool HasDataVoiLuts
		{
			get { return _hasDataVoiLuts; }
		}

		#endregion

		#region Palette Color LUT

		/// <summary>
		/// Palette Color LUT module
		/// </summary>
		public PaletteColorLut PaletteColorLut
		{
			get
			{
				if (_hasPaletteColorLut) return _paletteColorLut;
				return null;
			}
		}

		/// <summary>
		/// Does the Pixel data have a Palette Color LUT
		/// </summary>
		public bool HasPaletteColorLut
		{
			get { return _hasPaletteColorLut; }
			set { _hasPaletteColorLut = value; }
		}

		#endregion

		/// <summary>
		/// The frame size of an uncompressed frame.
		/// </summary>
		public int UncompressedFrameSize
		{
			get
			{
				// ybr full 422 only stores 2/3 of the pixels
				if (_photometricInterpretation != null && _photometricInterpretation.Equals("YBR_FULL_422"))
					return ImageWidth*ImageHeight*BytesAllocated*2;

				return ImageWidth*ImageHeight*BytesAllocated*SamplesPerPixel;
			}
		}

		/// <summary>
		/// The frame size of an uncompressed frame calculated by using the Bits Stored.
		/// </summary>
		/// <remarks>
		/// When performing a compression ratio for a compressed image, the actual source 
		/// image size should be calculated based on the Bits Stored, and not based on 
		/// Bits Allocated.  This property is used to calculate the size based on 
		/// Bits Stored.
		/// </remarks>
		public int BitsStoredFrameSize
		{
			get
			{
				// ybr full 422 only stores 2/3 of the pixels
				if (_photometricInterpretation != null && _photometricInterpretation.Equals("YBR_FULL_422"))
					return ImageWidth*ImageHeight*BytesAllocated*2;

				return (ImageWidth*ImageHeight*BitsStored*SamplesPerPixel)/8;
			}
		}

		/// <summary>
		/// The <see cref="TransferSyntax"/> the pixel data is encoded in.
		/// </summary>
		public TransferSyntax TransferSyntax
		{
			get { return _transferSyntax; }
			set { _transferSyntax = value; }
		}

		/// <summary>
		/// The <see cref="SopClass"/> of the pixel data.
		/// </summary>
		public SopClass SopClass
		{
			get { return _sopClass; }
			set { _sopClass = value; }
		}

		/// <summary>
		/// Flag telling if the SOP Class of the pixel data supports Modality LUTs.
		/// </summary>
		public bool SopSupportsModalityLut
		{
			get
			{
				if (_sopClass == null) return false;

				if (_sopClass.Uid.Equals(SopClass.CtImageStorageUid)
				    || _sopClass.Uid.Equals(SopClass.ComputedRadiographyImageStorageUid)
				    || _sopClass.Uid.Equals(SopClass.SecondaryCaptureImageStorageUid)
				    || _sopClass.Uid.Equals(SopClass.XRayAngiographicImageStorageUid)
				    || _sopClass.Uid.Equals(SopClass.XRayRadiofluoroscopicImageStorageUid)
				    || _sopClass.Uid.Equals(SopClass.XRayAngiographicBiPlaneImageStorageRetiredUid)
				    || _sopClass.Uid.Equals(SopClass.RtImageStorageUid)
				    || _sopClass.Uid.Equals(SopClass.MultiFrameGrayscaleByteSecondaryCaptureImageStorageUid)
				    || _sopClass.Uid.Equals(SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorageUid)
				    || _sopClass.Uid.Equals(SopClass.MultiFrameSingleBitSecondaryCaptureImageStorageUid)
				    || _sopClass.Uid.Equals(SopClass.MultiFrameTrueColorSecondaryCaptureImageStorageUid))
// PET IOD has a note in the IOD that Rescale Intercept is also 0, so we say its not supported here
//				    || _sopClass.Uid.Equals(Dicom.SopClass.PositronEmissionTomographyImageStorageUid))
					return true;

				return false;
			}
		}

		#region General Image Module

		/// <summary>
		/// A flag telling if the pixel dat ahas been lossy compressed at any time.
		/// </summary>
		public string LossyImageCompression
		{
			// Not always in an image, do a manual update.   
			get { return _lossyImageCompression; }
			set { _lossyImageCompression = value; }
		}

		/// <summary>
		/// If the pixel data has been lossy compressed at any time, the ratio of the compression.
		/// </summary>
		public float LossyImageCompressionRatio { get; set; }

		/// <summary>
		/// If the pixel data is derived, a description of the derivation.
		/// </summary>
		public string DerivationDescription
		{
			get { return _derivationDescription; }
			set { _derivationDescription = value; }
		}

		/// <summary>
		/// If the pixel data has been lossy compressed at any time, the method of the compression.
		/// </summary>
		public string LossyImageCompressionMethod
		{
			get { return _lossyImageCompressionMethod; }
			set { _lossyImageCompressionMethod = value; }
		}

		#endregion

		#endregion

		/// <summary>
		/// Gets the corresponding low bit for the given bits stored and high bit.
		/// </summary>
		public static ushort GetLowBit(int bitsStored, int highBit)
		{
			return (ushort) (highBit - (bitsStored - 1));
		}

		#region Min/Max Pixel Values

		/// <summary>
		/// Gets the minimum pixel value for the given bits stored and sign.
		/// </summary>
		public static int GetMinPixelValue(int bitsStored, bool isSigned)
		{
			return isSigned ? -(1 << (bitsStored - 1)) : 0;
		}

		/// <summary>
		/// Gets the maximum pixel value for the given bits stored and sign.
		/// </summary>
		public static int GetMaxPixelValue(int bitsStored, bool isSigned)
		{
			return isSigned ? (1 << (bitsStored - 1)) - 1 : (1 << bitsStored) - 1;
		}

		#endregion
	}
}
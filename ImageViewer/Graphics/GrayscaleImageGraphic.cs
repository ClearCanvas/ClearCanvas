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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Validation;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A grayscale <see cref="ImageGraphic"/>.
	/// </summary>
	[Cloneable]
	public class GrayscaleImageGraphic
		: ImageGraphic, 
		IVoiLutInstaller, 
		IColorMapInstaller, 
		IModalityLutProvider, 
		IVoiLutProvider, 
		IColorMapProvider,
		ILutPipelineProvider
	{
		private enum Luts
		{ 
			Modality = 1,
			Voi = 2
        }

		#region Private fields

		private int _bitsStored;
		private int _highBit;
		private bool _isSigned;

		private double _rescaleSlope;
		private double _rescaleIntercept;

		private LutComposer _lutComposer;
		private LutFactory _lutFactory;
		private IVoiLutManager _voiLutManager;

		[CloneCopyReference]
		private IGraphicVoiLutFactory _voiLutFactory;

		private IColorMapManager _colorMapManager;
		private IColorMap _colorMap;

		#endregion

		#region Public constructors

		/// <summary>
		/// Initializes a new instance of <see cref="GrayscaleImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <remarks>
		/// <para>
		/// Creates an empty grayscale image of a specific size.
		/// By default, all pixels are set to zero (i.e., black). 
		/// Useful as a canvas on which pixels can be set by the client.
		/// </para>
		/// <para>
		/// By default, the image is 16-bit unsigned with
		/// <i>bits stored = 16</i>, <i>high bit = 15</i>,
		/// <i>rescale slope = 1.0</i> and <i>rescale intercept = 0.0</i>.
		/// </para>
		/// </remarks>
		public GrayscaleImageGraphic(int rows, int columns)
			: base(rows, columns, 16 /* bits allocated */)
		{
			Initialize(16, 15, false, 1, 0, false);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="GrayscaleImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsAllocated">Can be 8 or 16.</param>
		/// <param name="bitsStored"></param>
		/// <param name="highBit"></param>
		/// <param name="isSigned"></param>
		/// <param name="inverted"></param>
		/// <param name="rescaleSlope"></param>
		/// <param name="rescaleIntercept"></param>
		/// <param name="pixelData"></param>
		/// <remarks>
		/// Creates an grayscale image using existing pixel data.
		/// </remarks>
		public GrayscaleImageGraphic(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			bool isSigned,
			bool inverted,
			double rescaleSlope,
			double rescaleIntercept,
			byte[] pixelData)
			: base(
				rows,
				columns,
				bitsAllocated,
				pixelData)
		{
			Initialize(bitsStored, highBit, isSigned, rescaleSlope, rescaleIntercept, inverted);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="GrayscaleImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsAllocated"></param>
		/// <param name="bitsStored"></param>
		/// <param name="highBit"></param>
		/// <param name="isSigned"></param>
		/// <param name="inverted"></param>
		/// <param name="rescaleSlope"></param>
		/// <param name="rescaleIntercept"></param>
		/// <param name="pixelDataGetter"></param>
		/// <remarks>
		/// Creates a grayscale image using existing pixel data but does so
		/// without ever storing a reference to the pixel data. This is necessary
		/// to ensure that pixel data can be properly garbage collected in
		/// any future memory management schemes.
		/// </remarks>
		public GrayscaleImageGraphic(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			bool isSigned,
			bool inverted,
			double rescaleSlope,
			double rescaleIntercept,
			PixelDataGetter pixelDataGetter)
			: base(
				rows,
				columns,
				bitsAllocated,
				pixelDataGetter)
		{
			Initialize(bitsStored, highBit, isSigned, rescaleSlope, rescaleIntercept, inverted);
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected GrayscaleImageGraphic(GrayscaleImageGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);

			if (source.LutComposer.ModalityLut != null) //modality lut is constant; no need to clone.
				this.InitializeNecessaryLuts(Luts.Modality);

			if (source.LutComposer.NormalizationLut != null)
				LutComposer.NormalizationLut = source.NormalizationLut.Clone();

			if (source.LutComposer.VoiLut != null) //clone the voi lut.
				(this as IVoiLutInstaller).InstallVoiLut(source.VoiLut.Clone());

            if (source.LutComposer.PresentationLut != null)//not really necessary, but consistent.
                LutComposer.PresentationLut = source.PresentationLut.Clone();

			//color map has already been cloned.
		}

		#endregion

		#region Public Properties / Methods

		/// <summary>
		/// Gets the number of bits stored in the image.
		/// </summary>
		/// <remarks>
		/// The number of bits stored does not necessarily equal the number of bits
		/// allocated. Values of 8, 10, 12 and 16 are typical.
		/// </remarks>
		public int BitsStored
		{
			get { return _bitsStored; }
		}

		/// <summary>
		/// Gets the high bit.
		/// </summary>
		/// <remarks>
		/// Theoretically, the high bit does not necessarily have to equal
		/// Bits Stored - 1.  But in almost all cases this assumption is true;
		/// we too make this assumption.
		/// </remarks>
		public int HighBit
		{
			get { return _highBit; }
		}

		/// <summary>
		/// Get a value indicating whether the image's pixel data is signed.
		/// </summary>
		public bool IsSigned
		{
			get { return _isSigned; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the image should be inverted.
		/// </summary>
		/// <remarks>
		/// Inversion is equivalent to polarity.
		/// </remarks>
		public bool Invert { get; set; }

	    /// <summary>
	    /// Gets the default value of <see cref="Invert"/>.  In DICOM, this would be true
	    /// for all MONOCHROME1 images.
	    /// </summary>
	    public bool DefaultInvert { get; private set; }

		/// <summary>
		/// Gets or sets the VOI LUT factory for this <see cref="GrayscaleImageGraphic"/>.
		/// </summary>
		public IGraphicVoiLutFactory VoiLutFactory
		{
			get { return _voiLutFactory; }
			set { _voiLutFactory = value; }
		}

		/// <summary>
		/// Gets the slope of the linear modality LUT rescale function.
		/// </summary>
		public double RescaleSlope
		{
			get { return _rescaleSlope; }
		}

		/// <summary>
		/// Gets the intercept of the linear modality LUT rescale function.
		/// </summary>
		public double RescaleIntercept
		{
			get { return _rescaleIntercept; }
		}

		/// <summary>
		/// Gets an object that encapsulates the pixel data.
		/// </summary>
		public new GrayscalePixelData PixelData
		{
			get
			{
				return base.PixelData as GrayscalePixelData;
			}
		}

		#region IVoiLutProvider Members

		/// <summary>
		/// Retrieves this image's <see cref="IVoiLutManager"/>.
		/// </summary>
		public IVoiLutManager VoiLutManager
		{
			get 
			{
				if (_voiLutManager == null)
					_voiLutManager = new VoiLutManager(this, false);

				return _voiLutManager;
			}
		}

		#endregion

		#region IColorMapProvider Members

		/// <summary>
		/// Retrieves this image's <see cref="IColorMapManager"/>.
		/// </summary>
		public IColorMapManager ColorMapManager
		{
			get
			{
				if (_colorMapManager == null)
					_colorMapManager = new ColorMapManager(this);

				return _colorMapManager;
			}
		}

		#endregion

		#region ILutPipelineProvider Members

		public double LookupPixelValue(int rawPixelValue, LutPipelineStage outStage)
		{
			double value = rawPixelValue;

			if (outStage == LutPipelineStage.Source)
				return value;

			var modalityLut = ModalityLut;
			if (modalityLut != null)
				value = modalityLut[value];

			if (outStage == LutPipelineStage.Modality)
				return value;

			var normalizationLut = NormalizationLut;
			if (normalizationLut != null)
				value = normalizationLut[value];

			var voiLut = VoiLut;
			if (voiLut != null)
				value = voiLut[value];

			if (outStage == LutPipelineStage.Voi)
				return value;

			Platform.Log(LogLevel.Debug, "Unrecognized LUT pipeline stage");
			return value;
		}

		#endregion

		/// <summary>
		/// Retrieves this image's modality lut.
		/// </summary>
		public IModalityLut ModalityLut
		{
			get
			{
				InitializeNecessaryLuts(Luts.Modality);
				return this.LutComposer.ModalityLut; 
			}
		}

        /// <summary>
        /// Gets or sets a LUT to normalize the output of the modality LUT immediately prior to the VOI LUT.
        /// </summary>
        /// <remarks>
        /// <para>
        /// In most cases, this should be left NULL. However, some PET images have a very small rescale slope (&lt;&lt; 1)
        /// and thus need this to fix the input to the VOI LUT.
        /// </para>
        /// <para>
        /// At any rate, DO NOT use the output of this LUT for any purpose other than as an input to the VOI LUT, as it is meaningless otherwise.</para>
        /// </remarks>
        public IComposableLut NormalizationLut
        {
            get { return LutComposer.NormalizationLut; }
            set { LutComposer.NormalizationLut = value; }
        }

		/// <summary>
		/// Retrieves this image's Voi Lut.
		/// </summary>
		public IVoiLut VoiLut
		{
			get
			{
				InitializeNecessaryLuts(Luts.Voi);
				return this.LutComposer.VoiLut;
			}
		}

        /// <summary>
        /// Gets this image's presentation LUT.
        /// </summary>
        /// <remarks>You should not need to set this property unless you are writing a custom renderer
        /// for 10-bit grayscale (for example) displays, or need an alternate range of p-values for some other output device.
        /// Otherwise, this property is managed internally, assuming an 8-bit display.</remarks>
	    public IPresentationLut PresentationLut
	    {
	        get
	        {
                InitializeNecessaryLuts(Luts.Voi);
	            return this.LutComposer.PresentationLut;
	        }
            set { this.LutComposer.PresentationLut = value; }
	    }

	    /// <summary>
		/// Gets the output lut composed of both the Modality and Voi Luts,
		/// properly rescaled/normalized for the given output display range.
		/// </summary>
		public IComposedLut GetOutputLut(int minDisplayValue, int maxDisplayValue)
		{
			InitializeNecessaryLuts(Luts.Voi);
            return LutComposer.GetOutputLut(minDisplayValue, maxDisplayValue);
		}

		/// <summary>
		/// Retrieves this image's color map.
		/// </summary>
		public IColorMap ColorMap
		{
			get
			{
				if (_colorMap == null)
					(this as IColorMapInstaller).InstallColorMap(this.LutFactory.GetGrayscaleColorMap());

				return _colorMap;
			}
		}

		#endregion

		#region Private properties

		private LutComposer LutComposer
		{
			get
			{
				if (_lutComposer == null)
					_lutComposer = new LutComposer(this.BitsStored, this.IsSigned);

				return _lutComposer;
			}
		}

		private LutFactory LutFactory
		{
			get
			{
				if (_lutFactory == null)
					_lutFactory = LutFactory.Create();

				return _lutFactory;
			}
		}

		#endregion

		#region Disposal

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_lutFactory != null)
				{
					_lutFactory.Dispose();
					_lutFactory = null;
				}

				if (_lutComposer != null)
				{
					_lutComposer.Dispose();
					_lutComposer = null;
				}
			}
		}

		#endregion

		/// <summary>
		/// Creates an object that encapsulates the pixel data.
		/// </summary>
		/// <returns></returns>
		protected override PixelData CreatePixelDataWrapper()
		{
			if (this.PixelDataRaw != null)
			{
				return new GrayscalePixelData(
					this.Rows,
					this.Columns,
					this.BitsPerPixel,
					this.BitsStored,
					this.HighBit,
					this.IsSigned,
					this.PixelDataRaw);
			}
			else
			{
				return new GrayscalePixelData(
					this.Rows,
					this.Columns,
					this.BitsPerPixel,
					this.BitsStored,
					this.HighBit,
					this.IsSigned,
					this.PixelDataGetter);
			}
		}

		#region Private methods

		private void Initialize(
			int bitsStored,
			int highBit,
			bool isSigned,
			double rescaleSlope, 
			double rescaleIntercept, 
			bool invert)
		{
			DicomValidator.ValidateBitsStored(bitsStored);
			DicomValidator.ValidateHighBit(highBit);

			_bitsStored = bitsStored;
			_highBit = highBit;
			_isSigned = isSigned;
			_rescaleSlope = rescaleSlope <= double.Epsilon ? 1 : rescaleSlope;
			_rescaleIntercept = rescaleIntercept;
			DefaultInvert = Invert = invert;
		}

		private void InitializeNecessaryLuts(Luts luts)
		{
			if (luts >= Luts.Modality && LutComposer.ModalityLut == null)
			{
				IModalityLut modalityLut =
					this.LutFactory.GetModalityLutLinear(this.BitsStored, this.IsSigned, _rescaleSlope, _rescaleIntercept);
			
				this.LutComposer.ModalityLut = modalityLut;
			}

			if (luts >= Luts.Voi && LutComposer.VoiLut == null)
			{
				IVoiLut lut = null;

				if (_voiLutFactory != null)
					lut = _voiLutFactory.CreateVoiLut(this);

				if (lut == null)
					lut = new IdentityVoiLinearLut();

				(this as IVoiLutInstaller).InstallVoiLut(lut);
			}
        }

		#endregion

		#region Explicit Properties / Methods

		IEnumerable<ColorMapDescriptor> IColorMapInstaller.AvailableColorMaps
		{
			get
			{
				return this.LutFactory.AvailableColorMaps;
			}
		}

		void IVoiLutInstaller.InstallVoiLut(IVoiLut voiLut)
		{
			Platform.CheckForNullReference(voiLut, "voiLut");

			InitializeNecessaryLuts(Luts.Modality);

			this.LutComposer.VoiLut = voiLut;
		}

		void IColorMapInstaller.InstallColorMap(string name)
		{
			(this as IColorMapInstaller).InstallColorMap(this.LutFactory.GetColorMap(name));
		}

		void IColorMapInstaller.InstallColorMap(ColorMapDescriptor descriptor)
		{
			(this as IColorMapInstaller).InstallColorMap(descriptor.Name);
		}

		void IColorMapInstaller.InstallColorMap(IColorMap colorMap)
		{
			if (_colorMap == colorMap)
				return;

			_colorMap = colorMap;
		}

		#endregion
	}
}

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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An image where pixel values are ARGB.
	/// </summary>
	/// <remarks>
	/// A typical usage of this class is overlaying
	/// colour regions on a grayscale image to highlight areas of interest.
	/// Note that you can control not just the colour, but also the 
	/// opacity (i.e. alpha) of each pixel.
	/// </remarks>
	[Cloneable]
	public class ColorImageGraphic
		: ImageGraphic,
		IVoiLutProvider,
		IVoiLutInstaller
	{
		/// <summary>
		/// We only support VOI LUTs right now, but theoretically we
		/// can implement ICC Profiles with an "ICC Transform LUT"
		/// after the VOI.
		/// </summary>
		private enum Luts
		{
			/// <summary>
			/// This LUT is initially a <see cref="IdentityVoiLinearLut"/>,
			/// but may be replaced by W/L tools with something else, such as
			/// <see cref="BasicVoiLutLinear"/>
			/// </summary>
			Voi = 0
		}

		#region Private Fields

		private LutComposer _lutComposer;
		private IVoiLutManager _voiLutManager;

		[CloneCopyReference]
		private IGraphicVoiLutFactory _voiLutFactory;

		#endregion

		#region Public constructors

		/// <summary>
		/// Initializes a new instance of <see cref="ColorImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <remarks>
		/// Creates an empty colour image of a specific size.
		/// By default, all pixels are set to ARGB=(0,0,0,0) (i.e.,
		/// transparent). 
		/// </remarks>
		public ColorImageGraphic(int rows, int columns)
			: base(rows, columns, 32)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ColorImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="pixelData">The pixel data in ARGB format.</param>
		/// <remarks>
		/// Creates a colour image using existing pixel data.
		/// </remarks>
		public ColorImageGraphic(int rows, int columns, byte[] pixelData)
			: base(rows, columns, 32, pixelData)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ColorImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="pixelDataGetter"></param>
		/// <remarks>
		/// Creates a grayscale image using existing pixel data but does so
		/// without ever storing a reference to the pixel data. This is necessary
		/// to ensure that pixel data can be properly garbage collected in
		/// any future memory management schemes.
		/// </remarks>
		public ColorImageGraphic(int rows, int columns, PixelDataGetter pixelDataGetter)
			: base(rows, columns, 32, pixelDataGetter)
		{
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected ColorImageGraphic(ColorImageGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);

			if (source.LutComposer.VoiLut != null) //clone the voi lut.
				this.InstallVoiLut(source.VoiLut.Clone());
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Gets or sets a value indicating whether the LUT should be used in rendering this graphic.
		/// </summary>
		public bool VoiLutsEnabled
		{
			get { return this.VoiLutManager.Enabled; }
			set { this.VoiLutManager.Enabled = value; }
		}

		/// <summary>
		/// Gets or sets the VOI LUT factory for this <see cref="ColorImageGraphic"/>.
		/// </summary>
		public IGraphicVoiLutFactory VoiLutFactory
		{
			get { return _voiLutFactory; }
			set { _voiLutFactory = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not the image should be inverted.
		/// </summary>
		/// <remarks>
		/// Inversion is equivalent to polarity.
		/// </remarks>
		public bool Invert { get; set; }

	    /// <summary>
	    /// Gets the default value of <see cref="Invert"/>.  In DICOM, this would be true
	    /// for all MONOCHROME1 images.
	    /// </summary>
        public bool DefaultInvert { get { return false; } }

		/// <summary>
		/// Gets an object that encapsulates the pixel data.
		/// </summary>
		public new ColorPixelData PixelData
		{
			get
			{
				return base.PixelData as ColorPixelData;
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
					_voiLutManager = new VoiLutManager(this, true);

				return _voiLutManager;
			}
		}

		#endregion

		/// <summary>
		/// Gets or sets the LUT used to select and rescale values of interest in the color pixel data <b>on a per-channel basis</b>.
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
		/// The output lut composed of all LUTs in the pipeline.
		/// </summary>
		public IComposedLut OutputLut
		{
			get
			{
				InitializeNecessaryLuts(Luts.Voi);
				return this.LutComposer.GetOutputLut(0, byte.MaxValue);
			}
		}

		#endregion

		#region Private properties

		private LutComposer LutComposer
		{
			get
			{
				if (_lutComposer == null)
					_lutComposer = new LutComposer(8, false);

				return _lutComposer;
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
				if (_lutComposer != null)
					_lutComposer.Dispose();
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
				return new ColorPixelData(this.Rows, this.Columns, this.PixelDataRaw);
			else
				return new ColorPixelData(this.Rows, this.Columns, this.PixelDataGetter);
		}

		#region Private methods

		private void InitializeNecessaryLuts(Luts luts)
		{
			if (luts >= Luts.Voi && LutComposer.VoiLut == null)
			{
				IVoiLut lut = null;
				
				if (_voiLutFactory != null)
					lut = _voiLutFactory.CreateVoiLut(this);

				if (lut == null)
					lut = new IdentityVoiLinearLut();

				InstallVoiLut(lut);
			}
		}

		#endregion

		#region Private / Explicit Members

		private void InstallVoiLut(IVoiLut voiLut)
		{
			Platform.CheckForNullReference(voiLut, "voiLut");

			this.LutComposer.VoiLut = voiLut;
		}

		void IVoiLutInstaller.InstallVoiLut(IVoiLut voiLut)
		{
			InstallVoiLut(voiLut);
		}

		#endregion
	}
}

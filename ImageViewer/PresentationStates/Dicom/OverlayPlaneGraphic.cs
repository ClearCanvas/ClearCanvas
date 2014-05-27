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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// A 1-bit bitmap overlay plane <see cref="IGraphic"/>.
	/// </summary>
	/// <remarks>
	/// This implementation does not support overlays embedded in the unused bits of the
	/// <see cref="DicomTags.PixelData"/> attribute, a retired usage from previous versions
	/// of the DICOM Standard. 
	/// </remarks>
	[Cloneable]
	public class OverlayPlaneGraphic : CompositeGraphic, IShutterGraphic
	{
		[CloneIgnore]
		private GrayscaleImageGraphic _overlayGraphic;

		private readonly int _index;
		private readonly int _frameIndex;
		private readonly OverlayPlaneSource _source;
		private string _label;
		private string _description;
		private OverlayPlaneSubtype _subtype;
		private OverlayPlaneType _type;
		private ushort _grayPresentationValue = 0;
		private Color? _color;

		/// <summary>
		/// Constructs an <see cref="OverlayPlaneGraphic"/> for a single-frame overlay plane using a pre-processed overlay pixel data buffer.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This overload should only be used for single-frame overlay planes. Multi-frame overlay planes should process the overlay data
		/// into separate buffers and then construct individual graphics using <see cref="OverlayPlaneGraphic(OverlayPlane, byte[], int, OverlayPlaneSource)"/>.
		/// </para>
		/// <para>
		/// The <paramref name="overlayPixelData"/> parameter allows for the specification of an alternate source of overlay pixel data, such
		/// as the unpacked contents of <see cref="DicomTags.OverlayData"/> or the extracted, inflated overlay pixels of <see cref="DicomTags.PixelData"/>.
		/// Although the format should be 8-bits per pixel, every pixel should either be 0 or 255. This will allow pixel interpolation algorithms
		/// sufficient range to produce a pleasant image. (If the data was either 0 or 1, regardless of the bit-depth, most interpolation algorithms
		/// will interpolate 0s for everything in between!)
		/// </para>
		/// </remarks>
		/// <param name="overlayPlaneIod">The IOD object containing properties of the overlay plane.</param>
		/// <param name="overlayPixelData">The overlay pixel data in 8-bits per pixel format, with each pixel being either 0 or 255.</param>
		/// <param name="source">A value identifying the source of the overlay plane.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="overlayPixelData"/> is NULL or 0-length.</exception>
		public OverlayPlaneGraphic(OverlayPlane overlayPlaneIod, byte[] overlayPixelData, OverlayPlaneSource source) : this(overlayPlaneIod, overlayPixelData, 0, source) {}

		/// <summary>
		/// Constructs an <see cref="OverlayPlaneGraphic"/> for a single or multi-frame overlay plane using a pre-processed overlay pixel data buffer.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The <paramref name="overlayPixelData"/> parameter allows for the specification of an alternate source of overlay pixel data, such
		/// as the unpacked contents of <see cref="DicomTags.OverlayData"/> or the extracted, inflated overlay pixels of <see cref="DicomTags.PixelData"/>.
		/// Although the format should be 8-bits per pixel, every pixel should either be 0 or 255. This will allow pixel interpolation algorithms
		/// sufficient range to produce a pleasant image. (If the data was either 0 or 1, regardless of the bit-depth, most interpolation algorithms
		/// will interpolate 0s for everything in between!)
		/// </para>
		/// </remarks>
		/// <param name="overlayPlaneIod">The IOD object containing properties of the overlay plane.</param>
		/// <param name="overlayPixelData">The overlay pixel data in 8-bits per pixel format, with each pixel being either 0 or 255.</param>
		/// <param name="frameIndex">The overlay frame index (0-based). Single-frame overlays should specify 0.</param>
		/// <param name="source">A value identifying the source of the overlay plane.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="overlayPixelData"/> is NULL or 0-length.</exception>
		public OverlayPlaneGraphic(OverlayPlane overlayPlaneIod, byte[] overlayPixelData, int frameIndex, OverlayPlaneSource source)
		{
			Platform.CheckNonNegative(frameIndex, "frameIndex");
			_frameIndex = frameIndex;
			_index = overlayPlaneIod.Index;
			_label = overlayPlaneIod.OverlayLabel;
			_description = overlayPlaneIod.OverlayDescription;
			_type = overlayPlaneIod.OverlayType;
			_subtype = (OverlayPlaneSubtype) overlayPlaneIod.OverlaySubtype;
			_source = source;

			GrayscaleImageGraphic overlayImageGraphic = CreateOverlayImageGraphic(overlayPlaneIod, overlayPixelData);
			if (overlayImageGraphic != null)
			{
				_overlayGraphic = overlayImageGraphic;
				this.Color = System.Drawing.Color.PeachPuff;
				base.Graphics.Add(overlayImageGraphic);
			}

			if (string.IsNullOrEmpty(overlayPlaneIod.OverlayLabel))
			{
				if (overlayPlaneIod.IsMultiFrame)
					base.Name = string.Format(SR.FormatDefaultMultiFrameOverlayGraphicName, _source, _index, frameIndex);
				else
					base.Name = string.Format(SR.FormatDefaultSingleFrameOverlayGraphicName, _source, _index, frameIndex);
			}
			else
			{
				base.Name = overlayPlaneIod.OverlayLabel;
			}
		}

		/// <summary>
		/// Constructs a new user-created <see cref="OverlayPlaneGraphic"/> with the specified dimensions.
		/// </summary>
		/// <param name="rows">The number of rows in the overlay.</param>
		/// <param name="columns">The number of columns in the overlay.</param>
		protected OverlayPlaneGraphic(int rows, int columns)
		{
			Platform.CheckPositive(rows, "rows");
			Platform.CheckPositive(columns, "columns");

			_index = -1;
			_frameIndex = 0;
			_label = string.Empty;
			_description = string.Empty;
			_type = OverlayType.G;
			_subtype = null;
			_source = OverlayPlaneSource.User;
			_overlayGraphic = new GrayscaleImageGraphic(
				rows, columns, // the reported overlay dimensions
				8, // bits allocated is always 8
				8, // overlays always have bit depth of 1, but we upconverted the data
				7, // the high bit is now 7 after upconverting
				false, false, // overlays aren't signed and don't get inverted
				1, 0, // overlays have no rescale
				MemoryManager.Allocate<byte>(rows*columns)); // new empty pixel buffer

			this.Color = System.Drawing.Color.PeachPuff;
			base.Graphics.Add(_overlayGraphic);
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected OverlayPlaneGraphic(OverlayPlaneGraphic source, ICloningContext context) : base()
		{
			context.CloneFields(source, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_overlayGraphic = CollectionUtils.SelectFirst(base.Graphics,
			                                              delegate(IGraphic graphic) { return graphic is GrayscaleImageGraphic; }) as GrayscaleImageGraphic;
		}

		private static GrayscaleImageGraphic CreateOverlayImageGraphic(OverlayPlane overlayPlaneIod, byte[] overlayData)
		{
			Point origin = (overlayPlaneIod.OverlayOrigin ?? new Point(1, 1)) - new Size(1, 1);
			int rows = overlayPlaneIod.OverlayRows;
			int cols = overlayPlaneIod.OverlayColumns;

			if (overlayData == null || overlayData.Length == 0)
				return null;

			GrayscaleImageGraphic imageGraphic = new GrayscaleImageGraphic(
				rows, cols, // the reported overlay dimensions
				8, // bits allocated is always 8
				8, // overlays always have bit depth of 1, but we upconverted the data
				7, // the high bit is now 7 after upconverting
				false, false, // overlays aren't signed and don't get inverted
				1, 0, // overlays have no rescale
				overlayData); // the upconverted overlay data

			imageGraphic.SpatialTransform.TranslationX = origin.X;
			imageGraphic.SpatialTransform.TranslationY = origin.Y;

			return imageGraphic;
		}

		private void UpdateLuts()
		{
			if (_overlayGraphic == null)
				return;

			// NOTE: this determination is actually supposed to be based on the client display device
			if (_color == null || _color.Value.IsEmpty)
			{
				//Normalize the gray presentation value because our algorithms here don't work for presentation value=0
				ushort normalizedGrayPresentationValue = Math.Max((ushort) 1, _grayPresentationValue);
				_overlayGraphic.VoiLutManager.InstallVoiLut(new OverlayVoiLut(normalizedGrayPresentationValue, 65535));
				_overlayGraphic.ColorMapManager.InstallColorMap(new GrayscaleColorMap(normalizedGrayPresentationValue));
			}
			else
			{
				//The color makes the gray p-value irrelevant, so do this to save space.
				_overlayGraphic.VoiLutManager.InstallVoiLut(new OverlayVoiLut(255, 255));
				_overlayGraphic.ColorMapManager.InstallColorMap(new OverlayColorMap(_color.Value));
			}
		}

		/// <summary>
		/// Gets the raw overlay pixel data as an 8-bit grayscale pixel data buffer.
		/// </summary>
		protected byte[] OverlayPixelData
		{
			get { return _overlayGraphic != null ? _overlayGraphic.PixelData.Raw : null; }
		}

		/// <summary>
		/// Gets the overlay group index in which the overlay was encoded.
		/// </summary>
		public int Index
		{
			get { return _index; }
		}

		/// <summary>
		/// Gets the frame index of this overlay frame within the original overlay data.
		/// </summary>
		public int FrameIndex
		{
			get { return _frameIndex; }
		}

		/// <summary>
		/// Gets a value indicating the source of the overlay plane.
		/// </summary>
		public OverlayPlaneSource Source
		{
			get { return _source; }
		}

		/// <summary>
		/// Gets the label or name of the overlay plane.
		/// </summary>
		public string Label
		{
			get { return _label; }
			protected set { _label = value; }
		}

		/// <summary>
		/// Gets the description of the overlay plane.
		/// </summary>
		public string Description
		{
			get { return _description; }
			protected set { _description = value; }
		}

		/// <summary>
		/// Gets a value indicating the type of content represented by the overlay plane.
		/// </summary>
		public OverlayPlaneType Type
		{
			get { return _type; }
			protected set { _type = value; }
		}

		/// <summary>
		/// Gets a value identifying the intended purpose of the overlay.
		/// </summary>
		public OverlayPlaneSubtype Subtype
		{
			get { return _subtype; }
			protected set { _subtype = value; }
		}

		/// <summary>
		/// Gets the location of the top left corner of the overlay relative to the image.
		/// </summary>
		public PointF Origin
		{
			get
			{
				if (_overlayGraphic == null)
					return new PointF(1, 1);
				return new PointF(_overlayGraphic.SpatialTransform.TranslationX + 1, _overlayGraphic.SpatialTransform.TranslationY + 1);
			}
			protected set
			{
				if (_overlayGraphic != null)
				{
					_overlayGraphic.SpatialTransform.TranslationX = value.X - 1;
					_overlayGraphic.SpatialTransform.TranslationY = value.Y - 1;
				}
			}
		}

		/// <summary>
		/// Gets the number of rows in the overlay plane.
		/// </summary>
		public int Rows
		{
			get { return _overlayGraphic != null ? _overlayGraphic.Rows : 0; }
		}

		/// <summary>
		/// Gets the number of columns in the overlay plane.
		/// </summary>
		public int Columns
		{
			get { return _overlayGraphic != null ? _overlayGraphic.Columns : 0; }
		}

		/// <summary>
		/// Gets or sets the 16-bit grayscale presentation value in which the overlay should be drawn.
		/// </summary>
		/// <remarks>
		/// DICOM overlays are strictly 1-bit images, so each overlay pixels is either present or not.
		/// The display of present pixels is no longer defined by the data source but is now
		/// implementation specific. The <see cref="GrayPresentationValue"/> and <see cref="Color"/>
		/// properties allows this display to be customized by client code. A null value for
		/// <see cref="Color"/> causes the <see cref="GrayPresentationValue"/> to be used.
		/// </remarks>
		public ushort GrayPresentationValue
		{
			get { return _grayPresentationValue; }
			set
			{
				if (_grayPresentationValue == value)
					return;

				Platform.CheckArgumentRange(value, 0, 65535, "_grayPresentationValue");

				_grayPresentationValue = value;
				UpdateLuts();
			}
		}

		/// <summary>
		/// Gets or sets the presentation color in which the overlay should be drawn.
		/// </summary>
		/// <remarks>
		/// DICOM overlays are strictly 1-bit images, so each overlay pixels is either present or not.
		/// The display of present pixels is no longer defined by the data source but is now
		/// implementation specific. The <see cref="GrayPresentationValue"/> and <see cref="Color"/>
		/// properties allows this display to be customized by client code. A null value for
		/// <see cref="Color"/> causes the <see cref="GrayPresentationValue"/> to be used.
		/// </remarks>
		public Color? Color
		{
			get { return _color; }
			set
			{
				if (_color == value)
					return;

				_color = value;
				UpdateLuts();
			}
		}

		/// <summary>
		/// Creates a packed overlay data object from the contents of this overlay plane.
		/// </summary>
		/// <param name="bigEndianWords">A value indciating if the packed overlay data should be encoded as 16-bit words with big endian byte ordering.</param>
		/// <returns>A packed overlay data object.</returns>
		public OverlayData CreateOverlayData(bool bigEndianWords)
		{
			if (_overlayGraphic == null)
				return null;

			GrayscalePixelData pixelData = _overlayGraphic.PixelData;
			return OverlayData.CreateOverlayData(
				pixelData.Rows, pixelData.Columns,
				pixelData.BitsStored,
				pixelData.BitsAllocated,
				pixelData.HighBit,
				bigEndianWords,
				pixelData.Raw);
		}

		#region IShutterGraphic Members

		/// <summary>
		/// Gets or sets the 16-bit grayscale presentation value which should replace the shuttered pixels.
		/// </summary>
		/// <remarks>
		/// The display of shuttered pixels is no longer defined by the data source but is now
		/// implementation specific. The <see cref="IShutterGraphic.PresentationValue"/> and <see cref="IShutterGraphic.PresentationColor"/>
		/// properties allows this display to be customized by client code.
		/// </remarks>
		ushort IShutterGraphic.PresentationValue
		{
			get { return this.GrayPresentationValue; }
			set { this.GrayPresentationValue = value; }
		}

		/// <summary>
		/// Gets or sets the presentation color which should replace the shuttered pixels.
		/// </summary>
		/// <remarks>
		/// The display of shuttered pixels is no longer defined by the data source but is now
		/// implementation specific. The <see cref="IShutterGraphic.PresentationValue"/> and <see cref="IShutterGraphic.PresentationColor"/>
		/// properties allows this display to be customized by client code.
		/// </remarks>
		Color IShutterGraphic.PresentationColor
		{
			get { return this.Color ?? System.Drawing.Color.Empty; }
			set { this.Color = value; }
		}

		#endregion

		#region Voi Lut

		[Cloneable(true)]
		private class OverlayVoiLut : ComposableVoiLut
		{
			private readonly int _presentationValue;
			private double _minInputValue = 0;
			private double _maxInputValue = 0;
			private readonly int _maxOutputValue;

			public OverlayVoiLut(int presentationValue, int maxOutputValue)
			{
				_presentationValue = presentationValue;
				_maxOutputValue = maxOutputValue;
			}

			//for cloning.
			private OverlayVoiLut() {}

			public override double MinInputValue
			{
				get { return _minInputValue; }
				set { _minInputValue = value; }
			}

			public override double MaxInputValue
			{
				get { return _maxInputValue; }
				set { _maxInputValue = value; }
			}

			public override double MinOutputValue
			{
				get { return 0; }
				protected set { }
			}

			public override double MaxOutputValue
			{
				get { return _maxOutputValue; }
				protected set { }
			}

			public override double this[double index]
			{
				get { return (index/_maxInputValue)*_presentationValue; }
			}

			public override void LookupValues(double[] input, double[] output, int count)
			{
				LutFunctions.LookupScaleValue(input, output, count, _presentationValue/_maxInputValue);
			}

			public override string GetKey()
			{
				return String.Format("OverlayVoi_{0}_{1}_{2}", this.MinInputValue, this.MaxInputValue, _presentationValue);
			}

			public override string GetDescription()
			{
				return "Overlay Voi";
			}
		}

		#endregion

		#region Color Map

		[Cloneable(true)]
		internal class GrayscaleColorMap : Imaging.GrayscaleColorMap
		{
			private readonly ushort _presentationValue;

			public GrayscaleColorMap(ushort presentationValue)
			{
				_presentationValue = presentationValue;
			}

			/// <summary>
			/// Cloning constructor
			/// </summary>
			private GrayscaleColorMap() {}

			protected override void Create()
			{
				int j = 0;
				int min = MinInputValue;
				int max = MaxInputValue;
				double maxGrayLevel = this.Length - 1;
				double alphaRange = _presentationValue - min;

				for (int i = min; i <= max; i++)
				{
					double scale = j/maxGrayLevel;
					double alphaScale = Math.Min(1f, j/alphaRange);
					j++;

					byte value = (byte) Math.Round(byte.MaxValue*scale);
					byte alpha = (byte) Math.Round(byte.MaxValue*alphaScale);
					this[i] = alpha << 24 | (value << 16) | (value << 8) | value;
				}
			}

			public override string GetKey()
			{
				return String.Format("OverlayGrayColorMap_{0}_{1}", this.MinInputValue, this.MaxInputValue);
			}
		}

		[Cloneable(true)]
		private class OverlayColorMap : ColorMap
		{
			private Color _color = Color.Gray;

			public OverlayColorMap() {}

			public OverlayColorMap(Color color)
			{
				_color = color;
			}

			public Color Color
			{
				get { return _color; }
				set
				{
					if (_color != value)
					{
						_color = value;
						this.Clear();
					}
				}
			}

			protected override void Create()
			{
				for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
					this[i] = Color.FromArgb(i, _color).ToArgb();
			}

			public override string GetDescription()
			{
				return string.Format("OverlayColorMap_{0}_{1}_{2}_{3}", this.Color.A, this.Color.R, this.Color.G, this.Color.B);
			}
		}

		#endregion
	}
}
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	partial class ProgressGraphic
	{
		[Cloneable]
		internal class ProgressCompositeGraphic : CompositeGraphic
		{
			[CloneIgnore]
			private readonly Size _size = new Size(240, 80);

			[CloneIgnore]
			private ITextGraphic _progressTextGraphic;

			[CloneIgnore]
			private ProgressBarGraphic _progressBarGraphic;

			[CloneIgnore]
			private ColorImageGraphic _background;

			[CloneIgnore]
			private readonly ProgressBarGraphicStyle _style;

			public ProgressCompositeGraphic(ProgressBarGraphicStyle style)
			{
				_style = style;

				Initialize();
			}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			/// <param name="source">The source object from which to clone.</param>
			/// <param name="context">The cloning context object.</param>
			protected ProgressCompositeGraphic(ProgressCompositeGraphic source, ICloningContext context)
			{
				context.CloneFields(source, this);

				_style = source._style;
			}

			[OnCloneComplete]
			private void OnCloneComplete()
			{
				_progressTextGraphic = (ITextGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g is ITextGraphic);
				_progressBarGraphic = (ProgressBarGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g is ProgressBarGraphic);
				_background = (ColorImageGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g is ColorImageGraphic);

				Initialize();
			}

			private void Initialize()
			{
				if (_background == null)
				{
					var pixelCount = _size.Width*_size.Height;
					var pixelData = new byte[4*pixelCount];
					var fillColor = Color.FromArgb(196, 85, 85, 85).ToArgb();
					unsafe
					{
						fixed (byte* pPixelBytes = pixelData)
						{
							var pPixels = (int*) pPixelBytes;
							for (var n = 0; n < pixelCount; ++n)
								pPixels[n] = fillColor;
						}
					}
					base.Graphics.Add(_background = new ColorImageGraphic(_size.Height, _size.Width, pixelData));
				}

				if (_progressBarGraphic == null)
				{
					base.Graphics.Add(_progressBarGraphic = ProgressBarGraphic.Create(_style));
					var offset = Center(_size, _progressBarGraphic.Size) + new Size(0, 10);
					_progressBarGraphic.SpatialTransform.TranslationX = offset.X;
					_progressBarGraphic.SpatialTransform.TranslationY = offset.Y;
				}

				if (_progressTextGraphic == null)
				{
					base.Graphics.Add(_progressTextGraphic = new InvariantTextPrimitive());
					var offset = Center(_size, new Size(1, 1)) - new Size(0, 15);
					_progressTextGraphic.SpatialTransform.TranslationX = offset.X;
					_progressTextGraphic.SpatialTransform.TranslationY = offset.Y;
				}
			}

			public override void OnDrawing()
			{
				if (base.ParentPresentationImage != null)
				{
					SpatialTransform transform = base.SpatialTransform;
					transform.TranslationX = (base.ParentPresentationImage.ClientRectangle.Width - this.Width)/2f;
					transform.TranslationY = (base.ParentPresentationImage.ClientRectangle.Height - this.Height)/2f;
				}
				base.OnDrawing();
			}

			protected override SpatialTransform CreateSpatialTransform()
			{
				return new InvariantSpatialTransform(this);
			}

			public string Text
			{
				get { return _progressTextGraphic.Text; }
				set { _progressTextGraphic.Text = value; }
			}

			public float Progress
			{
				get { return _progressBarGraphic.Progress; }
				set { _progressBarGraphic.Progress = value; }
			}

			public ProgressBarGraphicStyle Style
			{
				get { return _style; }
			}

			public Size Size
			{
				get { return _size; }
			}

			public int Width
			{
				get { return _size.Width; }
			}

			public int Height
			{
				get { return _size.Height; }
			}

			private static Point Center(Size bounds, Size size)
			{
				return new Point((bounds.Width - size.Width)/2, (bounds.Height - size.Height)/2);
			}
		}
	}
}
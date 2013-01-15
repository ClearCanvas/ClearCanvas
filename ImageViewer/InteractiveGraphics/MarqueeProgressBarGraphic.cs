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

using System.Drawing;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	partial class ProgressBarGraphic
	{
		[Cloneable(true)]
		private class MarqueeProgressBarGraphic : ProgressBarGraphic
		{
			[CloneIgnore]
			private readonly Image _tray;

			[CloneIgnore]
			private readonly Image _bar;

			[CloneIgnore]
			private readonly Image _border;

			[CloneIgnore]
			private Bitmap _mask;

			[CloneIgnore]
			private int _offset;

			[CloneIgnore]
			private readonly int _maskWidth;

			[CloneIgnore]
			private readonly int _barWidth;

			public MarqueeProgressBarGraphic()
			{
				_tray = GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicTray.png");
				_bar = GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicContinuousBar.png");
				_border = GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicBorder.png");
				_mask = new Bitmap(GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicMarqueeMask.png"));

				_maskWidth = _mask.Width;
				_barWidth = _bar.Width;
				_offset = 0;
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					//TODO (CR Sept 2010): what about _tray, etc?
					if (_mask != null)
					{
						_mask.Dispose();
						_mask = null;
					}
				}

				base.Dispose(disposing);
			}

			public override ProgressBarGraphicStyle Style
			{
				get { return ProgressBarGraphicStyle.Marquee; }
			}

			public override Size Size
			{
				get { return _tray.Size; }
			}

			public override void OnDrawing()
			{
				if (Progress > 0f && Progress < 1f)
				{
					_offset = ((_offset + 3)%(_barWidth + _maskWidth));
					Update();
				}

				base.OnDrawing();
			}

			protected override void RenderProgressBar(float progress, System.Drawing.Graphics g)
			{
				g.DrawImageUnscaledAndClipped(_tray, new Rectangle(Point.Empty, _tray.Size));

				if (progress <= 0f)
				{
					// paint nothing
				}
				else if (progress >= 1f)
				{
					// paint the full bar
					DrawImageCentered(g, _bar);
				}
				else
				{
					// paint a portion of the bar using the alpha channel of the marquee mask
					using (Bitmap bar = new Bitmap(_bar))
					{
						int cols = _bar.Size.Width;
						int rows = _bar.Size.Height;
						int size = rows*cols;

						for (int i = 0; i < size; i++)
						{
							int x = i%cols;
							int offsetX = x - _offset + _maskWidth;
							int y = i/cols;
							if (offsetX >= 0 && offsetX < _mask.Width)
							{
								var c = bar.GetPixel(x, y);
								bar.SetPixel(x, y, Color.FromArgb(_mask.GetPixel(offsetX, y).A*c.A/255, c));
							}
							else
							{
								bar.SetPixel(x, y, Color.Transparent);
							}
						}

						DrawImageCentered(g, bar);
					}
				}
				DrawImageCentered(g, _border);
			}
		}
	}
}
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
		private class ContinuousProgressBarGraphic : ProgressBarGraphic
		{
			[CloneIgnore]
			private readonly Image _tray;

			[CloneIgnore]
			private readonly Image _bar;

			[CloneIgnore]
			private readonly Image _border;

			public ContinuousProgressBarGraphic()
			{
				_tray = GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicTray.png");
				_bar = GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicContinuousBar.png");
				_border = GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicBorder.png");
			}

			public override ProgressBarGraphicStyle Style
			{
				get { return ProgressBarGraphicStyle.Continuous; }
			}

			public override Size Size
			{
				get { return _tray.Size; }
			}

			protected override void RenderProgressBar(float progress, System.Drawing.Graphics g)
			{
				g.DrawImageUnscaledAndClipped(_tray, new Rectangle(Point.Empty, _tray.Size));
				using (Bitmap bar = new Bitmap(_bar))
				{
					int cols = _bar.Size.Width;
					int rows = _bar.Size.Height;
					int size = rows*cols;
					int max = (int) (progress*cols);
					for (int i = 0; i < size; i++)
					{
						if (i%cols > max)
							bar.SetPixel(i%cols, i/cols, Color.Transparent);
					}
					DrawImageCentered(g, bar);
				}
				DrawImageCentered(g, _border);
			}
		}
	}
}
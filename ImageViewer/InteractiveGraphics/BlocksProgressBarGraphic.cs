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

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	partial class ProgressBarGraphic
	{
		[Cloneable(true)]
		private class BlocksProgressBarGraphic : ProgressBarGraphic
		{
			[CloneIgnore]
			private readonly Image _border;

			[CloneIgnore]
			private readonly Image _block;

			[CloneIgnore]
			private Bitmap _tray;

			[CloneIgnore]
			private readonly int _blockWidth;

			[CloneIgnore]
			private readonly int _blockCount;

			[CloneIgnore]
			private readonly int _blockOffsetY;

			public BlocksProgressBarGraphic()
			{
				_tray = new Bitmap(GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicTray.png"));
				_border = GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicBorder.png");
				_block = GetImageResource("InteractiveGraphics.Resources.ProgressBarGraphicBlocksUnit.png");

				_blockWidth = _block.Width;
				_blockCount = (int) Math.Ceiling(1f*_tray.Width/_blockWidth);
				_blockOffsetY = (_tray.Height - _block.Height)/2;
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					//TODO (CR Sept 2010): dispose _border and _block?
					if (_tray != null)
					{
						_tray.Dispose();
						_tray = null;
					}
				}

				base.Dispose(disposing);
			}

			public override ProgressBarGraphicStyle Style
			{
				get { return ProgressBarGraphicStyle.Blocks; }
			}

			public override Size Size
			{
				get { return _tray.Size; }
			}

			protected override void RenderProgressBar(float progress, System.Drawing.Graphics g)
			{
				int max = (int) (progress*_tray.Width);
				if (progress >= 1f)
					max = _blockCount*_blockWidth;

				using (Bitmap blocks = new Bitmap(_tray))
				{
					// draw the blocks to a temporary buffer
					using (System.Drawing.Graphics gBlocks = System.Drawing.Graphics.FromImage(blocks))
					{
						for (int x = 0; x < max; x += _blockWidth)
							gBlocks.DrawImageUnscaledAndClipped(_block, new Rectangle(new Point(x, _blockOffsetY), _block.Size));
					}

					// remask the blocks using the tray as a guideline
					int cols = _tray.Size.Width;
					int rows = _tray.Size.Height;
					int size = rows*cols;
					for (int i = 0; i < size; i++)
					{
						int x = i%cols;
						int y = i/cols;
						blocks.SetPixel(x, y, Color.FromArgb(_tray.GetPixel(x, y).A, blocks.GetPixel(x, y)));
					}

					// paint the temporary buffer onto the real buffer
					DrawImageCentered(g, blocks);
				}

				DrawImageCentered(g, _border);
			}
		}
	}
}
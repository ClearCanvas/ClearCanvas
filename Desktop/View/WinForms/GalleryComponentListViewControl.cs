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
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	internal class GalleryComponentListViewControl : ListView
	{
		private int _insertionBoxIndex = -1;
		private long _lastDragScrollEvent = 0;

		public int InsertionBoxIndex
		{
			get { return _insertionBoxIndex; }
			set
			{
				if (value < -1 || value >= base.Items.Count)
					value = -1;
				if (_insertionBoxIndex != value)
				{
					_insertionBoxIndex = value;
					base.Invalidate();
				}
			}
		}

		protected override void OnDragOver(DragEventArgs drgevent)
		{
			base.OnDragOver(drgevent);

			//TODO (Time Review): Use Environment.TickCount
			const int dragScrollDelay = 200; // in ms
			long now = DateTime.Now.Ticks/10000;
			if (base.Scrollable && now - _lastDragScrollEvent > dragScrollDelay)
			{
				Point clientPoint = base.PointToClient(new Point(drgevent.X, drgevent.Y));
				bool scrollUp = (clientPoint.Y < this.Height*3/20);
				bool scrollDown = (clientPoint.Y > this.Height*17/20);

				if (scrollUp || scrollDown)
				{
					int nearestIndex;
					ListViewItem lvi = base.GetItemAt(clientPoint.X, clientPoint.Y);
					if (lvi != null)
						nearestIndex = base.Items.IndexOf(lvi);
					else
						nearestIndex = base.InsertionMark.NearestIndex(clientPoint);

					if (nearestIndex >= 0)
					{
						_lastDragScrollEvent = now;
						if (scrollUp)
						{
							nearestIndex = Math.Max(nearestIndex - 1, 0);
						}
						else
						{
							nearestIndex = Math.Min(nearestIndex + 1, base.Items.Count - 1);
						}
						base.EnsureVisible(nearestIndex);
					}
				}
			}
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			const int WM_PAINT = 0x0f;
			if (m.Msg == WM_PAINT)
			{
				if (_insertionBoxIndex >= 0 && _insertionBoxIndex < base.Items.Count)
				{
					Rectangle rect = base.GetItemRect(_insertionBoxIndex, ItemBoundsPortion.Entire);
					using (Graphics g = base.CreateGraphics())
					{
						using (Pen p = new Pen(base.ForeColor, 2))
						{
							g.DrawRectangle(p, rect);
						}
					}
				}
			}
		}
	}
}
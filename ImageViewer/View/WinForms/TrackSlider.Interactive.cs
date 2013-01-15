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
using System.Diagnostics;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	partial class TrackSlider
	{
		// these aren't volatile because System.Windows.Forms.Timer uses the message pump to execute the tick on the UI thread
		private bool _thumbDrag = false;
		private bool _arrowHold = false;
		private int _arrowHoldCount = -1;
		private int _autoHideAlpha = 0;

		public void Flash()
		{
			if (_autoHideAlpha != 255)
			{
				_autoHideAlpha = 255;
				this.Invalidate();
			}
		}

		private void OnTimerTick(object sender, EventArgs e)
		{
			// don't do anything if this control isn't visible, or is not on an active form
			if (!this.Visible || this.Parent == null || Form.ActiveForm != this.FindForm())
				return;

			// perform sanity check now; do not allow actions if it fails check
			if (_minimumValue > _maximumValue)
				return;

			int alpha = _autoHideAlpha;

			if (_thumbDrag || !_autoHide)
			{
				// user is dragging the thumb slider, or we're not in auto hide mode
				alpha = 255;
			}
			else
			{
				// increase the opacity if the cursor is inside the control bounds
				if (this.RectangleToScreen(this.ClientRectangle).Contains(Cursor.Position))
					alpha = Math.Min(255, alpha + 5);
				else
					alpha = Math.Max(_minimumAlpha, alpha - 5);
			}

			if (_autoHideAlpha != alpha)
			{
				// we only need to invalidate and paint if the alpha changed
				_autoHideAlpha = alpha;
				this.Invalidate();
			}

			// if the user is holding down on the arrowheads, increment/decrement every now and then
			if (_arrowHold && _arrowHoldCount == 0)
			{
				UserAction userAction;
				int value = this.ComputeTrackClickValue(this.PointToClient(Cursor.Position), out userAction);
				if (userAction != UserAction.None)
					SetValue(value, userAction);
			}

			_arrowHoldCount = (_arrowHoldCount + 1)%16;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			// if the control is invisible or disabled, disallow interaction
			if (!base.CanFocus)
				return;

			// if the control isn't already focused, give it focus now
			if (!base.Focused)
				base.Focus();

			base.OnMouseDown(e);

			// perform sanity check now; do not allow actions if it fails check
			if (_minimumValue > _maximumValue)
				return;

			_thumbDrag = _trackBar.ThumbBounds.Contains(e.Location - this.PaddingOffset);
			if (!_thumbDrag)
			{
				UserAction userAction;
				int value = this.ComputeTrackClickValue(e.Location - this.PaddingOffset, out userAction);
				if (userAction != UserAction.None)
					SetValue(value, userAction);

				_arrowHoldCount = 1;
				_arrowHold = true;
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			// if the control is invisible or disabled, disallow interaction
			if (!base.CanFocus)
				return;

			base.OnMouseMove(e);

			// perform sanity check now; do not allow actions if it fails check
			if (_minimumValue > _maximumValue)
				return;

			if (e.Button == MouseButtons.Left)
			{
				if (_thumbDrag)
					SetValue(this.ComputeThumbDragValue(e.Location - this.PaddingOffset), UserAction.DragThumb);
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			_arrowHold = false;
			_thumbDrag = false;

			// if the control is invisible or disabled, disallow interaction
			if (!base.CanFocus)
				return;

			base.OnMouseUp(e);
		}

		/// <summary>
		/// Computes the value of the slider at the specified location.
		/// </summary>
		private int ComputeThumbDragValue(Point location)
		{
			float percentile;

			if (_orientation == Orientation.Vertical)
				percentile = 1f*(location.Y - _trackBar.TrackBounds.Top)/_trackBar.TrackBounds.Height;
			else
				percentile = 1f*(location.X - _trackBar.TrackBounds.Left)/_trackBar.TrackBounds.Width;

			percentile = Math.Min(1, Math.Max(0, percentile));
			return (int) Math.Round((_maximumValue - _minimumValue)*percentile + _minimumValue);
		}

		/// <summary>
		/// Computes the next value of the slider based on a clicked location.
		/// </summary>
		private int ComputeTrackClickValue(Point location, out UserAction userAction)
		{
			if (_trackBar.TrackBounds.Contains(location))
			{
				userAction = UserAction.ClickTrack;
				return this.ComputeThumbDragValue(location);
			}
			else if (_trackBar.TrackStartBounds.Contains(location))
			{
				userAction = UserAction.ClickArrow;
				return Math.Max(_minimumValue, Math.Min(_maximumValue, _value - _increment));
				
			}
			else if (_trackBar.TrackEndBounds.Contains(location))
			{
				userAction = UserAction.ClickArrow;
				return Math.Max(_minimumValue, Math.Min(_maximumValue, _value + _increment));
			}

			userAction = UserAction.None;
			return _value;
		}
	}
}
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// A minimalistic slider control that does not have as much wasted space as <see cref="TrackBar"/>.
	/// </summary>
	public class SliderBar : TrackBar
	{
		private const int _defaultThumbSize = 8;
		private const int _defaultTrackSize = 4;

		private Rectangle _trackBounds;
		private Rectangle _thumbBounds;

		private int _trackSize = _defaultTrackSize;
		private int _thumbSize = _defaultThumbSize;
		private bool _showFocusRectangle = true;
		private bool _thumbDragging = false;

		/// <summary>
		/// Initializes a new instance of <see cref="SliderBar"/>.
		/// </summary>
		public SliderBar()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.Selectable, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.UserMouse, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.UseTextForAccessibility, false);

			base.AutoSize = false;
		}

		[DefaultValue(false)]
		public override bool AutoSize
		{
			get { return base.AutoSize; }
			set { base.AutoSize = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not focus rectangles should be drawn when the control is focused.
		/// </summary>
		[DefaultValue(true)]
		public bool ShowFocusRectangle
		{
			get { return _showFocusRectangle; }
			set
			{
				if (_showFocusRectangle == value) return;
				_showFocusRectangle = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the width or height of the slider thumb.
		/// </summary>
		/// <remarks>
		/// Which dimension is modified depends on the orientation, as the other dimension is always fixed.
		/// </remarks>
		[DefaultValue(_defaultThumbSize)]
		public int ThumbSize
		{
			get { return _thumbSize; }
			set
			{
				if (value < 1)
				{
					const string message = "ThumbSize must be at least 1.";
					throw new ArgumentOutOfRangeException("value", value, message);
				}
				if (_thumbSize == value) return;
				_thumbSize = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the width or height of the slider track bar.
		/// </summary>
		/// <remarks>
		/// Which dimension is modified depends on the orientation, as the other dimension is always fixed.
		/// </remarks>
		[DefaultValue(_defaultTrackSize)]
		public int TrackSize
		{
			get { return _trackSize; }
			set
			{
				if (value < 1)
				{
					const string message = "TrackSize must be at least 1.";
					throw new ArgumentOutOfRangeException("value", value, message);
				}
				if (_trackSize == value) return;
				_trackSize = value;
				Invalidate();
			}
		}

		protected override void OnGotFocus(EventArgs e)
		{
			Invalidate();
			base.OnGotFocus(e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			Invalidate();
			base.OnLostFocus(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			Invalidate();
			base.OnMouseDown(e);

			if (IsLeftMouseButtonDown())
			{
				_thumbDragging = true;
				Value = PointToValue(e.Location);
				OnScroll(new EventArgs());
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			Invalidate();
			base.OnMouseUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			Invalidate();
			base.OnMouseMove(e);

			if (IsLeftMouseButtonDown())
			{
				_thumbDragging = true;
				Value = PointToValue(e.Location);
				OnScroll(new EventArgs());
			}
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			Invalidate();
			base.OnMouseEnter(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			Invalidate();
			base.OnMouseLeave(e);
		}

		protected override void OnValueChanged(EventArgs e)
		{
			Invalidate();
			base.OnValueChanged(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			const int thumbPadding = 2;
			const int trackPadding = 2;

			var left = ClientRectangle.Left;
			var top = ClientRectangle.Top;
			var width = ClientRectangle.Width;
			var height = ClientRectangle.Height;
			var cursor = PointToClient(MousePosition);

			// paint focus rectangle
			if (ShowFocusRectangle && Focused) ControlPaint.DrawFocusRectangle(e.Graphics, ClientRectangle);

			if (!IsLeftMouseButtonDown()) _thumbDragging = false;

			// compute object bounds
			if (Orientation == Orientation.Horizontal)
			{
				_trackBounds = new Rectangle(left + trackPadding, top + height/2 - _trackSize/2, width - 2*trackPadding, _trackSize);
				_thumbBounds = new Rectangle(ValueToPoint(Value).X - _thumbSize/2, top + thumbPadding, _thumbSize, height - 2*thumbPadding);
			}
			else
			{
				_trackBounds = new Rectangle(left + width/2 - _trackSize/2, top + trackPadding, _trackSize, height - 2*trackPadding);
				_thumbBounds = new Rectangle(left + thumbPadding, ValueToPoint(Value).Y - _thumbSize/2, width - 2*thumbPadding, _thumbSize);
			}

			if (System.Windows.Forms.Application.RenderWithVisualStyles)
			{
				if (Orientation == Orientation.Horizontal)
				{
					// paint track
					var renderer = new VisualStyleRenderer(VisualStyleElement.TrackBar.Track.Normal);
					renderer.DrawBackground(e.Graphics, _trackBounds);

					// paint thumb
					if (!Enabled)
						renderer.SetParameters(VisualStyleElement.TrackBar.Thumb.Disabled);
					else if (_thumbDragging)
						renderer.SetParameters(VisualStyleElement.TrackBar.Thumb.Pressed);
					else if (!_thumbBounds.Contains(cursor))
						renderer.SetParameters(Focused ? VisualStyleElement.TrackBar.Thumb.Focused : VisualStyleElement.TrackBar.Thumb.Normal);
					else
						renderer.SetParameters(IsLeftMouseButtonDown() ? VisualStyleElement.TrackBar.Thumb.Pressed : VisualStyleElement.TrackBar.Thumb.Hot);
					renderer.DrawBackground(e.Graphics, _thumbBounds);
				}
				else
				{
					// paint track
					var renderer = new VisualStyleRenderer(VisualStyleElement.TrackBar.TrackVertical.Normal);
					renderer.DrawBackground(e.Graphics, _trackBounds);

					// paint thumb
					if (!Enabled)
						renderer.SetParameters(VisualStyleElement.TrackBar.ThumbVertical.Disabled);
					else if (_thumbDragging)
						renderer.SetParameters(VisualStyleElement.TrackBar.ThumbVertical.Pressed);
					else if (!_thumbBounds.Contains(cursor))
						renderer.SetParameters(Focused ? VisualStyleElement.TrackBar.ThumbVertical.Focused : VisualStyleElement.TrackBar.ThumbVertical.Normal);
					else
						renderer.SetParameters(IsLeftMouseButtonDown() ? VisualStyleElement.TrackBar.ThumbVertical.Pressed : VisualStyleElement.TrackBar.ThumbVertical.Hot);
					renderer.DrawBackground(e.Graphics, _thumbBounds);
				}
			}
			else
			{
				// paint track
				ControlPaint.DrawBorder3D(e.Graphics, _trackBounds, Border3DStyle.Sunken);

				// paint thumb
				if (!Enabled)
					ControlPaint.DrawButton(e.Graphics, _thumbBounds, ButtonState.Inactive);
				else
					ControlPaint.DrawButton(e.Graphics, _thumbBounds, ButtonState.Normal);
			}
		}

		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case 0x114:
				case 0x115:
				case 0x2114:
				case 0x2115:
					switch (((int) m.WParam.ToInt64() & 65535))
					{
						case 0:
						case 1:
						case 2:
						case 3:
						case 5:
						case 6:
						case 7:
						case 8:
							Invalidate();
							OnScroll(EventArgs.Empty);
							OnValueChanged(EventArgs.Empty);
							return;
					}
					break;
			}
			base.WndProc(ref m);
		}

		/// <summary>
		/// Calculates the equivalent value along the track bar for the specified point.
		/// </summary>
		/// <param name="point">The location of the point, in control coordinates.</param>
		/// <returns>The equivalent value along the track bar for the specified point.</returns>
		public int PointToValue(Point point)
		{
			if (Orientation != Orientation.Horizontal)
				return Math.Max(Minimum, Math.Min(Maximum, Rescale(point.Y, _trackBounds.Bottom - _thumbSize/2, _trackBounds.Top + _thumbSize/2, Minimum, Maximum)));
			else if (RightToLeft == RightToLeft.Yes && !IsMirrored)
				return Math.Max(Minimum, Math.Min(Maximum, Rescale(point.X, _trackBounds.Right - _thumbSize/2, _trackBounds.Left + _thumbSize/2, Minimum, Maximum)));
			else
				return Math.Max(Minimum, Math.Min(Maximum, Rescale(point.X, _trackBounds.Left + _thumbSize/2, _trackBounds.Right - _thumbSize/2, Minimum, Maximum)));
		}

		/// <summary>
		/// Calculates the point on the control equivalent to specified value on the track bar.
		/// </summary>
		/// <param name="value">The value on the track bar.</param>
		/// <returns>The equivalent point along the track bar, in control coordinates, for the specified value.</returns>
		public Point ValueToPoint(int value)
		{
			if (Orientation != Orientation.Horizontal)
				return new Point(Width/2, Rescale(Math.Max(Minimum, Math.Min(Maximum, value)), Minimum, Maximum, _trackBounds.Bottom - _thumbSize/2, _trackBounds.Top + _thumbSize/2));
			else if (RightToLeft == RightToLeft.Yes && !IsMirrored)
				return new Point(Rescale(Math.Max(Minimum, Math.Min(Maximum, value)), Minimum, Maximum, _trackBounds.Right - _thumbSize/2, _trackBounds.Left + _thumbSize/2), Height/2);
			else
				return new Point(Rescale(Math.Max(Minimum, Math.Min(Maximum, value)), Minimum, Maximum, _trackBounds.Left + _thumbSize/2, _trackBounds.Right - _thumbSize/2), Height/2);
		}

		/// <summary>
		/// Determine if the specified point is over any part of the slider control.
		/// </summary>
		/// <param name="p">The location of the point, in control coordinates.</param>
		/// <returns>A <see cref="SliderBarHitTestParts"/> value indicating which parts of the slider control.</returns>
		public SliderBarHitTestParts HitTest(Point p)
		{
			return HitTest(p.X, p.Y);
		}

		/// <summary>
		/// Determine if the specified point is over any part of the slider control.
		/// </summary>
		/// <param name="x">The X-coordinate of the point, in control coordinates.</param>
		/// <param name="y">The Y-coordinate of the point, in control coordinates.</param>
		/// <returns>A <see cref="SliderBarHitTestParts"/> value indicating which parts of the slider control.</returns>
		public SliderBarHitTestParts HitTest(int x, int y)
		{
			var result = SliderBarHitTestParts.None;
			if (_thumbBounds.Contains(x, y)) result |= SliderBarHitTestParts.Thumb;
			if (_trackBounds.Contains(x, y)) result |= SliderBarHitTestParts.Track;
			return result;
		}

		/// <summary>
		/// Gets the current bounds of the slider thumb.
		/// </summary>
		public Rectangle GetThumbRect()
		{
			return _thumbBounds;
		}

		/// <summary>
		/// Gets the current bounds of the slider track bar.
		/// </summary>
		public Rectangle GetTrackBounds()
		{
			return _trackBounds;
		}

		private static bool IsLeftMouseButtonDown()
		{
			return (MouseButtons & MouseButtons.Left) == MouseButtons.Left;
		}

		private static int Rescale(int srcValue, int srcMinimum, int srcMaximum, int dstMinimum, int dstMaximum)
		{
			var srcRange = srcMaximum - srcMinimum;
			return srcRange == 0 ? dstMinimum : (int) Math.Round(1f*(srcValue - srcMinimum)/srcRange*(dstMaximum - dstMinimum) + dstMinimum);
		}
	}

	[Flags]
	public enum SliderBarHitTestParts
	{
		None = 0,
		Thumb = 1,
		Track = 2,
		Both = Thumb | Track
	}
}
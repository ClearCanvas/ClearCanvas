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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Interactive builder class that interprests two mouse clicks as the
	/// circle centre followed by a point on the circumference.
	/// </summary>
	/// <remarks>
	/// This builder takes exactly two clicks, after which the graphic is
	/// complete and control is released.
	/// </remarks>
	public class InteractiveCircleGraphicBuilder : InteractiveGraphicBuilder
	{
		private int _numberOfPointsAnchored = 0;
		private PointF _centre;

		/// <summary>
		/// Constructs an interactive builder for the specified boundable graphic.
		/// </summary>
		/// <param name="boundableGraphic">The boundable graphic to be interactively built.</param>
		public InteractiveCircleGraphicBuilder(IBoundableGraphic boundableGraphic) : base(boundableGraphic) {}

		/// <summary>
		/// Gets the graphic that the builder is operating on.
		/// </summary>
		public new IBoundableGraphic Graphic
		{
			get { return (IBoundableGraphic) base.Graphic; }
		}

		/// <summary>
		/// Resets any internal state of the builder, allowing the same graphic to be rebuilt.
		/// </summary>
		public override void Reset()
		{
			_numberOfPointsAnchored = 0;
			base.Reset();
		}

		/// <summary>
		/// Rolls back the internal state of the builder by one mouse click, allowing the same graphic to be rebuilt by resuming from an earlier state.
		/// </summary>
		protected override void Rollback()
		{
			_numberOfPointsAnchored = Math.Max(_numberOfPointsAnchored - 1, 0);
		}

		/// <summary>
		/// Passes user input to the builder when <see cref="IMouseButtonHandler.Start"/> is called on the owning tool.
		/// </summary>
		/// <param name="mouseInformation">The user input data.</param>
		/// <returns>True if the builder did something as a result of the call, and hence would like to receive capture; False otherwise.</returns>
		public override bool Start(IMouseInformation mouseInformation)
		{
			// We just started creating
			if (_numberOfPointsAnchored == 0)
			{
				_centre = mouseInformation.Location;

				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
				this.Graphic.TopLeft = _centre;
				this.Graphic.BottomRight = _centre;
				this.Graphic.ResetCoordinateSystem();

				_numberOfPointsAnchored++;
			}
			// We're done creating
			else
			{
				_numberOfPointsAnchored++;

                // TODO ??
                // When user moves the mouse very quickly and events are filtered for performance purpose (eg web viewer case), 
                // the final point may not be the same as the last tracked point. Must update the final point based on the latest mouse position.

                
				this.NotifyGraphicComplete();
			}

			return true;
		}

		/// <summary>
		/// Passes user input to the builder when <see cref="IMouseButtonHandler.Track"/> is called on the owning tool.
		/// </summary>
		/// <param name="mouseInformation">The user input data.</param>
		/// <returns>True if the builder handled the message; False otherwise.</returns>
		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_numberOfPointsAnchored == 1)
			{
				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
				try
				{
					float radius = (float) Vector.Distance(_centre, mouseInformation.Location);
					SizeF offset = new SizeF(radius, radius);
					this.Graphic.TopLeft = _centre - offset;
					this.Graphic.BottomRight = _centre + offset;
				}
				finally
				{
					this.Graphic.ResetCoordinateSystem();
				}

				this.Graphic.Draw();
			}
			return true;
		}

		/// <summary>
		/// Passes user input to the builder when <see cref="IMouseButtonHandler.Stop"/> is called on the owning tool.
		/// </summary>
		/// <param name="mouseInformation">The user input data.</param>
		/// <returns>True if the tool should not release capture; False otherwise.</returns>
		public override bool Stop(IMouseInformation mouseInformation)
		{
			return true;
		}
	}
}
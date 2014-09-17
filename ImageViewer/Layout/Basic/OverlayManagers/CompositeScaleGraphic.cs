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
using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Layout.Basic.OverlayManagers
{
	[Cloneable(false)]
	public class CompositeScaleGraphic : CompositeGraphic
	{
		private const string _horizontalName = "Horizontal scale";
		private const string _verticalName = "Vertical scale";

		[CloneIgnore]
		private ScaleGraphic _horizontalScale;

		[CloneIgnore]
		private ScaleGraphic _verticalScale;

		public CompositeScaleGraphic()
		{
			Graphics.Add(_horizontalScale = new ScaleGraphic {Name = _horizontalName});
			Graphics.Add(_verticalScale = new ScaleGraphic {Name = _verticalName});

			_horizontalScale.Visible = false;
			_horizontalScale.IsMirrored = true;

			_verticalScale.Visible = false;
			_verticalScale.IsMirrored = true;
		}

		protected CompositeScaleGraphic(CompositeScaleGraphic source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_horizontalScale = Graphics.OfType<ScaleGraphic>().FirstOrDefault(g => g.Name == _horizontalName);
			_verticalScale = Graphics.OfType<ScaleGraphic>().FirstOrDefault(g => g.Name == _verticalName);
		}

		/// <summary>
		/// Fires the <see cref="Graphic.Drawing"/> event.  Should be called by an <see cref="IRenderer"/>
		/// for each object just before it is drawn/rendered, hence the reason it is public.
		/// </summary>
		public override void OnDrawing()
		{
			base.CoordinateSystem = CoordinateSystem.Destination;
			_horizontalScale.CoordinateSystem = CoordinateSystem.Destination;
			_verticalScale.CoordinateSystem = CoordinateSystem.Destination;

			try
			{
				Rectangle hScaleBounds = ComputeScaleBounds(base.ParentPresentationImage, 0.10f, 0.05f);
				_horizontalScale.SetEndPoints(new PointF(hScaleBounds.Left, hScaleBounds.Bottom), new SizeF(hScaleBounds.Width, 0));
				_horizontalScale.Visible = true;

				Rectangle vScaleBounds = ComputeScaleBounds(base.ParentPresentationImage, 0.05f, 0.10f);
				_verticalScale.SetEndPoints(new PointF(vScaleBounds.Right, vScaleBounds.Top), new SizeF(0, vScaleBounds.Height));
				_verticalScale.Visible = true;
			}
			finally
			{
				base.ResetCoordinateSystem();
				_horizontalScale.ResetCoordinateSystem();
				_verticalScale.ResetCoordinateSystem();
			}

			base.OnDrawing();
		}

		/// <summary>
		/// Computes the maximum bounds for scales on a given <see cref="IPresentationImage"/>.
		/// </summary>
		/// <param name="presentationImage">The image to compute bounds for.</param>
		/// <param name="horizontalReduction">The percentage of width to subtract from both the client bounds and the source image bounds.</param>
		/// <param name="verticalReduction">The percentage of height to subtract from both the client bounds and the source image bounds.</param>
		/// <returns>The maximum scale bounds.</returns>
		private static Rectangle ComputeScaleBounds(IPresentationImage presentationImage, float horizontalReduction, float verticalReduction)
		{
			RectangleF clientBounds = presentationImage.ClientRectangle;
			float hReduction = horizontalReduction*Math.Min(1000f, clientBounds.Width);
			float vReduction = verticalReduction*Math.Min(1000f, clientBounds.Height);

			clientBounds = new RectangleF(clientBounds.X + hReduction, clientBounds.Y + vReduction, clientBounds.Width - 2*hReduction, clientBounds.Height - 2*vReduction);

			if (presentationImage is IImageGraphicProvider)
			{
				ImageGraphic imageGraphic = ((IImageGraphicProvider) presentationImage).ImageGraphic;
				Rectangle srcRectangle = new Rectangle(0, 0, imageGraphic.Columns, imageGraphic.Rows);

				RectangleF imageBounds = imageGraphic.SpatialTransform.ConvertToDestination(srcRectangle);
				imageBounds = RectangleUtilities.ConvertToPositiveRectangle(imageBounds);
				hReduction = horizontalReduction*imageBounds.Width;
				vReduction = verticalReduction*imageBounds.Height;

				imageBounds = new RectangleF(imageBounds.X + hReduction, imageBounds.Y + vReduction, imageBounds.Width - 2*hReduction, imageBounds.Height - 2*vReduction);
				return Rectangle.Round(RectangleUtilities.Intersect(imageBounds, clientBounds));
			}
			else
			{
				return Rectangle.Round(clientBounds);
			}
		}
	}
}
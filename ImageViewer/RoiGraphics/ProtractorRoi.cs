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

using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	public class ProtractorRoi : Roi
	{
		private readonly List<PointF> _points;

		public ProtractorRoi(ProtractorGraphic protractor) : base(protractor.ParentPresentationImage)
		{
			_points = new List<PointF>();

			protractor.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				for (int i = 0; i < protractor.Points.Count; ++i)
					_points.Add(protractor.Points[i]);
			}
			finally
			{
				protractor.ResetCoordinateSystem();
			}
		}

		public ProtractorRoi(PointF point1, PointF vertex, PointF point2, IPresentationImage presentationImage)
			: base(presentationImage)
		{
			_points = new List<PointF>();
			_points.Add(point1);
			_points.Add(vertex);
			_points.Add(point2);
		}

		/// <summary>
		/// Three points in destination coordinates that define the angle.
		/// </summary>
		public List<PointF> Points
		{
			get { return _points; }
		}

		protected override RectangleF ComputeBounds()
		{
			return RectangleF.Empty;
		}

		public override Roi CopyTo(IPresentationImage presentationImage)
		{
			return new ProtractorRoi(_points[0], _points[1], _points[2], presentationImage);
		}

		public override bool Contains(PointF point)
		{
			return false;
		}
	}
}
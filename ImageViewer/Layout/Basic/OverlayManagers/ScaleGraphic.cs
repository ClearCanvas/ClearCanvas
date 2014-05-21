#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Layout.Basic.OverlayManagers
{
	/// <summary>
	/// Generic scale graphic class.
	/// </summary>
	[Cloneable(false)]
	public sealed class ScaleGraphic : CompositeGraphic
	{
		private const string _baseLineName = "Base scale line";
		private const string _tickLineName = "Scale tick line";

		[CloneIgnore]
		private LinePrimitive _baseLine;

		[CloneIgnore]
		private readonly List<LinePrimitive> _ticklines = new List<LinePrimitive>();

		private PointF _point1, _point2;
		private bool _isMirrored;
		private bool _isDirty;

		private float _minorTick = 10;
		private int _majorTickFrequency = 5;
		private int _minorTickLength = 13;
		private int _majorTickLength = 25;

		/// <summary>
		/// Constructs a <see cref="ScaleGraphic"/>.
		/// </summary>
		public ScaleGraphic()
		{
			Graphics.Add(_baseLine = new LinePrimitive {Name = _baseLineName});
			_isDirty = false;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		private ScaleGraphic(ScaleGraphic source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_baseLine = Graphics.OfType<LinePrimitive>().FirstOrDefault(g => g.Name == _baseLineName);
			_ticklines.AddRange(Graphics.OfType<LinePrimitive>().Where(g => g.Name == _tickLineName));
			_isDirty = true;
		}

		/// <summary>
		/// Gets or sets the minor tick spacing in millimetres.
		/// </summary>
		/// <remarks>
		/// The default minor tick spacing is 10 millimetres.
		/// </remarks>
		public float MinorTick
		{
			get { return _minorTick; }
			set
			{
// ReSharper disable CompareOfFloatsByEqualityOperator
				if (_minorTick != value)
// ReSharper restore CompareOfFloatsByEqualityOperator
				{
					_minorTick = value;
					FlagAsDirty();
				}
			}
		}

		/// <summary>
		/// Gets the major tick spacing in millimetres.
		/// </summary>
		public float MajorTick
		{
			get { return _majorTickFrequency*_minorTick; }
		}

		/// <summary>
		/// Gets or sets the major tick frequency.
		/// </summary>
		/// <remarks>
		/// The default major tick frequency is every fifth tick.
		/// </remarks>
		public int MajorTickFrequency
		{
			get { return _majorTickFrequency; }
			set
			{
				if (_majorTickFrequency != value)
				{
					_majorTickFrequency = value;
					FlagAsDirty();
				}
			}
		}

		/// <summary>
		/// Gets the minor tick length in display pixels.
		/// </summary>
		/// <remarks>
		/// The default minor tick length is 13 pixels.
		/// </remarks>
		public int MinorTickLength
		{
			get { return _minorTickLength; }
			set
			{
				if (_minorTickLength != value)
				{
					_minorTickLength = value;
					FlagAsDirty();
				}
			}
		}

		/// <summary>
		/// Gets the major tick length in display pixels.
		/// </summary>
		/// <remarks>
		/// The default major tick length is 25 pixels.
		/// </remarks>
		public int MajorTickLength
		{
			get { return _majorTickLength; }
			set
			{
				if (_majorTickLength != value)
				{
					_majorTickLength = value;
					FlagAsDirty();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating that the ticks are drawn in the opposite direction.
		/// </summary>
		public bool IsMirrored
		{
			get { return _isMirrored; }
			set
			{
				if (_isMirrored != value)
				{
					_isMirrored = value;
					FlagAsDirty();
				}
			}
		}

		/// <summary>
		/// Gets or sets an endpoint of the scale's base line segment in the current coordinate space.
		/// </summary>
		public PointF Point1
		{
			get { return CoordinateSystem == CoordinateSystem.Source ? _point1 : SpatialTransform.ConvertToDestination(_point1); }
			set
			{
				if (CoordinateSystem != CoordinateSystem.Source)
					value = SpatialTransform.ConvertToSource(value);

				if (value != _point1)
				{
					_point1 = value;
					FlagAsDirty();
				}
			}
		}

		/// <summary>
		/// Gets or sets the other endpoint of the scale's base line segment in the current coordinate space.
		/// </summary>
		public PointF Point2
		{
			get { return CoordinateSystem == CoordinateSystem.Source ? _point2 : SpatialTransform.ConvertToDestination(_point2); }
			set
			{
				if (CoordinateSystem != CoordinateSystem.Source)
					value = SpatialTransform.ConvertToSource(value);

				if (value != _point2)
				{
					_point2 = value;
					FlagAsDirty();
				}
			}
		}

		/// <summary>
		/// Sets both endpoints of the scale's base line segment in one atomic operation, cause only one update and draw.
		/// </summary>
		/// <param name="point1">One endpoint of the base line segment.</param>
		/// <param name="point2">The other endpoint of the base line segment.</param>
		public void SetEndPoints(PointF point1, PointF point2)
		{
			if (CoordinateSystem != CoordinateSystem.Source)
			{
				point1 = SpatialTransform.ConvertToSource(point1);
				point2 = SpatialTransform.ConvertToSource(point2);
			}

			if (point1 != _point1 || point2 != _point2)
			{
				_point1 = point1;
				_point2 = point2;
				FlagAsDirty();
			}
		}

		/// <summary>
		/// Sets both endpoints of the scale's base line segment in one atomic operation, cause only one update and draw.
		/// </summary>
		/// <param name="location">One endpoint of the base line segment.</param>
		/// <param name="offset">The offset of the other endpoint relative to the first endpoint.</param>
		public void SetEndPoints(PointF location, SizeF offset)
		{
			SetEndPoints(location, location + offset);
		}

		private void FlagAsDirty()
		{
			_isDirty = true;
		}

		/// <summary>
		/// Formats the scale's base line segment.
		/// </summary>
		/// <param name="baseLine">A reference to the <see cref="LinePrimitive"/> that is the base line segment.</param>
		/// <param name="endpoint1">One endpoint where the base line segment should be drawn.</param>
		/// <param name="endpoint2">The other endpoint where the base line segment should be drawn.</param>
		private void FormatBaseLine(LinePrimitive baseLine, PointF endpoint1, PointF endpoint2)
		{
			baseLine.Point1 = endpoint1;
			baseLine.Point2 = endpoint2;
			baseLine.Color = Color.White;
		}

		/// <summary>
		/// Formats the scale's major ticks.
		/// </summary>
		/// <param name="tickLine">A reference to the <see cref="LinePrimitive"/> that is the tick.</param>
		/// <param name="point">The point along the base line segment where the tick begins.</param>
		/// <param name="unitNormal">A unit normal vector that is perpendicular to the base line segment.</param>
		private void FormatMajorTick(LinePrimitive tickLine, PointF point, SizeF unitNormal)
		{
			float length = _majorTickLength;
			if (_isMirrored)
				length = -length;
			tickLine.Point1 = point;
			tickLine.Point2 = new PointF(point.X + length*unitNormal.Width, point.Y + length*unitNormal.Height);
			tickLine.Color = Color.White;
		}

		/// <summary>
		/// Formats the scale's minor ticks.
		/// </summary>
		/// <param name="tickLine">A reference to the <see cref="LinePrimitive"/> that is the tick.</param>
		/// <param name="point">The point along the base line segment where the tick begins.</param>
		/// <param name="unitNormal">A unit normal vector that is perpendicular to the base line segment.</param>
		private void FormatMinorTick(LinePrimitive tickLine, PointF point, SizeF unitNormal)
		{
			float length = _minorTickLength;
			if (_isMirrored)
				length = -length;
			tickLine.Point1 = point;
			tickLine.Point2 = new PointF(point.X + length*unitNormal.Width, point.Y + length*unitNormal.Height);
			tickLine.Color = Color.White;
		}

		/// <summary>
		/// Recomputes and reformats the graphics that comprise the scale.
		/// </summary>
		private void UpdateScale()
		{
			// no point recomputing the scale if client code has made us invisible or we aren't on screen
			if (!Visible || (ParentPresentationImage != null && ParentPresentationImage.ClientRectangle.IsEmpty))
			{
				_isDirty = true;
				return;
			}

			CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				PointF pt0 = Point1;
				PointF pt1 = Point2;

				// draw base line
				FormatBaseLine(_baseLine, pt0, pt1);

				// compute normal to the base line
				var normalX = (double) pt0.Y - pt1.Y;
				var normalY = (double) pt1.X - pt0.X;
				var normalMagnitude = Math.Sqrt(normalX*normalX + normalY*normalY);
				var unitNormal = new SizeF((float) (Math.Abs(normalX)/normalMagnitude), (float) (Math.Abs(normalY)/normalMagnitude));

				// compute tick locations
				var ticks = ComputeTickMarks(pt0, pt1);
				if (ticks == null)
				{
					_baseLine.Visible = false;
					return;
				}

				// draw tick marks
				if (ticks.Length > 1) // must be at least 2 ticks for this to be useful
				{
					var n = 0;
					var tickLines = AllocateTickLines(ticks.Length);
					foreach (var tick in ticks)
					{
						if (tick.IsMajorTick)
							FormatMajorTick(tickLines[n++], tick.Location, unitNormal);
						else
							FormatMinorTick(tickLines[n++], tick.Location, unitNormal);
					}
					_baseLine.Visible = true;
				}
				else
				{
					AllocateTickLines(0);
					_baseLine.Visible = false;
				}

				_isDirty = false;
			}
			finally
			{
				ResetCoordinateSystem();
			}
		}

		/// <summary>
		/// Allocates sufficient tick <see cref="LinePrimitive"/>s, adding new objects and disposing extra objects as necessary.
		/// </summary>
		/// <param name="tickCount">The number of tick <see cref="LinePrimitive"/>s to allocate.</param>
		/// <returns></returns>
		private IList<LinePrimitive> AllocateTickLines(int tickCount)
		{
			LinePrimitive tick;
			while (_ticklines.Count < tickCount)
			{
				tick = new LinePrimitive {Name = _tickLineName};
				_ticklines.Add(tick);
				Graphics.Add(tick);
			}
			while (_ticklines.Count > tickCount)
			{
				tick = _ticklines[0];
				_ticklines.RemoveAt(0);
				Graphics.Remove(tick);
				tick.Dispose();
			}
			return _ticklines.AsReadOnly();
		}

		protected override void OnParentPresentationImageChanged(IPresentationImage oldParentPresentationImage, IPresentationImage newParentPresentationImage)
		{
			IImageSopProvider sopProvider = oldParentPresentationImage as IImageSopProvider;
			if (sopProvider != null)
				sopProvider.Frame.NormalizedPixelSpacing.Calibrated -= OnNormalizedPixelSpacingCalibrated;

			base.OnParentPresentationImageChanged(oldParentPresentationImage, newParentPresentationImage);

			sopProvider = newParentPresentationImage as IImageSopProvider;
			if (sopProvider != null)
				sopProvider.Frame.NormalizedPixelSpacing.Calibrated += OnNormalizedPixelSpacingCalibrated;
		}

		public override void OnDrawing()
		{
			if (_isDirty)
				UpdateScale();

			base.OnDrawing();
		}

		private void OnNormalizedPixelSpacingCalibrated(object sender, EventArgs e)
		{
			_isDirty = true;
			Draw();
		}

		/// <summary>
		/// Computes positions of major and minor tick marks along the specified line segment.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For performance reasons the arguments are <b>not</b> validated. In particular, the line segment specified by the two points
		/// must be valid and non-trivial - specifying the same point (or close to the same point) for both end points produces indeterminate
		/// results.
		/// </para>
		/// </remarks>
		/// <param name="linePoint1">One endpoint of the line segment along which to compute tick positions.</param>
		/// <param name="linePoint2">The other endpoint of the line segment along which to compute tick positions.</param>
		private Tick[] ComputeTickMarks(PointF linePoint1, PointF linePoint2)
		{
			// get the pixel dimensions of the image - if it's not calibrated, there's nothing to show!
			double pxW, pxH;
			bool isCalibrated = TryGetPixelDimensions(out pxW, out pxH);
			if (!isCalibrated)
				return null;

			// convert the given end points in screen coordinates to image coordinates
			// then figure out how long the base line is in mm, and thus how many ticks apart they are
			var src1 = SpatialTransform.ConvertToSource(linePoint1);
			var src2 = SpatialTransform.ConvertToSource(linePoint2);
			var mmLength = GetDistance(src1.X*pxW, src1.Y*pxH, src2.X*pxW, src2.Y*pxH);
			var numTicks = mmLength/_minorTick;

			// get the total distance between the end points in screen units
			var dstLength = Vector.Distance(linePoint1, linePoint2);

			// compute number of screen pixels between consecutive ticks
			// if the effective spacing is less than 3 pixels, the ticks will not render distinctly so we can stop here
			// additionally, if there isn't at least one full tick of spacing along the line, there's also nothing to show
			if (dstLength/numTicks < 3 || numTicks < 1)
				return null;

			// only distinguish between major/minor ticks if there are at least 6 ticks of spacing
			var useMajorTicks = numTicks >= 6;

			var dst1 = new Vector3D(linePoint1.X, linePoint1.Y, 0);
			var dst2 = new Vector3D(linePoint2.X, linePoint2.Y, 0);
			var dirVector = dst2 - dst1;

			var countTicks = (int) numTicks;
			var ticks = new Tick[countTicks];
			for (var n = 0; n < countTicks; ++n)
			{
				var dstLocation = dst1 + dirVector*(float) (n/numTicks);
				var point = new PointF(dstLocation.X, dstLocation.Y);
				ticks[n] = new Tick(point, useMajorTicks && n%_majorTickFrequency == 0);
			}

			return ticks;
		}

		private static double GetDistance(double x1, double y1, double x2, double y2)
		{
			var dX = x2 - x1;
			var dY = y2 - y1;
			return Math.Sqrt(dX*dX + dY*dY);
		}

		/// <summary>
		/// Gets the source image's pixel dimensions in millimetres.
		/// </summary>
		/// <param name="width">Output variable to receive the pixel millimetre width.</param>
		/// <param name="height">Output variable to receive the pixel millimetre height.</param>
		private bool TryGetPixelDimensions(out double width, out double height)
		{
			var imageSopProvider = ParentPresentationImage as IImageSopProvider;
			if (imageSopProvider != null)
			{
				var spacing = imageSopProvider.Frame.NormalizedPixelSpacing;
				if (!spacing.IsNull)
				{
					width = spacing.Column;
					height = spacing.Row;
					return true;
				}
			}
			width = 0;
			height = 0;
			return false;
		}

		private struct Tick
		{
			private readonly PointF _location;
			private readonly bool _isMajorTick;

			public Tick(PointF location, bool isMajorTick)
			{
				_location = location;
				_isMajorTick = isMajorTick;
			}

			public PointF Location
			{
				get { return _location; }
			}

			public bool IsMajorTick
			{
				get { return _isMajorTick; }
			}
		}
	}
}
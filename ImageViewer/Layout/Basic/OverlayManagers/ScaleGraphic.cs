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
		/// Recomputes and reformats the graphics that comprise the scale.
		/// </summary>
		private void UpdateScale()
		{
			// no point recomputing the scale if client code has made us invisible or we aren't on screen
			if (!Visible || (ParentPresentationImage != null && ParentPresentationImage.ClientRectangle.IsEmpty))
			{
				// force a recalculation when we become visible again
				_isDirty = true;
				return;
			}

			// determine locations of ticks
			var ticks = ComputeTickMarks();

			// if there are no ticks to show, exit now
			if (ticks == null || ticks.Length < 2)
			{
				AllocateTickLines(0);
				_baseLine.Visible = false;
				_isDirty = false;
				return;
			}

			// compute the tick vectors
			SizeF minorTickVector, majorTickVector;
			GetTickVectors(out majorTickVector, out minorTickVector);

			CoordinateSystem = CoordinateSystem.Source;
			try
			{
				// update base line
				_baseLine.Point1 = Point1;
				_baseLine.Point2 = Point2;
				_baseLine.Color = Color.White;
				_baseLine.Visible = true;

				// update ticks
				var n = 0;
				var tickLines = AllocateTickLines(ticks.Length);
				foreach (var tick in ticks)
				{
					var tickLine = tickLines[n++];
					tickLine.Point1 = tick.Location;
					tickLine.Point2 = tick.Location + (tick.IsMajorTick ? majorTickVector : minorTickVector);
					tickLine.Color = Color.White;
					tickLine.Visible = true;
				}

				_isDirty = false;
			}
			finally
			{
				ResetCoordinateSystem();
			}
		}

		/// <summary>
		/// Gets the vectors describing the tick marks in source coordinates.
		/// </summary>
		private void GetTickVectors(out SizeF majorTickVector, out SizeF minorTickVector)
		{
			// compute tick vectors in display coordinates (since they are effectively invariant graphics)
			// but then convert them back to source coordinates because the graphics all use source coordinates
			CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				var pt0 = Point1;
				var pt1 = Point2;

				// compute normal to the base line
				var normalX = (double) pt0.Y - pt1.Y;
				var normalY = (double) pt1.X - pt0.X;
				var normalMagnitude = Math.Sqrt(normalX*normalX + normalY*normalY);
				var normalUnit = new SizeF((float) (Math.Abs(normalX)/normalMagnitude), (float) (Math.Abs(normalY)/normalMagnitude));

				var majorTickLength = _isMirrored ? -_majorTickLength : _majorTickLength;
				var minorTickLength = _isMirrored ? -_minorTickLength : _minorTickLength;

				majorTickVector = SpatialTransform.ConvertToSource(new SizeF(majorTickLength*normalUnit.Width, majorTickLength*normalUnit.Height));
				minorTickVector = SpatialTransform.ConvertToSource(new SizeF(minorTickLength*normalUnit.Width, minorTickLength*normalUnit.Height));
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
			var ticklines = Graphics.OfType<LinePrimitive>().Where(g => g.Name == _tickLineName).ToList();
			if (ticklines.Count < tickCount)
			{
				var newTicks = Enumerable.Range(0, tickCount - ticklines.Count).Select(n => new LinePrimitive {Name = _tickLineName}).ToList();
				ticklines.AddRange(newTicks);
				Graphics.AddRange(newTicks);
			}
			else if (ticklines.Count > tickCount)
			{
				foreach (var oldTick in ticklines.Skip(tickCount))
				{
					Graphics.Remove(oldTick);
					oldTick.Dispose();
				}
				ticklines = ticklines.Take(tickCount).ToList();
			}
			return ticklines;
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
			FlagAsDirty();
			Draw();
		}

		/// <summary>
		/// Computes positions of tick marks along the base line.
		/// </summary>
		private Tick[] ComputeTickMarks()
		{
			// compute tick locations in source coordinates because all the underlying graphics use source coordinates, so we save on a ton of ridiculous back-and-forth conversions
			CoordinateSystem = CoordinateSystem.Source;
			try
			{
				var srcPoint1 = Point1;
				var srcPoint2 = Point2;

				// get the pixel dimensions of the image - if it's not calibrated, there's nothing to show!
				double pxW, pxH;
				bool isCalibrated = TryGetPixelDimensions(out pxW, out pxH);
				if (!isCalibrated)
					return null;

				// figure out how long the base line is in mm, and thus how many ticks apart they are
				var mmLength = GetDistance(srcPoint1.X*pxW, srcPoint1.Y*pxH, srcPoint2.X*pxW, srcPoint2.Y*pxH);
				var countTickUnits = mmLength/_minorTick;

				// round down and add 1 to get the number of tick marks displayable on the given base line
				var countTickMarks = 1 + (int) countTickUnits;

				// get the total distance between the end points in screen units
				var dstLength = Vector.Distance(SpatialTransform.ConvertToDestination(srcPoint1),
				                                SpatialTransform.ConvertToDestination(srcPoint2));

				// compute number of screen pixels between consecutive ticks
				// if the effective spacing is less than 3 pixels, the ticks will not render distinctly so we can stop here
				// additionally, we need at least two tick visible tick marks in order to have anything to show
				if (dstLength/countTickUnits < 3 || countTickMarks < 2)
					return null;

				// only distinguish between major/minor ticks if there are at least MAJOR_TICK_FREQ+1 ticks of spacing (i.e. at least two major ticks)
				var useMajorTicks = countTickMarks >= _majorTickFrequency + 1;

				var src1 = new Vector3D(srcPoint1.X, srcPoint1.Y, 0);
				var src2 = new Vector3D(srcPoint2.X, srcPoint2.Y, 0);
				var srcV = src2 - src1; // vector from point2 to point1

				var ticks = new Tick[countTickMarks];
				for (var n = 0; n < countTickMarks; ++n)
				{
					// compute location for each tick from point1 + the vector scaled appropriately by index and the number of tick units along the line
					// don't just compute one offset and keep adding it, as the floating point errors would then be cumulative
					var location = src1 + srcV*(float) (n/countTickUnits);
					ticks[n] = new Tick(location.X, location.Y, useMajorTicks && n%_majorTickFrequency == 0);
				}
				return ticks;
			}
			finally
			{
				ResetCoordinateSystem();
			}
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

			public Tick(float x, float y, bool isMajorTick)
			{
				_location = new PointF(x, y);
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
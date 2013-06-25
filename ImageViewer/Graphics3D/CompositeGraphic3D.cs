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
using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics3D
{
	/// <summary>
	/// A <see cref="Graphic3D"/> that can group other <see cref="Graphic3D"/> objects.
	/// </summary>
	[Cloneable(true)]
	public class CompositeGraphic3D : Graphic3D
	{
		private GraphicCollection3D _graphics;

		/// <summary>
		/// Gets a collection of this <see cref="CompositeGraphic3D"/>'s child graphics.
		/// </summary>
		public GraphicCollection3D Graphics
		{
			get
			{
				if (_graphics == null)
				{
					_graphics = new GraphicCollection3D();
					_graphics.ItemAdded += OnGraphicAdded;
					_graphics.ItemRemoved += OnGraphicRemoved;
				}

				return _graphics;
			}
		}

		//leave this override because the comments are important.

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="CompositeGraphic3D"/> is visible.
		/// </summary>
		/// <remarks>
		/// Setting the <see cref="Visible"/> property will <b>not</b> recursively set the 
		/// <see cref="Visible"/> property for child objects in the subtree for reasons
		/// of preservation of state.  For example, if a <see cref="CompositeGraphic3D"/> at
		/// the very top of the scene graph had many child graphics whose visibility were
		/// dependent on, say, the position of the mouse, when the <see cref="CompositeGraphic3D"/>'s
		/// visibility changed, the visibility of the child objects would no longer be correct.
		/// Also, when a parent graphic's <see cref="Visible"/> property is false, it's children
		/// are not rendered regardless of the value of their <see cref="Visible"/> property.
		/// </remarks>
		public override bool Visible
		{
			get { return base.Visible; }
			set { base.Visible = value; }
		}

		/// <summary>
		/// Gets the tightest bounding box that encloses all the bounding boxes of the child graphics in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <para><see cref="IGraphic3D.CoordinateSystem"/> determines whether this property is in source or destination coordinates.</para>
		/// </remarks>
		public override Rectangle3D BoundingBox
		{
			get
			{
				var rectangles = Graphics.Select(graphic => graphic.BoundingBox).Where(rect => !rect.Size.IsNull || !rect.Location.IsNull).ToList();
				return rectangles.Count == 0 ? Rectangle3D.Empty : rectangles.Aggregate(new Rectangle3D(), Rectangle3D.Union);
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// Setting the <see cref="CoordinateSystem"/> property will recursively set the 
		/// <see cref="CoordinateSystem"/> property for <i>all</i> <see cref="Graphic3D"/> 
		/// objects in the subtree.
		/// </remarks>
		public override CoordinateSystem CoordinateSystem
		{
			get { return base.CoordinateSystem; }
			set
			{
				base.CoordinateSystem = value;

				foreach (var graphic in Graphics)
					graphic.CoordinateSystem = value;
			}
		}

		/// <summary>
		/// Resets the <see cref="CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="ResetCoordinateSystem"/> will reset the <see cref="CoordinateSystem"/>
		/// to what it was before the <see cref="CoordinateSystem"/> was last set.
		/// </para>
		/// <para>
		/// Calling <see cref="ResetCoordinateSystem"/> will recursively call
		/// <see cref="ResetCoordinateSystem"/> on <i>all</i> <see cref="Graphic3D"/> 
		/// objects in the subtree.
		/// </para>
		/// </remarks>
		public override void ResetCoordinateSystem()
		{
			base.ResetCoordinateSystem();

			foreach (var graphic in Graphics)
				graphic.ResetCoordinateSystem();
		}

		internal override void SetImageViewer(IImageViewer imageViewer)
		{
			base.SetImageViewer(imageViewer);

			foreach (Graphic3D graphic in Graphics)
				graphic.SetImageViewer(imageViewer);
		}

		internal override void SetParentPresentationImage(IPresentationImage parentPresentationImage)
		{
			base.SetParentPresentationImage(parentPresentationImage);

			foreach (Graphic3D graphic in Graphics)
				graphic.SetParentPresentationImage(parentPresentationImage);
		}

		/// <summary>
		/// Performs a hit test on the <see cref="CompositeGraphic3D"/> at given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" any <see cref="Graphic3D"/>
		/// in the subtree, <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// Calling <see cref="HitTest"/> will recursively call <see cref="HitTest"/> on
		/// <see cref="Graphic3D"/> objects in the subtree.
		/// </remarks>
		public override bool HitTest(Vector3D point)
		{
			return Graphics.Any(graphic => graphic.HitTest(point));
		}

		/// <summary>
		/// Gets the point on the <see cref="CompositeGraphic3D"/> closest to the specified point.
		/// </summary>
		/// <param name="point">A point in either source or destination coordinates.</param>
		/// <returns>The point on the graphic closest to the given <paramref name="point"/>.</returns>
		/// <remarks>
		/// <para>
		/// Depending on the value of <see cref="Graphic3D.CoordinateSystem"/>,
		/// the computation will be carried out in either source
		/// or destination coordinates.</para>
		/// <para>Calling <see cref="GetClosestPoint"/> will recursively call <see cref="GetClosestPoint"/>
		/// on <see cref="Graphic3D"/> objects in the subtree and return the closest result.</para>
		/// </remarks>
		public override Vector3D GetClosestPoint(Vector3D point)
		{
			var result = Vector3D.Null;
			double min = double.MaxValue;
			foreach (var graphic in Graphics)
			{
				var pt = graphic.GetClosestPoint(point);
				var d = (point - pt).Magnitude;
				if (min > d)
				{
					min = d;
					result = pt;
				}
			}
			return result;
		}

		/// <summary>
		/// Moves the <see cref="CompositeGraphic3D"/> by a specified delta.
		/// </summary>
		/// <param name="delta">The distance to move.</param>
		/// <remarks>
		/// Depending on the value of <see cref="CoordinateSystem"/>,
		/// <paramref name="delta"/> will be interpreted in either source
		/// or destination coordinates.
		/// </remarks>
		public override void Move(Vector3D delta)
		{
			foreach (var graphic in Graphics)
				graphic.Move(delta);
		}

		/// <summary>
		/// Releases all resources used by this <see cref="CompositeGraphic3D"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Graphics.ItemAdded -= OnGraphicAdded;
				Graphics.ItemRemoved -= OnGraphicRemoved;

				foreach (var graphic in Graphics)
					graphic.Dispose();
			}
		}

		private void OnGraphicAdded(object sender, ListEventArgs<IGraphic3D> e)
		{
			var graphic = (Graphic3D) e.Item;

			graphic.SetParentGraphic(this);
			graphic.SetParentPresentationImage(ParentPresentationImage);
			graphic.SetImageViewer(ImageViewer);
			graphic.CoordinateSystem = CoordinateSystem;
			graphic.VisualStateChanged += OnGraphicVisualStateChanged;
		}

		private void OnGraphicRemoved(object sender, ListEventArgs<IGraphic3D> e)
		{
			var graphic = (Graphic3D) e.Item;
			graphic.VisualStateChanged -= OnGraphicVisualStateChanged;

			var selectableGraphic = graphic as ISelectableGraphic;
			if (selectableGraphic != null) (selectableGraphic).Selected = false;

			var focussableGraphic = graphic as IFocussableGraphic;
			if (focussableGraphic != null) (focussableGraphic).Focussed = false;

			graphic.SetParentGraphic(null);
			graphic.SetParentPresentationImage(null);
			graphic.SetImageViewer(null);
		}

		private void OnGraphicVisualStateChanged(object sender, VisualStateChanged3DEventArgs e)
		{
			NotifyVisualStateChanged(e);
		}

		[CloneInitialize]
		private void Initialize(CompositeGraphic3D source, ICloningContext context)
		{
			foreach (var graphic in source.Graphics)
			{
				var clone = graphic.Clone();
				if (clone != null)
					Graphics.Add(clone);
			}
		}

		/// <summary>
		/// Enumerates the immediate child graphics of this <see cref="CompositeGraphic3D"/>.
		/// </summary>
		/// <param name="reverse">A value specifying that the enumeration should be carried out in reverse order.</param>
		/// <returns>An enumeration of child graphics.</returns>
		public IEnumerable<IGraphic3D> EnumerateChildGraphics(bool reverse)
		{
			return reverse ? Graphics.Reverse() : Graphics;
		}
	}
}
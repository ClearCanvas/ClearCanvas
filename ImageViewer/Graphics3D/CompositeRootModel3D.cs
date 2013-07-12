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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics3D
{
	/// <summary>
	/// A composite model whose <see cref="SpatialTransform"/>
	/// is tailored for the root model of a 3D scene.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Use <see cref="CompositeRootModel3D"/> when you want to anchor graphics to
	/// the scene root.  Create an instance of <see cref="CompositeRootModel3D"/> using
	/// parameters (dimensions, spacing, etc.) from the scene root
	/// which you want to anchor other graphics to.
	/// </para>
	/// </remarks>
	[Cloneable]
	internal class CompositeRootModel3D : CompositeGraphic, IGraphic3D
	{
		#region Private fields

		[CloneIgnore]
		private readonly Guid _uid = Guid.NewGuid();

		[CloneIgnore]
		private readonly GraphicCollection3D _graphicCollection3D;

		[CloneCopyReference]
		private readonly Vector3D _dimensions;

		[CloneCopyReference]
		private readonly Vector3D _spacing;

		[CloneCopyReference]
		private readonly Vector3D _aspectRatio;

		private readonly Size _sceneSize;

		private event VisualStateChanged3DEventHandler _visualStateChanged3D;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="CompositeRootModel3D"/> with
		/// the specified dimensions.
		/// </summary>
		/// <param name="sceneWidth"></param>
		/// <param name="sceneHeight"></param>
		/// <param name="dimensionX"></param>
		/// <param name="dimensionY"></param>
		/// <param name="dimensionZ"></param>
		public CompositeRootModel3D(
			int sceneWidth,
			int sceneHeight,
			float dimensionX,
			float dimensionY,
			float dimensionZ)
			: this(sceneWidth, sceneHeight, dimensionX, dimensionY, dimensionZ, 0, 0, 0) {}

		/// <summary>
		/// Initializes a new instance of <see cref="CompositeRootModel3D"/> with
		/// the specified dimensions and spacing.
		/// </summary>
		/// <param name="sceneWidth"></param>
		/// <param name="sceneHeight"></param>
		/// <param name="dimensionX"></param>
		/// <param name="dimensionY"></param>
		/// <param name="dimensionZ"></param>
		/// <param name="spacingX"></param>
		/// <param name="spacingY"></param>
		/// <param name="spacingZ"></param>
		public CompositeRootModel3D(
			int sceneWidth,
			int sceneHeight,
			float dimensionX,
			float dimensionY,
			float dimensionZ,
			float spacingX,
			float spacingY,
			float spacingZ)
			: this(sceneWidth, sceneHeight, dimensionX, dimensionY, dimensionZ, spacingX, spacingY, spacingZ, 0, 0, 0) {}

		/// <summary>
		/// Initializes a new instance of <see cref="CompositeRootModel3D"/> with
		/// the specified dimensions, spacing and aspect ratio.
		/// </summary>
		/// <param name="sceneWidth"></param>
		/// <param name="sceneHeight"></param>
		/// <param name="dimensionX"></param>
		/// <param name="dimensionY"></param>
		/// <param name="dimensionZ"></param>
		/// <param name="spacingX"></param>
		/// <param name="spacingY"></param>
		/// <param name="spacingZ"></param>
		/// <param name="aspectRatioX"></param>
		/// <param name="aspectRatioY"></param>
		/// <param name="aspectRatioZ"></param>
		public CompositeRootModel3D(
			int sceneWidth,
			int sceneHeight,
			float dimensionX,
			float dimensionY,
			float dimensionZ,
			float spacingX,
			float spacingY,
			float spacingZ,
			float aspectRatioX,
			float aspectRatioY,
			float aspectRatioZ)
		{
			_sceneSize = new Size(sceneWidth, sceneHeight);
			_dimensions = new Vector3D(dimensionX, dimensionY, dimensionZ);
			_spacing = new Vector3D(spacingX, spacingY, spacingZ);
			_aspectRatio = new Vector3D(aspectRatioX, aspectRatioY, aspectRatioZ);

			_graphicCollection3D = new GraphicCollection3D();
			_graphicCollection3D.ItemAdded += OnGraphicAdded;
			_graphicCollection3D.ItemRemoved += OnGraphicRemoved;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected CompositeRootModel3D(CompositeRootModel3D source, ICloningContext context)
		{
			context.CloneFields(source, this);

			_graphicCollection3D = new GraphicCollection3D();
			_graphicCollection3D.ItemAdded += OnGraphicAdded;
			_graphicCollection3D.ItemRemoved += OnGraphicRemoved;
			_graphicCollection3D.AddRange(source._graphicCollection3D.Select(g => g.Clone()).Where(g => g != null));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_graphicCollection3D.ItemAdded -= OnGraphicAdded;
				_graphicCollection3D.ItemRemoved -= OnGraphicRemoved;
				foreach (var graphic in _graphicCollection3D)
					graphic.Dispose();
			}

			base.Dispose(disposing);
		}

		public Guid Uid
		{
			get { return _uid; }
		}

		/// <summary>
		/// Gets the dimensions of the scene represented by this graphic.
		/// </summary>
		public Size SceneSize
		{
			get { return _sceneSize; }
		}

		/// <summary>
		/// Gets or sets the origin of the model.
		/// </summary>
		public Vector3D Origin
		{
			get { return ((RootModelSpatialTransform) SpatialTransform).ModelOrigin; }
			set { ((RootModelSpatialTransform) SpatialTransform).ModelOrigin = value; }
		}

		public GraphicCollection3D Graphics3D
		{
			get { return _graphicCollection3D; }
		}

		/// <summary>
		/// Gets the tightest bounding box that encloses all the bounding boxes of the child graphics in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <para><see cref="IGraphic3D.CoordinateSystem"/> determines whether this property is in source or destination coordinates.</para>
		/// </remarks>
		public virtual Rectangle3D BoundingBox3D
		{
			get
			{
				var rectangles = Graphics3D.Select(graphic => graphic.BoundingBox).Where(rect => !rect.Size.IsNull || !rect.Location.IsNull).ToList();
				return rectangles.Count == 0 ? Rectangle3D.Empty : rectangles.Aggregate(new Rectangle3D(), Rectangle3D.Union);
			}
		}

		public override CoordinateSystem CoordinateSystem
		{
			get { return base.CoordinateSystem; }
			set
			{
				base.CoordinateSystem = value;

				foreach (var graphic in _graphicCollection3D)
					graphic.CoordinateSystem = value;
			}
		}

		/// <summary>
		/// Occurs when a property is changed on a graphic, resulting in a change in the graphic's visual state.
		/// </summary>
		public event VisualStateChanged3DEventHandler VisualStateChanged3D
		{
			add { _visualStateChanged3D += value; }
			remove { _visualStateChanged3D -= value; }
		}

		/// <summary>
		/// Gets this <see cref="Graphic"/> object's <see cref="SpatialTransform"/>.
		/// </summary>
		public SpatialTransform3D SpatialTransform3D
		{
			get { return ((RootModelSpatialTransform) SpatialTransform).SpatialTransform3D; }
		}

		/// <summary>
		/// This member overrides <see cref="Graphic.CreateSpatialTransform"/>.
		/// </summary>
		/// <returns></returns>
		protected override SpatialTransform CreateSpatialTransform()
		{
			return new RootModelSpatialTransform(this, _sceneSize, _dimensions, _spacing, _aspectRatio);
		}

		public override void ResetCoordinateSystem()
		{
			base.ResetCoordinateSystem();

			foreach (var graphic in _graphicCollection3D)
				graphic.ResetCoordinateSystem();
		}

		internal override void SetImageViewer(IImageViewer imageViewer)
		{
			base.SetImageViewer(imageViewer);

			foreach (Graphic3D graphic in _graphicCollection3D)
				graphic.SetImageViewer(imageViewer);
		}

		internal override void SetParentPresentationImage(IPresentationImage parentPresentationImage)
		{
			base.SetParentPresentationImage(parentPresentationImage);

			foreach (Graphic3D graphic in _graphicCollection3D)
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
		/// Calling <see cref="HitTest3D"/> will recursively call <see cref="HitTest3D"/> on
		/// <see cref="Graphic3D"/> objects in the subtree.
		/// </remarks>
		public virtual bool HitTest3D(Vector3D point)
		{
			return Graphics3D.Any(graphic => graphic.HitTest(point));
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
		/// <para>Calling <see cref="GetClosestPoint3D"/> will recursively call <see cref="GetClosestPoint3D"/>
		/// on <see cref="Graphic3D"/> objects in the subtree and return the closest result.</para>
		/// </remarks>
		public virtual Vector3D GetClosestPoint3D(Vector3D point)
		{
			var result = Vector3D.Null;
			double min = double.MaxValue;
			foreach (var graphic in Graphics3D)
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
		public virtual void Move3D(Vector3D delta)
		{
			foreach (var graphic in Graphics3D)
				graphic.Move(delta);
		}

		private void OnGraphicAdded(object sender, ListEventArgs<IGraphic3D> e)
		{
			var graphic = (Graphic3D) e.Item;

			graphic.SetParentGraphic(this);
			graphic.SetParentPresentationImage(ParentPresentationImage);
			graphic.SetImageViewer(ImageViewer);
			graphic.CoordinateSystem = CoordinateSystem;
			graphic.VisualStateChanged += OnGraphicVisualStateChanged3D;
		}

		private void OnGraphicRemoved(object sender, ListEventArgs<IGraphic3D> e)
		{
			var graphic = (Graphic3D) e.Item;
			graphic.VisualStateChanged -= OnGraphicVisualStateChanged3D;

			var selectableGraphic = graphic as ISelectableGraphic;
			if (selectableGraphic != null) (selectableGraphic).Selected = false;

			var focussableGraphic = graphic as IFocussableGraphic;
			if (focussableGraphic != null) (focussableGraphic).Focussed = false;

			graphic.SetParentGraphic(null);
			graphic.SetParentPresentationImage(null);
			graphic.SetImageViewer(null);
		}

		private void OnGraphicVisualStateChanged3D(object sender, VisualStateChanged3DEventArgs e)
		{
			if (e.Graphic == this) OnVisualStateChanged(e.PropertyName);
			EventsHelper.Fire(_visualStateChanged3D, this, e);
		}

		/// <summary>
		/// Enumerates the immediate child graphics of this <see cref="CompositeGraphic3D"/>.
		/// </summary>
		/// <param name="reverse">A value specifying that the enumeration should be carried out in reverse order.</param>
		/// <returns>An enumeration of child graphics.</returns>
		public IEnumerable<IGraphic3D> EnumerateChildGraphics3D(bool reverse)
		{
			return reverse ? Graphics3D.Reverse() : Graphics3D;
		}

		#region IGraphic3D Explicit Implementations

		IGraphic3D IGraphic3D.ParentGraphic
		{
			get { return null; }
		}

		Rectangle3D IGraphic3D.BoundingBox
		{
			get { return BoundingBox3D; }
		}

		SpatialTransform3D IGraphic3D.SpatialTransform
		{
			get { return SpatialTransform3D; }
		}

		event VisualStateChanged3DEventHandler IGraphic3D.VisualStateChanged
		{
			add { VisualStateChanged3D += value; }
			remove { VisualStateChanged3D -= value; }
		}

		bool IGraphic3D.HitTest(Vector3D point)
		{
			return HitTest3D(point);
		}

		Vector3D IGraphic3D.GetClosestPoint(Vector3D point)
		{
			return GetClosestPoint3D(point);
		}

		void IGraphic3D.Move(Vector3D delta)
		{
			Move3D(delta);
		}

		IGraphic3D IGraphic3D.Clone()
		{
			return (IGraphic3D) Clone();
		}

		#endregion
	}
}
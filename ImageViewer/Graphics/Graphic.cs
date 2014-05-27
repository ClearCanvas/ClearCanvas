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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Coordinate system enumeration.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Consider a 512x512 image that has anchored to it a line
	/// defined by points P1(0,0) and P2(10,10), where those points are
	/// in the coordinate system of the image.  In that case, the line is said
	/// to be in <i>source</i> coordinates.  Now consider if the image 
	/// is zoomed by 2x where the center of expansion is (0,0).
	/// In that case, the line's source coordinates are still the same, but 
	/// its <i>destination</i> or Tile coordinates are now P1'(0,0), P2'(20,20).
	/// </para>
	/// <para>
	/// In general, the source coordinates of a particular <see cref="Graphic"/>
	/// are coordinates in the coordinate system of its immediate parent.
	/// Destination coordinates are coordinates in the coordinate system
	/// of the root of the scene graph, i.e., the Tile.  Put another way,
	/// destination coordinates = T(source coordinates), where T represents
	/// the a graphic's cumulative transformation.
	/// </para>
	/// </remarks>
	public enum CoordinateSystem
	{
		/// <summary>
		/// Represent a <see cref="Graphic"/> object's coordinates in
		/// its immediate parent's, or <i>source</i> coordinate system.
		/// </summary>
		Source = 0,

		/// <summary>
		/// Represent a <see cref="Graphic"/> object's coordinates in
		/// the coordinate system of the root of the scene graph, that is,
		/// in Tile or <i>destination</i> coordinates.
		/// </summary>
		Destination = 1
	}

	/// <summary>
	/// An graphical object that can be rendered.
	/// </summary>
	[Cloneable(true)]
	public abstract class Graphic : IGraphic
	{
		#region Private fields

		[CloneIgnore]
		private IGraphic _parentGraphic;

		[CloneIgnore]
		private IImageViewer _parentImageViewer;

		[CloneIgnore]
		private IPresentationImage _parentPresentationImage;

		private SpatialTransform _spatialTransform;

		private string _name;
		private bool _visible = true;
		private Stack<CoordinateSystem> _coordinateSystemStack;
		private event EventHandler _drawing;
		private event VisualStateChangedEventHandler _visualStateChanged;

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected Graphic() {}

		private Stack<CoordinateSystem> CoordinateSystemStack
		{
			get
			{
				if (_coordinateSystemStack == null)
				{
					_coordinateSystemStack = new Stack<CoordinateSystem>();
					_coordinateSystemStack.Push(CoordinateSystem.Source);
				}

				return _coordinateSystemStack;
			}
		}

		/// <summary>
		/// Gets this <see cref="Graphic"/> object's parent <see cref="Graphic"/>.
		/// </summary>
		public IGraphic ParentGraphic
		{
			get { return _parentGraphic; }
		}

		internal void SetParentGraphic(IGraphic parentGraphic)
		{
			_parentGraphic = parentGraphic;
			SpatialTransform.ForceRecalculation();
		}

		/// <summary>
		/// Gets this <see cref="Graphic"/> object's associated 
		/// <see cref="IPresentationImage"/>.
		/// </summary>
		/// <value>The associated <see cref="IPresentationImage"/> or <b>null</b>
		/// if the <see cref="Graphic"/> is not yet part of the scene graph.</value>
		public virtual IPresentationImage ParentPresentationImage
		{
			get { return _parentPresentationImage; }
		}

		/// <summary>
		/// Called when the value of the <see cref="ParentPresentationImage"/> property changes.
		/// </summary>
		/// <param name="oldParentPresentationImage">A reference to the old parent presentation image.</param>
		/// <param name="newParentPresentationImage">A reference to the new parent presentation image.</param>
		protected virtual void OnParentPresentationImageChanged(IPresentationImage oldParentPresentationImage, IPresentationImage newParentPresentationImage) {}

		internal virtual void SetParentPresentationImage(IPresentationImage parentPresentationImage)
		{
			if (_parentPresentationImage != parentPresentationImage)
			{
				IPresentationImage oldParent = _parentPresentationImage;
				_parentPresentationImage = parentPresentationImage;
				this.OnParentPresentationImageChanged(oldParent, parentPresentationImage);
			}
		}

		/// <summary>
		/// Gets this <see cref="Graphic"/> object's associated 
		/// <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b>
		/// if the <see cref="Graphic"/> is not yet associated with
		/// an <see cref="IImageViewer"/>.</value>
		public virtual IImageViewer ImageViewer
		{
			get { return _parentImageViewer; }
		}

		internal virtual void SetImageViewer(IImageViewer imageViewer)
		{
			_parentImageViewer = imageViewer;
		}

		/// <summary>
		/// Gets or sets the name of this <see cref="Graphic"/>.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					OnNameChanged();
				}
			}
		}

		/// <summary>
		/// Called when the value of the <see cref="Name"/> property changes.
		/// </summary>
		protected virtual void OnNameChanged() {}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Graphic"/> is visible.
		/// </summary>
		public virtual bool Visible
		{
			get { return _visible; }
			set { _visible = value; }
		}

		/// <summary>
		/// Gets the tightest bounding box that encloses the graphic in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public virtual RectangleF BoundingBox
		{
			get { return RectangleF.Empty; }
		}

		/// <summary>
		/// Gets or sets the <see cref="CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// After the <see cref="Graphic.CoordinateSystem"/> has been set and the
		/// desired operations have been performed on the <see cref="Graphic"/>,
		/// it is proper practice to call <see cref="ResetCoordinateSystem"/>
		/// to restore the previous coordinate system.
		/// </remarks>
		public virtual CoordinateSystem CoordinateSystem
		{
			get { return CoordinateSystemStack.Peek(); }
			set { CoordinateSystemStack.Push(value); }
		}

		/// <summary>
		/// Gets this <see cref="Graphic"/> object's <see cref="SpatialTransform"/>.
		/// </summary>
		public virtual SpatialTransform SpatialTransform
		{
			get
			{
				if (_spatialTransform == null)
					_spatialTransform = CreateSpatialTransform();

				return _spatialTransform;
			}
			internal set { _spatialTransform = value; }
		}

		/// <summary>
		/// Performs a hit test on the <see cref="Graphic"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="Graphic"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// It is up to the <see cref="Graphic"/> to define what a "hit" is.
		/// </remarks>
		public abstract bool HitTest(Point point);

		/// <summary>
		/// Gets the point on the <see cref="Graphic"/> closest to the specified point.
		/// </summary>
		/// <param name="point">A point in either source or destination coordinates.</param>
		/// <returns>The point on the graphic closest to the given <paramref name="point"/>.</returns>
		/// <remarks>
		/// <para>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// the computation will be carried out in either source
		/// or destination coordinates.</para>
		/// </remarks>
		public virtual PointF GetClosestPoint(PointF point)
		{
			return RectanglePrimitive.GetClosestPoint(point, this.BoundingBox);
		}

		/// <summary>
		/// Moves the <see cref="Graphic"/> by a specified delta.
		/// </summary>
		/// <param name="delta">The distance to move.</param>
		/// <remarks>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// <paramref name="delta"/> will be interpreted in either source
		/// or destination coordinates.
		/// </remarks>
		public abstract void Move(SizeF delta);

		/// <summary>
		/// Resets the <see cref="CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="ResetCoordinateSystem"/> will reset the <see cref="CoordinateSystem"/>
		/// to what it was before the <see cref="Graphic.CoordinateSystem"/> property
		/// was last set.
		/// </para>
		/// </remarks>
		public virtual void ResetCoordinateSystem()
		{
			if (CoordinateSystemStack.Count == 1)
				return;

			CoordinateSystemStack.Pop();
		}

		/// <summary>
		/// Fires just before the <see cref="Graphic"/> is drawn/rendered.
		/// </summary>
		public event EventHandler Drawing
		{
			add { _drawing += value; }
			remove { _drawing -= value; }
		}

		/// <summary>
		/// Draws the <see cref="Graphic"/>.
		/// </summary>
		public virtual void Draw()
		{
			if (this.ParentPresentationImage != null)
				this.ParentPresentationImage.Draw();
		}

		/// <summary>
		/// Fires the <see cref="Drawing"/> event.  Should be called by an <see cref="IRenderer"/>
		/// for each object just before it is drawn/rendered, hence the reason it is public.
		/// </summary>
		public virtual void OnDrawing()
		{
			EventsHelper.Fire(_drawing, this, EventArgs.Empty);
		}

		/// <summary>
		/// Creates a deep copy of the graphic.
		/// </summary>
		/// <remarks>
		/// Graphic objects that are not cloneable may return null.
		/// </remarks>
		public IGraphic Clone()
		{
			IGraphic clone = CloneBuilder.Clone(this) as IGraphic;
			if (clone != null && ImageViewer != null)
				ImageViewer.EventBroker.OnCloneCreated(new CloneCreatedEventArgs(this, clone));

			return clone;
		}

		/// <summary>
		/// Gets an object describing the region of interest on the <see cref="Graphic.ParentPresentationImage"/> selected by this <see cref="Graphic"/>.
		/// </summary>
		/// <remarks>
		/// Graphic objects that do not describe a region of interest may return null.
		/// </remarks>
		/// <returns>A <see cref="Roi"/> describing this region of interest, or null if the graphic does not describe a region of interest.</returns>
		public virtual Roi GetRoi()
		{
			return null;
		}

		/// <summary>
		/// Creates the <see cref="SpatialTransform"/> for this <see cref="Graphic"/>.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// Override this if the default <see cref="SpatialTransform"/> created
		/// is not appropriate.
		/// </remarks>
		protected virtual SpatialTransform CreateSpatialTransform()
		{
			return new SpatialTransform(this);
		}

		#region Disposal

		#region IDisposable Members

		/// <summary>
		/// Releases all resources used by this <see cref="Graphic"/>.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing) {}
		}

		#endregion

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			if (_spatialTransform != null)
				_spatialTransform.OwnerGraphic = this;
		}

		/// <summary>
		/// Occurs when a property is changed on a graphic, resulting in a change in the graphic's visual state.
		/// </summary>
		public event VisualStateChangedEventHandler VisualStateChanged
		{
			add { _visualStateChanged += value; }
			remove { _visualStateChanged -= value; }
		}

		/// <summary>
		/// Fires the <see cref="VisualStateChanged"/> event.
		/// </summary>
		/// <param name="propertyName">The name of the property whose value changed.</param>
		/// <param name="propertyKind">The kind of the property whose value changed.</param>
		protected void NotifyVisualStateChanged(string propertyName, VisualStatePropertyKind propertyKind = VisualStatePropertyKind.Unspecified)
		{
			OnVisualStateChanged(propertyName);
			FireVisualStateChanged(new VisualStateChangedEventArgs(this, propertyName, propertyKind));
		}

		/// <summary>
		/// Forwards the <see cref="VisualStateChanged"/> event from a child graphic to listeners of a parent graphic.
		/// </summary>
		/// <param name="e">Data for the <see cref="VisualStateChanged"/> event.</param>
		private void ForwardVisualStateChanged(VisualStateChangedEventArgs e)
		{
			if (ReferenceEquals(e.Graphic, this))
				OnVisualStateChanged(e.PropertyName);
			FireVisualStateChanged(e);
		}

		private void FireVisualStateChanged(VisualStateChangedEventArgs e)
		{
			if (_visualStateChanged != null)
			{
	            //NOTE: don't use EventsHelper because it will use DynamicInvoke.
	            try
	            {
	                _visualStateChanged(this, e);
	            }
	            catch (Exception ex)
	            {
	                Platform.Log(LogLevel.Error, ex);
	                throw;
	            }
			}

			var parentGraphic = ParentGraphic as Graphic;
			if (parentGraphic != null)
				parentGraphic.ForwardVisualStateChanged(e);
        }

		/// <summary>
		/// Called when the visual state changes on the current graphic, before notification of other handlers observing the <see cref="VisualStateChanged"/> event.
		/// </summary>
		/// <param name="propertyName">The name of the property whose value changed.</param>
		protected virtual void OnVisualStateChanged(string propertyName) {}
	}
}
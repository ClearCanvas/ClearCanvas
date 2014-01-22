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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Graphics3D
{
	/// <summary>
	/// A 3D graphical object that can be rendered.
	/// </summary>
	[Cloneable(true)]
	public abstract class Graphic3D : IGraphic3D
	{
		#region Private fields

		[CloneIgnore]
		private readonly Guid _uid = Guid.NewGuid();

		[CloneIgnore]
		private IGraphic3D _parentGraphic;

		[CloneIgnore]
		private IImageViewer _parentImageViewer;

		[CloneIgnore]
		private IPresentationImage _parentPresentationImage;

		private SpatialTransform3D _spatialTransform;

		private string _name;
		private bool _visible = true;
		private Stack<CoordinateSystem> _coordinateSystemStack;
		private event EventHandler _drawing;
		private event VisualStateChanged3DEventHandler _visualStateChanged;

		#endregion

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

		public IGraphic3D ParentGraphic
		{
			get { return _parentGraphic; }
		}

		internal void SetParentGraphic(IGraphic3D parentGraphic)
		{
			_parentGraphic = parentGraphic;
			SpatialTransform.ForceRecalculation();
		}

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
				var oldParent = _parentPresentationImage;
				_parentPresentationImage = parentPresentationImage;
				OnParentPresentationImageChanged(oldParent, parentPresentationImage);
			}
		}

		/// <summary>
		/// Gets this <see cref="Graphic3D"/> object's associated 
		/// <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b>
		/// if the <see cref="Graphic3D"/> is not yet associated with
		/// an <see cref="IImageViewer"/>.</value>
		public virtual IImageViewer ImageViewer
		{
			get { return _parentImageViewer; }
		}

		internal virtual void SetImageViewer(IImageViewer imageViewer)
		{
			_parentImageViewer = imageViewer;
		}

		public Guid Uid
		{
			get { return _uid; }
		}

		/// <summary>
		/// Gets or sets the name of this <see cref="Graphic3D"/>.
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
		/// Gets or sets a value indicating whether this <see cref="Graphic3D"/> is visible.
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
		/// <see cref="Graphic3D.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public virtual Rectangle3D BoundingBox
		{
			get { return Rectangle3D.Empty; }
		}

		/// <summary>
		/// Gets or sets the <see cref="CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// After the <see cref="Graphic3D.CoordinateSystem"/> has been set and the
		/// desired operations have been performed on the <see cref="Graphic3D"/>,
		/// it is proper practice to call <see cref="ResetCoordinateSystem"/>
		/// to restore the previous coordinate system.
		/// </remarks>
		public virtual CoordinateSystem CoordinateSystem
		{
			get { return CoordinateSystemStack.Peek(); }
			set { CoordinateSystemStack.Push(value); }
		}

		/// <summary>
		/// Gets this <see cref="Graphic3D"/> object's <see cref="SpatialTransform"/>.
		/// </summary>
		public virtual SpatialTransform3D SpatialTransform
		{
			get { return _spatialTransform ?? (_spatialTransform = CreateSpatialTransform()); }
			internal set { _spatialTransform = value; }
		}

		/// <summary>
		/// Performs a hit test on the <see cref="Graphic3D"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="Graphic3D"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// It is up to the <see cref="Graphic3D"/> to define what a "hit" is.
		/// </remarks>
		public abstract bool HitTest(Vector3D point);

		/// <summary>
		/// Gets the point on the <see cref="Graphic3D"/> closest to the specified point.
		/// </summary>
		/// <param name="point">A point in either source or destination coordinates.</param>
		/// <returns>The point on the graphic closest to the given <paramref name="point"/>.</returns>
		/// <remarks>
		/// <para>
		/// Depending on the value of <see cref="Graphic3D.CoordinateSystem"/>,
		/// the computation will be carried out in either source
		/// or destination coordinates.</para>
		/// </remarks>
		public virtual Vector3D GetClosestPoint(Vector3D point)
		{
			return point;
			// TODO: Implemlement
		}

		/// <summary>
		/// Moves the <see cref="Graphic3D"/> by a specified delta.
		/// </summary>
		/// <param name="delta">The distance to move.</param>
		/// <remarks>
		/// Depending on the value of <see cref="Graphic3D.CoordinateSystem"/>,
		/// <paramref name="delta"/> will be interpreted in either source
		/// or destination coordinates.
		/// </remarks>
		public abstract void Move(Vector3D delta);

		/// <summary>
		/// Resets the <see cref="CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="ResetCoordinateSystem"/> will reset the <see cref="CoordinateSystem"/>
		/// to what it was before the <see cref="Graphic3D.CoordinateSystem"/> property
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
		/// Fires just before the <see cref="Graphic3D"/> is drawn/rendered.
		/// </summary>
		public event EventHandler Drawing
		{
			add { _drawing += value; }
			remove { _drawing -= value; }
		}

		/// <summary>
		/// Draws the <see cref="Graphic3D"/>.
		/// </summary>
		public virtual void Draw()
		{
			if (ParentPresentationImage != null)
				ParentPresentationImage.Draw();
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
		/// Graphic3D objects that are not cloneable may return null.
		/// </remarks>
		public IGraphic3D Clone()
		{
			var clone = CloneBuilder.Clone(this) as IGraphic3D;
			if (clone != null && ImageViewer != null)
				ImageViewer.EventBroker.OnCloneCreated(new CloneCreatedEventArgs(this, clone));
			return clone;
		}

		/// <summary>
		/// Creates the <see cref="SpatialTransform"/> for this <see cref="Graphic3D"/>.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// Override this if the default <see cref="SpatialTransform"/> created
		/// is not appropriate.
		/// </remarks>
		protected virtual SpatialTransform3D CreateSpatialTransform()
		{
			return new SpatialTransform3D(this);
		}

		#region Disposal

		#region IDisposable Members

		/// <summary>
		/// Releases all resources used by this <see cref="Graphic3D"/>.
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
		protected virtual void Dispose(bool disposing) {}

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
		public event VisualStateChanged3DEventHandler VisualStateChanged
		{
			add { _visualStateChanged += value; }
			remove { _visualStateChanged -= value; }
		}

		/// <summary>
		/// Fires the <see cref="VisualStateChanged"/> event.
		/// </summary>
		/// <param name="propertyName">The name of the property whose value changed.</param>
		protected void NotifyVisualStateChanged(string propertyName)
		{
			OnVisualStateChanged(propertyName);
			EventsHelper.Fire(_visualStateChanged, this, new VisualStateChanged3DEventArgs(this, propertyName));
		}

		/// <summary>
		/// Forwards the <see cref="VisualStateChanged"/> event from a child graphic to listeners of a parent graphic.
		/// </summary>
		/// <param name="e">Data for the <see cref="VisualStateChanged"/> event.</param>
		internal void NotifyVisualStateChanged(VisualStateChanged3DEventArgs e)
		{
			if (e.Graphic == this) OnVisualStateChanged(e.PropertyName);
			EventsHelper.Fire(_visualStateChanged, this, e);
		}

		/// <summary>
		/// Called when the visual state changes on the current graphic, before notification of other handlers observing the <see cref="VisualStateChanged"/> event.
		/// </summary>
		/// <param name="propertyName">The name of the property whose value changed.</param>
		protected virtual void OnVisualStateChanged(string propertyName) {}
	}
}
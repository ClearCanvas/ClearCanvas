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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics3D
{
	/// <summary>
	/// Defines a 3D graphical object that can be rendered.
	/// </summary>
	public interface IGraphic3D : IDrawable, IDisposable
	{
		/// <summary>
		/// Gets this <see cref="IGraphic3D"/> object's parent <see cref="IGraphic3D"/>.
		/// </summary>
		IGraphic3D ParentGraphic { get; }

		/// <summary>
		/// Gets this <see cref="IGraphic3D"/> object's associated 
		/// <see cref="IPresentationImage"/>.
		/// </summary>
		/// <value>The associated <see cref="IPresentationImage"/> or <b>null</b>
		/// if the <see cref="IGraphic3D"/> is not yet part of the scene graph.</value>
		IPresentationImage ParentPresentationImage { get; }

		/// <summary>
		/// Gets this <see cref="IGraphic3D"/> object's associated 
		/// <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b>
		/// if the <see cref="IGraphic3D"/> is not yet associated with
		/// an <see cref="IImageViewer"/>.</value>
		IImageViewer ImageViewer { get; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="IGraphic3D"/> is visible.
		/// </summary>
		bool Visible { get; set; }

		/// <summary>
		/// Gets the tightest bounding box that encloses the graphic in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		Rectangle3D BoundingBox { get; }

		/// <summary>
		/// Gets or sets the <see cref="CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// After the <see cref="CoordinateSystem"/> has been set and the
		/// desired operations have been performed on the <see cref="IGraphic3D"/>,
		/// it is proper practice to call <see cref="ResetCoordinateSystem"/>
		/// to restore the previous coordinate system.
		/// </remarks>
		CoordinateSystem CoordinateSystem { get; set; }

		/// <summary>
		/// Gets this <see cref="IGraphic3D"/> object's <see cref="SpatialTransform3D"/>.
		/// </summary>
		SpatialTransform3D SpatialTransform { get; }

		/// <summary>
		/// Gets or sets the name of this <see cref="IGraphic3D"/>.
		/// </summary>
		/// <remarks>
		/// This vallue need not be unique, and should be used to indicate a logical equivalence or relation between multiple graphics.
		/// The <see cref="Uid"/> property should be used to identify separate and distinct instances.
		/// For example, <see cref="Name"/> can be used to logically group multiple graphics in the same scene graph together,
		/// or to logically equate one graphic with another in a pair of image clones.
		/// </remarks>
		string Name { get; set; }

		/// <summary>
		/// Gets a value uniquely identifying this <see cref="IGraphic3D"/> instance.
		/// </summary>
		/// <remarks>
		/// This value shall be unique, even for clones, as they are separate and distinct instances.
		/// The <see cref="Name"/> property should be used to indicate a logical equivalence or relation between multiple graphics.
		/// For example, <see cref="Uid"/> can be used to correlate graphic model objects and the renderer object used to render the model.
		/// </remarks>
		Guid Uid { get; }

		/// <summary>
		/// Performs a hit test on the <see cref="IGraphic3D"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="IGraphic3D"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// It is up to the implementation of <see cref="IGraphic3D"/> to define what a "hit" is.
		/// </remarks>
		bool HitTest(Vector3D point);

		/// <summary>
		/// Gets the point on the graphic closest to the specified point.
		/// </summary>
		/// <param name="point">A point in either source or destination coordinates.</param>
		/// <returns>The point on the graphic closest to the given <paramref name="point"/>.</returns>
		/// <remarks>
		/// Depending on the value of <see cref="CoordinateSystem"/>,
		/// the computation will be carried out in either source
		/// or destination coordinates.
		/// </remarks>
		Vector3D GetClosestPoint(Vector3D point);

		/// <summary>
		/// Moves the <see cref="IGraphic3D"/> by a specified delta.
		/// </summary>
		/// <param name="delta">The distance to move.</param>
		/// <remarks>
		/// Depending on the value of <see cref="CoordinateSystem"/>,
		/// <paramref name="delta"/> will be interpreted in either source
		/// or destination coordinates.
		/// </remarks>
		void Move(Vector3D delta);

		/// <summary>
		/// Resets the <see cref="CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="ResetCoordinateSystem"/> will reset the <see cref="CoordinateSystem"/>
		/// to what it was before the <see cref="CoordinateSystem"/> property
		/// was last set.
		/// </para>
		/// </remarks>
		void ResetCoordinateSystem();

		/// <summary>
		/// Creates a deep copy of the graphic.
		/// </summary>
		/// <remarks>
		/// Graphic objects that are not cloneable may return null.
		/// </remarks>
		IGraphic3D Clone();

		/// <summary>
		/// Occurs when a property is changed on a graphic, resulting in a change in the graphic's visual state.
		/// </summary>
		event VisualStateChanged3DEventHandler VisualStateChanged;
	}
}
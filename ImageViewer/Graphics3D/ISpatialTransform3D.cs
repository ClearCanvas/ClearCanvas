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

using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics3D
{
	/// <summary>
	/// Defines a 3D spatial transformation.
	/// </summary>
	public interface ISpatialTransform3D : IMemorable
	{
// ReSharper disable InconsistentNaming

		/// <summary>
		/// Gets or sets a value indicating that the object is mirrored across the yz-plane.
		/// </summary>
		bool FlipYZ { get; set; }

		/// <summary>
		/// Gets or sets a value indicating that the object is mirrored across the xz-plane.
		/// </summary>
		bool FlipXZ { get; set; }

		/// <summary>
		/// Gets or sets a value indicating that the object is mirrored across the xy-plane.
		/// </summary>
		bool FlipXY { get; set; }

// ReSharper restore InconsistentNaming

		/// <summary>
		/// Gets or sets the rotation.
		/// </summary>
		Matrix3D Rotation { get; set; }

		/// <summary>
		/// Gets or sets the scale.
		/// </summary>
		float Scale { get; set; }

		/// <summary>
		/// Gets or sets the translation in the x-direction.
		/// </summary>
		float TranslationX { get; set; }

		/// <summary>
		/// Gets or sets the translation in the y-direction.
		/// </summary>
		float TranslationY { get; set; }

		/// <summary>
		/// Gets or sets the translation in the z-direction.
		/// </summary>
		float TranslationZ { get; set; }

		/// <summary>
		/// Gets or sets the center of rotation.
		/// </summary>
		/// <remarks>
		/// The point should be specified in terms of the coordinate system
		/// of the parent object.
		/// </remarks>
		Vector3D CenterOfRotation { get; set; }

		/// <summary>
		/// Gets the cumulative scale.
		/// </summary>
		/// <remarks>
		/// Gets the scale relative to the root of the scene graph.
		/// </remarks>
		float CumulativeScale { get; }

		/// <summary>
		/// Resets all transform parameters to their defaults.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Resets all transform parameters to their defaults.
		/// </summary>
		void Reset();

		/// <summary>
		/// Converts a <see cref="Vector3D"/> point from source to destination coordinates.
		/// </summary>
		/// <param name="sourcePoint"></param>
		/// <returns></returns>
		Vector3D ConvertPointToDestination(Vector3D sourcePoint);

		/// <summary>
		/// Converts a <see cref="Vector3D"/> point from destination to source coordinates.
		/// </summary>
		/// <param name="destinationPoint"></param>
		/// <returns></returns>
		Vector3D ConvertPointToSource(Vector3D destinationPoint);

		/// <summary>
		/// Converts a <see cref="Rectangle3D"/> region from source to destination coordinates.
		/// </summary>
		/// <param name="sourceRectangle"></param>
		/// <returns></returns>
		Rectangle3D ConvertRectToDestination(Rectangle3D sourceRectangle);

		/// <summary>
		/// Converts a <see cref="Rectangle3D"/> region from destination to source coordinates.
		/// </summary>
		/// <param name="destinationRectangle"></param>
		/// <returns></returns>
		Rectangle3D ConvertRectToSource(Rectangle3D destinationRectangle);

		/// <summary>
		/// Converts a <see cref="Vector3D"/> vector from source to destination coordinates.
		/// </summary>
		/// <remarks>
		/// Only scale and rotation are applied when converting vectors, as direction vectors have only magnitude
		/// and direction information, but no position.
		/// </remarks>
		Vector3D ConvertVectorToDestination(Vector3D sourceVector);

		/// <summary>
		/// Converts a <see cref="Vector3D"/> vector from destination to source coordinates.
		/// </summary>
		/// <remarks>
		/// Only scale and rotation are applied when converting vectors, as direction vectors have only magnitude
		/// and direction information, but no position.
		/// </remarks>
		Vector3D ConvertVectorToSource(Vector3D destinationVector);
	}
}
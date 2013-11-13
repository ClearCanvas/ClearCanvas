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

using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provider for a <see cref="IPatientPresentation"/> instance describing the current presentation of the patient.
	/// </summary>
	public interface IPatientPresentationProvider
	{
		/// <summary>
		/// Gets the <see cref="IPatientPresentation"/> describing the current presentation of the patient.
		/// </summary>
		IPatientPresentation PatientPresentation { get; }
	}

	/// <summary>
	/// Represents the current presentation of the patient.
	/// </summary>
	public interface IPatientPresentation
	{
		/// <summary>
		/// Indicates whether or not the patient presentation is valid.
		/// </summary>
		bool IsValid { get; }

		/// <summary>
		/// Gets the current orientation of the image presentation X-axis in the patient coordinate system.
		/// </summary>
		Vector3D OrientationX { get; }

		/// <summary>
		/// Gets the current orientation of the image presentation Y-axis in the patient coordinate system.
		/// </summary>
		Vector3D OrientationY { get; }

		/// <summary>
		/// Gets the current orientation of the image presentation axes in the patient coordinate system.
		/// </summary>
		/// <param name="orientationX">The presentation X-axis.</param>
		/// <param name="orientationY">The presentation Y-axis.</param>
		void GetOrientation(out Vector3D orientationX, out Vector3D orientationY);
	}

	/// <summary>
	/// Basic implementation of <see cref="IPatientPresentation"/> based on the information provided via the <see cref="IImageSopProvider"/> and <see cref="ISpatialTransformProvider"/> interfaces.
	/// </summary>
	public sealed class BasicPatientPresentation : IPatientPresentation
	{
		private readonly IPresentationImage _owner;

		/// <summary>
		/// Initializes a new instance of <see cref="BasicPatientPresentation"/>.
		/// </summary>
		/// <param name="owner">The owner image.</param>
		public BasicPatientPresentation(IPresentationImage owner)
		{
			_owner = owner;
		}

		public bool IsValid
		{
			get { return _owner is IImageSopProvider && ((IImageSopProvider) _owner).Frame.ImagePlaneHelper.IsValid; }
		}

		public Vector3D OrientationX
		{
			get
			{
				Vector3D orientationX, orientationY;
				GetOrientation(out orientationX, out orientationY);
				return orientationX;
			}
		}

		public Vector3D OrientationY
		{
			get
			{
				Vector3D orientationX, orientationY;
				GetOrientation(out orientationX, out orientationY);
				return orientationY;
			}
		}

		public void GetOrientation(out Vector3D orientationX, out Vector3D orientationY)
		{
			var imageSopProvider = _owner as IImageSopProvider;
			if (imageSopProvider == null || !imageSopProvider.Frame.ImagePlaneHelper.IsValid)
			{
				orientationX = new Vector3D(1, 0, 0);
				orientationY = new Vector3D(0, 1, 0);
				return;
			}

			var point0 = new PointF(0, 0);
			var pointR = new PointF(100, 0);
			var pointD = new PointF(0, 100);

			var spatialTransformProvider = _owner as ISpatialTransformProvider;
			if (spatialTransformProvider != null)
			{
				point0 = spatialTransformProvider.SpatialTransform.ConvertToSource(point0);
				pointR = spatialTransformProvider.SpatialTransform.ConvertToSource(pointR);
				pointD = spatialTransformProvider.SpatialTransform.ConvertToSource(pointD);
			}

			var imagePlaneHelper = imageSopProvider.Frame.ImagePlaneHelper;
			var patient0 = imagePlaneHelper.ConvertToPatient(point0);
			orientationX = (imagePlaneHelper.ConvertToPatient(pointR) - patient0).Normalize();
			orientationY = (imagePlaneHelper.ConvertToPatient(pointD) - patient0).Normalize();
		}
	}
}
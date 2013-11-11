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
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Represents a mapping between image coordinates to patient coordinates.
	/// </summary>
	public interface IPatientCoordinateMapping
	{
		/// <summary>
		/// Gets whether or not the mapping is valid.
		/// </summary>
		bool IsValid { get; }

		/// <summary>
		/// Maps the specified location in image coordinates to patient coordinates.
		/// </summary>
		/// <param name="imageCoordinate">The image coordinates.</param>
		/// <returns>The equivalent patient coordinates, or NULL if the mapping is not valid.</returns>
		Vector3D ConvertToPatient(PointF imageCoordinate);
	}

	/// <summary>
	/// Default <see cref="Frame"/>-based implementation of <see cref="IPatientCoordinateMapping"/>.
	/// </summary>
	public class PatientCoordinateMapping : IPatientCoordinateMapping
	{
		private readonly Frame _frame;

		/// <summary>
		/// Initializes a new instance of <see cref="PatientCoordinateMapping"/>.
		/// </summary>
		public PatientCoordinateMapping(Frame frame)
		{
			_frame = frame;
		}

		public bool IsValid
		{
			get { return _frame.ImagePlaneHelper.IsValid; }
		}

		public Vector3D ConvertToPatient(PointF imageCoordinate)
		{
			return _frame.ImagePlaneHelper.ConvertToPatient(imageCoordinate);
		}
	}
}
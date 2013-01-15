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

using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Utilities
{
	public static class Math3D
	{
		public static Matrix OrientationMatrixFromVectors(Vector3D xVec, Vector3D yVec, Vector3D zVec)
		{
			return new Matrix
				(new float[4,4]
				 	{
				 		{xVec.X, xVec.Y, xVec.Z, 0},
				 		{yVec.X, yVec.Y, yVec.Z, 0},
				 		{zVec.X, zVec.Y, zVec.Z, 0},
				 		{0, 0, 0, 1}
				 	});
		}
	}
}
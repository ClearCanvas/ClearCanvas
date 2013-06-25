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

namespace ClearCanvas.ImageViewer.Graphics3D
{
	internal static class TransformMatrixHelpers
	{
		public static Vector3D TransformPoint(this Matrix transform, Vector3D point)
		{
			var transformed = transform*point.Augment();
			return new Vector3D(transformed[0, 0], transformed[1, 0], transformed[2, 0]);
		}

		public static Vector3D TransformVector(this Matrix transform, Vector3D vector)
		{
			var transformed = transform*vector.Augment(true);
			return new Vector3D(transformed[0, 0], transformed[1, 0], transformed[2, 0]);
		}

		public static Matrix Augment(this Matrix3D m)
		{
			return new Matrix(new[,]
			                  	{
			                  		{m[0, 0], m[0, 1], m[0, 2], 0},
			                  		{m[1, 0], m[1, 1], m[1, 2], 0},
			                  		{m[2, 0], m[2, 1], m[2, 2], 0},
			                  		{0, 0, 0, 1}
			                  	});
		}

		public static Matrix Augment(this Vector3D m, bool ignoreTranslation = false)
		{
			return new Matrix(new[,]
			                  	{
			                  		{m.X},
			                  		{m.Y},
			                  		{m.Z},
			                  		{ignoreTranslation ? 0 : 1}
			                  	});
		}

		public static void Scale(this Matrix m, float x, float y, float z)
		{
			var s = new Matrix(new[,]
			                   	{
			                   		{x, 0, 0, 0},
			                   		{0, y, 0, 0},
			                   		{0, 0, z, 0},
			                   		{0, 0, 0, 1}
			                   	});
			m.Set(s*m);
		}

		public static void Translate(this Matrix m, float x, float y, float z)
		{
			var t = new Matrix(new[,]
			                   	{
			                   		{1, 0, 0, x},
			                   		{0, 1, 0, y},
			                   		{0, 0, 1, z},
			                   		{0, 0, 0, 1}
			                   	});
			m.Set(t*m);
		}

		public static void Rotate(this Matrix m, Matrix3D rotation)
		{
			var r = rotation != null ? rotation.Augment() : Matrix.GetIdentity(4);
			m.Set(r*m);
		}

		public static void Multiply(this Matrix m, Matrix r)
		{
			m.Set(r*m);
		}

		private static void Set(this Matrix m, Matrix value)
		{
			m.SetRow(0, value[0, 0], value[0, 1], value[0, 2], value[0, 3]);
			m.SetRow(1, value[1, 0], value[1, 1], value[1, 2], value[1, 3]);
			m.SetRow(2, value[2, 0], value[2, 1], value[2, 2], value[2, 3]);
			m.SetRow(3, value[3, 0], value[3, 1], value[3, 2], value[3, 3]);
		}
	}
}
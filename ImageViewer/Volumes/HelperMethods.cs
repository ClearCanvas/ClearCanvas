#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

using System;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Volumes
{
	internal static class HelperMethods
	{
		/// <summary>
		/// Parses the value of a CS attribute as a boolean.
		/// </summary>
		public static bool? GetBoolean(this DicomAttribute attribute, int i, string trueString, string falseString)
		{
			var value = attribute.GetString(i, string.Empty);
			if (string.Equals(value, trueString, StringComparison.InvariantCultureIgnoreCase)) return true;
			else if (string.Equals(value, falseString, StringComparison.InvariantCultureIgnoreCase)) return false;
			return null;
		}

		/// <summary>
		/// Sets the boolean value of a CS attribute.
		/// </summary>
		public static void SetBoolean(this DicomAttribute attribute, int i, bool? value, string trueString, string falseString)
		{
			if (!value.HasValue) attribute.SetStringValue(string.Empty);
			else attribute.SetStringValue(value.Value ? trueString : falseString);
		}

		/// <summary>
		/// Gets the specified orientation row vector from a 4x4 affine transformation matrix.
		/// </summary>
		public static Vector3D GetRow(this Matrix matrix4X4, int row)
		{
			return new Vector3D(matrix4X4[row, 0], matrix4X4[row, 1], matrix4X4[row, 2]);
		}

		/// <summary>
		/// Augments the 3D orientation matrix as a 4x4 affine transformation matrix.
		/// </summary>
		public static Matrix Augment(this Matrix3D orientationMatrix, bool transpose = false)
		{
			var x = transpose ? orientationMatrix.GetColumn(0) : orientationMatrix.GetRow(0);
			var y = transpose ? orientationMatrix.GetColumn(1) : orientationMatrix.GetRow(1);
			var z = transpose ? orientationMatrix.GetColumn(2) : orientationMatrix.GetRow(2);
			return new Matrix(new[,]
			                  	{
			                  		{x.X, x.Y, x.Z, 0},
			                  		{y.X, y.Y, y.Z, 0},
			                  		{z.X, z.Y, z.Z, 0},
			                  		{0, 0, 0, 1}
			                  	});
		}
	}
}
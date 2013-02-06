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
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Mathematics
{
    // TODO (CR Phoenix5 - Med): although very similar to Matrix, it should have unit tests.

	public class Matrix3D
	{
		private readonly float[,] _matrix = new float[3,3];

		public Matrix3D() {}

		public Matrix3D(float[,] matrix)
		{
			Platform.CheckTrue(matrix.GetLength(0) == 3 && matrix.GetLength(1) == 3, "matrix must be 3x3");

			_matrix[0, 0] = matrix[0, 0];
			_matrix[0, 1] = matrix[0, 1];
			_matrix[0, 2] = matrix[0, 2];
			_matrix[1, 0] = matrix[1, 0];
			_matrix[1, 1] = matrix[1, 1];
			_matrix[1, 2] = matrix[1, 2];
			_matrix[2, 0] = matrix[2, 0];
			_matrix[2, 1] = matrix[2, 1];
			_matrix[2, 2] = matrix[2, 2];
		}

		public Matrix3D(Matrix matrix)
		{
			Platform.CheckTrue(matrix.Rows == 3 && matrix.Columns == 3, "matrix must be 3x3");

			_matrix[0, 0] = matrix[0, 0];
			_matrix[0, 1] = matrix[0, 1];
			_matrix[0, 2] = matrix[0, 2];
			_matrix[1, 0] = matrix[1, 0];
			_matrix[1, 1] = matrix[1, 1];
			_matrix[1, 2] = matrix[1, 2];
			_matrix[2, 0] = matrix[2, 0];
			_matrix[2, 1] = matrix[2, 1];
			_matrix[2, 2] = matrix[2, 2];
		}

		public bool IsIdentity
		{
			get
			{
				return FloatComparer.AreEqual(_matrix[0, 0], 1)
				       && FloatComparer.AreEqual(_matrix[0, 1], 0)
				       && FloatComparer.AreEqual(_matrix[0, 2], 0)
				       && FloatComparer.AreEqual(_matrix[1, 0], 0)
				       && FloatComparer.AreEqual(_matrix[1, 1], 1)
				       && FloatComparer.AreEqual(_matrix[1, 2], 0)
				       && FloatComparer.AreEqual(_matrix[2, 0], 0)
				       && FloatComparer.AreEqual(_matrix[2, 1], 0)
				       && FloatComparer.AreEqual(_matrix[2, 2], 1);
			}
		}

		public float this[int row, int column]
		{
			get
			{
				Platform.CheckArgumentRange(row, 0, 2, "row");
				Platform.CheckArgumentRange(column, 0, 2, "column");

				return _matrix[row, column];
			}
			set
			{
				Platform.CheckArgumentRange(row, 0, 2, "row");
				Platform.CheckArgumentRange(column, 0, 2, "column");

				_matrix[row, column] = value;
			}
		}

		public Vector3D GetRow(int row)
		{
			return new Vector3D(_matrix[row, 0], _matrix[row, 1], _matrix[row, 2]);
		}

		public Vector3D GetColumn(int column)
		{
			return new Vector3D(_matrix[0, column], _matrix[1, column], _matrix[2, column]);
		}

		public void SetRow(int row, Vector3D vector)
		{
			SetRow(row, vector.X, vector.Y, vector.Z);
		}

		public void SetRow(int row, float value0, float value1, float value2)
		{
			Platform.CheckArgumentRange(row, 0, 2, "row");

			_matrix[row, 0] = value0;
			_matrix[row, 1] = value1;
			_matrix[row, 2] = value2;
		}

		public void SetColumn(int column, Vector3D vector)
		{
			SetColumn(column, vector.X, vector.Y, vector.Z);
		}

		public void SetColumn(int column, float value0, float value1, float value2)
		{
			Platform.CheckArgumentRange(column, 0, 2, "column");

			_matrix[0, column] = value0;
			_matrix[1, column] = value1;
			_matrix[2, column] = value2;
		}

		public Matrix3D Clone()
		{
			return new Matrix3D(_matrix);
		}

		private void Scale(float scale)
		{
			for (int row = 0; row < 3; ++row)
			{
				for (int column = 0; column < 3; ++column)
					this[row, column] = this[row, column]*scale;
			}
		}

		/// <summary>
		/// Returns a matrix that is the transpose of this matrix.
		/// </summary>
		public Matrix3D Transpose()
		{
			Matrix3D transpose = new Matrix3D();

			for (int row = 0; row < 3; ++row)
			{
				for (int column = 0; column < 3; ++column)
					transpose[column, row] = this[row, column];
			}

			return transpose;
		}

		/// <summary>
		/// Gets a string describing the matrix.
		/// </summary>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			for (int row = 0; row < 3; ++row)
			{
				builder.Append("( ");
				for (int column = 0; column < 3; ++column)
				{
					builder.Append(_matrix[row, column].ToString("F4"));
					if (column < (3 - 1))
						builder.Append("  ");
				}

				builder.Append(")");
				if (row < (3 - 1))
					builder.Append(", ");
			}

			return builder.ToString();
		}

		public static Matrix3D GetIdentity()
		{
			return new Matrix3D(new float[,] {{1, 0, 0}, {0, 1, 0}, {0, 0, 1}});
		}

		/// <summary>
		/// Performs scalar multiplication of <paramref name="matrix"/> by 1/<paramref name="scale"/>.
		/// </summary>
		public static Matrix3D operator /(Matrix3D matrix, float scale)
		{
			Matrix3D clone = matrix.Clone();
			clone.Scale(1/scale);
			return clone;
		}

		/// <summary>
		/// Performs element-by-element addition of <paramref name="left"/> and <paramref name="right"/>.
		/// </summary>
		/// <exception cref="ArgumentException">If the matrices do not have the same dimensions.</exception>
		public static Matrix3D operator +(Matrix3D left, Matrix3D right)
		{
			Matrix3D clone = left.Clone();

			for (int row = 0; row < 3; ++row)
			{
				for (int column = 0; column < 3; ++column)
					clone[row, column] += right[row, column];
			}

			return clone;
		}

		/// <summary>
		/// Performs matrix multiplication of <paramref name="left"/> and <paramref name="right"/>.
		/// </summary>
		public static Matrix3D operator *(Matrix3D left, Matrix3D right)
		{
			Matrix3D result = new Matrix3D();

			for (int row = 0; row < 3; ++row)
			{
				for (int column = 0; column < 3; ++column)
				{
					float value = 0F;

					for (int k = 0; k < 3; ++k)
						value = value + left[row, k]*right[k, column];

					result[row, column] = value;
				}
			}

			return result;
		}

		/// <summary>
		/// Performs multiplication of matrix <paramref name="left"/> and column vector <paramref name="right"/>.
		/// </summary>
		public static Vector3D operator *(Matrix3D left, Vector3D right)
		{
			var rights = new[] {right.X, right.Y, right.Z};
			var result = new float[3];

			for (int row = 0; row < 3; ++row)
			{
				float value = 0F;

				for (int column = 0; column < 3; ++column)
					value += left[row, column]*rights[column];

				result[row] = value;
			}

			return new Vector3D(result[0], result[1], result[2]);
		}

		/// <summary>
		/// Performs multiplication of row vector <paramref name="left"/> and matrix <paramref name="right"/>.
		/// </summary>
		public static Vector3D operator *(Vector3D left, Matrix3D right)
		{
			var lefts = new[] {left.X, left.Y, left.Z};
			var result = new float[3];

			for (int column = 0; column < 3; ++column)
			{
				float value = 0F;

				for (int row = 0; row < 3; ++row)
					value += lefts[row]*right[row, column];

				result[column] = value;
			}

			return new Vector3D(result[0], result[1], result[2]);
		}

		/// <summary>
		/// Performs scalar multiplication of <paramref name="matrix"/> by <paramref name="scale"/>.
		/// </summary>
		public static Matrix3D operator *(float scale, Matrix3D matrix)
		{
			return matrix*scale;
		}

		/// <summary>
		/// Performs scalar multiplication of <paramref name="matrix"/> by <paramref name="scale"/>.
		/// </summary>
		public static Matrix3D operator *(Matrix3D matrix, float scale)
		{
			Matrix3D clone = matrix.Clone();
			clone.Scale(scale);
			return clone;
		}

		/// <summary>
		/// Performs element-by-element subtraction of <paramref name="right"/> from <paramref name="left"/>.
		/// </summary>
		/// <exception cref="ArgumentException">If the matrices do not have the same dimensions.</exception>
		public static Matrix3D operator -(Matrix3D left, Matrix3D right)
		{
			Matrix3D clone = left.Clone();

			for (int row = 0; row < 3; ++row)
			{
				for (int column = 0; column < 3; ++column)
					clone[row, column] -= right[row, column];
			}

			return clone;
		}

		/// <summary>
		/// Gets a value indicating whether or not the elements of <paramref name="left"/> are equal to <paramref name="right"/> within the given tolerance.
		/// </summary>
		/// <exception cref="ArgumentException">If the matrices do not have the same dimensions.</exception>

		// TODO (CR Phoenix5 - High): FloatComparer tolerance for floats is actually a float tolerance, not in units of epsilons. 100 will make this basically say TONS of matrices are equal.
        public static bool AreEqual(Matrix3D left, Matrix3D right, float tolerance = 100)
		{
			for (int row = 0; row < 3; ++row)
			{
				for (int column = 0; column < 3; ++column)
				{
					if (!FloatComparer.AreEqual(left[row, column], right[row, column], tolerance))
						return false;
				}
			}

			return true;
		}

		public static implicit operator Matrix(Matrix3D matrix)
		{
			return new Matrix((float[,]) matrix._matrix.Clone());
		}

		public static explicit operator Matrix3D(Matrix matrix)
		{
			return new Matrix3D(matrix);
		}
	}
}
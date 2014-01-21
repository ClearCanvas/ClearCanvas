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
	/// <summary>
	/// Represents a matrix of arbitrary dimensions.
	/// </summary>
	public sealed class Matrix
	{
		private readonly int _rows;
		private readonly int _columns;

		private readonly float[,] _matrix;

		/// <summary>
		/// Constructs a new zero matrix of the given dimensions.
		/// </summary>
		/// <param name="rows">The number of rows in the matrix.</param>
		/// <param name="columns">The number of columns in the matrix.</param>
		/// <exception cref="ArgumentException">Thrown if either <paramref name="rows"/> or <paramref name="columns"/> is zero or negative.</exception>
		public Matrix(int rows, int columns)
		{
			Platform.CheckPositive(rows, "rows");
			Platform.CheckPositive(columns, "columns");

			_rows = rows;
			_columns = columns;

			_matrix = new float[rows,columns];
		}

		/// <summary>
		/// Constructs a new matrix and initializes its values to that of the given 2-dimensional array.
		/// </summary>
		/// <param name="matrix">The 2-dimensional array (rows, then columns) with which to initialize the matrix.</param>
		/// <exception cref="ArgumentNullException">Thrown if the supplied matrix is null.</exception>
		public Matrix(float[,] matrix)
		{
			Platform.CheckForNullReference(matrix, "matrix");

			_matrix = matrix;
			_rows = matrix.GetLength(0);
			_columns = matrix.GetLength(1);
		}

		/// <summary>
		/// Constructs a new matrix as a clone of an existing matrix.
		/// </summary>
		/// <param name="matrix">The source matrix to clone.</param>
		/// <exception cref="ArgumentNullException">Thrown if the supplied matrix is null.</exception>
		public Matrix(Matrix matrix)
		{
			Platform.CheckForNullReference(matrix, "matrix");

			_rows = matrix._rows;
			_columns = matrix._columns;
			_matrix = (float[,]) matrix._matrix.Clone();
		}

		/// <summary>
		/// Gets the number of rows in the matrix.
		/// </summary>
		public int Rows
		{
			get { return _rows; }
		}

		/// <summary>
		/// Gets the number of columns in the matrix.
		/// </summary>
		public int Columns
		{
			get { return _columns; }
		}

		/// <summary>
		/// Gets whether or not the matrix is square (rows == columns).
		/// </summary>
		public bool IsSquare
		{
			get { return _rows == _columns; }
		}

		/// <summary>
		/// Gets whether or not this is an identity matrix.
		/// </summary>
		public bool IsIdentity
		{
			get
			{
				if (!IsSquare)
					return false;

				for (int row = 0; row < _rows; ++row)
				{
					for (int column = 0; column < _columns; ++column)
					{
						if (_matrix[row, column] != ((row == column) ? 1F : 0F))
							return false;
					}
				}

				return true;
			}
		}

		/// <summary>
		/// Gets or sets the value of the cell at the specified row and column indices.
		/// </summary>
		/// <param name="row">The row index</param>
		/// <param name="column">The column index</param>
		public float this[int row, int column]
		{
			get
			{
				Platform.CheckArgumentRange(row, 0, _rows - 1, "row");
				Platform.CheckArgumentRange(column, 0, _columns - 1, "column");

				return _matrix[row, column];
			}
			set
			{
				Platform.CheckArgumentRange(row, 0, _rows - 1, "row");
				Platform.CheckArgumentRange(column, 0, _columns - 1, "column");

				_matrix[row, column] = value;
			}
		}

		private void Scale(float scale)
		{
			for (int row = 0; row < _rows; ++row)
			{
				for (int column = 0; column < _columns; ++column)
					this[row, column] = this[row, column]*scale;
			}
		}

		/// <summary>
		/// Sets all the values in a particular row.
		/// </summary>
		/// <remarks>
		/// This is more efficient than setting each value separately.
		/// </remarks>
		public void SetRow(int row, params float[] values)
		{
			Platform.CheckArgumentRange(row, 0, _rows - 1, "row");
			Platform.CheckTrue(values.Length == _columns, "number of parameters == _columns");

			for (int column = 0; column < _columns; ++column)
				_matrix[row, column] = values[column];
		}

		/// <summary>
		/// Sets all the values in a particular column.
		/// </summary>
		/// <remarks>
		/// This is more efficient than setting each value separately.
		/// </remarks>
		public void SetColumn(int column, params float[] values)
		{
			Platform.CheckArgumentRange(column, 0, _columns - 1, "column");
			Platform.CheckTrue(values.Length == _rows, "number of parameters == _rows");

			for (int row = 0; row < _rows; ++row)
				_matrix[row, column] = values[row];
		}

		/// <summary>
		/// Clones this matrix and its values.
		/// </summary>
		public Matrix Clone()
		{
			float[,] matrix = (float[,]) _matrix.Clone();
			for (int row = 0; row < _rows; ++row)
			{
				for (int column = 0; column < _columns; ++column)
					matrix[row, column] = _matrix[row, column];
			}

			return new Matrix(matrix);
		}

		/// <summary>
		/// Returns a matrix that is the transpose of this matrix.
		/// </summary>
		public Matrix Transpose()
		{
			Matrix transpose = new Matrix(_columns, _rows);

			for (int row = 0; row < Rows; ++row)
			{
				for (int column = 0; column < Columns; ++column)
					transpose[column, row] = this[row, column];
			}

			return transpose;
		}

		public float GetDeterminant()
		{
			Platform.CheckTrue(_rows == _columns, "Matrix must be square!");

			switch (_rows)
			{
				case 1:
					// det(I) = 1, and det(k*A) = (k^n)*det(A)
					// so for matrix A (n=1), with entry A[0,0] = k,
					// det(A) = det(k*I) = (k^1)*det(I) = k = A[0,0]
					return _matrix[0, 0];
				case 2:
					return Determinant2(_matrix);
				case 3:
					return Determinant3(_matrix);
				case 4:
					return Determinant4(_matrix);
				default:
					throw new NotImplementedException("GetDeterminant is not implemented for matrix size N > 4");
			}
		}

		public Matrix Invert()
		{
			Platform.CheckTrue(_rows == _columns, "Matrix must be square!");

			switch (_rows)
			{
				case 1:
					// A*inv(A) = I
					// so for matrix A (n=1), with entry A[0,0] = k,
					// A*inv(A) = A[0,0]*invA[0,0] = 1 => invA[0,0] = 1/A[0,0]
					Platform.CheckFalse(FloatComparer.AreEqual(0, _matrix[0, 0]), "Matrix is not invertible!");
					return new Matrix(new[,] {{1/_matrix[0, 0]}});
				case 2:
					return new Matrix(Invert2(_matrix));
				case 3:
					return new Matrix(Invert3(_matrix));
				case 4:
					return new Matrix(Invert4(_matrix));
				default:
					throw new NotImplementedException("Invert is not implemented for matrix size N > 4");
			}
		}

		/// <summary>
		/// Gets a string describing the matrix.
		/// </summary>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			for (int row = 0; row < _rows; ++row)
			{
				builder.Append("( ");
				for (int column = 0; column < _columns; ++column)
				{
					builder.Append(_matrix[row, column].ToString("F4"));
					if (column < (_columns - 1))
						builder.Append("  ");
				}

				builder.Append(")");
				if (row < (_rows - 1))
					builder.Append(", ");
			}

			return builder.ToString();
		}

		/// <summary>
		/// Gets an identity matrix with the input dimensions.
		/// </summary>
		public static Matrix GetIdentity(int dimensions)
		{
			Matrix matrix = new Matrix(dimensions, dimensions);
			for (int i = 0; i < dimensions; ++i)
				matrix[i, i] = 1.0F;

			return matrix;
		}

		/// <summary>
		/// Gets an identity matrix with the dimensions of the given square matrix.
		/// </summary>
		public static Matrix GetIdentity(Matrix matrix)
		{
			Platform.CheckForNullReference(matrix, "matrix");
			Platform.CheckTrue(matrix.IsSquare, "Matrix must be square.");
			return GetIdentity(matrix.Rows);
		}

		/// <summary>
		/// Performs matrix multiplication of <paramref name="left"/> and <paramref name="right"/>.
		/// </summary>
		public static Matrix operator *(Matrix left, Matrix right)
		{
			if (left.Columns != right.Rows)
				throw new ArgumentException("Cannot multiply the two matrices together; their sizes are incompatible.");

			Matrix result = new Matrix(left.Rows, right.Columns);
			int mutualDimension = right.Rows;

			for (int row = 0; row < result.Rows; ++row)
			{
				for (int column = 0; column < result.Columns; ++column)
				{
					float value = 0F;

					for (int k = 0; k < mutualDimension; ++k)
						value = value + left[row, k]*right[k, column];

					result[row, column] = value;
				}
			}

			return result;
		}

		/// <summary>
		/// Performs scalar multiplication of <paramref name="matrix"/> by <paramref name="scale"/>.
		/// </summary>
		public static Matrix operator *(float scale, Matrix matrix)
		{
			return matrix*scale;
		}

		/// <summary>
		/// Performs scalar multiplication of <paramref name="matrix"/> by <paramref name="scale"/>.
		/// </summary>
		public static Matrix operator *(Matrix matrix, float scale)
		{
			Matrix clone = matrix.Clone();
			clone.Scale(scale);
			return clone;
		}

		/// <summary>
		/// Performs scalar multiplication of <paramref name="matrix"/> by -1.
		/// </summary>
		public static Matrix operator -(Matrix matrix)
		{
			Matrix clone = matrix.Clone();
			clone.Scale(-1);
			return clone;
		}

		/// <summary>
		/// This notation is no longer supported.
		/// </summary>
		/// <remarks>
		/// <para>This notation is ambiguous. Please use an explicit notation of the expected operation, such as one of the following.</para>
		/// <code>scale * matrix.Inverse()</code>
		/// <code>matrix / scale</code>
		/// <code>scale * Matrix.GetIdentity(matrix) * matrix.Inverse()</code>
		/// <code>(scale * Matrix.GetIdentity(matrix)).Inverse() * matrix</code>
		/// </remarks>
		[Obsolete("Did you mean scale * Inverse(matrix), matrix / scale, scale * Identity(Dimensions(matrix)) * Inverse(matrix), or Inverse(scale * Identity(Dimensions(matrix))) * matrix?", true)]
		public static Matrix operator /(float scale, Matrix matrix)
		{
			// any existing compiled code depending on this operator will continue to work as it always has
			// any new code referencing this method will fail to compile
			return matrix/scale;
		}

		/// <summary>
		/// Performs scalar multiplication of <paramref name="matrix"/> by 1/<paramref name="scale"/>.
		/// </summary>
		public static Matrix operator /(Matrix matrix, float scale)
		{
			Matrix clone = matrix.Clone();
			clone.Scale(1/scale);
			return clone;
		}

		/// <summary>
		/// Performs element-by-element addition of <paramref name="left"/> and <paramref name="right"/>.
		/// </summary>
		/// <exception cref="ArgumentException">If the matrices do not have the same dimensions.</exception>
		public static Matrix operator +(Matrix left, Matrix right)
		{
			Platform.CheckTrue(left.Columns == right.Columns && left.Rows == right.Rows, "Matrix Same Dimensions");

			Matrix clone = left.Clone();

			for (int row = 0; row < left.Rows; ++row)
			{
				for (int column = 0; column < left.Columns; ++column)
					clone[row, column] += right[row, column];
			}

			return clone;
		}

		/// <summary>
		/// Performs element-by-element subtraction of <paramref name="right"/> from <paramref name="left"/>.
		/// </summary>
		/// <exception cref="ArgumentException">If the matrices do not have the same dimensions.</exception>
		public static Matrix operator -(Matrix left, Matrix right)
		{
			Platform.CheckTrue(left.Columns == right.Columns && left.Rows == right.Rows, "Matrix Same Dimensions");

			Matrix clone = left.Clone();

			for (int row = 0; row < left.Rows; ++row)
			{
				for (int column = 0; column < left.Columns; ++column)
					clone[row, column] -= right[row, column];
			}

			return clone;
		}

		/// <summary>
		/// Gets a value indicating whether or not the elements of <paramref name="left"/> are equal to <paramref name="right"/> within the given tolerance.
		/// </summary>
		/// <exception cref="ArgumentException">If the matrices do not have the same dimensions.</exception>
		public static bool AreEqual(Matrix left, Matrix right, float tolerance)
		{
			Platform.CheckTrue(left.Columns == right.Columns && left.Rows == right.Rows, "Matrix Same Dimensions");

			for (int row = 0; row < left.Rows; ++row)
			{
				for (int column = 0; column < left.Columns; ++column)
				{
					if (!FloatComparer.AreEqual(left[row, column], right[row, column], tolerance))
						return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Gets a value indicating whether or not the elements of <paramref name="left"/> are equal to <paramref name="right"/> within a small tolerance.
		/// </summary>
		/// <exception cref="ArgumentException">If the matrices do not have the same dimensions.</exception>
		public static bool AreEqual(Matrix left, Matrix right)
		{
			Platform.CheckTrue(left.Columns == right.Columns && left.Rows == right.Rows, "Matrix Same Dimensions");

			for (int row = 0; row < left.Rows; ++row)
			{
				for (int column = 0; column < left.Columns; ++column)
				{
					if (!FloatComparer.AreEqual(left[row, column], right[row, column]))
						return false;
				}
			}

			return true;
		}

		#region Advanced Math Implementations

		#region 2x2

		internal static float Determinant2(float[,] m)
		{
			return (float) ((double) m[0, 0]*m[1, 1] - (double) m[0, 1]*m[1, 0]);
		}

		internal static float[,] Invert2(float[,] m)
		{
			var det = (double) m[0, 0]*m[1, 1] - (double) m[0, 1]*m[1, 0];
			Platform.CheckFalse(FloatComparer.AreEqual(0, det), "Matrix is not invertible!");

			var inv = new float[2,2];
			det = 1/det;

			inv[0, 0] = (float) (m[1, 1]*det);
			inv[0, 1] = (float) (-m[0, 1]*det);
			inv[1, 0] = (float) (-m[1, 0]*det);
			inv[1, 1] = (float) (m[0, 0]*det);

			return inv;
		}

		#endregion

		#region 3x3

		internal static float Determinant3(float[,] m)
		{
			return (float) (m[0, 0]*((double) m[1, 1]*m[2, 2] - (double) m[1, 2]*m[2, 1])
			                - m[0, 1]*((double) m[1, 0]*m[2, 2] - (double) m[1, 2]*m[2, 0])
			                + m[0, 2]*((double) m[1, 0]*m[2, 1] - (double) m[1, 1]*m[2, 0]));
		}

		internal static float[,] Invert3(float[,] m)
		{
			var b0 = (double) m[1, 1]*m[2, 2] - (double) m[1, 2]*m[2, 1];
			var b1 = (double) m[1, 0]*m[2, 2] - (double) m[1, 2]*m[2, 0];
			var b2 = (double) m[1, 0]*m[2, 1] - (double) m[1, 1]*m[2, 0];

			var c0 = (double) m[0, 1]*m[2, 2] - (double) m[0, 2]*m[2, 1];
			var s0 = (double) m[0, 1]*m[1, 2] - (double) m[0, 2]*m[1, 1];
			var c1 = (double) m[0, 0]*m[2, 2] - (double) m[0, 2]*m[2, 0];

			var s1 = (double) m[0, 0]*m[1, 2] - (double) m[0, 2]*m[1, 0];
			var c2 = (double) m[0, 0]*m[2, 1] - (double) m[0, 1]*m[2, 0];
			var s2 = (double) m[0, 0]*m[1, 1] - (double) m[0, 1]*m[1, 0];

			var det = m[0, 0]*b0 - m[0, 1]*b1 + m[0, 2]*b2;
			Platform.CheckFalse(FloatComparer.AreEqual(0, det), "Matrix is not invertible!");

			var inv = new float[3,3];
			det = 1/det;

			inv[0, 0] = (float) (b0*det);
			inv[0, 1] = (float) (-c0*det);
			inv[0, 2] = (float) (s0*det);
			inv[1, 0] = (float) (-b1*det);
			inv[1, 1] = (float) (c1*det);
			inv[1, 2] = (float) (-s1*det);
			inv[2, 0] = (float) (b2*det);
			inv[2, 1] = (float) (-c2*det);
			inv[2, 2] = (float) (s2*det);

			return inv;
		}

		#endregion

		#region 4x4

		internal static float Determinant4(float[,] m)
		{
			var s0 = (double) m[0, 0]*m[1, 1] - (double) m[1, 0]*m[0, 1];
			var s1 = (double) m[0, 0]*m[1, 2] - (double) m[1, 0]*m[0, 2];
			var s2 = (double) m[0, 0]*m[1, 3] - (double) m[1, 0]*m[0, 3];
			var s3 = (double) m[0, 1]*m[1, 2] - (double) m[1, 1]*m[0, 2];
			var s4 = (double) m[0, 1]*m[1, 3] - (double) m[1, 1]*m[0, 3];
			var s5 = (double) m[0, 2]*m[1, 3] - (double) m[1, 2]*m[0, 3];

			var c5 = (double) m[2, 2]*m[3, 3] - (double) m[3, 2]*m[2, 3];
			var c4 = (double) m[2, 1]*m[3, 3] - (double) m[3, 1]*m[2, 3];
			var c3 = (double) m[2, 1]*m[3, 2] - (double) m[3, 1]*m[2, 2];
			var c2 = (double) m[2, 0]*m[3, 3] - (double) m[3, 0]*m[2, 3];
			var c1 = (double) m[2, 0]*m[3, 2] - (double) m[3, 0]*m[2, 2];
			var c0 = (double) m[2, 0]*m[3, 1] - (double) m[3, 0]*m[2, 1];

			return (float) (s0*c5 - s1*c4 + s2*c3 + s3*c2 - s4*c1 + s5*c0);
		}

		internal static float[,] Invert4(float[,] m)
		{
			var s0 = (double) m[0, 0]*m[1, 1] - (double) m[1, 0]*m[0, 1];
			var s1 = (double) m[0, 0]*m[1, 2] - (double) m[1, 0]*m[0, 2];
			var s2 = (double) m[0, 0]*m[1, 3] - (double) m[1, 0]*m[0, 3];
			var s3 = (double) m[0, 1]*m[1, 2] - (double) m[1, 1]*m[0, 2];
			var s4 = (double) m[0, 1]*m[1, 3] - (double) m[1, 1]*m[0, 3];
			var s5 = (double) m[0, 2]*m[1, 3] - (double) m[1, 2]*m[0, 3];

			var c5 = (double) m[2, 2]*m[3, 3] - (double) m[3, 2]*m[2, 3];
			var c4 = (double) m[2, 1]*m[3, 3] - (double) m[3, 1]*m[2, 3];
			var c3 = (double) m[2, 1]*m[3, 2] - (double) m[3, 1]*m[2, 2];
			var c2 = (double) m[2, 0]*m[3, 3] - (double) m[3, 0]*m[2, 3];
			var c1 = (double) m[2, 0]*m[3, 2] - (double) m[3, 0]*m[2, 2];
			var c0 = (double) m[2, 0]*m[3, 1] - (double) m[3, 0]*m[2, 1];

			var det = s0*c5 - s1*c4 + s2*c3 + s3*c2 - s4*c1 + s5*c0;
			Platform.CheckFalse(FloatComparer.AreEqual(0, det), "Matrix is not invertible!");

			var inv = new float[4,4];
			det = 1/det;

			inv[0, 0] = (float) ((m[1, 1]*c5 - m[1, 2]*c4 + m[1, 3]*c3)*det);
			inv[0, 1] = (float) ((-m[0, 1]*c5 + m[0, 2]*c4 - m[0, 3]*c3)*det);
			inv[0, 2] = (float) ((m[3, 1]*s5 - m[3, 2]*s4 + m[3, 3]*s3)*det);
			inv[0, 3] = (float) ((-m[2, 1]*s5 + m[2, 2]*s4 - m[2, 3]*s3)*det);
			inv[1, 0] = (float) ((-m[1, 0]*c5 + m[1, 2]*c2 - m[1, 3]*c1)*det);
			inv[1, 1] = (float) ((m[0, 0]*c5 - m[0, 2]*c2 + m[0, 3]*c1)*det);
			inv[1, 2] = (float) ((-m[3, 0]*s5 + m[3, 2]*s2 - m[3, 3]*s1)*det);
			inv[1, 3] = (float) ((m[2, 0]*s5 - m[2, 2]*s2 + m[2, 3]*s1)*det);
			inv[2, 0] = (float) ((m[1, 0]*c4 - m[1, 1]*c2 + m[1, 3]*c0)*det);
			inv[2, 1] = (float) ((-m[0, 0]*c4 + m[0, 1]*c2 - m[0, 3]*c0)*det);
			inv[2, 2] = (float) ((m[3, 0]*s4 - m[3, 1]*s2 + m[3, 3]*s0)*det);
			inv[2, 3] = (float) ((-m[2, 0]*s4 + m[2, 1]*s2 - m[2, 3]*s0)*det);
			inv[3, 0] = (float) ((-m[1, 0]*c3 + m[1, 1]*c1 - m[1, 2]*c0)*det);
			inv[3, 1] = (float) ((m[0, 0]*c3 - m[0, 1]*c1 + m[0, 2]*c0)*det);
			inv[3, 2] = (float) ((-m[3, 0]*s3 + m[3, 1]*s1 - m[3, 2]*s0)*det);
			inv[3, 3] = (float) ((m[2, 0]*s3 - m[2, 1]*s1 + m[2, 2]*s0)*det);

			return inv;
		}

		#endregion

		#endregion
	}
}
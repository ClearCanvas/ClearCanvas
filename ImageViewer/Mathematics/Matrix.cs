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
	// TODO: Determinant, Inverse, etc are still missing.

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
	}
}
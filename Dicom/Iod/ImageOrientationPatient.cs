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
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod
{
	/// <summary>
	/// Represents the orientation of the image in the patient coordinate system.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="ImageOrientationPatient"/> class is quite simple, basically providing a centralized place to store the
	/// row/column direction cosines from the DICOM Header, which are basically the unit vectors in the patient coordinate
	/// system that describe the orientation of the a row and a column in the image. One additional piece of functionality is
	/// the primary and secondary row/column directions, which are transformed (using the cosines) into more meaningful 
	/// values (Anterior, Left, Head, etc).
	/// </para>
	/// <para>
	/// The components of each of the cosine vectors (row/column, X,Y,Z) corresponds to the patient based coordinate
	/// system as follows (using a right-handed convention):
	/// </para>
	/// <para>
	/// +X --> Left,			-X --> Right,
	/// +Y --> Posterior,		-Y --> Anterior,
	/// +Z --> Head,			-Z --> Foot
	/// </para>
	/// <para>
	/// The primary and secondary directions of a cosine vector correspond directly to the 2 largest
	/// values in the cosine vector, disregarding the sign. The sign determines the direction along 
	/// a particular axis in the patient based system as described above.
	/// </para>
	/// <para>
	/// The row cosine vector completely describes the direction, in the patient based system, of the first row
	/// in the image (increasing X). Similarly, the column cosine vector completely describes the 
	/// direction of the first column in the image in the patient based system.
	/// </para>
	/// </remarks>
	public class ImageOrientationPatient : IEquatable<ImageOrientationPatient>
	{
		#region Directions Enum

		/// <summary>
		/// Defines the direction of the axes in the patient coordinate system.
		/// </summary>
		public enum Directions
		{
			None = 0,
			Left = 1,
			Right = -1,
			Posterior = 2,
			Anterior = -2,
			Head = 3,
			Foot = -3
		};

		#endregion

		#region Standard Orientations

		private static readonly int[] _left = new[] {1, 0, 0};
		private static readonly int[] _right = new[] {-1, 0, 0};
		private static readonly int[] _posterior = new[] {0, 1, 0};
		private static readonly int[] _anterior = new[] {0, -1, 0};
		private static readonly int[] _foot = new[] {0, 0, -1};
		// head would (0,0,1) but is not used because no standard orientation has the head pointing sideways or down

		public static ImageOrientationPatient Empty = new ImageOrientationPatient();
		public static ImageOrientationPatient AxialRight = new ImageOrientationPatient(_right, _posterior);
		public static ImageOrientationPatient AxialLeft = new ImageOrientationPatient(_left, _posterior);
		public static ImageOrientationPatient SaggittalPosterior = new ImageOrientationPatient(_posterior, _foot);
		public static ImageOrientationPatient SaggittalAnterior = new ImageOrientationPatient(_anterior, _foot);
		public static ImageOrientationPatient CoronalRight = new ImageOrientationPatient(_right, _foot);
		public static ImageOrientationPatient CoronalLeft = new ImageOrientationPatient(_left, _foot);

		#endregion

		#region Private Members

		private int _primaryRowDirection;
		private int _secondaryRowDirection;
		private int _primaryColumnDirection;
		private int _secondaryColumnDirection;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="ImageOrientationPatient"/> from the direction cosines of the row and column orientations.
		/// </summary>
		/// <remarks>
		/// Another way to think of direction cosine vectors is that they are simply the unit vectors in the
		/// patient coordinate system describing the orientation of a row and a column in the image.
		/// </remarks>
		/// <param name="rowX">The X component of the row direction cosine vector.</param>
		/// <param name="rowY">The Y component of the row direction cosine vector.</param>
		/// <param name="rowZ">The Z component of the row direction cosine vector.</param>
		/// <param name="columnX">The X component of the column direction cosine vector.</param>
		/// <param name="columnY">The Y component of the column direction cosine vector.</param>
		/// <param name="columnZ">The Z component of the column direction cosine vector.</param>
		public ImageOrientationPatient(double rowX, double rowY, double rowZ, double columnX, double columnY, double columnZ)
		{
			RowX = rowX;
			RowY = rowY;
			RowZ = rowZ;
			ColumnX = columnX;
			ColumnY = columnY;
			ColumnZ = columnZ;
			Recalculate();
		}

		/// <summary>
		/// Private constructor.
		/// </summary>
		private ImageOrientationPatient(int[] rowCosines, int[] columnCosines)
		{
			RowX = rowCosines[0];
			RowY = rowCosines[1];
			RowZ = rowCosines[2];

			ColumnX = columnCosines[0];
			ColumnY = columnCosines[1];
			ColumnZ = columnCosines[2];
		}

		/// <summary>
		/// private constructor.
		/// </summary>
		private ImageOrientationPatient()
		{
			RowX = RowY = RowZ = ColumnX = ColumnY = ColumnZ = 0;
			Recalculate();
		}

		#region Public Properties

		/// <summary>
		/// Gets whether or not this object represents a NULL value.
		/// </summary>
		public bool IsNull
		{
			get
			{
// ReSharper disable CompareOfFloatsByEqualityOperator
				return (RowX == 0 && RowY == 0 && RowZ == 0) ||
				       (ColumnX == 0 && ColumnY == 0 && ColumnZ == 0);
// ReSharper restore CompareOfFloatsByEqualityOperator
			}
		}

		/// <summary>
		/// Gets the X component of the direction cosine for the first row in the image.
		/// </summary>
		public double RowX { get; private set; }

		/// <summary>
		/// Gets the Y component of the direction cosine for the first row in the image.
		/// </summary>
		public double RowY { get; private set; }

		/// <summary>
		/// Gets the Z component of the direction cosine for the first row in the image.
		/// </summary>
		public double RowZ { get; private set; }

		/// <summary>
		/// Gets the X component of the direction cosine for the first column in the image.
		/// </summary>
		public double ColumnX { get; private set; }

		/// <summary>
		/// Gets the Y component of the direction cosine for the first column in the image.
		/// </summary>
		public double ColumnY { get; private set; }

		/// <summary>
		/// Gets the Z component of the direction cosine for the first column in the image.
		/// </summary>
		public double ColumnZ { get; private set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the effective <see cref="PatientOrientation"/> that describes the image orientation.
		/// </summary>
		/// <returns>A <see cref="PatientOrientation"/> describing the primary and secondary directions of the image orientation.</returns>
		public PatientOrientation ToPatientOrientation()
		{
			return !IsNull ? new PatientOrientation(PrimaryRow + (PatientDirection) SecondaryRow, PrimaryColumn + (PatientDirection) SecondaryColumn) : PatientOrientation.Empty;
		}

		/// <summary>
		/// Gets a string suitable for direct insertion into a <see cref="DicomAttributeMultiValueText"/> attribute.
		/// </summary>
		public override string ToString()
		{
			return String.Format(@"{0:G12}\{1:G12}\{2:G12}\{3:G12}\{4:G12}\{5:G12}", RowX, RowY, RowZ, ColumnX, ColumnY, ColumnZ);
		}

		/// <summary>
		/// Creates an <see cref="ImageOrientationPatient"/> object from a DICOM multi-valued string.
		/// </summary>
		/// <param name="multiValuedString">A DICOM multi-valued string containing the 6 components of the row and column cosine vectors.</param>
		/// <returns>
		/// Returns NULL if there are not exactly six parseable values in the input string.
		/// </returns>
		public static ImageOrientationPatient FromString(string multiValuedString)
		{
			if (string.IsNullOrEmpty(multiValuedString)) return null;

			double[] values;
			if (DicomStringHelper.TryGetDoubleArray(multiValuedString, out values) && values.Length == 6)
				return new ImageOrientationPatient(values[0], values[1], values[2], values[3], values[4], values[5]);
			return null;
		}

		/// <summary>
		/// Gets the primary patient direction of the first row of the image (increasing X).
		/// </summary>
		public Directions PrimaryRow
		{
			get { return (Directions) _primaryRowDirection; }
		}

		/// <summary>
		/// Gets the primary patient direction of the first column of the image (increasing Y).
		/// </summary>
		public Directions PrimaryColumn
		{
			get { return (Directions) _primaryColumnDirection; }
		}

		/// <summary>
		/// Gets the secondary patient direction of the first row of the image (increasing X).
		/// </summary>
		public Directions SecondaryRow
		{
			get { return (Directions) _secondaryRowDirection; }
		}

		/// <summary>
		/// Gets the secondary patient direction of the first column of the image (increasing Y).
		/// </summary>
		public Directions SecondaryColumn
		{
			get { return (Directions) _secondaryColumnDirection; }
		}

		/// <summary>
		/// Gets the primary patient direction of the first row of the image (increasing X).
		/// </summary>
		/// <param name="opposingDirection">Indicates that the opposite of the primary direction should be returned.
		/// (e.g. if the direction is <see cref="Directions.Anterior"/>, the opposite would be <see cref="Directions.Posterior"/>).</param>
		/// <returns>The primary direction of the row in terms of the patient coordinate system.</returns>
		public Directions GetPrimaryRowDirection(bool opposingDirection)
		{
			return (Directions) (_primaryRowDirection*(opposingDirection ? -1 : 1));
		}

		/// <summary>
		/// Gets the primary patient direction of the first column of the image (increasing Y).
		/// </summary>
		/// <param name="opposingDirection">Indicates that the opposite of the primary direction should be returned.
		/// (e.g. if the direction is <see cref="Directions.Anterior"/>, the opposite would be <see cref="Directions.Posterior"/>).</param>
		/// <returns>The primary direction of the column in terms of the patient coordinate system.</returns>
		public Directions GetPrimaryColumnDirection(bool opposingDirection)
		{
			return (Directions) (_primaryColumnDirection*(opposingDirection ? -1 : 1));
		}

		/// <summary>
		/// Gets the secondary patient direction of the first row of the image (increasing X).
		/// </summary>
		/// <returns>The secondary direction of the row in terms of the patient coordinate system.</returns>
		public Directions GetSecondaryRowDirection()
		{
			return GetSecondaryRowDirection(false);
		}

		/// <summary>
		/// Gets the secondary patient direction of the first row of the image (increasing X).
		/// </summary>
		/// <param name="degreesTolerance">Angle of tolerance in degrees (i.e. if the direction is actually within the specified
		/// angle of being perpendicular to the secondary direction, the result will be <see cref="Directions.None"/> instead).</param>
		/// <returns>The secondary direction of the row in terms of the patient coordinate system.</returns>
		public Directions GetSecondaryRowDirection(double degreesTolerance)
		{
			return GetSecondaryRowDirection(false, degreesTolerance);
		}

		/// <summary>
		/// Gets the secondary patient direction of the first row of the image (increasing X).
		/// </summary>
		/// <param name="opposingDirection">Indicates that the opposite of the secondary direction should be returned.
		/// (e.g. if the direction is <see cref="Directions.Anterior"/>, the opposite would be <see cref="Directions.Posterior"/>).</param>
		/// <returns>The secondary direction of the row in terms of the patient coordinate system.</returns>
		public Directions GetSecondaryRowDirection(bool opposingDirection)
		{
			return (Directions) (_secondaryRowDirection*(opposingDirection ? -1 : 1));
		}

		//TODO (CR June 2011): Need tertiary?

		/// <summary>
		/// Gets the secondary patient direction of the first row of the image (increasing X).
		/// </summary>
		/// <param name="opposingDirection">Indicates that the opposite of the secondary direction should be returned.
		/// (e.g. if the direction is <see cref="Directions.Anterior"/>, the opposite would be <see cref="Directions.Posterior"/>).</param>
		/// <param name="degreesTolerance">Angle of tolerance in degrees (i.e. if the direction is actually within the specified
		/// angle of being perpendicular to the secondary direction, the result will be <see cref="Directions.None"/> instead).</param>
		/// <returns>The secondary direction of the row in terms of the patient coordinate system.</returns>
		public Directions GetSecondaryRowDirection(bool opposingDirection, double degreesTolerance)
		{
			// JY: Note that the same tolerance functionality is purposefully not available to the primary direction
			//TODO (CR June 2011): why 10?
			if (degreesTolerance < 0 || degreesTolerance > 10)
			{
				const string msg = "Value must be between 0 and 10.";
				throw new ArgumentOutOfRangeException("degreesTolerance", degreesTolerance, msg);
			}

			if (_secondaryRowDirection == 0)
				return Directions.None;

			var rowCosines = GetRowCosines();
			double secondaryCosine = rowCosines[Math.Abs(_secondaryRowDirection) - 1];

			// report no secondary direction if the secondary cosine is within degrees of 90 - that is, nearly perpendicular to the given direction
			if (90 - 180*Math.Acos(Math.Abs(secondaryCosine))/Math.PI < degreesTolerance)
				return Directions.None;
			return GetSecondaryRowDirection(opposingDirection);
		}

		/// <summary>
		/// Gets the secondary patient direction of the first column of the image (increasing Y).
		/// </summary>
		/// <returns>The secondary direction of the column in terms of the patient coordinate system.</returns>
		public Directions GetSecondaryColumnDirection()
		{
			return GetSecondaryColumnDirection(false);
		}

		/// <summary>
		/// Gets the secondary patient direction of the first column of the image (increasing Y).
		/// </summary>
		/// <param name="degreesTolerance">Angle of tolerance in degrees (i.e. if the direction is actually within the specified
		/// angle of being perpendicular to the secondary direction, the result will be <see cref="Directions.None"/> instead).</param>
		/// <returns>The secondary direction of the column in terms of the patient coordinate system.</returns>
		public Directions GetSecondaryColumnDirection(double degreesTolerance)
		{
			return GetSecondaryColumnDirection(false, degreesTolerance);
		}

		/// <summary>
		/// Gets the secondary patient direction of the first column of the image (increasing Y).
		/// </summary>
		/// <param name="opposingDirection">Indicates that the opposite of the secondary direction should be returned.
		/// (e.g. if the direction is <see cref="Directions.Anterior"/>, the opposite would be <see cref="Directions.Posterior"/>).</param>
		/// <returns>The secondary direction of the column in terms of the patient coordinate system.</returns>
		public Directions GetSecondaryColumnDirection(bool opposingDirection)
		{
			return (Directions) (_secondaryColumnDirection*(opposingDirection ? -1 : 1));
		}

		//TODO (CR June 2011): Need tertiary?

		/// <summary>
		/// Gets the secondary patient direction of the first column of the image (increasing Y).
		/// </summary>
		/// <param name="opposingDirection">Indicates that the opposite of the secondary direction should be returned.
		/// (e.g. if the direction is <see cref="Directions.Anterior"/>, the opposite would be <see cref="Directions.Posterior"/>).</param>
		/// <param name="degreesTolerance">Angle of tolerance in degrees (i.e. if the direction is actually within the specified
		/// angle of being perpendicular to the secondary direction, the result will be <see cref="Directions.None"/> instead).</param>
		/// <returns>The secondary direction of the column in terms of the patient coordinate system.</returns>
		public Directions GetSecondaryColumnDirection(bool opposingDirection, double degreesTolerance)
		{
			// JY: Note that the same tolerance functionality is purposefully not available to the primary direction
			//TODO (CR June 2011): why 10?
			if (degreesTolerance < 0 || degreesTolerance > 10)
			{
				const string msg = "Value must be between 0 and 10.";
				throw new ArgumentOutOfRangeException("degreesTolerance", degreesTolerance, msg);
			}

			if (_secondaryColumnDirection == 0)
				return Directions.None;

			var columnCosines = GetColumnCosines();
			double secondaryCosine = columnCosines[Math.Abs(_secondaryColumnDirection) - 1];

			// report no secondary direction if the secondary cosine is within degrees of 90 - that is, nearly perpendicular to the given direction
			if (90 - 180*Math.Acos(Math.Abs(secondaryCosine))/Math.PI < degreesTolerance)
				return Directions.None;
			return GetSecondaryColumnDirection(opposingDirection);
		}

		#region IEquatable<ImageOrientationPatient> Members

		public bool Equals(ImageOrientationPatient other)
		{
			if (other == null)
				return false;

// ReSharper disable CompareOfFloatsByEqualityOperator
			return other.RowX == RowX && other.RowY == RowY && other.RowZ == RowZ &&
			       other.ColumnX == ColumnX && other.ColumnY == ColumnY && other.ColumnZ == ColumnZ;
// ReSharper restore CompareOfFloatsByEqualityOperator
		}

		#endregion

		public override bool Equals(object obj)
		{
			return obj != null && Equals(obj as ImageOrientationPatient);
		}

		/// <summary>
		/// Determines if this <see cref="ImageOrientationPatient"/> is equal to another orientation
		/// within the specified tolerance.
		/// </summary>
		/// <param name="other">The other <see cref="ImageOrientationPatient"/> with which to compare.</param>
		/// <param name="withinTolerance">The absolute tolerance of each component of the orientation cosine vectors.</param>
		/// <returns>True if they are considered equal; False otherwise.</returns>
		public bool EqualsWithinTolerance(ImageOrientationPatient other, float withinTolerance)
		{
			// TODO CR (26 Sep 2013): This determines equality with an absolute tolerance - a more useful comparison might be one with an angular tolerance
			if (other == null)
				return false;

			return EqualsWithinTolerance(other.RowX, RowX, withinTolerance) &&
			       EqualsWithinTolerance(other.RowY, RowY, withinTolerance) &&
			       EqualsWithinTolerance(other.RowZ, RowZ, withinTolerance) &&
			       EqualsWithinTolerance(other.ColumnX, ColumnX, withinTolerance) &&
			       EqualsWithinTolerance(other.ColumnY, ColumnY, withinTolerance) &&
			       EqualsWithinTolerance(other.ColumnZ, ColumnZ, withinTolerance);
		}

		private static bool EqualsWithinTolerance(double d1, double d2, float tolerance)
		{
			return Math.Abs(d1 - d2) < tolerance;
		}

		public override int GetHashCode()
		{
			return 0x6033CEC7; // use a constant value because the real values are mutable and otherwise certain equality comparisons won't work
		}

		#endregion

		/// <summary>
		/// Gets the row cosines as an array of doubles (x,y,z).
		/// </summary>
		private double[] GetRowCosines()
		{
			return new[] {RowX, RowY, RowZ};
		}

		/// <summary>
		/// Gets the column cosines as an array of doubles (x,y,z).
		/// </summary>
		private double[] GetColumnCosines()
		{
			return new[] {ColumnX, ColumnY, ColumnZ};
		}

		/// <summary>
		/// Recalculates the primary/secondary directions (in patient based system) for the first row and first column.
		/// </summary>
		private void Recalculate()
		{
			var rowCosines = GetRowCosines();
			var columnCosines = GetColumnCosines();

			int[] rowCosineSortedIndices = BubbleSortCosineIndices(rowCosines);
			int[] columnCosineSortedIndices = BubbleSortCosineIndices(columnCosines);

			SetDirectionValue(out _primaryRowDirection, rowCosines[rowCosineSortedIndices[0]], rowCosineSortedIndices[0]);
			SetDirectionValue(out _secondaryRowDirection, rowCosines[rowCosineSortedIndices[1]], rowCosineSortedIndices[1]);
			SetDirectionValue(out _primaryColumnDirection, columnCosines[columnCosineSortedIndices[0]], columnCosineSortedIndices[0]);
			SetDirectionValue(out _secondaryColumnDirection, columnCosines[columnCosineSortedIndices[1]], columnCosineSortedIndices[1]);
		}

		/// <summary>
		/// Sets one of the member primary/secondary direction variables.
		/// </summary>
		/// <param name="member">the member to set</param>
		/// <param name="cosineValue">the cosine value</param>
		/// <param name="cosineIndex">the index of the cosine value in the original direction cosine vector</param>
		private static void SetDirectionValue(out int member, double cosineValue, int cosineIndex)
		{
			member = Math.Abs(cosineValue) > float.Epsilon ? (cosineIndex + 1)*Math.Sign(cosineValue) : 0;
		}

		/// <summary>
		/// Bubble sorts an array of cosines in descending order (largest to smallest), ignoring the sign.
		/// This helps to determine the primary/secondary directions for the cosines.
		/// </summary>
		/// <param name="cosineArray">the array of cosines (row or column)</param>
		/// <returns>an array of indices into the input array (cosineArray), that when applied would sort the cosines appropriately.</returns>
		private static int[] BubbleSortCosineIndices(double[] cosineArray)
		{
			var indexArray = new[] {0, 1, 2};

			for (int i = 2; i > 0; --i)
			{
				for (int j = 0; j < i; ++j)
				{
					if (Math.Abs(cosineArray[indexArray[j + 1]]) > Math.Abs(cosineArray[indexArray[j]]))
					{
						int tempint = indexArray[j];
						indexArray[j] = indexArray[j + 1];
						indexArray[j + 1] = tempint;
					}
				}
			}

			return indexArray;
		}
	}
}
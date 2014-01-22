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
	/// Represents the orientation of the image in the patient using dicom enumerated values
	/// to indicate the direction of the first row and column in the image.
	/// </summary>
	public class PatientOrientation : IEquatable<PatientOrientation>
	{
		public static PatientOrientation Empty = new PatientOrientation(String.Empty, String.Empty);
		public static PatientOrientation AxialRight = new PatientOrientation(PatientDirection.Right, PatientDirection.Posterior);
		public static PatientOrientation AxialLeft = new PatientOrientation(PatientDirection.Left, PatientDirection.Posterior);
		public static PatientOrientation SaggittalPosterior = new PatientOrientation(PatientDirection.Posterior, PatientDirection.Foot);
		public static PatientOrientation SaggittalAnterior = new PatientOrientation(PatientDirection.Anterior, PatientDirection.Foot);
		public static PatientOrientation CoronalRight = new PatientOrientation(PatientDirection.Right, PatientDirection.Foot);
		public static PatientOrientation CoronalLeft = new PatientOrientation(PatientDirection.Left, PatientDirection.Foot);

		public PatientOrientation(PatientDirection row, PatientDirection column)
		{
			Row = new PatientDirection(row);
			Column = new PatientDirection(column);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public PatientOrientation(string row, string column)
			: this(row, column, AnatomicalOrientationType.Biped) {}

		/// <summary>
		/// Constructor.
		/// </summary>
		public PatientOrientation(string row, string column, AnatomicalOrientationType anatomicalOrientationType)
			: this(new PatientDirection(row, anatomicalOrientationType), new PatientDirection(column, anatomicalOrientationType)) {}

		/// <summary>
		/// Constructor.
		/// </summary>
		public PatientOrientation(string row, string column, string anatomicalOrientationType)
			: this(new PatientDirection(row, ParseAnatomicalOrientationType(anatomicalOrientationType)), new PatientDirection(column, ParseAnatomicalOrientationType(anatomicalOrientationType))) {}

		private static AnatomicalOrientationType ParseAnatomicalOrientationType(string anatomicalOrientationType)
		{
			return IodBase.ParseEnum(anatomicalOrientationType, AnatomicalOrientationType.None);
		}

		#region Public Properties

		/// <summary>
		/// Gets whether or not this <see cref="PatientOrientation"/> object is empty.
		/// </summary>
		public bool IsEmpty
		{
			get { return Row.IsEmpty && Column.IsEmpty; }
		}

		public bool IsValid
		{
			get
			{
				//Both unspecified is not valid.
				if (Row.Equals(Column))
					return false;

				if (!Row.IsValid || !Column.IsValid)
					return false;

				//We could do something with vectors and check they are "orthogonal", but this is pretty good.
				for (int i = 0; i < Math.Max(Row.ComponentCount, Column.ComponentCount); ++i)
				{
					//They can both point to the same direction in the patient, but
					//they can't both have "primary component == Foot", for example
					//or event have primary components along the same axis (e.g. Right and Left).
					var component = (PatientDirection.Component) i;
					var row = Row[component];
					var column = Column[component];
					if (row.Equals(column))
						return false;

					if (row.Equals(column.OpposingDirection))
						return false;
				}

				return true;
			}
		}

		/// <summary>
		/// Gets the direction of the first row in the image.
		/// </summary>
		public virtual PatientDirection Row { get; private set; }

		/// <summary>
		/// Gets the direction of the first column in the image.
		/// </summary>
		public virtual PatientDirection Column { get; private set; }

		/// <summary>
		/// Gets the primary direction of the first row in the image.
		/// </summary>
		public PatientDirection PrimaryRow
		{
			get { return Row.Primary; }
		}

		/// <summary>
		/// Gets the primary direction of the first column in the image.
		/// </summary>
		public PatientDirection PrimaryColumn
		{
			get { return Column.Primary; }
		}

		/// <summary>
		/// Gets the secondary direction of the first row in the image.
		/// </summary>
		public PatientDirection SecondaryRow
		{
			get { return Row.Secondary; }
		}

		/// <summary>
		/// Gets the secondary direction of the first column in the image.
		/// </summary>
		public PatientDirection SecondaryColumn
		{
			get { return Column.Secondary; }
		}

		/// <summary>
		/// Gets the tertiary direction of the first row in the image.
		/// </summary>
		public PatientDirection TertiaryRow
		{
			get { return Row.Tertiary; }
		}

		/// <summary>
		/// Gets the tertiary direction of the first column in the image.
		/// </summary>
		public PatientDirection TertiaryColumn
		{
			get { return Column.Tertiary; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets a string suitable for direct insertion into a <see cref="DicomAttributeMultiValueText"/> attribute.
		/// </summary>
		public override string ToString()
		{
			if (Row.IsEmpty && Column.IsEmpty)
				return String.Empty;

			return String.Format(@"{0}\{1}", Row, Column);
		}

		/// <summary>
		/// Parses a <see cref="PatientOrientation"/> from a DICOM multi-valued string.
		/// </summary>
		/// <param name="multiValuedString">Patient orientation, defined in orientation of rows and orientation of columns, separated by a backslash.</param>
		/// <returns>
		/// NULL if there are not exactly 2 parsed values in the input string.
		/// </returns>
		public static PatientOrientation FromString(string multiValuedString)
		{
			if (string.IsNullOrEmpty(multiValuedString)) return null;
			var values = DicomStringHelper.GetStringArray(multiValuedString);
			return values != null && values.Length == 2 ? new PatientOrientation(values[0], values[1]) : null;
		}

		/// <summary>
		/// Parses a <see cref="PatientOrientation"/> from a DICOM multi-valued string.
		/// </summary>
		/// <param name="multiValuedString">Patient orientation, defined in orientation of rows and orientation of columns, separated by a backslash.</param>
		/// <param name="anatomicalOrientationType">The anatomical orientation type code.</param>
		/// <returns>
		/// NULL if there are not exactly 2 parsed values in the input string.
		/// </returns>
		public static PatientOrientation FromString(string multiValuedString, string anatomicalOrientationType)
		{
			if (string.IsNullOrEmpty(multiValuedString)) return null;
			var values = DicomStringHelper.GetStringArray(multiValuedString);
			return values != null && values.Length == 2 ? new PatientOrientation(values[0], values[1], anatomicalOrientationType) : null;
		}

		#region IEquatable<PatientOrientation> Members

		public bool Equals(PatientOrientation other)
		{
			if (other == null)
				return false;

			return other.Row == Row && other.Column == Column;
		}

		#endregion

		public override bool Equals(object obj)
		{
			return obj != null && Equals(obj as PatientOrientation);
		}

		public override int GetHashCode()
		{
			return -0x7360AA3E;
		}

		#endregion
	}
}
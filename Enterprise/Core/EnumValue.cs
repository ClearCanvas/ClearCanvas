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

using ClearCanvas.Enterprise.Core.Modelling;

namespace ClearCanvas.Enterprise.Core
{
	[DeactivationFlag("Deactivated")]
	public class EnumValue : DomainObject
	{
		// these are the values we have been using in the Hibernate mapping files
		public const int CodeLength = 255;  // default SQL server varchar
		public const int ValueLength = 50;
		public const int DescriptionLength = 200;

		private string _code;
		private string _value;
		private string _description;
		private float _displayOrder;
		private bool _deactivated;

		protected EnumValue()
		{
		}

		/// <summary>
		/// This constructor is needed for unit tests, to create fake enum values.
		/// </summary>
		/// <param name="code"></param>
		/// <param name="value"></param>
		/// <param name="description"></param>
		public EnumValue(string code, string value, string description)
		{
			_code = code;
			_value = value;
			_description = description;
		}

		/// <summary>
		/// Gets the code.
		/// </summary>
		[Required]
		[Length(CodeLength)]
		public virtual string Code
		{
			get { return _code; }
			protected set { _code = value; }
		}

		/// <summary>
		/// Gets the display value.
		/// </summary>
		[Required]
		[Length(ValueLength)]
		public virtual string Value
		{
			get { return _value; }
			protected set { _value = value; }
		}

		/// <summary>
		/// Gets the description
		/// </summary>
		[Length(DescriptionLength)]
		public virtual string Description
		{
			get { return _description; }
			protected set { _description = value; }
		}

		/// <summary>
		/// Gets a value providing a relative position for sorting the enumeration.
		/// </summary>
		public virtual float DisplayOrder
		{
			get { return _displayOrder; }
			protected set { _displayOrder = value; }
		}

		/// <summary>
		/// Gets a value indicating whether this enumerated value has been de-activated.
		/// </summary>
		public virtual bool Deactivated
		{
			get { return _deactivated; }
			protected set { _deactivated = value; }
		}

		/// <summary>
		/// Overridden to provide value-based hash code
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return _code.GetHashCode();
		}

		/// <summary>
		/// Overridden to provide value-based equality.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, this))
				return true;
			var other = obj as EnumValue;
			if (other == null)
				return false;

			// must be of same class, in addition to having same code, since codes are not guaranteed
			// unique across classes
			return (other.GetClass() == this.GetClass()) && other.Code == this.Code;
		}

	}
}

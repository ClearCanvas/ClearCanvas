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

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Represents a change made to a property value of an entity.
	/// </summary>
	public class PropertyChange
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="isCollection"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		public PropertyChange(string propertyName, bool isCollection, object oldValue, object newValue)
		{
			PropertyName = propertyName;
			OldValue = oldValue;
			NewValue = newValue;
			IsCollection = isCollection;
		}

		/// <summary>
		/// Gets the name of the property.
		/// </summary>
		public string PropertyName { get; private set; }

		/// <summary>
		/// Gets the old value of the property.
		/// </summary>
		public object OldValue { get; private set; }

		/// <summary>
		/// Gets the new value of the property.
		/// </summary>
		public object NewValue { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the property is a collection property.
		/// </summary>
		public bool IsCollection { get; private set; }

		/// <summary>
		/// Returns a new <see cref="PropertyChange"/> that is the result of adding this change
		/// to <paramref name="previousChange"/>.
		/// </summary>
		/// <param name="previousChange"></param>
		/// <returns></returns>
		/// <remarks>
		/// This operation is not commutative.
		/// </remarks>
		public PropertyChange AddTo(PropertyChange previousChange)
		{
			return new PropertyChange(PropertyName, IsCollection, previousChange.OldValue, NewValue);
		}
	}
}

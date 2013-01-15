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

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines order note categories.  These categories are entirely in the domain of the client -
	/// the server is ignorant of the order note categories.
	/// </summary>
	public class OrderNoteCategory : IEquatable<OrderNoteCategory>
	{
		/// <summary>
		/// General category.
		/// </summary>
		public static readonly OrderNoteCategory General = new OrderNoteCategory("General", SR.OrderNoteCategoryGeneral);

		/// <summary>
		/// Preliminary Diagnosis category.
		/// </summary>
		public static readonly OrderNoteCategory PreliminaryDiagnosis = new OrderNoteCategory("PrelimDiagnosis", SR.OrderNoteCategoryPrelimDiagnosis);

		/// <summary>
		/// Preliminary Diagnosis category.
		/// </summary>
		public static readonly OrderNoteCategory Protocol = new OrderNoteCategory("Protocol", SR.OrderNoteCategoryProtocol);

		/// <summary>
		/// Gets the <see cref="OrderNoteCategory"/> object corresponding to the specified key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static OrderNoteCategory FromKey(string key)
		{
			if (key.Equals(General.Key, StringComparison.InvariantCultureIgnoreCase))
				return General;

			if (key.Equals(PreliminaryDiagnosis.Key, StringComparison.InvariantCultureIgnoreCase))
				return PreliminaryDiagnosis;

			throw new ArgumentOutOfRangeException("key");
		}


		private readonly string _key;
		private readonly string _displayValue;

		private OrderNoteCategory(string key, string displayValue)
		{
			_key = key;
			_displayValue = displayValue;
		}

		/// <summary>
		/// Gets the key.
		/// </summary>
		public string Key
		{
			get { return _key; }
		}

		/// <summary>
		/// Gets the display value.
		/// </summary>
		public string DisplayValue
		{
			get { return _displayValue; }
		}


		#region Equality Operators


		public static bool operator !=(OrderNoteCategory orderNoteCategory1, OrderNoteCategory orderNoteCategory2)
		{
			return !Equals(orderNoteCategory1, orderNoteCategory2);
		}

		public static bool operator ==(OrderNoteCategory orderNoteCategory1, OrderNoteCategory orderNoteCategory2)
		{
			return Equals(orderNoteCategory1, orderNoteCategory2);
		}

		public bool Equals(OrderNoteCategory orderNoteCategory)
		{
			if (orderNoteCategory == null) return false;
			return Equals(_key, orderNoteCategory._key);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as OrderNoteCategory);
		}

		public override int GetHashCode()
		{
			return _key.GetHashCode();
		}

		#endregion
	}
}

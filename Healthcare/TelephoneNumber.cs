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

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// TelephoneNumber component
	/// </summary>
	public partial class TelephoneNumber : IFormattable
	{
		private void CustomInitialize()
		{
		}

		public bool IsCurrent
		{
			get { return this.ValidRange == null || this.ValidRange.Includes(Platform.Time); }
		}

		/// <summary>
		/// Equivalence comparison which ignores ValidRange
		/// </summary>
		/// <param name="that">The TelephoneNumber to compare to</param>
		/// <returns>True if all fields other than the validity range are the same, False otherwise</returns>
		public bool IsSameNumber(TelephoneNumber that)
		{
			return (that != null) &&
				((this._countryCode == default(string)) ? (that._countryCode == default(string)) : this._countryCode.Equals(that._countryCode)) &&
				((this._areaCode == default(string)) ? (that._areaCode == default(string)) : this._areaCode.Equals(that._areaCode)) &&
				((this._number == default(string)) ? (that._number == default(string)) : this._number.Equals(that._number)) &&
				((this._extension == default(string)) ? (that._extension == default(string)) : this._extension.Equals(that._extension)) &&
				((this._use == default(TelephoneUse)) ? (that._use == default(TelephoneUse)) : this._use.Equals(that._use)) &&
				((this._equipment == default(TelephoneEquipment)) ? (that._equipment == default(TelephoneEquipment)) : this._equipment.Equals(that._equipment)) &&
				true;
		}

		#region IFormattable Members

		public string ToString(string format, IFormatProvider formatProvider)
		{
			// TODO interpret the format string according to custom-defined format characters
			var sb = new StringBuilder();
			sb.AppendFormat("{0} ", _countryCode);
			if (!String.IsNullOrEmpty(_areaCode))
			{
				sb.AppendFormat("({0}) ", _areaCode);
			}
			if (!String.IsNullOrEmpty(_number) && _number.Length == 7)
			{
				sb.AppendFormat("{0}-{1}", _number.Substring(0, 3), _number.Substring(3,4));
			}
			else
			{
				sb.AppendFormat("{0}", _number);
			}
			if (!String.IsNullOrEmpty(_extension))
			{
				sb.Append(" x");
				sb.Append(_extension);
			}
			return sb.ToString();
		}

		#endregion

		public override string ToString()
		{
			return this.ToString(null, null);
		}
	}
}
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
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Externals.General
{
	public interface IArgumentHint : IDisposable
	{
		ArgumentHintValue this[string key] { get; }
	}

	public struct ArgumentHintValue : IEquatable<ArgumentHintValue>, IComparable<ArgumentHintValue>
	{
		private readonly string[] _values;

		public ArgumentHintValue(string value)
		{
			_values = null;
			if (value != null)
			{
				_values = new string[] {value};
			}
		}

		public ArgumentHintValue(string[] values)
		{
			_values = null;
			if (values != null && values.Length > 0)
			{
				_values = new string[values.Length];
				values.CopyTo(_values, 0);
			}
		}

		public ArgumentHintValue(IEnumerable<string> values)
		{
			_values = null;
			if (values != null)
			{
				List<string> valueList = new List<string>(values);
				if (valueList.Count > 0)
					_values = valueList.ToArray();
			}
		}

		public bool IsMultiValued
		{
			get { return _values != null && _values.Length > 1; }
		}

		public bool IsNull
		{
			get { return _values == null; }
		}

		public int Count
		{
			get
			{
				if (_values == null)
					return 0;
				return _values.Length;
			}
		}

		public override int GetHashCode()
		{
			int hashCode = 0x1925C138;
			if (_values != null)
			{
				foreach (string value in _values)
				{
					if (value != null)
						hashCode ^= value.GetHashCode();
					else
						hashCode ^= 0x229A3E06;
				}
			}
			return hashCode;
		}

		public override bool Equals(object obj)
		{
			if (obj is ArgumentHintValue)
				return this.Equals((ArgumentHintValue) obj);
			return false;
		}

		public bool Equals(ArgumentHintValue other)
		{
			if (ReferenceEquals(_values, other._values) || (_values == null && other._values == null))
				return true;
			if (_values == null || other._values == null)
				return false;

			int small = Math.Min(_values.Length, other._values.Length);
			for (int n = 0; n < small; n++)
			{
				int result = string.Compare(_values[n], other._values[n]);
				if (result != 0)
					return false;
			}

			return _values.Length.Equals(other._values.Length);
		}

		public int CompareTo(ArgumentHintValue other)
		{
			if (ReferenceEquals(_values, other._values) || (_values == null && other._values == null))
				return 0;
			if (_values == null)
				return -1;
			if (other._values == null)
				return +1;

			int small = Math.Min(_values.Length, other._values.Length);
			for (int n = 0; n < small; n++)
			{
				int result = string.Compare(_values[n], other._values[n]);
				if (result != 0)
					return result;
			}

			return _values.Length.CompareTo(other._values.Length);
		}

		public override string ToString()
		{
			if (this.IsNull)
				return string.Empty;
			if (this.IsMultiValued)
				return _values.ToString();
			return _values[0];
		}

		public string ToString(string multiValueSeparator)
		{
			if (this.IsNull)
				return string.Empty;
			return string.Join(multiValueSeparator, _values);
		}

		public static implicit operator string(ArgumentHintValue value)
		{
			if (value._values == null)
				return null;
			return value._values[0];
		}

		public static implicit operator string[](ArgumentHintValue value)
		{
			return value._values;
		}

		public static readonly ArgumentHintValue Empty = new ArgumentHintValue();
	}
}
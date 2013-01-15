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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public class DicomArray<T>
		: IComparable<DicomArray<T>>, IComparable, IEquatable<DicomArray<T>>, IEnumerable<T?>
		where T : struct, IComparable<T>, IEquatable<T>
	{
		public delegate bool ElementParserDelegate(string s, out T result);

		private readonly IList<T?> _values;
		private readonly string _displayString;

		public DicomArray()
		{
			_values = new List<T?>().AsReadOnly();
			_displayString = string.Empty;
		}

		public DicomArray(params T?[] values) : this(values, null) {}

		public DicomArray(IEnumerable<T?> values) : this(values, null) {}

		public DicomArray(IEnumerable<T?> values, string displayString)
		{
			Platform.CheckForNullReference(values, "values");
			_values = new List<T?>(values).AsReadOnly();

			if (displayString != null)
				_displayString = displayString;
			else
				_displayString = FormatArray(values);
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
				return 1;
			if (obj is DicomArray<T>)
				return this.CompareTo((DicomArray<T>) obj);
			throw new ArgumentException(string.Format("Parameter must be a DicomArray<{0}>.", typeof(T).Name), "obj");
		}

		public int CompareTo(DicomArray<T> other)
		{
			return Compare(this, other, CompareElement);
		}

		public bool Equals(DicomArray<T> other)
		{
			return this.CompareTo(other) == 0;
		}

		public override bool Equals(object obj)
		{
			if (obj is DicomArray<T>)
				return this.Equals((DicomArray<T>) obj);
			return false;
		}

		public override int GetHashCode()
		{
			int hashcode = 0x6A25AC99 ^ typeof (T).GetHashCode();
			foreach (T? t in _values)
			{
				if (t == null)
					hashcode ^= -0x15788718;
				else
					hashcode ^= t.GetHashCode();
			}
			return hashcode;
		}

		public override string ToString()
		{
			return _displayString;
		}

		public static DicomArray<T> Parse(string s, ElementParserDelegate elementParser)
		{
			DicomArray<T> array;
			if (TryParse(s, elementParser, out array))
				return array;
			throw new FormatException("Parameter was not in the expected format.");
		}

		public static bool TryParse(string s, ElementParserDelegate elementParser, out DicomArray<T> result)
		{
			Platform.CheckForNullReference(elementParser, "elementParser");

			if (s != null)
			{
				if (s.Length == 0)
				{
					result = new DicomArray<T>();
					return true;
				}

				int countParserAttempts = 0;
				int countParserFailures = 0;

				List<T?> list = new List<T?>();
				foreach (string elementString in s.Split(new char[] { '\\' }, StringSplitOptions.None))
				{
					if (string.IsNullOrEmpty(elementString))
					{
						list.Add(null);
					}
					else
					{
						countParserAttempts++;

						T element;
						if (elementParser(elementString, out element))
						{
							list.Add(element);
						}
						else
						{
							list.Add(null);
							countParserFailures++;
						}
					}
				}

				// the overall parse succeeds if every non-empty element parsed succeeded (or if everything was empty and nothing was parsed)
				if (countParserAttempts == 0 || countParserAttempts != countParserFailures)
				{
					result = new DicomArray<T>(list);
					return true;
				}
			}
			result = null;
			return false;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();	
		}

		public IEnumerator<T?> GetEnumerator()
		{
			foreach (T? t in _values)
				yield return t;
		}

		private static string FormatArray(IEnumerable<T?> input)
		{
			StringBuilder sb = new StringBuilder();
			foreach (T? element in input)
			{
				if (element != null)
					sb.Append(element.ToString());
				sb.Append('\\');
			}
			return sb.ToString(0, Math.Max(0, sb.Length - 1));
		}

		private static int CompareElement(T? x, T? y)
		{
			if (x == null && y == null)
				return 0;
			if (x == null)
				return -1;
			if (y == null)
				return 1;
			return x.Value.CompareTo(y.Value);
		}

		public static int Compare(DicomArray<T> x, DicomArray<T> y)
		{
			return Compare(x, y, CompareElement);
		}

		public static int Compare(DicomArray<T> x, DicomArray<T> y, Comparison<T?> comparer)
		{
			if (comparer == null)
				return 0;
			if (x == null && y == null)
				return 0;
			if (x == null)
				return -1;
			if (y == null)
				return 1;

			IEnumerator<T?> xnumerator = x._values.GetEnumerator();
			IEnumerator<T?> ynumerator = y._values.GetEnumerator();

			int result = 0;
			do
			{
				bool xHasMore = xnumerator.MoveNext();
				bool yHasMore = ynumerator.MoveNext();

				if (xHasMore ^ yHasMore)
				{
					result = xHasMore ? 1 : -1;
				}
				else if (xHasMore) // note that yHasMore == xHasMore
				{
					result = comparer(xnumerator.Current, ynumerator.Current);
				}
				else
				{
					result = 0;
					break;
				}
			} while (result == 0);

			return result;
		}
	}
}
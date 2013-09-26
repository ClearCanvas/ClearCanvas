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

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// A static string helper class.
	/// </summary>
	public static class StringUtilities
	{
		/// <summary>
		/// A delegate used by <see cref="StringUtilities"/> to format output strings.
		/// </summary>
		public delegate string FormatDelegate<in T>(T value);

		/// <summary>
		/// Combines the input <paramref name="values"/> into a string, separated by <paramref name="separator"/>,
		/// using the given <paramref name="formatSpecifier"/> to format each entry in the string.
		/// </summary>
		/// <remarks>
		/// <typeparam name="T">Must implement <see cref="IFormattable"/>.</typeparam>
		/// </remarks>
		public static string Combine<T>(IEnumerable<T> values, string separator, string formatSpecifier, bool skipEmptyValues = true)
			where T : IFormattable
		{
			return Combine(values, separator, formatSpecifier, null, skipEmptyValues);
		}

		/// <summary>
		/// Combines the input <paramref name="values"/> into a string, separated by <paramref name="separator"/>,
		/// using the given <paramref name="formatSpecifier"/> to format each entry in the string.
		/// </summary>
		/// <remarks>
		/// <typeparam name="T">Must implement <see cref="IFormattable"/>.</typeparam>
		/// </remarks>
		public static string Combine<T>(IEnumerable<T> values, string separator, string formatSpecifier, IFormatProvider formatProvider, bool skipEmptyValues = true)
			where T : IFormattable
		{
			var formatter = string.IsNullOrEmpty(formatSpecifier) ? new FormatDelegate<T>(v => v.ToString()) : (v => v.ToString(formatSpecifier, formatProvider));
			return Combine(values, separator, formatter, skipEmptyValues);
		}

		/// <summary>
		/// Combines the input <paramref name="values"/> into a string separated by the <paramref name="separator"/>.
		/// </summary>
		/// <remarks>
		/// Empty values are skipped.
		/// </remarks>
		public static string Combine<T>(IEnumerable<T> values, string separator)
		{
			return Combine(values, separator, true);
		}

		/// <summary>
		/// Combines the input <paramref name="values"/> into a string separated by the <paramref name="separator"/>;
		/// empty values are skipped when <paramref name="skipEmptyValues"/> is true.
		/// </summary>
		public static string Combine<T>(IEnumerable<T> values, string separator, bool skipEmptyValues)
		{
			return Combine(values, separator, (FormatDelegate<T>) null, skipEmptyValues);
		}

		/// <summary>
		/// Combines the input <paramref name="values"/> into a string separated by the <paramref name="separator"/> 
		/// and formatted using <paramref name="formatDelegate"/>.
		/// </summary>
		public static string Combine<T>(IEnumerable<T> values, string separator, FormatDelegate<T> formatDelegate)
		{
			return Combine(values, separator, formatDelegate, true);
		}

		/// <summary>
		/// Combines the input <paramref name="values"/> into a string separated by <paramref name="separator"/> 
		/// and formatted using <paramref name="formatDelegate"/>; empty values are skipped when <paramref name="skipEmptyValues"/> is true.
		/// </summary>
		public static string Combine<T>(IEnumerable<T> values, string separator, FormatDelegate<T> formatDelegate, bool skipEmptyValues)
		{
			if (values == null)
				return "";

			if (separator == null)
				separator = "";

			StringBuilder builder = new StringBuilder();
			int count = 0;
			foreach (T value in values)
			{
				string stringValue;
				if (formatDelegate == null)
					stringValue = (value == null) ? "" : value.ToString();
				else
					stringValue = formatDelegate(value) ?? "";

				if (String.IsNullOrEmpty(stringValue) && skipEmptyValues)
					continue;

				if (count++ > 0)
					builder.Append(separator);

				builder.Append(stringValue);
			}

			return builder.ToString();
		}

		/// <summary>
		/// Splits any string into sub-strings using the specified <paramref name="delimiters"/>, 
		/// ignoring delimiters inside double quotes.
		/// </summary>
		/// <remarks>
		/// This is different from the <b>String.Split</b> methods 
		/// as we ignore delimiters inside double quotes.
		/// </remarks>
		/// <param name="text">The string to split.</param>
		/// <param name="delimiters">The characters to split on.</param>
		/// <returns></returns>
		public static string[] SplitQuoted(string text, string delimiters)
		{
			ArrayList res = new ArrayList();

			StringBuilder tokenBuilder = new StringBuilder();
			bool insideQuote = false;

			foreach (char c in text.ToCharArray())
			{
				if (!insideQuote && delimiters.Contains(c.ToString()))
				{
					res.Add(tokenBuilder.ToString());
					tokenBuilder.Length = 0;
				}
				else if (c.Equals('\"'))
				{
					insideQuote = !insideQuote;
				}
				else
				{
					tokenBuilder.Append(c);
				}
			}

			// add the last token
			res.Add(tokenBuilder.ToString());

			return (string[]) res.ToArray(typeof (string));
		}

		/// <summary>
		/// Converts an empty string to a null string, otherwise returns the argument unchanged.
		/// </summary>
		public static string NullIfEmpty(string s)
		{
			return string.IsNullOrEmpty(s) ? null : s;
		}

		/// <summary>
		/// Converts a null argument to an empty string, otherwise returns the argument unchanged.
		/// </summary>
		public static string EmptyIfNull(string s)
		{
			return s ?? "";
		}

		#region Hex String Helpers

		/// <summary>
		/// Formats a byte array as a compact, sequential string of uppercase hexadecimal digits (i.e. consisting of digits 0-9 and uppercase letters A-F).
		/// </summary>
		/// <remarks>
		/// The bytes in the input are converted sequentially. For each byte, the hexadecimal digit sequence is the most signficant nibble (high nibble),
		/// followed immediately by the least significant nibble (low nibble). Unlike <see cref="BitConverter.ToString(byte[])"/>, no delimiters are
		/// inserted in the output.
		/// </remarks>
		/// <param name="bytes">The input byte array.</param>
		/// <returns>A sequential string of uppercase hexadecimal digits.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="bytes"/> is NULL.</exception>
		public static string ToHexString(byte[] bytes)
		{
			return ToHexString(bytes, 0, bytes.Length);
		}

		/// <summary>
		/// Formats a byte array as a compact, sequential string of hexadecimal digits (i.e. consisting of digits 0-9 and letters A-F/a-f).
		/// </summary>
		/// <remarks>
		/// The bytes in the input are converted sequentially. For each byte, the hexadecimal digit sequence is the most signficant nibble (high nibble),
		/// followed immediately by the least significant nibble (low nibble). Unlike <see cref="BitConverter.ToString(byte[])"/>, no delimiters are
		/// inserted in the output.
		/// </remarks>
		/// <param name="bytes">The input byte array.</param>
		/// <param name="lowercase">True if the output should use lowercase hexadecimal digits; False if the output should use uppercase hexadecimal digits.</param>
		/// <returns>A sequential string of hexadecimal digits.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="bytes"/> is NULL.</exception>
		public static string ToHexString(byte[] bytes, bool lowercase)
		{
			return ToHexString(bytes, 0, bytes.Length, lowercase);
		}

		/// <summary>
		/// Formats a byte array as a compact, sequential string of uppercase hexadecimal digits (i.e. consisting of digits 0-9 and uppercase letters A-F).
		/// </summary>
		/// <remarks>
		/// The bytes in the input are converted sequentially. For each byte, the hexadecimal digit sequence is the most signficant nibble (high nibble),
		/// followed immediately by the least significant nibble (low nibble). Unlike <see cref="BitConverter.ToString(byte[],int,int)"/>, no delimiters are
		/// inserted in the output.
		/// </remarks>
		/// <param name="bytes">The input byte array.</param>
		/// <param name="offset">The byte offset in <paramref name="bytes"/> at which to start converting.</param>
		/// <param name="length">The number of bytes in <paramref name="bytes"/> to convert.</param>
		/// <returns>A sequential string of uppercase hexadecimal digits.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="bytes"/> is NULL.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="offset"/> and/or <paramref name="length"/> represents an invalid selection in <paramref name="bytes"/>.</exception>
		public static string ToHexString(byte[] bytes, int offset, int length)
		{
			return ToHexString(bytes, offset, length, false);
		}

		/// <summary>
		/// Formats a byte array as a compact, sequential string of hexadecimal digits (i.e. consisting of digits 0-9 and letters A-F/a-f).
		/// </summary>
		/// <remarks>
		/// The bytes in the input are converted sequentially. For each byte, the hexadecimal digit sequence is the most signficant nibble (high nibble),
		/// followed immediately by the least significant nibble (low nibble). Unlike <see cref="BitConverter.ToString(byte[],int,int)"/>, no delimiters are
		/// inserted in the output.
		/// </remarks>
		/// <param name="bytes">The input byte array.</param>
		/// <param name="offset">The byte offset in <paramref name="bytes"/> at which to start converting.</param>
		/// <param name="length">The number of bytes in <paramref name="bytes"/> to convert.</param>
		/// <param name="lowercase">True if the output should use lowercase hexadecimal digits; False if the output should use uppercase hexadecimal digits.</param>
		/// <returns>A sequential string of hexadecimal digits.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="bytes"/> is NULL.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="offset"/> and/or <paramref name="length"/> represents an invalid selection in <paramref name="bytes"/>.</exception>
		public static string ToHexString(byte[] bytes, int offset, int length, bool lowercase)
		{
			Platform.CheckForNullReference(bytes, "bytes");
			Platform.CheckNonNegative(offset, "offset");
			Platform.CheckArgumentRange(length, 0, bytes.Length - offset, "length");
			if (length == 0) return string.Empty;

			// this could be significantly faster if it were rewritten using unsafe code and incrementing pointers
			// however, doing so would also make this method become security critical and thus require full trust for callers
			// since this is a general use library, we'll leave it fully managed
			var c = -1;
			var end = offset + length;
			var chars = new char[length*2];
			var nibbles = lowercase ? "0123456789abcdef" : "0123456789ABCDEF";
			for (var n = offset; n < end; ++n)
			{
				var b = bytes[n];
				chars[++c] = nibbles[(b >> 4) & 0x0F];
				chars[++c] = nibbles[b & 0x0F];
			}
			return new string(chars);
		}

		#endregion
	}
}
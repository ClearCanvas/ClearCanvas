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

namespace ClearCanvas.Dicom.Iod
{
	/// <summary>
	/// Abstract base class for an Iod and Modules, which is a strongly typed wrapper to attributes in a dicom attribute collection.
	/// </summary>
	public abstract class IodBase
	{
		#region Private Variables

		/// <summary>
		/// Contains dicom attribute collection which contains all the dicom tags
		/// </summary>
		private IDicomAttributeProvider _dicomAttributeProvider;

		/// <summary>
		/// Contains a dictionary of module Iods.  Subclassed Iods can use dictionary for a lazy-loaded way of loading modules, this way
		/// if the <see cref="DicomAttributeProvider"/> gets updated, the new attribute collection will be used (lazy loaded).
		/// </summary>
		private Dictionary<Type, IodBase> _moduleIods = new Dictionary<Type, IodBase>();

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="IodBase"/> class.
		/// </summary>
		protected IodBase()
			: this(new DicomAttributeCollection()) {}

		protected IodBase(IDicomAttributeProvider dicomAttributeProvider)
		{
			_dicomAttributeProvider = dicomAttributeProvider;
		}

		#endregion

		#region Protected Properties

		/// <summary>
		/// Gets the dicom attribute collection.
		/// </summary>
		/// <value>The dicom attribute collection.</value>
		public IDicomAttributeProvider DicomAttributeProvider
		{
			get { return _dicomAttributeProvider; }
			set
			{
				_dicomAttributeProvider = value;
				// Clear the moduleIods so next time accessed it gets rebuilt cos of lazy loading
				_moduleIods.Clear();
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Sets the specified <paramref name="dicomTag"/>'s attribute value to be null.
		/// </summary>
		/// <param name="dicomTag">The dicom tag.</param>
		public void SetAttributeNull(DicomTag dicomTag)
		{
			this.DicomAttributeProvider[dicomTag].SetNullValue();
		}

		/// <summary>
		/// Sets the specified <paramref name="dicomTag"/>'s attribute value to be null.
		/// </summary>
		/// <param name="dicomTag">The dicom tag.</param>
		public void SetAttributeNull(uint dicomTag)
		{
			this.DicomAttributeProvider[dicomTag].SetNullValue();
		}

		/// <summary>
		/// Sets the list of dicom tags' attributes null.
		/// </summary>
		/// <param name="dicomTags">The dicom tags.</param>
		public void SetAttributesNull(IList<DicomTag> dicomTags)
		{
			foreach (DicomTag dicomTag in dicomTags)
				SetAttributeNull(dicomTag);
		}

		/// <summary>
		/// Sets the list of dicom tags' attributes null.
		/// </summary>
		/// <param name="dicomTags">The dicom tags.</param>
		public void SetAttributesNull(IList<uint> dicomTags)
		{
			foreach (uint dicomTag in dicomTags)
				SetAttributeNull(dicomTag);
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Gets the module iod for the specified T.  This is to be used by a subclassed IO for lazy loading of a Module IOD within that IOD.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		protected T GetModuleIod<T>()
			where T : IodBase, new()
		{
			if (!_moduleIods.ContainsKey(typeof (T)))
			{
				T newT = new T();
				(newT as IodBase).DicomAttributeProvider = DicomAttributeProvider;

				_moduleIods.Add(typeof (T), newT);
			}
			return _moduleIods[typeof (T)] as T;
		}

		#endregion

		#region Public Static Methods

		protected static bool IsNullOrEmpty(string value)
		{
			return string.IsNullOrEmpty(value);
		}

		protected static bool IsNullOrEmpty(DicomAttribute attribute)
		{
			return attribute.IsEmpty || attribute.IsNull;
		}

		protected static bool IsNullOrEmpty<T>(T value)
		{
			return Equals(value, default(T));
		}

		protected static bool IsNullOrEmpty<T>(T? value)
			where T : struct
		{
			return !value.HasValue;
		}

		protected static bool IsNullOrEmpty<T>(T[] array)
		{
			return array == null || array.Length == 0;
		}

		public static bool? ParseBool(string input, string trueString, string falseString)
		{
			if (!string.IsNullOrEmpty(input))
			{
				input = input.ToUpperInvariant();
				if (input == trueString.ToUpperInvariant())
					return true;
				else if (input == falseString.ToUpperInvariant())
					return false;
			}
			return null;
		}

		public static void SetAttributeFromBool(DicomAttribute dicomAttribute, bool? value, string trueString, string falseString)
		{
			if (!value.HasValue)
				dicomAttribute.SetNullValue();
			else
				dicomAttribute.SetStringValue(value.Value ? trueString : falseString);
		}

		/// <summary>
		/// Parses an enum value for the enum type T, automatically converting to Pascal if necessary since enum names don't have spaces.  Returns <paramref name="defaultValue"/> if string not found.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="input">The input.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns></returns>
		public static T ParseEnum<T>(string input, T defaultValue)
			where T : struct, IComparable, IFormattable, IConvertible
		{
			// First check for empty/null and default value to avoid exception performance hit
			if (String.IsNullOrEmpty(input) || input.ToUpperInvariant() == defaultValue.ToString().ToUpperInvariant())
				return defaultValue;

			try
			{
				if (input.Contains(" "))
					input = input.Replace(" ", "");
				return (T) Enum.Parse(typeof (T), input, true);
			}
			catch (Exception) {}
			return defaultValue;
		}

		/// <summary>
		/// Sets the dicom attribute value from enum value.
		/// </summary>
		/// <param name="dicomAttribute">The dicom attribute.</param>
		/// <param name="value">The value.</param>
		public static void SetAttributeFromEnum(DicomAttribute dicomAttribute, object value)
		{
			SetAttributeFromEnum(dicomAttribute, value, false);
		}

		/// <summary>
		/// Sets the dicom attribute value from enum.  Sets it to upper case as per dicom Standard.  
		/// If <paramref name="formatFromPascal"/> is true, then it formats the <paramref name="value"/> from Pascal - ie, MammoClear would 
		/// be set as MAMMO CLEAR .
		/// </summary>
		/// <param name="dicomAttribute">The dicom attribute.</param>
		/// <param name="value">The value.</param>
		/// <param name="formatFromPascal">if set to <c>true</c> [format from pascal].</param>
		public static void SetAttributeFromEnum(DicomAttribute dicomAttribute, object value, bool formatFromPascal)
		{
			if (value == null || String.IsNullOrEmpty(value.ToString()) || String.Compare(value.ToString(), "None", StringComparison.OrdinalIgnoreCase) == 0 || String.Compare(value.ToString(), "Unknown", StringComparison.OrdinalIgnoreCase) == 0)
				dicomAttribute.SetNullValue();
			else
			{
				if (formatFromPascal)
					value = FormatFromPascal(value.ToString());
				dicomAttribute.SetStringValue(value.ToString().ToUpperInvariant());
			}
		}

		/// <summary>
		/// Formats a string from pascal notation.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static string FormatFromPascal(string value)
		{
			if (value == null)
				return null;

			StringBuilder sb = new StringBuilder();
			int i = 0;
			foreach (char c in value)
			{
				if (i > 0 && char.IsUpper(c) && !IsPrevCharWhiteSpace(value, i) && !char.IsWhiteSpace(c) && !IsPrevCharUpper(value, i))
					sb.AppendFormat(" {0}", c.ToString());
				else if (char.IsWhiteSpace(c) && IsNextCharWhiteSpace(value, i))
					sb.Append(""); // don't append 2 spaces
				else
					sb.Append(c);
				i++;
			}
			return sb.ToString();
		}

		/// <summary>
		/// Determines whether [is prev char white space] [the specified value].  Helper function for FormatFromPascal.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="currentIndex">Index of the current.</param>
		/// <returns>
		/// 	<c>true</c> if [is prev char white space] [the specified value]; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsPrevCharWhiteSpace(string value, int currentIndex)
		{
			return currentIndex > 0 && char.IsWhiteSpace(value.Substring(currentIndex - 1, 1).ToCharArray()[0]);
		}

		/// <summary>
		/// Determines whether [is next char white space] [the specified value].  Helper function for FormatFromPascal.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="currentIndex">Index of the current.</param>
		/// <returns>
		/// 	<c>true</c> if [is next char white space] [the specified value]; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsNextCharWhiteSpace(string value, int currentIndex)
		{
			return currentIndex < value.Length && char.IsWhiteSpace(value.Substring(currentIndex + 1, 1).ToCharArray()[0]);
		}

		/// <summary>
		/// Determines whether [is prev char upper] [the specified value].  Helper function for FormatFromPascal.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="currentIndex">Index of the current.</param>
		/// <returns>
		/// 	<c>true</c> if [is prev char upper] [the specified value]; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsPrevCharUpper(string value, int currentIndex)
		{
			return currentIndex > 0 && char.IsUpper(value.Substring(currentIndex - 1, 1).ToCharArray()[0]);
		}

		#endregion
	}
}
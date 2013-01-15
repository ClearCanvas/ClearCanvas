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
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Provides a <see cref="TypeConverter"/> to convert <see cref="XMouseButtons"/> values to and from localized and invariant <see cref="string"/> representations.
	/// </summary>
	public class XMouseButtonsConverter : TypeConverter
	{
		/// <summary>
		/// The character used to separate individual buttons.
		/// </summary>
		public static readonly char ButtonSeparator = '+';

		private static readonly IDictionary<XMouseButtons, string> _invariantNames;
		private static readonly IDictionary<string, XMouseButtons> _invariantValues;

		/// <remarks>
		/// <![CDATA[The pattern is effectively @"^\s*(.+?)(?:\s*\+\s*(.+?)(?:\s*\+\s*(.+?)(?:\s*\+\s*(.+?)(?:\s*\+\s*(.+?))?)?)?)?\s*$" for a ButtonSeparator of '+']]>
		/// </remarks>
		private static readonly Regex _stringParser = new Regex(string.Format(@"^\s*(.+?)(?:\s*{0}\s*(.+?)(?:\s*{0}\s*(.+?)(?:\s*{0}\s*(.+?)(?:\s*{0}\s*(.+?))?)?)?)?\s*$",
		                                                                      Regex.Escape(ButtonSeparator.ToString())), RegexOptions.Compiled);

		private readonly IDictionary<XMouseButtons, string> _localizedNames;
		private readonly IDictionary<string, XMouseButtons> _localizedValues;
		private readonly CultureInfo _culture;

		/// <summary>
		/// Type initializer for <see cref="XMouseButtonsConverter"/>.
		/// </summary>
		/// <remarks>
		/// The cache for the button names in the invariant culture is generated during the type initialization process.
		/// </remarks>
		static XMouseButtonsConverter()
		{
			InitializeMaps(CultureInfo.InvariantCulture, _invariantNames = new Dictionary<XMouseButtons, string>(), _invariantValues = new Dictionary<string, XMouseButtons>());
		}

		/// <summary>
		/// Constructs a new instance of an <see cref="XMouseButtonsConverter"/>.
		/// </summary>
		public XMouseButtonsConverter() : this(null) {}

		/// <summary>
		/// Constructs a new instance of an <see cref="XMouseButtonsConverter"/> for a specific culture.
		/// </summary>
		/// <param name="culture">The culture for which to cache localized button names. If this value is NULL, the <see cref="CultureInfo"/> is obtained using the current thread's <see cref="CultureInfo.CurrentUICulture"/> property.</param>
		/// <remarks>
		/// The <paramref name="culture"/> parameter is used to cache a set of localized button names, allowing for improved performance when
		/// converting in the context of the specified culture or the invariant culture.
		/// </remarks>
		public XMouseButtonsConverter(CultureInfo culture)
		{
			_culture = culture ?? CultureInfo.CurrentUICulture;
			if (!CultureInfo.InvariantCulture.Equals(_culture))
			{
				// if we want the invariant culture, we don't need to initialize the maps since we'll fallback to the static maps anyway
				InitializeMaps(_culture, _localizedNames = new Dictionary<XMouseButtons, string>(), _localizedValues = new Dictionary<string, XMouseButtons>());
			}
		}

		/// <summary>
		/// Initializes the localization maps for the specified culture.
		/// </summary>
		private static void InitializeMaps(CultureInfo culture, IDictionary<XMouseButtons, string> names, IDictionary<string, XMouseButtons> values)
		{
			XKeysNames.Culture = culture;
			try
			{
				names.Clear();

				#region Mappings

				names.Add(XMouseButtons.Left, XKeysNames.LeftMouseButton ?? string.Empty);
				names.Add(XMouseButtons.Right, XKeysNames.RightMouseButton ?? string.Empty);
				names.Add(XMouseButtons.Middle, XKeysNames.MiddleMouseButton ?? string.Empty);
				names.Add(XMouseButtons.XButton1, XKeysNames.XMouseButton1 ?? string.Empty);
				names.Add(XMouseButtons.XButton2, XKeysNames.XMouseButton2 ?? string.Empty);

				#endregion
			}
			finally
			{
				XKeysNames.Culture = null;
			}

			if (values != null)
			{
				values.Clear();
				foreach (KeyValuePair<XMouseButtons, string> pair in names)
				{
					if (values.ContainsKey(pair.Value))
					{
						Platform.Log(LogLevel.Debug, "{1}(Culture={2}) has an ambiguous translation for {0}", pair.Value, typeof (XKeysNames).FullName, culture);
						continue;
					}
					values.Add(pair.Value, pair.Key);
				}
			}
		}

		#region Static Helpers

		/// <summary>
		/// Gets the default instance of <see cref="XMouseButtonsConverter"/>.
		/// </summary>
		/// <remarks>
		/// This is equivalent to calling <see cref="TypeDescriptor.GetConverter(System.Type)"/> for the <see cref="XMouseButtons"/> <see cref="Type"/>.
		/// </remarks>
		public static XMouseButtonsConverter Default
		{
			get { return TypeDescriptor.GetConverter(typeof (XMouseButtons)) as XMouseButtonsConverter; }
		}

		/// <summary>
		/// Formats a <see cref="XMouseButtons"/> value as a string using the <see cref="CultureInfo.CurrentUICulture">current thread's UI CultureInfo</see>.
		/// </summary>
		/// <param name="value">The <see cref="XMouseButtons"/> value to be formatted.</param>
		/// <returns>The string representation of the given <paramref name="value"/>.</returns>
		public static string Format(XMouseButtons value)
		{
			return Format(value, CultureInfo.CurrentUICulture);
		}

		/// <summary>
		/// Formats a <see cref="XMouseButtons"/> value as a string using the specified <see cref="CultureInfo"/>.
		/// </summary>
		/// <param name="value">The <see cref="XMouseButtons"/> value to be formatted.</param>
		/// <param name="culture">The <see cref="CultureInfo"/> for which the value should be formatted.</param>
		/// <returns>The string representation of the given <paramref name="value"/>.</returns>
		public static string Format(XMouseButtons value, CultureInfo culture)
		{
			return Default.ConvertToString(null, culture, value);
		}

		/// <summary>
		/// Formats a <see cref="XMouseButtons"/> value as a string using the <see cref="CultureInfo.InvariantCulture"/>.
		/// </summary>
		/// <param name="value">The <see cref="XMouseButtons"/> value to be formatted.</param>
		/// <returns>The string representation of the given <paramref name="value"/>.</returns>
		public static string FormatInvariant(XMouseButtons value)
		{
			return Format(value, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Parses a string as an <see cref="XMouseButtons"/> value using the <see cref="CultureInfo.CurrentUICulture">current thread's UI CultureInfo</see>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <returns>The <see cref="XMouseButtons"/> value parsed from <paramref name="s"/>.</returns>
		/// <exception cref="FormatException">Thrown if <paramref name="s"/> is not a valid <see cref="XMouseButtons"/> string representation.</exception>
		public static XMouseButtons Parse(string s)
		{
			return Parse(s, CultureInfo.CurrentUICulture);
		}

		/// <summary>
		/// Parses a string as an <see cref="XMouseButtons"/> value using the specified <see cref="CultureInfo"/>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <param name="culture">The <see cref="CultureInfo"/> for which the string should be parsed.</param>
		/// <returns>The <see cref="XMouseButtons"/> value parsed from <paramref name="s"/>.</returns>
		/// <exception cref="FormatException">Thrown if <paramref name="s"/> is not a valid <see cref="XMouseButtons"/> string representation.</exception>
		public static XMouseButtons Parse(string s, CultureInfo culture)
		{
			return (XMouseButtons) Default.ConvertFromString(null, culture, s);
		}

		/// <summary>
		/// Parses a string as an <see cref="XMouseButtons"/> value using the <see cref="CultureInfo.InvariantCulture"/>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <returns>The <see cref="XMouseButtons"/> value parsed from <paramref name="s"/>.</returns>
		/// <exception cref="FormatException">Thrown if <paramref name="s"/> is not a valid <see cref="XMouseButtons"/> string representation.</exception>
		public static XMouseButtons ParseInvariant(string s)
		{
			return Parse(s, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Parses a string as an <see cref="XMouseButtons"/> value using the <see cref="CultureInfo.CurrentUICulture">current thread's UI CultureInfo</see>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <param name="result">The <see cref="XMouseButtons"/> value parsed from <paramref name="s"/> if the string was successfully parsed; <see cref="XMouseButtons.None"/> otherwise.</param>
		/// <returns>True if the string was successfully parsed; False otherwise.</returns>
		public static bool TryParse(string s, out XMouseButtons result)
		{
			return TryParse(s, CultureInfo.CurrentUICulture, out result);
		}

		/// <summary>
		/// Parses a string as an <see cref="XMouseButtons"/> value using the specified <see cref="CultureInfo"/>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <param name="culture">The <see cref="CultureInfo"/> for which the string should be parsed.</param>
		/// <param name="result">The <see cref="XMouseButtons"/> value parsed from <paramref name="s"/> if the string was successfully parsed; <see cref="XMouseButtons.None"/> otherwise.</param>
		/// <returns>True if the string was successfully parsed; False otherwise.</returns>
		public static bool TryParse(string s, CultureInfo culture, out XMouseButtons result)
		{
			try
			{
				result = Parse(s, culture);
				return true;
			}
			catch (FormatException)
			{
				result = XMouseButtons.None;
				return false;
			}
		}

		/// <summary>
		/// Parses a string as an <see cref="XMouseButtons"/> value using the <see cref="CultureInfo.InvariantCulture"/>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <param name="result">The <see cref="XMouseButtons"/> value parsed from <paramref name="s"/> if the string was successfully parsed; <see cref="XMouseButtons.None"/> otherwise.</param>
		/// <returns>True if the string was successfully parsed; False otherwise.</returns>
		public static bool TryParseInvariant(string s, out XMouseButtons result)
		{
			return TryParse(s, CultureInfo.InvariantCulture, out result);
		}

		#endregion

		#region Unit Test Support

#if UNIT_TESTS
		internal static IEnumerable<KeyValuePair<XMouseButtons, string>> InvariantNames
		{
			get { return _invariantNames; }
		}

		internal IEnumerable<KeyValuePair<XMouseButtons, string>> LocalizedNames
		{
			get { return _localizedNames; }
			set
			{
				_localizedNames.Clear();
				_localizedValues.Clear();
				foreach (KeyValuePair<XMouseButtons, string> pair in value)
				{
					_localizedNames.Add(pair);
					_localizedValues.Add(pair.Value, pair.Key);
				}
			}
		}

		internal CultureInfo Culture
		{
			get { return _culture; }
		}
#endif

		#endregion

		#region Converter Implementation

		/// <summary>
		/// Gets the correct localization map for the specified culture.
		/// </summary>
		private IDictionary<XMouseButtons, string> GetNamesMap(CultureInfo culture)
		{
			if (CultureInfo.InvariantCulture.Equals(culture))
				return _invariantNames;
			else if (_culture.Equals(culture))
				return _localizedNames;
			else
			{
				IDictionary<XMouseButtons, string> names = new Dictionary<XMouseButtons, string>();
				InitializeMaps(culture, names, null);
				return names;
			}
		}

		/// <summary>
		/// Gets the correct localization map for the specified culture.
		/// </summary>
		private IDictionary<string, XMouseButtons> GetValuesMap(CultureInfo culture)
		{
			if (CultureInfo.InvariantCulture.Equals(culture))
				return _invariantValues;
			else if (_culture.Equals(culture))
				return _localizedValues;
			else
			{
				IDictionary<string, XMouseButtons> values = new Dictionary<string, XMouseButtons>();
				InitializeMaps(culture, new Dictionary<XMouseButtons, string>(), values);
				return values;
			}
		}

		private static string GetName(XMouseButtons value, IDictionary<XMouseButtons, string> map)
		{
			if (map.ContainsKey(value))
				return map[value];
			else if (Enum.IsDefined(typeof (XMouseButtons), (int) value))
				return Enum.GetName(typeof (XMouseButtons), (int) value);
			return string.Empty;
		}

		private static XMouseButtons GetValue(string name, IDictionary<string, XMouseButtons> map)
		{
			if (map.ContainsKey(name))
				return map[name];
			else if (Enum.IsDefined(typeof (XMouseButtons), name))
				return (XMouseButtons) Enum.Parse(typeof (XMouseButtons), name);
			else
				throw new FormatException(string.Format("{0} is not a valid member of {1}", name, typeof (XMouseButtons).FullName));
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof (string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				string stringValue = (string) value;
				if (string.IsNullOrEmpty(stringValue))
					return XMouseButtons.None;

				Match m = _stringParser.Match((string) value);
				if (!m.Success)
					throw new FormatException(string.Format("Input string was not in the expected format for {0}", typeof (XMouseButtons).FullName));

				XMouseButtons buttons = XMouseButtons.None;
				IDictionary<string, XMouseButtons> map = GetValuesMap(culture);
				for (int n = 1; n <= 5; n++)
				{
					if (m.Groups[n].Length == 0)
						break;

					buttons |= GetValue(m.Groups[n].Value, map);
				}
				return buttons;
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof (XMouseButtons))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof (string))
			{
				List<string> names = new List<string>(5);
				XMouseButtons eValue = (XMouseButtons) value;
				IDictionary<XMouseButtons, string> map = GetNamesMap(culture);
				if ((eValue & XMouseButtons.Left) == XMouseButtons.Left)
				{
					string buttonName = GetName(XMouseButtons.Left, map);
					if (!string.IsNullOrEmpty(buttonName))
						names.Add(buttonName);
				}
				if ((eValue & XMouseButtons.Right) == XMouseButtons.Right)
				{
					string buttonName = GetName(XMouseButtons.Right, map);
					if (!string.IsNullOrEmpty(buttonName))
						names.Add(buttonName);
				}
				if ((eValue & XMouseButtons.Middle) == XMouseButtons.Middle)
				{
					string buttonName = GetName(XMouseButtons.Middle, map);
					if (!string.IsNullOrEmpty(buttonName))
						names.Add(buttonName);
				}
				if ((eValue & XMouseButtons.XButton1) == XMouseButtons.XButton1)
				{
					string buttonName = GetName(XMouseButtons.XButton1, map);
					if (!string.IsNullOrEmpty(buttonName))
						names.Add(buttonName);
				}
				if ((eValue & XMouseButtons.XButton2) == XMouseButtons.XButton2)
				{
					string buttonName = GetName(XMouseButtons.XButton2, map);
					if (!string.IsNullOrEmpty(buttonName))
						names.Add(buttonName);
				}
				if (names.Count == 0)
					return string.Empty;
				return string.Join(ButtonSeparator.ToString(), names.ToArray());
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		#endregion
	}
}
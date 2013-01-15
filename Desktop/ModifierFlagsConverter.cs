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
	/// Provides a <see cref="TypeConverter"/> to convert <see cref="ModifierFlags"/> values to and from localized and invariant <see cref="string"/> representations.
	/// </summary>
	public class ModifierFlagsConverter : TypeConverter
	{
		/// <summary>
		/// The character used to separate individual modifiers.
		/// </summary>
		public static readonly char ModifierSeparator = '+';

		private static readonly IDictionary<ModifierFlags, string> _invariantNames;
		private static readonly IDictionary<string, ModifierFlags> _invariantValues;

		/// <remarks>
		/// <![CDATA[The pattern is effectively @"^\s*(.+?)(?:\s*\+\s*(.+?)(?:\s*\+\s*(.+?))?)?\s*$" for a ModifierSeparator of '+']]>
		/// </remarks>
		private static readonly Regex _stringParser = new Regex(string.Format(@"^\s*(.+?)(?:\s*{0}\s*(.+?)(?:\s*{0}\s*(.+?))?)?\s*$",
		                                                                      Regex.Escape(ModifierSeparator.ToString())), RegexOptions.Compiled);

		private readonly IDictionary<ModifierFlags, string> _localizedNames;
		private readonly IDictionary<string, ModifierFlags> _localizedValues;
		private readonly CultureInfo _culture;

		/// <summary>
		/// Type initializer for <see cref="ModifierFlagsConverter"/>.
		/// </summary>
		/// <remarks>
		/// The cache for the modifier names in the invariant culture is generated during the type initialization process.
		/// </remarks>
		static ModifierFlagsConverter()
		{
			InitializeMaps(CultureInfo.InvariantCulture, _invariantNames = new Dictionary<ModifierFlags, string>(), _invariantValues = new Dictionary<string, ModifierFlags>());
		}

		/// <summary>
		/// Constructs a new instance of an <see cref="ModifierFlagsConverter"/>.
		/// </summary>
		public ModifierFlagsConverter() : this(null) {}

		/// <summary>
		/// Constructs a new instance of an <see cref="ModifierFlagsConverter"/> for a specific culture.
		/// </summary>
		/// <param name="culture">The culture for which to cache localized modifier names. If this value is NULL, the <see cref="CultureInfo"/> is obtained using the current thread's <see cref="CultureInfo.CurrentUICulture"/> property.</param>
		/// <remarks>
		/// The <paramref name="culture"/> parameter is used to cache a set of localized modifier names, allowing for improved performance when
		/// converting in the context of the specified culture or the invariant culture.
		/// </remarks>
		public ModifierFlagsConverter(CultureInfo culture)
		{
			_culture = culture ?? CultureInfo.CurrentUICulture;
			if (!CultureInfo.InvariantCulture.Equals(_culture))
			{
				// if we want the invariant culture, we don't need to initialize the maps since we'll fallback to the static maps anyway
				InitializeMaps(_culture, _localizedNames = new Dictionary<ModifierFlags, string>(), _localizedValues = new Dictionary<string, ModifierFlags>());
			}
		}

		/// <summary>
		/// Initializes the localization maps for the specified culture.
		/// </summary>
		private static void InitializeMaps(CultureInfo culture, IDictionary<ModifierFlags, string> names, IDictionary<string, ModifierFlags> values)
		{
			XKeysNames.Culture = culture;
			try
			{
				names.Clear();

				#region Mappings

				names.Add(ModifierFlags.Alt, XKeysNames.Alt ?? string.Empty);
				names.Add(ModifierFlags.Control, XKeysNames.Control ?? string.Empty);
				names.Add(ModifierFlags.Shift, XKeysNames.Shift ?? string.Empty);

				#endregion
			}
			finally
			{
				XKeysNames.Culture = null;
			}

			if (values != null)
			{
				values.Clear();
				foreach (KeyValuePair<ModifierFlags, string> pair in names)
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
		/// Gets the default instance of <see cref="ModifierFlagsConverter"/>.
		/// </summary>
		/// <remarks>
		/// This is equivalent to calling <see cref="TypeDescriptor.GetConverter(System.Type)"/> for the <see cref="ModifierFlags"/> <see cref="Type"/>.
		/// </remarks>
		public static ModifierFlagsConverter Default
		{
			get { return TypeDescriptor.GetConverter(typeof (ModifierFlags)) as ModifierFlagsConverter; }
		}

		/// <summary>
		/// Formats a <see cref="ModifierFlags"/> value as a string using the <see cref="CultureInfo.CurrentUICulture">current thread's UI CultureInfo</see>.
		/// </summary>
		/// <param name="value">The <see cref="ModifierFlags"/> value to be formatted.</param>
		/// <returns>The string representation of the given <paramref name="value"/>.</returns>
		public static string Format(ModifierFlags value)
		{
			return Format(value, CultureInfo.CurrentUICulture);
		}

		/// <summary>
		/// Formats a <see cref="ModifierFlags"/> value as a string using the specified <see cref="CultureInfo"/>.
		/// </summary>
		/// <param name="value">The <see cref="ModifierFlags"/> value to be formatted.</param>
		/// <param name="culture">The <see cref="CultureInfo"/> for which the value should be formatted.</param>
		/// <returns>The string representation of the given <paramref name="value"/>.</returns>
		public static string Format(ModifierFlags value, CultureInfo culture)
		{
			return Default.ConvertToString(null, culture, value);
		}

		/// <summary>
		/// Formats a <see cref="ModifierFlags"/> value as a string using the <see cref="CultureInfo.InvariantCulture"/>.
		/// </summary>
		/// <param name="value">The <see cref="ModifierFlags"/> value to be formatted.</param>
		/// <returns>The string representation of the given <paramref name="value"/>.</returns>
		public static string FormatInvariant(ModifierFlags value)
		{
			return Format(value, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Parses a string as an <see cref="ModifierFlags"/> value using the <see cref="CultureInfo.CurrentUICulture">current thread's UI CultureInfo</see>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <returns>The <see cref="ModifierFlags"/> value parsed from <paramref name="s"/>.</returns>
		/// <exception cref="FormatException">Thrown if <paramref name="s"/> is not a valid <see cref="ModifierFlags"/> string representation.</exception>
		public static ModifierFlags Parse(string s)
		{
			return Parse(s, CultureInfo.CurrentUICulture);
		}

		/// <summary>
		/// Parses a string as an <see cref="ModifierFlags"/> value using the specified <see cref="CultureInfo"/>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <param name="culture">The <see cref="CultureInfo"/> for which the string should be parsed.</param>
		/// <returns>The <see cref="ModifierFlags"/> value parsed from <paramref name="s"/>.</returns>
		/// <exception cref="FormatException">Thrown if <paramref name="s"/> is not a valid <see cref="ModifierFlags"/> string representation.</exception>
		public static ModifierFlags Parse(string s, CultureInfo culture)
		{
			return (ModifierFlags) Default.ConvertFromString(null, culture, s);
		}

		/// <summary>
		/// Parses a string as an <see cref="ModifierFlags"/> value using the <see cref="CultureInfo.InvariantCulture"/>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <returns>The <see cref="ModifierFlags"/> value parsed from <paramref name="s"/>.</returns>
		/// <exception cref="FormatException">Thrown if <paramref name="s"/> is not a valid <see cref="ModifierFlags"/> string representation.</exception>
		public static ModifierFlags ParseInvariant(string s)
		{
			return Parse(s, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Parses a string as an <see cref="ModifierFlags"/> value using the <see cref="CultureInfo.CurrentUICulture">current thread's UI CultureInfo</see>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <param name="result">The <see cref="ModifierFlags"/> value parsed from <paramref name="s"/> if the string was successfully parsed; <see cref="ModifierFlags.None"/> otherwise.</param>
		/// <returns>True if the string was successfully parsed; False otherwise.</returns>
		public static bool TryParse(string s, out ModifierFlags result)
		{
			return TryParse(s, CultureInfo.CurrentUICulture, out result);
		}

		/// <summary>
		/// Parses a string as an <see cref="ModifierFlags"/> value using the specified <see cref="CultureInfo"/>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <param name="culture">The <see cref="CultureInfo"/> for which the string should be parsed.</param>
		/// <param name="result">The <see cref="ModifierFlags"/> value parsed from <paramref name="s"/> if the string was successfully parsed; <see cref="ModifierFlags.None"/> otherwise.</param>
		/// <returns>True if the string was successfully parsed; False otherwise.</returns>
		public static bool TryParse(string s, CultureInfo culture, out ModifierFlags result)
		{
			try
			{
				result = Parse(s, culture);
				return true;
			}
			catch (FormatException)
			{
				result = ModifierFlags.None;
				return false;
			}
		}

		/// <summary>
		/// Parses a string as an <see cref="ModifierFlags"/> value using the <see cref="CultureInfo.InvariantCulture"/>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <param name="result">The <see cref="ModifierFlags"/> value parsed from <paramref name="s"/> if the string was successfully parsed; <see cref="ModifierFlags.None"/> otherwise.</param>
		/// <returns>True if the string was successfully parsed; False otherwise.</returns>
		public static bool TryParseInvariant(string s, out ModifierFlags result)
		{
			return TryParse(s, CultureInfo.InvariantCulture, out result);
		}

		#endregion

		#region Unit Test Support

#if UNIT_TESTS
		internal static IEnumerable<KeyValuePair<ModifierFlags, string>> InvariantNames
		{
			get { return _invariantNames; }
		}

		internal IEnumerable<KeyValuePair<ModifierFlags, string>> LocalizedNames
		{
			get { return _localizedNames; }
			set
			{
				_localizedNames.Clear();
				_localizedValues.Clear();
				foreach (KeyValuePair<ModifierFlags, string> pair in value)
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
		private IDictionary<ModifierFlags, string> GetNamesMap(CultureInfo culture)
		{
			if (CultureInfo.InvariantCulture.Equals(culture))
				return _invariantNames;
			else if (_culture.Equals(culture))
				return _localizedNames;
			else
			{
				IDictionary<ModifierFlags, string> names = new Dictionary<ModifierFlags, string>();
				InitializeMaps(culture, names, null);
				return names;
			}
		}

		/// <summary>
		/// Gets the correct localization map for the specified culture.
		/// </summary>
		private IDictionary<string, ModifierFlags> GetValuesMap(CultureInfo culture)
		{
			if (CultureInfo.InvariantCulture.Equals(culture))
				return _invariantValues;
			else if (_culture.Equals(culture))
				return _localizedValues;
			else
			{
				IDictionary<string, ModifierFlags> values = new Dictionary<string, ModifierFlags>();
				InitializeMaps(culture, new Dictionary<ModifierFlags, string>(), values);
				return values;
			}
		}

		private static string GetName(ModifierFlags value, IDictionary<ModifierFlags, string> map)
		{
			if (map.ContainsKey(value))
				return map[value];
			else if (Enum.IsDefined(typeof (ModifierFlags), (int) value))
				return Enum.GetName(typeof (ModifierFlags), (int) value);
			return string.Empty;
		}

		private static ModifierFlags GetValue(string name, IDictionary<string, ModifierFlags> map)
		{
			if (map.ContainsKey(name))
				return map[name];
			else if (Enum.IsDefined(typeof (ModifierFlags), name))
				return (ModifierFlags) Enum.Parse(typeof (ModifierFlags), name);
			else
				throw new FormatException(string.Format("{0} is not a valid member of {1}", name, typeof (ModifierFlags).FullName));
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
					return ModifierFlags.None;

				Match m = _stringParser.Match((string) value);
				if (!m.Success)
					throw new FormatException(string.Format("Input string was not in the expected format for {0}", typeof (ModifierFlags).FullName));

				ModifierFlags modifiers = ModifierFlags.None;
				IDictionary<string, ModifierFlags> map = GetValuesMap(culture);
				for (int n = 1; n <= 3; n++)
				{
					if (m.Groups[n].Length == 0)
						break;

					modifiers |= GetValue(m.Groups[n].Value, map);
				}
				return modifiers;
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof (ModifierFlags))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof (string))
			{
				List<string> names = new List<string>(3);
				ModifierFlags eValue = (ModifierFlags) value;
				IDictionary<ModifierFlags, string> map = GetNamesMap(culture);
				if ((eValue & ModifierFlags.Control) == ModifierFlags.Control)
				{
					string modifierName = GetName(ModifierFlags.Control, map);
					if (!string.IsNullOrEmpty(modifierName))
						names.Add(modifierName);
				}
				if ((eValue & ModifierFlags.Alt) == ModifierFlags.Alt)
				{
					string modifierName = GetName(ModifierFlags.Alt, map);
					if (!string.IsNullOrEmpty(modifierName))
						names.Add(modifierName);
				}
				if ((eValue & ModifierFlags.Shift) == ModifierFlags.Shift)
				{
					string modifierName = GetName(ModifierFlags.Shift, map);
					if (!string.IsNullOrEmpty(modifierName))
						names.Add(modifierName);
				}
				if (names.Count == 0)
					return string.Empty;
				return string.Join(ModifierSeparator.ToString(), names.ToArray());
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		#endregion
	}
}
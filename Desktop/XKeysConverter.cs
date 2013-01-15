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
	/// Provides a <see cref="TypeConverter"/> to convert <see cref="XKeys"/> values to and from localized and invariant <see cref="string"/> representations.
	/// </summary>
	public class XKeysConverter : TypeConverter
	{
		/// <summary>
		/// The character used to separate individual keys.
		/// </summary>
		public static readonly char KeySeparator = '+';

		private static readonly IDictionary<XKeys, string> _invariantKeyNames;
		private static readonly IDictionary<string, XKeys> _invariantKeyValues;

		/// <remarks>
		/// <![CDATA[The pattern is effectively @"^\s*(.+?)(?:\s*\+\s*(.+?)(?:\s*\+\s*(.+?)(?:\s*\+\s*(.+?))?)?)?\s*$" for a KeySeparator of '+']]>
		/// </remarks>
		private static readonly Regex _keyStringParser = new Regex(string.Format(@"^\s*(.+?)(?:\s*{0}\s*(.+?)(?:\s*{0}\s*(.+?)(?:\s*{0}\s*(.+?))?)?)?\s*$",
		                                                                         Regex.Escape(KeySeparator.ToString())), RegexOptions.Compiled);

		private readonly IDictionary<XKeys, string> _localizedKeyNames;
		private readonly IDictionary<string, XKeys> _localizedKeyValues;
		private readonly CultureInfo _culture;

		/// <summary>
		/// Type initializer for <see cref="XKeysConverter"/>.
		/// </summary>
		/// <remarks>
		/// The cache for the key names in the invariant culture is generated during the type initialization process.
		/// </remarks>
		static XKeysConverter()
		{
			InitializeMaps(CultureInfo.InvariantCulture, _invariantKeyNames = new Dictionary<XKeys, string>(), _invariantKeyValues = new Dictionary<string, XKeys>());
		}

		/// <summary>
		/// Constructs a new instance of an <see cref="XKeysConverter"/>.
		/// </summary>
		public XKeysConverter() : this(null) {}

		/// <summary>
		/// Constructs a new instance of an <see cref="XKeysConverter"/> for a specific culture.
		/// </summary>
		/// <param name="culture">The culture for which to cache localized key names. If this value is NULL, the <see cref="CultureInfo"/> is obtained using the current thread's <see cref="CultureInfo.CurrentUICulture"/> property.</param>
		/// <remarks>
		/// The <paramref name="culture"/> parameter is used to cache a set of localized key names, allowing for improved performance when
		/// converting in the context of the specified culture or the invariant culture.
		/// </remarks>
		public XKeysConverter(CultureInfo culture)
		{
			_culture = culture ?? CultureInfo.CurrentUICulture;
			if (!CultureInfo.InvariantCulture.Equals(_culture))
			{
				// if we want the invariant culture, we don't need to initialize the maps since we'll fallback to the static maps anyway
				InitializeMaps(_culture, _localizedKeyNames = new Dictionary<XKeys, string>(), _localizedKeyValues = new Dictionary<string, XKeys>());
			}
		}

		/// <summary>
		/// Initializes the localization maps for the specified culture.
		/// </summary>
		private static void InitializeMaps(CultureInfo culture, IDictionary<XKeys, string> keyNames, IDictionary<string, XKeys> keyValues)
		{
			XKeysNames.Culture = culture;
			try
			{
				keyNames.Clear();

				#region Key Mappings

				keyNames.Add(XKeys.Add, XKeysNames.Add ?? string.Empty);
				keyNames.Add(XKeys.Alt, XKeysNames.Alt ?? string.Empty);
				keyNames.Add(XKeys.AltKey, XKeysNames.AltKey ?? string.Empty);
				keyNames.Add(XKeys.Apps, XKeysNames.Apps ?? string.Empty);
				keyNames.Add(XKeys.Attn, XKeysNames.Attn ?? string.Empty);
				keyNames.Add(XKeys.Backspace, XKeysNames.Backspace ?? string.Empty);
				keyNames.Add(XKeys.BrowserBack, XKeysNames.BrowserBack ?? string.Empty);
				keyNames.Add(XKeys.BrowserFavorites, XKeysNames.BrowserFavorites ?? string.Empty);
				keyNames.Add(XKeys.BrowserForward, XKeysNames.BrowserForward ?? string.Empty);
				keyNames.Add(XKeys.BrowserHome, XKeysNames.BrowserHome ?? string.Empty);
				keyNames.Add(XKeys.BrowserRefresh, XKeysNames.BrowserRefresh ?? string.Empty);
				keyNames.Add(XKeys.BrowserSearch, XKeysNames.BrowserSearch ?? string.Empty);
				keyNames.Add(XKeys.BrowserStop, XKeysNames.BrowserStop ?? string.Empty);
				keyNames.Add(XKeys.Cancel, XKeysNames.Cancel ?? string.Empty);
				keyNames.Add(XKeys.CapsLock, XKeysNames.CapsLock ?? string.Empty);
				keyNames.Add(XKeys.Clear, XKeysNames.Clear ?? string.Empty);
				keyNames.Add(XKeys.Control, XKeysNames.Control ?? string.Empty);
				keyNames.Add(XKeys.ControlKey, XKeysNames.ControlKey ?? string.Empty);
				keyNames.Add(XKeys.Crsel, XKeysNames.Crsel ?? string.Empty);
				keyNames.Add(XKeys.Decimal, XKeysNames.Decimal ?? string.Empty);
				keyNames.Add(XKeys.Delete, XKeysNames.Delete ?? string.Empty);
				keyNames.Add(XKeys.Digit0, XKeysNames.Digit0 ?? string.Empty);
				keyNames.Add(XKeys.Digit1, XKeysNames.Digit1 ?? string.Empty);
				keyNames.Add(XKeys.Digit2, XKeysNames.Digit2 ?? string.Empty);
				keyNames.Add(XKeys.Digit3, XKeysNames.Digit3 ?? string.Empty);
				keyNames.Add(XKeys.Digit4, XKeysNames.Digit4 ?? string.Empty);
				keyNames.Add(XKeys.Digit5, XKeysNames.Digit5 ?? string.Empty);
				keyNames.Add(XKeys.Digit6, XKeysNames.Digit6 ?? string.Empty);
				keyNames.Add(XKeys.Digit7, XKeysNames.Digit7 ?? string.Empty);
				keyNames.Add(XKeys.Digit8, XKeysNames.Digit8 ?? string.Empty);
				keyNames.Add(XKeys.Digit9, XKeysNames.Digit9 ?? string.Empty);
				keyNames.Add(XKeys.Divide, XKeysNames.Divide ?? string.Empty);
				keyNames.Add(XKeys.Down, XKeysNames.Down ?? string.Empty);
				keyNames.Add(XKeys.End, XKeysNames.End ?? string.Empty);
				keyNames.Add(XKeys.Enter, XKeysNames.Enter ?? string.Empty);
				keyNames.Add(XKeys.EraseEof, XKeysNames.EraseEof ?? string.Empty);
				keyNames.Add(XKeys.Escape, XKeysNames.Escape ?? string.Empty);
				keyNames.Add(XKeys.Execute, XKeysNames.Execute ?? string.Empty);
				keyNames.Add(XKeys.Exsel, XKeysNames.Exsel ?? string.Empty);
				keyNames.Add(XKeys.F1, XKeysNames.F1 ?? string.Empty);
				keyNames.Add(XKeys.F2, XKeysNames.F2 ?? string.Empty);
				keyNames.Add(XKeys.F3, XKeysNames.F3 ?? string.Empty);
				keyNames.Add(XKeys.F4, XKeysNames.F4 ?? string.Empty);
				keyNames.Add(XKeys.F5, XKeysNames.F5 ?? string.Empty);
				keyNames.Add(XKeys.F6, XKeysNames.F6 ?? string.Empty);
				keyNames.Add(XKeys.F7, XKeysNames.F7 ?? string.Empty);
				keyNames.Add(XKeys.F8, XKeysNames.F8 ?? string.Empty);
				keyNames.Add(XKeys.F9, XKeysNames.F9 ?? string.Empty);
				keyNames.Add(XKeys.F10, XKeysNames.F10 ?? string.Empty);
				keyNames.Add(XKeys.F11, XKeysNames.F11 ?? string.Empty);
				keyNames.Add(XKeys.F12, XKeysNames.F12 ?? string.Empty);
				keyNames.Add(XKeys.F13, XKeysNames.F13 ?? string.Empty);
				keyNames.Add(XKeys.F14, XKeysNames.F14 ?? string.Empty);
				keyNames.Add(XKeys.F15, XKeysNames.F15 ?? string.Empty);
				keyNames.Add(XKeys.F16, XKeysNames.F16 ?? string.Empty);
				keyNames.Add(XKeys.F17, XKeysNames.F17 ?? string.Empty);
				keyNames.Add(XKeys.F18, XKeysNames.F18 ?? string.Empty);
				keyNames.Add(XKeys.F19, XKeysNames.F19 ?? string.Empty);
				keyNames.Add(XKeys.F20, XKeysNames.F20 ?? string.Empty);
				keyNames.Add(XKeys.F21, XKeysNames.F21 ?? string.Empty);
				keyNames.Add(XKeys.F22, XKeysNames.F22 ?? string.Empty);
				keyNames.Add(XKeys.F23, XKeysNames.F23 ?? string.Empty);
				keyNames.Add(XKeys.F24, XKeysNames.F24 ?? string.Empty);
				keyNames.Add(XKeys.Help, XKeysNames.Help ?? string.Empty);
				keyNames.Add(XKeys.Home, XKeysNames.Home ?? string.Empty);
				keyNames.Add(XKeys.ImeAccept, XKeysNames.ImeAccept ?? string.Empty);
				keyNames.Add(XKeys.ImeConvert, XKeysNames.ImeConvert ?? string.Empty);
				keyNames.Add(XKeys.ImeFinalMode, XKeysNames.ImeFinalMode ?? string.Empty);
				keyNames.Add(XKeys.ImeMode1, XKeysNames.ImeMode1 ?? string.Empty);
				keyNames.Add(XKeys.ImeMode3, XKeysNames.ImeMode3 ?? string.Empty);
				keyNames.Add(XKeys.ImeMode5, XKeysNames.ImeMode5 ?? string.Empty);
				keyNames.Add(XKeys.ImeModeChange, XKeysNames.ImeModeChange ?? string.Empty);
				keyNames.Add(XKeys.ImeNonConvert, XKeysNames.ImeNonConvert ?? string.Empty);
				keyNames.Add(XKeys.Insert, XKeysNames.Insert ?? string.Empty);
				keyNames.Add(XKeys.LaunchApplication1, XKeysNames.LaunchApplication1 ?? string.Empty);
				keyNames.Add(XKeys.LaunchApplication2, XKeysNames.LaunchApplication2 ?? string.Empty);
				keyNames.Add(XKeys.LaunchMail, XKeysNames.LaunchMail ?? string.Empty);
				keyNames.Add(XKeys.Left, XKeysNames.Left ?? string.Empty);
				keyNames.Add(XKeys.LeftAltKey, XKeysNames.LeftAltKey ?? string.Empty);
				keyNames.Add(XKeys.LeftControlKey, XKeysNames.LeftControlKey ?? string.Empty);
				keyNames.Add(XKeys.LeftMouseButton, XKeysNames.LeftMouseButton ?? string.Empty);
				keyNames.Add(XKeys.LeftShiftKey, XKeysNames.LeftShiftKey ?? string.Empty);
				keyNames.Add(XKeys.LeftWinKey, XKeysNames.LeftWinKey ?? string.Empty);
				keyNames.Add(XKeys.LineFeed, XKeysNames.LineFeed ?? string.Empty);
				keyNames.Add(XKeys.MediaNextTrack, XKeysNames.MediaNextTrack ?? string.Empty);
				keyNames.Add(XKeys.MediaPlayPause, XKeysNames.MediaPlayPause ?? string.Empty);
				keyNames.Add(XKeys.MediaPreviousTrack, XKeysNames.MediaPreviousTrack ?? string.Empty);
				keyNames.Add(XKeys.MediaStop, XKeysNames.MediaStop ?? string.Empty);
				keyNames.Add(XKeys.MiddleMouseButton, XKeysNames.MiddleMouseButton ?? string.Empty);
				keyNames.Add(XKeys.Multiply, XKeysNames.Multiply ?? string.Empty);
				keyNames.Add(XKeys.NumLock, XKeysNames.NumLock ?? string.Empty);
				keyNames.Add(XKeys.NumPad0, XKeysNames.NumPad0 ?? string.Empty);
				keyNames.Add(XKeys.NumPad1, XKeysNames.NumPad1 ?? string.Empty);
				keyNames.Add(XKeys.NumPad2, XKeysNames.NumPad2 ?? string.Empty);
				keyNames.Add(XKeys.NumPad3, XKeysNames.NumPad3 ?? string.Empty);
				keyNames.Add(XKeys.NumPad4, XKeysNames.NumPad4 ?? string.Empty);
				keyNames.Add(XKeys.NumPad5, XKeysNames.NumPad5 ?? string.Empty);
				keyNames.Add(XKeys.NumPad6, XKeysNames.NumPad6 ?? string.Empty);
				keyNames.Add(XKeys.NumPad7, XKeysNames.NumPad7 ?? string.Empty);
				keyNames.Add(XKeys.NumPad8, XKeysNames.NumPad8 ?? string.Empty);
				keyNames.Add(XKeys.NumPad9, XKeysNames.NumPad9 ?? string.Empty);
				keyNames.Add(XKeys.OemBackslash, XKeysNames.OemBackslash ?? string.Empty);
				keyNames.Add(XKeys.OemClear, XKeysNames.OemClear ?? string.Empty);
				keyNames.Add(XKeys.OemCloseBrackets, XKeysNames.OemCloseBrackets ?? string.Empty);
				keyNames.Add(XKeys.OemComma, XKeysNames.OemComma ?? string.Empty);
				keyNames.Add(XKeys.OemMinus, XKeysNames.OemMinus ?? string.Empty);
				keyNames.Add(XKeys.OemOpenBrackets, XKeysNames.OemOpenBrackets ?? string.Empty);
				keyNames.Add(XKeys.OemPeriod, XKeysNames.OemPeriod ?? string.Empty);
				keyNames.Add(XKeys.OemPipe, XKeysNames.OemPipe ?? string.Empty);
				keyNames.Add(XKeys.OemPlus, XKeysNames.OemPlus ?? string.Empty);
				keyNames.Add(XKeys.OemQuestion, XKeysNames.OemQuestion ?? string.Empty);
				keyNames.Add(XKeys.OemQuotes, XKeysNames.OemQuotes ?? string.Empty);
				keyNames.Add(XKeys.OemSemicolon, XKeysNames.OemSemicolon ?? string.Empty);
				keyNames.Add(XKeys.OemTilde, XKeysNames.OemTilde ?? string.Empty);
				keyNames.Add(XKeys.Oem8, XKeysNames.Oem8 ?? string.Empty);
				keyNames.Add(XKeys.Pa1, XKeysNames.Pa1 ?? string.Empty);
				keyNames.Add(XKeys.PageDown, XKeysNames.PageDown ?? string.Empty);
				keyNames.Add(XKeys.PageUp, XKeysNames.PageUp ?? string.Empty);
				keyNames.Add(XKeys.Pause, XKeysNames.Pause ?? string.Empty);
				keyNames.Add(XKeys.Play, XKeysNames.Play ?? string.Empty);
				keyNames.Add(XKeys.Print, XKeysNames.Print ?? string.Empty);
				keyNames.Add(XKeys.PrintScreen, XKeysNames.PrintScreen ?? string.Empty);
				keyNames.Add(XKeys.ProcessKey, XKeysNames.ProcessKey ?? string.Empty);
				keyNames.Add(XKeys.Right, XKeysNames.Right ?? string.Empty);
				keyNames.Add(XKeys.RightAltKey, XKeysNames.RightAltKey ?? string.Empty);
				keyNames.Add(XKeys.RightControlKey, XKeysNames.RightControlKey ?? string.Empty);
				keyNames.Add(XKeys.RightMouseButton, XKeysNames.RightMouseButton ?? string.Empty);
				keyNames.Add(XKeys.RightShiftKey, XKeysNames.RightShiftKey ?? string.Empty);
				keyNames.Add(XKeys.RightWinKey, XKeysNames.RightWinKey ?? string.Empty);
				keyNames.Add(XKeys.ScrollLock, XKeysNames.ScrollLock ?? string.Empty);
				keyNames.Add(XKeys.Select, XKeysNames.Select ?? string.Empty);
				keyNames.Add(XKeys.SelectMedia, XKeysNames.SelectMedia ?? string.Empty);
				keyNames.Add(XKeys.Separator, XKeysNames.Separator ?? string.Empty);
				keyNames.Add(XKeys.Shift, XKeysNames.Shift ?? string.Empty);
				keyNames.Add(XKeys.ShiftKey, XKeysNames.ShiftKey ?? string.Empty);
				keyNames.Add(XKeys.Space, XKeysNames.Space ?? string.Empty);
				keyNames.Add(XKeys.Subtract, XKeysNames.Subtract ?? string.Empty);
				keyNames.Add(XKeys.Tab, XKeysNames.Tab ?? string.Empty);
				keyNames.Add(XKeys.Up, XKeysNames.Up ?? string.Empty);
				keyNames.Add(XKeys.VolumeDown, XKeysNames.VolumeDown ?? string.Empty);
				keyNames.Add(XKeys.VolumeMute, XKeysNames.VolumeMute ?? string.Empty);
				keyNames.Add(XKeys.VolumeUp, XKeysNames.VolumeUp ?? string.Empty);
				keyNames.Add(XKeys.XMouseButton1, XKeysNames.XMouseButton1 ?? string.Empty);
				keyNames.Add(XKeys.XMouseButton2, XKeysNames.XMouseButton2 ?? string.Empty);
				keyNames.Add(XKeys.Zoom, XKeysNames.Zoom ?? string.Empty);

				#endregion
			}
			finally
			{
				XKeysNames.Culture = null;
			}

			if (keyValues != null)
			{
				keyValues.Clear();
				foreach (KeyValuePair<XKeys, string> pair in keyNames)
				{
					if (keyValues.ContainsKey(pair.Value))
					{
						Platform.Log(LogLevel.Debug, "{1}(Culture={2}) has an ambiguous translation for {0}", pair.Value, typeof (XKeysNames).FullName, culture);
						continue;
					}
					keyValues.Add(pair.Value, pair.Key);
				}
			}
		}

		#region Static Helpers

		/// <summary>
		/// Gets the default instance of <see cref="XKeysConverter"/>.
		/// </summary>
		/// <remarks>
		/// This is equivalent to calling <see cref="TypeDescriptor.GetConverter(System.Type)"/> for the <see cref="XKeys"/> <see cref="Type"/>.
		/// </remarks>
		public static XKeysConverter Default
		{
			get { return TypeDescriptor.GetConverter(typeof (XKeys)) as XKeysConverter; }
		}

		/// <summary>
		/// Formats a <see cref="XKeys"/> value as a string using the <see cref="CultureInfo.CurrentUICulture">current thread's UI CultureInfo</see>.
		/// </summary>
		/// <param name="value">The <see cref="XKeys"/> value to be formatted.</param>
		/// <returns>The string representation of the given <paramref name="value"/>.</returns>
		public static string Format(XKeys value)
		{
			return Format(value, CultureInfo.CurrentUICulture);
		}

		/// <summary>
		/// Formats a <see cref="XKeys"/> value as a string using the specified <see cref="CultureInfo"/>.
		/// </summary>
		/// <param name="value">The <see cref="XKeys"/> value to be formatted.</param>
		/// <param name="culture">The <see cref="CultureInfo"/> for which the value should be formatted.</param>
		/// <returns>The string representation of the given <paramref name="value"/>.</returns>
		public static string Format(XKeys value, CultureInfo culture)
		{
			return Default.ConvertToString(null, culture, value);
		}

		/// <summary>
		/// Formats a <see cref="XKeys"/> value as a string using the <see cref="CultureInfo.InvariantCulture"/>.
		/// </summary>
		/// <param name="value">The <see cref="XKeys"/> value to be formatted.</param>
		/// <returns>The string representation of the given <paramref name="value"/>.</returns>
		public static string FormatInvariant(XKeys value)
		{
			return Format(value, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Parses a string as an <see cref="XKeys"/> value using the <see cref="CultureInfo.CurrentUICulture">current thread's UI CultureInfo</see>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <returns>The <see cref="XKeys"/> value parsed from <paramref name="s"/>.</returns>
		/// <exception cref="FormatException">Thrown if <paramref name="s"/> is not a valid <see cref="XKeys"/> string representation.</exception>
		public static XKeys Parse(string s)
		{
			return Parse(s, CultureInfo.CurrentUICulture);
		}

		/// <summary>
		/// Parses a string as an <see cref="XKeys"/> value using the specified <see cref="CultureInfo"/>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <param name="culture">The <see cref="CultureInfo"/> for which the string should be parsed.</param>
		/// <returns>The <see cref="XKeys"/> value parsed from <paramref name="s"/>.</returns>
		/// <exception cref="FormatException">Thrown if <paramref name="s"/> is not a valid <see cref="XKeys"/> string representation.</exception>
		public static XKeys Parse(string s, CultureInfo culture)
		{
			return (XKeys) Default.ConvertFromString(null, culture, s);
		}

		/// <summary>
		/// Parses a string as an <see cref="XKeys"/> value using the <see cref="CultureInfo.InvariantCulture"/>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <returns>The <see cref="XKeys"/> value parsed from <paramref name="s"/>.</returns>
		/// <exception cref="FormatException">Thrown if <paramref name="s"/> is not a valid <see cref="XKeys"/> string representation.</exception>
		public static XKeys ParseInvariant(string s)
		{
			return Parse(s, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Parses a string as an <see cref="XKeys"/> value using the <see cref="CultureInfo.CurrentUICulture">current thread's UI CultureInfo</see>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <param name="result">The <see cref="XKeys"/> value parsed from <paramref name="s"/> if the string was successfully parsed; <see cref="XKeys.None"/> otherwise.</param>
		/// <returns>True if the string was successfully parsed; False otherwise.</returns>
		public static bool TryParse(string s, out XKeys result)
		{
			return TryParse(s, CultureInfo.CurrentUICulture, out result);
		}

		/// <summary>
		/// Parses a string as an <see cref="XKeys"/> value using the specified <see cref="CultureInfo"/>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <param name="culture">The <see cref="CultureInfo"/> for which the string should be parsed.</param>
		/// <param name="result">The <see cref="XKeys"/> value parsed from <paramref name="s"/> if the string was successfully parsed; <see cref="XKeys.None"/> otherwise.</param>
		/// <returns>True if the string was successfully parsed; False otherwise.</returns>
		public static bool TryParse(string s, CultureInfo culture, out XKeys result)
		{
			try
			{
				result = Parse(s, culture);
				return true;
			}
			catch (FormatException)
			{
				result = XKeys.None;
				return false;
			}
		}

		/// <summary>
		/// Parses a string as an <see cref="XKeys"/> value using the <see cref="CultureInfo.InvariantCulture"/>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <param name="result">The <see cref="XKeys"/> value parsed from <paramref name="s"/> if the string was successfully parsed; <see cref="XKeys.None"/> otherwise.</param>
		/// <returns>True if the string was successfully parsed; False otherwise.</returns>
		public static bool TryParseInvariant(string s, out XKeys result)
		{
			return TryParse(s, CultureInfo.InvariantCulture, out result);
		}

		#endregion

		#region Unit Test Support

#if UNIT_TESTS
		internal static IEnumerable<KeyValuePair<XKeys, string>> InvariantKeyNames
		{
			get { return _invariantKeyNames; }
		}

		internal IEnumerable<KeyValuePair<XKeys, string>> LocalizedKeyNames
		{
			get { return _localizedKeyNames; }
			set
			{
				_localizedKeyNames.Clear();
				_localizedKeyValues.Clear();
				foreach (KeyValuePair<XKeys, string> pair in value)
				{
					_localizedKeyNames.Add(pair);
					_localizedKeyValues.Add(pair.Value, pair.Key);
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
		private IDictionary<XKeys, string> GetKeyNamesMap(CultureInfo culture)
		{
			if (CultureInfo.InvariantCulture.Equals(culture))
				return _invariantKeyNames;
			else if (_culture.Equals(culture))
				return _localizedKeyNames;
			else
			{
				IDictionary<XKeys, string> keyNames = new Dictionary<XKeys, string>();
				InitializeMaps(culture, keyNames, null);
				return keyNames;
			}
		}

		/// <summary>
		/// Gets the correct localization map for the specified culture.
		/// </summary>
		private IDictionary<string, XKeys> GetKeyValuesMap(CultureInfo culture)
		{
			if (CultureInfo.InvariantCulture.Equals(culture))
				return _invariantKeyValues;
			else if (_culture.Equals(culture))
				return _localizedKeyValues;
			else
			{
				IDictionary<string, XKeys> keyValues = new Dictionary<string, XKeys>();
				InitializeMaps(culture, new Dictionary<XKeys, string>(), keyValues);
				return keyValues;
			}
		}

		private static string GetKeyName(XKeys key, IDictionary<XKeys, string> map)
		{
			if (map.ContainsKey(key))
				return map[key];
			else if (Enum.IsDefined(typeof (XKeys), (int) key))
				return Enum.GetName(typeof (XKeys), (int) key);
			return string.Empty;
		}

		private static XKeys GetKeyValue(string keyName, IDictionary<string, XKeys> map)
		{
			if (map.ContainsKey(keyName))
				return map[keyName];
			else if (Enum.IsDefined(typeof (XKeys), keyName))
				return (XKeys) Enum.Parse(typeof (XKeys), keyName);
			else
				throw new FormatException(string.Format("{0} is not a valid member of {1}", keyName, typeof (XKeys).FullName));
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
					return XKeys.None;

				Match m = _keyStringParser.Match((string) value);
				if (!m.Success)
					throw new FormatException(string.Format("Input string was not in the expected format for {0}", typeof (XKeys).FullName));

				XKeys modifiers = XKeys.None;
				XKeys keyCode = XKeys.None;
				IDictionary<string, XKeys> map = GetKeyValuesMap(culture);
				for (int n = 1; n <= 4; n++)
				{
					if (m.Groups[n].Length == 0)
						break;

					XKeys keyValue = GetKeyValue(m.Groups[n].Value, map);
					if ((keyValue & XKeys.Modifiers) > 0)
					{
						modifiers |= keyValue;
					}
					else if ((keyValue & XKeys.KeyCode) > 0)
					{
						if (keyCode != XKeys.None)
							throw new FormatException(string.Format("Values for {0} may have only one non-modifier key.", typeof (XKeys).FullName));
						keyCode = keyValue;
					}
				}
				return modifiers | keyCode;
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof (XKeys))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof (string))
			{
				List<string> keyNames = new List<string>(4);
				XKeys keyValue = (XKeys) value;
				IDictionary<XKeys, string> map = GetKeyNamesMap(culture);
				if ((keyValue & XKeys.Control) == XKeys.Control)
				{
					string modifierName = GetKeyName(XKeys.Control, map);
					if (!string.IsNullOrEmpty(modifierName))
						keyNames.Add(modifierName);
				}
				if ((keyValue & XKeys.Alt) == XKeys.Alt)
				{
					string modifierName = GetKeyName(XKeys.Alt, map);
					if (!string.IsNullOrEmpty(modifierName))
						keyNames.Add(modifierName);
				}
				if ((keyValue & XKeys.Shift) == XKeys.Shift)
				{
					string modifierName = GetKeyName(XKeys.Shift, map);
					if (!string.IsNullOrEmpty(modifierName))
						keyNames.Add(modifierName);
				}
				if ((keyValue & XKeys.KeyCode) != XKeys.None)
				{
					string keyName = GetKeyName(keyValue & XKeys.KeyCode, map);
					if (!string.IsNullOrEmpty(keyName))
						keyNames.Add(keyName);
				}
				if (keyNames.Count == 0)
					return string.Empty;
				return string.Join(KeySeparator.ToString(), keyNames.ToArray());
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		#endregion
	}
}
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
using System.ComponentModel;
using System.Globalization;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Represents a combination of pressed mouse buttons and keyboard modifiers.
	/// </summary>
	[TypeConverter(typeof (XMouseButtonComboConverter))]
	public struct XMouseButtonCombo : IEquatable<XMouseButtonCombo>, IComparable<XMouseButtonCombo>
	{
		/// <summary>
		/// Represents the empty combination (no mouse buttons pressed).
		/// </summary>
		public static readonly XMouseButtonCombo None = new XMouseButtonCombo();

		private XMouseButtons _mouseButtons;
		private ModifierFlags _modifiers;

		/// <summary>
		/// Initializes an <see cref="XMouseButtonCombo"/> with the specified pressed mouse buttons.
		/// </summary>
		/// <param name="mouseButtons">The pressed mouse buttons.</param>
		public XMouseButtonCombo(XMouseButtons mouseButtons)
		{
			_mouseButtons = mouseButtons;
			_modifiers = ModifierFlags.None;
		}

		/// <summary>
		/// Initializes an <see cref="XMouseButtonCombo"/> with the specified pressed mouse buttons.
		/// </summary>
		/// <param name="mouseButtons">The pressed mouse buttons.</param>
		/// <param name="modifiers">The pressed keyboard modifiers.</param>
		public XMouseButtonCombo(XMouseButtons mouseButtons, ModifierFlags modifiers)
		{
			_mouseButtons = mouseButtons;
			_modifiers = mouseButtons != XMouseButtons.None ? modifiers : ModifierFlags.None;
		}

		/// <summary>
		/// Gets or sets the pressed mouse buttons.
		/// </summary>
		public XMouseButtons MouseButtons
		{
			get { return _mouseButtons; }
			set
			{
				_mouseButtons = value;
				_modifiers = value != XMouseButtons.None ? _modifiers : ModifierFlags.None;
			}
		}

		/// <summary>
		/// Gets or sets the pressed keyboard modifiers.
		/// </summary>
		public ModifierFlags Modifiers
		{
			get { return _modifiers; }
			set { _modifiers = value; }
		}

		/// <summary>
		/// Gets a value indicating whether or not this mouse button combination is empty.
		/// </summary>
		public bool IsEmpty
		{
			get { return Equals(None); }
		}

		/// <summary>
		/// Computes a hash code for this mouse button combination.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code of the values in this mouse button combination.</returns>
		public override int GetHashCode()
		{
			return -0x0C054525 ^ MouseButtons.GetHashCode() ^ Modifiers.GetHashCode();
		}

		/// <summary>
		/// Compares this and another mouse button combination and returns an indication of their relative values.
		/// </summary>
		/// <param name="other">Another object with which to compare.</param>
		/// <returns>A positive integer if this combination is relatively greater than the other, a negative integer if the reverse is true, or zero if the two are equal.</returns>
		public int CompareTo(XMouseButtonCombo other)
		{
			int result = MouseButtons.CompareTo(other.MouseButtons);
			if (result != 0)
				return result;
			return Modifiers.CompareTo(other.Modifiers);
		}

		/// <summary>
		/// Tests whether or not this mouse button combination and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object with which to compare.</param>
		/// <returns>True if <paramref name="obj"/> is a <see cref="XMouseButtonCombo"/> and the represented mouse button combinations are equal; False otherwise.</returns>
		public override bool Equals(object obj)
		{
			if (obj is XMouseButtonCombo)
				return Equals((XMouseButtonCombo) obj);
			return false;
		}

		/// <summary>
		/// Tests whether or not this and another mouse button combination are equal.
		/// </summary>
		/// <param name="other">Another <see cref="XMouseButtonCombo"/> with which to compare.</param>
		/// <returns>True if the represented mouse button combinations are equal; False otherwise.</returns>
		public bool Equals(XMouseButtonCombo other)
		{
			return MouseButtons == other.MouseButtons && Modifiers == other.Modifiers;
		}

		/// <summary>
		/// Formats the mouse button combination as a string using the <see cref="CultureInfo.InvariantCulture"/>.
		/// </summary>
		/// <returns>The string representation of the mouse button combination.</returns>
		public string ToInvariantString()
		{
			return ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Formats the mouse button combination as a string using the <see cref="CultureInfo.CurrentUICulture">current thread's UI CultureInfo</see>.
		/// </summary>
		/// <returns>The string representation of the mouse button combination.</returns>
		public override string ToString()
		{
			return ToString(CultureInfo.CurrentUICulture);
		}

		/// <summary>
		/// Formats the mouse button combination as a string using the specified <see cref="CultureInfo"/>.
		/// </summary>
		/// <param name="culture">The <see cref="CultureInfo"/> for which the value should be formatted.</param>
		/// <returns>The string representation of the mouse button combination.</returns>
		public string ToString(CultureInfo culture)
		{
			string mouseButtons = XMouseButtonsConverter.Format(MouseButtons, culture);
			if (Modifiers != ModifierFlags.None && MouseButtons != XMouseButtons.None)
				return string.Format("{0}{1}{2}", ModifierFlagsConverter.Format(Modifiers, culture), XMouseButtonsConverter.ButtonSeparator, mouseButtons);
			return mouseButtons;
		}

		/// <summary>
		/// Parses a string as an <see cref="XMouseButtonCombo"/> value using the <see cref="CultureInfo.InvariantCulture"/>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <returns>The <see cref="XMouseButtonCombo"/> value parsed from <paramref name="s"/>.</returns>
		/// <exception cref="FormatException">Thrown if <paramref name="s"/> is not a valid <see cref="XMouseButtonCombo"/> string representation.</exception>
		public static XMouseButtonCombo ParseInvariant(string s)
		{
			return Parse(s, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Parses a string as an <see cref="XMouseButtonCombo"/> value using the <see cref="CultureInfo.CurrentUICulture">current thread's UI CultureInfo</see>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <returns>The <see cref="XMouseButtonCombo"/> value parsed from <paramref name="s"/>.</returns>
		/// <exception cref="FormatException">Thrown if <paramref name="s"/> is not a valid <see cref="XMouseButtonCombo"/> string representation.</exception>
		public static XMouseButtonCombo Parse(string s)
		{
			return Parse(s, CultureInfo.CurrentUICulture);
		}

		/// <summary>
		/// Parses a string as an <see cref="XMouseButtonCombo"/> value using the specified <see cref="CultureInfo"/>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <param name="culture">The <see cref="CultureInfo"/> for which the string should be parsed.</param>
		/// <returns>The <see cref="XMouseButtonCombo"/> value parsed from <paramref name="s"/>.</returns>
		/// <exception cref="FormatException">Thrown if <paramref name="s"/> is not a valid <see cref="XMouseButtonCombo"/> string representation.</exception>
		public static XMouseButtonCombo Parse(string s, CultureInfo culture)
		{
			XMouseButtonCombo result;
			if (TryParse(s, culture, out result))
				return result;
			throw new FormatException(string.Format("Input string was not in the expected format for {0}", typeof (XMouseButtonCombo).FullName));
		}

		/// <summary>
		/// Parses a string as an <see cref="XMouseButtonCombo"/> value using the <see cref="CultureInfo.InvariantCulture"/>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <param name="result">The <see cref="XMouseButtonCombo"/> value parsed from <paramref name="s"/>.</param>
		/// <returns>True if the string was successfully parsed; False otherwise.</returns>
		public static bool TryParseInvariant(string s, out XMouseButtonCombo result)
		{
			return TryParse(s, CultureInfo.InvariantCulture, out result);
		}

		/// <summary>
		/// Parses a string as an <see cref="XMouseButtonCombo"/> value using the <see cref="CultureInfo.CurrentUICulture">current thread's UI CultureInfo</see>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <param name="result">The <see cref="XMouseButtonCombo"/> value parsed from <paramref name="s"/>.</param>
		/// <returns>True if the string was successfully parsed; False otherwise.</returns>
		public static bool TryParse(string s, out XMouseButtonCombo result)
		{
			return TryParse(s, CultureInfo.CurrentUICulture, out result);
		}

		/// <summary>
		/// Parses a string as an <see cref="XMouseButtonCombo"/> value using the specified <see cref="CultureInfo"/>.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <param name="culture">The <see cref="CultureInfo"/> for which the string should be parsed.</param>
		/// <param name="result">The <see cref="XMouseButtonCombo"/> value parsed from <paramref name="s"/>.</param>
		/// <returns>True if the string was successfully parsed; False otherwise.</returns>
		public static bool TryParse(string s, CultureInfo culture, out XMouseButtonCombo result)
		{
			if (string.IsNullOrEmpty(s))
			{
				result = None;
				return true;
			}

			XMouseButtons mouseButtons;
			if (XMouseButtonsConverter.TryParse(s, culture, out mouseButtons))
			{
				result = new XMouseButtonCombo(mouseButtons);
				return true;
			}

			if (s.Length > 1)
			{
				// we want trailing and leading separators to fail the conversion
				int lastIndex = s.Length - 1;
				while ((lastIndex = s.LastIndexOf(XMouseButtonsConverter.ButtonSeparator, lastIndex - 1)) > 0)
				{
					ModifierFlags modifiers;
					if (XMouseButtonsConverter.TryParse(s.Substring(lastIndex + 1), culture, out mouseButtons) &&
					    ModifierFlagsConverter.TryParse(s.Substring(0, lastIndex), culture, out modifiers))
					{
						result = new XMouseButtonCombo(mouseButtons, modifiers);
						return true;
					}
				}
			}

			result = None;
			return false;
		}

		/// <summary>
		/// Tests whether or not two mouse button combinations are equal.
		/// </summary>
		/// <param name="x">One mouse button combination.</param>
		/// <param name="y">The other mouse button combination.</param>
		/// <returns>True if the two mouse button combinations are equal; False otherwise.</returns>
		public static bool operator ==(XMouseButtonCombo x, XMouseButtonCombo y)
		{
			return x.Equals(y);
		}

		/// <summary>
		/// Tests whether or not two mouse button combinations are not equal.
		/// </summary>
		/// <param name="x">One mouse button combination.</param>
		/// <param name="y">The other mouse button combination.</param>
		/// <returns>True if the two mouse button combinations are not equal; False otherwise.</returns>
		public static bool operator !=(XMouseButtonCombo x, XMouseButtonCombo y)
		{
			return !x.Equals(y);
		}

		/// <summary>
		/// Implicitly casts an <see cref="XMouseButtons"/> flags combination as the equivalent <see cref="XMouseButtonCombo"/>.
		/// </summary>
		/// <param name="value">An <see cref="XMouseButtons"/> flags value.</param>
		/// <returns>The equivalent <see cref="XMouseButtonCombo"/> value.</returns>
		public static implicit operator XMouseButtonCombo(XMouseButtons value)
		{
			return new XMouseButtonCombo(value);
		}

		/// <summary>
		/// Explicitly casts an <see cref="XKeys"/> value as the equivalent <see cref="XMouseButtonCombo"/>.
		/// </summary>
		/// <param name="value">An <see cref="XKeys"/> value.</param>
		/// <returns>The equivalent <see cref="XMouseButtonCombo"/> value.</returns>
		/// <exception cref="InvalidCastException">Thrown if <paramref name="value"/> does not represent a mouse button combination.</exception>
		public static explicit operator XMouseButtonCombo(XKeys value)
		{
			XMouseButtons mouseButtons;
			switch (value & XKeys.KeyCode)
			{
				case XKeys.LeftMouseButton:
					mouseButtons = XMouseButtons.Left;
					break;
				case XKeys.RightMouseButton:
					mouseButtons = XMouseButtons.Right;
					break;
				case XKeys.MiddleMouseButton:
					mouseButtons = XMouseButtons.Middle;
					break;
				case XKeys.XMouseButton1:
					mouseButtons = XMouseButtons.XButton1;
					break;
				case XKeys.XMouseButton2:
					mouseButtons = XMouseButtons.XButton2;
					break;
				case XKeys.None:
					mouseButtons = XMouseButtons.None;
					break;
				default:
					throw new InvalidCastException(string.Format("Input key stroke is not a valid value for {0}", typeof (XMouseButtons).FullName));
			}

			ModifierFlags modifiers = ModifierFlags.None;
			if ((value & XKeys.Control) == XKeys.Control)
				modifiers |= ModifierFlags.Control;
			if ((value & XKeys.Alt) == XKeys.Alt)
				modifiers |= ModifierFlags.Alt;
			if ((value & XKeys.Shift) == XKeys.Shift)
				modifiers |= ModifierFlags.Shift;

			return new XMouseButtonCombo(mouseButtons, modifiers);
		}

		/// <summary>
		/// Explicitly casts an <see cref="XMouseButtonCombo"/> value as the equivalent <see cref="XKeys"/>.
		/// </summary>
		/// <param name="value">An <see cref="XMouseButtonCombo"/> value.</param>
		/// <returns>The equivalent <see cref="XKeys"/> value.</returns>
		/// <exception cref="InvalidCastException">Thrown if <paramref name="value"/> has more than one pressed mouse button and thus cannot be represented as a single <see cref="XKeys"/> value.</exception>
		public static explicit operator XKeys(XMouseButtonCombo value)
		{
			XKeys keys;
			switch (value.MouseButtons)
			{
				case XMouseButtons.Left:
					keys = XKeys.LeftMouseButton;
					break;
				case XMouseButtons.Right:
					keys = XKeys.RightMouseButton;
					break;
				case XMouseButtons.Middle:
					keys = XKeys.MiddleMouseButton;
					break;
				case XMouseButtons.XButton1:
					keys = XKeys.XMouseButton1;
					break;
				case XMouseButtons.XButton2:
					keys = XKeys.XMouseButton2;
					break;
				case XMouseButtons.None:
					keys = XKeys.None;
					break;
				default:
					throw new InvalidCastException(string.Format("Input combination has more than one mouse button and cannot be represented as an {0}.", typeof (XKeys).FullName));
			}

			if ((value.Modifiers & ModifierFlags.Control) == ModifierFlags.Control)
				keys |= XKeys.Control;
			if ((value.Modifiers & ModifierFlags.Alt) == ModifierFlags.Alt)
				keys |= XKeys.Alt;
			if ((value.Modifiers & ModifierFlags.Shift) == ModifierFlags.Shift)
				keys |= XKeys.Shift;

			return keys;
		}

		private class XMouseButtonComboConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				if (sourceType == typeof (string))
					return true;
				else if (sourceType == typeof (XMouseButtons))
					return true;
				else if (sourceType == typeof (XKeys))
					return true;
				return base.CanConvertFrom(context, sourceType);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				if (value is string)
					return Parse((string) value, culture);
				else if (value is XMouseButtons)
					return (XMouseButtonCombo) (XMouseButtons) value;
				else if (value is XKeys)
					return (XMouseButtonCombo) (XKeys) value;
				return base.ConvertFrom(context, culture, value);
			}

			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				if (destinationType == typeof (string))
					return true;
				else if (destinationType == typeof (XKeys))
					return true;
				return base.CanConvertTo(context, destinationType);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				if (destinationType == typeof (string))
					return ((XMouseButtonCombo) value).ToString(culture);
				else if (destinationType == typeof (XKeys))
					return (XKeys) (XMouseButtonCombo) value;
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
	}
}
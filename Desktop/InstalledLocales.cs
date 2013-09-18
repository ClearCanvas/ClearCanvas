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
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Represents the collection of installed localizations available for use in the application.
	/// </summary>
	public sealed class InstalledLocales : IXmlSerializable
	{
		private const string _elementLocale = "Locale";
		private const string _attributeCulture = "Culture";
		private const string _attributeDefault = "Default";
		private const string _attributeDisplayName = "DisplayName";

		private static InstalledLocales _instance;

		/// <summary>
		/// Gets a <see cref="Locale"/> representing the invariant locale.
		/// </summary>
		public static readonly Locale InvariantLocale = new Locale(string.Empty, null);

		/// <summary>
		/// Gets the default instance of <see cref="InstalledLocales"/> from the application settings.
		/// </summary>
		public static InstalledLocales Instance
		{
			get
			{
				if (_instance == null)
				{
					try
					{
						_instance = LocaleSettings.Default.InstalledLocales;
					}
					catch (Exception ex)
					{
						Platform.Log(LogLevel.Debug, ex, "Unexpected error reading {0}", typeof (LocaleSettings));
					}
					_instance = _instance ?? new InstalledLocales();
				}
				return _instance;
			}
		}

		private readonly List<Locale> _installedLocales = new List<Locale>();
		private string _defaultLocale;

		#region Public API

		/// <summary>
		/// Gets the count of installed localizations.
		/// </summary>
		public int Count
		{
			get { return _installedLocales.Count; }
		}

		/// <summary>
		/// Enumerates the installed localizations.
		/// </summary>
		public IEnumerable<Locale> Locales
		{
			get { return _installedLocales.AsReadOnly(); }
		}

		/// <summary>
		/// Enumerates the subset of installed localizations that are allowed by the locale policy setting.
		/// </summary>
		public IEnumerable<Locale> AllowedLocales
		{
			get { return ApplyLocalePolicy(_installedLocales).ToList().AsReadOnly(); }
		}

		/// <summary>
		/// Gets the default <see cref="Locale"/> of the installed localizations.
		/// </summary>
		public Locale Default
		{
			get { return ((!string.IsNullOrEmpty(_defaultLocale) ? Find(_defaultLocale) : null) ?? FindBestMatchSystemLocale(_installedLocales)) ?? InvariantLocale; }
		}

		/// <summary>
		/// Gets or sets the selected <see cref="Locale"/> for the current system user.
		/// </summary>
		/// <remarks>
		/// This value only persists in the local configuration file for the system user and not for the login user as defined by the application
		/// because the availability of locales is determined by the installation, and never by a central settings store.
		/// </remarks>
		public Locale Selected
		{
			get
			{
				try
				{
					var selected = LocaleSettings.Default.SelectedLocale;
					if (!string.IsNullOrEmpty(selected))
						return Find(selected) ?? Default;
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Debug, ex, "Unable to read SelectedLocale from settings");
				}
				return Default;
			}
			set
			{
				try
				{
					var selected = Default.Culture;
					if (value != null)
						selected = value.Culture;
					LocaleSettings.Default.SelectedLocale = selected;
					LocaleSettings.Default.Save();
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Debug, ex, "Unable to save SelectedLocale to settings");
				}
			}
		}

		/// <summary>
		/// Finds an installed localization with the specified culture code.
		/// </summary>
		public Locale Find(string culture)
		{
			return _installedLocales.FirstOrDefault(x => string.Equals(culture, x.Culture, StringComparison.InvariantCultureIgnoreCase));
		}

		#endregion

		#region Helpers

		private static IEnumerable<Locale> ApplyLocalePolicy(IEnumerable<Locale> locales)
		{
			var allowed = (LocalePolicy.Default.AllowedLocalizationsList ?? string.Empty).Split(new[] {','})
				.Select(i => i.Trim())
				.Where(i => !string.IsNullOrEmpty(i))
				.ToList();

			// if nothing is explicitly specified, then everything is allowed
			if (allowed.Count <= 0)
				return locales;

			// return a filtered list
			return from locale in locales
			       where locale == InvariantLocale || locale.Culture == @"en" || allowed.Contains(locale.Culture)
			       select locale;
		}

		private static Locale FindBestMatchSystemLocale(IEnumerable<Locale> locales)
		{
			var culture = CultureInfo.CurrentUICulture;
			while (culture != CultureInfo.InvariantCulture)
			{
				var cultureCode = culture.Name;
				var locale = CollectionUtils.SelectFirst(locales, l => l.Culture == cultureCode);
				if (locale != null)
					return locale;

				culture = culture.Parent;
			}
			return null;
		}

		#endregion

		#region IXmlSerializable Members

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			if (reader.MoveToContent() != XmlNodeType.Element)
				throw new InvalidOperationException();

			var emptyCollection = reader.IsEmptyElement; // check if container element is empty
			var locales = new Dictionary<string, Locale>();

			_defaultLocale = reader.GetAttribute(_attributeDefault);
			reader.ReadStartElement(); // consume the container element tag
			if (!emptyCollection)
			{
				// children of container should each be elements representing individual collection items
				// at the end of this loop, we must be positioned past each item, whether an end element or an empty element tag
				while (reader.MoveToContent() == XmlNodeType.Element)
				{
					var emptyItem = reader.IsEmptyElement;
					var locale = (Locale) null;

					if (reader.Name == _elementLocale)
					{
						try
						{
							var culture = reader.GetAttribute(_attributeCulture);
							var displayName = reader.GetAttribute(_attributeDisplayName);
							locale = new Locale(culture, displayName);
						}
						catch (Exception ex)
						{
							Platform.Log(LogLevel.Debug, ex, "Invalid installed locale definition");
						}

						// consume the entire element
						if (!emptyItem)
						{
							reader.ReadSubtree().Close(); // read start tag and all descendants
							reader.ReadEndElement(); // read end tag
						}
						else
						{
							reader.ReadStartElement(); // read empty element tag
						}
					}
					else
					{
						// if the child element is unrecognized, we still have to consume it!
						//  otherwise we will not be at the right position to pick up the next item
						if (!emptyItem)
						{
							reader.ReadSubtree().Close(); // read start tag and all descendants
							reader.ReadEndElement(); // read end tag
						}
						else
						{
							reader.ReadStartElement(); // read empty element tag
						}
					}

					// ignore duplicate entries
					if (locale != null && !locales.ContainsKey(locale.Culture.ToLowerInvariant()))
						locales.Add(locale.Culture.ToLowerInvariant(), locale);
				}
			}

			_installedLocales.Clear();
			_installedLocales.AddRange(locales.Values);
			_installedLocales.Sort((x, y) => string.Compare(x.DisplayName, y.DisplayName, StringComparison.InvariantCultureIgnoreCase));
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			foreach (var locale in _installedLocales)
			{
				writer.WriteStartElement(_elementLocale);
				writer.WriteAttributeString(_attributeCulture, locale.Culture);
				writer.WriteAttributeString(_attributeDisplayName, locale.DisplayName);
				writer.WriteEndElement();
			}
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		#endregion

		#region Locale Class

		/// <summary>
		/// Represents a single installed localization.
		/// </summary>
		public sealed class Locale : IEquatable<Locale>
		{
			/// <summary>
			/// Gets the culture code of the localization.
			/// </summary>
			public readonly string Culture;

			/// <summary>
			/// Gets the name of the localization in the localization's own language and script.
			/// </summary>
			public readonly string Name;

			/// <summary>
			/// Gets the display name of the localization.
			/// </summary>
			/// <remarks>
			/// This value is equal to <see cref="Name"/> by default, but can be overridden in the settings.
			/// </remarks>
			public readonly string DisplayName;

			/// <summary>
			/// Gets the name of the localization in the invariant language (i.e. English).
			/// </summary>
			public readonly string InvariantName;

			internal Locale(string culture, string displayName)
			{
				var cultureInfo = !string.IsNullOrEmpty(culture) ? CultureInfo.GetCultureInfo(culture) : CultureInfo.InvariantCulture;
				Culture = cultureInfo.Name;
				Name = cultureInfo.NativeName;
				InvariantName = cultureInfo.EnglishName;
				DisplayName = !string.IsNullOrEmpty(displayName) ? displayName : Name;
			}

			/// <summary>
			/// Gets a <see cref="CultureInfo"/> representing this locale.
			/// </summary>
			public CultureInfo GetCultureInfo()
			{
				return CultureInfo.GetCultureInfo(Culture);
			}

			public override string ToString()
			{
				return DisplayName;
			}

			public override int GetHashCode()
			{
				return 0x0F5E0071 ^ Culture.GetHashCode();
			}

			/// <summary>
			/// Checks if this locale is equivalent to <paramref name="object"/>.
			/// </summary>
			public override bool Equals(object @object)
			{
				return @object is Locale && Equals((Locale) @object);
			}

			/// <summary>
			/// Checks if this locale is equivalent to <paramref name="other"/>.
			/// </summary>
			public bool Equals(Locale other)
			{
				return !ReferenceEquals(other, null) && string.Equals(Culture, other.Culture, StringComparison.InvariantCultureIgnoreCase);
			}

			/// <summary>
			/// Checks if the locales <paramref name="x"/> and <paramref name="y"/> are equivalent.
			/// </summary>
			public static bool operator ==(Locale x, Locale y)
			{
				return !ReferenceEquals(x, null) ? x.Equals(y) : ReferenceEquals(y, null);
			}

			/// <summary>
			/// Checks if the locales <paramref name="x"/> and <paramref name="y"/> are not equivalent.
			/// </summary>
			public static bool operator !=(Locale x, Locale y)
			{
				return !(x == y);
			}
		}

		#endregion
	}
}
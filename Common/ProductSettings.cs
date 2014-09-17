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
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common
{
	[ExtensionOf(typeof (ApplicationRootExtensionPoint))]
	internal sealed class DumpProductSettingsApplication : IApplicationRoot
	{
		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			Dump();
		}

		private static void Dump()
		{
// ReSharper disable LocalizableElement
			var settings = new DecryptedProductSettings();
			Console.WriteLine(@"Name: {0}", settings.Name);
			Console.WriteLine(@"Family: {0}", settings.FamilyName);
			Console.WriteLine(@"Product: {0}", settings.Product);
			Console.WriteLine(@"Component: {0}", settings.Component);
			Console.WriteLine(@"SubComponent: {0}", settings.SubComponent);
			Console.WriteLine(@"Edition: {0}", settings.DisplayEdition);
			Console.WriteLine(@"Version: {0}", settings.Version);
			Console.WriteLine(@"VersionSuffix: {0}", settings.VersionSuffix);
			Console.WriteLine(@"Release: {0}", settings.Release);
			Console.WriteLine(@"Copyright:\n{0}", settings.Copyright);
			Console.WriteLine(@"\nLicense:\n{0}", settings.License);
// ReSharper restore LocalizableElement
		}

		#endregion
	}

	/// <summary>
	/// Provides some basic information about the product, such as the name and version.
	/// </summary>
	internal class DecryptedProductSettings
	{
		private string _name;
		private string _family;
		private string _product;
		private string _component;
		private string _subComponent;
		private string _edition;
		private string _release;
		private Version _version;
		private string _versionSuffix;
		private string _copyright;
		private string _license;

		private readonly ProductSettings _settings;

		public DecryptedProductSettings()
		{
			_settings = ((ProductSettings) (SettingsBase.Synchronized(new ProductSettings())));
		}

		/// <summary>
		/// Gets the component marketing name.
		/// </summary>
		public string Name
		{
			get
			{
				if (_name == null)
					_name = Decrypt(_settings.Name);
				return _name;
			}
		}

		/// <summary>
		/// Gets the product's family name.
		/// </summary>
		public string FamilyName
		{
			get
			{
				if (_family == null)
					_family = Decrypt(_settings.FamilyName);
				return _family;
			}
		}

		/// <summary>
		/// Gets the component name.
		/// </summary>
		public string Component
		{
			get
			{
				if (_component == null)
					_component = Decrypt(_settings.Component);
				return _component;
			}
		}

		/// <summary>
		/// Gets the Subcomponent name.
		/// </summary>
		public string SubComponent
		{
			get
			{
				if (_subComponent == null)
					_subComponent = Decrypt(_settings.SubComponent);
				return _subComponent;
			}
		}

		/// <summary>
		/// Gets the product name.
		/// </summary>
		public string Product
		{
			get
			{
				if (_product == null)
					_product = Decrypt(_settings.Product);
				return _product;
			}
		}

		/// <summary>
		/// Gets the product edition.
		/// </summary>
		public string Edition
		{
			get
			{
				if (_edition == null)
					_edition = Decrypt(_settings.Edition);
				return _edition;
			}
		}

		/// <summary>
		/// Gets the product release type.
		/// </summary>
		public string Release
		{
			get
			{
				if (_release == null)
				{
					var release = Decrypt(_settings.Release);
					_release = string.IsNullOrEmpty(release) || release[0] != '*' ? @"Unofficial" : release.Substring(1);
				}
				return _release;
			}
		}

		/// <summary>
		/// Gets the product version.
		/// </summary>
		public Version Version
		{
			get
			{
				if (_version == null)
				{
					string version = Decrypt(_settings.Version);
					try
					{
						if (String.IsNullOrEmpty(version))
							_version = Assembly.GetExecutingAssembly().GetName().Version;
						else
							_version = new Version(version);
					}
					catch (Exception)
					{
						// don't allow a poorly formatted version string to kill the app
						_version = Assembly.GetExecutingAssembly().GetName().Version;
					}
				}
				return _version;
			}
		}

		/// <summary>
		/// Gets the product version suffix (e.g. "SP1").
		/// </summary>
		public string VersionSuffix
		{
			get
			{
				if (_versionSuffix == null)
				{
					string versionSuffix = Decrypt(_settings.VersionSuffix);
					_versionSuffix = string.IsNullOrEmpty(versionSuffix) || versionSuffix[0] != '*' ? @"Unofficial Build" : versionSuffix.Substring(1);
				}
				return _versionSuffix;
			}
		}

		/// <summary>
		/// Gets the product copyright (e.g. "Copyright 2009 ClearCanvas Inc.").
		/// </summary>
		public string Copyright
		{
			get
			{
				if (_copyright == null)
					_copyright = Decrypt(_settings.Copyright);
				return _copyright;
			}
		}

		/// <summary>
		/// Gets the product license.
		/// </summary>
		public string License
		{
			get
			{
				if (_license == null)
					_license = Decrypt(_settings.License);
				return _license;
			}
		}

		/// <summary>
		/// Gets the product edition for display.
		/// </summary>
		public string DisplayEdition
		{
			get
			{
				var edition = Edition;
				return edition != "<empty>" ? edition : string.Empty;
			}
		}

		private static string Decrypt(string @string)
		{
			if (String.IsNullOrEmpty(@string))
				return @string;

			string result;
			try
			{
				using (var dataStream = new MemoryStream(Convert.FromBase64String(@string)))
				using (var cryptoService = new XorCryptoServiceProvider {Key = Encoding.UTF8.GetBytes(@"ClearCanvas"), IV = Encoding.UTF8.GetBytes(@"IsSoCool")})
				using (var cryptoStream = new CryptoStream(dataStream, cryptoService.CreateDecryptor(), CryptoStreamMode.Read))
				using (var reader = new StreamReader(cryptoStream, Encoding.UTF8))
				{
					result = reader.ReadToEnd().TrimEnd('\0');
				}
			}
			catch (Exception)
			{
				result = string.Empty;
			}
			return result;
		}
	}

	/// <summary>
	/// Utility class for reading product identity information.
	/// </summary>
	public static class ProductInformation
	{
		private static readonly DecryptedProductSettings _settings = new DecryptedProductSettings();

		/// <summary>
		/// Gets the component marketing name.
		/// </summary>
		public static string Name
		{
			get
			{
				var name = _settings.Name;
				return !string.IsNullOrEmpty(name) ? name : _settings.Component;
			}
		}

		/// <summary>
		/// Gets the product's family name.
		/// </summary>
		public static string FamilyName
		{
			get { return _settings.FamilyName; }
		}

		/// <summary>
		/// Gets the component name.
		/// </summary>
		public static string Component
		{
			get { return _settings.Component; }
		}

		/// <summary>
		/// Gets the subcomponent name.
		/// </summary>
		public static string SubComponent
		{
			get { return _settings.SubComponent; }
		}

		/// <summary>
		/// Gets the product name.
		/// </summary>
		public static string Product
		{
			get { return _settings.Product; }
		}

		/// <summary>
		/// Gets the product edition.
		/// </summary>
		public static string Edition
		{
			get { return _settings.Edition; }
		}

		/// <summary>
		/// Gets the product release type.
		/// </summary>
		public static string Release
		{
			get { return _settings.Release; }
		}

		/// <summary>
		/// Gets the product version.
		/// </summary>
		public static Version Version
		{
			get { return _settings.Version; }
		}

		/// <summary>
		/// Gets the product version suffix (e.g. "SP1").
		/// </summary>
		public static string VersionSuffix
		{
			get { return _settings.VersionSuffix; }
		}

		/// <summary>
		/// Gets the product copyright (e.g. "Copyright 2009 ClearCanvas Inc.").
		/// </summary>
		public static string Copyright
		{
			get { return _settings.Copyright; }
		}

		/// <summary>
		/// Gets the product license.
		/// </summary>
		public static string License
		{
			get { return _settings.License; }
		}

		/// <summary>
		/// Gets the product display edition.
		/// </summary>
		public static string DisplayEdition
		{
			get { return _settings.DisplayEdition; }
		}

		/// <summary>
		/// Gets the component name, optionally with the product edition and/or release type.
		/// </summary>
		/// <param name="includeEdition">A value indciating whether or not to include the product edition in the name.</param>
		/// <param name="includeRelease">A value indicating whether or not to include the release type in the name.</param>
		public static string GetName(bool includeEdition, bool includeRelease)
		{
			return Concatenate(Name, GetNameSuffix(includeEdition, includeRelease));
		}

		/// <summary>
		/// Gets the suffixes to the component name (i.e. the product edition and/or release type).
		/// </summary>
		/// <param name="includeEdition">A value indciating whether or not to include the product edition in the name.</param>
		/// <param name="includeRelease">A value indicating whether or not to include the release type in the name.</param>
		private static string GetNameSuffix(bool includeEdition, bool includeRelease)
		{
			return Concatenate(includeEdition ? DisplayEdition : string.Empty, includeRelease && !string.IsNullOrEmpty(Release) ? string.Format(SR.FormatReleaseType, Release) : string.Empty);
		}

		/// <summary>
		/// Gets a string containing both the component name, product edition, release type and version.
		/// </summary>
		/// <param name="includeBuildAndRevision">A value indicating whether or not to include the build and revision numbers in the version; False means only the major and minor numbers are included.</param>
		/// <param name="includeVersionSuffix">A value indicating whether or not to include the version suffix.</param>
		public static string GetNameAndVersion(bool includeBuildAndRevision, bool includeVersionSuffix)
		{
			return GetNameAndVersion(includeBuildAndRevision, includeVersionSuffix, true, true);
		}

		/// <summary>
		/// Gets a string containing both the component name and version, optionally with the product edition and/or release type.
		/// </summary>
		/// <param name="includeBuildAndRevision">A value indicating whether or not to include the build and revision numbers in the version; False means only the major and minor numbers are included.</param>
		/// <param name="includeVersionSuffix">A value indicating whether or not to include the version suffix.</param>
		/// <param name="includeEdition">A value indciating whether or not to include the product edition in the name.</param>
		/// <param name="includeRelease">A value indicating whether or not to include the release type in the name.</param>
		public static string GetNameAndVersion(bool includeBuildAndRevision, bool includeVersionSuffix, bool includeEdition, bool includeRelease)
		{
			return Concatenate(Name, string.Format("v{0}", GetVersion(includeBuildAndRevision, includeVersionSuffix, false)), GetNameSuffix(includeEdition, includeRelease));
		}

		/// <summary>
		/// Gets the version as a string, optionally with build and revision numbers, and/or version suffix.
		/// </summary>
		/// <param name="includeBuildAndRevision">Specifies whether or not to include the build and revision numbers in the version; false means only the major and minor numbers are included.</param>
		/// <param name="includeVersionSuffix">Specifies whether or not to include the version suffix.</param>
		public static string GetVersion(bool includeBuildAndRevision, bool includeVersionSuffix)
		{
			return GetVersion(includeBuildAndRevision, includeVersionSuffix, false);
		}

		/// <summary>
		/// Gets the version as a string, optionally with build and revision numbers, version suffix, and/or release type.
		/// </summary>
		/// <param name="includeBuildAndRevision">Specifies whether or not to include the build and revision numbers in the version; false means only the major and minor numbers are included.</param>
		/// <param name="includeVersionSuffix">Specifies whether or not to include the version suffix.</param>
		/// <param name="includeRelease">Specifies whether or not to include the release type in the string.</param>
		/// <param name="includeEdition">Specifies whether or not to include the edition name in the string.</param>
		public static string GetVersion(bool includeBuildAndRevision, bool includeVersionSuffix, bool includeRelease, bool includeEdition = false)
		{
			var version = Version;
			var versionString = new StringBuilder(string.Format(@"{0}.{1}", version.Major, version.Minor));

			if (includeBuildAndRevision && version.Build >= 0)
			{
				versionString.AppendFormat(@".{0}", version.Build);
				if (version.Revision >= 0)
					versionString.AppendFormat(@".{0}", version.Revision);
			}

			return Concatenate(includeEdition ? DisplayEdition : string.Empty, versionString.ToString(), includeVersionSuffix ? VersionSuffix : string.Empty, includeRelease ? Release : string.Empty);
		}

		public static bool IsEvaluation
		{
			get
			{
				TimeSpan? ignore;
				return LicenseInformation.GetTrialStatus(out ignore);
			}
		}

		/// <summary>
		/// Concatenates a number of strings with spaces, skipping empty strings.
		/// </summary>
		private static string Concatenate(params string[] strings)
		{
			if (strings == null || strings.Length == 0)
				return string.Empty;

			var sb = new StringBuilder();
			foreach (var s in strings)
			{
				if (string.IsNullOrEmpty(s))
					continue;

				if (sb.Length > 0)
					sb.Append(' ');
				sb.Append(s);
			}
			return sb.ToString();
		}
	}

	[SettingsGroupDescription("Settings that describe the product, such as the product name and version.")]
	[SettingsProvider(typeof (ApplicationCriticalSettingsProvider))]
	[SharedSettingsMigrationDisabled]
	internal sealed partial class ProductSettings
	{
		internal ProductSettings() {}
	}
}
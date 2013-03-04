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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Provides access to product licensing information.
	/// </summary>
	public static class LicenseInformation
	{
		private static readonly object _syncRoot = new object();
		private static event EventHandler _licenseChanged;

		private static ILicenseProvider _licenseProvider;
		private static ILicenseDetailsProvider _licenseDetailsProvider;

		private static string _licenseKey;
		private static string _machineIdentifier;

		private static void CheckLicenseProvider()
		{
			if (_licenseProvider != null) return;

			lock (_syncRoot)
			{
				if (_licenseProvider != null) return;
				_licenseProvider = LicenseProviderExtensionPoint.CreateInstance();
				_licenseProvider.LicenseInfoChanged += (s, e) => EventsHelper.Fire(_licenseChanged, null, e);
			}
		}

		private static void CheckLicenseDetailsProvider()
		{
			if (_licenseDetailsProvider != null) return;

			lock (_syncRoot)
			{
				if (_licenseDetailsProvider != null) return;
				_licenseDetailsProvider = LicenseDetailsProviderExtensionPoint.CreateInstance();
			}
		}

		/// <summary>
		/// Notifies when the license information has changed.
		/// </summary>
		public static event EventHandler LicenseChanged
		{
			add { _licenseChanged += value; }
			remove { _licenseChanged -= value; }
		}

		/// <summary>
		/// Gets a unique identifier for the installation.
		/// </summary>
		public static string MachineIdentifier
		{
			get
			{
				CheckLicenseProvider();

				lock (_syncRoot)
				{
					if (_machineIdentifier == null)
					{
						_licenseProvider.GetLicenseInfo(out _licenseKey, out _machineIdentifier);
					}
					return _machineIdentifier;
				}
			}
		}

		/// <summary>
		/// Gets or sets the product license key.
		/// </summary>
		public static string LicenseKey
		{
			get
			{
				CheckLicenseProvider();

				lock (_syncRoot)
				{
					// don't cache this result - we want to know if license key changes
					_licenseProvider.GetLicenseInfo(out _licenseKey, out _machineIdentifier);
					return _licenseKey;
				}
			}
			set
			{
				CheckLicenseProvider();

				lock (_syncRoot)
				{
					_licenseKey = value;
					_licenseProvider.SetLicenseInfo(_licenseKey);
				}
			}
		}

		/// <summary>
		/// Gets a string indicating the product license type.
		/// </summary>
		public static string LicenseType
		{
			get
			{
				CheckLicenseDetailsProvider();

				lock (_syncRoot)
				{
					return _licenseDetailsProvider.LicenseType ?? string.Empty;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating the licensed diagnostic use of the product.
		/// </summary>
		public static LicenseDiagnosticUse DiagnosticUse
		{
			get
			{
				CheckLicenseDetailsProvider();

				lock (_syncRoot)
				{
					return _licenseDetailsProvider.DiagnosticUse;
				}
			}
		}

		/// <summary>
		/// Gets the date when the product was first run.
		/// </summary>
		public static DateTime? FirstRun
		{
			get
			{
				CheckLicenseDetailsProvider();

				lock (_syncRoot)
				{
					return _licenseDetailsProvider.FirstRun;
				}
			}
		}

		/// <summary>
		/// Gets the status if the license is for a limited-use trial.
		/// </summary>
		/// <param name="timeRemaining">Time remaining in trial period.</param>
		/// <returns>True if license is for a limited-use trial; False otherwise.</returns>
		public static bool GetTrialStatus(out TimeSpan? timeRemaining)
		{
			CheckLicenseDetailsProvider();

			lock (_syncRoot)
			{
				return _licenseDetailsProvider.GetTrialStatus(out timeRemaining);
			}
		}

		/// <summary>
		/// Checks if a specific feature is authorized by the license.
		/// </summary>
		/// <param name="featureToken"></param>
		/// <returns></returns>
		public static bool IsFeatureAuthorized(string featureToken)
		{
			if (string.IsNullOrEmpty(featureToken)) return true;

			CheckLicenseDetailsProvider();

			lock (_syncRoot)
			{
				return _licenseDetailsProvider.IsFeatureAuthorized(featureToken);
			}
		}

		public static DateTime? ExpiryTime
		{
			get
			{
				CheckLicenseDetailsProvider();

				lock (_syncRoot)
				{
					return _licenseDetailsProvider.GetExpiryDate();
				}
			}
		}

		/// <summary>
		/// Forces license information to be reloaded when it is requested next time
		/// </summary>
		public static void Reset()
		{
			_machineIdentifier = null; // will force reload when requested
		}
	}
}
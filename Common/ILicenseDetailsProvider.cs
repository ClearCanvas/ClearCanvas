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

namespace ClearCanvas.Common
{
	/// <summary>
	/// Represents a license details provider.
	/// </summary>
	/// <remarks>
	/// For internal framework use only.
	/// </remarks>
	public interface ILicenseDetailsProvider
	{
		/// <summary>
		/// Gets a string indicating license type.
		/// </summary>
		string LicenseType { get; }

		/// <summary>
		/// Gets the licensed diagnostic use.
		/// </summary>
		LicenseDiagnosticUse DiagnosticUse { get; }

		/// <summary>
		/// Gets the first run date.
		/// </summary>
		DateTime? FirstRun { get; }

		/// <summary>
		/// Gets the status of the license if it is a time-limited trial.
		/// </summary>
		/// <param name="timeRemaining">Time remaining in trial period.</param>
		/// <returns>True if license is for a limited-use trial; False otherwise.</returns>
		bool GetTrialStatus(out TimeSpan? timeRemaining);

		/// <summary>
		/// Gets the license expiry date in local time.
		/// </summary>
		/// <returns>
		/// License expiry time or NULL if the license does not expire.
		/// </returns>
		DateTime? GetExpiryDate();

		/// <summary>
		/// Gets a value indicating whether or not a specific feature is authorized by the license.
		/// </summary>
		/// <param name="featureToken"></param>
		/// <returns></returns>
		bool IsFeatureAuthorized(string featureToken);
	}

	/// <summary>
	/// Specifies the allowed diagnostic use of the product license.
	/// </summary>
	public enum LicenseDiagnosticUse
	{
		/// <summary>
		/// Indicates product is not licensed for diagnostic use.
		/// </summary>
		None,

		/// <summary>
		/// Indicates product is licensed for general diagnostic use, including humans.
		/// </summary>
		Allowed,

		/// <summary>
		/// Indicates product is licensed only for veterinary diagnosis.
		/// </summary>
		VeterinaryOnly
	}

	/// <summary>
	/// An extension point for <see cref="ILicenseDetailsProvider"/>s.
	/// </summary>
	/// <remarks>
	/// For internal framework use only.
	/// </remarks>
	[ExtensionPoint]
	public sealed class LicenseDetailsProviderExtensionPoint : ExtensionPoint<ILicenseDetailsProvider>
	{
		private LicenseDetailsProviderExtensionPoint() {}

		internal static ILicenseDetailsProvider CreateInstance()
		{
			try
			{
				// check for a provider extension
				return (ILicenseDetailsProvider) new LicenseDetailsProviderExtensionPoint().CreateExtension();
			}
			catch (NotSupportedException)
			{
				return new DefaultLicenseDetailsProvider();
			}
		}

		private sealed class DefaultLicenseDetailsProvider : ILicenseDetailsProvider
		{
			public string LicenseType
			{
				get { return null; }
			}

			public LicenseDiagnosticUse DiagnosticUse
			{
				get { return LicenseDiagnosticUse.None; }
			}

			public DateTime? FirstRun
			{
				get { return null; }
			}

			public bool GetTrialStatus(out TimeSpan? timeRemaining)
			{
				timeRemaining = null;
				return false;
			}

			public DateTime? GetExpiryDate()
			{
				return null;
			}

			public bool IsFeatureAuthorized(string featureToken)
			{
				return true;
			}
		}
	}
}
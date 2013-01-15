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
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Represents a license information provider.
	/// </summary>
	/// <remarks>
	/// For internal framework use only.
	/// </remarks>
	public interface ILicenseProvider
	{
		/// <summary>
		/// Gets license information.
		/// </summary>
		void GetLicenseInfo(out string licenseKey, out string machineId);

		/// <summary>
		/// Sets the current license key.
		/// </summary>
		/// <param name="licenseKey"></param>
		void SetLicenseInfo(string licenseKey);

		/// <summary>
		/// Fired when the license information has changed.
		/// </summary>
		event EventHandler LicenseInfoChanged;
	}

	/// <summary>
	/// An extension point for <see cref="ILicenseProvider"/>s.
	/// </summary>
	/// <remarks>
	/// For internal framework use only.
	/// </remarks>
	[ExtensionPoint]
	public sealed class LicenseProviderExtensionPoint : ExtensionPoint<ILicenseProvider>
	{
		private LicenseProviderExtensionPoint() {}

		internal static ILicenseProvider CreateInstance()
		{
			try
			{
				// check for a license provider extension
				return (ILicenseProvider) new LicenseProviderExtensionPoint().CreateExtension();
			}
			catch (NotSupportedException)
			{
				return new LocalLicenseProvider();
			}
		}

		private sealed class LocalLicenseProvider : ILicenseProvider
		{
			public void GetLicenseInfo(out string licenseKey, out string machineId)
			{
				licenseKey = ApplicationSettingsExtensions.GetSharedPropertyValue(new LicenseSettings(), "LicenseKey").ToString();
				machineId = EnvironmentUtilities.MachineIdentifier;
			}

			public void SetLicenseInfo(string licenseKey)
			{
				var oldLicenseKey = ApplicationSettingsExtensions.GetSharedPropertyValue(new LicenseSettings(), "LicenseKey").ToString();
				ApplicationSettingsExtensions.SetSharedPropertyValue(new LicenseSettings(), "LicenseKey", licenseKey);

				if (oldLicenseKey != licenseKey)
					EventsHelper.Fire(LicenseInfoChanged, this, new EventArgs());
			}

			public event EventHandler LicenseInfoChanged;
		}
	}
}
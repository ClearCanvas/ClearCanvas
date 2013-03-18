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
using System.Linq;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Login;

namespace ClearCanvas.Ris.Client
{
	[ExtensionOf(typeof(LoginFacilityProviderExtensionPoint))]
	internal sealed class LoginFacilityProvider : ILoginFacilityProvider
	{
		/// <summary>
		/// The puropse of this tool is to act as a "rescue" in the rare event that the working
		/// facility was not set via the login process.
		/// </summary>
		/// <remarks>
		/// The very first time the workstation is run with the RIS.Core feature enabled, the facility chooser
		/// won't be visible in the login dialog.  In this case, the facility will not have been set, so it falls
		/// to this tool to ensure that a current facility is established.
		/// </remarks>
		[ExtensionOf(typeof(ApplicationToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
		internal class RescueTool : Tool<IApplicationToolContext>
		{
			public override void Initialize()
			{
				base.Initialize();

				// if login session has already been established (via login dialog), nothing to do here
				if (LoginSession.Current != null)
					return;

				// otherwise, attempt to establish it, using a valid facility value
				try
				{
					Platform.GetService<ILoginService>(service =>
					{
						var facilities = RetrieveFacilityChoices(service);
						if (facilities.Any())
							LoginSession.Create(facilities.First());
					});
				}
				catch (EndpointNotFoundException e)
				{
					Platform.Log(LogLevel.Error, SR.ExceptionFailedToRetrieveFacilitiesFromRisServer);
					Platform.Log(LogLevel.Debug, e);
				}
			}
		}


		private readonly object _syncRoot = new object();
		private IList<string> _facilities;

		public LoginFacilityProvider()
		{
			// check the cache to see if RIS is licensed or not
			var lastKnownLicenseStatus = LoginFacilityProviderSettings.Default.IsRisCoreFeatureLicensed;

			// if we have a cached value
			if (lastKnownLicenseStatus.HasValue)
			{
				// kick off an asynchronous check, in order to update the cache for next time
				ThreadPool.QueueUserWorkItem(nothing => CheckFeatureAuthorizedAsync());

				// if the last known value was false, indicate that this extension is not supported
				if (!lastKnownLicenseStatus.Value)
					throw new NotSupportedException();
			}
			else
			{
				// we don't have a cached value, so we need to do a synchronous check
				// the cached will be updated for next time
				if (!CheckFeatureAuthorized())
					throw new NotSupportedException();
			}
		}

		public IList<string> GetAvailableFacilities()
		{
			if (_facilities != null)
				return _facilities;

			lock (_syncRoot)
			{
				if (_facilities == null)
				{
					try
					{
						Platform.Log(LogLevel.Debug, "Contacting server to obtain facility choices for login dialog...");
						Platform.GetService<ILoginService>(service => { _facilities = RetrieveFacilityChoices(service); });
						Platform.Log(LogLevel.Debug, "Got facility choices for login dialog.");
					}
					catch (Exception ex)
					{
						Platform.Log(LogLevel.Error, ex);
						_facilities = new string[0];
					}
				}
			}
			return _facilities;
		}

		public string CurrentFacility
		{
			get
			{
				return LoginSession.Current == null || LoginSession.Current.WorkingFacility == null ? null : LoginSession.Current.WorkingFacility.Code;
			}
			set
			{
				// once the session has been established, it can't be changed, but we
				// can just fail silently in order to support the case where the user
				// reconnects after a session expires
				if (LoginSession.Current != null)
					return;

				if (value != null && !string.IsNullOrEmpty(value))
				{
					LoginSession.Create(value);
				}
			}
		}

		private static IList<string> RetrieveFacilityChoices(ILoginService service)
		{
			var choices = service.GetWorkingFacilityChoices(new GetWorkingFacilityChoicesRequest()).FacilityChoices;
			return choices != null ? choices.Select(fs => fs.Code).ToArray() : new string[0];
		}

		private static void CheckFeatureAuthorizedAsync()
		{
			ThreadPool.QueueUserWorkItem(nothing =>
			{
				try
				{
					CheckFeatureAuthorized();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			});
		}

		private static bool CheckFeatureAuthorized()
		{
			var b = LicenseInformation.IsFeatureAuthorized(FeatureTokens.RIS.Core);
			LoginFacilityProviderSettings.Default.IsRisCoreFeatureLicensed = b;
			LoginFacilityProviderSettings.Default.Save();
			return b;
		}
	}
}
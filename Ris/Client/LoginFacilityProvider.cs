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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Login;

namespace ClearCanvas.Ris.Client
{
	//[ExtensionOf(typeof (LoginFacilityProviderExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	[ExtensionOf(typeof(LoginFacilityProviderExtensionPoint))]
	internal sealed class LoginFacilityProvider : ILoginFacilityProvider
	{
		private readonly object _syncRoot = new object();
		private IList<FacilityInfo> _listFacilities;

		public IList<FacilityInfo> GetAvailableFacilities()
		{
			lock (_syncRoot)
			{
				try
				{
					Platform.Log(LogLevel.Debug, "Contacting server to obtain facility choices for login dialog...");
					Platform.GetService<ILoginService>(service => { _listFacilities = RetrieveFacilityChoices(service); });
					Platform.Log(LogLevel.Debug, "Got facility choices for login dialog.");
					return _listFacilities;
				}
				catch (Exception ex)
				{
					Desktop.Application.ShowMessageBox("Unable to connect to RIS server.  The workstation may be configured incorrectly, or the server may be unreachable.", MessageBoxActions.Ok);
					Platform.Log(LogLevel.Error, ex);
					return new FacilityInfo[0];
				}
			}
		}

		private FacilityInfo GetFacility(string code)
		{
			lock (_syncRoot)
			{
				return _listFacilities != null ? _listFacilities.FirstOrDefault(fs => fs.Code == code) : null;
			}
		}

		private static IList<FacilityInfo> RetrieveFacilityChoices(ILoginService service)
		{
			var choices = service.GetWorkingFacilityChoices(new GetWorkingFacilityChoicesRequest()).FacilityChoices;
			return choices != null ? choices.Select(fs => new FacilityInfo(fs.Code, fs.Name)).ToArray() : new FacilityInfo[0];
		}

		public FacilityInfo CurrentFacility
		{
			get
			{
				return LoginSession.Current == null  || LoginSession.Current.WorkingFacility == null ? null
					: GetFacility(LoginSession.Current.WorkingFacility.Code);
			}
			set
			{
				// once the session has been established, it can't be changed, but we
				// can just fail silently in order to support the case where the user
				// reconnects after a session expires
				if (LoginSession.Current != null)
					return;

				if (value != null && !string.IsNullOrEmpty(value.Code))
				{
					LoginSession.Create(value.Code);
				}
			}
		}
	}
}
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
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using System.Security.Principal;
using System.IdentityModel.Claims;

namespace ClearCanvas.ImageViewer.DesktopServices
{
	public abstract class DesktopServiceHostTool : Tool<IDesktopToolContext>
	{
		private class AuthorizationPolicy : IAuthorizationPolicy
		{
			private readonly IPrincipal _principal;
			private readonly Guid _id;

			public AuthorizationPolicy(IPrincipal principal)
			{
				_principal = principal;
				_id = Guid.NewGuid();
			}

			#region IAuthorizationPolicy Members

			public bool Evaluate(EvaluationContext evaluationContext, ref object state)
			{
				if (_principal != null)
					evaluationContext.Properties["Principal"] = _principal;

				return true;
			}

			public System.IdentityModel.Claims.ClaimSet Issuer
			{
				get { return ClaimSet.System; }
			}

			#endregion

			#region IAuthorizationComponent Members

			public string Id
			{
				get { return _id.ToString(); }
			}

			#endregion
		}

		internal static SynchronizationContext HostSynchronizationContext;
		internal static AppDomain HostAppDomain;

		private ServiceHost _host = null;

		protected DesktopServiceHostTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			HostSynchronizationContext = SynchronizationContext.Current;
			HostAppDomain = AppDomain.CurrentDomain;

			StartHost();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			StopHost();
		}

		private void StartHost()
		{
			try
			{
				object[] permissionAttributes = this.GetType().GetCustomAttributes(typeof (DesktopServiceHostPermissionAttribute), true);
				foreach (DesktopServiceHostPermissionAttribute permissionAttribute in permissionAttributes)
				{
					if (!PermissionsHelper.IsInRoles(permissionAttribute.AuthorityTokens))
					{
						Platform.Log(LogLevel.Warn, "User does not have appropriate permissions to start desktop service ('{0}').", this.GetType().FullName);
						return;
					}
				}

				ServiceHost host = CreateServiceHost();

				if (Thread.CurrentPrincipal == null)
				{
					host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.None;
				}
				else
				{
					ServiceAuthorizationBehavior sa = host.Description.Behaviors.Find<ServiceAuthorizationBehavior>();
					sa.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
					List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
					policies.Add(new AuthorizationPolicy(Thread.CurrentPrincipal));
					host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();
				}

				host.Open();
				_host = host;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Debug, e, "Unable to start desktop service; another instance may be running.");
			}
		}

		protected abstract ServiceHost CreateServiceHost();

		private void StopHost()
		{
			try
			{
				if (_host != null)
				{
					_host.Close();
					_host = null;
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Debug, e, "Failed to stop desktop service.");
			}
		}
	}
}

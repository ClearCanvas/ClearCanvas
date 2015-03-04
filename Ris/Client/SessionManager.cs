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
using System.Drawing;
using System.Management;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Login;

namespace ClearCanvas.Ris.Client
{
	[ExtensionPoint]
	public class LoginDialogExtensionPoint : ExtensionPoint<ILoginDialog>
	{
	}

	[ExtensionPoint]
	public class ChangePasswordDialogExtensionPoint : ExtensionPoint<IChangePasswordDialog>
	{
	}


	[ExtensionOf(typeof(SessionManagerExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	class SessionManager : ISessionManager
	{
		struct LoginResult
		{
			public static readonly LoginResult None = new LoginResult(false, null, null);

			public LoginResult(string userName, SessionToken sessionToken)
				: this(true, userName, sessionToken)
			{
			}

			private LoginResult(bool loggedIn, string userName, SessionToken sessionToken)
			{
				LoggedIn = loggedIn;
				UserName = userName;
				SessionToken = sessionToken;
			}

			public readonly bool LoggedIn;
			public readonly string UserName;
			public readonly SessionToken SessionToken;
		}

		private static SessionManager _current;
		private LoginResult _loginResult = LoginResult.None;
		private string _facilityCode;

		#region ISessionManager Members

		bool ISessionManager.InitiateSession()
		{
			try
			{
				_current = this;
				return Login(ref _facilityCode);
			}
			catch (Exception e)
			{
				// can't use ExceptionHandler here because no desktopWindow exists yet
				Desktop.Application.ShowMessageBox(e.Message, MessageBoxActions.Ok);
				return false;
			}
		}

		public void InvalidateSession()
		{
			// todo
		}

		void ISessionManager.TerminateSession()
		{
			try
			{
				Terminate();
				_current = null;
			}
			catch (Exception e)
			{
				// since we're logging out, just log the exception and move on
				Platform.Log(LogLevel.Error, e);
			}
		}

		public SessionStatus SessionStatus
		{
			get
			{
				// the RIS can only be used online, so the session status will always be online
				return SessionStatus.Online;
			}
		}

		public event EventHandler<SessionStatusChangedEventArgs> SessionStatusChanged;

		#endregion

		#region Internal methods

		internal static string FacilityCode
		{
			get { return _current._facilityCode; }
		}

		internal static bool RenewLogin()
		{
			if (_current == null)
				return false;
			if (!_current._loginResult.LoggedIn)
				return false;

			_current._loginResult = Login(LoginDialogMode.RenewLogin, _current._loginResult.UserName, ref _current._facilityCode);
			return _current._loginResult.LoggedIn;
		}

		internal static bool ChangePassword()
		{
			if (_current == null)
				return false;
			if (!_current._loginResult.LoggedIn)
				return false;

			string newPassword;
			return ChangePassword(_current._loginResult.UserName, null, out newPassword);
		}

		#endregion

		private bool Login(ref string facility)
		{
			_loginResult = Login(LoginDialogMode.InitialLogin, null, ref facility);
			return _loginResult.LoggedIn;
		}

		private static LoginResult Login(LoginDialogMode mode, string userName, ref string facility)
		{
			var needLoginDialog = true;
			string password = null;

			var facilities = new List<FacilitySummary>();
			try
			{
				Platform.Log(LogLevel.Debug, "Contacting server to obtain facility choices for login dialog...");
				facilities = GetFacilityChoices();
				Platform.Log(LogLevel.Debug, "Got facility choices for login dialog.");
			}
			catch (Exception e)
			{
				Desktop.Application.ShowMessageBox(SR.MessageRisServerUnreachable, MessageBoxActions.Ok);
				Platform.Log(LogLevel.Error, e);
				return LoginResult.None;
			}

			while (true)
			{
				if (needLoginDialog)
				{
					if (!ShowLoginDialog(mode, facilities, ref userName, ref facility, out password))
					{
						// user cancelled
						return LoginResult.None;
					}
				}

				try
				{
					// try to create the session
					return DoLogin(userName, password);
				}
				catch (PasswordExpiredException)
				{
					string newPassword;
					if (!ChangePassword(userName, password, out newPassword))
					{
						// user cancelled password change, so just abort everything
						return LoginResult.None;
					}

					// loop again, but this time using the new password, and don't show the login dialog
					// since we already have the credentials
					password = newPassword;
					needLoginDialog = false;
				}
				catch (Exception e)
				{
					ReportException(e);
				}
			}
		}

		private static bool ShowLoginDialog(LoginDialogMode mode, List<FacilitySummary> facilities, ref string userName, ref string facility, out string password)
		{
			using (var loginDialog = (ILoginDialog)(new LoginDialogExtensionPoint()).CreateExtension())
			{
				var facilityCodes = CollectionUtils.Map(facilities, (FacilitySummary fs) => fs.Code).ToArray();

				var initialFacilityCode = LoginDialogSettings.Default.SelectedFacility;
				var location = LoginDialogSettings.Default.DialogScreenLocation;


				// if no saved facility, just choose the first one
				if (string.IsNullOrEmpty(initialFacilityCode) && facilityCodes.Length > 0)
					initialFacilityCode = facilityCodes[0];

				loginDialog.Mode = mode;
				loginDialog.FacilityChoices = facilityCodes;
				loginDialog.Facility = initialFacilityCode;
				loginDialog.UserName = userName;
				if (location != Point.Empty)
					loginDialog.Location = location;

				Platform.Log(LogLevel.Debug, "Showing login dialog.");
				if (loginDialog.Show())
				{
					// save selected facility
					LoginDialogSettings.Default.SelectedFacility = loginDialog.Facility;
					LoginDialogSettings.Default.DialogScreenLocation = loginDialog.Location;
					LoginDialogSettings.Default.Save();

					userName = loginDialog.UserName;
					password = loginDialog.Password;
					facility = loginDialog.Facility;

					return true;
				}
			}
			userName = null;
			password = null;
			facility = null;

			return false;
		}

		private static LoginResult DoLogin(string userName, string password)
		{
			try
			{
				Platform.Log(LogLevel.Debug, "Attempting login...");

				var result = LoginResult.None;
				Platform.GetService(
					delegate(IAuthenticationService service)
					{
						var request = new InitiateSessionRequest(userName, ProductInformation.Component, Dns.GetHostName(), password) { GetAuthorizations = true };
						var response = service.InitiateSession(request);

						if (response.SessionToken == null)
							throw new Exception("Invalid session token returned from authentication service.");

						// if the call succeeded, set a default principal object on this thread, containing
						// the set of authority tokens for this user
						Thread.CurrentPrincipal = DefaultPrincipal.CreatePrincipal(
							new GenericIdentity(userName),
							response.SessionToken,
							response.AuthorityTokens);

						result = new LoginResult(userName, response.SessionToken);
					});

				Platform.Log(LogLevel.Debug, "Login attempt was successful.");
				return result;
			}
			catch (FaultException<UserAccessDeniedException> e)
			{
				Platform.Log(LogLevel.Debug, e.Detail, "Login attempt failed.");
				throw e.Detail;
			}
			catch (FaultException<PasswordExpiredException> e)
			{
				Platform.Log(LogLevel.Debug, e.Detail, "Login attempt failed.");
				throw e.Detail;
			}
		}

		private static bool ChangePassword(string userName, string oldPassword, out string newPassword)
		{
			using (var changePasswordDialog = (IChangePasswordDialog)(new ChangePasswordDialogExtensionPoint()).CreateExtension())
			{
				changePasswordDialog.UserName = userName;
				changePasswordDialog.Password = oldPassword;
				while (true)
				{
					if (changePasswordDialog.Show())
					{
						try
						{
							ChangePassword(userName, changePasswordDialog.Password, changePasswordDialog.NewPassword);

							newPassword = changePasswordDialog.NewPassword;
							return true;
						}
						catch (Exception e)
						{
							ReportException(e);
						}
					}
					else
					{
						// user cancelled
						newPassword = null;
						return false;
					}
				}
			}
		}

		private static void ChangePassword(string userName, string oldPassword, string newPassword)
		{
			try
			{
				Platform.GetService(
					delegate(IAuthenticationService service)
					{
						var request = new ChangePasswordRequest(userName, oldPassword, newPassword);
						service.ChangePassword(request);
					});
			}
			catch (FaultException<UserAccessDeniedException> e)
			{
				throw e.Detail;
			}
			catch (FaultException<RequestValidationException> e)
			{
				throw e.Detail;
			}
		}

		/// <summary>
		/// Terminates the current session.
		/// </summary>
		private void Terminate()
		{
			if (!_loginResult.LoggedIn)
				return;

			try
			{
				Platform.GetService(
					delegate(IAuthenticationService service)
					{
						var request = new TerminateSessionRequest(_loginResult.UserName, _loginResult.SessionToken);
						service.TerminateSession(request);
					});
			}
			finally
			{
				_loginResult = LoginResult.None;
			}
		}

		private static void ReportException(Exception e)
		{
			if (e is RequestValidationException)
			{
				Desktop.Application.ShowMessageBox(e.Message, MessageBoxActions.Ok);
			}
			else if (e is UserAccessDeniedException)
			{
				Desktop.Application.ShowMessageBox(SR.MessageLoginAccessDenied, MessageBoxActions.Ok);
			}
			else if (e is CommunicationException)
			{
				Platform.Log(LogLevel.Error, e);
				Desktop.Application.ShowMessageBox(SR.MessageCommunicationError, MessageBoxActions.Ok);
			}
			else if (e is TimeoutException)
			{
				Platform.Log(LogLevel.Error, e);
				Desktop.Application.ShowMessageBox(SR.MessageLoginTimeout, MessageBoxActions.Ok);
			}
			else
			{
				Platform.Log(LogLevel.Error, e);
				Desktop.Application.ShowMessageBox(SR.MessageUnknownErrorCommunicatingWithServer, MessageBoxActions.Ok);
			}
		}

		private static List<FacilitySummary> GetFacilityChoices()
		{
			List<FacilitySummary> choices = null;
			Platform.GetService<ILoginService>(
				service => choices = service.GetWorkingFacilityChoices(new GetWorkingFacilityChoicesRequest()).FacilityChoices);
			return choices;
		}
	}
}

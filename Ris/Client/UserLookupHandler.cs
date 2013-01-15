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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.Enterprise.Desktop;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// This class is created so it can be shared between model and the view.  This way the summary object does not have to be exposed to the view.
	/// </summary>
	public class UserLookupData
	{
		private readonly string _userName;

		public UserLookupData(string userName)
		{
			_userName = userName;
		}

		public string UserName
		{
			get { return _userName; }
		}
	}

	/// <summary>
	/// Provides utilities for user name resolution.
	/// </summary>
	public class UserLookupHandler : ILookupHandler
	{
		private ISuggestionProvider _suggestionProvider;
		private readonly IDesktopWindow _desktopWindow;


		public UserLookupHandler(IDesktopWindow desktopWindow)
		{
			_desktopWindow = desktopWindow;
		}

		private static string FormatItem(UserLookupData user)
		{
			return user.UserName;
		}

		private static IList<UserLookupData> ListUsers(string query)
		{
			var users = new List<UserLookupData>();
			Platform.GetService<IUserAdminService>(
				service =>
				{
					var request = new ListUsersRequest {UserName = query};
					var response = service.ListUsers(request);
					users = CollectionUtils.Map(response.Users, (UserSummary summary) => new UserLookupData(summary.UserName));
				});

			return users;
		}

		#region ILookupHandler Members

		bool ILookupHandler.Resolve(string query, bool interactive, out object result)
		{
			result = null;

			var userComponent = new UserSummaryComponent(true);
			var exitCode = ApplicationComponent.LaunchAsDialog(
				_desktopWindow, userComponent, SR.TitleUser);

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				var summary = (UserSummary) userComponent.SummarySelection.Item;
				result = new UserLookupData(summary.UserName);
			}

			return (result != null);
		}

		string ILookupHandler.FormatItem(object item)
		{
			return FormatItem((UserLookupData)item);
		}

		ISuggestionProvider ILookupHandler.SuggestionProvider
		{
			get
			{
				if (_suggestionProvider == null)
				{
					_suggestionProvider = new DefaultSuggestionProvider<UserLookupData>(ListUsers, FormatItem);
				}

				return _suggestionProvider;
			}
		}

		#endregion
	}
}

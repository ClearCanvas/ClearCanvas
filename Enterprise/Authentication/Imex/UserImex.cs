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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core.Imex;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication.Brokers;

namespace ClearCanvas.Enterprise.Authentication.Imex
{
	[ExtensionOf(typeof(XmlDataImexExtensionPoint))]
	[ImexDataClass("User")]
	public class UserImex : XmlEntityImex<User, UserImex.UserData>
	{
		[DataContract]
		public class UserData
		{
			[DataMember]
			public string AccountType;

			[DataMember]
			public string UserName;

			[DataMember]
			public string DisplayName;

			[DataMember]
			public string EmailAddress;

			[DataMember]
			public DateTime? ValidFrom;

			[DataMember]
			public DateTime? ValidUntil;

			[DataMember]
			public bool Enabled;

			[DataMember]
			public List<string> AuthorityGroups;
		}

		private readonly AuthenticationSettings _settings = new AuthenticationSettings();


		#region Overrides

		protected override IList<User> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			var where = new UserSearchCriteria();
			where.UserName.SortAsc(0);
			return context.GetBroker<IUserBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override UserData Export(User user, IReadContext context)
		{
			var data = new UserData();
			data.AccountType = user.AccountType.ToString();
			data.UserName = user.UserName;
			data.DisplayName = user.DisplayName;
			data.ValidFrom = user.ValidFrom;
			data.ValidUntil = user.ValidUntil;
			data.Enabled = user.Enabled;
			data.AuthorityGroups = CollectionUtils.Map<AuthorityGroup, string>(
				user.AuthorityGroups,
				group => group.Name);

			return data;
		}

		protected override void Import(UserData data, IUpdateContext context)
		{
			var accountType = string.IsNullOrEmpty(data.AccountType)
								? UserAccountType.U
								: (UserAccountType)Enum.Parse(typeof(UserAccountType), data.AccountType, true);

			var info = new UserInfo(accountType, data.UserName, data.DisplayName, data.EmailAddress, data.ValidFrom, data.ValidUntil);
			var user = LoadOrCreateUser(info, context);
			user.Enabled = data.Enabled;

			if (data.AuthorityGroups != null)
			{
				foreach (var group in data.AuthorityGroups)
				{
					var where = new AuthorityGroupSearchCriteria();
					where.Name.EqualTo(group);

					var authGroup = CollectionUtils.FirstElement(context.GetBroker<IAuthorityGroupBroker>().Find(where));
					if (authGroup != null)
						user.AuthorityGroups.Add(authGroup);
				}
			}
		}

		#endregion


		private User LoadOrCreateUser(UserInfo info, IPersistenceContext context)
		{
			User user;
			try
			{
				var criteria = new UserSearchCriteria();
				criteria.UserName.EqualTo(info.UserName);

				var broker = context.GetBroker<IUserBroker>();
				user = broker.FindOne(criteria);
			}
			catch (EntityNotFoundException)
			{
				user = User.CreateNewUser(info, _settings.DefaultTemporaryPassword);
				context.Lock(user, DirtyState.New);
			}
			return user;
		}
	}
}


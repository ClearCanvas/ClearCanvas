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
using System.Threading;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Services
{
	/// <summary>
	/// Base class for all RIS application services.
	/// </summary>
	/// <remarks>
	/// This class makes several important assumptions:
	/// 1. Instances are never shared across threads.
	/// 2. Instances are used on a per-call basis.  That is, an instance services a single request and is then discarded.
	/// </remarks>
	public abstract class ApplicationServiceBase : IApplicationServiceLayer
	{
		/// <summary>
		/// Cached current-user Staff object.  Caching is acceptable assuming this service instance is not used for more than 1 call.
		/// </summary>
		private Staff _currentUserStaff;

		/// <summary>
		/// Gets the current user (on whose behalf this service call is executing).
		/// </summary>
		public string CurrentUser
		{
			get { return Thread.CurrentPrincipal.Identity.Name; }
		}

		/// <summary>
		/// Checks that the current user has the specified authority token.
		/// </summary>
		/// <param name="authorityToken"></param>
		/// <returns></returns>
		public bool UserHasToken(string authorityToken)
		{
			return Thread.CurrentPrincipal.IsInRole(authorityToken);
		}

		/// <summary>
		/// Checks that the current user has all of the specified authority tokens.
		/// </summary>
		/// <param name="authorityTokens"></param>
		/// <returns></returns>
		public bool UserHasAllTokens(params string[] authorityTokens)
		{
			return authorityTokens.Length > 0
				&& CollectionUtils.TrueForAll(authorityTokens, UserHasToken);
		}

		/// <summary>
		/// Checks that the current user has at least one of the specified authority tokens.
		/// </summary>
		/// <param name="authorityTokens"></param>
		/// <returns></returns>
		public bool UserHasAnyTokens(params string[] authorityTokens)
		{
			return CollectionUtils.Contains(authorityTokens, UserHasToken);
		}

		/// <summary>
		/// Obtains the staff associated with the current user.  If no <see cref="Staff"/> is associated with the current user,
		/// a <see cref="RequestValidationException"/> is thrown.
		/// </summary>
		public Staff CurrentUserStaff
		{
			get
			{
				if (_currentUserStaff == null)
				{
					var where = new StaffSearchCriteria();
					where.UserName.EqualTo(CurrentUser);
					_currentUserStaff = CollectionUtils.FirstElement(
						PersistenceContext.GetBroker<IStaffBroker>().Find(where, new SearchResultPage(0, 1), new EntityFindOptions { Cache = true }));

					if (_currentUserStaff == null)
						throw new RequestValidationException(SR.ExceptionNoStaffForUser);
				}

				return _currentUserStaff;
			}
		}

		/// <summary>
		/// Gets the current <see cref="IPersistenceContext"/>.
		/// </summary>
		public IPersistenceContext PersistenceContext
		{
			get
			{
				var context = PersistenceScope.CurrentContext;
				if(context == null)
					throw new InvalidOperationException("There is no active persistence context.  Ensure the appropriate ReadOperation or UpdateOperation attribute has been applied to the service method.");
				return context;
			}
		}
	}
}

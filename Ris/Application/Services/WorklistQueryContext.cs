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

using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Services
{
	/// <summary>
	/// Implementation of <see cref="IWorklistQueryContext"/>.
	/// </summary>
	class WorklistQueryContext : IWorklistQueryContext
	{
		private readonly ApplicationServiceBase _applicationService;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="service"></param>
		/// <param name="workingFacility"></param>
		/// <param name="page"></param>
		/// <param name="downtimeRecoveryMode"></param>
		public WorklistQueryContext(ApplicationServiceBase service, Facility workingFacility, SearchResultPage page, bool downtimeRecoveryMode)
		{
			_applicationService = service;
			WorkingFacility = workingFacility;
			Page = page;
			DowntimeRecoveryMode = downtimeRecoveryMode;
		}

		#region IWorklistQueryContext Members

		/// <summary>
		/// Gets the current user <see cref="Healthcare.Staff"/> object.
		/// </summary>
		public Staff ExecutingStaff
		{
			get { return _applicationService.CurrentUserStaff; }
		}

		/// <summary>
		/// Gets the working <see cref="Facility"/> associated with the current user, or null if no facility is associated.
		/// </summary>
		public Facility WorkingFacility { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the worklist is being invoked in downtime recovery mode.
		/// </summary>
		public bool DowntimeRecoveryMode { get; private set; }

		/// <summary>
		/// Gets the <see cref="SearchResultPage"/> that specifies which page of the worklist is requested.
		/// </summary>
		public SearchResultPage Page { get; private set; }

		/// <summary>
		/// Obtains an instance of the specified broker.
		/// </summary>
		/// <typeparam name="TBrokerInterface"></typeparam>
		/// <returns></returns>
		public TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker
		{
			return _applicationService.PersistenceContext.GetBroker<TBrokerInterface>();
		}

		#endregion
	}
}

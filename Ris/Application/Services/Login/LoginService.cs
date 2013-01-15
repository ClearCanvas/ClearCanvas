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

using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common.Login;

namespace ClearCanvas.Ris.Application.Services.Login
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(ILoginService))]
	public class LoginService : ApplicationServiceBase, ILoginService
	{
		#region ILoginService Members

		[ReadOperation]
		public GetWorkingFacilityChoicesResponse GetWorkingFacilityChoices(GetWorkingFacilityChoicesRequest request)
		{
			// load all facilities and sort by code
			var facilities = PersistenceContext.GetBroker<IFacilityBroker>().FindAll(false);
			facilities = facilities.OrderBy(x => x.Code).ToList();

			var facilityAssembler = new FacilityAssembler();
			return new GetWorkingFacilityChoicesResponse(
				CollectionUtils.Map(facilities,
					(Facility input) => facilityAssembler.CreateFacilitySummary(input)));
		}

		#endregion
	}
}

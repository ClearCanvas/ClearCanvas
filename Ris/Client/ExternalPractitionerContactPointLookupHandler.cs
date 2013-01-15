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
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerContactPointLookupHandler : ILookupHandler
	{
		private readonly EntityRef _practitionerRef;
		private readonly IList<ExternalPractitionerContactPointDetail> _contactPoints;
		private readonly IDesktopWindow _desktopWindow;
		private ISuggestionProvider _suggestionProvider;

		public ExternalPractitionerContactPointLookupHandler(
			EntityRef practitionerRef,
			IList<ExternalPractitionerContactPointDetail> contactPoints,
			IDesktopWindow desktopWindow)
		{
			_practitionerRef = practitionerRef;
			_contactPoints = contactPoints;
			_desktopWindow = desktopWindow;
		}

		private static string FormatItem(ExternalPractitionerContactPointDetail cp)
		{
			return string.Format("{0} ({1})", cp.Name, cp.Description);
		}

		#region ILookupHandler Members

		bool ILookupHandler.Resolve(string query, bool interactive, out object result)
		{
			result = null;

			ExternalPractitionerDetail practitionerDetail = null;
			Platform.GetService<IExternalPractitionerAdminService>(
				service =>
				{
					var response = service.LoadExternalPractitionerForEdit(new LoadExternalPractitionerForEditRequest(_practitionerRef));
					practitionerDetail = response.PractitionerDetail;
				});

			var component = new ExternalPractitionerContactPointSummaryComponent(_practitionerRef);

			practitionerDetail.ContactPoints.ForEach(p => component.Subject.Add(p));

			var exitCode = ApplicationComponent.LaunchAsDialog(
				_desktopWindow, component, SR.TitleContactPoints);

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				result = component.SummarySelection.Item;
			}

			return (result != null);
		
		}

		string ILookupHandler.FormatItem(object item)
		{
			return FormatItem((ExternalPractitionerContactPointDetail)item);
		}

		ISuggestionProvider ILookupHandler.SuggestionProvider
		{
			get
			{
				if (_suggestionProvider == null)
				{
					var sorted = CollectionUtils.Sort(_contactPoints, (x, y) => FormatItem(x).CompareTo(FormatItem(y)));
					_suggestionProvider = new DefaultSuggestionProvider<ExternalPractitionerContactPointDetail>(sorted, FormatItem);
				}
				return _suggestionProvider;
			}
		}

		#endregion
	}
}

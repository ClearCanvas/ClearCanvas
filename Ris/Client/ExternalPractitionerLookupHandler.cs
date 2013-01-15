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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
    public class ExternalPractitionerLookupHandler : LookupHandler<TextQueryRequest, ExternalPractitionerSummary>
    {
        private readonly DesktopWindow _desktopWindow;

        public ExternalPractitionerLookupHandler(DesktopWindow desktopWindow)
        {
            _desktopWindow = desktopWindow;
        }

        protected override TextQueryResponse<ExternalPractitionerSummary> DoQuery(TextQueryRequest request)
        {
            TextQueryResponse<ExternalPractitionerSummary> response = null;
            Platform.GetService<IExternalPractitionerAdminService>(
                delegate(IExternalPractitionerAdminService service)
                {
                    response = service.TextQuery(request);
                });
            return response;
        }
        
        public override bool ResolveNameInteractive(string query, out ExternalPractitionerSummary result)
        {
            result = null;

            ExternalPractitionerSummaryComponent component = new ExternalPractitionerSummaryComponent(true);
			component.IncludeDeactivatedItems = this.IncludeDeactivatedItems;
			if (!string.IsNullOrEmpty(query))
            {
                string[] names = query.Split(',');
                if (names.Length > 0)
                    component.LastName = names[0].Trim();
                if (names.Length > 1)
                    component.FirstName = names[1].Trim();
            }

            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                _desktopWindow, component, SR.TitleExternalPractitioner);

            if (exitCode == ApplicationComponentExitCode.Accepted)
            {
                result = (ExternalPractitionerSummary)component.SummarySelection.Item;
            }

            return (result != null);
        }

        public override string FormatItem(ExternalPractitionerSummary item)
        {
            return PersonNameFormat.Format(item.Name);
        }
    }
}

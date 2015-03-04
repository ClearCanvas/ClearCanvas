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
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client
{
	public interface IDiagnosticServiceInteractiveLookupProvider
	{
		DiagnosticServiceSummary ResolveDiagnosticService(string query, IDesktopWindow desktopWindow);
	}

	[ExtensionPoint]
	public class DiagnosticServiceInteractiveLookupProviderExtensionPoint: ExtensionPoint<IDiagnosticServiceInteractiveLookupProvider>
	{
	}

    public class DiagnosticServiceLookupHandler : LookupHandler<TextQueryRequest, DiagnosticServiceSummary>
    {
        private readonly DesktopWindow _desktopWindow;

        public DiagnosticServiceLookupHandler(DesktopWindow desktopWindow)
			: base(DiagnosticServiceLookupSettings.Default.MinQueryStringLength, DiagnosticServiceLookupSettings.Default.QuerySpecificityThreshold)
        {
            _desktopWindow = desktopWindow;
        }

        protected override TextQueryResponse<DiagnosticServiceSummary> DoQuery(TextQueryRequest request)
        {
            TextQueryResponse<DiagnosticServiceSummary> response = null;
            Platform.GetService<IDiagnosticServiceAdminService>(
                delegate(IDiagnosticServiceAdminService service)
                {
                    response = service.TextQuery(request);
                });
            return response;
        }

        public override bool ResolveNameInteractive(string query, out DiagnosticServiceSummary result)
        {
			result = null;

			try
			{
				IDiagnosticServiceInteractiveLookupProvider provider = (IDiagnosticServiceInteractiveLookupProvider)
					new DiagnosticServiceInteractiveLookupProviderExtensionPoint().CreateExtension();
				result = provider.ResolveDiagnosticService(query, _desktopWindow);

			}
			catch (NotSupportedException)
			{
				// default
				DiagnosticServiceSummaryComponent summaryComponent = new DiagnosticServiceSummaryComponent(true);
				summaryComponent.IncludeDeactivatedItems = this.IncludeDeactivatedItems;
				if (!string.IsNullOrEmpty(query))
				{
					summaryComponent.Name = query;
				}

				ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
					_desktopWindow, summaryComponent, SR.TitleImagingServices);

				if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					result = (DiagnosticServiceSummary)summaryComponent.SummarySelection.Item;
				}
			}

			return (result != null);
		}


        public override string FormatItem(DiagnosticServiceSummary item)
        {
            return item.Name;
        }
    }
}

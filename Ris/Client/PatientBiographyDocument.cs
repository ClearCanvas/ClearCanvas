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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    public class PatientBiographyDocument : Document
    {
		private readonly EntityRef _patientRef;
		private readonly EntityRef _profileRef;
    	private readonly EntityRef _orderRef;
    	private readonly PersonNameDetail _patientName;
    	private readonly CompositeIdentifierDetail _mrn;

		public PatientBiographyDocument(PatientProfileSummary patientProfile, IDesktopWindow window)
			: base(patientProfile.PatientRef, window)
		{
			Platform.CheckForNullReference(patientProfile.PatientRef, "PatientRef");
			Platform.CheckForNullReference(patientProfile.PatientProfileRef, "PatientProfileRef");

			_patientRef = patientProfile.PatientRef;
			_profileRef = patientProfile.PatientProfileRef;
			_patientName = patientProfile.Name;
			_mrn = patientProfile.Mrn;
		}

		public PatientBiographyDocument(WorklistItemSummaryBase worklistItem, IDesktopWindow window)
			: base(worklistItem.PatientRef, window)
        {
			Platform.CheckForNullReference(worklistItem.PatientRef, "PatientRef");
			Platform.CheckForNullReference(worklistItem.PatientProfileRef, "PatientProfileRef");
			// the worklist item may not have an OrderRef (may represent just a patient)

			_patientRef = worklistItem.PatientRef;
			_profileRef = worklistItem.PatientProfileRef;
			_patientName = worklistItem.PatientName;
			_mrn = worklistItem.Mrn;

			_orderRef = worklistItem.OrderRef;
        }

        public override string GetTitle()
        {
            return "";  // not relevant - component will set title
        }

		public override bool SaveAndClose()
		{
			return base.Close();
		}

        public override IApplicationComponent GetComponent()
        {
			var component = new BiographyOverviewComponent(_patientRef, _profileRef, _orderRef);
        	return component;
        }

    	public override OpenWorkspaceOperationAuditData GetAuditData()
    	{
			return new OpenWorkspaceOperationAuditData("Biography", _mrn, _patientName);
    	}
    }    
}

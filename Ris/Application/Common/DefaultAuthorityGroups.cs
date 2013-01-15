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
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.Ris.Application.Common
{
	/// <summary>
	/// Defines a default set of authority groups to be imported at deployment time.  This class
	/// is not used post-deployment.
	/// </summary>
	[ExtensionOf(typeof(DefineAuthorityGroupsExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	public class DefaultAuthorityGroups : IDefineAuthorityGroups
	{
		// note: we do not define the sys admin ("Administrators") group here because it is defined 
		// automatically in ClearCanvas.Enterprise.Authentication

		public const string HealthcareAdmin = "Healthcare Administrators";
		public const string Clerical = "Clerical";
		public const string Technologists = "Technologists";
		public const string Radiologists = "Radiologists";
		public const string RadiologyResidents = "Radiology Residents";
		public const string EmergencyPhysicians = "Emergency Physicians";


		#region IDefineAuthorityGroups Members

		public AuthorityGroupDefinition[] GetAuthorityGroups()
		{
			return new AuthorityGroupDefinition[]
            {
                new AuthorityGroupDefinition(HealthcareAdmin,
                    new string[] 
                    {
                        AuthorityTokens.Workflow.HomePage.View,
                        AuthorityTokens.Workflow.PatientBiography.View,
                        AuthorityTokens.Workflow.CannedText.Personal,
                        AuthorityTokens.Workflow.CannedText.Group,
                        AuthorityTokens.Workflow.Patient.Create,
                        AuthorityTokens.Workflow.Patient.Update,
                        AuthorityTokens.Workflow.PatientProfile.Update,
                        AuthorityTokens.Workflow.Visit.Create,
                        AuthorityTokens.Workflow.Visit.Update,
                        AuthorityTokens.Workflow.Order.Create,
                        AuthorityTokens.Workflow.Order.Modify,
                        AuthorityTokens.Workflow.Order.Cancel,
                        AuthorityTokens.Workflow.Order.Replace,
                        AuthorityTokens.Workflow.Procedure.CheckIn,
                        AuthorityTokens.Workflow.Documentation.Create,
                        AuthorityTokens.Workflow.Documentation.Accept,
                        AuthorityTokens.Workflow.Report.Create,
                        AuthorityTokens.Workflow.Report.Cancel,
                        AuthorityTokens.Workflow.Report.Verify,
                        AuthorityTokens.Workflow.Report.OmitSupervisor,
                        AuthorityTokens.Workflow.Report.Reassign,
                        AuthorityTokens.Workflow.ExternalPractitioner.Create,
                        AuthorityTokens.Workflow.ExternalPractitioner.Update,
                        AuthorityTokens.Workflow.ExternalPractitioner.Merge,

                        AuthorityTokens.Admin.Data.DiagnosticService,
                        AuthorityTokens.Admin.Data.Enumeration,
                        AuthorityTokens.Admin.Data.ExternalPractitioner,
                        AuthorityTokens.Admin.Data.Facility,
                        AuthorityTokens.Admin.Data.Location,
                        AuthorityTokens.Admin.Data.Modality,
                        AuthorityTokens.Admin.Data.PatientNoteCategory,
                        AuthorityTokens.Admin.Data.ProcedureType,
                        AuthorityTokens.Admin.Data.ProcedureTypeGroup,
                        AuthorityTokens.Admin.Data.Staff,
                        AuthorityTokens.Admin.Data.StaffGroup,
                        AuthorityTokens.Admin.Data.Worklist,
                        AuthorityTokens.Admin.Data.Scheduling,

						AuthorityTokens.FolderSystems.Registration,
						AuthorityTokens.FolderSystems.Performing,
						AuthorityTokens.FolderSystems.Reporting,
						AuthorityTokens.FolderSystems.RadiologistAdmin,
                    }),

                new AuthorityGroupDefinition(Clerical,
                    new string[] 
                    {
                        AuthorityTokens.Workflow.HomePage.View,
                        AuthorityTokens.Workflow.PatientBiography.View,
                        AuthorityTokens.Workflow.CannedText.Personal,
                        AuthorityTokens.Workflow.Patient.Create,
                        AuthorityTokens.Workflow.Patient.Update,
                        AuthorityTokens.Workflow.PatientProfile.Update,
                        AuthorityTokens.Workflow.Visit.Create,
                        AuthorityTokens.Workflow.Visit.Update,
                        AuthorityTokens.Workflow.Order.Create,
                        AuthorityTokens.Workflow.Order.Modify,
                        AuthorityTokens.Workflow.Order.Cancel,
                        AuthorityTokens.Workflow.Order.Replace,
                        AuthorityTokens.Workflow.Procedure.CheckIn,
                        AuthorityTokens.Workflow.ExternalPractitioner.Create,
                        AuthorityTokens.Workflow.ExternalPractitioner.Update,

						AuthorityTokens.FolderSystems.Registration,
                   }),

                new AuthorityGroupDefinition(Technologists,
                    new string[] 
                    {
                        AuthorityTokens.Workflow.HomePage.View,
                        AuthorityTokens.Workflow.PatientBiography.View,
                        AuthorityTokens.Workflow.CannedText.Personal,
                        AuthorityTokens.Workflow.Order.Create,
                        AuthorityTokens.Workflow.Order.Modify,
                        AuthorityTokens.Workflow.Order.Cancel,
                        AuthorityTokens.Workflow.Order.Replace,
                        AuthorityTokens.Workflow.Procedure.CheckIn,
                        AuthorityTokens.Workflow.Documentation.Create,
                        AuthorityTokens.Workflow.Documentation.Accept,

						AuthorityTokens.FolderSystems.Registration,
						AuthorityTokens.FolderSystems.Performing,
                    }),

                new AuthorityGroupDefinition(Radiologists,
                    new string[] 
                    {
                        AuthorityTokens.Workflow.HomePage.View,
                        AuthorityTokens.Workflow.PatientBiography.View,
                        AuthorityTokens.Workflow.CannedText.Personal,
                        AuthorityTokens.Workflow.Report.Create,
                        AuthorityTokens.Workflow.Report.Verify,
                        AuthorityTokens.Workflow.Report.OmitSupervisor,

						AuthorityTokens.FolderSystems.Reporting,
                   }),

                new AuthorityGroupDefinition(RadiologyResidents,
                    new string[] 
                    {
                        AuthorityTokens.Workflow.HomePage.View,
						AuthorityTokens.Workflow.PatientBiography.View,
						AuthorityTokens.Workflow.CannedText.Personal,
						AuthorityTokens.Workflow.Report.Create,

						AuthorityTokens.FolderSystems.Reporting,
                   }),

                new AuthorityGroupDefinition(EmergencyPhysicians,
                    new string[] 
                    {
                        AuthorityTokens.Workflow.HomePage.View,
                        AuthorityTokens.Workflow.PatientBiography.View,
                        AuthorityTokens.Workflow.CannedText.Personal,
                    }),

            };
		}

		#endregion
	}
}

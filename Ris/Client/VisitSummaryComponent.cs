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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// VisitSummaryComponent class
	/// </summary>
	public class VisitSummaryComponent : SummaryComponentBase<VisitSummary, VisitSummaryTable>
	{
		private readonly PatientProfileSummary _patientProfile;

		public VisitSummaryComponent(PatientProfileSummary patientProfile, bool dialogMode)
			:base(dialogMode)
		{
			_patientProfile = patientProfile;
		}

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Visit.Create);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Visit.Update);
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <param name="firstItem"></param>
		/// <param name="maxItems"></param>
		/// <returns></returns>
		protected override IList<VisitSummary> ListItems(int firstItem, int maxItems)
		{
			ListVisitsForPatientResponse listResponse = null;
			Platform.GetService(
				delegate(IVisitAdminService service)
				{
					listResponse = service.ListVisitsForPatient(new ListVisitsForPatientRequest(_patientProfile.PatientRef));
				});

			return listResponse.Visits;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<VisitSummary> addedItems)
		{
			addedItems = new List<VisitSummary>();
			var editor = new VisitEditorComponent(_patientProfile);
			if (ApplicationComponentExitCode.Accepted == 
				LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddVisit))
			{
				addedItems.Add(editor.VisitSummary);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to handle the "edit" action.
		/// </summary>
		/// <param name="items">A list of items to edit.</param>
		/// <param name="editedItems">The list of items that were edited.</param>
		/// <returns>True if items were edited, false otherwise.</returns>
		protected override bool EditItems(IList<VisitSummary> items, out IList<VisitSummary> editedItems)
		{
			editedItems = new List<VisitSummary>();
			var item = CollectionUtils.FirstElement(items);

			var editor = new VisitEditorComponent(item);
			if (ApplicationComponentExitCode.Accepted == 
				LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateVisit))
			{
				editedItems.Add(editor.VisitSummary);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to handle the "delete" action, if supported.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="deletedItems">The list of items that were deleted.</param>
		/// <param name="failureMessage">The message if there any errors that occurs during deletion.</param>
		/// <returns>True if items were deleted, false otherwise.</returns>
		protected override bool DeleteItems(IList<VisitSummary> items, out IList<VisitSummary> deletedItems, out string failureMessage)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override bool IsSameItem(VisitSummary x, VisitSummary y)
		{
			return x.VisitRef.Equals(y.VisitRef, true);
		}
	}
}

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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.OrderNotes;
using ClearCanvas.Ris.Client.Formatting;
using System;

namespace ClearCanvas.Ris.Client.Workflow
{
	static class PreliminaryDiagnosis
	{
		/// <summary>
		/// Determines if the PD dialog must be shown upon verification for the specified worklist item, and shows it if needed.
		/// </summary>
		/// <param name="worklistItem"></param>
		/// <param name="desktopWindow"></param>
		/// <param name="continuation">Code block that is executed if the dialog was shown and accepted, or if it was not required. </param>
		/// <returns>True if the dialog was shown and accepted, or if it was not required.  False if the user cancelled out of the dialog.</returns>
		public static bool ShowDialogOnVerifyIfRequired(WorklistItemSummaryBase worklistItem, IDesktopWindow desktopWindow,
			Action<object> continuation)
		{
			if(!NeedDialogOnVerify(worklistItem, desktopWindow))
			{
				// we don't need the dialog, so we can continue
				continuation(null);
				return true;
			}

			// show the pd dialog
			var completed = false;
			ShowDialog(worklistItem, desktopWindow,
				exitCode =>
				{
					if(exitCode == ApplicationComponentExitCode.Accepted)
					{
						// if the dialog was accepted, continue
						continuation(null);
						completed = true;
					}
				});

			return completed;
		}

		/// <summary>
		/// Checks the PD dialog must be shown when verifying the specified worklist item.
		/// </summary>
		/// <param name="worklistItem"></param>
		/// <param name="window"></param>
		/// <returns></returns>
		private static bool NeedDialogOnVerify(WorklistItemSummaryBase worklistItem, IDesktopWindow window)
		{
			var existingConv = ConversationExists(worklistItem.OrderRef);

			// if no existing conversation, may not need to show the dialog
			if (!existingConv)
			{
				// if this is not an emergency order, do not show the dialog
				if (!IsEmergencyOrder(worklistItem.PatientClass.Code))
					return false;

				// otherwise, ask the user if they would like to initiate a PD review
				var msg = string.Format(SR.MessageQueryPrelimDiagnosisReviewRequired, worklistItem.PatientClass.Value);
				var action = window.ShowMessageBox(msg, MessageBoxActions.YesNo);
				if (action == DialogBoxAction.No)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Displays the PD dialog.
		/// </summary>
		/// <param name="worklistItem"></param>
		/// <param name="window"></param>
		/// <param name="continuationCode"></param>
		private static void ShowDialog(WorklistItemSummaryBase worklistItem, IDesktopWindow window, Action<ApplicationComponentExitCode> continuationCode)
		{
			var component = CreateComponent(worklistItem);
			var dialog = ApplicationComponent.LaunchAsWorkspaceDialog(window, component, MakeTitle(worklistItem));
			dialog.Closed += delegate { continuationCode(component.ExitCode); };
		}


		private static OrderNoteConversationComponent CreateComponent(WorklistItemSummaryBase worklistItem)
		{
			return new OrderNoteConversationComponent(worklistItem.OrderRef, OrderNoteCategory.PreliminaryDiagnosis.Key,
													  PreliminaryDiagnosisSettings.Default.VerificationTemplatesXml,
													  PreliminaryDiagnosisSettings.Default.VerificationSoftKeysXml);
		}

		private static string MakeTitle(WorklistItemSummaryBase worklistItem)
		{
			return string.Format(SR.FormatTitleContextDescriptionReviewOrderNoteConversation,
								 PersonNameFormat.Format(worklistItem.PatientName),
								 MrnFormat.Format(worklistItem.Mrn),
								 AccessionFormat.Format(worklistItem.AccessionNumber));
		}

		private static bool ConversationExists(EntityRef orderRef)
		{
			var filters = new List<string>(new[] { OrderNoteCategory.PreliminaryDiagnosis.Key });
			var show = false;
			Platform.GetService<IOrderNoteService>(
				service => show = service.GetConversation(new GetConversationRequest(orderRef, filters, true)).NoteCount > 0);

			return show;
		}

		private static bool IsEmergencyOrder(string patientClassCode)
		{
			var patientClassFilters = ReportingSettings.Default.PreliminaryDiagnosisReviewForPatientClass;
			var patientClassCodesForReview = string.IsNullOrEmpty(patientClassFilters)
												? new List<string>()
												: CollectionUtils.Map(patientClassFilters.Split(','), (string s) => s.Trim());

			return patientClassCodesForReview.Contains(patientClassCode);
		}

	}
}

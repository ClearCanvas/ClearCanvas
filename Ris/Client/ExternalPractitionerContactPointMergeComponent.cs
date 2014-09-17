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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerContactPointMergeComponent : MergeComponentBase<ExternalPractitionerContactPointDetail>
	{
		public ExternalPractitionerContactPointMergeComponent(
			EntityRef practitionerRef,
			IList<ExternalPractitionerContactPointDetail> contactPoints)
			: base(contactPoints)
		{
		}

		public override object FormatItem(object p)
		{
			var cp = (ExternalPractitionerContactPointDetail)p;
			return ExternalPractitionerContactPointFormat.Format(cp);
		}

		protected override bool IsSameItem(ExternalPractitionerContactPointDetail x, ExternalPractitionerContactPointDetail y)
		{
			return x == null || y == null ? false : x.ContactPointRef.Equals(y.ContactPointRef, true);
		}

		public override void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			try
			{
				var cost = CalculateMergeCost(this.SelectedOriginalSummary, this.SelectedDuplicateSummary);

                var msg = string.Format("Merge operation will affect {0} orders.", cost);
				msg += "\n\nPress 'Cancel' to cancel the operation.\nPress 'OK' to continue. The merge operation cannot be undone.";
				var action = this.Host.ShowMessageBox(msg, MessageBoxActions.OkCancel);
				if (action == DialogBoxAction.Cancel)
				{
					return;
				}

				// perform the merge
				PerformMergeOperation(this.SelectedOriginalSummary, this.SelectedDuplicateSummary);

				base.Accept();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.ExceptionFailedToMergeDuplicateContactPoints, this.Host.DesktopWindow);
			}
		}

		private long CalculateMergeCost(ExternalPractitionerContactPointDetail original, ExternalPractitionerContactPointDetail duplicate)
		{
			return ShowProgress("Calculating number of records affected...",
					  service => service.MergeDuplicateContactPoint(new MergeDuplicateContactPointRequest(
										original.ContactPointRef, duplicate.ContactPointRef) { EstimateCostOnly = true }).CostEstimate);
		}

		private static void PerformMergeOperation(ExternalPractitionerContactPointDetail original, ExternalPractitionerContactPointDetail duplicate)
		{
			Platform.GetService<IExternalPractitionerAdminService>(
				service => service.MergeDuplicateContactPoint(new MergeDuplicateContactPointRequest(
										original.ContactPointRef, duplicate.ContactPointRef)));
		}

		private T ShowProgress<T>(
			string message,
			Converter<IExternalPractitionerAdminService, T> action)
		{
			var result = default(T);
			var task = new BackgroundTask(
				delegate(IBackgroundTaskContext context)
				{
					context.ReportProgress(new BackgroundTaskProgress(0, message));

					try
					{
						Platform.GetService<IExternalPractitionerAdminService>(service =>
									{
										result = action(service);
									});
					}
					catch (Exception e)
					{
						context.Error(e);
					}
				}, false) { ThreadUICulture = Desktop.Application.CurrentUICulture };

			ProgressDialog.Show(task, this.Host.DesktopWindow, true, ProgressBarStyle.Marquee);
			return result;
		}
	}
}

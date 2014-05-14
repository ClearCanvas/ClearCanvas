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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerMergeNavigatorComponent : NavigatorComponentContainer
	{
		private ExternalPractitionerMergeSelectedDuplicateComponent _selectedDuplicateComponent;
		private ExternalPractitionerMergePropertiesComponent _mergePropertiesComponent;
		private ExternalPractitionerSelectDisabledContactPointsComponent _selectContactPointsComponent;
		private ExternalPractitionerReplaceDisabledContactPointsComponent _replaceContactPointsComponent;
		private ExternalPractitionerOverviewComponent _confirmationComponent;

		private readonly EntityRef _originalPractitionerRef;
		private readonly EntityRef _specifiedDuplicatePractitionerRef;
		private readonly ExternalPractitionerDetail _mergedPractitioner;
		private ExternalPractitionerDetail _originalPractitioner;
		private ExternalPractitionerDetail _selectedDuplicate;

		/// <summary>
		/// Constructor for selecting a single practitioner to merge.
		/// </summary>
		public ExternalPractitionerMergeNavigatorComponent(EntityRef practitionerRef)
			: this(practitionerRef, null)
		{
		}

		/// <summary>
		/// Constructor for selecting two practitioners to merge.
		/// </summary>
		public ExternalPractitionerMergeNavigatorComponent(EntityRef practitionerRef, EntityRef duplicatePractitionerRef)
		{
			_originalPractitionerRef = practitionerRef;
			_specifiedDuplicatePractitionerRef = duplicatePractitionerRef;
			_mergedPractitioner = new ExternalPractitionerDetail();
		}

		public override void Start()
		{
			this.Pages.Add(new NavigatorPage(SR.TitleSelectDuplicate, _selectedDuplicateComponent = new ExternalPractitionerMergeSelectedDuplicateComponent(_specifiedDuplicatePractitionerRef)));
			this.Pages.Add(new NavigatorPage(SR.TitleResolvePropertyConflicts, _mergePropertiesComponent = new ExternalPractitionerMergePropertiesComponent()));
			this.Pages.Add(new NavigatorPage(SR.TitleSelectActiveContactPoints, _selectContactPointsComponent = new ExternalPractitionerSelectDisabledContactPointsComponent()));
			this.Pages.Add(new NavigatorPage(SR.TitleReplaceInactiveContactPoints, _replaceContactPointsComponent = new ExternalPractitionerReplaceDisabledContactPointsComponent()));
			this.Pages.Add(new NavigatorPage(SR.TitlePreviewMergedPractitioner, _confirmationComponent = new ExternalPractitionerOverviewComponent()));
			this.ValidationStrategy = new AllComponentsValidationStrategy();

			_selectedDuplicateComponent.SelectedPractitionerChanged += delegate { this.ForwardEnabled = _selectedDuplicateComponent.HasValidationErrors == false; };
			_selectContactPointsComponent.ContactPointSelectionChanged += delegate { this.ForwardEnabled = _selectContactPointsComponent.HasValidationErrors == false; };

			base.Start();

			// Start the component with forward button disabled.
			// The button will be enabled if there is a practitioner selected.
			this.ForwardEnabled = false;

			// Immediately activate validation after component start
			this.ShowValidation(true);
		}

		public override bool ShowTree
		{
			// Disable tree pane, so user can only navigate with the Forward and Backward buttons.
			// It is very important that each page navigates forward in a sequential order.
			get { return false; }
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
				// build request
				var defaultContactPoint = CollectionUtils.SelectFirst(_mergedPractitioner.ContactPoints, cp => cp.IsDefaultContactPoint);
				var deactivatedContactPoints = CollectionUtils.Select(_mergedPractitioner.ContactPoints, cp => cp.Deactivated);
				var request = new MergeExternalPractitionerRequest
				{
					RightPractitionerRef = _mergedPractitioner.PractitionerRef,
					LeftPractitionerRef = _selectedDuplicate.PractitionerRef,
					Name = _mergedPractitioner.Name,
					LicenseNumber = _mergedPractitioner.LicenseNumber,
					BillingNumber = _mergedPractitioner.BillingNumber,
					ExtendedProperties = _mergedPractitioner.ExtendedProperties,
					DefaultContactPointRef = defaultContactPoint == null ? null : defaultContactPoint.ContactPointRef,
					DeactivatedContactPointRefs = CollectionUtils.Map(deactivatedContactPoints, (ExternalPractitionerContactPointDetail cp) => cp.ContactPointRef),
					ContactPointReplacements = _replaceContactPointsComponent.ContactPointReplacements
				};

				var cost = CalculateMergeCost(request);

				var msg = string.Format("Merge operation will affect {0} orders and/or visits.", cost);
				msg += "\n\nPress 'Cancel' to cancel the operation.\nPress 'OK' to continue. The merge operation cannot be undone.";
				var action = this.Host.ShowMessageBox(msg, MessageBoxActions.OkCancel);
				if (action == DialogBoxAction.Cancel)
				{
					return;
				}

				// perform the merge
				PerformMergeOperation(request);

				Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		protected override void MoveTo(int index)
		{
			var previousIndex = this.CurrentPageIndex;

			if (CanMoveTo(index) == false)
				return;

			base.MoveTo(index);

			if (previousIndex < 0 || index > previousIndex)
				OnMovedForward();
			else
				OnMovedBackward();
		}

		private bool CanMoveTo(int index)
		{
			// don't prevent moving to first page during initialization
			if (this.CurrentPageIndex < 0)
				return true;

			// Moving forward
			if (index > this.CurrentPageIndex)
				if(this.CurrentPage.Component == _replaceContactPointsComponent && _replaceContactPointsComponent.HasValidationErrors)
				{
					_replaceContactPointsComponent.ShowValidation(true);
					return false;
				}

			// Moving back
			return true;
		}

		private void OnMovedForward()
		{
			var currentComponent = this.CurrentPage.Component;
			if (currentComponent == _selectedDuplicateComponent)
			{
				_originalPractitioner = LoadPractitionerDetail(_originalPractitionerRef);
				_selectedDuplicateComponent.OriginalPractitioner = _originalPractitioner;
				_mergePropertiesComponent.OriginalPractitioner = _originalPractitioner;
				_selectContactPointsComponent.OriginalPractitioner = _originalPractitioner;
			}
			else if (currentComponent == _mergePropertiesComponent)
			{
				// If selection change, load detail of the selected duplicate practitioner.
				if (_selectedDuplicateComponent.SelectedPractitioner == null)
					_selectedDuplicate = null;
				else if (_selectedDuplicate == null)
					_selectedDuplicate = LoadPractitionerDetail(_selectedDuplicateComponent.SelectedPractitioner.PractitionerRef);
				else if (!_selectedDuplicate.PractitionerRef.Equals(_selectedDuplicateComponent.SelectedPractitioner.PractitionerRef, true))
					_selectedDuplicate = LoadPractitionerDetail(_selectedDuplicateComponent.SelectedPractitioner.PractitionerRef);

				_mergePropertiesComponent.DuplicatePractitioner = _selectedDuplicate;
			}
			else if (currentComponent == _selectContactPointsComponent)
			{
				_selectContactPointsComponent.DuplicatePractitioner = _selectedDuplicate;
			}
			else if (currentComponent == _replaceContactPointsComponent)
			{
				_replaceContactPointsComponent.ActiveContactPoints = _selectContactPointsComponent.ActiveContactPoints;
				_replaceContactPointsComponent.DeactivatedContactPoints = _selectContactPointsComponent.DeactivatedContactPoints;

				if (_replaceContactPointsComponent.DeactivatedContactPoints.Count == 0)
					Forward();
			}
			else if (currentComponent == _confirmationComponent)
			{
				_mergedPractitioner.PractitionerRef = _originalPractitionerRef;
				_mergePropertiesComponent.Save(_mergedPractitioner);
				_selectContactPointsComponent.Save(_mergedPractitioner);
				_replaceContactPointsComponent.Save();
				_confirmationComponent.PractitionerDetail = _mergedPractitioner;

				// The accept is enabled only on the very last page.
				this.AcceptEnabled = true;
			}
		}

		private void OnMovedBackward()
		{
			// The accept is enabled only on the very last page.  Nothing else to do when moving backward.
			this.AcceptEnabled = false;

			if (this.CurrentPage.Component == _replaceContactPointsComponent)
			{
				if (_replaceContactPointsComponent.DeactivatedContactPoints.Count == 0)
					Back();
			}
		}

		private static ExternalPractitionerDetail LoadPractitionerDetail(EntityRef practitionerRef)
		{
			ExternalPractitionerDetail detail = null;

			if (practitionerRef != null)
			{
				Platform.GetService(
					delegate(IExternalPractitionerAdminService service)
					{
						var request = new LoadExternalPractitionerForEditRequest(practitionerRef);
						var response = service.LoadExternalPractitionerForEdit(request);
						detail = response.PractitionerDetail;
					});
			}

			return detail;
		}

		private long CalculateMergeCost(MergeExternalPractitionerRequest request)
		{
			request.EstimateCostOnly = true;
			return ShowProgress("Calculating number of records affected...",
					  service => service.MergeExternalPractitioner(request).CostEstimate);
		}

		private static void PerformMergeOperation(MergeExternalPractitionerRequest request)
		{
			request.EstimateCostOnly = false;
			Platform.GetService<IExternalPractitionerAdminService>(
					service => service.MergeExternalPractitioner(request));
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

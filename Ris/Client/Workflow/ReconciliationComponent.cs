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
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.PatientReconciliation;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="ReconciliationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class PatientReconciliationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PatientReconciliationComponent class
	/// </summary>
	[AssociateView(typeof(PatientReconciliationComponentViewExtensionPoint))]
	public class ReconciliationComponent : ApplicationComponent
	{
		private PatientProfileDiffComponent _diffComponent;
		private ChildComponentHost _diffComponentHost;

		private PatientProfileSummary _selectedTargetProfile;
		private PatientProfileSummary _selectedReconciliationProfile;

		private PatientProfileTable _targetProfileTable;
		private ReconciliationCandidateTable _reconciliationProfileTable;

		private readonly IList<ReconciliationCandidate> _candidates;
		private readonly IList<PatientProfileSummary> _targetProfiles;

		/// <summary>
		/// Constructor
		/// </summary>
		public ReconciliationComponent(EntityRef targetProfileRef, IList<PatientProfileSummary> reconciledProfiles, IList<ReconciliationCandidate> candidates)
		{
			_targetProfiles = reconciledProfiles;
			_candidates = candidates;

			_selectedTargetProfile = CollectionUtils.SelectFirst(reconciledProfiles, p => p.PatientProfileRef.Equals(targetProfileRef, true));
		}

		public override void Start()
		{
			// create the diff component
			_diffComponentHost = new ChildComponentHost(this.Host, _diffComponent = new PatientProfileDiffComponent());
			_diffComponentHost.StartComponent();

			// add all target profiles - ensure the initially selected one is at the top of the list
			_targetProfileTable = new PatientProfileTable();
			_targetProfileTable.Items.Add(_selectedTargetProfile);
			foreach (var profile in _targetProfiles)
			{
				if (!profile.PatientProfileRef.Equals(_selectedTargetProfile.PatientProfileRef, true))
				{
					_targetProfileTable.Items.Add(profile);
				}
			}

			_reconciliationProfileTable = new ReconciliationCandidateTable();
			foreach (var match in _candidates)
			{
				var entry = new ReconciliationCandidateTableEntry(match);
				entry.CheckedChanged += CandidateCheckedChangedEventHandler;
				_reconciliationProfileTable.Items.Add(entry);
			}

			base.Start();
		}

		public override void Stop()
		{
			if (_diffComponentHost != null)
			{
				_diffComponentHost.StopComponent();
				_diffComponentHost = null;
			}

			base.Stop();
		}

		public IApplicationComponentView DiffComponentView
		{
			get { return _diffComponentHost.ComponentView; }
		}

		#region Presentation Model

		public ITable TargetProfileTable
		{
			get { return _targetProfileTable; }
		}

		public ITable ReconciliationProfileTable
		{
			get { return _reconciliationProfileTable; }
		}

		public void SetSelectedTargetProfile(ISelection selection)
		{
			var profile = (PatientProfileSummary)selection.Item;
			if (profile != _selectedTargetProfile)
			{
				_selectedTargetProfile = profile;
				UpdateDiff();
			}
		}

		public void SetSelectedReconciliationProfile(ISelection selection)
		{
			var entry = (ReconciliationCandidateTableEntry)selection.Item;
			var profile = (entry == null) ? null : entry.ReconciliationCandidate.PatientProfile;
			if (profile != _selectedReconciliationProfile)
			{
				_selectedReconciliationProfile = profile;
				UpdateDiff();
			}
		}

		public bool ReconcileEnabled
		{
			get { return AnyCandidatesChecked(); }
		}

		public void Reconcile()
		{
			try
			{
				DoReconciliation();
				this.Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.ExceptionReconcilePatientProfiles, this.Host.DesktopWindow,
					delegate
					{
						this.ExitCode = ApplicationComponentExitCode.Error;
						this.Host.Exit();
					});
			}
		}

		public void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
			this.Host.Exit();
		}

		#endregion

		private void UpdateDiff()
		{
			if (_selectedTargetProfile == null || _selectedReconciliationProfile == null)
			{
				_diffComponent.ProfilesToCompare = null;
			}
			else
			{
				_diffComponent.ProfilesToCompare = new[] { _selectedTargetProfile.PatientProfileRef, _selectedReconciliationProfile.PatientProfileRef };
			}
		}

		private void DoReconciliation()
		{
			var checkedPatients = new List<EntityRef>();
			foreach (var entry in _reconciliationProfileTable.Items)
			{
				if (entry.Checked && !checkedPatients.Contains(entry.ReconciliationCandidate.PatientProfile.PatientRef))
				{
					checkedPatients.Add(entry.ReconciliationCandidate.PatientProfile.PatientRef);
				}
			}

			Platform.GetService<IPatientReconciliationService>(service =>
			{
				// get the full list of all the profiles that will be reconciled if this operation is carried out
					var indirectlyReconciledProfiles = service.ListProfilesForPatients(new ListProfilesForPatientsRequest(checkedPatients)).Profiles;

					// confirmation
				var confirmComponent = new ReconciliationConfirmComponent(_targetProfiles, indirectlyReconciledProfiles);
				var confirmExitCode = ApplicationComponent.LaunchAsDialog(
					this.Host.DesktopWindow, confirmComponent, SR.TitleConfirmReconciliation);

				if (confirmExitCode == ApplicationComponentExitCode.Accepted)
				{
					// add the target patient to the set
					checkedPatients.Add(_targetProfiles[0].PatientRef);

					// reconcile
					service.ReconcilePatients(new ReconcilePatientsRequest(checkedPatients));
				}
			});
		}

		private void CandidateCheckedChangedEventHandler(object sender, EventArgs e)
		{
			var changedEntry = (ReconciliationCandidateTableEntry)sender;

			foreach (var entry in _reconciliationProfileTable.Items)
			{
				if (entry != changedEntry && entry.ReconciliationCandidate.PatientProfile.PatientRef.Equals(changedEntry.ReconciliationCandidate.PatientProfile.PatientRef, true))
				{
					entry.Checked = changedEntry.Checked;
					_reconciliationProfileTable.Items.NotifyItemUpdated(entry);
				}
			}

			NotifyPropertyChanged("ReconcileEnabled");
		}

		private bool AnyCandidatesChecked()
		{
			return CollectionUtils.Contains(_reconciliationProfileTable.Items, item => item.Checked);
		}
	}
}

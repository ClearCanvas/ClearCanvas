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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines an interface for providing custom editing pages to be displayed in the visit editor.
	/// </summary>
	public interface IVisitEditorPageProvider : IExtensionPageProvider<IVisitEditorPage, IVisitEditorContext>
	{
	}

	/// <summary>
	/// Defines an interface for providing a custom editor page with access to the editor
	/// context.
	/// </summary>
	public interface IVisitEditorContext
	{
		EntityRef VisitRef { get; }

		IDictionary<string, string> VisitExtendedProperties { get; }
	}

	/// <summary>
	/// Defines an interface to a custom visit editor page.
	/// </summary>
	public interface IVisitEditorPage : IExtensionPage
	{
		void Save();
	}

	/// <summary>
	/// Defines an extension point for adding custom pages to the visit editor.
	/// </summary>
	public class VisitEditorPageProviderExtensionPoint : ExtensionPoint<IVisitEditorPageProvider>
	{
	}

    public class VisitEditorComponent : NavigatorComponentContainer
    {
		#region VisitEditorContext

		class EditorContext : IVisitEditorContext
		{
			private readonly VisitEditorComponent _owner;

			public EditorContext(VisitEditorComponent owner)
			{
				_owner = owner;
			}

			public EntityRef VisitRef
			{
				get { return _owner._visitRef; }
			}

			public IDictionary<string, string> VisitExtendedProperties
			{
				get { return _owner._visit.ExtendedProperties; }
			}
		}

		#endregion

        private PatientProfileSummary _patient;
        private EntityRef _visitRef;
        private VisitDetail _visit;
        private VisitSummary _addedVisit;

        private VisitDetailsEditorComponent _visitEditor;

        private readonly bool _isNew;

		private List<IVisitEditorPage> _extensionPages;
		
		/// <summary>
        /// Constructor
        /// </summary>
        public VisitEditorComponent(PatientProfileSummary patientProfile)
        {
            _isNew = true;
			_patient = patientProfile;
        }

        public VisitEditorComponent(VisitSummary editVisit)
        {
            _isNew = false;
			_patient = editVisit.Patient;
            _visitRef = editVisit.VisitRef;
        }

        public override void Start()
        {
            Platform.GetService<IVisitAdminService>(
                delegate(IVisitAdminService service)
                {
                    LoadVisitEditorFormDataResponse response = service.LoadVisitEditorFormData(new LoadVisitEditorFormDataRequest());

                    this.Pages.Add(new NavigatorPage("Visit",
                        _visitEditor = new VisitDetailsEditorComponent(
                            response.VisitNumberAssigningAuthorityChoices,
                            response.PatientClassChoices,
                            response.PatientTypeChoices,
                            response.AdmissionTypeChoices,
                            response.AmbulatoryStatusChoices,
                            response.VisitStatusChoices,
							response.FacilityChoices,
							response.CurrentLocationChoices)));

					// JR (may 2008): these pages are not currently needed, and they are not complete, so
					// better just to remove them for now

					//this.Pages.Add(new NavigatorPage("Visit/Practitioners", 
					//    _visitPractionersSummary = new VisitPractitionersSummaryComponent(
					//        response.VisitPractitionerRoleChoices
					//    )));

					//this.Pages.Add(new NavigatorPage("Visit/Location", 
					//    _visitLocationsSummary = new VisitLocationsSummaryComponent(
					//        response.VisitLocationRoleChoices
					//    )));

                    if (_isNew)
                    {
                        _visit = new VisitDetail();
                        _visit.Patient = _patient;
                        _visit.VisitNumber.AssigningAuthority = response.VisitNumberAssigningAuthorityChoices.Count > 0 ?
                            response.VisitNumberAssigningAuthorityChoices[0] : null;
                        _visit.PatientClass = response.PatientClassChoices[0];
                        _visit.PatientType = response.PatientTypeChoices[0];
                        _visit.AdmissionType = response.AdmissionTypeChoices[0];
                        _visit.Status = response.VisitStatusChoices[0];
                        _visit.Facility = response.FacilityChoices[0];
                    }
                    else
                    {
                        LoadVisitForEditResponse loadVisitResponse = service.LoadVisitForEdit(new LoadVisitForEditRequest(_visitRef));
						_patient = loadVisitResponse.VisitDetail.Patient;
                        _visitRef = loadVisitResponse.VisitRef;
                        _visit = loadVisitResponse.VisitDetail;
                    }

                });

            _visitEditor.Visit = _visit;
			//_visitPractionersSummary.Visit = _visit;
			//_visitLocationsSummary.Visit = _visit;

			// instantiate all extension pages
			_extensionPages = new List<IVisitEditorPage>();
			foreach (IVisitEditorPageProvider pageProvider in new VisitEditorPageProviderExtensionPoint().CreateExtensions())
			{
				_extensionPages.AddRange(pageProvider.GetPages(new EditorContext(this)));
			}

			// add extension pages to navigator
			// the navigator will start those components if the user goes to that page
			foreach (IVisitEditorPage page in _extensionPages)
			{
				this.Pages.Add(new NavigatorPage(page.Path, page.GetComponent()));
			}

			this.ValidationStrategy = new AllComponentsValidationStrategy();

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }
        
        public VisitSummary VisitSummary
        {
            get { return _addedVisit; }
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
				// give extension pages a chance to save data prior to commit
				_extensionPages.ForEach(delegate(IVisitEditorPage page) { page.Save(); });

                Platform.GetService<IVisitAdminService>(
                    delegate(IVisitAdminService service)
                    {
                        if (_isNew)
                        {
                            AddVisitResponse response = service.AddVisit(new AddVisitRequest(_visit));
                            _addedVisit = response.Visit;
                            _patient = response.Visit.Patient;
                            _visitRef = response.Visit.VisitRef;
                        }
                        else
                        {
                            UpdateVisitResponse response = service.UpdateVisit(new UpdateVisitRequest(_visitRef, _visit));
                            _addedVisit = response.Visit;
							_patient = response.Visit.Patient;
							_visitRef = response.Visit.VisitRef;
                        }
                    });
                this.Exit(ApplicationComponentExitCode.Accepted);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionCannotAddUpdateVisit, this.Host.DesktopWindow,
                    delegate
                    {
                        this.ExitCode = ApplicationComponentExitCode.Error;
                        this.Host.Exit();
                    });
            }
        }

        public override void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
            this.Host.Exit();
        }
    }
}

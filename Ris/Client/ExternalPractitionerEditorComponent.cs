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
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines an interface for providing custom editing pages to be displayed in the practitioner editor.
	/// </summary>
	public interface IExternalPractitionerEditorPageProvider : IExtensionPageProvider<IExternalPractitionerEditorPage, IExternalPractitionerEditorContext>
	{
	}

	/// <summary>
	/// Defines an interface for providing a custom editor page with access to the editor
	/// context.
	/// </summary>
	public interface IExternalPractitionerEditorContext
	{
		EntityRef PractitionerRef { get; }

		IDictionary<string, string> PractitionerExtendedProperties { get; }
	}

	/// <summary>
	/// Defines an interface to a custom practitioner editor page.
	/// </summary>
	public interface IExternalPractitionerEditorPage : IExtensionPage
	{
		void Save();
	}

	/// <summary>
	/// Defines an extension point for adding custom pages to the staff editor.
	/// </summary>
	public class ExternalPractitionerEditorPageProviderExtensionPoint : ExtensionPoint<IExternalPractitionerEditorPageProvider>
	{
	}


	public class ExternalPractitionerEditorComponent : NavigatorComponentContainer
	{
		#region EditorContext

		class EditorContext : IExternalPractitionerEditorContext
		{
			private readonly ExternalPractitionerEditorComponent _owner;

			public EditorContext(ExternalPractitionerEditorComponent owner)
			{
				_owner = owner;
			}

			public EntityRef PractitionerRef
			{
				get { return _owner._practitionerRef; }
			}

			public IDictionary<string, string> PractitionerExtendedProperties
			{
				get { return _owner._practitionerDetail.ExtendedProperties; }
			}
		}

		#endregion

		private EntityRef _practitionerRef;
		private ExternalPractitionerDetail _practitionerDetail;

		// return values for staff
		private ExternalPractitionerSummary _practitionerSummary;

		private readonly bool _isNew;

		private ExternalPractitionerDetailsEditorComponent _detailsEditor;
		private ExternalPractitionerContactPointSummaryComponent _contactPointSummaryComponent;
		private readonly List<ExternalPractitionerContactPointDetail> _hiddenMergedContactPoints;

		private List<IExternalPractitionerEditorPage> _extensionPages;

		/// <summary>
		/// Constructs an editor to edit a new staff
		/// </summary>
		public ExternalPractitionerEditorComponent()
		{
			_isNew = true;
			_hiddenMergedContactPoints = new List<ExternalPractitionerContactPointDetail>();
		}

		/// <summary>
		/// Constructs an editor to edit an existing staff profile
		/// </summary>
		/// <param name="reference"></param>
		public ExternalPractitionerEditorComponent(EntityRef reference)
		{
			_isNew = false;
			_practitionerRef = reference;
			_hiddenMergedContactPoints = new List<ExternalPractitionerContactPointDetail>();
		}

		/// <summary>
		/// Gets summary of staff that was added or edited
		/// </summary>
		public ExternalPractitionerSummary ExternalPractitionerSummary
		{
			get { return _practitionerSummary; }
		}

		public override void Start()
		{
			LoadExternalPractitionerEditorFormDataResponse formDataResponse = null;

			Platform.GetService<IExternalPractitionerAdminService>(service =>
			{
				formDataResponse = service.LoadExternalPractitionerEditorFormData(new LoadExternalPractitionerEditorFormDataRequest());

				if (_isNew)
				{
					_practitionerDetail = new ExternalPractitionerDetail();
				}
				else
				{
					var response = service.LoadExternalPractitionerForEdit(new LoadExternalPractitionerForEditRequest(_practitionerRef));
					_practitionerRef = response.PractitionerDetail.PractitionerRef;
					_practitionerDetail = response.PractitionerDetail;
				}
			});

			_contactPointSummaryComponent = new ExternalPractitionerContactPointSummaryComponent(_practitionerRef,
				formDataResponse.AddressTypeChoices,
				formDataResponse.PhoneTypeChoices,
				formDataResponse.ResultCommunicationModeChoices,
				formDataResponse.InformationAuthorityChoices,
				Formatting.PersonNameFormat.Format(_practitionerDetail.Name));
			_contactPointSummaryComponent.SetModifiedOnListChange = true;

			var rootPath = SR.TitleExternalPractitioner;
			this.Pages.Add(new NavigatorPage("NodeExternalPractitioner", _detailsEditor = new ExternalPractitionerDetailsEditorComponent(_isNew)));
			this.Pages.Add(new NavigatorPage("NodeExternalPractitioner/NodeContactPoints", _contactPointSummaryComponent));

			this.ValidationStrategy = new AllComponentsValidationStrategy();

			_detailsEditor.ExternalPractitionerDetail = _practitionerDetail;

			// Hide the merged contact points from user.
			CollectionUtils.ForEach(_practitionerDetail.ContactPoints,
				delegate(ExternalPractitionerContactPointDetail cp)
					{
						if (cp.IsMerged)
							_hiddenMergedContactPoints.Add(cp);
						else
							_contactPointSummaryComponent.Subject.Add(cp);
					});

			// instantiate all extension pages
			_extensionPages = new List<IExternalPractitionerEditorPage>();
			foreach (IExternalPractitionerEditorPageProvider pageProvider in new ExternalPractitionerEditorPageProviderExtensionPoint().CreateExtensions())
			{
				_extensionPages.AddRange(pageProvider.GetPages(new EditorContext(this)));
			}

			// add extension pages to navigator
			// the navigator will start those components if the user goes to that page
			foreach (var page in _extensionPages)
			{
				this.Pages.Add(new NavigatorPage(page.Path, page.GetComponent()));
			}

			base.Start();
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
				_practitionerDetail.ContactPoints.Clear();
				foreach (var detail in _contactPointSummaryComponent.Subject)
				{
					_practitionerDetail.ContactPoints.Add(detail);
				}

				// Add the contact points back, otherwise server will think they are deleted.
				_practitionerDetail.ContactPoints.AddRange(_hiddenMergedContactPoints);

				// give extension pages a chance to save data prior to commit
				_extensionPages.ForEach(page => page.Save());

				Platform.GetService<IExternalPractitionerAdminService>(service =>
				{
					if (_isNew)
					{
						var response = service.AddExternalPractitioner(
								new AddExternalPractitionerRequest(_practitionerDetail, _detailsEditor.MarkVerified));

						_practitionerRef = response.Practitioner.PractitionerRef;
						_practitionerSummary = response.Practitioner;
					}
					else
					{
						var response = service.UpdateExternalPractitioner(
								new UpdateExternalPractitionerRequest(_practitionerDetail, _detailsEditor.MarkVerified));

						_practitionerRef = response.Practitioner.PractitionerRef;
						_practitionerSummary = response.Practitioner;
					}
				});

				this.Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.ExceptionSaveExternalPractitioner, this.Host.DesktopWindow,
					delegate
					{
						this.ExitCode = ApplicationComponentExitCode.Error;
						this.Host.Exit();
					});
			}
		}
	}
}

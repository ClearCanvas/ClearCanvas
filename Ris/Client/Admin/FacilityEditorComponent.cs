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
using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client.Admin
{
	/// <summary>
	/// Extension point for views onto <see cref="FacilityEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class FacilityEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// FacilityEditorComponent class
	/// </summary>
	[AssociateView(typeof(FacilityEditorComponentViewExtensionPoint))]
	public class FacilityEditorComponent : ApplicationComponent
	{
		private IList _informationAuthorityChoices; 
		
		private FacilityDetail _facilityDetail;
		private EntityRef _facilityRef;
		private readonly bool _isNew;

		private FacilitySummary _facilitySummary;

		/// <summary>
		/// Constructor
		/// </summary>
		public FacilityEditorComponent()
		{
			_isNew = true;
		}

		public FacilityEditorComponent(EntityRef facilityRef)
		{
			_isNew = false;
			_facilityRef = facilityRef;
		}

		public override void Start()
		{
			Platform.GetService(
				delegate(IFacilityAdminService service)
				{
					var formResponse = service.GetFacilityEditFormData(new GetFacilityEditFormDataRequest());
					_informationAuthorityChoices = formResponse.InformationAuthorityChoices;

					if (_isNew)
					{
						_facilityDetail = new FacilityDetail();
					}
					else
					{
						var response = service.LoadFacilityForEdit(new LoadFacilityForEditRequest(_facilityRef));
						_facilityRef = response.FacilityDetail.FacilityRef;
						_facilityDetail = response.FacilityDetail;
					}
				});

			
			base.Start();
		}

		public FacilitySummary FacilitySummary
		{
			get { return _facilitySummary; }
		}

		public FacilityDetail FacilityDetail
		{
			get { return _facilityDetail; }
			set { _facilityDetail = value; }
		}

		#region Presentation Model

		public IList InformationAuthorityChoices
		{
			get { return _informationAuthorityChoices; }
		}

		[ValidateNotNull]
		public string Name
		{
			get { return _facilityDetail.Name; }
			set
			{
				_facilityDetail.Name = value;
				this.Modified = true;
			}
		}

		[ValidateNotNull]
		public string Code
		{
			get { return _facilityDetail.Code; }
			set
			{
				_facilityDetail.Code = value;
				this.Modified = true;
			}
		}

		[ValidateNotNull]
		public string Description
		{
			get { return _facilityDetail.Description; }
			set
			{
				_facilityDetail.Description = value;
				this.Modified = true;
			}
		}

		[ValidateNotNull]
		public EnumValueInfo InformationAuthority
		{
			get { return _facilityDetail.InformationAuthority; }
			set
			{
				_facilityDetail.InformationAuthority = value;
				this.Modified = true;
			}
		}

		public void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
			}
			else
			{
				try
				{
					Platform.GetService(
						delegate(IFacilityAdminService service)
						{
							if (_isNew)
							{
								var response = service.AddFacility(new AddFacilityRequest(_facilityDetail));
								_facilityRef = response.Facility.FacilityRef;
								_facilitySummary = response.Facility;
							}
							else
							{
								var response = service.UpdateFacility(new UpdateFacilityRequest(_facilityDetail));
								_facilityRef = response.Facility.FacilityRef;
								_facilitySummary = response.Facility;
							}
						});

					this.Exit(ApplicationComponentExitCode.Accepted);
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, SR.ExceptionSaveFacility, this.Host.DesktopWindow,
						delegate
						{
							this.ExitCode = ApplicationComponentExitCode.Error;
							this.Host.Exit();
						});
				}
			}
		}

		public void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}

		public bool AcceptEnabled
		{
			get { return this.Modified; }
		}

		#endregion

		public event EventHandler AcceptEnabledChanged
		{
			add { this.ModifiedChanged += value; }
			remove { this.ModifiedChanged -= value; }
		}
	}
}

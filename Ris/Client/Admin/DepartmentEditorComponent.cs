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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.DepartmentAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
	/// <summary>
	/// Extension point for views onto <see cref="DepartmentEditorComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class DepartmentEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// DepartmentEditorComponent class.
	/// </summary>
	[AssociateView(typeof(DepartmentEditorComponentViewExtensionPoint))]
	public class DepartmentEditorComponent : ApplicationComponent
	{
		private List<FacilitySummary> _facilityChoices;

		private DepartmentDetail _detail;
		private EntityRef _itemRef;
		private readonly bool _isNew;

		private DepartmentSummary _summary;

		/// <summary>
		/// Constructor.
		/// </summary>
		public DepartmentEditorComponent()
		{
			_isNew = true;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public DepartmentEditorComponent(EntityRef itemRef)
		{
			_isNew = false;
			_itemRef = itemRef;
		}

		public DepartmentSummary DepartmentSummary
		{
			get { return _summary; }
		}

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			Platform.GetService<IDepartmentAdminService>(
				service =>
				{
					var formDataResponse = service.LoadDepartmentEditorFormData(
						new LoadDepartmentEditorFormDataRequest());
					_facilityChoices = formDataResponse.FacilityChoices;

					if (_isNew)
					{
						_detail = new DepartmentDetail();
					}
					else
					{
						var response = service.LoadDepartmentForEdit(new LoadDepartmentForEditRequest(_itemRef));
						_itemRef = response.Department.DepartmentRef;
						_detail = response.Department;
					}
				});

			base.Start();
		}

		#region Presentation Model

		[ValidateNotNull]
		public string Id
		{
			get { return _detail.Id; }
			set
			{
				_detail.Id = value;
				this.Modified = true;
			}
		}

		[ValidateNotNull]
		public string Name
		{
			get { return _detail.Name; }
			set
			{
				_detail.Name = value;
				this.Modified = true;
			}
		}

		public string Description
		{
			get { return _detail.Description; }
			set
			{
				_detail.Description = value;
				this.Modified = true;
			}
		}

		public IList FacilityChoices
		{
			get { return _facilityChoices; }
		}

		[ValidateNotNull]
		public FacilitySummary Facility
		{
			get { return _detail.Facility; }
			set
			{
				_detail.Facility = value;
				this.Modified = true;
			}
		}

		public string FormatFacility(object item)
		{
			var f = (FacilitySummary)item;
			return f.Name;
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
					SaveChanges();
					Exit(ApplicationComponentExitCode.Accepted);
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, SR.ExceptionSaveDepartment, this.Host.DesktopWindow,
					                        () => Exit(ApplicationComponentExitCode.Error));
				}
			}
		}

		public void Cancel()
		{
			Exit(ApplicationComponentExitCode.None);
		}

		public bool AcceptEnabled
		{
			get { return this.Modified; }
		}

		public event EventHandler AcceptEnabledChanged
		{
			add { this.ModifiedChanged += value; }
			remove { this.ModifiedChanged -= value; }
		}

		#endregion

		private void SaveChanges()
		{
			if (_isNew)
			{
				Platform.GetService<IDepartmentAdminService>(
					service =>
					{
						var response = service.AddDepartment(new AddDepartmentRequest(_detail));
						_itemRef = response.Department.DepartmentRef;
						_summary = response.Department;
					});
			}
			else
			{
				Platform.GetService<IDepartmentAdminService>(
					service =>
					{
						var response = service.UpdateDepartment(new UpdateDepartmentRequest(_detail));
						_itemRef = response.Department.DepartmentRef;
						_summary = response.Department;
					});
			}
		}
	}
}

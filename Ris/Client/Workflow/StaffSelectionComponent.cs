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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="StaffSelectionComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class StaffSelectionComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// SupervisorSelectionComponent class.
	/// </summary>
	[AssociateView(typeof(StaffSelectionComponentViewExtensionPoint))]
	public abstract class StaffSelectionComponent : ApplicationComponent
	{
		#region Private Methods

		private ILookupHandler _staffLookupHandler;
		private StaffSummary _staff;

		#endregion

		#region ApplicationComponent overrides

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			_staffLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow, this.StaffTypes);
			_staff = !string.IsNullOrEmpty(this.DefaultSupervisorID) ? GetStaffByID(this.DefaultSupervisorID) : null;

			base.Start();
		}

		#endregion

		#region Presentation model

		public StaffSummary Staff
		{
			get { return _staff; }
			set
			{
				if (!Equals(value, _staff))
				{
					SetStaff(value);
					NotifyPropertyChanged("Staff");
				}
			}
		}

		public ILookupHandler StaffLookupHandler
		{
			get { return _staffLookupHandler; }
		}

		public abstract string LabelText { get; }

		public void Accept()
		{
			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion

		protected abstract string[] StaffTypes { get; }
		protected abstract string DefaultSupervisorID { get; }

		protected virtual void SetStaff(StaffSummary staff)
		{
			_staff = staff;
		}

		private StaffSummary GetStaffByID(string id)
		{
			StaffSummary staff = null;
			Platform.GetService<IStaffAdminService>(
				delegate(IStaffAdminService service)
				{
					ListStaffResponse response = service.ListStaff(
						new ListStaffRequest(id, null, null, null, null, true));
					staff = CollectionUtils.FirstElement(response.Staffs);
				});
			return staff;
		}
	}
}

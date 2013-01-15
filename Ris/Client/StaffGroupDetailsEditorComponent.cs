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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="StaffGroupDetailsEditorComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class StaffGroupDetailsEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// StaffGroupDetailsEditorComponent class.
	/// </summary>
	[AssociateView(typeof(StaffGroupDetailsEditorComponentViewExtensionPoint))]
	public class StaffGroupDetailsEditorComponent : ApplicationComponent
	{
		private StaffGroupDetail _staffGroupDetail;

		public StaffGroupDetail StaffGroupDetail
		{
			get { return _staffGroupDetail; }
			set { _staffGroupDetail = value; }
		}

		#region Presentation Model

		[ValidateNotNull]
		public string Name
		{
			get { return _staffGroupDetail.Name; }
			set
			{
				if (_staffGroupDetail.Name != value)
				{
					_staffGroupDetail.Name = value;
					this.Modified = true;
					NotifyPropertyChanged("Name");
				}
			}
		}

		public string Description
		{
			get { return _staffGroupDetail.Description; }
			set
			{
				if (_staffGroupDetail.Description != value)
				{
					_staffGroupDetail.Description = value;
					this.Modified = true;
					NotifyPropertyChanged("Description");
				}
			}
		}

		public bool IsElective
		{
			get { return _staffGroupDetail.IsElective; }
			set
			{
				if (_staffGroupDetail.IsElective != value)
				{
					_staffGroupDetail.IsElective = value;
					this.Modified = true;
					NotifyPropertyChanged("IsElective");
				}
			}
		}

		#endregion
	}
}

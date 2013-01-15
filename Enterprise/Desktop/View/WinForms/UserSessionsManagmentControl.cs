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
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Enterprise.Desktop.View.WinForms
{
    public partial class UserSessionsManagmentControl : ApplicationComponentUserControl
    {
        private readonly UserSessionManagmentComponent _component;

        public UserSessionsManagmentControl(UserSessionManagmentComponent component):base(component)
        {
            InitializeComponent();

            _component = component;

            _userName.Value = component.User.UserName;
            _displayName.Value = component.User.DisplayName;
            _lastLogin.Value = component.User.LastLoginTime.HasValue
                            ? component.User.LastLoginTime.Value.ToString(ClearCanvas.Desktop.Format.DateTimeFormat)
                            : SR.LabelUnknownLastLoginTime;


            _sessionsTable.Table = component.SummaryTable;
            _sessionsTable.MenuModel = component.SummaryTableActionModel;
        	_sessionsTable.ToolbarModel = component.SummaryTableActionModel;
            _sessionsTable.DataBindings.Add("Selection", _component, "SummarySelection", true, DataSourceUpdateMode.OnPropertyChanged);
        }

		private void _closeButton_Click(object sender, EventArgs e)
		{
			_component.Close();
		}
    }
}

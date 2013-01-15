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
using System.Linq;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto subclasses of <see cref="SummaryComponentBase"/>
    /// </summary>
    [ExtensionOf(typeof(UserSessionManagmentComponentViewExtensionPoint))]
    class UserSessionsManagementView : WinFormsView, IApplicationComponentView
    {
        private UserSessionManagmentComponent _component;
        private UserSessionsManagmentControl _control;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (UserSessionManagmentComponent) component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                    _control = new UserSessionsManagmentControl(_component);

                return _control;

            }
        }
    }
}

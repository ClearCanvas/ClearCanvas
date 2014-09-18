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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	[MenuAction("changePassword", "global-menus/MenuTools/MenuUserProfile/MenuChangePassword", "ChangePassword")]
	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
    public class SessionManagerTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
    {
        public SessionManagerTool()
        {
        }

        public void ChangePassword()
        {
            if(SessionManager.ChangePassword())
            {
				this.Context.DesktopWindow.ShowMessageBox(SR.MessagePasswordChanged, MessageBoxActions.Ok);
            }
        }
    }
}

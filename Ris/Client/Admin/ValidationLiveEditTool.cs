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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Tool that allows editing of validation rules on a live application component.
    /// </summary>
	[MenuAction("launch", "applicationcomponent-metacontextmenu/MenuEditValidationRules", "Launch")]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Desktop.UIValidationRules)]
	[ExtensionOf(typeof(ApplicationComponentMetaToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
    public class ValidationLiveEditTool : Tool<IApplicationComponentMetaToolContext>
    {
        private Shelf _shelf;

        public void Launch()
        {
            if (_shelf == null)
            {
                try
                {
                    ValidationEditorComponent editor = new ValidationEditorComponent(this.Context.Component);

                    _shelf = ApplicationComponent.LaunchAsShelf(
                        this.Context.DesktopWindow,
                        editor,
                        string.Format("{0} rules editor", this.Context.Component.GetType().Name),
                        ShelfDisplayHint.DockFloat);
                    _shelf.Closed += delegate { _shelf = null; };

                }
                catch (Exception e)
                {
                    // could not launch component
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
            else
            {
                _shelf.Activate();
            }
        }
    }
}

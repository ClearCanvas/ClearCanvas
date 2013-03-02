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
using System.Configuration;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Configuration
{
	[SettingsGroupDescription("MPR user settings.")]
	[SettingsProvider(typeof(StandardSettingsProvider))]
	internal sealed partial class MprSettings
	{
        //TODO (Phoenix5): #10730 - remove this when it's fixed.
        #region WebStation Settings Hack
        [ThreadStatic]
        private static MprSettings _webDefault;

        public static MprSettings DefaultInstance
        {
            get
            {
                if (Application.GuiToolkitID == GuiToolkitID.Web)
                    return _webDefault ?? (_webDefault = new MprSettings());

                return Default;
            }
        }
        #endregion

	}
}
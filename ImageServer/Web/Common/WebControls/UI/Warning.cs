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
using System.Web.UI;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.UI
{
    [ToolboxData("<{0}:Warning runat=server></{0}:Warning>")]
    [Themeable(true)]
    public class Warning : System.Web.UI.WebControls.Image
    {
        
        #region Public Properties

        /// <summary>
        /// Sets or gets the warning message
        /// </summary>
        public string Message { get; set; }

        #endregion Public Properties

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            base.ToolTip = Message;
        }
    }
}
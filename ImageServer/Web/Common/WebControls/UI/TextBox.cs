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

using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.UI
{
    [ToolboxData("<{0}:TextBox runat=server></{0}:TextBox>")]
    [Themeable(true)]
    public class TextBox : System.Web.UI.WebControls.TextBox
    {
        private bool _readOnly;

        #region Public Properties

        /// <summary>
        /// Sets or gets the ReadOnly property. Fixes a bug that prevents data
        /// changed in a textbox programatically from posting back to the server.
        /// </summary>
        public new bool ReadOnly
        {
            get
            {
                return _readOnly;               
            }

            set
            {
                _readOnly = value;
                if (_readOnly)
                    Attributes.Add("readonly", "readonly");
                else
                    Attributes.Remove("readonly"); 
            }
        }

        /// <summary>
        /// Sets or gets the text.  The text is trimmed of whitespace.
        /// </summary>
        public string TrimText
        {
            get { return base.Text == null ? null : base.Text.Trim(); }
            set { base.Text = value == null ? null : value.Trim(); }
        }

        #endregion Public Properties

    }
}

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

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    public partial class JQuery : System.Web.UI.UserControl
    {
        private bool _multiselect = false;
    	private bool _maskedinput = false;

        public bool MultiSelect
        {
            get { return _multiselect;  }
            set { _multiselect = value; }
        }

		public bool MaskedInput
		{
			get { return _maskedinput; }
			set { _maskedinput = value; }
		}
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "jQuery", ResolveUrl("~/Scripts/jquery/jquery-1.3.2.min.js"));

            //Default Libraries
            Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "ClearCanvas", ResolveUrl("~/Scripts/ClearCanvas.js"));
            Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "DropShadow", ResolveUrl("~/Scripts/jquery/jquery.dropshadow.js")); 

            if(MultiSelect)
            {
                Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "Dimensions", ResolveUrl("~/Scripts/jquery/jquery.dimensions.js"));
                Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "MultiSelect", ResolveUrl("~/Scripts/jquery/jquery.multiselect.js")); 
            }

			if (MaskedInput)
			{
				Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "MaskedInput", ResolveUrl("~/Scripts/jquery/jquery.maskedinput-1.2.1.js"));
			}
		}
    }
}
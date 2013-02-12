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
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Web.Common.WebControls.Validators;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    [Themeable(true)]
    public partial class InvalidInputIndicator : UserControl, IInvalidInputIndicator
    {
        private int _referenceCounter;

        public String ImageUrl
        {
            get { return Image.ImageUrl; }
            set { Image.ImageUrl = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ContainerPanel.Attributes.Add("shared", _referenceCounter>1? "true":"false");
            ContainerPanel.Attributes.Add("numberofinvalidfields", "0");
 
            ContainerPanel.Style.Add("display", "block");
            ContainerPanel.Style.Add("visibility", "hidden");
        }

        public bool IsVisible
        {
            get { return ContainerPanel.Style[HtmlTextWriterStyle.Visibility] == "visible"; }
        }
                
           
        public Control Container
        {
            get { return ContainerPanel; }
        }

        public void Show()
        {
            ContainerPanel.Style[HtmlTextWriterStyle.Visibility] = "visible";

        }

        public void Hide()
        {
            ContainerPanel.Style[HtmlTextWriterStyle.Visibility] = "hidden";
             
        }


        public Label TooltipLabel
        {
            get { return HintLabel; }
        }

        

        public void AttachValidator(Common.WebControls.Validators.BaseValidator validator)
        {
            _referenceCounter ++;
            validator.InputControl.Attributes.Add("multiplevalidators", _referenceCounter > 1 ? "true" : "false");
        }


        #region IInvalidInputIndicator Members


        public Control TooltipLabelContainer
        {
            get
            {
                return HintPanel;

            }
        }

        #endregion
    }
}
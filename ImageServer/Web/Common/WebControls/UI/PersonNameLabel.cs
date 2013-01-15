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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.UI
{
    /// <summary>
    /// Represents a label control which displays a person name on a Web page.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="PersonNameLabel"/> to display a person name on a web page. The person's name to be displayed is set through 
    /// the <see cref="PersonName"/> property. <see cref="PersonNameLabel"/> accepts person name in dicom format. To indicate 
    /// Dicom formatted name, set <see cref="PersonNameType"/> to <see cref="NameType.Dicom"/>. In such case, the The person name will be rendered 
    /// in a human readable format (e.g., "Smith, John" )
    /// 
    ///  
    /// is not set, the date/time format will set to one of the followings, in the listed order:
    /// 
    /// - The date and time formats specified in the web configuration. 
    /// - The default date/time formats for the UI culture specified in <globalization>
    /// - The default date/time format for the region setting of the system. For eg, for English (Canada) region, 
    /// the default Long date format is "MMMM dd, yyyy". For US, the default date format is "DDD, MMMM dd, yyyy"
    /// 
    /// </remarks>
    /// <example>
    /// </example>
    [DefaultProperty("Value")]
    [ToolboxData("<{0}:PatientNameLabel runat=server></{0}:PatientNameLabel>")]
    public class PersonNameLabel : Label
    {

        public enum NameType
        {
            Normal,
            Dicom
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string PersonName
        {
            get
            {
                return ViewState["PersonName"] as string;
            }
            set
            {
                ViewState["PersonName"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public NameType PersonNameType
        {
            get
            {
                return (NameType)ViewState["PersonNameType"];
            }
            set
            {
                ViewState["PersonNameType"] = value;
            }
        }


        protected override void RenderContents(HtmlTextWriter writer)
        {
            string text = GetRenderText();
            if (!String.IsNullOrEmpty(text))
                writer.Write(text);
        }
        
        protected string GetRenderText()
        {
            if (String.IsNullOrEmpty(Text))
                return GetRenderedPatientName();
            else
                return String.Format(Text, GetRenderedPatientName());
        }

        protected virtual string GetRenderedPatientName()
        {
            string name = PersonName;

            switch (PersonNameType)
            {
                case NameType.Normal:
                    return name;

                case NameType.Dicom:
                    PersonName pn = new PersonName(name);
                    return NameFormatter.Format(name, UISettings.Default.NameFormat);

                default:
                    return name; // no formatting
            }
        }
    }
}

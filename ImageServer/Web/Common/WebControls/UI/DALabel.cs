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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageServer.Web.Common.Utilities;
using System.Threading;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.UI
{
    /// <summary>
    /// Represents a text label control which displays a DICOM DA value on a Web page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use the <see cref="DALabel"/> to display a date on a web page for a DICOM DA value. Unlike the DA value, the time displayed on the web page will
    /// be more user-friendly. The date to be displayed (DA value) is set through  the <see cref="Value"/> property. The format of the time can be set through the <see cref="DateTimeLabel.Format"/> property. 
    /// If <see cref="DateTimeLabel.Format"/>  is not set, the date format will set to one of the followings, in the listed order:
    /// </para>
    /// 
    /// - The date format specified in the web configuration. 
    /// - The default date format for the UI culture specified in <globalization>
    /// - The default date format for the region and langugage of the system.
    /// 
    /// <para>
    /// If the DA value is empty or null, the text specified in <see cref="EmptyValueText"/> will be displayed. If the <see cref="Value"/> is an invalid DA value,
    /// the text specified in <see cref="InvalidValueText"/> will be displayed.
    /// </para>
    /// 
    /// <seealso cref="DateTimeLabel"/>
    /// 
    /// </remarks>
    /// <example>
    /// </example>
    /// 
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:DALabel runat=server></{0}:DALabel>")]
    public class DALabel : DateTimeLabel
    {
        /// <summary>
        /// The DA value for the date to be displayed on the web page
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public new string Value
        {
            get
            {
                return ViewState["ValueDA"] as string;
            }

            set
            {
                ViewState["ValueDA"] = value;
            }
        }

        /// <summary>
        /// The text to be displayed on the web page when the DA value is empty
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]        
        public string EmptyValueText
        {
            get
            {
                return ViewState["EmptyValueText"] as string;
            }

            set
            {
                ViewState["EmptyValueText"] = value;
            }
        }

        /// <summary>
        /// The text to be displayed on the web page when the DA value is invalid
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string InvalidValueText
        {
            get
            {
                return ViewState["InvalidValueText"] as string;
            }

            set
            {
                ViewState["InvalidValueText"] = value;
            }
        }


        protected override string GetRenderedDateTimeText()
        {
            if (String.IsNullOrEmpty(Value))
                return EmptyValueText;

            DateTime? dt = DateParser.Parse(Value);

            if (dt != null)
            {
                if (String.IsNullOrEmpty(Format))
                    return DateTimeFormatter.Format(dt.Value, DateTimeFormatter.Style.Date);
                else
                    return dt.Value.ToString(Format);
            }
            else
                return null;
           
        }

       
    }
}

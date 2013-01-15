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
using System.Globalization;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.Validators
{
    /// <summary>
    /// Validate if the value in the number input control is in a specified range.
    /// </summary>
    /// <remarks>
    /// This control has slightly different behaviour than standard ASP.NET <seealso cref="RangeValidator"/>.
    /// Developers can optionally specify the background color for the input control if the validation fails.
    /// </remarks>
    /// 
    /// <example>
    /// The following block adds validation for the Port number. If the input is not within 0 and 32000
    /// the Port text box will be highlighted.
    /// 
    /// <clearcanvas:RangeValidator 
    ///                                ID="RangeValidator1" runat="server"
    ///                                ControlToValidate="PortTextBox"
    ///                                InvalidInputBackColor="#FAFFB5"
    ///                                ValidationGroup="vg1" 
    ///                                MinValue="0"
    ///                                MaxValue="32000"
    ///                                ErrorMessage="The Port number is not valid.">
    /// </clearcanvas:RangeValidator>
    /// 
    /// </example>
    /// 
    public class RangeValidator : BaseValidator
    {
        #region Public Properties

        /// <summary>
        /// Sets or gets the minimum acceptable value.
        /// </summary>
        public int MinValue { get; set; }

        /// <summary>
        /// Sets or gets the maximum acceptable value.
        /// </summary>
        public int MaxValue { get; set; }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// Called during server-side validation
        /// </summary>
        /// <returns></returns>
        protected override bool OnServerSideEvaluate()
        {
            Decimal value;
            if (Decimal.TryParse(GetControlValidationValue(ControlToValidate), NumberStyles.Number, null, out value))
            {
                return value >= MinValue && value <= MaxValue;
            }

            return false;
        }


        protected override void RegisterClientSideValidationExtensionScripts()
        {
            var template =
                new ScriptTemplate(this, "ClearCanvas.ImageServer.Web.Common.WebControls.Validators.RangeValidator.js");
            template.Replace("@@CONDITION_CHECKBOX_CLIENTID@@",
                             ConditionalCheckBox != null ? ConditionalCheckBox.ClientID : "null");
            template.Replace("@@VALIDATE_WHEN_UNCHECKED@@", ValidateWhenUnchecked ? "true" : "false");
            template.Replace("@@MIN_VALUE@@", MinValue.ToString());
            template.Replace("@@MAX_VALUE@@", MaxValue.ToString());

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID + "_ValidatorClass", template.Script, true);
        }

        #endregion Protected Methods
    }
}
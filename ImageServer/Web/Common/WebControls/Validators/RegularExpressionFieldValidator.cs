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

using System.Text.RegularExpressions;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.Validators
{
    /// <summary>
    /// Validate if an input control value matches a specified regular expression.
    /// </summary>
    /// <remarks>
    /// This control has slightly different behaviour than standard ASP.NET <seealso cref="RegularExpressionFieldValidator"/>.
    /// Developers can optionally specify the background color for the input control if the validation fails.
    /// </remarks>
    /// 
    /// <example>
    /// The following block adds validation for the IP Address. If the input is not an IP address, the IP address
    /// text box will be highlighted.
    /// 
    /// <clearcanvas:RegularExpressionFieldValidator 
    ///                                ID="RegularExpressionFieldValidator1" runat="server"
    ///                                ControlToValidate="IPAddressTextBox"
    ///                                InvalidInputBackColor="#FAFFB5"
    ///                                ValidationGroup="vg1" 
    ///                                ValidationExpression="^([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])$"
    ///                                ErrorMessage="The IP address is not valid." Display="None">
    /// </clearcanvas:RegularExpressionFieldValidator>
    /// 
    /// </example>
    /// 
    public class RegularExpressionFieldValidator : BaseValidator
    {
        #region Public Properties

        /// <summary>
        /// Sets or gets the regular expression to validate the input.
        /// </summary>
        public string ValidationExpression { get; set; }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// Called during server-side validation
        /// </summary>
        /// <returns></returns>
        protected override bool OnServerSideEvaluate()
        {
            string value = GetControlValidationValue(ControlToValidate);

            if (string.IsNullOrEmpty(value) && IgnoreEmptyValue)
                return true;

            if (value!=null && value.Trim() == string.Empty && IgnoreEmptyValue)
                return true;

            var regex = new Regex(ValidationExpression);
            
            return value != null && regex.IsMatch(value);
        }


        protected override void RegisterClientSideValidationExtensionScripts()
        {
            var template =
                new ScriptTemplate(this,
                                   "ClearCanvas.ImageServer.Web.Common.WebControls.Validators.RegularExpressionValidator.js");
            template.Replace("@@REGULAR_EXPRESSION@@", ValidationExpression.Replace("\\", "\\\\").Replace("'", "\\'"));
            template.Replace("@@CONDITION_CHECKBOX_CLIENTID@@",
                             ConditionalCheckBox != null ? ConditionalCheckBox.ClientID : "null");
            template.Replace("@@VALIDATE_WHEN_UNCHECKED@@", ValidateWhenUnchecked ? "true" : "false");
            template.Replace("@@IGNORE_EMPTY_VALUE@@", IgnoreEmptyValue ? "true" : "false");


            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID + "_ValidatorClass", template.Script, true);
        }

        #endregion Protected Methods
    }
}
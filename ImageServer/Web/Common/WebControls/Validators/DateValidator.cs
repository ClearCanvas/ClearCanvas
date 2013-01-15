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
    /// Helper class to parse a date input from the GUI
    /// </summary>
    public static class InputDateParser
    {
        private const string InputFormat = "yyyyMMdd";

        public static string DateFormat
        {
            get
            {
                return InputDateParser.InputFormat;
            }
        }

        public static bool TryParse(string value, out DateTime result)
        {
            return DateTime.TryParseExact(value, DateFormat, null, DateTimeStyles.AssumeLocal, out result);
        }
    }

    /// <summary>
    /// Validate a given date against the current date ensuring the given date is not in the future.
    /// </summary>
    /// <example>
    /// <para>
    /// The following example adds min password length validation. 
    /// </code>
    /// <uc1:InvalidInputIndicator ID="BirthDateIndicator" runat="server" 
    ///     ImageUrl="~/images/icons/HelpSmall.png"
    ///     Visible="true"/>
    ///                                                        
    /// <clearcanvas:DateValidator runat="server" ID="DateValidator" 
    ///         ControlToValidate="BirthDateTextBox"
    ///         InputName="BirthDate" 
    ///         InvalidInputColor="#FAFFB5" 
    ///         InvalidInputIndicatorID="InvalidBirthDateIndicator"
    ///         MinLength="8"
    ///         ErrorMessage="Birth Date may not be in the future"
    ///         Display="None" ValidationGroup="vg1"/> 
    /// </code>
    /// </example>
    public class DateValidator : BaseValidator
    {
        #region Protected Methods

        protected override bool OnServerSideEvaluate()
        {
            string value = GetControlValidationValue(ControlToValidate);
            if (String.IsNullOrEmpty(value))
            {
                if (IgnoreEmptyValue)
                    return true;
                else
                {
                    Text = ValidationErrors.ThisFieldIsRequired;
                    return false;
                }
            }

            DateTime result;
            if (!InputDateParser.TryParse(value, out result))
            {
                return false;
            }

            return true;
        }

        #endregion Protected Methods

        protected override void RegisterClientSideValidationExtensionScripts()
        {
            var template =
                new ScriptTemplate(this, "ClearCanvas.ImageServer.Web.Common.WebControls.Validators.DateValidator.js");

            template.Replace("@@CONDITION_CHECKBOX_CLIENTID@@",
                             ConditionalCheckBox != null ? ConditionalCheckBox.ClientID : "null");
            template.Replace("@@VALIDATE_WHEN_UNCHECKED@@", ValidateWhenUnchecked ? "true" : "false");
            template.Replace("@@IGNORE_EMPTY_VALUE@@", IgnoreEmptyValue ? "true" : "false");
            template.Replace("@@DATE_FORMAT@@", InputDateParser.DateFormat);
            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID + "_ValidatorClass", template.Script, true);
        }
    }
}
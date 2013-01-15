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
using System.Web.UI.WebControls;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.Validators
{
    /// <summary>
    /// Validate a required Web UI input control containing value based on the state of another checkbox control.
    /// </summary>
    /// <remarks>
    /// This control has slightly different behaviour than standard ASP.NET <seealso cref="RequiredFieldValidator"/>.
    /// Users can use this control for required field validation based on state of a checkbox on the UI. When the 
    /// condition is satisfied, the control will validate the input field contains a value. Developers 
    /// can optionally specify the background color for the input control if the validation fails.
    /// </remarks>
    /// 
    /// <example>
    /// The following block adds validation for the SIN if the citizen checkbox is checked:
    /// 
    /// <clearcanvas:ConditionalRequiredFieldValidator 
    ///                                ID="RequiredFieldValidator2" runat="server" 
    ///                                ControlToValidate="SINTextBox"
    ///                                ConditionalCheckBoxID="IsCitizenCheckedBox" 
    ///                                RequiredWhenChecked="true"
    ///                                InvalidInputBackColor="#FAFFB5"
    ///                                ValidationGroup="vg1" 
    ///                                EnableClientScript="true"
    ///                                ErrorMessage="SIN is required for citizen!!">
    /// </clearcanvas:ConditionalRequiredFieldValidator>
    /// 
    /// </example>
    /// 
    public class ConditionalRequiredFieldValidator : BaseValidator
    {
        #region Public Properties

        #endregion Public Properties

        #region Protected Methods

        //protected override void RegisterClientSideValidationExtensionScripts()
        //{
        //    if (ConditionalCheckBoxID != null)
        //    {
        //        ScriptTemplate template =
        //            new ScriptTemplate(GetType().Assembly,
        //                               "ClearCanvas.ImageServer.Web.Common.WebControls.ConditionalRequiredFieldValidator_OnValidate_Conditional.js");
        //        template.Replace("@@CLIENTID@@", ClientID);
        //        template.Replace("@@FUNCTION_NAME@@", ClientEvalFunctionName);
        //        template.Replace("@@INPUT_CLIENTID@@", InputControl.ClientID);
        //        template.Replace("@@CONDITIONAL_CONTROL_CLIENTID@@", GetControlRenderID(ConditionalCheckBoxID));
        //        template.Replace("@@REQUIRED_WHEN_CHECKED@@", RequiredWhenChecked.ToString().ToLower());
        //        template.Replace("@@IGNORE_EMPTY_VALUE@@", IgnoreEmptyValue.ToString().ToLower());
        //        template.Replace("@@ERROR_MESSAGE@@", ErrorMessage);

        //        Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientEvalFunctionName, template.Script, true);
        //    }
        //    else
        //    {
        //        ScriptTemplate template =
        //            new ScriptTemplate(GetType().Assembly,
        //                               "ClearCanvas.ImageServer.Web.Common.WebControls.ConditionalRequiredFieldValidator_OnValidate.js");
        //        template.Replace("@@CLIENTID@@", ClientID); 
        //        template.Replace("@@FUNCTION_NAME@@", ClientEvalFunctionName);
        //        template.Replace("@@INPUT_CLIENTID@@", InputControl.ClientID);
        //        template.Replace("@@ERROR_MESSAGE@@", ErrorMessage);

        //        Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientEvalFunctionName, template.Script, true);
        //    }
        //}

        protected override bool OnServerSideEvaluate()
        {
            string value = GetControlValidationValue(ControlToValidate);

            if (String.IsNullOrEmpty(value))
            {
                return false;
            }


            return true;
        }

        #endregion Protected Methods

        protected override void RegisterClientSideValidationExtensionScripts()
        {
            RegisterClientSideBaseValidationScripts();

            var template =
                new ScriptTemplate(this,
                                   "ClearCanvas.ImageServer.Web.Common.WebControls.Validators.ConditionalRequiredFieldValidator.js");

            template.Replace("@@CONDITION_CHECKBOX_CLIENTID@@",
                             ConditionalCheckBox != null ? ConditionalCheckBox.ClientID : "null");
            template.Replace("@@VALIDATE_WHEN_UNCHECKED@@", ValidateWhenUnchecked ? "true" : "false");
            template.Replace("@@IGNORE_EMPTY_VALUE@@", IgnoreEmptyValue ? "true" : "false");

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID + "_ValidatorClass", template.Script, true);
        }
    }
}
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

namespace ClearCanvas.ImageServer.Web.Common.WebControls.Validators
{
    /// <summary>
    /// Validate length of the input.
    /// </summary>
    /// <example>
    /// <para>
    /// The following example adds min password length validation. 
    /// </code>
    /// <uc1:InvalidInputIndicator ID="InvalidZipCodeIndicator" runat="server" 
    ///     ImageUrl="~/images/icons/HelpSmall.png"
    ///     Visible="true"/>
    ///                                                        
    /// <clearcanvas:FilesystemPathValidator runat="server" ID="PasswordValidator" 
    ///         ControlToValidate="PasswordTextBox"
    ///         InputName="Password" 
    ///         InvalidInputColor="#FAFFB5" 
    ///         InvalidInputIndicatorID="InvalidPasswordIndicator"
    ///         MinLength="8"
    ///         ErrorMessage="Invalid password"
    ///         Display="None" ValidationGroup="vg1"/> 
    /// </code>
    /// </example>
    public class LengthValidator : BaseValidator
    {
        private int _maxLength = Int32.MaxValue;
        private int _minLength = Int32.MinValue;

        #region Public Properties

        public int MinLength
        {
            get { return _minLength; }
            set { _minLength = value; }
        }

        public int MaxLength
        {
            get { return _maxLength; }
            set { _maxLength = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected override bool OnServerSideEvaluate()
        {
            //String value = GetControlValidationValue(ControlToValidate);
            //if (value == null || value.Length < MinLength || value.Length>MaxLength)
            //{
            //    ErrorMessage = String.Format("Must be at least {0} and no more than {1} characters.", MinLength, MaxLength);
            //    return false;
            //}
            //else
            //    return true;
            return true;
        }

        #endregion Protected Methods

        protected override void RegisterClientSideValidationExtensionScripts()
        {
            var template =
                new ScriptTemplate(this, "ClearCanvas.ImageServer.Web.Common.WebControls.Validators.LengthValidator.js");

            template.Replace("@@MIN_LENGTH@@", MinLength.ToString());
            template.Replace("@@MAX_LENGTH@@", MaxLength.ToString());
            template.Replace("@@CONDITION_CHECKBOX_CLIENTID@@",
                             ConditionalCheckBox != null ? ConditionalCheckBox.ClientID : "null");
            template.Replace("@@VALIDATE_WHEN_UNCHECKED@@", ValidateWhenUnchecked ? "true" : "false");
            template.Replace("@@IGNORE_EMPTY_VALUE@@", IgnoreEmptyValue ? "true" : "false");

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID + "_ValidatorClass", template.Script, true);
        }
    }
}
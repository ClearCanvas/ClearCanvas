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
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.Validators
{
    /// <summary>
    /// Base validator class for all custom validators.
    /// </summary>
    /// <remarks>
    /// All derived classes must override <see cref="OnServerSideEvaluate"/> and <see cref="RegisterClientSideValidationExtensionScripts"/>
    /// to provide the result of the validation. Other validation aspects such as showing/hidding the error indication, highlighting
    /// the input will be taken cared of by the base class.
    /// </remarks>
    public abstract class BaseValidator : System.Web.UI.WebControls.BaseValidator
    {
        #region Private members

        private string _inputName;
        private string _invalidInputIndicatorID = "";

        #endregion Private members

        #region Public Properties

        /// <summary>
        /// Sets or gets the ID of the condition control.
        /// </summary>
        /// <remarks>
        /// The condition control indicates whether the input control associated with the validator
        /// control must contain a value.
        /// 
        /// If <seealso cref="ConditionalCheckBoxID"/> is not specified, <seealso cref="ConditionalRequiredFieldValidator"/>
        /// behaves the same as <seealso cref="RequiredFieldValidator"/> (ie, the input field must always contains value).
        /// </remarks>
        public string ConditionalCheckBoxID { get; set; }

        /// <summary>
        /// Gets the reference to the control that contains the input to be validated
        /// </summary>
        public WebControl ConditionalCheckBox
        {
            get
            {
                if (ConditionalCheckBoxID == null)
                    return null;
                return FindControl(ConditionalCheckBoxID) as WebControl;
            }
        }

        /// <summary>
        /// Indicates whether the input control must contain a value 
        /// when the checkbox specified by <seealso cref="ConditionalCheckBoxID"/>
        /// is checked or is unchecked.
        /// </summary>
        public bool ValidateWhenUnchecked { get; set; }

        /// <summary>
        /// Gets the reference to the control that contains the input to be validated
        /// </summary>
        public WebControl InputControl
        {
            get { return FindControl(ControlToValidate) as WebControl; }
        }

        /// <summary>
        /// Gets or sets the id of the control that will be displayed when the input is invalid.
        /// </summary>
        /// <remarks>
        /// The popup control must be have <see cref="IInvalidInputIndicator"/> interface.</remarks>
        public string InvalidInputIndicatorID
        {
            get { return _invalidInputIndicatorID; }
            set { _invalidInputIndicatorID = value; }
        }

        /// <summary>
        /// Name of the input to be validated.
        /// </summary>
        public string InputName
        {
            get
            {
                if (String.IsNullOrEmpty(_inputName))
                    return ControlToValidate;

                return _inputName;
            }
            set { _inputName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreEmptyValue { get; set; }


        /// <summary>
        /// Sets or retrieve the specified background color of the input control when the validation fails.
        /// </summary>
        public Color InvalidInputColor { get; set; }

        /// <summary>
        /// Sets or retrieve the specified css for the input control when the validation fails.
        /// </summary>
        public String InvalidInputCSS { get; set; }

        /// <summary>
        /// Sets or retrieve the specified background color of the input control when the validation fails.
        /// </summary>
        public Color InvalidInputBorderColor { get; set; }


        public bool ValidateWhenDisabled { get; set; }

        #endregion Public Properties

        #region Protected Properties

        /// <summary>
        /// Name of client side script evaluation function to be called during the validation.
        /// </summary>
        protected string ClientEvalFunctionName
        {
            get { return ClientID + "_Evaluation"; }
        }


        /// <summary>
        /// Gets the control used to indicate the input is invalid
        /// </summary>
        public IInvalidInputIndicator InvalidInputIndicator
        {
            get { return FindControl(InvalidInputIndicatorID) as IInvalidInputIndicator; }
        }


        /// <summary>
        /// Gets or sets the background color value for the input control when validation passes.
        /// </summary>
        public Color InputNormalColor
        {
            get {
                return ViewState["ValidateCtrlBackColor"] == null
                           ? Color.Empty
                           : (Color) ViewState["ValidateCtrlBackColor"];
            }
            set { ViewState["ValidateCtrlBackColor"] = value; }
        }

        /// <summary>
        /// Gets or sets the background color value for the input control when validation passes.
        /// </summary>
        public Color InputNormalBorderColor
        {
            get {
                return ViewState["ValidateCtrlBorderColor"] == null
                           ? Color.Empty
                           : (Color) ViewState["ValidateCtrlBorderColor"];
            }
            set { ViewState["ValidateCtrlBorderColor"] = value; }
        }

        /// <summary>
        /// Gets or sets the CSS class for the input control when validation passes.
        /// </summary>
        public string InputNormalCSS
        {
            get {
                return ViewState["ValidateCtrlNormalCSS"] == null
                           ? string.Empty
                           : (string) ViewState["ValidateCtrlNormalCSS"];
            }
            set { ViewState["ValidateCtrlNormalCSS"] = value; }
        }

        protected string ClientSideOnValidateFunctionName
        {
            get { return ClientID + "_OnClientSideValidation"; }
        }


        protected string OnInvalidInputFunctionName
        {
            get { return ClientID + "_OnInvalidInput"; }
        }

        protected string OnValidInputFunctionName
        {
            get { return ClientID + "_OnValidInput"; }
        }

        #endregion Protected Properties

        #region Protected methods

        /// <summary>
        /// Escapes special character in a text so that it can be used as a string in javascript.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected string Escape(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return text.Replace("'", "\\'");
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);

            if (DetermineRenderUplevel() && EnableClientScript)
            {
                Page.ClientScript.RegisterExpandoAttribute(ClientID, "evaluationfunction",
                                                           ClientSideOnValidateFunctionName);
                writer.AddAttribute("evaluationfunction", ClientSideOnValidateFunctionName);
            }
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (InvalidInputIndicator != null)
                InvalidInputIndicator.AttachValidator(this);

            // This are special attributes used during the validation. Basically we want to "attach" this validator
            // to the input control so that we are "aware of" other validators.
            //
            // "validatorscounter"  =  number of validator controls attached to a specific input
            // "calledvalidatorcounter" = number of validator controls which have been called during the validation.
            // 
            // During the validation, "calledvalidatorcounter" will change. We can use that to determine 
            // if we are the first validator called by the browser to evaluate the input. 
            // If so, we can safely hide any error indicator that's shown previously if the current input is valid. 
            //

            // Increment "validatorscounter" value to include this validator
            int counter = 0;
            if (InputControl.Attributes["validatorscounter"] != null)
            {
                counter = Int32.Parse(InputControl.Attributes["validatorscounter"]);
            }
            InputControl.Attributes.Add("validatorscounter", String.Format("{0}", counter + 1));

            // set calledvalidatorcounter=0
            if (InputControl.Attributes["calledvalidatorcounter"] == null)
            {
                InputControl.Attributes.Add("calledvalidatorcounter", "0");
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);


            if (!Page.IsPostBack)
            {
                // save current background color
                InputNormalColor = InputControl.BackColor;
                InputNormalBorderColor = InputControl.BorderColor;
                InputNormalCSS = InputControl.CssClass;
            }
            else
            {
                // Restore the input background color
                InputControl.BackColor = InputNormalColor;
                InputControl.BorderColor = InputNormalBorderColor;
                InputControl.Attributes["class"] = InputNormalCSS;
            }

            if (EnableClientScript)
            {
                RegisterClientSideBaseValidationScripts();
                RegisterClientSideValidationExtensionScripts();
            }
        }


        protected void RegisterClientSideBaseValidationScripts()
        {
            var template =
                new ScriptTemplate(this, "ClearCanvas.ImageServer.Web.Common.WebControls.Validators.BaseValidator.js");
            template.Replace("@@CLIENTID@@", ClientID);
            template.Replace("@@INPUT_CLIENTID@@", InputControl.ClientID);
            template.Replace("@@INPUT_NORMAL_BKCOLOR@@", ColorTranslator.ToHtml(InputNormalColor));
            template.Replace("@@INPUT_INVALID_BKCOLOR@@", ColorTranslator.ToHtml(InvalidInputColor));
            template.Replace("@@INPUT_NORMAL_BORDERCOLOR@@", ColorTranslator.ToHtml(InputNormalBorderColor));
            template.Replace("@@INPUT_INVALID_BORDERCOLOR@@", ColorTranslator.ToHtml(InvalidInputBorderColor));
            template.Replace("@@INPUT_NORMAL_CSS@@", InputNormalCSS);
            template.Replace("@@INPUT_INVALID_CSS@@", InvalidInputCSS);
            template.Replace("@@CLIENT_EVALUATION_CLASS@@", ClientSideOnValidateFunctionName);
            template.Replace("@@IGNORE_EMPTY_VALUE@@", IgnoreEmptyValue ? "true" : "false");

            Page.ClientScript.RegisterClientScriptBlock(GetType(), "BaseValidationScripts", template.Script, true);

            template =
                new ScriptTemplate(this,
                                   "ClearCanvas.ImageServer.Web.Common.WebControls.Validators.BaseValidator_OnClientValidation.js");

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientSideOnValidateFunctionName, template.Script,
                                                        true);
        }


        protected override bool EvaluateIsValid()
        {
            if (Enabled || ValidateWhenDisabled)
            {
                if (ConditionalCheckBox != null)
                {
                    bool skip = false;

                    if (ConditionalCheckBox is RadioButton)
                    {
                        bool isChecked = (ConditionalCheckBox as RadioButton).Checked;
                        skip = (ValidateWhenUnchecked && isChecked) || (!ValidateWhenUnchecked && !isChecked);
                    }
                    else if (ConditionalCheckBox is CheckBox)
                    {
                        bool isChecked = (ConditionalCheckBox as CheckBox).Checked;
                        skip = (ValidateWhenUnchecked && isChecked) || (!ValidateWhenUnchecked && !isChecked);
                    }

                    if (skip)
                        return true;
                }

                string value = GetControlValidationValue(ControlToValidate);

                if (String.IsNullOrEmpty(value) && IgnoreEmptyValue)
                {
                    return true;
                }

                if (OnServerSideEvaluate())
                {
                    if (!string.IsNullOrEmpty(InvalidInputCSS)) 
                        InputControl.RemoveCssClass(InvalidInputCSS);

                    if (InvalidInputIndicator != null)
                    {
                        if (!InvalidInputIndicator.IsVisible)
                            InvalidInputIndicator.Hide();
                    }

                    return true;
                }

                if (string.IsNullOrEmpty(InvalidInputCSS))
                {
                    if (InputControl.BackColor == InputNormalColor)
                        InputControl.BackColor = InvalidInputColor;
                    if (InputControl.BorderColor == InputNormalBorderColor)
                        InputControl.BorderColor = InvalidInputBorderColor;
                }
                else
                {
                    InputControl.AddCssClass(InvalidInputCSS);
                }

                if (InvalidInputIndicator != null)
                {
                    if (String.IsNullOrEmpty(ErrorMessage))
                        InvalidInputIndicator.TooltipLabel.Text = Text;
                    else
                        InvalidInputIndicator.TooltipLabel.Text = ErrorMessage;
                    InvalidInputIndicator.Show();
                }

                string errorMessage = "No error message provided. Stack Trace -\r\n" + Environment.StackTrace;
                if (!String.IsNullOrEmpty(ErrorMessage)) errorMessage = ErrorMessage;
                else if (!string.IsNullOrEmpty(Text)) errorMessage = Text;
                Platform.Log(LogLevel.Warn, "Control {0} failed server side validation. Error Message: {1}",
                             ControlToValidate, errorMessage);
                return false;
            }

            return true;
        }

        #endregion Protected methods

        #region Abstract methods

        /// <summary>
        /// Method called while server-side validation is taking place.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// <para>Derived classes must provide its own server-side validation which must return a boolean
        /// indicating the validation passes or fails. The base class will take care of other artifacts
        /// such as turning on/off <see cref="InvalidInputIndicator"/> or highlighting the input. 
        /// </para>
        /// <para>
        /// Client-side validation is specified in <see cref="RegisterClientSideValidationExtensionScripts"/>.
        /// </para> 
        /// </remarks>
        protected abstract bool OnServerSideEvaluate();

        /// <summary>
        /// Method called during initialization to register client-side validation script.
        /// </summary>
        /// <remarks>
        /// <para>Derived classes must provide its own client-side validation extension script 
        /// such as turning on/off <see cref="InvalidInputIndicator"/> or highlighting the input. 
        /// </para>
        /// <para>
        /// Server-side validation is specified in <see cref="OnServerSideEvaluate"/>.
        /// </para>
        /// </remarks>
        protected abstract void RegisterClientSideValidationExtensionScripts();

        #endregion Abstract methods
    }
}
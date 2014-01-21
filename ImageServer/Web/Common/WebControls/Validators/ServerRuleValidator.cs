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
using System.Xml;
using ClearCanvas.Dicom.Utilities.Rules;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.Validators
{
    /// <summary>
    /// Validator for Server Rules.  Note that this control only works with client side validation.
    /// It will not work properly with just server side validation.
    /// </summary>
    public class ServerRuleValidator : WebServiceValidator
    {
        public string RuleTypeControl
        {
            get
            {
                return ViewState["RULE_TYPE"].ToString();
            } 
            set
            {
                ViewState["RULE_TYPE"] = value;
            }
        }

        protected override bool OnServerSideEvaluate()
        {
            String ruleXml = GetControlValidationValue(ControlToValidate);

            if (String.IsNullOrEmpty(ruleXml))
            {
                ErrorMessage = ValidationErrors.ServerRuleXMLIsMissing;
                return false;
            }

            if (RuleTypeControl.Equals(ServerRuleTypeEnum.DataAccess.Lookup))
            {
                // Validated DataAccess rules only have the condition.  Make a fake 
                // rule that includes a non-op action
                ruleXml = String.Format("<rule>{0}<action><no-op/></action></rule>", ruleXml);
            }

            var theDoc = new XmlDocument();

            try
            {
                theDoc.LoadXml(ruleXml);
            }
            catch (Exception e)
            {
                ErrorMessage = String.Format(ValidationErrors.UnableToParseServerRuleXML, e.Message);
                return false;
            }

            string error;
                if (false == Rule<ServerActionContext>.ValidateRule(
					theDoc,
					ServerRulesEngine.GetSpecificationCompiler(),
					ServerRulesEngine.GetActionCompiler(ServerRuleTypeEnum.GetEnum(RuleTypeControl)),
					out error))
                {
                    ErrorMessage = error;
                    return false;
                }

            return true;
        }
    }
}
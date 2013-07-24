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

using System.Xml;
using System.Xml.Schema;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Dicom.Utilities.Rules;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.GrantAccessAction
{
	[ExtensionOf(typeof(ServerRuleActionCompilerOperatorExtensionPoint))]
	[ActionApplicability(ApplicableRuleType.DataAccess)]
	public class GrantAccessActionOperator : ActionOperatorCompilerBase, IXmlActionCompilerOperator<ServerActionContext>
    {
        public GrantAccessActionOperator()
            : base("grant-access")
        {
        }

        #region IXmlActionCompilerOperator<ServerActionContext> Members

        public IActionItem<ServerActionContext> Compile(XmlElement xmlNode)
        {
            if (xmlNode.Attributes["authorityGroupOid"] == null)
                throw new XmlActionCompilerException("Unexpected missing authorityGroupOid attribute for grant-access action");

            string authorityGroup = xmlNode.Attributes["authorityGroupOid"].Value;

            return new GrantAccessActionItem(authorityGroup);
        }

		public XmlSchemaElement GetSchema()
		{
            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaAttribute attrib = new XmlSchemaAttribute
                                            {
                                                Name = "authorityGroupOid",
                                                Use = XmlSchemaUse.Required,
                                                SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
                                            };
		    type.Attributes.Add(attrib);

            XmlSchemaElement element = new XmlSchemaElement
                                           {
                                               Name = "grant-access", 
                                               SchemaType = type
                                           };
		    return element;
        }

        #endregion
    }
}
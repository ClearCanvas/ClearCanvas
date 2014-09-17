#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using System.Xml;
using System.Xml.Schema;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Dicom.Utilities.Rules;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.StudyAutoRouteAction
{

	[ExtensionOf(typeof (ServerRuleActionCompilerOperatorExtensionPoint))]
	[ActionApplicability(ApplicableRuleType.StudyAutoRoute)]
	public class StudyAutoRouteActionOperator : ActionOperatorCompilerBase, IXmlActionCompilerOperator<ServerActionContext>
	{
		public StudyAutoRouteActionOperator()
			: base("study-auto-route")
		{
		}

		#region IXmlActionCompilerOperator<ServerActionContext> Members

		public IActionItem<ServerActionContext> Compile(XmlElement xmlNode)
		{
			if (xmlNode.Attributes["device"] == null)
				throw new XmlActionCompilerException("Unexpected missing device attribute for auto-route action");

			string device = xmlNode.Attributes["device"].Value;
			double? delayInMinutes = null;
			QCStatusEnum qcEnum = null;

			if (xmlNode.Attributes["delayInMinutes"] != null)
			{
				string delayString = xmlNode.Attributes["delayInMinutes"].Value;
				delayInMinutes = Double.Parse(delayString);
			}
			if (xmlNode.Attributes["qcStatus"] != null)
			{
				string qcStatusString = xmlNode.Attributes["qcStatus"].Value;
				if (QCStatusEnum.Failed.Description.Equals(qcStatusString))
					qcEnum = QCStatusEnum.Failed;
				else if (QCStatusEnum.Incomplete.Description.Equals(qcStatusString))
					qcEnum = QCStatusEnum.Incomplete;
				else if (QCStatusEnum.NA.Description.Equals(qcStatusString))
					qcEnum = QCStatusEnum.NA;
				else if (QCStatusEnum.Passed.Description.Equals(qcStatusString))
					qcEnum = QCStatusEnum.Passed;
			}

			if ((xmlNode.Attributes["startTime"] != null)
			    && (xmlNode.Attributes["endTime"] != null))
			{
				DateTime startTime;
				if (!DateTime.TryParseExact(xmlNode.Attributes["startTime"].Value, "HH:mm:ss",
				                            CultureInfo.InvariantCulture, DateTimeStyles.None,
				                            out startTime))
				{
					throw new XmlActionCompilerException("Incorrect format of startTime: " + xmlNode.Attributes["startTime"].Value);
				}

				DateTime endTime;
				if (!DateTime.TryParseExact(xmlNode.Attributes["endTime"].Value, "HH:mm:ss",
				                            CultureInfo.InvariantCulture, DateTimeStyles.None,
				                            out endTime))
				{
					throw new XmlActionCompilerException("Incorrect format of endTime: " + xmlNode.Attributes["endTime"].Value);
				}

				return new StudyAutoRouteActionItem(device, startTime, endTime, qcEnum);
			}
			if ((xmlNode.Attributes["startTime"] == null)
			    && (xmlNode.Attributes["endTime"] != null))
				throw new XmlActionCompilerException("Unexpected missing startTime attribute for study-auto-route action");
			if ((xmlNode.Attributes["startTime"] != null)
			    && (xmlNode.Attributes["endTime"] == null))
				throw new XmlActionCompilerException("Unexpected missing endTime attribute for study-auto-route action");

			if (delayInMinutes != null)
				return new StudyAutoRouteActionItem(device, Platform.Time.AddMinutes(delayInMinutes.Value), qcEnum);

			return new StudyAutoRouteActionItem(device, qcEnum);
		}

		public XmlSchemaElement GetSchema()
		{
			var type = new XmlSchemaComplexType();

			var attrib = new XmlSchemaAttribute
				{
					Name = "device",
					Use = XmlSchemaUse.Required,
					SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
				};
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute
				{
					Name = "startTime",
					Use = XmlSchemaUse.Optional,
					SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
				};
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute
				{
					Name = "endTime",
					Use = XmlSchemaUse.Optional,
					SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
				};
			type.Attributes.Add(attrib);

			var restriction = new XmlSchemaSimpleTypeRestriction
				{
					BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
				};

			restriction.Facets.Add(new XmlSchemaEnumerationFacet
				{
					Value = QCStatusEnum.Passed.Description
				});
			restriction.Facets.Add(new XmlSchemaEnumerationFacet
				{
					Value = QCStatusEnum.Failed.Description
				});
			restriction.Facets.Add(new XmlSchemaEnumerationFacet
				{
					Value = QCStatusEnum.Incomplete.Description
				});
			restriction.Facets.Add(new XmlSchemaEnumerationFacet
				{
					Value = QCStatusEnum.NA.Description
				});
			var qcStatusType = new XmlSchemaSimpleType
				{
					Content = restriction
				};

			attrib = new XmlSchemaAttribute
				{
					Name = "qcStatus",
					Use = XmlSchemaUse.Optional,
					SchemaType = qcStatusType
				};
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute
				{
					Name = "delayInMinutes",
					Use = XmlSchemaUse.Optional,
					SchemaTypeName = new XmlQualifiedName("double", "http://www.w3.org/2001/XMLSchema")
				};
			type.Attributes.Add(attrib);

			var element = new XmlSchemaElement
				{
					Name = "study-auto-route",
					SchemaType = type
				};

			return element;
		}

		#endregion
	}
}

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
using System.Xml.Schema;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Dicom.Utilities.Rules;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.JpegCodec.JpegLosslessAction
{
	[ExtensionOf(typeof(ServerRuleActionCompilerOperatorExtensionPoint))]
	[ActionApplicability(ApplicableRuleType.StudyCompress)]
	public class JpegLosslessActionOperator : ActionOperatorCompilerBase, IXmlActionCompilerOperator<ServerActionContext>
	{
		public JpegLosslessActionOperator()
			: base("jpeg-lossless")
		{
		}

		public IActionItem<ServerActionContext> Compile(XmlElement xmlNode)
		{
			if (xmlNode.Attributes["time"] == null)
				throw new XmlActionCompilerException(
					"Unexpected missing time attribute for jpeg-lossless scheduling action");
			if (xmlNode.Attributes["unit"] == null)
				throw new XmlActionCompilerException(
					"Unexpected missing unit attribute for jpeg-lossless scheduling action");

			int time;
			if (false == int.TryParse(xmlNode.Attributes["time"].Value, out time))
				throw new XmlActionCompilerException("Unable to parse time value for jpeg-lossless scheduling rule");

			string xmlUnit = xmlNode.Attributes["unit"].Value;

			// this will throw exception if the unit is not defined
			TimeUnit unit = (TimeUnit) Enum.Parse(typeof (TimeUnit), xmlUnit, true);

			bool convertFromPalette = false;
			if (xmlNode.Attributes["convertFromPalette"] != null)
			{
				if (false == bool.TryParse(xmlNode.Attributes["convertFromPalette"].Value, out convertFromPalette))
					throw new XmlActionCompilerException("Unable to parse convertFromPalette value for jpeg-lossless scheduling rule");
			}

			string refValue = xmlNode.Attributes["refValue"] != null ? xmlNode.Attributes["refValue"].Value : null;


			if (!String.IsNullOrEmpty(refValue))
			{
				if (xmlNode["expressionLanguage"] != null)
				{
					string language = xmlNode["expressionLanguage"].Value;
					Expression scheduledTime = CreateExpression(refValue, language);
					return new JpegLosslessActionItem(time, unit, scheduledTime, convertFromPalette);
				}
				else
				{
					Expression scheduledTime = CreateExpression(refValue);
					return new JpegLosslessActionItem(time, unit, scheduledTime, convertFromPalette);
				}
			}

			return new JpegLosslessActionItem(time, unit, convertFromPalette);
		}

		public XmlSchemaElement GetSchema()
		{
			XmlSchemaElement element = GetTimeSchema(OperatorTag);

			XmlSchemaAttribute attrib = new XmlSchemaAttribute
			                            	{
			                            		Name = "convertFromPalette",
			                            		Use = XmlSchemaUse.Optional,
			                            		SchemaTypeName = new XmlQualifiedName("boolean", "http://www.w3.org/2001/XMLSchema")
			                            	};
			(element.SchemaType as XmlSchemaComplexType).Attributes.Add(attrib);

			return element;
		}
	}
}
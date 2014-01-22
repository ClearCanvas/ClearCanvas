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
using System.Xml;
using System.Xml.Schema;
using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Dicom.Utilities.Rules.Specifications
{
	[ExtensionOf(typeof(DicomRuleSpecificationCompilerOperatorExtensionPoint))]
    public class DicomAgeLessThanSpecificationOperator : IXmlSpecificationCompilerOperator
    {
        #region IXmlSpecificationCompilerOperator Members

        public string OperatorTag
        {
            get { return "dicom-age-less-than"; }
        }

        public Specification Compile(XmlElement xmlNode, IXmlSpecificationCompilerContext context)
        {
            string units = xmlNode.GetAttribute("units").ToLower();
            if (units == null)
                throw new XmlSpecificationCompilerException("Xml attribute 'units' is required.");

            if (!units.Equals("years")
                && !units.Equals("weeks")
                && !units.Equals("days"))
                throw new XmlSpecificationCompilerException(
                    "Incorrect value for 'units' Xml attribute.  Should be 'years', 'weeks', or 'days'");

            string refValue = GetAttributeOrNull(xmlNode, "refValue");
            if (refValue == null)
                throw new XmlSpecificationCompilerException("Xml attribute 'refValue' is required.");


            return new DicomAgeLessThanSpecification(units, refValue);
        }

        public XmlSchemaElement GetSchema()
        {
            var type = new XmlSchemaComplexType();

            type.Attributes.Add(new XmlSchemaAttribute
                                    {
                                        Name = "refValue",
                                        Use = XmlSchemaUse.Required,
                                        SchemaTypeName =
                                            new XmlQualifiedName("positiveInteger", "http://www.w3.org/2001/XMLSchema")
                                    });

            type.Attributes.Add(new XmlSchemaAttribute
                                    {
                                        Name = "test",
                                        Use = XmlSchemaUse.Optional,
                                        SchemaTypeName =
                                            new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
                                    });

            
            var restriction = new XmlSchemaSimpleTypeRestriction
                                  {
                                      BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
                                  };

            restriction.Facets.Add(new XmlSchemaEnumerationFacet {Value = "years"});
            restriction.Facets.Add(new XmlSchemaEnumerationFacet {Value = "weeks"});
            restriction.Facets.Add(new XmlSchemaEnumerationFacet {Value = "days"});

            var simpleType = new XmlSchemaSimpleType {Content = restriction};


            type.Attributes.Add(new XmlSchemaAttribute
                                    {
                                        Name = "units",
                                        Use = XmlSchemaUse.Required,
                                        SchemaType = simpleType
                                    });


            type.Attributes.Add(new XmlSchemaAttribute
                                    {
                                        Name = "expressionLanguage",
                                        Use = XmlSchemaUse.Optional,
                                        SchemaTypeName =
                                            new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
                                    });

            var element = new XmlSchemaElement
                              {
                                  Name = "dicom-age-less-than",
                                  SchemaType = type
                              };

            return element;
        }

        #endregion

        private string GetAttributeOrNull(XmlElement node, string attr)
        {
            string val = node.GetAttribute(attr);
            return string.IsNullOrEmpty(val) ? null : val;
        }
    }

    public class DicomAgeLessThanSpecification : PrimitiveSpecification
    {
        private readonly string _refValue;
        private readonly string _units;

        public DicomAgeLessThanSpecification(string units, string refValue)
        {
            _refValue = refValue;
            _units = units.ToLower();
        }

        protected override TestResult InnerTest(object exp, object root)
        {
            // assume that null doesn't match
            if (exp == null || root == null)
                return DefaultTestResult(false);

            if (exp is string)
            {
                if ((exp as string).Length == 0)
                    return DefaultTestResult(false);

                DateTime comparisonTime = Platform.Time;
                double time;
                if (false == double.TryParse(_refValue, NumberStyles.Float, CultureInfo.InvariantCulture, out time))
                    throw new SpecificationException(Common.SR.ExceptionCastExpressionString);

                if (_units.Equals("weeks"))
                    comparisonTime = comparisonTime.AddDays(time*-7f);
                else if (_units.Equals("days"))
                    comparisonTime = comparisonTime.AddDays(-1*time);
                else
                    comparisonTime = comparisonTime.AddYears((int) (-1*time));

                DateTime? testTime = DateTimeParser.Parse(exp as string);

                return DefaultTestResult(comparisonTime < testTime);
            }
            throw new SpecificationException(Common.SR.ExceptionCastExpressionString);
        }
    }
}
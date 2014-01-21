using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace ClearCanvas.Common.Specifications
{
	class XmlSpecificationSchema
	{
		public static void ValidateSpecification(XmlElement containingNode, XmlSchema schema)
		{
			// We must parse the XML to get the schema validation to work.  So, we write
			// the xml out to a string, and read it back in with Schema Validation enabled
			var sw = new StringWriter();

			var xmlWriterSettings = new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				ConformanceLevel = ConformanceLevel.Fragment,
				Indent = false,
				NewLineOnAttributes = false,
				IndentChars = ""
			};

			var xmlWriter = XmlWriter.Create(sw, xmlWriterSettings);
			foreach (XmlNode node in containingNode.ChildNodes)
				node.WriteTo(xmlWriter);
			xmlWriter.Close();

			var xmlReaderSettings = new XmlReaderSettings
			{
				ValidationType = ValidationType.Schema,
				ConformanceLevel = ConformanceLevel.Fragment,
				Schemas = new XmlSchemaSet()
			};
			xmlReaderSettings.Schemas.Add(schema);

			var xmlReader = XmlTextReader.Create(new StringReader(sw.ToString()), xmlReaderSettings);
			while (xmlReader.Read()) ;
			xmlReader.Close();
		}

		public static XmlSchema CompileSchema(IEnumerable<IXmlSpecificationCompilerOperator> operators)
		{
			var baseSchema = new XmlSchema();
			foreach (var schema in operators.Select(op => op.GetSchema()))
			{
				baseSchema.Items.Add(schema);
			}

			var set = new XmlSchemaSet();
			set.Add(baseSchema);
			set.Compile();

			XmlSchema compiledSchema = null;
			foreach (XmlSchema schema in set.Schemas())
			{
				compiledSchema = schema;
			}

			return compiledSchema;
		}

		public static XmlSchemaElement AndSchema()
		{
			var type = new XmlSchemaComplexType();

			var any = new XmlSchemaAny();
			any.MinOccurs = 1;
			any.MaxOccursString = "unbounded";
			any.ProcessContents = XmlSchemaContentProcessing.Strict;
			any.Namespace = "##local";

			var sequence = new XmlSchemaSequence();
			type.Particle = sequence;
			sequence.Items.Add(any);

			var attrib = new XmlSchemaAttribute();
			attrib.Name = "expressionLanguage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "failMessage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			var element = new XmlSchemaElement();
			element.Name = "and";
			element.SchemaType = type;

			return element;
		}

		public static XmlSchemaElement OrSchema()
		{
			var type = new XmlSchemaComplexType();

			var any = new XmlSchemaAny();
			any.MinOccurs = 1;
			any.MaxOccursString = "unbounded";
			any.ProcessContents = XmlSchemaContentProcessing.Strict;
			any.Namespace = "##local";

			var sequence = new XmlSchemaSequence();
			type.Particle = sequence;
			sequence.Items.Add(any);

			var attrib = new XmlSchemaAttribute();
			attrib.Name = "expressionLanguage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "failMessage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);


			var element = new XmlSchemaElement();
			element.Name = "or";
			element.SchemaType = type;

			return element;
		}

		public static XmlSchemaElement NotSchema()
		{
			var type = new XmlSchemaComplexType();

			var any = new XmlSchemaAny();
			any.MinOccurs = 1;
			any.MaxOccursString = "unbounded";
			any.ProcessContents = XmlSchemaContentProcessing.Strict;
			any.Namespace = "##local";

			var sequence = new XmlSchemaSequence();
			type.Particle = sequence;
			sequence.Items.Add(any);

			var attrib = new XmlSchemaAttribute();
			attrib.Name = "expressionLanguage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "failMessage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			var element = new XmlSchemaElement();
			element.Name = "not";
			element.SchemaType = type;

			return element;
		}

		public static XmlSchemaElement RegexStringMatchingSchema()
		{
			return StringMatchingSchema("regex");
		}

		public static XmlSchemaElement StartsWithStringMatchingSchema()
		{
			return StringMatchingSchema("starts-with");
		}

		public static XmlSchemaElement EndsWithStringMatchingSchema()
		{
			return StringMatchingSchema("ends-with");
		}

		public static XmlSchemaElement ContainsStringMatchingSchema()
		{
			return StringMatchingSchema("contains");
		}

		public static XmlSchemaElement StringMatchingSchema(string elementName)
		{
			var type = new XmlSchemaComplexType();

			var attrib = new XmlSchemaAttribute();
			attrib.Name = "test";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "ignoreCase";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("boolean", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "nullMatches";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("boolean", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "pattern";
			attrib.Use = XmlSchemaUse.Required;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "expressionLanguage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "failMessage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			var element = new XmlSchemaElement();
			element.Name = elementName;
			element.SchemaType = type;

			return element;
		}

		public static XmlSchemaElement NotNullSchema()
		{
			var type = new XmlSchemaComplexType();

			var attrib = new XmlSchemaAttribute();
			attrib.Name = "test";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "expressionLanguage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "failMessage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			var element = new XmlSchemaElement();
			element.Name = "not-null";
			element.SchemaType = type;

			return element;
		}

		public static XmlSchemaElement IsNullSchema()
		{
			var type = new XmlSchemaComplexType();

			var attrib = new XmlSchemaAttribute();
			attrib.Name = "test";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "expressionLanguage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "failMessage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			var element = new XmlSchemaElement();
			element.Name = "null";
			element.SchemaType = type;

			return element;
		}

		public static XmlSchemaElement CountSchema()
		{
			var type = new XmlSchemaComplexType();

			var attrib = new XmlSchemaAttribute();
			attrib.Name = "min";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("positiveInteger", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "max";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("positiveInteger", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "expressionLanguage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "failMessage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			var any = new XmlSchemaAny();
			any.MinOccurs = 1;
			any.MaxOccursString = "unbounded";
			any.ProcessContents = XmlSchemaContentProcessing.Strict;
			any.Namespace = "##local";

			var sequence = new XmlSchemaSequence();
			type.Particle = sequence;
			sequence.Items.Add(any);

			var element = new XmlSchemaElement();
			element.Name = "count";
			element.SchemaType = type;

			return element;
		}

		public static XmlSchemaElement EachSchema()
		{
			var type = new XmlSchemaComplexType();

			var attrib = new XmlSchemaAttribute();
			attrib.Name = "expressionLanguage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "failMessage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			var any = new XmlSchemaAny();
			any.MinOccurs = 1;
			any.MaxOccursString = "unbounded";
			any.ProcessContents = XmlSchemaContentProcessing.Strict;
			any.Namespace = "##local";

			var sequence = new XmlSchemaSequence();
			type.Particle = sequence;
			sequence.Items.Add(any);

			var element = new XmlSchemaElement();
			element.Name = "each";
			element.SchemaType = type;

			return element;
		}

		public static XmlSchemaElement AllSchema()
		{
			var type = new XmlSchemaComplexType();

			var attrib = new XmlSchemaAttribute();
			attrib.Name = "expressionLanguage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "failMessage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			var any = new XmlSchemaAny();
			any.MinOccurs = 1;
			any.MaxOccursString = "unbounded";
			any.ProcessContents = XmlSchemaContentProcessing.Strict;
			any.Namespace = "##local";

			var sequence = new XmlSchemaSequence();
			type.Particle = sequence;
			sequence.Items.Add(any);

			var element = new XmlSchemaElement();
			element.Name = "all";
			element.SchemaType = type;

			return element;
		}

		public static XmlSchemaElement AnySchema()
		{
			var type = new XmlSchemaComplexType();

			var attrib = new XmlSchemaAttribute();
			attrib.Name = "expressionLanguage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "failMessage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			var any = new XmlSchemaAny();
			any.MinOccurs = 1;
			any.MaxOccursString = "unbounded";
			any.ProcessContents = XmlSchemaContentProcessing.Strict;
			any.Namespace = "##local";

			var sequence = new XmlSchemaSequence();
			type.Particle = sequence;
			sequence.Items.Add(any);

			var element = new XmlSchemaElement();
			element.Name = "any";
			element.SchemaType = type;

			return element;
		}

		public static XmlSchemaElement CaseSchema()
		{
			var type = new XmlSchemaComplexType();
			XmlSchemaAll allSchema = new XmlSchemaAll();
			type.Particle = allSchema;

			//When
			XmlSchemaComplexType subType = new XmlSchemaComplexType();
			var any = new XmlSchemaAny();
			any.MinOccurs = 1;
			any.MaxOccursString = "unbounded";
			any.ProcessContents = XmlSchemaContentProcessing.Strict;
			any.Namespace = "##local";

			var sequence = new XmlSchemaSequence();
			subType.Particle = sequence;
			sequence.Items.Add(any);

			XmlSchemaElement subElement = new XmlSchemaElement();
			subElement.Name = "when";
			subElement.SchemaType = subType;
			allSchema.Items.Add(subElement);

			//Then
			subType = new XmlSchemaComplexType();
			any = new XmlSchemaAny();
			any.MinOccurs = 1;
			any.MaxOccursString = "unbounded";
			any.ProcessContents = XmlSchemaContentProcessing.Strict;
			any.Namespace = "##local";

			sequence = new XmlSchemaSequence();
			subType.Particle = sequence;
			sequence.Items.Add(any);

			subElement = new XmlSchemaElement();
			subElement.Name = "then";
			subElement.SchemaType = subType;
			allSchema.Items.Add(subElement);

			//Else
			subType = new XmlSchemaComplexType();
			any = new XmlSchemaAny();
			any.MinOccurs = 1;
			any.MaxOccurs = 1;
			any.ProcessContents = XmlSchemaContentProcessing.Strict;
			any.Namespace = "##local";

			sequence = new XmlSchemaSequence();
			subType.Particle = sequence;
			sequence.Items.Add(any);

			subElement = new XmlSchemaElement();
			subElement.Name = "else";
			subElement.SchemaType = subType;
			allSchema.Items.Add(subElement);

			var attrib = new XmlSchemaAttribute();
			attrib.Name = "expressionLanguage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "failMessage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			var element = new XmlSchemaElement();
			element.Name = "case";
			element.SchemaType = type;

			return element;
		}

		public static XmlSchemaElement TrueSchema()
		{
			var type = new XmlSchemaComplexType();

			var attrib = new XmlSchemaAttribute();
			attrib.Name = "test";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "expressionLanguage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "failMessage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			var element = new XmlSchemaElement();
			element.Name = "true";
			element.SchemaType = type;

			return element;
		}

		public static XmlSchemaElement FalseSchema()
		{
			var type = new XmlSchemaComplexType();

			var attrib = new XmlSchemaAttribute();
			attrib.Name = "test";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "expressionLanguage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "failMessage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			var element = new XmlSchemaElement();
			element.Name = "false";
			element.SchemaType = type;

			return element;
		}

		public static XmlSchemaElement EqualSchema()
		{
			var type = new XmlSchemaComplexType();

			var attrib = new XmlSchemaAttribute();
			attrib.Name = "refValue";
			attrib.Use = XmlSchemaUse.Required;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "test";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "expressionLanguage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "failMessage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			var element = new XmlSchemaElement();
			element.Name = "equal";
			element.SchemaType = type;

			return element;
		}


		public static XmlSchemaElement NotEqualSchema()
		{
			var type = new XmlSchemaComplexType();

			var attrib = new XmlSchemaAttribute();
			attrib.Name = "refValue";
			attrib.Use = XmlSchemaUse.Required;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "test";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "expressionLanguage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "failMessage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			var element = new XmlSchemaElement();
			element.Name = "not-equal";
			element.SchemaType = type;

			return element;
		}

		public static XmlSchemaElement GreaterThanSchema()
		{
			var type = new XmlSchemaComplexType();

			var attrib = new XmlSchemaAttribute();
			attrib.Name = "refValue";
			attrib.Use = XmlSchemaUse.Required;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "expressionLanguage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "test";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "failMessage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "inclusive";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("boolean", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			var element = new XmlSchemaElement();
			element.Name = "greater-than";
			element.SchemaType = type;

			return element;
		}

		public static XmlSchemaElement LessThanSchema()
		{
			var type = new XmlSchemaComplexType();

			var attrib = new XmlSchemaAttribute();
			attrib.Name = "refValue";
			attrib.Use = XmlSchemaUse.Required;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "expressionLanguage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "test";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "failMessage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "inclusive";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("boolean", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			var element = new XmlSchemaElement();
			element.Name = "less-than";
			element.SchemaType = type;

			return element;
		}

		public static XmlSchemaElement DefinedSchema()
		{
			var type = new XmlSchemaComplexType();

			var attrib = new XmlSchemaAttribute();
			attrib.Name = "spec";
			attrib.Use = XmlSchemaUse.Required;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "expressionLanguage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "failMessage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			var element = new XmlSchemaElement();
			element.Name = "defined";
			element.SchemaType = type;

			return element;
		}
	}
}

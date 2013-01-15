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

#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using ClearCanvas.Common.Scripting;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Specifications
{
    [ExtensionPoint]
    public sealed class ExpressionFactoryExtensionPoint : ExtensionPoint<IExpressionFactory>
    {
        public IExpressionFactory CreateExtension(string language)
        {
            return (IExpressionFactory) CreateExtension(new AttributeExtensionFilter(new LanguageSupportAttribute(language)));
        }
    }

    public interface IXmlSpecificationCompilerContext
    {
        IExpressionFactory DefaultExpressionFactory { get; }
        IExpressionFactory GetExpressionFactory(string language);
        ISpecification Compile(XmlElement containingNode);
        ISpecification GetSpecification(string id);
    }

    [ExtensionPoint]
    public sealed class XmlSpecificationCompilerOperatorExtensionPoint : ExtensionPoint<IXmlSpecificationCompilerOperator>
    {
    }


    public class XmlSpecificationCompiler
    {
        #region IXmlSpecificationCompilerContext implementation class

        class Context : IXmlSpecificationCompilerContext
        {
            private readonly XmlSpecificationCompiler _compiler;

            public Context(XmlSpecificationCompiler compiler)
            {
                _compiler = compiler;
            }

            #region IXmlSpecificationCompilerContext Members

            public IExpressionFactory DefaultExpressionFactory
            {
                get { return _compiler._defaultExpressionFactory; }
            }

            public IExpressionFactory GetExpressionFactory(string language)
            {
                return CreateExpressionFactory(language);
            }

            public ISpecification Compile(XmlElement containingNode)
            {
                return _compiler.Compile(containingNode, false);
            }

            public ISpecification GetSpecification(string id)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
        }

        #endregion

        delegate Specification CreationDelegate(XmlElement xmlNode);

        private delegate XmlSchemaElement SchemaDelegate();

        #region BuiltInOperator class

        class BuiltInOperator : IXmlSpecificationCompilerOperator
        {
            private readonly string _operator;
            private readonly CreationDelegate _factoryMethod;
            private readonly SchemaDelegate _schemaMethod;

            public BuiltInOperator(string op, CreationDelegate factoryMethod, SchemaDelegate schemaMethod )
            {
                _operator = op;
                _factoryMethod = factoryMethod;
                _schemaMethod = schemaMethod;
            }

            #region IXmlSpecificationCompilerOperator Members

            public string OperatorTag
            {
                get { return _operator; }
            }

            public Specification Compile(XmlElement xmlNode, IXmlSpecificationCompilerContext context)
            {
                return _factoryMethod(xmlNode);
            }

            public XmlSchemaElement GetSchema()
            {
                return _schemaMethod();
            }

            #endregion
        }

        #endregion

        private readonly Dictionary<string, IXmlSpecificationCompilerOperator> _operatorMap = new Dictionary<string, IXmlSpecificationCompilerOperator>();
        private readonly ISpecificationProvider _resolver;
        private readonly IExpressionFactory _defaultExpressionFactory;
        private readonly IXmlSpecificationCompilerContext _compilerContext;
        private readonly XmlSchema _schema = new XmlSchema();

        #region Constructors
        public XmlSpecificationCompiler(ISpecificationProvider resolver, IExpressionFactory defaultExpressionFactory)
        {
            _resolver = resolver;

            _defaultExpressionFactory = defaultExpressionFactory;
            _compilerContext = new Context(this);

            // declare built-in operators
            AddOperator(new BuiltInOperator("true", CreateTrue, TrueSchema));
            AddOperator(new BuiltInOperator("false", CreateFalse, FalseSchema));
            AddOperator(new BuiltInOperator("equal", CreateEqual, EqualSchema));
            AddOperator(new BuiltInOperator("not-equal", CreateNotEqual, NotEqualSchema));
            AddOperator(new BuiltInOperator("greater-than", CreateGreaterThan, GreaterThanSchema));
            AddOperator(new BuiltInOperator("less-than", CreateLessThan, LessThanSchema));
            AddOperator(new BuiltInOperator("and", CreateAnd, AndSchema));
            AddOperator(new BuiltInOperator("or", CreateOr, OrSchema));
			AddOperator(new BuiltInOperator("not", CreateNot, NotSchema));
			AddOperator(new BuiltInOperator("regex", CreateRegex, RegexSchema));
            AddOperator(new BuiltInOperator("null", CreateIsNull, IsNullSchema));
            AddOperator(new BuiltInOperator("not-null", CreateNotNull, NotNullSchema));
            AddOperator(new BuiltInOperator("count", CreateCount, CountSchema));
            AddOperator(new BuiltInOperator("each", CreateEach, EachSchema));
            AddOperator(new BuiltInOperator("any", CreateAny, AnySchema));
            AddOperator(new BuiltInOperator("case", CreateCase, CaseSchema));
            AddOperator(new BuiltInOperator("defined", CreateDefined, DefinedSchema));

            // add extension operators
            XmlSpecificationCompilerOperatorExtensionPoint xp = new XmlSpecificationCompilerOperatorExtensionPoint();
            foreach (IXmlSpecificationCompilerOperator compilerOperator in xp.CreateExtensions())
            {
                AddOperator(compilerOperator);
            }

            _schema = CreateSchema();
        }

        public XmlSpecificationCompiler(ISpecificationProvider resolver, string defaultExpressionLanguage)
            : this(resolver, CreateExpressionFactory(defaultExpressionLanguage))
        {
        }


        public XmlSpecificationCompiler(IExpressionFactory defaultExpressionFactory)
            : this(null, defaultExpressionFactory)
        {
        }

        public XmlSpecificationCompiler(string defaultExpressionLanguage)
            : this(null, CreateExpressionFactory(defaultExpressionLanguage))
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// A compiled XML schema used by the compiler to verify specifications.
        /// </summary>
        public XmlSchema Schema
        {
            get { return _schema; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Compile a specification.  The XML Schema will not be checked.
        /// </summary>
        /// <param name="containingNode">The XML node to compile.</param>
        /// <returns>A compiled <see cref="ISpecification"/>.</returns>
        public ISpecification Compile(XmlElement containingNode)
        {
            return Compile(containingNode, false);
        }

        /// <summary>
        /// Compile a specification and check the schema if enabled.
        /// </summary>
        /// <param name="containingNode">The XML node to compile</param>
        /// <param name="checkSchema">Flag to determine if the schema will be checked.</param>
        /// <returns>A compiled <see cref="ISpecification"/>.</returns>
        public ISpecification Compile(XmlElement containingNode, bool checkSchema)
        {
            // Note, recursive calls are made to this method to compile.  The schema is not
            // checked on recursive calls, but should be checked once on an initial compile.
            if (checkSchema)
            {
                // We must parse the XML to get the schema validation to work.  So, we write
                // the xml out to a string, and read it back in with Schema Validation enabled
                StringWriter sw = new StringWriter();

                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = Encoding.UTF8;
                xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
                xmlWriterSettings.Indent = false;
                xmlWriterSettings.NewLineOnAttributes = false;
                xmlWriterSettings.IndentChars = "";

                XmlWriter xmlWriter = XmlWriter.Create(sw, xmlWriterSettings);
                foreach (XmlNode node in containingNode.ChildNodes)
                    node.WriteTo(xmlWriter);
                xmlWriter.Close();

                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.Schemas = new XmlSchemaSet();
                xmlReaderSettings.Schemas.Add(Schema);
                xmlReaderSettings.ValidationType = ValidationType.Schema;
                xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;

                XmlReader xmlReader = XmlTextReader.Create(new StringReader(sw.ToString()), xmlReaderSettings);
                while (xmlReader.Read()) ;
                xmlReader.Close();
            }
            return CreateImplicitAnd(GetChildElements(containingNode));
        }
        #endregion

        #region Private Methods
        private XmlSchema CreateSchema()
        {
            XmlSchema baseSchema = new XmlSchema();

            foreach (IXmlSpecificationCompilerOperator op in _operatorMap.Values)
            {
                XmlSchemaElement element = op.GetSchema();
                baseSchema.Items.Add(element);
            }

            XmlSchemaSet set = new XmlSchemaSet();
            set.Add(baseSchema);
            set.Compile();

            XmlSchema compiledSchema = null;
            foreach (XmlSchema schema in set.Schemas())
            {
                compiledSchema = schema;
            }

            //StringWriter sw = new StringWriter();
            //compiledSchema.Write(sw);
            //Platform.Log(LogLevel.Info, sw);

            return compiledSchema;
        }

        private void AddOperator(IXmlSpecificationCompilerOperator op)
        {
            _operatorMap.Add(op.OperatorTag, op);
        }

        private Specification BuildNode(XmlElement node)
        {
            if (!_operatorMap.ContainsKey(node.Name))
                throw new XmlSpecificationCompilerException("Unknown Xml specification node: " + node.Name);

            IXmlSpecificationCompilerOperator op = _operatorMap[node.Name];
            Specification spec = op.Compile(node, _compilerContext);

            string test = GetAttributeOrNull(node, "test");
            if(test != null)
            {
                spec.TestExpression = CreateExpression(test, GetAttributeOrNull(node, "expressionLanguage"));
            }

            spec.FailureMessage = GetAttributeOrNull(node, "failMessage");
            return spec;
        }

        private Specification CreateAnd(XmlElement node)
        {
            AndSpecification spec = new AndSpecification();
            foreach (XmlElement child in GetChildElements(node))
            {
                spec.Add(BuildNode(child));
            }
            return spec;
        }

        private static XmlSchemaElement AndSchema()
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaAny any = new XmlSchemaAny();
            any.MinOccurs = 1;
            any.MaxOccursString = "unbounded";
            any.ProcessContents = XmlSchemaContentProcessing.Strict;
            any.Namespace = "##local";

            XmlSchemaSequence sequence = new XmlSchemaSequence();
            type.Particle = sequence;
            sequence.Items.Add(any);

            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
            attrib.Name = "expressionLanguage";
            attrib.Use = XmlSchemaUse.Optional;
            attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            type.Attributes.Add(attrib);

            attrib = new XmlSchemaAttribute();
            attrib.Name = "failMessage";
            attrib.Use = XmlSchemaUse.Optional;
            attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            type.Attributes.Add(attrib);

            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "and";
            element.SchemaType = type;
            
            return element;
        }

        private Specification CreateOr(XmlElement node)
        {
            OrSpecification spec = new OrSpecification();
            foreach (XmlElement child in GetChildElements(node))
            {
                spec.Add(BuildNode(child));
            }
            return spec;
        }

        private static XmlSchemaElement OrSchema()
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaAny any = new XmlSchemaAny();
            any.MinOccurs = 1;
            any.MaxOccursString = "unbounded";
            any.ProcessContents = XmlSchemaContentProcessing.Strict;
            any.Namespace = "##local";

            XmlSchemaSequence sequence = new XmlSchemaSequence();
            type.Particle = sequence;
            sequence.Items.Add(any);

            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
            attrib.Name = "expressionLanguage";
            attrib.Use = XmlSchemaUse.Optional;
            attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            type.Attributes.Add(attrib);

            attrib = new XmlSchemaAttribute();
            attrib.Name = "failMessage";
            attrib.Use = XmlSchemaUse.Optional;
            attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            type.Attributes.Add(attrib);


            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "or";
            element.SchemaType = type;

            return element;
        }
		private Specification CreateNot(XmlElement node)
		{
			NotSpecification spec = new NotSpecification();
			foreach (XmlElement child in GetChildElements(node))
			{
				spec.Add(BuildNode(child));
			}
			return spec;
		}

		private static XmlSchemaElement NotSchema()
		{
			XmlSchemaComplexType type = new XmlSchemaComplexType();

			XmlSchemaAny any = new XmlSchemaAny();
			any.MinOccurs = 1;
			any.MaxOccursString = "unbounded";
			any.ProcessContents = XmlSchemaContentProcessing.Strict;
			any.Namespace = "##local";

			XmlSchemaSequence sequence = new XmlSchemaSequence();
			type.Particle = sequence;
			sequence.Items.Add(any);

			XmlSchemaAttribute attrib = new XmlSchemaAttribute();
			attrib.Name = "expressionLanguage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "failMessage";
			attrib.Use = XmlSchemaUse.Optional;
			attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			XmlSchemaElement element = new XmlSchemaElement();
			element.Name = "not";
			element.SchemaType = type;

			return element;
		}

        private static Specification CreateRegex(XmlElement node)
        {
            string stringIgnoreCase = node.GetAttribute("ignoreCase");

            bool ignoreCase = !stringIgnoreCase.Equals("false", StringComparison.InvariantCultureIgnoreCase);

            string pattern = GetAttributeOrNull(node, "pattern");
            if (pattern == null)
                throw new XmlSpecificationCompilerException("Xml attribute 'pattern' is required for regex.");

        	bool nullMatches = false;
			string stringNullMatches = GetAttributeOrNull(node, "nullMatches");
			if (stringNullMatches != null)
				nullMatches = stringNullMatches.Equals("true", StringComparison.InvariantCultureIgnoreCase);

            return new RegexSpecification(pattern, ignoreCase, nullMatches);
        }

        private static XmlSchemaElement RegexSchema()
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
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

            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "regex";
            element.SchemaType = type;

            return element;
        }

        private static Specification CreateNotNull(XmlElement node)
        {
            return new NotNullSpecification();
        }

        private static XmlSchemaElement NotNullSchema()
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
            attrib.Name = "test";
            attrib.Use = XmlSchemaUse.Optional;
            attrib.SchemaTypeName = new XmlQualifiedName("string","http://www.w3.org/2001/XMLSchema"); 
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

            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "not-null";
            element.SchemaType = type;

            return element;
        }


        private static Specification CreateIsNull(XmlElement node)
        {
            return new IsNullSpecification();
        }

        private static XmlSchemaElement IsNullSchema()
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
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

            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "null";
            element.SchemaType = type;

            return element;
        }

        private Specification CreateCount(XmlElement node)
        {
            string minString = node.GetAttribute("min");
            string maxString = node.GetAttribute("max");

            int min = (minString == "") ? 0 : Int32.Parse(minString);
            int max = (maxString == "") ? Int32.MaxValue : Int32.Parse(maxString);

            ICollection<XmlNode> childElements = GetChildElements(node);
            Specification elementSpec = childElements.Count > 0 ? CreateImplicitAnd(childElements) : null;
            return new CountSpecification(min, max, elementSpec);
        }

        private static XmlSchemaElement CountSchema()
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
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

            XmlSchemaAny any = new XmlSchemaAny();
            any.MinOccurs = 1;
            any.MaxOccursString = "unbounded";
            any.ProcessContents = XmlSchemaContentProcessing.Strict;
            any.Namespace = "##local";

            XmlSchemaSequence sequence = new XmlSchemaSequence();
            type.Particle = sequence;
            sequence.Items.Add(any);

            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "count";
            element.SchemaType = type;

            return element;
        }

        private Specification CreateEach(XmlElement node)
        {
            Specification elementSpec = CreateImplicitAnd(GetChildElements(node));
            return new EachSpecification(elementSpec);
        }

        private static XmlSchemaElement EachSchema()
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
            attrib.Name = "expressionLanguage";
            attrib.Use = XmlSchemaUse.Optional;
            attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            type.Attributes.Add(attrib);

            attrib = new XmlSchemaAttribute();
            attrib.Name = "failMessage";
            attrib.Use = XmlSchemaUse.Optional;
            attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            type.Attributes.Add(attrib);

            XmlSchemaAny any = new XmlSchemaAny();
            any.MinOccurs = 1;
            any.MaxOccursString = "unbounded";
            any.ProcessContents = XmlSchemaContentProcessing.Strict;
            any.Namespace = "##local";

            XmlSchemaSequence sequence = new XmlSchemaSequence();
            type.Particle = sequence;
            sequence.Items.Add(any);

            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "each";
            element.SchemaType = type;

            return element;
        }

        private Specification CreateAny(XmlElement node)
        {
            Specification elementSpec = CreateImplicitAnd(GetChildElements(node));
            return new AnySpecification(elementSpec);
        }

        private static XmlSchemaElement AnySchema()
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
            attrib.Name = "expressionLanguage";
            attrib.Use = XmlSchemaUse.Optional;
            attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            type.Attributes.Add(attrib);

            attrib = new XmlSchemaAttribute();
            attrib.Name = "failMessage";
            attrib.Use = XmlSchemaUse.Optional;
            attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            type.Attributes.Add(attrib);

            XmlSchemaAny any = new XmlSchemaAny();
            any.MinOccurs = 1;
            any.MaxOccursString = "unbounded";
            any.ProcessContents = XmlSchemaContentProcessing.Strict;
            any.Namespace = "##local";

            XmlSchemaSequence sequence = new XmlSchemaSequence();
            type.Particle = sequence;
            sequence.Items.Add(any);

            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "any";
            element.SchemaType = type;

            return element;
        }

        private Specification CreateCase(XmlElement node)
        {
            IList<XmlNode> childNodes = GetChildElements(node);
            List<WhenThenPair> whenThens = new List<WhenThenPair>();
            Specification elseSpec = null;

            int i = 0;
            while(i < childNodes.Count)
            {
                if (childNodes[i].Name == "else")
                {
                    elseSpec = CreateImplicitAnd(GetChildElements((XmlElement)childNodes[i]));
                    break;
                }

                if (childNodes[i].Name != "when")
                    throw new XmlSpecificationCompilerException("Expected <when> element.");
                Specification when = CreateImplicitAnd(GetChildElements((XmlElement)childNodes[i++]));

                if (childNodes[i].Name != "then")
                    throw new XmlSpecificationCompilerException("Expected <then> element.");
                Specification then = CreateImplicitAnd(GetChildElements((XmlElement)childNodes[i++]));

                whenThens.Add(new WhenThenPair(when, then));
            }

            if(elseSpec == null)
                throw new XmlSpecificationCompilerException("Expected <else> element following <when> - <then> pairs.");

            return new CaseSpecification(whenThens, elseSpec);
        }

        private static XmlSchemaElement CaseSchema()
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();
            XmlSchemaAll allSchema = new XmlSchemaAll();
            type.Particle = allSchema;

            //When
            XmlSchemaComplexType subType = new XmlSchemaComplexType();
            XmlSchemaAny any = new XmlSchemaAny();
            any.MinOccurs = 1;
            any.MaxOccursString = "unbounded";
            any.ProcessContents = XmlSchemaContentProcessing.Strict;
            any.Namespace = "##local";

            XmlSchemaSequence sequence = new XmlSchemaSequence();
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

            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
            attrib.Name = "expressionLanguage";
            attrib.Use = XmlSchemaUse.Optional;
            attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            type.Attributes.Add(attrib);

            attrib = new XmlSchemaAttribute();
            attrib.Name = "failMessage";
            attrib.Use = XmlSchemaUse.Optional;
            attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            type.Attributes.Add(attrib);

            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "case";
            element.SchemaType = type;

            return element;
        }

        private static Specification CreateTrue(XmlElement node)
        {
            return new TrueSpecification();
        }

        private static XmlSchemaElement TrueSchema()
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
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

            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "true";
            element.SchemaType = type;

            return element;
        }

        private static Specification CreateFalse(XmlElement node)
        {
            return new FalseSpecification();
        }

        private static XmlSchemaElement FalseSchema()
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
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

            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "false";
            element.SchemaType = type;

            return element;
        }

        private Specification CreateEqual(XmlElement node)
        {
            string refValue = GetAttributeOrNull(node, "refValue");
            if (refValue == null)
                throw new XmlSpecificationCompilerException("Xml attribute 'refValue' is required for equal.");

            EqualSpecification s = new EqualSpecification();
            s.RefValueExpression = CreateExpression(refValue, GetAttributeOrNull(node, "expressionLanguage"));
            return s;
        }

        private static XmlSchemaElement EqualSchema()
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
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

            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "equal";
            element.SchemaType = type;

            return element;
        }

        private Specification CreateNotEqual(XmlElement node)
        {
            string refValue = GetAttributeOrNull(node, "refValue");
            if (refValue == null)
                throw new XmlSpecificationCompilerException("Xml attribute 'refValue' is required for not-equal.");

            NotEqualSpecification s = new NotEqualSpecification();
            s.RefValueExpression = CreateExpression(refValue, GetAttributeOrNull(node, "expressionLanguage"));
            return s;
        }

        private static XmlSchemaElement NotEqualSchema()
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
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

            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "not-equal";
            element.SchemaType = type;

            return element;
        }

        private Specification CreateGreaterThan(XmlElement node)
        {
            string refValue = GetAttributeOrNull(node, "refValue");
            if (refValue == null)
                throw new XmlSpecificationCompilerException("Xml attribute 'refValue' is required.");

            GreaterThanSpecification s = new GreaterThanSpecification();
            s.RefValueExpression = CreateExpression(refValue, GetAttributeOrNull(node, "expressionLanguage"));

            string inclusive = GetAttributeOrNull(node, "inclusive");
            if (inclusive != null)
                s.Inclusive = bool.Parse(inclusive);
            return s;
        }

        private static XmlSchemaElement GreaterThanSchema( )
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();
            
            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
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

            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "greater-than";
            element.SchemaType = type;

            return element;
        }

        private Specification CreateLessThan(XmlElement node)
        {
            string refValue = GetAttributeOrNull(node, "refValue");
            if (refValue == null)
                throw new XmlSpecificationCompilerException("Xml attribute 'refValue' is required.");

            LessThanSpecification s = new LessThanSpecification();
            s.RefValueExpression = CreateExpression(refValue, GetAttributeOrNull(node, "expressionLanguage"));
            string inclusive = GetAttributeOrNull(node, "inclusive");
            if (inclusive != null)
                s.Inclusive = bool.Parse(inclusive);
            return s;
        }

        private static XmlSchemaElement LessThanSchema()
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
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

            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "less-than";
            element.SchemaType = type;

            return element;
        }

        private Specification CreateDefined(XmlElement node)
        {
            string id = GetAttributeOrNull(node, "spec");
            if (id == null)
                throw new XmlSpecificationCompilerException("Xml attribute 'spec' is required.");

            return new DefinedSpecification(ResolveSpecification(id));
        }

        private static XmlSchemaElement DefinedSchema()
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();
            
            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
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

            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "defined";
            element.SchemaType = type;

            return element;
        }

        private Specification CreateImplicitAnd(ICollection<XmlNode> nodes)
        {
            if (nodes.Count == 1)
            {
                // only 1 node, so we don't need to "and"
                return BuildNode((XmlElement)CollectionUtils.FirstElement(nodes));
            }
            else
            {
                // create an "and" for the child nodes
                AndSpecification spec = new AndSpecification();
                foreach (XmlElement node in nodes)
                {
                    spec.Add(BuildNode(node));
                }
                return spec;
            }
        }

        private static IList<XmlNode> GetChildElements(XmlElement node)
        {
            return CollectionUtils.Select<XmlNode>(node.ChildNodes, delegate(XmlNode child) { return child is XmlElement; });
        }

        private static string GetAttributeOrNull(XmlElement node, string attr)
        {
            string val = node.GetAttribute(attr);
            return string.IsNullOrEmpty(val) ? null : val;
        }

        private ISpecification ResolveSpecification(string id)
        {
            if (_resolver == null)
                throw new XmlSpecificationCompilerException(string.Format("Cannot resolve reference {0} because no resolver was provided.", id));
            return _resolver.GetSpecification(id);
        }

        private Expression CreateExpression(string text, string language)
        {
            IExpressionFactory exprFactory = _defaultExpressionFactory;
            if (language != null)
                exprFactory = CreateExpressionFactory(language);

            return exprFactory.CreateExpression(text);
        }

        private static IExpressionFactory CreateExpressionFactory(string language)
        {
            return new ExpressionFactoryExtensionPoint().CreateExtension(language);
        }
        #endregion
    }
}

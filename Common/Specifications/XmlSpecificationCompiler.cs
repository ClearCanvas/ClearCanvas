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
using System.Linq;
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
			return (IExpressionFactory)CreateExtension(new AttributeExtensionFilter(new LanguageSupportAttribute(language)));
		}
	}

	public interface IXmlSpecificationCompilerContext
	{
		IExpressionFactory DefaultExpressionFactory { get; }
		IExpressionFactory GetExpressionFactory(string language);
		ISpecification Compile(XmlElement containingNode);
		ISpecification GetSpecification(string id);
	}


	public class XmlSpecificationCompiler
	{
		#region IXmlSpecificationCompilerContext implementation

		class Context : IXmlSpecificationCompilerContext
		{
			private readonly XmlSpecificationCompiler _compiler;

			public Context(XmlSpecificationCompiler compiler)
			{
				_compiler = compiler;
			}

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
		}

		#endregion

		#region BuiltInOperator class

		class BuiltInOperator : IXmlSpecificationCompilerOperator
		{
			private readonly string _operator;
			private readonly Func<XmlElement, Specification> _factoryMethod;
			private readonly Func<XmlSchemaElement> _schemaMethod;

			public BuiltInOperator(string op, Func<XmlElement, Specification> factoryMethod, Func<XmlSchemaElement> schemaMethod)
			{
				_operator = op;
				_factoryMethod = factoryMethod;
				_schemaMethod = schemaMethod;
			}

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
		}

		#endregion

		#region NullResolver

		class NullResolver : ISpecificationProvider
		{
			public ISpecification GetSpecification(string id)
			{
				throw new XmlSpecificationCompilerException(string.Format("Cannot resolve reference {0} because no resolver was provided.", id));
			}
		}

		#endregion

		private readonly Dictionary<string, IXmlSpecificationCompilerOperator> _operatorMap = new Dictionary<string, IXmlSpecificationCompilerOperator>();
		private readonly ISpecificationProvider _resolver;
		private readonly IExpressionFactory _defaultExpressionFactory;
		private readonly IXmlSpecificationCompilerContext _compilerContext;
		private XmlSchema _schema;

		#region Constructors

		public XmlSpecificationCompiler(string defaultExpressionLanguage, ISpecificationProvider resolver = null)
			: this(CreateExpressionFactory(defaultExpressionLanguage), resolver)
		{
		}

		public XmlSpecificationCompiler(IExpressionFactory defaultExpressionFactory, ISpecificationProvider resolver = null)
			: this(defaultExpressionFactory, new IXmlSpecificationCompilerOperator[0], resolver)
		{
		}

		public XmlSpecificationCompiler(string defaultExpressionLanguage, IEnumerable<IXmlSpecificationCompilerOperator> extensionOperators, ISpecificationProvider resolver = null)
			: this(CreateExpressionFactory(defaultExpressionLanguage), extensionOperators, resolver)
		{
		}

		public XmlSpecificationCompiler(string defaultExpressionLanguage, IExtensionPoint extensionOperators, ISpecificationProvider resolver = null)
			: this(CreateExpressionFactory(defaultExpressionLanguage), extensionOperators.CreateExtensions().Cast<IXmlSpecificationCompilerOperator>(), resolver)
		{
		}

		public XmlSpecificationCompiler(IExpressionFactory defaultExpressionFactory, IExtensionPoint extensionOperators, ISpecificationProvider resolver = null)
			: this(defaultExpressionFactory, extensionOperators.CreateExtensions().Cast<IXmlSpecificationCompilerOperator>(), resolver)
		{
		}


		public XmlSpecificationCompiler(
			IExpressionFactory defaultExpressionFactory,
			IEnumerable<IXmlSpecificationCompilerOperator> extensionOperators,
			ISpecificationProvider resolver = null)
		{
			_resolver = resolver ?? new NullResolver();

			_defaultExpressionFactory = defaultExpressionFactory;
			_compilerContext = new Context(this);

			// declare built-in operators
			AddOperator(new BuiltInOperator("true", CreateTrue, XmlSpecificationSchema.TrueSchema));
			AddOperator(new BuiltInOperator("false", CreateFalse, XmlSpecificationSchema.FalseSchema));
			AddOperator(new BuiltInOperator("equal", CreateEqual, XmlSpecificationSchema.EqualSchema));
			AddOperator(new BuiltInOperator("not-equal", CreateNotEqual, XmlSpecificationSchema.NotEqualSchema));
			AddOperator(new BuiltInOperator("greater-than", CreateGreaterThan, XmlSpecificationSchema.GreaterThanSchema));
			AddOperator(new BuiltInOperator("less-than", CreateLessThan, XmlSpecificationSchema.LessThanSchema));
			AddOperator(new BuiltInOperator("and", CreateAnd, XmlSpecificationSchema.AndSchema));
			AddOperator(new BuiltInOperator("or", CreateOr, XmlSpecificationSchema.OrSchema));
			AddOperator(new BuiltInOperator("not", CreateNot, XmlSpecificationSchema.NotSchema));
			AddOperator(new BuiltInOperator("regex", CreateRegex, XmlSpecificationSchema.RegexStringMatchingSchema));
			AddOperator(new BuiltInOperator("starts-with", CreateStartsWith, XmlSpecificationSchema.StartsWithStringMatchingSchema));
			AddOperator(new BuiltInOperator("ends-with", CreateEndsWith, XmlSpecificationSchema.EndsWithStringMatchingSchema));
			AddOperator(new BuiltInOperator("contains", CreateContains, XmlSpecificationSchema.ContainsStringMatchingSchema));
			AddOperator(new BuiltInOperator("null", CreateIsNull, XmlSpecificationSchema.IsNullSchema));
			AddOperator(new BuiltInOperator("not-null", CreateNotNull, XmlSpecificationSchema.NotNullSchema));
			AddOperator(new BuiltInOperator("count", CreateCount, XmlSpecificationSchema.CountSchema));
			AddOperator(new BuiltInOperator("all", CreateAll, XmlSpecificationSchema.AllSchema));
			//note: "each" is a synonym for "all" - "each" is deprecated, but is still supported for backward compatibility
			AddOperator(new BuiltInOperator("each", CreateAll, XmlSpecificationSchema.EachSchema));
			AddOperator(new BuiltInOperator("any", CreateAny, XmlSpecificationSchema.AnySchema));
			AddOperator(new BuiltInOperator("case", CreateCase, XmlSpecificationSchema.CaseSchema));
			AddOperator(new BuiltInOperator("defined", CreateDefined, XmlSpecificationSchema.DefinedSchema));

			// add extension operators
			foreach (var compilerOperator in extensionOperators)
			{
				AddOperator(compilerOperator);
			}
		}

		#endregion

		#region Public API

		/// <summary>
		/// A compiled XML schema used by the compiler to verify specifications.
		/// </summary>
		public XmlSchema Schema
		{
			get { return _schema ?? (_schema = XmlSpecificationSchema.CompileSchema(_operatorMap.Values)); }
		}

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
				XmlSpecificationSchema.ValidateSpecification(containingNode, this.Schema);
			}
			return CreateImplicitAnd(GetChildElements(containingNode));
		}

		#endregion

		#region Helpers

		private void AddOperator(IXmlSpecificationCompilerOperator op)
		{
			_operatorMap.Add(op.OperatorTag, op);
		}

		private Specification BuildNode(XmlElement node)
		{
			if (!_operatorMap.ContainsKey(node.Name))
				throw new XmlSpecificationCompilerException("Unknown Xml specification node: " + node.Name);

			var op = _operatorMap[node.Name];
			var spec = op.Compile(node, _compilerContext);

			var test = GetAttributeOrNull(node, "test");
			if (test != null)
			{
				spec.TestExpression = CreateExpression(test, GetAttributeOrNull(node, "expressionLanguage"));
			}

			spec.FailureMessage = GetAttributeOrNull(node, "failMessage");
			return spec;
		}

		private Specification CreateAnd(XmlElement node)
		{
			var spec = new AndSpecification();
			foreach (XmlElement child in GetChildElements(node))
			{
				spec.Add(BuildNode(child));
			}
			return spec;
		}

		private Specification CreateOr(XmlElement node)
		{
			var spec = new OrSpecification();
			foreach (XmlElement child in GetChildElements(node))
			{
				spec.Add(BuildNode(child));
			}
			return spec;
		}

		private Specification CreateNot(XmlElement node)
		{
			var spec = new NotSpecification();
			foreach (XmlElement child in GetChildElements(node))
			{
				spec.Add(BuildNode(child));
			}
			return spec;
		}

		private static Specification CreateRegex(XmlElement node)
		{
			return CreateStringComparison(node, (pattern, ignoreCase, nullMatches) => new RegexSpecification(pattern, ignoreCase, nullMatches));
		}

		private static Specification CreateStartsWith(XmlElement node)
		{
			return CreateStringComparison(node, (pattern, ignoreCase, nullMatches) => new StartsWithSpecification(pattern, ignoreCase, nullMatches));
		}

		private static Specification CreateEndsWith(XmlElement node)
		{
			return CreateStringComparison(node, (pattern, ignoreCase, nullMatches) => new EndsWithSpecification(pattern, ignoreCase, nullMatches));
		}

		private static Specification CreateContains(XmlElement node)
		{
			return CreateStringComparison(node, (pattern, ignoreCase, nullMatches) => new ContainsSpecification(pattern, ignoreCase, nullMatches));
		}

		private static Specification CreateStringComparison(XmlElement node, Func<string, bool, bool, StringMatchingSpecification> factoryFunc)
		{
			var stringIgnoreCase = node.GetAttribute("ignoreCase");

			var ignoreCase = !stringIgnoreCase.Equals("false", StringComparison.InvariantCultureIgnoreCase);

			var pattern = GetAttributeOrNull(node, "pattern");
			if (pattern == null)
				throw new XmlSpecificationCompilerException("Xml attribute 'pattern' is required.");

			var nullMatches = false;
			var stringNullMatches = GetAttributeOrNull(node, "nullMatches");
			if (stringNullMatches != null)
				nullMatches = stringNullMatches.Equals("true", StringComparison.InvariantCultureIgnoreCase);

			return factoryFunc(pattern, ignoreCase, nullMatches);
		}

		private static Specification CreateNotNull(XmlElement node)
		{
			return new NotNullSpecification();
		}

		private static Specification CreateIsNull(XmlElement node)
		{
			return new IsNullSpecification();
		}

		private Specification CreateCount(XmlElement node)
		{
			var minString = node.GetAttribute("min");
			var maxString = node.GetAttribute("max");

			var min = (minString == "") ? 0 : Int32.Parse(minString);
			var max = (maxString == "") ? Int32.MaxValue : Int32.Parse(maxString);

			ICollection<XmlNode> childElements = GetChildElements(node);
			var elementSpec = childElements.Count > 0 ? CreateImplicitAnd(childElements) : null;
			return new CountSpecification(min, max, elementSpec);
		}

		private Specification CreateAll(XmlElement node)
		{
			var elementSpec = CreateImplicitAnd(GetChildElements(node));
			return new AllSpecification(elementSpec);
		}

		private Specification CreateAny(XmlElement node)
		{
			var elementSpec = CreateImplicitAnd(GetChildElements(node));
			return new AnySpecification(elementSpec);
		}

		private Specification CreateCase(XmlElement node)
		{
			var childNodes = GetChildElements(node);
			var whenThens = new List<WhenThenPair>();
			Specification elseSpec = null;

			var i = 0;
			while (i < childNodes.Count)
			{
				if (childNodes[i].Name == "else")
				{
					elseSpec = CreateImplicitAnd(GetChildElements((XmlElement)childNodes[i]));
					break;
				}

				if (childNodes[i].Name != "when")
					throw new XmlSpecificationCompilerException("Expected <when> element.");
				var when = CreateImplicitAnd(GetChildElements((XmlElement)childNodes[i++]));

				if (childNodes[i].Name != "then")
					throw new XmlSpecificationCompilerException("Expected <then> element.");
				var then = CreateImplicitAnd(GetChildElements((XmlElement)childNodes[i++]));

				whenThens.Add(new WhenThenPair(when, then));
			}

			if (elseSpec == null)
				throw new XmlSpecificationCompilerException("Expected <else> element following <when> - <then> pairs.");

			return new CaseSpecification(whenThens, elseSpec);
		}

		private static Specification CreateTrue(XmlElement node)
		{
			return new TrueSpecification();
		}

		private static Specification CreateFalse(XmlElement node)
		{
			return new FalseSpecification();
		}

		private Specification CreateEqual(XmlElement node)
		{
			var refValue = GetAttributeOrNull(node, "refValue");
			if (refValue == null)
				throw new XmlSpecificationCompilerException("Xml attribute 'refValue' is required for equal.");

			var s = new EqualSpecification();
			s.RefValueExpression = CreateExpression(refValue, GetAttributeOrNull(node, "expressionLanguage"));
			return s;
		}

		private Specification CreateNotEqual(XmlElement node)
		{
			var refValue = GetAttributeOrNull(node, "refValue");
			if (refValue == null)
				throw new XmlSpecificationCompilerException("Xml attribute 'refValue' is required for not-equal.");

			var s = new NotEqualSpecification();
			s.RefValueExpression = CreateExpression(refValue, GetAttributeOrNull(node, "expressionLanguage"));
			return s;
		}
		private Specification CreateGreaterThan(XmlElement node)
		{
			var refValue = GetAttributeOrNull(node, "refValue");
			if (refValue == null)
				throw new XmlSpecificationCompilerException("Xml attribute 'refValue' is required.");

			var s = new GreaterThanSpecification();
			s.RefValueExpression = CreateExpression(refValue, GetAttributeOrNull(node, "expressionLanguage"));

			var inclusive = GetAttributeOrNull(node, "inclusive");
			if (inclusive != null)
				s.Inclusive = bool.Parse(inclusive);
			return s;
		}

		private Specification CreateLessThan(XmlElement node)
		{
			var refValue = GetAttributeOrNull(node, "refValue");
			if (refValue == null)
				throw new XmlSpecificationCompilerException("Xml attribute 'refValue' is required.");

			var s = new LessThanSpecification();
			s.RefValueExpression = CreateExpression(refValue, GetAttributeOrNull(node, "expressionLanguage"));
			var inclusive = GetAttributeOrNull(node, "inclusive");
			if (inclusive != null)
				s.Inclusive = bool.Parse(inclusive);
			return s;
		}

		private Specification CreateDefined(XmlElement node)
		{
			var id = GetAttributeOrNull(node, "spec");
			if (id == null)
				throw new XmlSpecificationCompilerException("Xml attribute 'spec' is required.");

			return new DefinedSpecification(ResolveSpecification(id));
		}

		private Specification CreateImplicitAnd(ICollection<XmlNode> nodes)
		{
			if (nodes.Count == 1)
			{
				// only 1 node, so we don't need to "and"
				return BuildNode((XmlElement)CollectionUtils.FirstElement(nodes));
			}

			// create an "and" for the child nodes
			var spec = new AndSpecification();
			foreach (XmlElement node in nodes)
			{
				spec.Add(BuildNode(node));
			}
			return spec;
		}

		private static IList<XmlNode> GetChildElements(XmlElement node)
		{
			return CollectionUtils.Select<XmlNode>(node.ChildNodes, child => child is XmlElement);
		}

		private static string GetAttributeOrNull(XmlElement node, string attr)
		{
			var val = node.GetAttribute(attr);
			return string.IsNullOrEmpty(val) ? null : val;
		}

		private ISpecification ResolveSpecification(string id)
		{
			return _resolver.GetSpecification(id);
		}

		private Expression CreateExpression(string text, string language)
		{
			var exprFactory = _defaultExpressionFactory;
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

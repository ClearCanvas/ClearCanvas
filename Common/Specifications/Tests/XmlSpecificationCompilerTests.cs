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

#if UNIT_TESTS

#pragma warning disable 1591

using System;
using System.IO;
using NUnit.Framework;
using System.Collections.Generic;

namespace ClearCanvas.Common.Specifications.Tests
{
	[TestFixture]
	public class XmlSpecificationCompilerTests : TestsBase
	{
		/// <summary>
		/// The XML Spec Compiler relies on certain extension points, therefore we need to stub out the extension
		/// factory so we can supply the dependencies.
		/// </summary>
		class StubExtensionFactory : IExtensionFactory
		{
			public object[] CreateExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter, bool justOne)
			{
				Console.WriteLine(extensionPoint);
				if (extensionPoint is ExpressionFactoryExtensionPoint)
					return new object[] { new ConstantExpressionFactory() };

				return new object[0];
			}

			public ExtensionInfo[] ListExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter)
			{
				Console.WriteLine(extensionPoint);
				if (extensionPoint is ExpressionFactoryExtensionPoint)
					return new[] { new ExtensionInfo(typeof(ConstantExpressionFactory), extensionPoint.GetType(), null, null, true),  };

				return new ExtensionInfo[0];
			}
		}


		private readonly SpecificationFactory _factory;

		public XmlSpecificationCompilerTests()
		{
			// stub the extension factory
			Platform.SetExtensionFactory(new StubExtensionFactory());

			// load the test file
			using (Stream s = this.GetType().Assembly.GetManifestResourceStream("ClearCanvas.Common.Specifications.Tests.XmlSpecificationCompilerTests.xml"))
			{
				_factory = new SpecificationFactory(s);
			}
		}

		[Test]
		public void Test_TestExpression()
		{
			ISpecification s = _factory.GetSpecification("testExpression");

			// the above evaluates to an implicit and
			var s1 = (CompositeSpecification)s;
			foreach (Specification element in s1.Elements)
			{
				Assert.AreEqual("XXX", element.TestExpression.Text);
			}
		}

		[Test]
		public void Test_FailMessage()
		{
			ISpecification s = _factory.GetSpecification("failMessage");

			// the above evaluates to an implicit and
			var s1 = (CompositeSpecification)s;
			foreach (Specification element in s1.Elements)
			{
				Assert.AreEqual("XXX", element.FailureMessage);
			}
		}

		[Test]
		public void Test_True_Default()
		{
			ISpecification s = _factory.GetSpecification("true_default");
            Assert.IsInstanceOf(typeof(TrueSpecification), s);
		}

		[Test]
		public void Test_False_Default()
		{
			ISpecification s = _factory.GetSpecification("false_default");
            Assert.IsInstanceOf(typeof(FalseSpecification), s);
		}

		[Test]
		public void Test_Null_Default()
		{
			ISpecification s = _factory.GetSpecification("null_default");
            Assert.IsInstanceOf(typeof(IsNullSpecification), s);
		}

		[Test]
		public void Test_NotNull_Default()
		{
			ISpecification s = _factory.GetSpecification("notNull_default");
            Assert.IsInstanceOf(typeof(NotNullSpecification), s);
		}

		[Test]
		public void Test_Regex_Default()
		{
			ISpecification s = _factory.GetSpecification("regex_default");
            Assert.IsInstanceOf(typeof(RegexSpecification), s);

			var s1 = (RegexSpecification) s;
			Assert.AreEqual("XXX", s1.Pattern);
			Assert.AreEqual(false, s1.NullMatches);
			Assert.AreEqual(true, s1.IgnoreCase);
		}

		[Test]
		public void Test_Regex_Options1()
		{
			ISpecification s = _factory.GetSpecification("regex_options1");
            Assert.IsInstanceOf(typeof(RegexSpecification), s);

			var s1 = (RegexSpecification)s;
			Assert.AreEqual("XXX", s1.Pattern);
			Assert.AreEqual(false, s1.NullMatches);
			Assert.AreEqual(false, s1.IgnoreCase);
		}

		[Test]
		public void Test_Regex_Options2()
		{
			ISpecification s = _factory.GetSpecification("regex_options2");
            Assert.IsInstanceOf(typeof(RegexSpecification), s);

			var s1 = (RegexSpecification)s;
			Assert.AreEqual("XXX", s1.Pattern);
			Assert.AreEqual(true, s1.NullMatches);
			Assert.AreEqual(true, s1.IgnoreCase);
		}

		[Test]
		[ExpectedException(typeof(XmlSpecificationCompilerException))]
		public void Test_Regex_MissingPattern()
		{
			ISpecification s = _factory.GetSpecification("regex_missingPattern");
		}

		[Test]
		public void Test_StartsWith_Default()
		{
			var s = _factory.GetSpecification("startsWith_default");
			Assert.IsInstanceOf(typeof(StartsWithSpecification), s);

			var s1 = (StartsWithSpecification)s;
			Assert.AreEqual("XXX", s1.Pattern);
			Assert.AreEqual(false, s1.NullMatches);
			Assert.AreEqual(true, s1.IgnoreCase);
		}

		[Test]
		public void Test_StartsWith_Options1()
		{
			var s = _factory.GetSpecification("startsWith_options1");
			Assert.IsInstanceOf(typeof(StartsWithSpecification), s);

			var s1 = (StartsWithSpecification)s;
			Assert.AreEqual("XXX", s1.Pattern);
			Assert.AreEqual(false, s1.NullMatches);
			Assert.AreEqual(false, s1.IgnoreCase);
		}

		[Test]
		public void Test_StartsWith_Options2()
		{
			var s = _factory.GetSpecification("startsWith_options2");
			Assert.IsInstanceOf(typeof(StartsWithSpecification), s);

			var s1 = (StartsWithSpecification)s;
			Assert.AreEqual("XXX", s1.Pattern);
			Assert.AreEqual(true, s1.NullMatches);
			Assert.AreEqual(true, s1.IgnoreCase);
		}

		[Test]
		[ExpectedException(typeof(XmlSpecificationCompilerException))]
		public void Test_StartsWith_MissingPattern()
		{
			var s = _factory.GetSpecification("startsWith_missingPattern");
		}

		[Test]
		public void Test_EndsWith_Default()
		{
			var s = _factory.GetSpecification("endsWith_default");
			Assert.IsInstanceOf(typeof(EndsWithSpecification), s);

			var s1 = (EndsWithSpecification)s;
			Assert.AreEqual("XXX", s1.Pattern);
			Assert.AreEqual(false, s1.NullMatches);
			Assert.AreEqual(true, s1.IgnoreCase);
		}

		[Test]
		public void Test_EndsWith_Options1()
		{
			var s = _factory.GetSpecification("endsWith_options1");
			Assert.IsInstanceOf(typeof(EndsWithSpecification), s);

			var s1 = (EndsWithSpecification)s;
			Assert.AreEqual("XXX", s1.Pattern);
			Assert.AreEqual(false, s1.NullMatches);
			Assert.AreEqual(false, s1.IgnoreCase);
		}

		[Test]
		public void Test_EndsWith_Options2()
		{
			var s = _factory.GetSpecification("endsWith_options2");
			Assert.IsInstanceOf(typeof(EndsWithSpecification), s);

			var s1 = (EndsWithSpecification)s;
			Assert.AreEqual("XXX", s1.Pattern);
			Assert.AreEqual(true, s1.NullMatches);
			Assert.AreEqual(true, s1.IgnoreCase);
		}

		[Test]
		[ExpectedException(typeof(XmlSpecificationCompilerException))]
		public void Test_EndsWith_MissingPattern()
		{
			var s = _factory.GetSpecification("endsWith_missingPattern");
		}

		[Test]
		public void Test_Contains_Default()
		{
			var s = _factory.GetSpecification("contains_default");
			Assert.IsInstanceOf(typeof(ContainsSpecification), s);

			var s1 = (ContainsSpecification)s;
			Assert.AreEqual("XXX", s1.Pattern);
			Assert.AreEqual(false, s1.NullMatches);
			Assert.AreEqual(true, s1.IgnoreCase);
		}

		[Test]
		public void Test_Contains_Options1()
		{
			var s = _factory.GetSpecification("contains_options1");
			Assert.IsInstanceOf(typeof(ContainsSpecification), s);

			var s1 = (ContainsSpecification)s;
			Assert.AreEqual("XXX", s1.Pattern);
			Assert.AreEqual(false, s1.NullMatches);
			Assert.AreEqual(false, s1.IgnoreCase);
		}

		[Test]
		public void Test_Contains_Options2()
		{
			var s = _factory.GetSpecification("contains_options2");
			Assert.IsInstanceOf(typeof(ContainsSpecification), s);

			var s1 = (ContainsSpecification)s;
			Assert.AreEqual("XXX", s1.Pattern);
			Assert.AreEqual(true, s1.NullMatches);
			Assert.AreEqual(true, s1.IgnoreCase);
		}

		[Test]
		[ExpectedException(typeof(XmlSpecificationCompilerException))]
		public void Test_Contains_MissingPattern()
		{
			var s = _factory.GetSpecification("contains_missingPattern");
		}

		[Test]
		public void Test_Equal_Default()
		{
			ISpecification s = _factory.GetSpecification("equal_default");
            Assert.IsInstanceOf(typeof(EqualSpecification), s);

			var s1 = (EqualSpecification)s;
			Assert.AreEqual("XXX", s1.RefValueExpression.Text);
		}

		[Test]
		[ExpectedException(typeof(XmlSpecificationCompilerException))]
		public void Test_Equal_MissingRefValue()
		{
			ISpecification s = _factory.GetSpecification("equal_missingRefValue");
		}

		[Test]
		public void Test_GreaterThan_Default()
		{
			ISpecification s = _factory.GetSpecification("greaterThan_default");
            Assert.IsInstanceOf(typeof(GreaterThanSpecification), s);

			var s1 = (GreaterThanSpecification)s;
			Assert.AreEqual("XXX", s1.RefValueExpression.Text);
			Assert.AreEqual(false, s1.Inclusive);
		}

		[Test]
		public void Test_GreaterThan_Options1()
		{
			ISpecification s = _factory.GetSpecification("greaterThan_options1");
            Assert.IsInstanceOf(typeof(GreaterThanSpecification), s);

			var s1 = (GreaterThanSpecification)s;
			Assert.AreEqual("XXX", s1.RefValueExpression.Text);
			Assert.AreEqual(false, s1.Inclusive);
		}

		[Test]
		public void Test_GreaterThan_Options2()
		{
			ISpecification s = _factory.GetSpecification("greaterThan_options2");
            Assert.IsInstanceOf(typeof(GreaterThanSpecification), s);

			var s1 = (GreaterThanSpecification)s;
			Assert.AreEqual("XXX", s1.RefValueExpression.Text);
			Assert.AreEqual(true, s1.Inclusive);
		}

		[Test]
		[ExpectedException(typeof(XmlSpecificationCompilerException))]
		public void Test_GreaterThan_MissingRefValue()
		{
			ISpecification s = _factory.GetSpecification("greaterThan_missingRefValue");
		}

		[Test]
		public void Test_LessThan_Default()
		{
			ISpecification s = _factory.GetSpecification("lessThan_default");
            Assert.IsInstanceOf(typeof(LessThanSpecification), s);

			var s1 = (LessThanSpecification)s;
			Assert.AreEqual("XXX", s1.RefValueExpression.Text);
			Assert.AreEqual(false, s1.Inclusive);
		}

		[Test]
		public void Test_LessThan_Options1()
		{
			ISpecification s = _factory.GetSpecification("lessThan_options1");
            Assert.IsInstanceOf(typeof(LessThanSpecification), s);

			var s1 = (LessThanSpecification)s;
			Assert.AreEqual("XXX", s1.RefValueExpression.Text);
			Assert.AreEqual(false, s1.Inclusive);
		}

		[Test]
		public void Test_LessThan_Options2()
		{
			ISpecification s = _factory.GetSpecification("lessThan_options2");
            Assert.IsInstanceOf(typeof(LessThanSpecification), s);

			var s1 = (LessThanSpecification)s;
			Assert.AreEqual("XXX", s1.RefValueExpression.Text);
			Assert.AreEqual(true, s1.Inclusive);
		}

		[Test]
		[ExpectedException(typeof(XmlSpecificationCompilerException))]
		public void Test_LessThan_MissingRefValue()
		{
			ISpecification s = _factory.GetSpecification("lessThan_missingRefValue");
		}

		[Test]
		public void Test_Count_Default()
		{
			ISpecification s = _factory.GetSpecification("count_default");
            Assert.IsInstanceOf(typeof(CountSpecification), s);

			var s1 = (CountSpecification)s;
			Assert.AreEqual(0, s1.Min);
			Assert.AreEqual(int.MaxValue, s1.Max);
		}

		[Test]
		public void Test_Count_Options1()
		{
			ISpecification s = _factory.GetSpecification("count_options1");
            Assert.IsInstanceOf(typeof(CountSpecification), s);

			var s1 = (CountSpecification)s;
			Assert.AreEqual(1, s1.Min);
			Assert.AreEqual(int.MaxValue, s1.Max);
		}

		[Test]
		public void Test_Count_Options2()
		{
			ISpecification s = _factory.GetSpecification("count_options2");
            Assert.IsInstanceOf(typeof(CountSpecification), s);

			var s1 = (CountSpecification)s;
			Assert.AreEqual(0, s1.Min);
			Assert.AreEqual(2, s1.Max);
		}

		[Test]
		public void Test_Count_Options3()
		{
			ISpecification s = _factory.GetSpecification("count_options3");
            Assert.IsInstanceOf(typeof(CountSpecification), s);

			var s1 = (CountSpecification)s;
			Assert.AreEqual(1, s1.Min);
			Assert.AreEqual(2, s1.Max);
		}

		[Test]
		public void Test_Count_Filtered()
		{
			ISpecification s = _factory.GetSpecification("count_filtered");
            Assert.IsInstanceOf(typeof(CountSpecification), s);

			var s1 = (CountSpecification)s;
			Assert.AreEqual(1, s1.Min);
			Assert.AreEqual(2, s1.Max);
			Assert.IsNotNull(s1.FilterSpecification);
            Assert.IsInstanceOf(typeof(TrueSpecification), s1.FilterSpecification);
		}

		[Test]
		public void Test_And_Default()
		{
			ISpecification s = _factory.GetSpecification("and_default");
            Assert.IsInstanceOf(typeof(AndSpecification), s);

			var s1 = (AndSpecification)s;
			var elements = new List<ISpecification>(s1.Elements);
			Assert.AreEqual(2, elements.Count);
            Assert.IsInstanceOf(typeof(TrueSpecification), elements[0]);
            Assert.IsInstanceOf(typeof(FalseSpecification), elements[1]);
		}

		[Test]
		public void Test_And_Implicit()
		{
			ISpecification s = _factory.GetSpecification("and_implicit");
            Assert.IsInstanceOf(typeof(AndSpecification), s);

			var s1 = (AndSpecification)s;
			var elements = new List<ISpecification>(s1.Elements);
			Assert.AreEqual(2, elements.Count);
            Assert.IsInstanceOf(typeof(TrueSpecification), elements[0]);
            Assert.IsInstanceOf(typeof(FalseSpecification), elements[1]);
		}

		[Test]
		public void Test_And_Empty()
		{
			ISpecification s = _factory.GetSpecification("and_empty");
            Assert.IsInstanceOf(typeof(AndSpecification), s);

			var s1 = (AndSpecification)s;
			var elements = new List<ISpecification>(s1.Elements);
			Assert.AreEqual(0, elements.Count);
		}

		[Test]
		public void Test_Or_Default()
		{
			ISpecification s = _factory.GetSpecification("or_default");
            Assert.IsInstanceOf(typeof(OrSpecification), s);

			var s1 = (OrSpecification)s;
			var elements = new List<ISpecification>(s1.Elements);
			Assert.AreEqual(2, elements.Count);
            Assert.IsInstanceOf(typeof(TrueSpecification), elements[0]);
            Assert.IsInstanceOf(typeof(FalseSpecification), elements[1]);
		}

		[Test]
		public void Test_Or_Empty()
		{
			ISpecification s = _factory.GetSpecification("or_empty");
            Assert.IsInstanceOf(typeof(OrSpecification), s);

			var s1 = (OrSpecification)s;
			var elements = new List<ISpecification>(s1.Elements);
			Assert.AreEqual(0, elements.Count);
		}

		[Test]
		public void Test_Not_Default()
		{
			ISpecification s = _factory.GetSpecification("not_default");
            Assert.IsInstanceOf(typeof(NotSpecification), s);

			var s1 = (NotSpecification)s;
			var elements = new List<ISpecification>(s1.Elements);
			Assert.AreEqual(2, elements.Count);
            Assert.IsInstanceOf(typeof(TrueSpecification), elements[0]);
            Assert.IsInstanceOf(typeof(FalseSpecification), elements[1]);
		}

		[Test]
		public void Test_Not_Empty()
		{
			ISpecification s = _factory.GetSpecification("not_missingElement");
            Assert.IsInstanceOf(typeof(NotSpecification), s);

			var s1 = (NotSpecification)s;
			var elements = new List<ISpecification>(s1.Elements);
			Assert.AreEqual(0, elements.Count);
		}

		[Test]
		public void Test_Each_Default()
		{
			ISpecification s = _factory.GetSpecification("each_default");
            Assert.IsInstanceOf(typeof(AllSpecification), s);

			var s1 = (AllSpecification)s;
			Assert.IsNotNull(s1.ElementSpec);
            Assert.IsInstanceOf(typeof(TrueSpecification), s1.ElementSpec);
		}

		[Test]
		public void Test_Each_MissingElement()
		{
			//TODO: this scenario does not currently throw an exception - should it?
			ISpecification s = _factory.GetSpecification("each_missingElement");
            Assert.IsInstanceOf(typeof(AllSpecification), s);
		}

		[Test]
		public void Test_All_Default()
		{
			ISpecification s = _factory.GetSpecification("all_default");
			Assert.IsInstanceOf(typeof(AllSpecification), s);

			var s1 = (AllSpecification)s;
			Assert.IsNotNull(s1.ElementSpec);
			Assert.IsInstanceOf(typeof(TrueSpecification), s1.ElementSpec);
		}

		[Test]
		public void Test_All_MissingElement()
		{
			//TODO: this scenario does not currently throw an exception - should it?
			ISpecification s = _factory.GetSpecification("all_missingElement");
			Assert.IsInstanceOf(typeof(AllSpecification), s);
		}

		[Test]
		public void Test_Any_Default()
		{
			ISpecification s = _factory.GetSpecification("any_default");
            Assert.IsInstanceOf(typeof(AnySpecification), s);

			var s1 = (AnySpecification)s;
			Assert.IsNotNull(s1.ElementSpec);
            Assert.IsInstanceOf(typeof(TrueSpecification), s1.ElementSpec);
		}

		[Test]
		public void Test_Any_MissingElement()
		{
			//TODO: this scenario does not currently throw an exception - should it?
			ISpecification s = _factory.GetSpecification("any_missingElement");
            Assert.IsInstanceOf(typeof(AnySpecification), s);
		}
	}
}

#endif

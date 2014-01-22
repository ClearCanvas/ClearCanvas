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


using System;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Tests;
using ClearCanvas.Dicom.Utilities.Rules.Specifications;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Utilities.Rules.Tests
{
	public class TestContext: ActionContext
	{
		public bool WasApplied = false;
	}

	public enum TestEnum
	{
		Test1,
		Test2
	}

	[ExtensionPoint]
	public sealed class TestActionCompilerOperatorExtensionPoint : ExtensionPoint<IXmlActionCompilerOperator<TestContext>>
	{
	}

	[ExtensionOf(typeof(TestActionCompilerOperatorExtensionPoint))]
	public class TestActionOperator : ActionOperatorCompilerBase, IXmlActionCompilerOperator<TestContext>
	{
		public TestActionOperator()
			: base("test")
		{
		}

		#region IXmlActionCompilerOperator<ServerActionContext> Members

		public IActionItem<TestContext> Compile(XmlElement xmlNode)
		{
			
			return new TestActionItem();
		}

		public XmlSchemaElement GetSchema()
		{
			var type = new XmlSchemaComplexType();

			var element = new XmlSchemaElement
				{
					Name = "test", 
					SchemaType = type
				};

			return element;
		}

		#endregion
	}

    public class TestActionItem : ActionItemBase<TestContext>
    {
        public TestActionItem()
            : base("Test Action")
        {
        }

        protected override bool OnExecute(TestContext context)
        {
	        context.WasApplied = true;
            return true;
        }
    }

	public class TestRulesEngine : RulesEngine<TestContext,TestEnum>
	{
		public TestRulesEngine()
        {
            Statistics = new RulesEngineStatistics("TEST","TEST");
        }

		/// <summary>
		/// Load the rules engine from the Persistent Store and compile the conditions and actions.
		/// </summary>
		public void Load(XmlDocument theRuleXml, TestEnum test)
		{

			// Clearout the current type list.
			_typeList.Clear();

			try
			{
				var theRule = new Rule<TestContext>
					{
						Name = "TEST", IsDefault = false, 
						IsExempt = false, Description = "TEST"
					};

				var ruleNode =
					CollectionUtils.SelectFirst<XmlNode>(theRuleXml.ChildNodes,
					                                     delegate(XmlNode child) { return child.Name.Equals("rule"); });

				var specCompiler = GetSpecificationCompiler();
				var actionCompiler = GetActionCompiler(test);
				theRule.Compile(ruleNode, specCompiler, actionCompiler);

				RuleTypeCollection<TestContext, TestEnum> typeCollection;

				if (!_typeList.ContainsKey(test))
				{
					typeCollection = new RuleTypeCollection<TestContext, TestEnum>(test);
					_typeList.Add(test, typeCollection);
				}
				else
				{
					typeCollection = _typeList[test];
				}

				typeCollection.AddRule(theRule);
			}
			catch (Exception e)
			{
				// something wrong with the rule...
				Platform.Log(LogLevel.Warn, e, "Unable to add rule to the engine. It will be skipped");
			}

		}

		public static XmlSpecificationCompiler GetSpecificationCompiler()
		{
			return new XmlSpecificationCompiler("dicom", new DicomRuleSpecificationCompilerOperatorExtensionPoint());
		}

		public static XmlActionCompiler<TestContext> GetActionCompiler(TestEnum ruleType)
		{
			var xp = new TestActionCompilerOperatorExtensionPoint();
			var operators = xp.CreateExtensions(ext => IsApplicableAction(ext.ExtensionClass.Resolve(), ruleType))
				.Cast<IXmlActionCompilerOperator<TestContext>>();

			return new XmlActionCompiler<TestContext>(operators);
		}

		private static bool IsApplicableAction(Type actionClass, TestEnum ruleType)
		{
			return true;
		}
	}

	[TestFixture]
	class DicomExpressionTests : AbstractTest
	{

		[Test]
		public void SequenceMatchingTests()
		{
			var context = new TestContext { Message = new DicomFile() };
			SetupMultiframeXA(context.Message.DataSet, 64, 64, 5);

			var rulesEngine = new TestRulesEngine();

			var doc = new XmlDocument();
			doc.LoadXml("<rule expressionLanguage=\"dicom\" ><condition><regex test=\"$RequestAttributesSequence/$RequestedProcedureId\" pattern=\"XA123\" /></condition><action><test/></action></rule>");
			rulesEngine.Load(doc,TestEnum.Test1);
			rulesEngine.Execute(context);
			Assert.IsTrue(context.WasApplied);

			doc = new XmlDocument();
			context.WasApplied = false;
			doc.LoadXml("<rule expressionLanguage=\"dicom\" ><condition><regex test=\"$RequestAttributesSequence/$ScheduledProcedureStepId\" pattern=\"XA1234\" /></condition><action><test/></action></rule>");
			rulesEngine.Load(doc, TestEnum.Test1);
			rulesEngine.Execute(context);
			Assert.IsTrue(context.WasApplied);

			doc = new XmlDocument();
			context.WasApplied = false;
			doc.LoadXml("<rule expressionLanguage=\"dicom\" ><condition><regex test=\"$RequestAttributesSequence/$ScheduledProcedureStepId\" pattern=\"XA5678\" /></condition><action><test/></action></rule>");
			rulesEngine.Load(doc, TestEnum.Test1);
			rulesEngine.Execute(context);
			Assert.IsTrue(context.WasApplied);

			doc = new XmlDocument();
			context.WasApplied = false;
			doc.LoadXml("<rule expressionLanguage=\"dicom\" ><condition><regex test=\"$RequestAttributesSequence/$ScheduledProcedureStepId\" pattern=\"FAIL\" /></condition><action><test/></action></rule>");
			rulesEngine.Load(doc, TestEnum.Test1);
			rulesEngine.Execute(context);
			Assert.IsFalse(context.WasApplied);
		}

		[Test]
		public void BaseMatchingTests()
		{
			var context = new TestContext { Message = new DicomFile() };
			SetupMultiframeXA(context.Message.DataSet, 32, 32, 2);

			var rulesEngine = new TestRulesEngine();

			var doc = new XmlDocument();
			doc.LoadXml("<rule expressionLanguage=\"dicom\" ><condition><regex test=\"$StudyDescription\" pattern=\"HEART\" /></condition><action><test/></action></rule>");
			rulesEngine.Load(doc, TestEnum.Test1);
			rulesEngine.Execute(context);
			Assert.IsTrue(context.WasApplied);

			rulesEngine = new TestRulesEngine();
			context.WasApplied = false;
			doc = new XmlDocument();
			doc.LoadXml("<rule expressionLanguage=\"dicom\" ><condition><regex test=\"$StudyDescription\" pattern=\"HEART22\" /></condition><action><test/></action></rule>");
			rulesEngine.Load(doc, TestEnum.Test1);
			rulesEngine.Execute(context);
			Assert.IsFalse(context.WasApplied);

			rulesEngine = new TestRulesEngine();
			context.WasApplied = false;
			doc = new XmlDocument();
			doc.LoadXml("<rule expressionLanguage=\"dicom\" ><condition><regex test=\"$StudyDescription\" pattern=\"FAIL\" /></condition><action><test/></action></rule>");
			rulesEngine.Load(doc, TestEnum.Test1);
			rulesEngine.Execute(context);
			Assert.IsFalse(context.WasApplied);

		}
	}
}

#endif

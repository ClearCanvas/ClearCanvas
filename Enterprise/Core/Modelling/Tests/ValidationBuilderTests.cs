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

using ClearCanvas.Common.Specifications;
using NUnit.Framework;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling.Tests
{
	[TestFixture]
	public class ValidationBuilderTests
	{
		class FooA
		{
			[Length(5)]
			public string Name { get; set; }
		}

		class FooB
		{
			[Required]
			public string Name { get; set; }
		}

		class FooC
		{
			[EmbeddedValue]
			public FooA EmbeddedFoo { get; set; }
		}

		class FooD
		{
			[Length(5)]
			[Required]
			public string Name { get; set; }
		}

		class FooE
		{
			[Length(5)]
			public string Name { get; set; }

			[Required]
			public string Color { get; set; }
		}

		[Validation(HighLevelRulesProviderMethod = "GetRules")]
		class BarA
		{
			private static IValidationRuleSet GetRules()
			{
				var rule = new ValidationRule<BarA>(i => new TestResult(false, "This rule always fails."));
				return new ValidationRuleSet(new ISpecification[]{rule});
			}
		}

		class BarB : BarA
		{
		}

		[Validation(HighLevelRulesProviderMethod = "GetRules")]
		class BarC : BarA
		{
			private static IValidationRuleSet GetRules()
			{
				var rule = new ValidationRule<BarA>(i => new TestResult(false, "This rule always fails."));
				return new ValidationRuleSet(new ISpecification[] { rule });
			}
		}

		[Test]
		public void Test_LengthSpecification()
		{
			var builder = new ValidationBuilder(typeof(FooA));
			var ruleSet = builder.LowLevelRules;

			Assert.AreEqual(1, ruleSet.Rules.Count);
			object rule = ruleSet.Rules[0];
			Assert.IsInstanceOf<LengthSpecification>(rule);
			var property = CollectionUtils.FirstElement((rule as IPropertyBoundRule).Properties);
			Assert.AreEqual(typeof(FooA).GetProperty("Name"), property);

			var fooA = new FooA {Name = "Bob"};

			Assert.IsTrue(ruleSet.Test(fooA).Success);

			fooA.Name = "Robert";

			Assert.IsFalse(ruleSet.Test(fooA).Success);
		}

		[Test]
		public void Test_RequiredSpecification()
		{
			var builder = new ValidationBuilder(typeof(FooB));
			var ruleSet = builder.LowLevelRules;

			Assert.AreEqual(1, ruleSet.Rules.Count);
			object rule = ruleSet.Rules[0];
			Assert.IsInstanceOf<RequiredSpecification>(rule);
			var property = CollectionUtils.FirstElement((rule as IPropertyBoundRule).Properties);
			Assert.AreEqual(typeof(FooB).GetProperty("Name"), property);

			var foo = new FooB {Name = "Bob"};

			Assert.IsTrue(ruleSet.Test(foo).Success);

			// should fail
			foo.Name = null;
			Assert.IsFalse(ruleSet.Test(foo).Success);

			// emtpy string should fail as well
			foo.Name = "";
			Assert.IsFalse(ruleSet.Test(foo).Success);
		}

		[Test]
		public void Test_EmbeddedValue()
		{
			var builder = new ValidationBuilder(typeof(FooC));
			var ruleSet = builder.LowLevelRules;

			Assert.AreEqual(1, ruleSet.Rules.Count);
			object rule = ruleSet.Rules[0];
			var property = CollectionUtils.FirstElement((rule as IPropertyBoundRule).Properties);
			Assert.AreEqual(typeof(FooC).GetProperty("EmbeddedFoo"), property);

			var foo = new FooC {EmbeddedFoo = null};

			// this should pass, because rules on the embedded value class are not evaluated when the property is null
			Assert.IsTrue(ruleSet.Test(foo).Success);

			// should pass
			foo.EmbeddedFoo = new FooA {Name = "Bob"};
			Assert.IsTrue(ruleSet.Test(foo).Success);

			// should fail because Robert is longer than 5 chars
			foo.EmbeddedFoo.Name = "Robert";
			Assert.IsFalse(ruleSet.Test(foo).Success);
		}

		[Test]
		public void Test_CombinationOfRequiredAndLengthOnSameProperty()
		{
			var builder = new ValidationBuilder(typeof(FooD));
			var ruleSet = builder.LowLevelRules;

			var foo = new FooD();

			// should fail the "required" but not the "length" test
			var result = ruleSet.Test(foo);
			Assert.IsFalse(result.Success);
			Assert.AreEqual(1, result.Reasons.Length);

			// should fail the "length" test but not the "required" test
			foo.Name = "Robert";
			result = ruleSet.Test(foo);
			Assert.IsFalse(result.Success);
			Assert.AreEqual(1, result.Reasons.Length);

			// should fail neither
			foo.Name = "Bob";
			result = ruleSet.Test(foo);
			Assert.IsTrue(result.Success);
			Assert.AreEqual(0, result.Reasons.Length);
		}

		[Test]
		public void Test_MultipleBrokenRules()
		{
			var builder = new ValidationBuilder(typeof(FooE));
			var ruleSet = builder.LowLevelRules;

			var foo = new FooE {Name = "Robert"};

			// should fail both
			var result = ruleSet.Test(foo);
			Assert.IsFalse(result.Success);
			Assert.AreEqual(2, result.Reasons.Length);

			// should fail only Name length
			foo.Color = "Blue";
			result = ruleSet.Test(foo);
			Assert.IsFalse(result.Success);
			Assert.AreEqual(1, result.Reasons.Length);

			// should fail neither
			foo.Name = "Bot";
			result = ruleSet.Test(foo);
			Assert.IsTrue(result.Success);
			Assert.AreEqual(0, result.Reasons.Length);

			// should fail Color required
			foo.Color = null;
			result = ruleSet.Test(foo);
			Assert.IsFalse(result.Success);
			Assert.AreEqual(1, result.Reasons.Length);
		}

		[Test]
		public void Test_HighLevel_rules_method()
		{
			var builder = new ValidationBuilder(typeof(BarA));
			var result = builder.HighLevelRules.Test(new BarA());

			// expect exactly one broken rule
			Assert.IsFalse(result.Success);
			Assert.AreEqual(1, result.Reasons.Length);
		}

		[Test]
		public void Test_HighLevel_rules_method_inherited()
		{
			// BarB inherits from BarA, but does not define its own additional rules method
			var builder = new ValidationBuilder(typeof(BarB));
			var result = builder.HighLevelRules.Test(new BarB());

			// expect exactly one broken rule (more than 1 would indicate that the GetRules had been processed more than once)
			Assert.IsFalse(result.Success);
			Assert.AreEqual(1, result.Reasons.Length);

			// BarC inherits from BarA, and also defines its own additional rules method
			builder = new ValidationBuilder(typeof(BarC));
			result = builder.HighLevelRules.Test(new BarC());

			// expect 2 broken rules, one from BarA and one from BarC
			Assert.IsFalse(result.Success);
			Assert.AreEqual(2, result.Reasons.Length);
		}
	}
}

#endif

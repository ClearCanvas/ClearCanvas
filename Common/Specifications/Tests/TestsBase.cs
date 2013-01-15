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
using NUnit.Framework;

namespace ClearCanvas.Common.Specifications.Tests
{
	public abstract class TestsBase
	{
		private class ConstantSpecification : Specification
		{
			private readonly TestResult _result;

			public ConstantSpecification(TestResult result)
			{
				_result = result;
			}

			protected override TestResult InnerTest(object exp, object root)
			{
				return _result;
			}
		}

		public ISpecification AlwaysTrue = new ConstantSpecification(new TestResult(true, new TestResultReason("Always true")));
		public ISpecification AlwaysFalse = new ConstantSpecification(new TestResult(false, new TestResultReason("Always false")));

		protected class PredicateSpecification<T> : Specification
		{
			private readonly Predicate<T> _predicate;

			public PredicateSpecification(Predicate<T> predicate)
			{
				_predicate = predicate;
			}

			protected override TestResult InnerTest(object exp, object root)
			{
				return new TestResult(_predicate((T)exp));
			}
		}

		protected class ConstantExpression : Expression
		{
			private readonly object _value;

			public ConstantExpression(object constantValue)
				:base("")
			{
				_value = constantValue;
			}

			public ConstantExpression(string text, object constantValue)
				:base(text)
			{
				_value = constantValue;
			}

			public object Value
			{
				get { return _value; }
			}

			public override object Evaluate(object arg)
			{
				return _value;
			}
		}

		protected class ConstantExpressionFactory : IExpressionFactory
		{
			public Expression CreateExpression(string text)
			{
				return new ConstantExpression(text, null);
			}
		}
	}
}

#endif

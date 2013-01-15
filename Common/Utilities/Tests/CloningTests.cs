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

#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

using NUnit.Framework;

namespace ClearCanvas.Common.Utilities.Tests
{
	[Cloneable]
	internal class TestDerivedClass : TestCallOrder
	{
		public static bool Cloning = false;

		[CloneIgnore]
		private object _ignoredValue;
		
		private TestDerivedClass(TestDerivedClass source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		public TestDerivedClass()
		{
			if (Cloning)
				Assert.Fail("Cloning constructor should be called.");
		}

		public object IgnoredValue
		{
			get { return _ignoredValue; }
			set { _ignoredValue = value; }
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Assert.AreEqual(CallOrder++, 2);
		}
	}

	[Cloneable(true)]
	internal class TestCallOrder : TestDefaultConstructor
	{
		private int _testField;

		public TestCallOrder()
		{
		}

		public int TestField
		{
			get { return _testField; }
			set { _testField = value; }
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Assert.AreEqual(CallOrder++, 1);
		}
	}

	[Cloneable(true)]
	internal class TestDefaultConstructor
	{
		private object _cloneableObject;
		[CloneCopyReference]
		private object _copyReferenceObject;
		private int _value;

		[CloneIgnore] 
		public int CallOrder = 0;
		[CloneIgnore]
		public bool CloneInitializeCalled = false;
		[CloneIgnore]
		public bool CloneCompleteCalled = false;

		public TestDefaultConstructor()
		{
		}

		public object CloneableObject
		{
			get { return _cloneableObject; }
			set { _cloneableObject = value; }
		}

		public object CopyReferenceObject
		{
			get { return _copyReferenceObject; }
			set { _copyReferenceObject = value; }
		}

		public int Value
		{
			get { return _value; }
			set { _value = value; }
		}
		
		[CloneInitialize]
		private void Initialize(TestDefaultConstructor source, ICloningContext context)
		{
			context.CloneFields(source, this);
			CloneInitializeCalled = true;
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			CloneCompleteCalled = true;
			Assert.AreEqual(CallOrder++, 0);
		}
	}

	[Cloneable(true)]
	public class SimpleCloneableObject
	{
		public SimpleCloneableObject()
		{
		}
	}

	[TestFixture]
	public class CloningTests
	{
		public CloningTests()
		{
		}

		[Test]
		public void Test()
		{
			try
			{
				SimpleCloneableObject simple = new SimpleCloneableObject();

				TestDerivedClass.Cloning = false;

				TestDerivedClass test = new TestDerivedClass();

				TestDerivedClass.Cloning = true;

				test.IgnoredValue = simple;
				test.Value = 4;
				test.TestField = 5;
				test.CloneableObject = simple;
				test.CopyReferenceObject = simple;

				TestDerivedClass clone = (TestDerivedClass) CloneBuilder.Clone(test);

				Assert.AreEqual(clone.IgnoredValue, null);
				Assert.AreEqual(test.Value, clone.Value);
				Assert.AreEqual(test.TestField, clone.TestField);
				Assert.AreEqual(clone.CloneInitializeCalled, true);
				Assert.AreEqual(clone.CloneCompleteCalled, true);
				Assert.AreSame(clone.CopyReferenceObject, simple);
				Assert.AreNotSame(clone.CloneableObject, simple);
			}
			finally
			{
				TestDerivedClass.Cloning = false;
			}
		}
	}
}

#endif
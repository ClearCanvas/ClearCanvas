#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture(Description = "Tests for abstract ComposableLut base classes. Tests for specific implementations can be found in other specifically-named classes.")]
	internal class ComposableLutTests
	{
		[Test]
		public void TestModalityLut()
		{
			var lut = new MockModalityLut(i => 1.1*i);

			lut.AssertLookupValues(-1000, 1000);
		}

		[Test]
		public void TestGenericLut()
		{
			var lut = new MockGenericLut(i => 1.2*i);

			lut.AssertLookupValues(-1000, 1000);
		}

		[Test]
		public void TestVoiLut()
		{
			var lut = new MockVoiLut(i => 1.3*i);

			lut.AssertLookupValues(-1000, 1000);
		}

		[Test]
		public void TestPresentationLut()
		{
			var lut = new MockPresentationLut(i => (int) (1.3*i + 0.5));

			lut.AssertLookupValues(-1000, 1000);
		}

		#region Lut Implementations

		private class MockModalityLut : ComposableModalityLut
		{
			private readonly Func<int, double> _func;
			private readonly string _key;

			public MockModalityLut(Func<int, double> func)
			{
				_key = func.Method.Name;
				_func = func;
			}

			public override int MinInputValue { get; set; }

			public override int MaxInputValue { get; set; }

			public override double MinOutputValue { get; protected set; }

			public override double MaxOutputValue { get; protected set; }

			public override double this[int input]
			{
				get { return _func(input); }
			}

			public override string GetKey()
			{
				return _key;
			}

			public override string GetDescription()
			{
				return _key;
			}
		}

		private class MockGenericLut : ComposableLut
		{
			private readonly Func<double, double> _func;
			private readonly string _key;

			public MockGenericLut(Func<double, double> func)
			{
				_key = func.Method.Name;
				_func = func;
			}

			public override double MinInputValue { get; set; }

			public override double MaxInputValue { get; set; }

			public override double MinOutputValue { get; protected set; }

			public override double MaxOutputValue { get; protected set; }

			public override double this[double input]
			{
				get { return _func(input); }
			}

			public override string GetKey()
			{
				return _key;
			}

			public override string GetDescription()
			{
				return _key;
			}
		}

		private class MockVoiLut : ComposableVoiLut
		{
			private readonly Func<double, double> _func;
			private readonly string _key;

			public MockVoiLut(Func<double, double> func)
			{
				_key = func.Method.Name;
				_func = func;
			}

			public override double MinInputValue { get; set; }

			public override double MaxInputValue { get; set; }

			public override double MinOutputValue { get; protected set; }

			public override double MaxOutputValue { get; protected set; }

			public override double this[double input]
			{
				get { return _func(input); }
			}

			public override string GetKey()
			{
				return _key;
			}

			public override string GetDescription()
			{
				return _key;
			}
		}

		private class MockPresentationLut : PresentationLut
		{
			private readonly Func<double, int> _func;
			private readonly string _key;

			public MockPresentationLut(Func<double, int> func)
			{
				_key = func.Method.Name;
				_func = func;
			}

			public override int this[double input]
			{
				get { return _func(input); }
			}

			public override string GetKey()
			{
				return _key;
			}

			public override string GetDescription()
			{
				return _key;
			}
		}

		#endregion
	}
}

#endif
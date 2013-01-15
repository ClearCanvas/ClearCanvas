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

namespace ClearCanvas.Desktop.Actions.Tests
{
	/// <summary>
	/// Decorates an action to specify a desired outcome when the action permissions are evaluated.
	/// </summary>
	public sealed class MockActionPermissionAttribute : ActionDecoratorAttribute
	{
		private readonly bool _result = true;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="actionId">The ID of the action to which the mock action permission applies.</param>
		/// <param name="result">The desired result when the action permissions are evaluated. True indicates that permissions are granted; False indicates that permissions are denied.</param>
		public MockActionPermissionAttribute(string actionId, bool result)
			: base(actionId)
		{
			_result = result;
		}

		/// <summary>
		/// Applies permissions represented by this attribute to an action instance, via the specified <see cref="IActionBuildingContext"/>.
		/// </summary>
		public override void Apply(IActionBuildingContext builder)
		{
			builder.Action.PermissionSpecification = new MockSpecification {Result = _result};
		}

		private class MockSpecification : ISpecification
		{
			public bool Result { private get; set; }

			public TestResult Test(object @object)
			{
				return new TestResult(Result);
			}
		}
	}
}

#endif
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

namespace ClearCanvas.Common.Specifications
{
	public class TestResult
	{
		private readonly bool _success;
		private readonly TestResultReason[] _reasons;

		public TestResult(bool success)
			: this(success, new TestResultReason[] { })
		{
		}

		public TestResult(bool success, string reason)
			: this(success, new[] { new TestResultReason(reason) })
		{
		}

		public TestResult(bool success, TestResultReason reason)
			: this(success, new[] { reason })
		{
		}

		public TestResult(bool success, TestResultReason[] reasons)
		{
			_success = success;
			_reasons = reasons;
		}

		public bool Success
		{
			get { return _success; }
		}

		public bool Fail
		{
			get { return !_success; }
		}

		public TestResultReason[] Reasons
		{
			get { return _reasons; }
		}
	}
}

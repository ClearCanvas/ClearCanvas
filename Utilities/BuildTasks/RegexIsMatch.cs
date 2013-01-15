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

using System;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ClearCanvas.Utilities.BuildTasks
{
	public class RegexIsMatch : Task
	{
		private string _pattern;
		private string _input;
		private bool _isMatch;

		[Output]
		public bool IsMatch
		{
			get { return _isMatch; }
		}

		[Required]
		public string Pattern
		{
			get { return _pattern; }
			set { _pattern = value; }
		}

		[Required]
		public string Input
		{
			get { return _input; }
			set { _input = value; }
		}

		public override bool Execute()
		{
			if (String.IsNullOrEmpty(_input))
				return false;

			if (String.IsNullOrEmpty(_pattern))
				return false;

			_isMatch = Regex.IsMatch(_input, _pattern);

			return true;
		}
	}
}

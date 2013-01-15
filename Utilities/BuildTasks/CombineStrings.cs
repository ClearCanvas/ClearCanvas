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
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ClearCanvas.Utilities.BuildTasks
{
	public class CombineStrings : Task
	{
		private string[] _inputStrings;
		private string _delimiter;
		private string _combinedString;

		[Required]
		public string[] InputStrings
		{
			get { return _inputStrings; }
			set { _inputStrings = value; }
		}

		public string Delimiter
		{
			get { return _delimiter; }
			set { _delimiter = value; }
		}

		[Output]
		public string CombinedString
		{
			get { return _combinedString; }
		}

		public override bool Execute()
		{
			if (_inputStrings == null)
				return false;

			StringBuilder builder = new StringBuilder();
			int i = 0;
			foreach (string str in _inputStrings)
			{
				if (String.IsNullOrEmpty(str))
					continue;

				if (i++ > 0)
					builder.Append(_delimiter ?? "");
				builder.Append(str);
			}

			_combinedString = builder.ToString();

			return true;
		}
	}
}

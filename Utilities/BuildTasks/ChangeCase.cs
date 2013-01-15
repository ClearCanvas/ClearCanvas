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
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ClearCanvas.Utilities.BuildTasks
{
	public enum ChangeCaseMode
	{
		LowerCase = 0,
		UpperCase = 1,
		Lower = LowerCase,
		Upper = UpperCase
	}

	public class ChangeCase : Task
	{
		[Output]
		[Required]
		public string String { get; set; }

		[Required]
		public string Mode { get; set; }

		public override bool Execute()
		{
			ChangeCaseMode mode;
			if (!TryGetChangeCaseMode(out mode))
			{
				Log.LogError("Mode should be one of LowerCase or UpperCase");
				return false;
			}

			if (!string.IsNullOrEmpty(String))
			{
				switch (mode)
				{
					case ChangeCaseMode.LowerCase:
						String = String.ToLowerInvariant();
						break;
					case ChangeCaseMode.UpperCase:
						String = String.ToUpperInvariant();
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			return true;
		}

		/// <summary>
		/// Parses the value of <see cref="Mode"/>.
		/// </summary>
		protected bool TryGetChangeCaseMode(out ChangeCaseMode result)
		{
			if (string.IsNullOrEmpty(Mode))
			{
				result = ChangeCaseMode.LowerCase;
				return true;
			}

			foreach (ChangeCaseMode eValue in Enum.GetValues(typeof (ChangeCaseMode)))
			{
				if (string.Equals(Mode, eValue.ToString(), StringComparison.InvariantCultureIgnoreCase))
				{
					result = eValue;
					return true;
				}
			}

			result = ChangeCaseMode.LowerCase;
			return false;
		}
	}
}
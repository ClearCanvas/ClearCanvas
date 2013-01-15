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

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Defines the possible levels for alerts.
	/// </summary>
	public enum AlertLevel
	{
		/// <summary>
		/// An informational alert notifies the user of an event that is not a problem.
		/// </summary>
		Info,

		/// <summary>
		/// A warning alert notifies the user of a potentially problematic event.
		/// </summary>
		Warning,

		/// <summary>
		/// An error alert notifies the user of a failure which will likely require some corrective action.
		/// </summary>
		Error
	}

	internal static class AlertLevelExtensions
	{
		/// <summary>
		/// Gets the icon corresponding to the specified alert level.
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public static IconSet GetIcon(this AlertLevel level)
		{
			switch (level)
			{
				case AlertLevel.Info:
					return new IconSet("InfoMini.png", "InfoSmall.png", "InfoMedium.png");
				case AlertLevel.Warning:
					return new IconSet("WarningMini.png", "WarningSmall.png", "WarningMedium.png");
				case AlertLevel.Error:
					return new IconSet("ErrorMini.png", "ErrorSmall.png", "ErrorMedium.png");
			}
			throw new ArgumentOutOfRangeException();
		}
	}
}

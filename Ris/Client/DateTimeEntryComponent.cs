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
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="DateTimeEntryComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class DateTimeEntryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	/// <summary>
	/// DateTimeEntryComponent class
	/// </summary>
	[AssociateView(typeof (DateTimeEntryComponentViewExtensionPoint))]
	public class DateTimeEntryComponent : ApplicationComponent
	{
		/// <summary>
		/// Static helper method to prompt user for time in a single line of code.
		/// </summary>
		/// <param name="desktopWindow"></param>
		/// <param name="title"></param>
		/// <param name="time"></param>
		/// <param name="allowNull"></param>
		/// <returns></returns>
		public static bool PromptForTime(IDesktopWindow desktopWindow, [param : Localizable(true)] string title, bool allowNull, ref DateTime? time)
		{
			DateTimeEntryComponent component = new DateTimeEntryComponent(time, allowNull);
			if (LaunchAsDialog(desktopWindow, component, title)
			    == ApplicationComponentExitCode.Accepted)
			{
				time = component.DateAndTime;
				return true;
			}
			return false;
		}

		private DateTime? _dateAndTime;
		private readonly bool _allowNull;

		/// <summary>
		/// Constructor
		/// </summary>
		public DateTimeEntryComponent(DateTime? initialValue, bool allowNull)
		{
			_dateAndTime = initialValue;
			_allowNull = allowNull;
		}

		#region Presentation Model

		public DateTime? DateAndTime
		{
			get { return _dateAndTime; }
			set { _dateAndTime = value; }
		}

		public bool AllowNull
		{
			get { return _allowNull; }
		}

		public void Accept()
		{
			Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			Exit(ApplicationComponentExitCode.None);
		}

		#endregion
	}
}
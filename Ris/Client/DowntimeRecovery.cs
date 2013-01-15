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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public static class DowntimeRecovery
	{
		private static bool _inDowntimeRecoveryMode;
		private static event EventHandler _inDowntimeRecoveryModeChanged;

		/// <summary>
		/// Gets a value indicating whether the client application is running in downtime recovery mode.
		/// </summary>
		public static bool InDowntimeRecoveryMode
		{
			get { return _inDowntimeRecoveryMode; }
			internal set
			{
				if(_inDowntimeRecoveryMode != value)
				{
					_inDowntimeRecoveryMode = value;
					EventsHelper.Fire(_inDowntimeRecoveryModeChanged, null, EventArgs.Empty);
				}
			}
		}

		public static event EventHandler InDowntimeRecoveryModeChanged
		{
			add { _inDowntimeRecoveryModeChanged += value; }
			remove { _inDowntimeRecoveryModeChanged -= value; }
		}
	}
}

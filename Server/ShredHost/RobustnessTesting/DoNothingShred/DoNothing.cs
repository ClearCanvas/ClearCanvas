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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;


namespace ClearCanvas.Server.ShredHost.RobustnessTesting.DoNothingShred
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class DoNothingShred : Shred
	{
		private bool _stopRequested = false;
		private Thread _procedureThread = null;
		private int _iteration = 0;

		#region IShred Members

		public override void Start()
		{
			_procedureThread = new Thread(new ThreadStart(DoNothingProcedure));
			_procedureThread.Start();
		}

		public override void Stop()
		{
			_stopRequested = true;
			_procedureThread.Join();
		}

		public override string GetDisplayName()
		{
			return "Do Nothing Shred";
		}

		public override string GetDescription()
		{
			return "ShredHost Robustness Testing Do Nothing Shred";
		}

		#endregion

		public void DoNothingProcedure()
		{
			while (!_stopRequested)
			{
				Thread.Sleep(5000);
				Platform.Log(LogLevel.Info, "Do Nothing Shred heartbeat");
			}

			++_iteration;
			if (_iteration > 100)
				throw new Exception("PiCalculator throws an exception");

		}
	}
}

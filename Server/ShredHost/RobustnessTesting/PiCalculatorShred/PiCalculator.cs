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

namespace ClearCanvas.Server.ShredHost.RobustnessTesting.PiCalculatorShred
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class PiCalculatorShred : Shred
	{
		private bool _stopRequested = false;
		private Thread _procedureThread = null;
		private int _iteration = 0;

		public PiCalculatorShred()
		{

		}

		public override void Start()
		{
			_procedureThread = new Thread(new ThreadStart(PiProcedure));
			_procedureThread.Start();
		}

		public override void Stop()
		{
			_stopRequested = true;
			_procedureThread.Join();
		}

		public void PiProcedure()
		{
			Random generator = new Random((Int32)DateTime.Now.Ticks);
			int hits = 0;
			int throws = 0;

			while (!_stopRequested)
			{
				Thread.Sleep(500);
				double x = generator.NextDouble();
				double y = generator.NextDouble();
				if ((x * x + y * y) <= 1.0)
				{
					hits++;
					throws++;
				}
				else
				{
					throws++;
				}

				Platform.Log(LogLevel.Info, String.Format("Pi is approximately  - {0:f}", ((double) hits/ throws)*4));

				++_iteration;
				if (_iteration > 100)
				{
					Exception inner = new Exception("Pi Calculator new inner exception");
					Exception e = new Exception("Pi Calculator throws this exception", inner);
					throw e;
				}
			}
		}
		public override string GetDescription()
		{
			return "Part of robustness testing, this shred calculates approximate value of Pi";
		}

		public override string GetDisplayName()
		{
			return "Pi Calculator Shred";
		}

	}
}

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

namespace ClearCanvas.Server.ShredHost.RobustnessTesting.FibonacciCalculatorShred
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class FibonacciCalculator : Shred
	{
		private bool _stopRequested = false;
		private Thread _procedureThread = null;
		private int _lastSum = 1;
		private int _nextToLastSum = 1;
		private int _iteration = 0;

		#region IShred Members

		public override void Start()
		{
			_procedureThread = new Thread(new ThreadStart(FibonacciProcedure));
			_procedureThread.Start();
		}

		public override void Stop()
		{
			_stopRequested = true;
			_procedureThread.Join();
		}

		public override string GetDisplayName()
		{
			return "Fibonacci Calculator Shred";
		}

		public override string GetDescription()
		{
			return "ShredHost Robustness Testing Fibonacci Calculator";
		}

		#endregion

		public void FibonacciProcedure()
		{
			while (!_stopRequested)
			{
				Thread.Sleep(5000);
				int newLastSum = _nextToLastSum + _lastSum;
				_nextToLastSum = _lastSum;
				_lastSum = newLastSum;

				Platform.Log(LogLevel.Info, String.Format("Fibonacci Number - {0}", _lastSum));

				++_iteration;
				if (_iteration > 100)
					throw new Exception("PiCalculator throws an exception");

			}
		}
	}
}

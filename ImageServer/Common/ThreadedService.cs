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
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Common
{
	/// <summary>
	/// Base class for a service that runs in a dedicated thread.
	/// </summary>
	public abstract class ThreadedService
	{
		#region Private Members

		private Thread _theThread;
		private readonly string _name;

		#endregion

		#region Public Properties

		/// <summary>
		/// Flag set to true if stop has been requested.
		/// </summary>
		public bool StopFlag { get; private set; }

		/// <summary>
		/// Reset event to signal when stopping the service thread.
		/// </summary>
		public ManualResetEvent ThreadStop { get; private set; }

		/// <summary>
		/// The name of the thread.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Retry delay (in ms) before retrying after a failure
		/// </summary>
		public int ThreadRetryDelay { get; set; }

		#endregion

		#region Protected Abstract Methods
		protected abstract bool Initialize();
		protected abstract void Run();
		protected abstract void Stop();
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of the service.</param>
		protected ThreadedService(string name)
		{
			_name = name;
			StopFlag = false;
			ThreadRetryDelay = 60000;
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// Check if a stop is requested.
		/// </summary>
		/// <param name="msDelay"></param>
		/// <returns></returns>
		protected bool CheckStop(int msDelay)
		{
			ThreadStop.WaitOne(msDelay, false);
			ThreadStop.Reset();

			return StopFlag;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Start the service.
		/// </summary>
		public void StartService()
		{
			if (_theThread == null)
			{
				ThreadStop = new ManualResetEvent(false);
				_theThread = new Thread(delegate()
					{
						bool bInit = false;
						while (!bInit)
						{
							try
							{
								bInit = Initialize();
							}
							catch (Exception e)
							{
								Platform.Log(LogLevel.Warn, "Unexpected exception intializing {0} service: {1}", Name, e.Message);
							}
							if (!bInit)
							{
								if (CheckStop(ThreadRetryDelay))
									return;
							}
						}

						if (StopFlag)
							return;

						try
						{
							Run();
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Error, e, "Unexpected exception running service {0}", Name);
						}

					});
				_theThread.Name = String.Format("{0}:{1}", Name, _theThread.ManagedThreadId);
				_theThread.Start();
			}
		}

        public void StopService(string reason)
        {
            Platform.Log(LogLevel.Info, "{0} service is stopping : {1}", Name, reason);
            StopService();
        }
		
		/// <summary>
		/// Stop the service.
		/// </summary>
		public void StopService()
		{
			StopFlag = true;
			Stop();

			if (_theThread != null && _theThread.IsAlive)
			{
				ThreadStop.Set();
				_theThread.Join();
				_theThread = null;
			}
		}
		#endregion
	}
}

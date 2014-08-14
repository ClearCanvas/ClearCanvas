#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Core.Helpers
{
	/// <summary>
	/// Helper class to make sure the database connection is valid when starting up a Shred.
	/// </summary>
	public class ShredStartupHelper
	{
		#region Private Members
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

		#region Constructors
		public ShredStartupHelper(string name)
		{
			_name = name;
			ThreadStop = new ManualResetEvent(false);
			ThreadRetryDelay = 60*1000;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Check if a stop is requested.
		/// </summary>
		/// <param name="msDelay"></param>
		/// <returns></returns>
		private bool CheckStop(int msDelay)
		{
			ThreadStop.WaitOne(msDelay, false);
			ThreadStop.Reset();

			return StopFlag;
		}
		#endregion

		#region Public Methods
		public void Initialize()
		{
			bool bInit = false;
			while (!bInit)
			{
				try
				{
					// Force a read context to be opened.  When developing the retry mechanism 
					// for startup when the DB was down, there were problems when the type
					// initializer for enumerated values were failng first.  For some reason,
					// when the database went back online, they would still give exceptions.
					// changed to force the processor to open a dummy DB connect and cause an 
					// exception here, instead of getting to the enumerated value initializer.
					using (IReadContext readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
					{
						readContext.Dispose();
					}
					bInit = true;
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
		}

		/// <summary>
		/// Stop the service.
		/// </summary>
		public void StopService()
		{
			StopFlag = true;
			ThreadStop.Set();
		}
		#endregion
	}
}

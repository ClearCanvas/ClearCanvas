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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Services.ServiceLock.Shreds
{
	public class ServiceLockServerManager : ThreadedService
	{
		#region Private Members
		private static ServiceLockServerManager _instance;
		private ServiceLockProcessor _theProcessor;
		#endregion

		#region Constructors
		/// <summary>
		/// **** For internal use only***
		/// </summary>
		private ServiceLockServerManager(string name) : base(name)
		{ }
		#endregion

		#region Properties
		/// <summary>
		/// Singleton instance of the class.
		/// </summary>
		public static ServiceLockServerManager Instance
		{
			get { return _instance ?? (_instance = new ServiceLockServerManager("ServiceLock")); }
			set
			{
				_instance = value;
			}
		}
		#endregion

		#region Protected Methods
		protected override bool Initialize()
		{
			if (_theProcessor == null)
			{
				// Force a read context to be opened.  When developing the retry mechanism 
				// for startup when the DB was down, there were problems when the type
				// initializer for enumerated values were failng first.  For some reason,
				// when the database went back online, they would still give exceptions.
				// changed to force the processor to open a dummy DB connect and cause an 
				// exception here, instead of getting to the enumerated value initializer.
				using (IReadContext readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
				{
				}

				_theProcessor = new ServiceLockProcessor(2, ThreadStop); // 2 threads for processor
			}

            return true;
		}

		protected override void Run()
		{
			_theProcessor.Run();
		}

		protected override void Stop()
		{
			//TODO CR (Jan 2014): Move this into the base if it applies to all subclasses?
			PersistentStoreRegistry.GetDefaultStore().ShutdownRequested = true;

			if (_theProcessor != null)
			{
				_theProcessor.Stop();
				_theProcessor = null;
			}
		}
		#endregion
	}
}
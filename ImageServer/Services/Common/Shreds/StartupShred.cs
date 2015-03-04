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
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core.Helpers;

namespace ClearCanvas.ImageServer.Services.Common.Shreds
{
	/// <summary>
	/// Plugin to generate an alert for startup/shutdown of ShredHost
	/// </summary>
	/// <remarks>
	/// Note that the code to generate the alert was originally just in the core code
	/// for the ShredHostService.  It was moved, however, into this thread because it 
	/// was causing startup/shutdown issues when the database wasn't available. 
	/// The underlying code to generate the alert accesses the database, and the code was
	/// stuck in a loop attempting to reconnect to the database.
	/// </remarks>
	[ExtensionOf(typeof(ShredExtensionPoint))]
	[ShredIsolation(Level = ShredIsolationLevel.None)]
	public class StartupShred : Shred
	{
		#region Private Members

		private readonly string _className;
		private ShredStartupHelper _shredStartupHelper;

		#endregion

		#region Constructors

		public StartupShred()
		{
			_className = GetType().ToString();
		}

		#endregion

		#region IShred Implementation Shred Override

		public override void Start()
		{
			_shredStartupHelper = new ShredStartupHelper(GetDisplayName());
			_shredStartupHelper.Initialize();
			if (_shredStartupHelper.StopFlag)
				return;
			_shredStartupHelper = null;

			try
			{
				ServerPlatform.Alert(AlertCategory.System, AlertLevel.Informational,
								 SR.AlertComponentName, AlertTypeCodes.Starting,
								 null, TimeSpan.Zero,
								 SR.AlertShredHostServiceStarting);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, "Unexpected exception generating an alert for ShredHost startup: {0}", e.Message);
			}
		}

		public override void Stop()
		{
			if (_shredStartupHelper != null)
				_shredStartupHelper.StopService();

			PersistentStoreRegistry.GetDefaultStore().ShutdownRequested = true;
			try
			{
				ServerPlatform.Alert(AlertCategory.System, AlertLevel.Informational,
				                     SR.AlertComponentName, AlertTypeCodes.Stopped,
				                     null, TimeSpan.Zero,
				                     SR.AlertShredHostServiceStopped);
			}
			catch (Exception)
			{
				Platform.Log(LogLevel.Error, "Unexpected exception generating an alert for ShredHost shutdown");
			}
		}

		public override string GetDisplayName()
		{
			return _className;
		}

		public override string GetDescription()
		{
			return _className;
		}

		#endregion
	}

}

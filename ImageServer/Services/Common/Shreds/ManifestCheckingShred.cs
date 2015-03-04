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
using ClearCanvas.Common.Shreds;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core.Helpers;

namespace ClearCanvas.ImageServer.Services.Common.Shreds
{
	/// <summary>
	/// Plugin to check manifest and generate an alert every 24 hrs.
	/// </summary>
	[ExtensionOf(typeof(ShredExtensionPoint))]
	[ShredIsolation(Level = ShredIsolationLevel.None)]
	public class ManifestCheckingShred : Shred
	{
		#region Private Members

		private readonly string _className;

		private Timer _timer;
		private readonly TimeSpan _oneMinute = TimeSpan.FromSeconds(60);
		private readonly TimeSpan _repeatEvery24Hours = TimeSpan.FromHours(24);
		private ShredStartupHelper _shredStartupHelper;

		#endregion

		#region Constructors

		public ManifestCheckingShred()
		{
			_className = GetType().ToString();
		}

		#endregion

		#region IShred Implementation Shred Override

		public override void Start()
		{
			Platform.Log(LogLevel.Debug, "{0}[{1}]: Start invoked", _className, AppDomain.CurrentDomain.FriendlyName);

			_shredStartupHelper = new ShredStartupHelper(GetDisplayName());
			_shredStartupHelper.Initialize();
			if (_shredStartupHelper.StopFlag)
				return;
			_shredStartupHelper = null;

			try
			{
				_timer = new Timer(OnTimer, null, _oneMinute, _repeatEvery24Hours);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Fatal, e, "Unexpected exception starting {0}", GetDisplayName());
			}
		}

		public override void Stop()
		{
			if (_shredStartupHelper != null)
				_shredStartupHelper.StopService();

			if (_timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}

			PersistentStoreRegistry.GetDefaultStore().ShutdownRequested = true;
			Platform.Log(LogLevel.Info, "{0}[{1}]: Stop invoked", _className, AppDomain.CurrentDomain.FriendlyName);
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

		private void OnTimer(object nothing)
		{
			try
			{
				VerifyManifest();
			}
			catch (Exception ex)
			{
				//catch everything or the shred host will crash
				Platform.Log(LogLevel.Error, ex, "Error occurred when trying to check manifest");
			}
		}

		private void VerifyManifest()
		{
			Platform.Log(LogLevel.Info, "Checking manifest...");
			if (ServerPlatform.IsManifestVerified)
			{
				Platform.Log(LogLevel.Info, "Manifest is OK.");
				return;
			}

			Platform.Log(LogLevel.Warn, "Manifest is broken!");
			var componentName = string.IsNullOrEmpty(ProductInformation.SubComponent)
									? ProductInformation.Component
									: string.Format("{0} {1}", ProductInformation.Component, ProductInformation.SubComponent);

			ServerPlatform.Alert(AlertCategory.Security, AlertLevel.Warning,
				SR.ManifestComponentName, AlertTypeCodes.UnableToProcess, null, TimeSpan.FromHours(24),
				SR.NonStandardInstallation, componentName);
		}
	}
}
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
using System.Globalization;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.UsageTracking;
using ClearCanvas.Utilities.Manifest;

namespace ClearCanvas.Desktop
{
	internal static class PhoneHome
	{
		#region Private Fields

		/// <summary>
		/// Interval between periodic calls while application is active.
		/// </summary>
		private static readonly TimeSpan _periodicCallInterval = TimeSpan.FromHours(24);

		/// <summary>
		/// Delay before initial call when application is starting up.
		/// </summary>
		private static readonly TimeSpan _startupCallDelay = TimeSpan.FromSeconds(10);

		/// <summary>
		/// Timeout for final call when application is shutting down.
		/// </summary>
		private static readonly TimeSpan _shutdownCallTimeout = TimeSpan.FromSeconds(10);

		private static Timer _phoneHomeTimer;
		private static readonly object _sync = new object();
		private static bool _started;
		private static DateTime _startTimestamp;
		private static bool _sentStartUpMessage;

		#endregion

		#region Public Methods

		/// <summary>
		/// Start up the phone home service.
		/// </summary>
		internal static void Startup()
		{
			lock (_sync)
			{
				OnStartUp();

				_phoneHomeTimer = new Timer(ignore =>
				                            	{
				                            		var type = _sentStartUpMessage ? UsageType.Other : UsageType.Startup;
				                            		var msg = CreateUsageMessage(type);
				                            		msg.AppData = new List<UsageApplicationData>();
				                            		msg.AppData.AddRange(UsageUtilities.GetApplicationData(type));
				                            		UsageUtilities.Register(msg, UsageTrackingThread.Background);

				                            		_sentStartUpMessage = true;
				                            	},
				                            null,
				                            _startupCallDelay,
				                            _periodicCallInterval);
			}
		}

		/// <summary>
		/// Shut down the phone home service.
		/// </summary>
		internal static void ShutDown()
		{
			// Guard against incorrect use of this class when Startup() is not called.
			if (!_started)
				return;

			lock (_sync)
			{
				try
				{
					OnShutdown();

					// Note: use a thread to send the message because we don't want to block the app
					var workerThread = new Thread(SendShutdownMessage);
					workerThread.Start();

					// wait up to 10 seconds, this is a requirement.
					if (!workerThread.Join(_shutdownCallTimeout))
					{
						Platform.Log(LogLevel.Debug,
						             "PhoneHome.ShutDown(): web service does not return within 10 seconds. Continue shutting down.");
					}
				}
				catch (Exception ex)
				{
					// Requirement says log must be in debug
					Platform.Log(LogLevel.Debug, ex, "Error occurred when shutting down phone home service");
				}
			}
		}

		#endregion

		#region Helpers

		private static void SendShutdownMessage()
		{
			const string keyProcessUptime = "PROCESSUPTIME";
			try
			{
				TimeSpan uptime = DateTime.Now - _startTimestamp;

				var msg = CreateUsageMessage(UsageType.Shutdown);
				msg.AppData = new List<UsageApplicationData>();
				msg.AppData.AddRange(UsageUtilities.GetApplicationData(UsageType.Shutdown));
				msg.AppData.Add(new UsageApplicationData {Key = keyProcessUptime, Value = String.Format(CultureInfo.InvariantCulture, "{0}", uptime.TotalHours)});

				// Message must be sent using the current thread instead of threadpool when the app is being shut down
				UsageUtilities.Register(msg, UsageTrackingThread.Current);
			}
			catch (Exception ex)
			{
				// Requirement says log must be in debug
				Platform.Log(LogLevel.Debug, ex, "Error occurred when shutting down phone home service");
			}
		}

		private static UsageMessage CreateUsageMessage(UsageType type)
		{
			var msg = UsageUtilities.GetUsageMessage();
			msg.Certified = ManifestVerification.Valid;
			msg.MessageType = type;
			return msg;
		}

		private static void OnStartUp()
		{
			if (!_started)
			{
				_startTimestamp = DateTime.Now;
				_started = true;
			}
		}

		private static void OnShutdown()
		{
			if (_phoneHomeTimer != null)
			{
				_phoneHomeTimer.Dispose();
				_phoneHomeTimer = null;
				_started = false;
			}
		}

		#endregion
	}
}
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
using System.Globalization;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common.Time;

namespace ClearCanvas.Enterprise.Common
{
	[ExtensionPoint]
	public class EnterpriseTimeProviderOfflineCacheExtensionPoint : ExtensionPoint<IOfflineCache<string, string>>
	{
	}

	/// <summary>
	/// Provides a consistent time to the application from the enterprise server.
	/// </summary>
	[ExtensionOf(typeof(TimeProviderExtensionPoint))]
	public class EnterpriseTimeProvider : ITimeProvider
	{
		// this key could be anything, as long as it's unique
		private const string TimeOffsetCacheKey = "{92E55B13-96A5-4a03-A669-B22D5D29E95B}";

		private TimeSpan _localToEnterpriseOffset;
		private DateTime _lastSuccessfulResyncInLocalTime;
		private DateTime _lastAttemptedResyncInLocalTime;
		private readonly TimeSpan _resyncPeriod;
		private readonly TimeSpan _maxTimeBetweenSync;
		private readonly IOfflineCache<string, string> _offlineCache;

		public EnterpriseTimeProvider()
		{
			_lastSuccessfulResyncInLocalTime = DateTime.MinValue;
			_lastAttemptedResyncInLocalTime = DateTime.MinValue;
			_resyncPeriod = new TimeSpan(0, 0, 60);
			_maxTimeBetweenSync = new TimeSpan(0, 10, 0);

			try
			{
				_offlineCache = (IOfflineCache<string, string>)(new EnterpriseTimeProviderOfflineCacheExtensionPoint()).CreateExtension();
			}
			catch (NotSupportedException)
			{
				Platform.Log(LogLevel.Debug, SR.ExceptionOfflineCacheNotFound);

				_offlineCache = new NullOfflineCache<string, string>();
			}
		}

		#region ITimeProvider Members

		public DateTime GetCurrentTime(DateTimeKind kind)
		{
			if (ResyncRequired())
			{
				ResyncLocalToEnterpriseTime();
			}
			switch (kind)
			{
				case DateTimeKind.Utc:
					return EnterpriseTime(DateTime.UtcNow);
				case DateTimeKind.Local:
					return EnterpriseTime(DateTime.Now);
				default:
					throw new ArgumentOutOfRangeException("kind");
			}
		}

		#endregion

		private DateTime EnterpriseTime(DateTime localTime)
		{
			return localTime - _localToEnterpriseOffset;
		}

		private bool ResyncRequired()
		{
			return (DateTime.Now - _lastAttemptedResyncInLocalTime) > _resyncPeriod;
		}

		private void ResyncLocalToEnterpriseTime()
		{
			_lastAttemptedResyncInLocalTime = DateTime.Now;

			using (var client = _offlineCache.CreateClient())
			{
				try
				{
					var serverTime = GetCurrentEnterpriseTime();

					_lastSuccessfulResyncInLocalTime = _lastAttemptedResyncInLocalTime;
					_localToEnterpriseOffset = DateTime.Now - serverTime;

					// update offline cache
					client.Put(TimeOffsetCacheKey, _localToEnterpriseOffset.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
				}
				catch (Exception)
				{
					if ((DateTime.Now - _lastSuccessfulResyncInLocalTime) > _maxTimeBetweenSync)
						LogWarningNoSync();

					// if the process has just started up, and we have not yet been able to connect to the server,
					// attempt to read last known value from the offline cache
					if (_localToEnterpriseOffset == TimeSpan.Zero)
					{
						double value;
						var s = client.Get(TimeOffsetCacheKey);
						_localToEnterpriseOffset = !string.IsNullOrEmpty(s) && double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
						                           	? TimeSpan.FromMilliseconds(value) : TimeSpan.Zero;
					}
				}
			}
		}

		private void LogWarningNoSync()
		{
			if (_lastSuccessfulResyncInLocalTime == DateTime.MinValue)
			{
				Platform.Log(LogLevel.Warn, "The time server has been unreachable since process start.");
			}
			else
			{
				Platform.Log(LogLevel.Warn, "The time server has been unreachable since {0}.", _lastSuccessfulResyncInLocalTime);
			}
		}

		private static DateTime GetCurrentEnterpriseTime()
		{
			var time = default(DateTime);

			Platform.GetService<ITimeService>(
				service => time = service.GetTime(new GetTimeRequest()).Time);

			return time;
		}
	}
}

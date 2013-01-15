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
using System.ServiceModel;
using ClearCanvas.Common.Audit;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common.Audit
{
	[ExtensionPoint]
	public class EnterpriseAuditSinkOfflineCacheExtensionPoint : ExtensionPoint<IOfflineCache<Guid, AuditEntryInfo>>
	{
	}

	/// <summary>
	/// An implementation of <see cref="IAuditSink"/> that sinks to the <see cref="IAuditService"/>.
	/// </summary>
	[ExtensionOf(typeof(AuditSinkExtensionPoint))]
	public class AuditSink : IAuditSink
	{
		private readonly IOfflineCache<Guid, AuditEntryInfo> _offlineCache;

		public AuditSink()
		{
			try
			{
				_offlineCache = (IOfflineCache<Guid, AuditEntryInfo>)(new EnterpriseAuditSinkOfflineCacheExtensionPoint()).CreateExtension();
			}
			catch (NotSupportedException)
			{
				Platform.Log(LogLevel.Debug, SR.ExceptionOfflineCacheNotFound);
			}
		}

		#region IAuditSink Members

		/// <summary>
		/// Writes the specified entry to the sink.
		/// </summary>
		/// <param name="entry"></param>
		public void WriteEntry(AuditEntryInfo entry)
		{
			try
			{
				Platform.GetService<IAuditService>(service => service.WriteEntry(new WriteEntryRequest(entry)));
			}
			catch (EndpointNotFoundException e)
			{
				if(_offlineCache == null)
					throw new AuditException(SR.ExceptionAuditServiceNotReachableAndNoOfflineCache, e);

				StoreOffline(entry);
			}
		}

		#endregion

		private void StoreOffline(AuditEntryInfo entry)
		{
			try
			{
				using (var client = _offlineCache.CreateClient())
				{
					// stick it in the offline cache
					// any unique value can be used as a key, because it will never be accessed by key again
					client.Put(Guid.NewGuid(), entry);
				}
			}
			catch (Exception e)
			{
				throw new AuditException(SR.ExceptionAuditServiceNotReachableAndNoOfflineCache, e);
			}
		}
	}
}

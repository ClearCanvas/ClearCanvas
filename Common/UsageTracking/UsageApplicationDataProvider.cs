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
using System.Linq;
using System.Runtime.Serialization;

namespace ClearCanvas.Common.UsageTracking
{
	[ExtensionPoint]
	public sealed class UsageApplicationDataProviderExtensionPoint : ExtensionPoint<IUsageApplicationDataProvider>
	{
		public new IList<IUsageApplicationDataProvider> CreateExtensions()
		{
			return base.CreateExtensions().OfType<IUsageApplicationDataProvider>().ToList();
		}
	}

	public interface IUsageApplicationDataProvider
	{
		UsageApplicationData GetData(UsageType type);
	}

	public abstract class UsageApplicationDataProvider : IUsageApplicationDataProvider
	{
		private readonly string _key;

		protected UsageApplicationDataProvider(string key)
		{
			Platform.CheckForEmptyString(key, "key");
			_key = key;
		}

		public string Key
		{
			get { return _key; }
		}

		public abstract string GetData(UsageType type);

		protected virtual bool HasData(UsageType type)
		{
			return true;
		}

		protected virtual ExtensionDataObject GetExtensionData()
		{
			return null;
		}

		UsageApplicationData IUsageApplicationDataProvider.GetData(UsageType type)
		{
			try
			{
				if (HasData(type)) return new UsageApplicationData {Key = Key, Value = GetData(type), ExtensionData = GetExtensionData()};
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Error getting application data for usage tracking.");
			}
			return null;
		}
	}
}
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

namespace ClearCanvas.Enterprise.Common.Manifest
{
	public static class ServerManifestVerification
	{
		private static readonly object _syncRoot = new object();
		private static int _lastCheck;
		private static bool? _lastValid;

		public static bool Valid
		{
			get
			{
				const int cacheTimeout = 90000;

				if (_lastValid.HasValue && Math.Abs(Environment.TickCount - _lastCheck) < cacheTimeout) return _lastValid.Value;

				lock (_syncRoot)
				{
					if (!(_lastValid.HasValue && Math.Abs(Environment.TickCount - _lastCheck) < cacheTimeout))
					{
						_lastValid = Check();
						_lastCheck = Environment.TickCount;
					}
					return _lastValid.GetValueOrDefault(true); // just return true if server status unknown, since there will likely be other (more pressing) issues
				}
			}
		}

		private static bool? Check()
		{
			bool? valid = null;
			try
			{
				Platform.GetService<IManifestService>(s => valid = s.GetStatus(new GetStatusRequest()).IsValid);
			}
			catch (Exception)
			{
				// if the manifest service errors out, eat the exception and 
				valid = null;
			}
			return valid;
		}
	}
}
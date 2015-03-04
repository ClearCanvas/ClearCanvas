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

namespace ClearCanvas.Common.Caching
{
	/// <summary>
	/// Encapsulates options for the <see cref="ICacheClient.Put"/> method.
	/// </summary>
	public class CachePutOptions : CacheOptionsBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="expiration"></param>
		/// <param name="sliding"></param>
		public CachePutOptions(TimeSpan expiration, bool sliding)
		{
			Expiration = expiration;
			Sliding = sliding;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="region"></param>
		/// <param name="expiration"></param>
		/// <param name="sliding"></param>
		public CachePutOptions(string region, TimeSpan expiration, bool sliding)
			: base(region)
		{
			Expiration = expiration;
			Sliding = sliding;
		}

		/// <summary>
		/// Gets or sets the expiration time.
		/// </summary>
		public TimeSpan Expiration { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the expiration is sliding or not.
		/// </summary>
		public bool Sliding { get; set; }
	}
}

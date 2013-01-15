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

namespace ClearCanvas.ImageServer.Web.Common
{
    public class Cache
    {
        private readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(15);

        private static readonly Cache _instance = new Cache();

        public static Cache Current
        {
            get { return _instance; }
        }

        private Cache(){}

        public object this[string key]
        {
            get
            {
                return System.Web.HttpContext.Current.Cache[key];
            }
            set
            {
                System.Web.HttpContext.Current.Cache.Remove(key); // returns null if it's not in the cache
                System.Web.HttpContext.Current.Cache.Add(key, value, null,
                                                     System.Web.Caching.Cache.NoAbsoluteExpiration,
                                                     CacheDuration, System.Web.Caching.CacheItemPriority.Normal, null);
                
                
            }
        }

        public bool Contains(string key)
        {
            return this[key] != null;
        }
    }
}
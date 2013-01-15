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
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyManagement.Core;

namespace ClearCanvas.ImageViewer.Shreds
{
    internal static class UserIdentityCache
    {
        private static readonly object SyncObject = new object();
        private static ICache<UserIdentityContext> _identityContextCache;

        public static void Put(long identifier, UserIdentityContext context)
        {
            lock (SyncObject)
            {
               Initialize();

                _identityContextCache.Put(identifier.ToString(CultureInfo.InvariantCulture), context);
            }
        }

        public static void Remove(long identifier)
        {
            lock (SyncObject)
            {
                Initialize();
                _identityContextCache.Remove(identifier.ToString(CultureInfo.InvariantCulture));
            }
        }

        public static UserIdentityContext Get(long identifier)
        {
            lock (SyncObject)
            {
                Initialize();
                var context = _identityContextCache.Get(identifier.ToString(CultureInfo.InvariantCulture)) ??
                              new UserIdentityContext();

                return context;
            }
        }

        private static void Initialize()
        {
            if (_identityContextCache == null)
            {
                _identityContextCache = Cache<UserIdentityContext>.Create(typeof (UserIdentityContext).FullName);
                (_identityContextCache as Cache<UserIdentityContext>).Expiration = TimeSpan.FromMinutes(60);
            }
        }
    }
}

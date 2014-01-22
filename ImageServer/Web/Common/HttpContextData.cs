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
using System.Web;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Web.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpContextData: IDisposable
    {
        private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        private const string CUSTOM_DATA_ENTRY = "CUSTOM_DATA_ENTRY";
        private IReadContext _readContext;
        private readonly object _syncRoot = new object();

        private HttpContextData()
        {
        }

        static public HttpContextData Current
        {
            get
            {
                lock( HttpContext.Current.Items.SyncRoot)
                {
                    HttpContextData instance = HttpContext.Current.Items[CUSTOM_DATA_ENTRY] as HttpContextData;
                    if (instance == null)
                    {
                        instance = new HttpContextData();
                        HttpContext.Current.Items[CUSTOM_DATA_ENTRY] = instance;
                    }
                    return instance;
                }
                
            }
        }

        public IReadContext ReadContext
        {
            get
            {
                if (_readContext == null)
                {
                    lock (_syncRoot)
                    {
                        if (_readContext == null)
                        {
                            _readContext = _store.OpenReadContext();
                        }
                    }
                }
                return _readContext;
                
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_readContext != null)
            {
                lock (_syncRoot)
                {
                    if (_readContext != null)
                    {
                        _readContext.Dispose();
                        _readContext = null;
                    }
                }
            }
        }

        #endregion
    }

	public static class HttpContextExtension
	{
		public static IReadContext GetSharedPersistentContext(this HttpContext ctx)
		{
			return HttpContextData.Current.ReadContext;
		}
	}
}
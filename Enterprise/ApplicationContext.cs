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
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Provides convenience methods for the application to obtain references
    /// to needed resources without having to understand how those resources
    /// are acquired or how they are associated with the underlying session, etc.
    /// </summary>
    public static class ApplicationContext
    {
        /// <summary>
        /// Obtains the service that implements the specified interface.
        /// </summary>
        /// <typeparam name="TServiceInterface">The interface that the service must implement</typeparam>
        /// <returns>An object that implements the specified interface</returns>
        public static TServiceInterface GetService<TServiceInterface>()
        {
            return Core.ServiceManager.GetService<TServiceInterface>();
        }
    }
}

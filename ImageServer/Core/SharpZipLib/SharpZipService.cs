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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ICSharpCode.SharpZipLib.Zip;

namespace ClearCanvas.ImageServer.Core.SharpZipLib
{
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    class SharpZipServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            if (serviceType != typeof(IZipService))
                return null;

            return new SharpZipService();
        }
    }

   

 

    /// <summary>
    /// Implementation of IZipService using SharpZipLib
    /// </summary>
    class SharpZipService : IZipService
    {
        public IZipServiceReader OpenRead(string zipFile)
        {
            return new SharpZipReader(zipFile);
        }

        public IZipServiceWriter OpenWrite(string zipFile)
        {
            return new SharpZipWriter(zipFile);
        }
    }
}

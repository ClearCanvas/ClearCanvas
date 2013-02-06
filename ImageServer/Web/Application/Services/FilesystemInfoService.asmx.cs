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

using System.Web.Script.Services;
using System.Web.Services;
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Web.Common.Utilities;
using System.Globalization;
using System.Threading;

namespace ClearCanvas.ImageServer.Web.Application.Services
{
    //// <summary>
    /// Summary description for FilesystemInfoService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class FilesystemInfoService : WebService
    {

        [WebMethod]
        public FilesystemInfo GetFilesystemInfo(string path)
        {
            Platform.CheckForEmptyString(path, "requested path is empty or null");

            return ServerUtility.GetFilesystemInfo(path.Trim());
        }
    }
}

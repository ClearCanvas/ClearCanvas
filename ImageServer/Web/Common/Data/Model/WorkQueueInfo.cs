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

namespace ClearCanvas.ImageServer.Web.Common.Data.Model
{
    [Serializable]
    public class WorkQueueInfo
    {
        private string _server;
        private int _itemCount;

        public string Server
        {
            get { return _server;}
            set { _server = value;}
        }

        public int ItemCount
        {
            get { return _itemCount; }
            set { _itemCount = value; }
        }

        public WorkQueueInfo(string server, int itemCount)
        {
            _server = server;
            _itemCount = itemCount;
        }
    }
}

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
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// Encapsulates the address of the server on the network.
    /// </summary>
    [Serializable]
    public class ServerAddress
    {
        #region Private Members
        private string _hostName;
        private List<string> _ipAddresses = new List<string>();
        #endregion

        #region Static Properties
        /// <summary>
        /// Gets a instance of <see cref="ServerAddress"/> that represents the server on local machine.
        /// </summary>
        public static  ServerAddress Local
        {
            get
            {
                ServerAddress local = new ServerAddress();
                local.HostName = Dns.GetHostName();
                IPAddress[] ipAddresses = Dns.GetHostAddresses(local.HostName);
                foreach (IPAddress ip in ipAddresses)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        local.IPAddresses.Add(ip.ToString());
                    }
                    else if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        local.IPAddresses.Add(ip.ToString());
                    }
                    
                }
                return local;
            }
        }

        #endregion 

        #region Public Properties
        /// <summary>
        /// The host name of the machine where the server is running
        /// </summary>
        public string HostName
        {
            get { return _hostName; }
            set { _hostName = value; }
        }
        /// <summary>
        /// IP addresses of the machine where the server is running
        /// </summary>
        public List<string> IPAddresses
        {
            get { return _ipAddresses; }
            set { _ipAddresses = value; }
        }
        #endregion
    }
}

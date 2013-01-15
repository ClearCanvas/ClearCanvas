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

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ClearCanvas.Server.ShredHost
{
    public class ServiceEndpointDescription
    {
        public ServiceEndpointDescription(string name, string description)
        {
            _serviceName = name;
            _serviceDescription = description;
        }

        private string _serviceName;
        private string _serviceDescription;
        private Binding _binding;
        private ServiceHost _serviceHost;

        public ServiceHost ServiceHost
        {
            get { return _serviceHost; }
            set { _serviceHost = value; }
        }

        public Binding Binding
        {
            get { return _binding; }
            set { _binding = value; }
        }

        public string ServiceName
        {
            get { return _serviceName; }
        }

        public string ServiceDescription
        {
            get { return _serviceDescription; }
        }
    }
}

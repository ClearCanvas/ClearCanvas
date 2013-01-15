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

using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.ServerDirectory;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{
    public static class DeviceExtensions
    {
        public static ServerDirectoryEntry ToDataContract(this Device device)
        {
            bool isStreaming = device.StreamingHeaderPort != null && device.StreamingImagePort != null;
            var server = new ApplicationEntity
                                           {
                                               Name = device.Name,
                                               AETitle = device.AETitle,
                                               ScpParameters = new ScpParameters(device.HostName, device.Port),
                                               Location = device.Location,
                                               Description = device.Description
                                           };

            if (isStreaming)
                server.StreamingParameters = new StreamingParameters(device.StreamingHeaderPort.Value, device.StreamingImagePort.Value);

            Dictionary<string, object> extensionData = null;
            if (device.ExtensionData != null)
                extensionData = Serializer.DeserializeServerExtensionData(device.ExtensionData);

            return new ServerDirectoryEntry(server) {IsPriorsServer = device.IsPriorsServer, Data = extensionData};
        }

        public static Device ToDevice(this ServerDirectoryEntry serverDirectoryEntry)
        {
            Platform.CheckForNullReference(serverDirectoryEntry, "serverDirectoryEntry");
            Platform.CheckMemberIsSet(serverDirectoryEntry.Server, "Server");
            Platform.CheckMemberIsSet(serverDirectoryEntry.Server.ScpParameters, "ScpParameters");

            int? streamingHeaderPort = null;
            int? streamingImagePort = null;
            if (serverDirectoryEntry.Server.StreamingParameters != null)
            {
                streamingHeaderPort = serverDirectoryEntry.Server.StreamingParameters.HeaderServicePort;
                streamingImagePort = serverDirectoryEntry.Server.StreamingParameters.WadoServicePort;
            }

            string extensionData = null;
            if (serverDirectoryEntry.Data != null)
                extensionData = Serializer.SerializeServerExtensionData(serverDirectoryEntry.Data);

            return new Device
                       {
                           Name = serverDirectoryEntry.Server.Name,
                           AETitle = serverDirectoryEntry.Server.AETitle,
                           HostName = serverDirectoryEntry.Server.ScpParameters.HostName,
                           Port = serverDirectoryEntry.Server.ScpParameters.Port,
                           Location = serverDirectoryEntry.Server.Location,
                           Description = serverDirectoryEntry.Server.Description,
                           StreamingHeaderPort = streamingHeaderPort,
                           StreamingImagePort = streamingImagePort,
                           IsPriorsServer = serverDirectoryEntry.IsPriorsServer,
                           ExtensionData = extensionData
                       };
         }
    }

    public class DeviceBroker : Broker
    {
        public DeviceBroker(DicomStoreDataContext context)
            : base(context)
		{
		}

        public List<Device> GetDevices()
        {
            return Context.Devices.ToList();
        }

        public Device GetDeviceByName(string name)
        {
            var devices = from d in Context.Devices where d.Name == name select d;
            return devices.SingleOrDefault();
        }

        public List<Device> GetDevicesByAETitle(string aeTitle)
        {
            var devices = from d in Context.Devices where d.AETitle == aeTitle select d;
            return devices.ToList();
        }

        internal void AddDevice(Device device)
        {
            Context.Devices.InsertOnSubmit(device);
        }

        internal void DeleteDevice(Device device)
        {
            Context.Devices.DeleteOnSubmit(device);
        }

        internal void DeleteAllDevices()
        {
            Context.Devices.DeleteAllOnSubmit(Context.Devices);
        }
    }
}

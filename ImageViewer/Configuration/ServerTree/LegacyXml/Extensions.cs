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

using ClearCanvas.Dicom.ServiceModel;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree.LegacyXml
{
    public static class Extensions
    {
        public static ApplicationEntity ToDataContract(this Server server)
        {
            var ae = new ApplicationEntity
                         {
                             Name = server.NameOfServer,
                             AETitle = server.AETitle,
                             Location = server.Location,
                             ScpParameters = new ScpParameters(server.Host, server.Port)
                         };

            if (server.IsStreaming)
                ae.StreamingParameters = new StreamingParameters(server.HeaderServicePort, server.WadoServicePort);

            return ae;
        }

        internal static ServerTreeGroup ToServerTreeGroup(this ServerGroup legacyGroup)
        {
            var group = new ServerTreeGroup(legacyGroup.NameOfGroup);
            foreach (var legacyChildGroup in legacyGroup.ChildGroups)
                group.ChildGroups.Add(legacyChildGroup.ToServerTreeGroup());

            foreach (var legacyChildServer in legacyGroup.ChildServers)
                group.Servers.Add(new ServerTreeDicomServer(legacyChildServer.ToDataContract()));

            return group;
        }
    }
}
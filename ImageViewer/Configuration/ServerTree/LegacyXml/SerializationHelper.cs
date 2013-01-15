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

namespace ClearCanvas.ImageViewer.Configuration.ServerTree.LegacyXml
{
    internal static class SerializationHelper
    {
        private const string _myServersXmlFile = "DicomAEServers.xml";

        private static readonly System.Xml.Serialization.XmlSerializer _serializer
            = new System.Xml.Serialization.XmlSerializer(typeof(ServerTreeRoot),
                                new Type[]
                                    {
                                        typeof (ServerGroup),
                                        typeof (Server),
                                        typeof (List<ServerGroup>),
                                        typeof (List<Server>)
                                    });

        internal static ServerTreeRoot LoadFromXml(string file=null)
        {
            if (String.IsNullOrEmpty(file))
                file = GetServersXmlFileName();

            if (File.Exists(file))
            {
                using (Stream fStream = File.OpenRead(file))
                    return (ServerTreeRoot)_serializer.Deserialize(fStream);
            }

            return null;
        }

        internal static void SaveToXml(ServerTreeRoot root)
        {
			using (Stream fStream = new FileStream(GetServersXmlFileName(), FileMode.Create, FileAccess.Write, FileShare.Read))
			{
			    _serializer.Serialize(fStream, root);
                fStream.Close();
            }
        }

        private static string GetServersXmlFileName()
		{
			return Path.Combine(Platform.InstallDirectory, _myServersXmlFile);
		}
    }
}

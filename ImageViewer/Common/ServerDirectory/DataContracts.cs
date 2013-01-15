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
using System.Linq;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Dicom.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory
{
    public static class ServerDirectoryNamespace
    {
        public const string Value = ImageViewerNamespace.Value + "/serverDirectory";
    }

    #region Get

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class GetServersRequest: DataContractBase
    {
        /// <summary>
        /// Specify an exact name to find one server.
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Name { get; set; }

        /// <summary>
        /// Specify an exact AE Title to find one or more servers.
        /// </summary>
        [DataMember(IsRequired = false)]
        public string AETitle { get; set; }
    }

    /// <summary>
    /// In order to store custom information in <see cref="ServerDirectoryEntry.Data"/>,
    /// decorate your class with both this attribute, as well as the regular <see cref="DataContractAttribute"/>.
    /// </summary>
    public class ServerDataContractAttribute : PolymorphicDataContractAttribute
    {
        public ServerDataContractAttribute(string dataContractGuid)
            : base(dataContractGuid)
        {
        }
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    [KnownType("GetKnownTypes")]
    public class ServerDirectoryEntry : DataContractBase
    {
        public interface IDataTypeProvider
        {
            IEnumerable<Type> GetTypes();
        }

        public sealed class DataTypeProviderExtensionPoint : ExtensionPoint<IDataTypeProvider>{}

        private Dictionary<string, object> _data;

        public ServerDirectoryEntry()
        {
        }

        public ServerDirectoryEntry(ApplicationEntity server)
        {
            Server = server;
        }

        [DataMember(IsRequired = true)]
        public ApplicationEntity Server { get; set; }

        [DataMember(IsRequired = true)]
        public bool IsPriorsServer { get; set; }

        /// <summary>
        /// Used to store additional custom data about a <see cref="Server"/>.
        /// </summary>
        /// <remarks>Custom types must be decorated with both <see cref="ServerDataContractAttribute"/>
        /// and the regular <see cref="DataContractAttribute"/>. It is also recommended that they key
        /// used be the same GUID from the <see cref="ServerDataContractAttribute"/> as the value, in order to avoid collisions.</remarks>
        [DataMember(IsRequired = false)]
        public Dictionary<string, object> Data
        {
            get { return _data ?? (_data = new Dictionary<string, object>()); }
            set { _data = value; }
        }

        public static Type[] GetKnownTypes()
        {
            try
            {
                return new DataTypeProviderExtensionPoint().CreateExtensions().Cast<IDataTypeProvider>().SelectMany(e => e.GetTypes()).ToArray();
            }
            catch (NotSupportedException)
            {
                return new Type[0];
            }
        }
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class GetServersResult : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public List<ServerDirectoryEntry> ServerEntries { get; set; }
    }

    #endregion

    #region Update

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class UpdateServerRequest : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public ServerDirectoryEntry ServerEntry { get; set; }
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class UpdateServerResult : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public ServerDirectoryEntry ServerEntry { get; set; }
    }

    #endregion

    #region Delete

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class DeleteServerRequest : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public ServerDirectoryEntry ServerEntry { get; set; }
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class DeleteServerResult : DataContractBase
    {
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class DeleteAllServersRequest : DataContractBase
    {
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class DeleteAllServersResult : DataContractBase
    {
    }

    #endregion

    #region Add

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class AddServerRequest : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public ServerDirectoryEntry ServerEntry { get; set; }
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class AddServerResult : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public ServerDirectoryEntry ServerEntry { get; set; }
    }

    #endregion

    #region Faults

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class ServerNotFoundFault : DataContractBase
    {
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class ServerExistsFault : DataContractBase
    {
    }

    #endregion
}

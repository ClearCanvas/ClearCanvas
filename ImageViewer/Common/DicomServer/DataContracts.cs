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
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    public static class DicomServerNamespace
    {
        public const string Value = ImageViewerNamespace.Value + "/dicomServer";
    }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class GetDicomServerConfigurationResult
    {
        [DataMember(IsRequired = true)]
        public DicomServerConfiguration Configuration { get; set; }
    }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class GetDicomServerConfigurationRequest
    {}

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class GetDicomServerExtendedConfigurationResult
    {
        [DataMember(IsRequired = true)]
        public DicomServerExtendedConfiguration ExtendedConfiguration { get; set; }
    }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class GetDicomServerExtendedConfigurationRequest
    { }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class UpdateDicomServerConfigurationResult
    {
    }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class UpdateDicomServerConfigurationRequest
    {
        [DataMember(IsRequired = true)]
        public DicomServerConfiguration Configuration { get; set; }
    }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class UpdateDicomServerExtendedConfigurationResult
    {
    }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class UpdateDicomServerExtendedConfigurationRequest
    {
        [DataMember(IsRequired = true)]
        public DicomServerExtendedConfiguration ExtendedConfiguration { get; set; }
    }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class RestartListenerRequest
    { }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class RestartListenerResult
    { }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class GetListenerStateRequest
    { }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class GetListenerStateResult
    {
        [DataMember(IsRequired = true)]
        public ServiceStateEnum State { get; set; }
    }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class DicomServerConfiguration : IEquatable<DicomServerConfiguration>
    {
        [DataMember(IsRequired = true)]
        public string AETitle { get; set; }

        [DataMember(IsRequired = true)]
        public int Port { get; set; }

        [DataMember(IsRequired = true)]
        public string HostName { get; set; }

        public override int GetHashCode()
        {
            int hash = 0x5467912;

            if (AETitle != null)
                hash ^= AETitle.GetHashCode();
            if (HostName != null)
                hash ^= HostName.GetHashCode();

            hash ^= Port.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is DicomServerConfiguration)
                return Equals((DicomServerConfiguration) obj);

            return false;
        }

        #region IEquatable<DicomServerConfiguration> Members

        public bool Equals(DicomServerConfiguration other)
        {
            return AETitle == other.AETitle &&
                   HostName == other.HostName &&
                   Port == other.Port;
        }

        #endregion
    }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class DicomUidDescriptor
    {
        [DataMember(IsRequired = true)]
        public string Uid { get; set; }

        [DataMember(IsRequired = true)]
        public string Description { get; set; }
    }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class DicomServerExtendedConfiguration
    {
        [DataMember(IsRequired = true)]
        public bool QueryResponsesInUtf8 { get; set; }

        [DataMember(IsRequired = true)]
        public bool AllowUnknownCaller { get; set; }

        [DataMember(IsRequired = true)]
        public List<string> StorageTransferSyntaxUids { get; set; }

        [DataMember(IsRequired = true)]
        public List<string> ImageStorageSopClassUids { get; set; }

        [DataMember(IsRequired = true)]
        public List<string> NonImageStorageSopClassUids { get; set; }
    }
}

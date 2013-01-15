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
using System.Runtime.Serialization;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.ServiceModel
{
	public static class DicomNamespace
	{
		public const string Value = "http://www.clearcanvas.ca/dicom";
	}

    [DataContract(Namespace = DicomNamespace.Value)]
    public class ScpParameters : IScpParameters, IEquatable<ScpParameters>
    {
        public ScpParameters()
        {
        }

        public ScpParameters(string hostName, int port)
        {
            HostName = hostName;
            Port = port;
        }

        public ScpParameters(IScpParameters other)
        {
            HostName = other.HostName;
            Port = other.Port;
        }

        #region IScpParameters Members

        [DataMember(IsRequired = true)]
        public string HostName { get; set; }
        [DataMember(IsRequired = true)]
        public int Port { get; set; }

        #endregion

        public override string ToString()
        {
            return String.Format("HostName: {0}\nPort: {1}",HostName, Port);
        }

        public override bool Equals(object obj)
        {
            if (obj is ScpParameters)
                return Equals((ScpParameters)obj);

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hash = 0x2367589;
            hash ^= HostName.GetHashCode();
            hash ^= Port.GetHashCode();
            return hash;
        }

        #region IEquatable<ScpParameters> Members

        public bool Equals(ScpParameters other)
        {
            return (HostName ?? "") == (other.HostName ?? "") && Port == other.Port;
        }

        #endregion
    }

    [DataContract(Namespace = DicomNamespace.Value)]
    public class StreamingParameters : IStreamingParameters, IEquatable<StreamingParameters>
    {
        public StreamingParameters()
        {
        }

        public StreamingParameters(int headerServicePort, int wadoServicePort)
        {
            HeaderServicePort = headerServicePort;
            WadoServicePort = wadoServicePort;
        }

        public StreamingParameters(IStreamingParameters other)
        {
            HeaderServicePort = other.HeaderServicePort;
            WadoServicePort = other.WadoServicePort;
        }

        #region IScpParameters Members

        [DataMember(IsRequired = true)]
        public int HeaderServicePort { get; set; }
        [DataMember(IsRequired = true)]
        public int WadoServicePort { get; set; }

        #endregion

        public override string ToString()
        {
            return String.Format("HeaderServicePort: {0}\nWadoServicePort: {1}", HeaderServicePort, WadoServicePort);
        }

        public override bool Equals(object obj)
        {
            if (obj is StreamingParameters)
                return Equals((StreamingParameters)obj);

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hash = 0x24621789;
            hash ^= HeaderServicePort.GetHashCode();
            hash ^= WadoServicePort.GetHashCode();
            return hash;
        }

        #region IEquatable<StreamingParameters> Members

        public bool Equals(StreamingParameters other)
        {
            return HeaderServicePort == other.HeaderServicePort && WadoServicePort == other.WadoServicePort;
        }

        #endregion
    }

    [DataContract(Namespace = DicomNamespace.Value)]
    public class ApplicationEntity : IApplicationEntity, IEquatable<ApplicationEntity>
	{
		public ApplicationEntity()
		{}

        public ApplicationEntity(IApplicationEntity other)
        {
            Name = other.Name;
            AETitle = other.AETitle;
            Description = other.Description;
            Location = other.Location;
            
            if (other.ScpParameters != null)
                ScpParameters = new ScpParameters(other.ScpParameters);
            if (other.StreamingParameters != null)
                StreamingParameters = new StreamingParameters(other.StreamingParameters);
        }

		#region IApplicationEntity Members

		[DataMember(IsRequired = true)]
		public string AETitle { get; set; }

		[DataMember(IsRequired = false)]
		public string Name { get; set; }

		[DataMember(IsRequired = false)]
		public string Description { get; set; }

		[DataMember(IsRequired = false)]
		public string Location { get; set; }

        public bool SupportsStreaming { get { return StreamingParameters != null; } }

        [DataMember(IsRequired = false)]
        public ScpParameters ScpParameters { get; set; }

        [DataMember(IsRequired = false)]
        public StreamingParameters StreamingParameters { get; set; }

        IScpParameters IApplicationEntity.ScpParameters { get { return ScpParameters; } }
        IStreamingParameters IApplicationEntity.StreamingParameters { get { return StreamingParameters; } }

        public override string ToString()
        {
            return String.Format("Name: {0}\nAE: {1}\nDescription: {2}\nLocation:{3}\n{4}\n{5}",
                                 Name, AETitle, Description, Location , ScpParameters, StreamingParameters);
        }

		#endregion

        public override bool Equals(object obj)
        {
            if (obj is ApplicationEntity)
                return Equals((ApplicationEntity)obj);

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hash = 0x3568932;
            hash ^= AETitle.GetHashCode();
            hash ^= Name.GetHashCode();
            hash ^= Description.GetHashCode();
            hash ^= Location.GetHashCode();
            return hash;
        }

        #region IEquatable<ApplicationEntity> Members

        public bool Equals(ApplicationEntity other)
        {
            return (AETitle ?? "") == (other.AETitle ?? "") &&
                   (Name ?? "") == (other.Name ?? "") &&
                   (Description ?? "") == (other.Description ?? "") &&
                   (Location ?? "") == (other.Location ?? "") &&
                   Equals(ScpParameters, other.ScpParameters) &&
                   Equals(StreamingParameters, other.StreamingParameters);
        }

        #endregion
    }
}

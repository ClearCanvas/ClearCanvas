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

using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel
{
    [DataContract(Namespace = DicomNamespace.Value)]
	public class UnknownDestinationAEFault
	{
		public UnknownDestinationAEFault()
		{}
	}

    [DataContract(Namespace = DicomNamespace.Value)]
	public class UnknownCalledAEFault
	{
		public UnknownCalledAEFault()
		{ }
	}

    [DataContract(Namespace = DicomNamespace.Value)]
	public class UnknownCallingAEFault
	{
		public UnknownCallingAEFault()
		{ }
	}

    [DataContract(Namespace = DicomNamespace.Value)]
	public class UnknownSourceAEFault
	{
		public UnknownSourceAEFault()
		{ }
	}

    [DataContract(Namespace = DicomNamespace.Value)]
	public class StudyOfflineFault
	{
		public StudyOfflineFault()
		{ }
	}

    [DataContract(Namespace = DicomNamespace.Value)]
	public class StudyInUseFault
	{
		public StudyInUseFault()
		{ }
	}

    [DataContract(Namespace = DicomNamespace.Value)]
	public class StudyNearlineFault
	{
		public StudyNearlineFault()
		{ }

		[DataMember(IsRequired = false)]
		public bool IsStudyBeingRestored { get; set; }
	}

    [DataContract(Namespace = DicomNamespace.Value)]
	public class StudyNotFoundFault
	{
		public StudyNotFoundFault()
		{ }
	}

    [DataContract(Namespace = DicomNamespace.Value)]
    public class SeriesNotFoundFault
    {
        public SeriesNotFoundFault()
        { }
    }

	[DataContract(Namespace = DicomNamespace.Value)]
	public class CodecFault
	{
		public CodecFault()
		{ }

		public CodecFault(string transferSyntaxUid)
		{
			TransferSyntaxUid = transferSyntaxUid;
		}

		[DataMember(IsRequired = true)]
		public string TransferSyntaxUid { get; set; }
	}
}

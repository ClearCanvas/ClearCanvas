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
using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
	[ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName = "IStudyLocator", Namespace = StudyManagementNamespace.Value)]
	public interface IStudyLocator
	{
		LocateStudiesResult LocateStudies(LocateStudiesRequest request);
		LocateSeriesResult LocateSeries(LocateSeriesRequest request);
		LocateImagesResult LocateImages(LocateImagesRequest request);
	}

	[DataContract(Namespace = StudyManagementNamespace.Value)]
	public class LocateFailureInfo : DataContractBase
	{
		[DataMember(IsRequired = false)]
		public QueryFailedFault Fault { get; set; }

		[DataMember(IsRequired = false)]
		public string Description { get; set; }

		[DataMember(IsRequired = false)]
		public string ServerAE { get; set; }

		[DataMember(IsRequired = false)]
		public string ServerName { get; set; }

		public LocateFailureInfo() {}

		public LocateFailureInfo(QueryFailedFault fault, string faultDescription)
		{
			Fault = fault;
			Description = faultDescription;
		}
	}

	[DataContract(Namespace = StudyManagementNamespace.Value)]
	public class LocateStudiesRequest : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public StudyRootStudyIdentifier Criteria { get; set; }
	}

	[DataContract(Namespace = StudyManagementNamespace.Value)]
	public class LocateStudiesResult : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public IList<StudyRootStudyIdentifier> Studies { get; set; }

		[DataMember(IsRequired = false)]
		public IList<LocateFailureInfo> Failures { get; set; }
	}

	[DataContract(Namespace = StudyManagementNamespace.Value)]
	public class LocateSeriesRequest : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public SeriesIdentifier Criteria { get; set; }
	}

	[DataContract(Namespace = StudyManagementNamespace.Value)]
	public class LocateSeriesResult : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public IList<SeriesIdentifier> Series { get; set; }

		[DataMember(IsRequired = false)]
		public IList<LocateFailureInfo> Failures { get; set; }
	}

	[DataContract(Namespace = StudyManagementNamespace.Value)]
	public class LocateImagesRequest : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public ImageIdentifier Criteria { get; set; }
	}

	[DataContract(Namespace = StudyManagementNamespace.Value)]
	public class LocateImagesResult : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public IList<ImageIdentifier> Images { get; set; }

		[DataMember(IsRequired = false)]
		public IList<LocateFailureInfo> Failures { get; set; }
	}
}
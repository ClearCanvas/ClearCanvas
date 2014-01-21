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

namespace ClearCanvas.Dicom.ServiceModel.Streaming
{
	/// <summary>
	/// Encapsulates the parameters passed by the client to header streaming service.
	/// </summary>
	/// <remarks>
	/// <see cref="HeaderStreamingParameters"/> is passed to the service that implements <see cref="IHeaderStreamingService"/> 
	/// when the client wants to retrieve header information of a study. The study is identified by the <see cref="StudyInstanceUID"/>
	/// and the <see cref="ServerAETitle"/> where it is located.
	/// </remarks>
	[DataContract]
	public class HeaderStreamingParameters
	{
		#region Private members

		private string _serverAETitle;
		private string _studyInstanceUID;
		private string _referenceID;
		private bool _ignoreInUse;

		#endregion Private members

		#region Public Properties

		/// <summary>
		/// Study instance UID of the study whose header will be retrieved.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string StudyInstanceUID
		{
			get { return _studyInstanceUID; }
			set { _studyInstanceUID = value; }
		}

		/// <summary>
		/// AE title of the server where the study is located.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string ServerAETitle
		{
			get { return _serverAETitle; }
			set { _serverAETitle = value; }
		}

		/// <summary>
		/// A ticket for tracking purposes.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string ReferenceID
		{
			get { return _referenceID; }
			set { _referenceID = value; }
		}

		/// <summary>
		/// Indicates if the study loader should attempt to ignore if the study is in use
		/// and try to load the study anyway.
		/// </summary>
		[DataMember(IsRequired = false /* For backward-compatibility */)]
		public bool IgnoreInUse
		{
			get { return _ignoreInUse; }
			set { _ignoreInUse = value; }
		}

		#endregion Public Properties
	}
}
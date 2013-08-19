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
using System.IO;
using System.ServiceModel;

namespace ClearCanvas.Dicom.ServiceModel.Streaming
{
	/// <summary>
	/// Defines the interface of a service that provides study header information.
	/// </summary>
	[ServiceContract]
	public interface IHeaderStreamingService
	{
		/// <summary>
		/// Retrieves a stream containing the study header information.
		/// </summary>
		/// <param name="callingAETitle">The AE of the caller</param>
		/// <param name="parameters">Query parameters</param>
		/// <returns>The stream containing the study header information in compressed XML format</returns>
		/// <seealso cref="HeaderStreamingParameters"></seealso>
		[OperationContract]
		[FaultContract(typeof (StudyIsInUseFault))]
		[FaultContract(typeof (StudyIsNearlineFault))]
		[FaultContract(typeof (StudyNotFoundFault))]
		[FaultContract(typeof (string))]
		Stream GetStudyHeader(string callingAETitle, HeaderStreamingParameters parameters);
	}
}

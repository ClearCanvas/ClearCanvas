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

using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    /// <summary>
	/// WCF service interface to the local DICOM server.
	/// </summary>
	[ServiceContract(ConfigurationName = "IDicomServer")]
	public interface IDicomServer
	{
        //TODO (Marmot): Add Start/Stop?
        /// <summary>
        /// Gets the current state of the local DICOM listener.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetListenerStateResult GetListenerState(GetListenerStateRequest request);

        /// <summary>
        /// Requests that the DICOM listener be restarted. Would normally be called
        /// after a server configuration change via <see cref="IDicomServerConfiguration.UpdateConfiguration"/>.
        /// </summary>
        [OperationContract]
        RestartListenerResult RestartListener(RestartListenerRequest request);
	}
}

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
using System.ServiceModel;
using ClearCanvas.Dicom.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.Automation
{
	/// <summary>
	/// Service contract for automation of the viewer.
	/// </summary>
	[ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName="IViewerAutomation", Namespace = AutomationNamespace.Value)]
	public interface IViewerAutomation
	{
        [OperationContract(IsOneWay = false)]
        [FaultContract(typeof(OpenFilesFault))]
        OpenFilesResult OpenFiles(OpenFilesRequest request);

        [OperationContract(IsOneWay = false)]
        [FaultContract(typeof(NoViewersFault))]
        GetViewersResult GetViewers(GetViewersRequest request);
        
        /// <summary>
		/// Gets all active <see cref="Viewer"/>s.
		/// </summary>
		/// <exception cref="FaultException{NoActiveViewersFault}">Thrown if there are no active <see cref="Viewer"/>s.</exception>
		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(NoActiveViewersFault))]
        [Obsolete("Use GetViewers instead.")]
		GetActiveViewersResult GetActiveViewers();

		/// <summary>
		/// Gets information about the given <see cref="Viewer"/>.
		/// </summary>
		/// <exception cref="FaultException{ViewerNotFoundFault}">Thrown if the given <see cref="Viewer"/> no longer exists.</exception>
		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(ViewerNotFoundFault))]
		GetViewerInfoResult GetViewerInfo(GetViewerInfoRequest request);

		/// <summary>
		/// Opens the requested studies in a <see cref="Viewer"/>.
		/// </summary>
		/// <exception cref="FaultException{OpenStudiesFault}">Thrown if the primary study could not be opened.</exception>
		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(StudyNotFoundFault))]
		[FaultContract(typeof(StudyOfflineFault))]
		[FaultContract(typeof(StudyNearlineFault))]
		[FaultContract(typeof(StudyInUseFault))]
		[FaultContract(typeof(OpenStudiesFault))]
		OpenStudiesResult OpenStudies(OpenStudiesRequest request);

		/// <summary>
		/// Activates the given <see cref="Viewer"/>.
		/// </summary>
		/// <exception cref="FaultException{ViewerNotFoundFault}">Thrown if the given <see cref="Viewer"/> no longer exists.</exception>
		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(ViewerNotFoundFault))]
		void ActivateViewer(ActivateViewerRequest request);

		/// <summary>
		/// Closes the given <see cref="Viewer"/>.
		/// </summary>
		/// <exception cref="FaultException{ViewerNotFoundFault}">Thrown if the given <see cref="Viewer"/> no longer exists.</exception>
		[OperationContract(IsOneWay = false)]
		[FaultContract(typeof(ViewerNotFoundFault))]
		void CloseViewer(CloseViewerRequest request);
	}
}
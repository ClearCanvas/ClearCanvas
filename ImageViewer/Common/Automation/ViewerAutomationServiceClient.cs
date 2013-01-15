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
using System.ServiceModel.Channels;

namespace ClearCanvas.ImageViewer.Common.Automation
{
	public class ViewerAutomationServiceClient : ClientBase<IViewerAutomation>, IViewerAutomation
	{
		public ViewerAutomationServiceClient()
		{
		}

		public ViewerAutomationServiceClient(string endpointConfigurationName)
			: base(endpointConfigurationName)
		{
		}

		public ViewerAutomationServiceClient(Binding binding, EndpointAddress remoteAddress)
			: base(binding, remoteAddress)
		{
		}

		public ViewerAutomationServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
			: base(endpointConfigurationName, remoteAddress)
		{
		}

		#region IViewerAutomation Members

        public GetViewersResult GetViewers(GetViewersRequest request)
        {
            return base.Channel.GetViewers(request);
        }

		[Obsolete("Use GetViewers instead.")]
		public GetActiveViewersResult GetActiveViewers()
		{
			return base.Channel.GetActiveViewers();
		}

		public GetViewerInfoResult GetViewerInfo(GetViewerInfoRequest request)
		{
			return base.Channel.GetViewerInfo(request);
		}

        public OpenFilesResult OpenFiles(OpenFilesRequest request)
        {
            return base.Channel.OpenFiles(request);
        }

		public OpenStudiesResult OpenStudies(OpenStudiesRequest request)
		{
			return base.Channel.OpenStudies(request);
		}

		public void ActivateViewer(ActivateViewerRequest request)
		{
			base.Channel.ActivateViewer(request);
		}

		public void CloseViewer(CloseViewerRequest request)
		{
			base.Channel.CloseViewer(request);
		}

		#endregion
	}
}

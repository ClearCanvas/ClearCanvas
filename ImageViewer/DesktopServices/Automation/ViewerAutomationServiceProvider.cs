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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.Automation;

namespace ClearCanvas.ImageViewer.DesktopServices.Automation
{
	[ExtensionOf(typeof(ServiceProviderExtensionPoint))]
	public class ViewerAutomationServiceProvider : IServiceProvider
	{
		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IViewerAutomation))
				return new ViewerAutomationProxy();

			return null;
		}

		#endregion
	}

	internal class ViewerAutomationProxy : IViewerAutomation, IDisposable
	{
		#region IViewerAutomation Members

		[Obsolete("Use GetViewers instead.")]
		public GetActiveViewersResult GetActiveViewers()
		{
			// Done for reasons of speed, as well as the fact that a call to the service from the same thread
			// that the service is hosted on (the main UI thread) will cause a deadlock.
			if (SynchronizationContext.Current == ViewerAutomationServiceHostTool.HostSynchronizationContext)
			{
				return new ViewerAutomation().GetActiveViewers();
			}
			else
			{
				using (ViewerAutomationServiceClient client = new ViewerAutomationServiceClient())
				{
					return client.GetActiveViewers();
				}
			} 
		}

        public GetViewersResult GetViewers(GetViewersRequest request)
        {
            // Done for reasons of speed, as well as the fact that a call to the service from the same thread
            // that the service is hosted on (the main UI thread) will cause a deadlock.
            if (SynchronizationContext.Current == ViewerAutomationServiceHostTool.HostSynchronizationContext)
            {
                return new ViewerAutomation().GetViewers(request);
            }
            else
            {
                using (ViewerAutomationServiceClient client = new ViewerAutomationServiceClient())
                {
                    return client.GetViewers(request);
                }
            }
        }

		public GetViewerInfoResult GetViewerInfo(GetViewerInfoRequest request)
		{
			// Done for reasons of speed, as well as the fact that a call to the service from the same thread
			// that the service is hosted on (the main UI thread) will cause a deadlock.
			if (SynchronizationContext.Current == ViewerAutomationServiceHostTool.HostSynchronizationContext)
			{
				return new ViewerAutomation().GetViewerInfo(request);
			}
			else
			{
				using (ViewerAutomationServiceClient client = new ViewerAutomationServiceClient())
				{
					return client.GetViewerInfo(request);
				}
			} 
		}

		public OpenStudiesResult OpenStudies(OpenStudiesRequest request)
		{
			// Done for reasons of speed, as well as the fact that a call to the service from the same thread
			// that the service is hosted on (the main UI thread) will cause a deadlock.
			if (SynchronizationContext.Current == ViewerAutomationServiceHostTool.HostSynchronizationContext)
			{
				return new ViewerAutomation().OpenStudies(request);
			}
			else
			{
				using (ViewerAutomationServiceClient client = new ViewerAutomationServiceClient())
				{
					return client.OpenStudies(request);
				}
			} 
		}

        public OpenFilesResult OpenFiles(OpenFilesRequest request)
        {
            // Done for reasons of speed, as well as the fact that a call to the service from the same thread
            // that the service is hosted on (the main UI thread) will cause a deadlock.
            if (SynchronizationContext.Current == ViewerAutomationServiceHostTool.HostSynchronizationContext)
            {
                return new ViewerAutomation().OpenFiles(request);
            }
            else
            {
                using (ViewerAutomationServiceClient client = new ViewerAutomationServiceClient())
                {
                    return client.OpenFiles(request);
                }
            }
        }

		public void ActivateViewer(ActivateViewerRequest request)
		{
			// Done for reasons of speed, as well as the fact that a call to the service from the same thread
			// that the service is hosted on (the main UI thread) will cause a deadlock.
			if (SynchronizationContext.Current == ViewerAutomationServiceHostTool.HostSynchronizationContext)
			{
				new ViewerAutomation().ActivateViewer(request);
			}
			else
			{
				using (ViewerAutomationServiceClient client = new ViewerAutomationServiceClient())
				{
					client.ActivateViewer(request);
				}
			} 
		}

		public void CloseViewer(CloseViewerRequest request)
		{
			// Done for reasons of speed, as well as the fact that a call to the service from the same thread
			// that the service is hosted on (the main UI thread) will cause a deadlock.
			if (SynchronizationContext.Current == ViewerAutomationServiceHostTool.HostSynchronizationContext)
			{
				new ViewerAutomation().CloseViewer(request);
			}
			else
			{
				using (ViewerAutomationServiceClient client = new ViewerAutomationServiceClient())
				{
					client.CloseViewer(request);
				}
			} 
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
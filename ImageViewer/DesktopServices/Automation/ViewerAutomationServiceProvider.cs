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
			return Execute(a => a.GetActiveViewers());
		}

        public GetViewersResult GetViewers(GetViewersRequest request)
        {
			return Execute(a => a.GetViewers(request));
        }

		public GetViewerInfoResult GetViewerInfo(GetViewerInfoRequest request)
		{
			return Execute(a => a.GetViewerInfo(request));
		}

		public OpenStudiesResult OpenStudies(OpenStudiesRequest request)
		{
			return Execute(a => a.OpenStudies(request));
		}

        public OpenFilesResult OpenFiles(OpenFilesRequest request)
        {
			return Execute(a => a.OpenFiles(request));
        }

		public void ActivateViewer(ActivateViewerRequest request)
		{
			Execute(a => a.ActivateViewer(request));
		}

		public void CloseViewer(CloseViewerRequest request)
		{
			Execute(a => a.CloseViewer(request));
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion

		private static void Execute(Action<IViewerAutomation> action)
		{
			// Done for reasons of speed, as well as the fact that a call to the service from the same thread
			// that the service is hosted on (the main UI thread) will cause a deadlock.
			if (SynchronizationContext.Current == ViewerAutomationServiceHostTool.HostSynchronizationContext)
			{
				action(new ViewerAutomation());
			}
			else
			{
				ViewerAutomationServiceHostTool.HostSynchronizationContext.Send(s => action(new ViewerAutomation()), null);
			}
		}

		private static TResult Execute<TResult>(Func<IViewerAutomation, TResult> action)
		{
			// Done for reasons of speed, as well as the fact that a call to the service from the same thread
			// that the service is hosted on (the main UI thread) will cause a deadlock.
			if (SynchronizationContext.Current == ViewerAutomationServiceHostTool.HostSynchronizationContext)
			{
				return action(new ViewerAutomation());
			}
			else
			{
				var response = default(TResult);
				ViewerAutomationServiceHostTool.HostSynchronizationContext.Send(s =>
				{
					response = action(new ViewerAutomation());
				}, null);
				return response;
			}
		}
	}
}
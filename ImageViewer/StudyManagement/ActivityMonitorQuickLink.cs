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

using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public enum ActivityMonitorQuickLink
	{
		SystemConfiguration,
		LocalStorageConfiguration,
		StudyManagementRules
	}

	public static class ActivityMonitorQuickLinkExtensions
	{
		public static bool CanInvoke(this ActivityMonitorQuickLink link)
		{
			var handlers = (new ActivityMonitorQuickLinkHandlerExtensionPoint()).CreateExtensions().Cast<IActivityMonitorQuickLinkHandler>();
			return handlers.Any(h => h.CanHandle(link));
		}

		public static void Invoke(this ActivityMonitorQuickLink link, IDesktopWindow desktopWindow)
		{
			var handlers = (new ActivityMonitorQuickLinkHandlerExtensionPoint()).CreateExtensions().Cast<IActivityMonitorQuickLinkHandler>();
			var handler = handlers.FirstOrDefault(h => h.CanHandle(link));
			if (handler != null)
			{
				handler.Handle(link, desktopWindow);
			}
		}
	}


	[ExtensionPoint]
	public sealed class ActivityMonitorQuickLinkHandlerExtensionPoint : ExtensionPoint<IActivityMonitorQuickLinkHandler> { }

	public interface IActivityMonitorQuickLinkHandler
	{
		/// <summary>
		/// Asks handler if it can handle the specified link.
		/// </summary>
		/// <param name="link"></param>
		/// <returns></returns>
		bool CanHandle(ActivityMonitorQuickLink link);


		/// <summary>
		/// Asks handler to handle the specified link, returning true if handled.
		/// </summary>
		/// <param name="link"></param>
		/// <param name="window"> </param>
		/// <returns></returns>
		void Handle(ActivityMonitorQuickLink link, IDesktopWindow window);
	}
}

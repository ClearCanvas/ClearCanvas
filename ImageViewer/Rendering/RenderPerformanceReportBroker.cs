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
namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// Allows Renderers to publish the total time elapsed for a particular method to aid in 
	/// debugging and optimization.
	/// </summary>
	[Obsolete("Use PerformanceReportBroker instead.")]
	public static class RenderPerformanceReportBroker
	{
		/// <summary>
		/// A Delegate for publishing performance of a method.
		/// </summary>
		public delegate void PerformanceReportDelegate(string methodName, double totalTime);

		/// <summary>
		/// A Delegate that can be subscribed to in order to receive performance reports.
		/// </summary>
		public static event PerformanceReportDelegate PerformanceReport;

		/// <summary>
		/// Called from within a method to publish performance reports to subscribers.
		/// </summary>
		public static void PublishPerformanceReport(string methodName, double totalTime)
		{
			if (PerformanceReport == null)
				return;

			PerformanceReport(methodName, totalTime);
		}
	}
}

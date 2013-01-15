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

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Used to report performance statistics via the <see cref="PerformanceReportBroker"/>.
	/// </summary>
	public class PerformanceReport
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public PerformanceReport(string category, string identifier, TimeSpan totalTime)
		{
			Category = category;
			Identifier = identifier;
			TotalTime = totalTime;
		}

		/// <summary>
		/// The Category.
		/// </summary>
		public readonly string Category;

		/// <summary>
		/// The Identifier.
		/// </summary>
		public readonly string Identifier;

		/// <summary>
		/// The total time taken to perform the operation.
		/// </summary>
		public readonly TimeSpan TotalTime;
	}

	/// <summary>
	/// A generic way to publish events.
	/// </summary>
	/// <remarks>
	/// NOTE: Should be used for debugging purposes only.
	/// </remarks>
	public static class PerformanceReportBroker
	{
		private static readonly object _syncLock = new object();
		private static event EventHandler<ItemEventArgs<PerformanceReport>> _report;

		/// <summary>
		/// The <see cref="PerformanceReportBroker.Report"/> event delegate.
		/// </summary>
		public delegate void ReportDelegate(PerformanceReport reportItem);

		/// <summary>
		/// The event that is fired as reports are published.
		/// </summary>
		public static event EventHandler<ItemEventArgs<PerformanceReport>> Report
		{
			add
			{
				lock (_syncLock)
				{
					_report += value;
				}
			}
			remove
			{
				lock (_syncLock)
				{
					_report -= value;
				}
			}
		}

		/// <summary>
		/// Publishes a <see cref="PerformanceReport"/>.
		/// </summary>
		public static void PublishReport(string category, string identifier, double totalTimeSeconds)
		{
			PublishReport(new PerformanceReport(category, identifier, TimeSpan.FromSeconds(totalTimeSeconds)));
		}

		/// <summary>
		/// Publishes a <see cref="PerformanceReport"/>.
		/// </summary>
		public static void PublishReport(string category, string identifier, TimeSpan totalTime)
		{
			PublishReport(new PerformanceReport(category, identifier, totalTime));
		}

		/// <summary>
		/// Publishes a <see cref="PerformanceReport"/>.
		/// </summary>
		public static void PublishReport(PerformanceReport reportItem)
		{
			PublishReport(null, reportItem);	
		}

		/// <summary>
		/// Publishes a <see cref="PerformanceReport"/>.
		/// </summary>
		public static void PublishReport(object sender, PerformanceReport reportItem)
		{
			lock (_syncLock)
			{
				if (_report == null)
					return;

				_report(sender, new ItemEventArgs<PerformanceReport>(reportItem));
			}
		}
	}
}

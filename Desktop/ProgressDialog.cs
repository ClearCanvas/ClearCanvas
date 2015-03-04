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
using System.Collections.Generic;
using System.ComponentModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Copied from System.Windows.Forms.ProgressBarStyle so we don't need dependency from System.Windows.Forms.
	/// </summary>
	public enum ProgressBarStyle
	{
		/// <summary>
		/// The progress bar appears as block segments.
		/// </summary>
		Blocks = 0,

		/// <summary>
		/// The progress bar is continuous, not blocky.
		/// </summary>
		Continuous = 1,

		/// <summary>
		/// Marquee mode, doesn't actually show progress, just activity.
		/// </summary>
		Marquee = 2,
	}

	/// <summary>
	/// Contains static methods used to show a progress dialog box.
	/// </summary>
	public static class ProgressDialog
	{
		/// <summary>
		/// Show the progress dialog to to the user.
		/// </summary>
		/// <param name="task">The <see cref="BackgroundTask"/> to execute.</param>
		/// <param name="desktopWindow">Desktop window that parents the progress dialog.</param>
		/// <param name="autoClose">Close the progress dialog after task completion.</param>
		/// <param name="progressBarStyle">The style of the progress bar.</param>
		/// <param name="startProgressMessage">Initial progress message to display on the dialog.</param>
		public static void Show(BackgroundTask task, IDesktopWindow desktopWindow, bool autoClose = true, ProgressBarStyle progressBarStyle = ProgressBarStyle.Blocks, [param : Localizable(true)] string startProgressMessage = null)
		{
			// as the progress dialog involves UI, the task should *always* be run under the application UI culture
			task.ThreadUICulture = Application.CurrentUICulture;

			var progressComponent = new ProgressDialogComponent(task, autoClose, progressBarStyle, startProgressMessage);
			ApplicationComponentExitCode result = ApplicationComponent.LaunchAsDialog(
				desktopWindow,
				progressComponent,
				Application.Name);

			if (result == ApplicationComponentExitCode.Error)
				throw progressComponent.TaskException;
		}

		/// <summary>
		/// Shows a progress dialog that processes the specified list of items on a background task.
		/// </summary>
		/// <remarks>
		/// This is essentially a convenience method for processing a list of items.  It creates a background task internally.
		/// </remarks>
		/// <typeparam name="T"></typeparam>
		/// <param name="desktopWindow"></param>
		/// <param name="items"></param>
		/// <param name="processor"></param>
		/// <param name="cancelable"></param>
		/// <returns></returns>
		public static int Show<T>(IDesktopWindow desktopWindow, IList<T> items, Func<T, int, string> processor, bool cancelable)
		{
			var totalItems = items.Count;
			var processedCount = 0;
			var task = new BackgroundTask(
				context =>
					{
						try
						{
							foreach (var item in items)
							{
								if (context.CancelRequested)
								{
									context.Cancel();
									return;
								}

								var msg = processor(item, processedCount);
								context.ReportProgress(new BackgroundTaskProgress(processedCount, totalItems, msg));
								processedCount++;
							}
							context.Complete();
						}
						catch (Exception e)
						{
							context.Error(e);
						}
					}, cancelable);

			//note: any exceptions occurring on the background task will be re-thrown from this call
			Show(task, desktopWindow, true);

			return processedCount;
		}
	}
}
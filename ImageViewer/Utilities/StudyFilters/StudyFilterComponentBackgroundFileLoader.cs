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
using System.IO;
using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public partial class StudyFilterComponent
	{
		public bool Load(IDesktopWindow desktopWindow, params string[] paths)
		{
			return Load(desktopWindow, true, paths, true);
		}

		public bool Load(IDesktopWindow desktopWindow, bool allowCancel, params string[] paths)
		{
			return Load(desktopWindow, allowCancel, paths, true);
		}

		public bool Load(IDesktopWindow desktopWindow, IEnumerable<string> paths)
		{
			return Load(desktopWindow, paths, true);
		}

		public bool Load(IDesktopWindow desktopWindow, IEnumerable<string> paths, bool recursive)
		{
			return Load(desktopWindow, true, paths, recursive);
		}

		public bool Load(IDesktopWindow desktopWindow, bool allowCancel, IEnumerable<string> paths)
		{
			return Load(desktopWindow, allowCancel, paths, true);
		}

		public bool Load(IDesktopWindow desktopWindow, bool allowCancel, IEnumerable<string> paths, bool recursive)
		{
			return BackgroundFileLoader.Load(this, desktopWindow, allowCancel, paths, recursive);
		}

		private static class BackgroundFileLoader
		{
			public static bool Load(StudyFilterComponent component, IDesktopWindow desktopWindow, bool allowCancel, IEnumerable<string> paths, bool recursive)
			{
				BackgroundTaskTerminatedEventArgs evArgs = null;

				try
				{
					using (var task = new BackgroundTask(LoadWorker, allowCancel, new State(component, paths, recursive)))
					{
						task.Terminated += (s, e) => evArgs = e;
						ProgressDialog.Show(task, desktopWindow, true, ProgressBarStyle.Continuous);
					}
				}
				catch (Exception ex)
				{
					ExceptionHandler.Report(ex, desktopWindow);
					return false;
				}

				if (evArgs.Reason == BackgroundTaskTerminatedReason.Exception && evArgs.Exception != null)
					ExceptionHandler.Report(evArgs.Exception, desktopWindow);
				return evArgs.Reason == BackgroundTaskTerminatedReason.Completed;
			}

			private static void LoadWorker(IBackgroundTaskContext context)
			{
				State state = context.UserState as State;
				if (state == null)
				{
					context.Cancel();
					return;
				}

				context.ReportProgress(new BackgroundTaskProgress(0, 1000, SR.MessageLoading));
				if (context.CancelRequested)
				{
					context.Cancel();
					return;
				}

				List<string> fileList = new List<string>();
				try
				{
					foreach (string path in state.Paths)
						fileList.AddRange(EnumerateFiles(path, state.Recursive));
				}
				catch (Exception ex)
				{
					context.Error(ex);
					return;
				}

				for (int n = 0; n < fileList.Count; n++)
				{
					context.ReportProgress(new BackgroundTaskProgress(n, fileList.Count, SR.MessageLoading));
					if (context.CancelRequested)
					{
						context.Cancel();
						return;
					}
					state.SynchronizationContext.Send(i => state.Component.Load(fileList[(int) i]), n);
				}

				if (context.CancelRequested)
				{
					context.Cancel();
					return;
				}

				context.Complete();
			}

			private static IEnumerable<string> EnumerateFiles(string path, bool recurse)
			{
				if (File.Exists(path))
				{
					yield return path;
				}
				if (recurse && Directory.Exists(path))
				{
					foreach (string directory in Directory.GetDirectories(path))
						foreach (string filename in EnumerateFiles(directory, true))
							yield return filename;
					foreach (string filename in Directory.GetFiles(path))
						yield return filename;
				}
			}

			private class State
			{
				public readonly StudyFilterComponent Component;
				public readonly IEnumerable<string> Paths;
				public readonly SynchronizationContext SynchronizationContext;
				public readonly bool Recursive;

				public State(StudyFilterComponent component, IEnumerable<string> paths, bool recursive)
				{
					this.Component = component;
					this.Paths = paths;
					this.SynchronizationContext = SynchronizationContext.Current;
					this.Recursive = recursive;
				}
			}
		}
	}
}
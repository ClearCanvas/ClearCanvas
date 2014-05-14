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
using System.Globalization;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Provides a simple mechanism for executing code asynchronously.
	/// </summary>
	public class AsyncTask : IDisposable
	{
		public delegate void Action();

		private BackgroundTask _backgroundTask;
		private Action _continuationCode;
		private Action<Exception> _errorHandler;

		public AsyncTask()
		{
			// default ThreadUICulture to the value of the desktop application
			ThreadUICulture = Desktop.Application.CurrentUICulture;
		}

		/// <summary>
		/// Runs specified code asynchronously, executing the continuation code when the asynchronous code completes.
		/// </summary>
		/// <remarks>
		/// The <paramref name="asyncCode"/> block is executed on the thread pool.  When this block completes, the
		/// <paramref name="continuationCode"/> block is executed on the calling thread.  If an exception is thrown
		/// in the async block, the exeception is logged and the continuation code is not executed.
		/// This method returns immediately to the caller.  Subsequent calls to this method will cause any pending
		/// prior call to be effectively abandoned.
		/// </remarks>
		/// <param name="asyncCode"></param>
		/// <param name="continuationCode"></param>
		public void Run(Action asyncCode, Action continuationCode)
		{
			Run(asyncCode, continuationCode, DefaultErrorHandler);
		}

		/// <summary>
		/// Runs specified code asynchronously, executing the continuation code when the asynchronous code completes.
		/// </summary>
		/// <remarks>
		/// The <paramref name="asyncCode"/> block is executed on the thread pool.  When this block completes, the
		/// <paramref name="continuationCode"/> block is executed on the calling thread.  If an exception is thrown
		/// in the async block, the <paramref name="errorHandler"/> is executed instead of the continuation block.
		/// This method returns immediately to the caller.  Subsequent calls to this method will cause any pending
		/// prior call to be effectively abandoned.
		/// </remarks>
		/// <param name="asyncCode"></param>
		/// <param name="continuationCode"></param>
		/// <param name="errorHandler"></param>
		public void Run(Action asyncCode, Action continuationCode, Action<Exception> errorHandler)
		{
			// clear any previous task
			Cancel();

			_continuationCode = continuationCode;
			_errorHandler = errorHandler;

			_backgroundTask = new BackgroundTask(
				delegate(IBackgroundTaskContext context)
				{
					try
					{
						asyncCode();
						context.Complete();
					}
					catch (Exception e)
					{
						context.Error(e);
					}
				}, false) {ThreadUICulture = ThreadUICulture, ThreadCulture = ThreadCulture};

			_backgroundTask.Terminated += TerminatedEventHandler;
			_backgroundTask.Run();
		}

		/// <summary>
		/// Causes any pending asynchronous execution to be discarded (the continuation/error handler will not be called). 
		/// </summary>
		public void Cancel()
		{
			if (_backgroundTask != null)
			{
				_continuationCode = null;
				_errorHandler = null;
				_backgroundTask.Terminated -= TerminatedEventHandler;
				_backgroundTask.Dispose();
				_backgroundTask = null;
			}
		}

		/// <summary>
		/// Default error handler, used when no error handler is explicitly provided.
		/// </summary>
		/// <param name="e"></param>
		public static void DefaultErrorHandler(Exception e)
		{
			Platform.Log(LogLevel.Error, e);
		}

		#region Thread Culture Support

		/// <summary>
		/// Gets or sets the culture for the asynchronous task. If NULL, the culture will not be modified.
		/// </summary>
		public CultureInfo ThreadCulture { get; set; }

		/// <summary>
		/// Gets or sets the culture used by the Resource Manager to look up culture-specific resources for the asynchronous task. If NULL, the culture will not be modified.
		/// </summary>
		public CultureInfo ThreadUICulture { get; set; }

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <remarks>
		/// Cancels any pending asynchronous results.
		/// </remarks>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			Cancel();
		}

		#endregion

		private void TerminatedEventHandler(object sender, BackgroundTaskTerminatedEventArgs args)
		{
			if(args.Reason == BackgroundTaskTerminatedReason.Completed)
			{
				_continuationCode();
			}
			else
			{
				_errorHandler(args.Exception);
			}
		}


	}
}

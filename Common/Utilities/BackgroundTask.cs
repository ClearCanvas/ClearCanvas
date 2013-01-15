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
using System.ComponentModel;
using System.Globalization;
using System.Threading;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Allows the background thread to communicate with the <see cref="BackgroundTask"/> object.
	/// </summary>
	/// <remarks>
	/// The background thread should not directly access the <see cref="BackgroundTask"/> object.
	/// </remarks>
	public interface IBackgroundTaskContext
	{
		/// <summary>
		/// Provides access to the user state object.
		/// </summary>
		/// <remarks>
		/// The user state object is available to both threads, and therefore it should either be immutable or thread-safe.
		/// </remarks>
		object UserState { get; }

		/// <summary>
		/// Allows the background thread to ask whether cancellation has been requested.
		/// </summary>
		/// <remarks>
		/// If possible, the <see cref="BackgroundTaskMethod"/> should periodically poll this flag, 
		/// and if true, terminate as quickly as possible without completing its work.  
		/// It should call <see cref="Cancel"/> to confirm cancellation.
		/// </remarks>
		bool CancelRequested { get; }

		/// <summary>
		/// Allows the <see cref="BackgroundTaskMethod"/> to report progress to the foreground thread.
		/// </summary>
		/// <remarks>
		/// The progress object may be an instance of <see cref="BackgroundTaskProgress"/> or a derived class.</remarks>
		void ReportProgress(BackgroundTaskProgress progress);

		/// <summary>
		/// Allows the <see cref="BackgroundTaskMethod"/> to inform that it has completed successfully, and return the result objects
		/// to the foreground thread.
		/// </summary>
		/// <remarks>
		/// After calling this method, the <see cref="BackgroundTaskMethod"/> should simply exit.
		/// </remarks>
		void Complete(params object[] results);

		/// <summary>
		/// Allows the <see cref="BackgroundTaskMethod"/> to inform that it has successfully cancelled,
		/// in response to querying the <see cref="CancelRequested"/> flag, and return the result object
		/// to the foreground thread.
		/// </summary>
		/// <remarks>
		/// After calling this method, the <see cref="BackgroundTaskMethod"/> should simply exit.
		/// </remarks>
		void Cancel();

		/// <summary>
		/// Allows the <see cref="BackgroundTaskMethod"/> to inform that it cannot complete due to an exception,
		/// and return the exception to the foreground thread.
		/// </summary>
		/// <remarks>
		/// After calling this method, the <see cref="BackgroundTaskMethod"/> should simply exit.  
		/// It is technically ok for the background method to allow an exception to go unhandled, 
		/// an the unhandled exception will still be reported to the foreground thread as an error.  
		/// However, the VS debugger will break in this case, which may not be desirable.
		/// </remarks>
		void Error(Exception e);
	}

	/// <summary>
	/// Defines the signature for a method that is to be executed as a background task.
	/// </summary>
	public delegate void BackgroundTaskMethod(IBackgroundTaskContext context);

	/// <summary>
	/// Encapsulates information about the progress of the task.
	/// </summary>
	/// <remarks>
	/// This class may be subclassed in order to add additional information, or override the existing methods.
	/// </remarks>
	public class BackgroundTaskProgress
	{
		private readonly int _percent;
		private readonly string _message;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected BackgroundTaskProgress()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="BackgroundTaskProgress"/> with the specified parameters.
		/// </summary>
		public BackgroundTaskProgress(int percent, string message)
		{
			Platform.CheckArgumentRange(percent, 0, 100, "percent");
			_percent = percent;
			_message = message;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="BackgroundTaskProgress"/> with the specified parameters.
		/// </summary>
		/// <param name="index">A zero-based index.</param>
		/// <param name="total">Total number of increments.</param>
		/// <param name="message">A use friendly message.</param>
		public BackgroundTaskProgress(int index, int total, string message)
		{
			Platform.CheckNonNegative(index, "index");
			Platform.CheckNonNegative(index, "total");

			_percent = (int)(((float)(index + 1) / total) * 100);
			_message = message;
		}

		/// <summary>
		/// Gets the percent completion, as an integer between 0 and 100.
		/// </summary>
		public virtual int Percent { get { return _percent; } }

		/// <summary>
		/// Gets a status message describing the current state of the task.
		/// </summary>
		public virtual string Message { get { return _message; } }
	}

	/// <summary>
	/// Conveys progress information about a <see cref="BackgroundTask"/>.
	/// </summary>
	public class BackgroundTaskProgressEventArgs : EventArgs
	{
		private readonly object _userState;
		private readonly BackgroundTaskProgress _progress;

		/// <summary>
		/// Constructor.
		/// </summary>
		internal BackgroundTaskProgressEventArgs(object userState, BackgroundTaskProgress progress)
		{
			_userState = userState;
			_progress = progress;
		}

		/// <summary>
		/// Gets the progress object passed from the background thread.
		/// </summary>
		public BackgroundTaskProgress Progress { get { return _progress; } }

		/// <summary>
		/// Gets the user state object associated with the task.
		/// </summary>
		public object UserState { get { return _userState; } }
	}

	/// <summary>
	/// Defines the possible reasons why a <see cref="BackgroundTask"/> has terminated.
	/// </summary>
	public enum BackgroundTaskTerminatedReason
	{
		/// <summary>
		/// The task completed.
		/// </summary>
		Completed,

		/// <summary>
		/// The task was cancelled.
		/// </summary>
		Cancelled,

		/// <summary>
		/// The task encountered an exception and could not complete.
		/// </summary>
		Exception
	}

	/// <summary>
	/// Conveys information about the termination of a <see cref="BackgroundTask"/>.
	/// </summary>
	public class BackgroundTaskTerminatedEventArgs : EventArgs
	{
		private object _userState;
		private readonly BackgroundTaskTerminatedReason _reason;
		private readonly object[] _results;
		private readonly Exception _exception;

		/// <summary>
		/// Constructor.
		/// </summary>
		internal BackgroundTaskTerminatedEventArgs(object userState, BackgroundTaskTerminatedReason reason, object[] results, Exception ex)
		{
			_userState = userState;
			_reason = reason;
			_results = results;
			_exception = ex;
		}

		/// <summary>
		/// Gets the reason for termination.
		/// </summary>
		public BackgroundTaskTerminatedReason Reason { get { return _reason; } }

		/// <summary>
		/// Gets the result of the background task, assuming it returned exactly one result, otherwise null.
		/// </summary>
		public object Result { get { return _results.Length > 0 ? _results[0] : null; } }

		/// <summary>
		/// Gets the results of the background task.
		/// </summary>
		public object[] Results { get { return _results; } }

		/// <summary>
		/// Gets the exception that occured, if <see cref="Reason"/> is <see cref="BackgroundTaskTerminatedReason.Exception"/>.
		/// </summary>
		public Exception Exception { get { return _exception; } }
	}

	/// <summary>
	/// Encapsulates a background task, allowing the task to run asynchronously on a background thread
	/// and report progress and completion events to the foreground thread.
	/// </summary>
	public sealed class BackgroundTask : IDisposable
	{
		#region Helper implementation of IBackgroundTaskContext

		private class BackgroundTaskContext : IBackgroundTaskContext
		{
			private readonly BackgroundTask _owner;
			private readonly DoWorkEventArgs _doWorkArgs;

			public BackgroundTaskContext(BackgroundTask owner, DoWorkEventArgs doWorkArgs)
			{
				_owner = owner;
				_doWorkArgs = doWorkArgs;
			}

			#region IBackgroundTaskContext Members

			public object UserState
			{
				get { return _doWorkArgs.Argument; }
			}

			public bool CancelRequested
			{
				get { return _owner.CancelRequestPending; }
			}

			public void Complete(params object[] results)
			{
				_doWorkArgs.Result = results;
			}

			public void Cancel()
			{
				_doWorkArgs.Cancel = true;
			}

			public void Error(Exception e)
			{
				_owner._error = e;
			}

			public void ReportProgress(BackgroundTaskProgress progress)
			{
				_owner._backgroundWorker.ReportProgress(0, progress);
			}

			#endregion
		}

		#endregion

		private readonly BackgroundWorker _backgroundWorker;
		private readonly BackgroundTaskMethod _method;
		private readonly object _userState;
		private Exception _error;
		private event EventHandler<BackgroundTaskProgressEventArgs> _progressUpdated;
		private event EventHandler<BackgroundTaskTerminatedEventArgs> _terminated;
		private BackgroundTaskProgressEventArgs _lastBackgroundTaskProgress;
		private bool _disposed;

		/// <summary>
		/// Creates and executes a new <see cref="BackgroundTask"/> based on the specified arguments.
		/// </summary>
		/// <param name="method">The method to run in the background.</param>
		/// <param name="supportsCancel">Indicates whether the task supports cancellation or not.</param>
		/// <param name="terminateHandler">Method that will be called when the task terminates.</param>
		/// <param name="progressHandler">Optional method to handle progress updates, may be null.</param>
		/// <param name="userState">Optional state to be passed to the background task, may be null.</param>
		/// <returns>A running <see cref="BackgroundTask"/> object.</returns>
		public static BackgroundTask CreateAndRun(
			BackgroundTaskMethod method,
			bool supportsCancel,
			EventHandler<BackgroundTaskTerminatedEventArgs> terminateHandler,
			EventHandler<BackgroundTaskProgressEventArgs> progressHandler,
			object userState)
		{
			Platform.CheckForNullReference(method, "method");
			Platform.CheckForNullReference(terminateHandler, "terminateHandler");

			var task = new BackgroundTask(method, supportsCancel, userState);
			task.Terminated += terminateHandler;
			if (progressHandler != null)
			{
				task.ProgressUpdated += progressHandler;
			}
			task.Run();
			return task;
		}

		/// <summary>
		/// Constructs a new background task based on the specified method and optional state object.
		/// </summary>
		/// <remarks>
		/// The task is not executed until the <see cref="Run"/> method is called.
		/// </remarks>
		/// <param name="method">The method to run in the background.</param>
		/// <param name="supportsCancel">Indicates whether the task supports cancellation or not.</param>
		/// <param name="userState">Optional state to be passed to the background method.</param>
		public BackgroundTask(BackgroundTaskMethod method, bool supportsCancel, object userState)
		{
			Platform.CheckForNullReference(method, "method");
			_method = method;
			_userState = userState;

			_backgroundWorker = new BackgroundWorker
									{
										WorkerReportsProgress = true,
										WorkerSupportsCancellation = supportsCancel
									};
			_backgroundWorker.DoWork += BackgroundWorkerDoWork;
			_backgroundWorker.ProgressChanged += BackgroundWorkerProgressChanged;
			_backgroundWorker.RunWorkerCompleted += BackgroundWorkerRunWorkerCompleted;
		}

		/// <summary>
		/// Constructs a new background task based on the specified method.
		/// </summary>
		/// <remarks>
		/// The task is not executed until the <see cref="Run"/> method is called.
		/// </remarks>
		/// <param name="method">The method to run in the background.</param>
		/// <param name="supportsCancel">Indicates whether the task supports cancellation or not.</param>
		public BackgroundTask(BackgroundTaskMethod method, bool supportsCancel)
			: this(method, supportsCancel, null)
		{
		}

		/// <summary>
		/// Runs the background task.
		/// </summary>
		public void Run()
		{
			if (_backgroundWorker.IsBusy)
				throw new InvalidOperationException(SR.ExceptionBackgroundTaskAlreadyRunning);

			_backgroundWorker.RunWorkerAsync(_userState);
		}

		/// <summary>
		/// True if the task is currently running.
		/// </summary>
		public bool IsRunning
		{
			get { return _backgroundWorker.IsBusy; }
		}

		/// <summary>
		/// True if the task supports cancellation.
		/// </summary>
		public bool SupportsCancel
		{
			get { return _backgroundWorker.WorkerSupportsCancellation; }
		}

		/// <summary>
		/// Requests that the background task be cancelled.
		/// </summary>
		/// <remarks>
		/// The task does not actually stop running until the <see cref="Terminated"/> event is fired.
		/// </remarks>
		public void RequestCancel()
		{
			_backgroundWorker.CancelAsync();
		}

		/// <summary>
		/// True if the <see cref="RequestCancel"/> method has been called, and the request is pending.
		/// </summary>
		public bool CancelRequestPending
		{
			get { return _backgroundWorker.CancellationPending; }
		}

		/// <summary>
		/// Gets the last background task progress event args
		/// </summary>
		public BackgroundTaskProgressEventArgs LastBackgroundTaskProgress
		{
			get { return _lastBackgroundTaskProgress; }
		}

		/// <summary>
		/// Notifies that the progress of the task has been updated.
		/// </summary>
		public event EventHandler<BackgroundTaskProgressEventArgs> ProgressUpdated
		{
			add { _progressUpdated += value; }
			remove { _progressUpdated -= value; }
		}

		/// <summary>
		/// Notifies that the task has terminated.
		/// </summary>
		/// <remarks>
		/// Check the <see cref="BackgroundTaskTerminatedEventArgs"/> to determine the reason for termination, and obtain results.
		/// </remarks>
		public event EventHandler<BackgroundTaskTerminatedEventArgs> Terminated
		{
			add { _terminated += value; }
			remove { _terminated -= value; }
		}

		#region Thread Culture Support

		/// <summary>
		/// Gets or sets the culture for the background task. If NULL, the culture will not be modified.
		/// </summary>
		public CultureInfo ThreadCulture { get; set; }

		/// <summary>
		/// Gets or sets the culture used by the Resource Manager to look up culture-specific resources for the background task. If NULL, the culture will not be modified.
		/// </summary>
		public CultureInfo ThreadUICulture { get; set; }

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Disposes of the task.
		/// </summary>
		public void Dispose()
		{
			_disposed = true;
			_backgroundWorker.Dispose();
		}

		#endregion

		private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
		{
			var thread = Thread.CurrentThread;
			var oldCulture = thread.CurrentCulture;
			var oldUICulture = thread.CurrentUICulture;

			if (ThreadCulture != null)
				thread.CurrentCulture = ThreadCulture;
			if (ThreadUICulture != null)
				thread.CurrentUICulture = ThreadUICulture;

			try
			{
				// execute the operation
				_method.Invoke(new BackgroundTaskContext(this, e));
			}
			finally
			{
				thread.CurrentCulture = oldCulture;
				thread.CurrentUICulture = oldUICulture;
			}
		}

		private void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			// do not propagate any events after task has been disposed
			if (_disposed)
				return;

			_lastBackgroundTaskProgress = new BackgroundTaskProgressEventArgs(_userState, (BackgroundTaskProgress)e.UserState);
			EventsHelper.Fire(_progressUpdated, this, _lastBackgroundTaskProgress);
		}

		private void BackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			// do not propagate any events after task has been disposed
			if (_disposed)
				return;

			// if there was an unhandled exception in the worker thread, then e.Error will be non-null
			// if there was a handled exception in the worker thread, and the worker passed it back, _error will be non-null
			// therefore, coalesce the results, giving precedence to the unhandled exception
			_error = e.Error ?? _error;

			// determine the reason
			var reason = (_error != null) ? BackgroundTaskTerminatedReason.Exception
				: (e.Cancelled ? BackgroundTaskTerminatedReason.Cancelled : BackgroundTaskTerminatedReason.Completed);

			// the e.Result object is an exception for e.Cancelled status, we don't want that
			var results = (e.Cancelled ? null : (object[])e.Result);

			EventsHelper.Fire(_terminated, this,
				new BackgroundTaskTerminatedEventArgs(_userState, reason, results, _error));
		}
	}
}

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
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	[ExtensionOf(typeof (FatalExceptionHandlerExtensionPoint))]
	internal class DummyFatalExceptionHandler : FatalExceptionHandler
	{
		public DummyFatalExceptionHandler()
		{
			// We don't want to rely on the extension framework in the 
			// case of a fatal exception, so the ExceptionDialog class will
			// create it's factory on app startup.
			ExceptionDialog.CheckCanShow();
		}

		public override bool Handle(Exception exception)
		{
			ExceptionHandler.ReportUnhandled(exception);
			return true;
		}
	}

	///<summary>
	/// Provides an <see cref="IExceptionPolicy"/> with a callback to abort the Exception-causing operation.
	///</summary>
	///<remarks>
	/// Each individual <see cref="IExceptionPolicy"/> will determine if this is appropriate to be called.
	/// </remarks>
	public delegate void AbortOperationDelegate();

	/// <summary>
	/// Contains static methods used to report exceptions to the user.
	/// </summary>
	public static class ExceptionHandler
	{
		private static readonly object _syncLock = new object();
		private static bool _fatalExceptionReported;

		internal static bool ShowStackTraceInDialog
		{
			get
			{
				try
				{
					return ExceptionHandlerSettings.Default.ShowStackTraceInDialog;
				}
				catch (Exception e)
				{
					// if we can't retrieve the setting for whatever reason, just log it and move on
					Platform.Log(LogLevel.Error, e);
				}

				return false;
			}
		}

		/// <summary>
		/// Reports an exception that was not handled in code to the user.
		/// </summary>
		/// <remarks>
		/// An exception dialog will be shown to the user, but will only show the stack trace based
		/// on the value of <see cref="ExceptionHandlerSettings.ShowStackTraceInDialog"/>.  The application
		/// will be shut down automatically by this method.
		/// </remarks>
		public static void ReportUnhandled(Exception e)
		{
			Platform.Log(LogLevel.Fatal, e);

			lock (_syncLock)
			{
				if (_fatalExceptionReported)
					return;

				//Only ever show it once.
				_fatalExceptionReported = true;
			}

			ExceptionDialog.Show(SR.MessageUnexpectedErrorQuit, ShowStackTraceInDialog ? e : null, ExceptionDialogActions.Quit);
		}

		/// <summary>
		/// Reports the specified exception to the user, using the <see cref="Exception.Message"/> property value as the
		/// message.
		/// </summary>
		/// <remarks>
		/// The exception is also automatically logged.
		/// </remarks>
		/// <param name="e">Exception to report.</param>
		/// <param name="desktopWindow">Desktop window that parents the exception dialog.</param>
		public static void Report(Exception e, IDesktopWindow desktopWindow)
		{
			Report(e, null, desktopWindow);
		}

		/// <summary>
		/// Reports the specified exception to the user, displaying the specified user message first.
		/// </summary>
		/// <remarks>
		/// The exception is also automatically logged.
		/// </remarks>
		/// <param name="e">Exception to report.</param>
		/// <param name="userMessage">User-friendly message to display, instead of the message contained in the exception.</param>
		/// <param name="desktopWindow">Desktop window that parents the exception dialog.</param>
		public static void Report(Exception e, [param : Localizable(true)] string userMessage, IDesktopWindow desktopWindow)
		{
			Report(e, userMessage, desktopWindow, null);
		}

		/// <summary>
		/// Reports the specified exception to the user, displaying the specified user message first.
		/// </summary>
		/// <remarks>
		/// The exception is also automatically logged.
		/// </remarks>
		/// <param name="e">Exception to report.</param>
		/// <param name="contextualMessage">User-friendly (contextual) message to display, instead of the message contained in the exception.</param>
		/// <param name="desktopWindow">Desktop window that parents the exception dialog.</param>
		/// <param name="abortDelegate">A callback delegate for aborting the exception-causing operation.  Decision as to whether or
		/// not the callback is called is up to the individual <see cref="IExceptionPolicy"/>.</param>
		public static void Report(Exception e, [param : Localizable(true)] string contextualMessage, IDesktopWindow desktopWindow, AbortOperationDelegate abortDelegate)
		{
			ExceptionPolicyFactory.GetPolicy(e.GetType()).
				Handle(e, new ExceptionHandlingContext(e, contextualMessage, desktopWindow, abortDelegate));
		}
	}
}
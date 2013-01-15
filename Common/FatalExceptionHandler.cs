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
using System.Diagnostics;

namespace ClearCanvas.Common
{
	public sealed class FatalExceptionHandlerExtensionPoint : ExtensionPoint<IFatalExceptionHandler>
	{}

	public interface IFatalExceptionHandler
	{
		bool Handle(Exception exception);
	}

	public abstract class FatalExceptionHandler : IFatalExceptionHandler
	{
		private static readonly object _syncLock = new object();
		private static IFatalExceptionHandler _handler = Create();

		#region IFatalExceptionHandler Members

		public abstract bool Handle(Exception exception);

		#endregion

		private static IFatalExceptionHandler Create()
		{
			try
			{
				return (IFatalExceptionHandler)new FatalExceptionHandlerExtensionPoint().CreateExtension();
			}
			catch (NotSupportedException)
			{
			}

			return null;
		}

		private static void Log(Exception e)
		{
			const string message = "Fatal exception occurred in application.";
			Platform.Log(LogLevel.Fatal, e, message);
			Console.WriteLine(message);
			Console.WriteLine(e.Message);
		}

		private static void OnFatalException(Exception e)
		{
			IFatalExceptionHandler handler;
			lock (_syncLock)
			{
				//Call the real handler exactly once; all subsequent exceptions are just logged.
				handler = _handler;
				_handler = null;
			}

			if (handler == null)
			{
				Log(e);
				if (Debugger.IsAttached)
					Environment.Exit(-1);
				
				return;
			}

			bool handled = false;
			try
			{
				handled = handler.Handle(e);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex);
			}

			if (!handled)
				Log(e);

			bool suppressSillyCrashDialog = handled;
			//Whether or not to skip showing the silly crash dialog is based
			//on whether or not the very first fatal exception was "handled".
			if (suppressSillyCrashDialog)
				Environment.Exit(-1);
		}

		internal static void Initialize()
		{
			AppDomain.CurrentDomain.UnhandledException += 
				(sender, e) => OnFatalException((Exception)e.ExceptionObject);
		}
	}
}

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
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Utility class to add <see cref="ExceptionLogEntry"/>
	/// </summary>
	public class ExceptionLogger
	{
		/// <summary>
		/// Adds an <see cref="ExceptionLogEntry"/>.  If the <see cref="ExceptionLogEntry"/> cannot be created, the error is logged to the log file instead.
		/// </summary>
		/// <param name="operationName"></param>
		/// <param name="e"></param>
		public static void Log(string operationName, Exception e)
		{
			try
			{
				// log the error to the database
				using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update, PersistenceScopeOption.RequiresNew))
				{
					// disable change-set auditing for this context
					((IUpdateContext)PersistenceScope.CurrentContext).ChangeSetRecorder = null;

					DefaultExceptionRecorder recorder = new DefaultExceptionRecorder();
					ExceptionLogEntry logEntry = recorder.CreateLogEntry(operationName, e);

					PersistenceScope.CurrentContext.Lock(logEntry, DirtyState.New);

					scope.Complete();
				}
			}
			catch (Exception x)
			{
				// if we fail to properly log the exception, there is nothing we can do about it
				// just log a message to the log file
				Platform.Log(LogLevel.Error, x);

				// also log the original exception to the log file, since it did not get logged to the DB
				Platform.Log(LogLevel.Error, e);
			}
		}
	}
}
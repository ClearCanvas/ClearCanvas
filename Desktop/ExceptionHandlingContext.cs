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

namespace ClearCanvas.Desktop
{
    internal class ExceptionHandlingContext : IExceptionHandlingContext
    {
        private readonly Exception _exception;
    	private readonly AbortOperationDelegate _abortDelegate;

        public ExceptionHandlingContext(Exception e, string contextualMessage, IDesktopWindow desktopWindow, AbortOperationDelegate abortOperationDelegate)
        {
            _exception = e;
            ContextualMessage = contextualMessage;
            DesktopWindow = desktopWindow;
            _abortDelegate = abortOperationDelegate;
        }

    	public IDesktopWindow DesktopWindow { get; private set; }
    	public string ContextualMessage { get; private set; }

    	public void Log(LogLevel level, Exception e)
        {
            Platform.Log(level, e);
        }

        public void Abort()
        {
            if (_abortDelegate != null)
            {
                _abortDelegate();
            }
        }

		public void ShowMessageBox(string detailMessage)
        {
			ShowMessageBox(detailMessage, true);
        }

        public void ShowMessageBox(string detailMessage, bool prependContextualMessage)
        {
        	string message = GetMessage(detailMessage, prependContextualMessage);
			if (ExceptionHandler.ShowStackTraceInDialog)
			{
				ShowExceptionDialog(message);
			}
			else
			{
				DesktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);
			}
        }

		private string GetMessage(string detailMessage, bool prependContextualMessage)
		{
			return string.IsNullOrEmpty(ContextualMessage) || prependContextualMessage == false
				? detailMessage
				: string.Format("{0}\r\n - {1}", ContextualMessage, detailMessage);
		}
		
		private void ShowExceptionDialog(string message)
        {
			try
			{
				ExceptionDialog.Show(message, _exception, ExceptionDialogActions.Ok);
			}
			catch (Exception dialogException)
			{
				Platform.Log(LogLevel.Debug, dialogException);

				// fallback to displaying the message in a message box
				DesktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);
			}
		}
    }
}
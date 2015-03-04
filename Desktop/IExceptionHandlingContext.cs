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
	///<summary>
	/// Provides contextual information for an <see cref="IExceptionPolicy"/> to handle an <see cref="Exception"/>.
	///</summary>
	public interface IExceptionHandlingContext
	{
		///<summary>
		/// The <see cref="IDesktopWindow"/> of the component in which the exception has occurred.
		///</summary>
		IDesktopWindow DesktopWindow { get; }

		///<summary>
		/// A contextual user-friendly message provided by the component which should be common for all exceptions.
		///</summary>
		string ContextualMessage { get; }

		///<summary>
		/// Logs the specified exception.
		///</summary>
		void Log(LogLevel level, Exception e);

		///<summary>
		/// Aborts the exception-causing operation.
		///</summary>
		void Abort();

		///<summary>
		/// Shows the specified detail message in a message box in the context's <see cref="IDesktopWindow"/>.
		///</summary>
		/// <remarks>
		/// Automatically prepends the contextual message supplied by the application component to the detail message.
		/// </remarks>
		///<param name="detailMessage">The message to be shown.</param>
		void ShowMessageBox([param : Localizable(true)] string detailMessage);

		///<summary>
		/// Shows the specified detail message in a message box in the context's <see cref="IDesktopWindow"/>.
		///</summary>
		/// <remarks>
		/// Optionally prepends the contextual message supplied by the application component to the detail message.
		/// </remarks>
		///<param name="detailMessage">The message to be shown.</param>
		///<param name="prependContextualMessage">Indicates whether or not to prepend the <see cref="ContextualMessage"/>
		/// before <paramref name="detailMessage"/>.</param>
		void ShowMessageBox([param : Localizable(true)] string detailMessage, bool prependContextualMessage);
	}
}
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
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Exception handling policy for <see cref="NoVisibleDisplaySetsException"/>s.
	/// </summary>
	[ExceptionPolicyFor(typeof(NoVisibleDisplaySetsException))]

	[ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
	public sealed class NoVisibleDisplaySetsExceptionHandlingPolicy : IExceptionPolicy
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public NoVisibleDisplaySetsExceptionHandlingPolicy()
		{
		}

		#region IExceptionPolicy Members

		///<summary>
		/// Handles the specified exception.
		///</summary>
		public void Handle(Exception e, IExceptionHandlingContext exceptionHandlingContext)
		{
			exceptionHandlingContext.Log(LogLevel.Error, e);
			exceptionHandlingContext.ShowMessageBox(SR.MessageNoVisibleDisplaySets);
		}

		#endregion
	}

	/// <summary>
	/// Exception thrown by the <see cref="LayoutManager"/> when no display sets
	/// were added to the <see cref="ILogicalWorkspace"/>.
	/// </summary>
	public class NoVisibleDisplaySetsException : Exception
	{
		internal NoVisibleDisplaySetsException(string message)
			: base(message)
		{
		}
	}
}

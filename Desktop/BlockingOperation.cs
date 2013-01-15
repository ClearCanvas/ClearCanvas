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
	/// <summary>
	/// Extension point for views onto <see cref="BlockingOperation"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class BlockingOperationViewExtensionPoint : ExtensionPoint<IBlockingOperationView>
	{
	}

	/// <summary>
	/// The BlockingOperation class is a static class that allows application level code to
	/// use a wait cursor without having to explicitly reference a particular Gui Toolkit's API.
	/// </summary>
	public static class BlockingOperation
	{
		/// <summary>
		/// Executes the provided operation in the view, showing a wait cursor for the duration of the call.
		/// </summary>
		/// <param name="operation">The operation to execute in the view layer.</param>
		public static void Run(BlockingOperationDelegate operation)
		{
			Platform.CheckForNullReference(operation, "operation");

			IBlockingOperationView operationView = null;

			try
			{
				operationView = (IBlockingOperationView)ViewFactory.CreateView(new BlockingOperationViewExtensionPoint());
			}
			catch (Exception e)
			{
                Platform.Log(LogLevel.Error, e);
			}

			if (operationView == null)
			{
				operation();
			}
			else
			{
				operationView.Run(operation);
			}
		}
	}
}

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

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// A delegate to be executed in the view.
	/// </summary>
	public delegate void BlockingOperationDelegate();

	/// <summary>
	/// An interface for executing long-running operations in the
	/// view while showing a wait cursor.
	/// </summary>
	public interface IBlockingOperationView : IView
	{
		/// <summary>
		/// Executes the specified operation in the view, showing a wait cursor.
		/// </summary>
		void Run(BlockingOperationDelegate operation);
	}
}

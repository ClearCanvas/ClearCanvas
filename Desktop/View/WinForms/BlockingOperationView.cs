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

using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a way for application level code to use a wait cursor.
    /// </summary>
    [ExtensionOf(typeof(BlockingOperationViewExtensionPoint))]
	public class BlockingOperationView : WinFormsView, IBlockingOperationView
    {
		#region IBlockingOperationView Members

		public void Run(BlockingOperationDelegate operation)
		{
			Cursor previousCursor = Cursor.Current;

			try
			{
				Cursor.Current = (Cursor)GuiElement;

				operation();
			}
			catch
			{
				throw;
			}
			finally
			{
				Cursor.Current = previousCursor;
			}
		}

		#endregion

		public override object GuiElement
		{
			get { return System.Windows.Forms.Cursors.WaitCursor; }
		}
	}
}

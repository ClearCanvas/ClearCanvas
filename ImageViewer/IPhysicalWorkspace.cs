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
using ClearCanvas.Desktop;
using System.Drawing;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines a container for <see cref="IImageBox"/> objects.
	/// </summary>
	public interface IPhysicalWorkspace : IDrawable, IMemorable, IDisposable
	{
		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		IImageViewer ImageViewer { get; }

		/// <summary>
		/// Gets the associated <see cref="ILogicalWorkspace"/>.
		/// </summary>
		ILogicalWorkspace LogicalWorkspace { get; }

		/// <summary>
		/// Gets the collection of <see cref="IImageBox"/> objects that belong
		/// to this <see cref="IPhysicalWorkspace"/>.
		/// </summary>
		ImageBoxCollection ImageBoxes { get; }

		/// <summary>
		/// Gets the selected <see cref="IImageBox"/>.
		/// </summary>
		/// <value>The currently selected <see cref="IImageBox"/>, or <b>null</b> if
		/// no <see cref="IImageBox"/> is currently selected.</value>
		IImageBox SelectedImageBox { get; }

		/// <summary>
		/// Gets the number of rows of <see cref="IImageBox"/> objects in the
		/// <see cref="IPhysicalWorkspace"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="Rows"/> is <i>only</i> valid if <see cref="SetImageBoxGrid"/> has
		/// been called.  Otherwise, the value is meaningless.
		/// </remarks>
		int Rows { get; }

		/// <summary>
		/// Gets the number of columns of <see cref="IImageBox"/> objects in the
		/// <see cref="IPhysicalWorkspace"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="Columns"/> is <i>only</i> valid if <see cref="SetImageBoxGrid"/> has
		/// been called.  Otherwise, the value is meaningless.
		/// </remarks>
		int Columns { get; }

		/// <summary>
		/// Returns the image box at a specified row and column index.
		/// </summary>
		/// <param name="row">the zero-based row index of the image box to retrieve</param>
		/// <param name="column">the zero-based column index of the image box to retrieve</param>
		/// <returns>the image box at the specified row and column indices</returns>
		/// <remarks>This method is only valid if <see cref="SetImageBoxGrid"/> has been called and/or the 
		/// layout is, in fact, rectangular.</remarks>
		IImageBox this[int row, int column] { get; }

		/// <summary>
		/// Gets or sets whether the workspace is currently enabled.
		/// </summary>
		bool Enabled { get; set; }

		/// <summary>
		/// Occurs when <see cref="Enabled"/> has changed.
		/// </summary>
		event EventHandler EnabledChanged;

		//TODO (cr Oct 2009): LayoutLocked?

		/// <summary>
		/// Gets or sets whether or not the <see cref="IPhysicalWorkspace"/>'s <see cref="IImageBox"/> layout
		/// should be locked.
		/// </summary>
		bool Locked { get; set; }

		/// <summary>
		/// Occurs when <see cref="Locked"/> has changed.
		/// </summary>
		event EventHandler LockedChanged;

		/// <summary>
		/// Gets the rectangle that the <see cref="IPhysicalWorkspace"/> occupies
		/// in virtual screen coordinates.
		/// </summary>
		Rectangle ScreenRectangle { get; }

		/// <summary>
		/// Occurs when <see cref="ScreenRectangle"/> changes.
		/// </summary>
		event EventHandler ScreenRectangleChanged;

		/// <summary>
		/// Occurs when all changes to image box collection are complete.
		/// </summary>
		/// <remarks>
		/// <see cref="LayoutCompleted"/> is raised by the Framework when
		/// <see cref="SetImageBoxGrid"/> has been called.  If you are adding/removing
		/// <see cref="IImageBox"/> objects manually, you should raise this event when
		/// you're done by calling <see cref="OnLayoutCompleted"/>.  This event is
		/// consumed by the view to reduce flicker when layouts are changed.  
		/// In that way, it is similar to the WinForms methods <b>SuspendLayout</b>
		/// and <b>ResumeLayout</b>.
		/// </remarks>
		event EventHandler LayoutCompleted;

		/// <summary>
		/// Creates a rectangular <see cref="IImageBox"/> grid.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <remarks>
		/// <see cref="SetImageBoxGrid"/> is a convenience method that adds
		/// <see cref="IImageBox"/> objects to the <see cref="IPhysicalWorkspace"/>
		/// in a rectangular grid.
		/// </remarks>
		void SetImageBoxGrid(int rows, int columns);

		/// <summary>
		/// Raises the <see cref="LayoutCompleted"/> event.
		/// </summary>
		/// <remarks>
		/// If you are adding/removing <see cref="IImageBox"/> objects manually 
		/// (i.e., instead of using <see cref="SetImageBoxGrid"/>), you should call
		/// <see cref="OnLayoutCompleted"/> to raise the <see cref="LayoutCompleted"/> event.  
		/// This event is consumed by the view to reduce flicker when layouts are changed.  
		/// In that way, it is similar to the WinForms methods <b>SuspendLayout</b>
		/// and <b>ResumeLayout</b>.
		/// </remarks>
		void OnLayoutCompleted();

		/// <summary>
		/// Selects the first <see cref="IImageBox"/> in the image box collection.
		/// </summary>
		/// <remarks>
		/// When <see cref="SetImageBoxGrid"/> has been used to setup the 
		/// <see cref="IPhysicalWorkspace"/>, the first <see cref="IImageBox"/> in the
		/// image box collection will be the top-left <see cref="IImageBox"/>.
		/// </remarks>
		void SelectDefaultImageBox();
    }
}

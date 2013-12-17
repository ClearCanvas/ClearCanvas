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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines a container for <see cref="ITile"/> objects.
	/// </summary>
	public interface IImageBox : IDrawable, IMemorable, IDisposable
	{
		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b> if the 
		/// <see cref="IImageBox"/> is not part of the 
		/// physical workspace yet.</value>
		IImageViewer ImageViewer { get; }

		/// <summary>
		/// Gets the parent <see cref="IPhysicalWorkspace"/>
		/// </summary>
		/// <value>The parent <see cref="IPhysicalWorkspace"/> or <b>null</b> if the 
		/// <see cref="ImageBox"/> has not
		/// been added to the <see cref="IPhysicalWorkspace"/> yet.</value>
		IPhysicalWorkspace ParentPhysicalWorkspace { get; }

		/// <summary>
		/// Gets the collection of <see cref="ITile"/> objects that belong
		/// to this <see cref="ImageBox"/>.
		/// </summary>
		TileCollection Tiles { get; }

		/// <summary>
		/// Gets the currently selected <see cref="ITile"/>.
		/// </summary>
		ITile SelectedTile { get; }

		/// <summary>
		/// Gets or sets this <see cref="ImageBox"/>'s normalized rectangle.
		/// </summary>
		/// <remarks>
		/// Normalized coordinates specify the top-left corner,
		/// width and height of the <see cref="IImageBox"/> as a 
		/// fraction of the physical workspace.  For example, if the
		/// <see cref="NormalizedRectangle"/> is (left=0.25, top=0.0, width=0.5, height=0.5) 
		/// and the physical workspace has dimensions of (width=1000, height=800), the 
		/// <see cref="IImageBox"/> rectangle would be (left=250, top=0, width=500, height=400)
		/// </remarks>
		RectangleF NormalizedRectangle { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="IDisplaySet"/> associated with this <see cref="IImageBox"/>.
		/// </summary>
		/// <value>The <see cref="IDisplaySet"/> associated with this <see cref="IImageBox"/>.
		/// <b>null</b> if the <see cref="IImageBox"/> is empty.</value>
		/// <remarks>
		/// Setting this property to an <see cref="IDisplaySet"/> automatically populates the tiles
		/// in this <see cref="IImageBox"/> with presentation images contained in the 
		/// <see cref="IDisplaySet"/>.  Any <see cref="IDisplaySet"/> previously associated with
		/// this <see cref="IImageBox"/> are removed.  Setting this property to <b>null</b>
		/// results in an empty <see cref="IImageBox"/> and empty tiles.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown when <see cref="DisplaySetLocked"/> is true.</exception>
		IDisplaySet DisplaySet { get; set; }

		/// <summary>
		/// Gets or sets whether or not <see cref="DisplaySet"/> is locked and cannot currently be changed.
		/// </summary>
		/// <remarks>
		/// The intended usage of this property is such that code responsible for the layout
		/// of the <see cref="IImageViewer"/> manage this property's state, and other parts of the code,
		/// like tools for example, should simply check the value and respond appropriately.
		/// </remarks>
		bool DisplaySetLocked { get; set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="IImageBox"/> is
		/// selected.
		/// </summary>
		/// <remarks>
		/// <see cref="IImageBox"/> is selection is mutually exclusive.  That is,
		/// only one <see cref="IImageBox"/> is ever selected at a given time.  
		/// </remarks>
		bool Selected { get; }

		/// <summary>
		/// Gets the number of rows of tiles in this <see cref="IImageBox"/>.
		/// </summary>
		int Rows { get; }

		/// <summary>
		/// Gets the number of columns of tiles in this <see cref="IImageBox"/>.
		/// </summary>
		int Columns { get; }

		/// <summary>
		/// Gets the <see cref="ITile"/> at the specified row and column.
		/// </summary>
		/// <exception cref="IndexOutOfRangeException"><pararef name="row"/> or
		/// <pararef name="column"/> is less than 0 or is greater than or equal to 
		/// the <see cref="IImageBox.Rows"/> or <see cref="IImageBox.Columns"/> respectively.
		/// </exception>
		ITile this[int row, int column] { get; }

		/// <summary>
		/// Gets or sets the <see cref="IPresentationImage"/> in the top-left 
		/// <see cref="Tile"/> of this <see cref="ImageBox"/>.
		/// </summary>
		/// <remarks>
		/// Because a <see cref="IDisplaySet"/> is an <i>ordered</i> set of 
		/// presentation images, setting this property to a specified
		/// <see cref="IPresentationImage"/> image results in the images that follow 
		/// to "flow" into the other tiles from left to right, top to bottom so that
		/// order is preserved.
		/// </remarks>
		/// <exception cref="ArgumentException"><b>TopLeftPresentationImage</b>
		/// is not found this image box's <see cref="IDisplaySet"/></exception>
		IPresentationImage TopLeftPresentationImage { get; set; }

		/// <summary>
		/// Gets or sets the index of the <see cref="IPresentationImage"/> that is
		/// to be placed in the top-left <see cref="ITile"/> of this <see cref="IImageBox"/>.
		/// </summary>
		/// <remarks>
		/// The index is the index of the <see cref="IPresentationImage"/> in the
		/// <see cref="IDisplaySet"/>.  Because a <see cref="IDisplaySet"/> is an 
		/// <i>ordered</i> set of presentation images, setting this property to a specified
		/// <see cref="IPresentationImage"/> image results in the images that follow 
		/// to "flow" into the other tiles from left to right, top to bottom so that
		/// order is preserved.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <see cref="TopLeftPresentationImageIndex"/> is less than 0 or greater 
		/// than or equal to the number of presentation images in this
		/// image box's <see cref="IDisplaySet"/></exception>
		int TopLeftPresentationImageIndex { get; set; }

		/// <summary>
		/// Gets or sets whether the image box is currently enabled.
		/// </summary>
		bool Enabled { get; set; }

		/// <summary>
		/// A place for extensions to store custom data about the tile.
		/// </summary>
		ExtensionData ExtensionData { get; }

		/// <summary>
		/// Creates a rectangular grid of tiles.
		/// </summary>
		/// <remarks>
		/// Each time this method is called, existing tiles in the <see cref="ImageBox"/>
		/// are removed and new ones added.  The exception is when the number of rows
		/// and columns has not changed, in which case the method does nothing
		/// and returns immediately.
		/// </remarks>
		/// <param name="numberOfRows">Number of <see cref="ImageBox"/> rows.</param>
		/// <param name="numberOfColumns">Number of <see cref="ImageBox"/> columns.</param>
		/// <exception cref="ArgumentException"><paramref name="numberOfRows"/> or 
		/// <paramref name="numberOfColumns"/> is less than 1.</exception>
		void SetTileGrid(int numberOfRows, int numberOfColumns);

		/// <summary>
		/// Helper method for setting the display set of this image box.
		/// </summary>
		/// <param name="displaySet">The display set to show in this image box, or NULL to clear it.</param>
		/// <param name="createFreshCopy">Value indicating whether or not <see cref="IDisplaySet.CreateFreshCopy"/> should be called on the display set.</param>
		/// <param name="addToCommandHistory">Value indicating whether or not the viewer command history should be updated.</param>
		/// <param name="selectDefaultTile">Value indicating whether or not <see cref="SelectDefaultTile"/> should be called after updating the display set.</param>
		void SetDisplaySet(IDisplaySet displaySet, bool createFreshCopy = false, bool addToCommandHistory = true, bool selectDefaultTile = true);

		/// <summary>
		/// Selects the top left tile.
		/// </summary>
		void SelectDefaultTile();

		/// <summary>
		/// Occurs when the <see cref="DisplaySet"/> property has changed.
		/// </summary>
		event EventHandler<DisplaySetChangedEventArgs> DisplaySetChanged;
	}

	/// <summary>
	/// An extension point for ImageBox "tools" 
	/// </summary>
	[ExtensionPoint]
	public sealed class ImageBoxExtensionPoint : ExtensionPoint<IImageBoxExtension> {}

	/// TODO: Now IImageBoxExtension is basically an extension to the ImageBoxControl instead of the ImageBox.
	/// Consider combining IImageBoxExtensionView and IImageBoxExtension.
	/// 
	/// <summary>
	/// Defines interface to the view of an image box extension.
	/// </summary>
	public interface IImageBoxExtensionView : IView, IDisposable
	{
		///<summary>
		/// Sets the parent size for the view
		///</summary>
		Size ParentSize { set; }

		/// <summary>
		/// Gets the render size of the view
		/// </summary>
		Size ActualSize { get; }

		///<summary>
		/// Updates the view
		///</summary>
		void UpdateLayout();

		///<summary>
		/// Gets a boolean indicating whether the view is visible
		///</summary>
		bool Visible { get; }
	}

	/// TODO: Now IImageBoxExtension is basically an extension to the ImageBoxControl instead of the ImageBox.
	/// Consider combining IImageBoxExtensionView and IImageBoxExtension.
	/// 
	/// <summary>
	/// Defines the interface of an extension to the <see cref="IImageBox"/>
	/// </summary>
	public interface IImageBoxExtension : INotifyPropertyChanged, IDisposable
	{
		/// <summary>
		/// Name of the extension
		/// </summary>
		string Name { get; }

		///<summary>
		/// Sets the associated ImageBox
		///</summary>
		IImageBox ImageBox { set; }

		///<summary>
		/// Create an instance of the view for the plugin
		///</summary>
		/// <remarks>
		/// Caller will be responsible for managing the view
		/// </remarks>
		IImageBoxExtensionView CreateView();

		///<summary>
		/// Gets or sets a value indicating whether the extension wants its <see cref="View"/> to be visible.
		///</summary>
		/// <remarks>
		/// It is up to the image box control implementation to decide whether it's feasible 
		/// to display the extension's view on the screen.
		/// The extension must fire <see cref="PropertyChanged"/> if it wants to change its visibility. 
		/// </remarks>
		bool Visible { get; set; }

		///<summary>
		/// Gets the icons for the extension
		///</summary>
		IconSet IconSet { get; }
	}
}
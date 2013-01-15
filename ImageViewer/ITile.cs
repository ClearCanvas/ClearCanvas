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
using System.Drawing;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines a container for a single <see cref="IPresentationImage"/>.
	/// </summary>
	public interface ITile : IDrawable, IDisposable
	{
		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b> if the 
		/// <see cref="ITile"/> is not part of the 
		/// physical workspace yet.</value>
		IImageViewer ImageViewer { get; }

		/// <summary>
		/// Gets the parent <see cref="IImageBox"/>
		/// </summary>
		/// <value>The parent <see cref="IImageBox"/> or <b>null</b> if the 
		/// <see cref="ITile"/> has not
		/// been added to the <see cref="IImageBox"/> yet.</value>
		IImageBox ParentImageBox { get; }

		/// <summary>
		/// Gets the <see cref="IPresentationImage"/> associated with this
		/// <see cref="ITile"/>.
		/// </summary>
		IPresentationImage PresentationImage { get; }

		/// <summary>
		/// Gets this <see cref="ITile"/>'s normalized rectangle.
		/// </summary>
		/// <remarks>
		/// Normalized coordinates specify the top-left corner,
		/// width and height of the <see cref="ITile"/> as a 
		/// fraction of the image box.  For example, if the
		/// <see cref="NormalizedRectangle"/> is (left=0.25, top=0.0, width=0.5, height=0.5) 
		/// and the image box has dimensions of (width=1000, height=800), the 
		/// <see cref="ITile"/> rectangle would be (left=250, top=0, width=500, height=400)
		/// </remarks>
		RectangleF NormalizedRectangle { get; }

		/// <summary>
		/// Gets this <see cref="ITile"/>'s client rectangle.
		/// </summary>
		Rectangle ClientRectangle { get; }

		/// <summary>
		/// Gets the presentation image index.
		/// </summary>
		int PresentationImageIndex { get; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ITile"/> is
		/// selected.
		/// </summary>
		/// <remarks>
		/// <see cref="ITile"/> selection is mutually exclusive.  That is,
		/// only one <see cref="ITile"/> is ever selected at a given time.  
		/// </remarks>
		bool Selected { get; }

		/// <summary>
		/// Gets or sets whether the tile is currently enabled.
		/// </summary>
		bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets this <see cref="ITile"/>'s <see cref="ClearCanvas.ImageViewer.InformationBox">InformationBox</see>.
		/// </summary>
		InformationBox InformationBox { get; set; }

		/// <summary>
		/// Gets or sets this <see cref="ITile"/>'s <see cref="ClearCanvas.ImageViewer.EditBox">EditBox</see>.
		/// </summary>
		EditBox EditBox { get; set; }

        /// <summary>
        /// A place for extensions to store custom data about the tile.
        /// </summary>
	    ExtensionData ExtensionData { get; }

		/// <summary>
		/// Selects the <see cref="ITile"/>.
		/// </summary>
		/// <remarks>
		/// Selecting a <see cref="ITile"/> also selects the containing <see cref="IImageBox"/>
		/// and deselects any other currently seleccted <see cref="ITile"/> 
		/// and <see cref="IImageBox"/>.
		/// </remarks>
		void Select();

		/// <summary>
		/// Occurs when the <see cref="InformationBox"/> property has changed.
		/// </summary>
		event EventHandler<InformationBoxChangedEventArgs> InformationBoxChanged;

		/// <summary>
		/// Occurs when the <see cref="EditBox"/> property has changed.
		/// </summary>
		event EventHandler EditBoxChanged;

        /// <summary>
        /// Invokes a context menu showing the given <paramref name="actionModel"/>, at the given point in the tile.
        /// </summary>
        /// <param name="tileLocation">If provided, this is the point where the context menu is shown. Otherwise, the
        /// current mouse position within the tile is used.</param>
        /// <param name="actionModel">If provided, this is the action model shown in the context menu. Otherwise, it's
        /// the action model of the the graphic at the given location.</param>
	    void ShowContextMenu(Point? tileLocation, ActionModelNode actionModel);
	}
}

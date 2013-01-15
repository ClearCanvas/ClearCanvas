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

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Encapsulates the application of an <see cref="IUndoableOperation{T}"/> to a list
	/// of <see cref="IPresentationImage"/>s and also creates an <see cref="UndoableCommand"/>
	/// that can be entered into the <see cref="CommandHistory"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This interface may be deprecated in a future release. Consider using the <see cref="CompositeUndoableCommand"/> instead.</para>
	/// <para>
	/// It is often desirable to apply an operation across all linked 
	/// <see cref="IPresentationImage"/> objects.  For
	/// example, when an image is zoomed, it is expected that all linked images 
	/// will zoom as well.  When that operation is undone, it is expected that
	/// it is undone on all of those images.  This class encapsulates that functionality
	/// so that the plugin developer doesn't have to deal with such details.
	/// </para>
	/// </remarks>
	public class ImageOperationApplicator
	{
		private readonly LinkedImageEnumerator _imageEnumerator;
		private readonly IUndoableOperation<IPresentationImage> _operation;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="referenceImage">The 'current' (or reference) <see cref="IPresentationImage"/>.</param>
		/// <param name="operation">The operation to be performed on the current <see cref="IPresentationImage"/> and/or its linked images.</param>
		public ImageOperationApplicator(IPresentationImage referenceImage, IUndoableOperation<IPresentationImage> operation)
		{
			Platform.CheckForNullReference(referenceImage, "referenceImage");
			Platform.CheckForNullReference(operation, "operation");

			_imageEnumerator = new LinkedImageEnumerator(referenceImage);
			_operation = operation;
		}

		/// <summary>
		/// Gets or sets whether the operation should be applied to all <see cref="IImageSet"/>s.
		/// </summary>
		/// <remarks>
		/// <para>
		/// When this value is true, the operation will be applied to all <see cref="IImageSet"/>s with linked <see cref="IDisplaySet"/>s.  
		/// When false, the operation will only be applied to the current <see cref="IImageSet"/>'s linked <see cref="IDisplaySet"/>s 
		/// (determined from the current <see cref="IPresentationImage"/>).
		/// </para>
		/// <para>
		/// When the current <see cref="IDisplaySet"/> is not linked, the operation is only applied to the current <see cref="IDisplaySet"/>.
		/// </para>
		/// <para>
		/// The default value is false.
		/// </para>
		/// </remarks>
		public bool ApplyToAllImageSets
		{
			get { return _imageEnumerator.IncludeAllImageSets; }
			set { _imageEnumerator.IncludeAllImageSets = value; }
		}

		/// <summary>
		/// Applies the same <see cref="IUndoableOperation{T}"/> to the current image as well as all its linked images.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="IUndoableOperation{T}.Apply"/> will be called only for images where 
		/// <see cref="IUndoableOperation{T}.AppliesTo"/> has returned true <b>and</b> <see cref="IUndoableOperation{T}.GetOriginator"/> 
		/// has returned a non-null value.
		/// </para>
		/// <para>
		/// Each affected image is drawn automatically by this method.
		/// </para>
		/// </remarks>
		public UndoableCommand ApplyToAllImages()
		{
			_imageEnumerator.ExcludeReferenceImage = false;

			DrawableUndoableOperationCommand<IPresentationImage> command = new DrawableUndoableOperationCommand<IPresentationImage>(_operation, _imageEnumerator);
			command.Execute();

			if (command.Count == 0)
				return null;
			else
				return command;
		}

		/// <summary>
		/// Applies the <see cref="IUndoableOperation{T}"/> to only the current (reference) image.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="IUndoableOperation{T}.Apply"/> will be called only for the image if 
		/// <see cref="IUndoableOperation{T}.AppliesTo"/> has returned true <b>and</b> <see cref="IUndoableOperation{T}.GetOriginator"/> 
		/// has returned a non-null value.
		/// </para>
		/// <para>
		/// The affect image is drawn automatically by this method.
		/// </para>
		/// </remarks>
		public UndoableCommand ApplyToReferenceImage()
		{
			_imageEnumerator.ExcludeReferenceImage = true;

			DrawableUndoableOperationCommand<IPresentationImage> command = new DrawableUndoableOperationCommand<IPresentationImage>(_operation, _imageEnumerator.ReferenceImage);
			command.Execute();
			if (command.Count == 0)
				return null;
			else
				return command;
		}

		/// <summary>
		/// Applies the same <see cref="IUndoableOperation{T}"/> to all linked images, but not the current (reference) image itself.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="IUndoableOperation{T}.Apply"/> will be called only for images where 
		/// <see cref="IUndoableOperation{T}.AppliesTo"/> has returned true <b>and</b> <see cref="IUndoableOperation{T}.GetOriginator"/> 
		/// has returned a non-null value.
		/// </para>
		/// <para>
		/// Each affected image is drawn automatically by this method.
		/// </para>
		/// </remarks>
		public UndoableCommand ApplyToLinkedImages()
		{
			_imageEnumerator.ExcludeReferenceImage = true;

			DrawableUndoableOperationCommand<IPresentationImage> command = new DrawableUndoableOperationCommand<IPresentationImage>(_operation, _imageEnumerator);
			command.Execute();
			if (command.Count == 0)
				return null;
			else
				return command;
		}
	}
}

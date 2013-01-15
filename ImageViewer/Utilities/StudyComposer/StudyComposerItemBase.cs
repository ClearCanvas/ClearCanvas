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
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Utilities.StudyBuilder;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	/// <summary>
	/// A generic base class for items in the <see cref=StudyComposerComponent"/> tree.
	/// </summary>
	public abstract class StudyComposerItemBase<T> : IStudyComposerItem, IGalleryItem, ICloneable
		where T : StudyBuilderNode
	{
		private event PropertyChangedEventHandler _propertyChanged;
		private Image _icon = null;
		private T _node;

		/// <summary>
		/// Constructs a new <see cref="StudyComposerItemBase{T}"/>.
		/// </summary>
		internal StudyComposerItemBase() {}

		~StudyComposerItemBase()
		{
			// disconnect the node event handler if we still have a reference to the node
			if (this.Node != null)
				_node.PropertyChanged -= Node_PropertyChanged;
		}

		/// <summary>
		/// Indicates that a property on the node has changed, and that any views should refresh its display of the item.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		/// <summary>
		/// Gets or sets the name label of this item.
		/// </summary>
		public abstract string Name { get; set; }

		/// <summary>
		/// Gets a short, multi-line description of the item that contains ancillary information.
		/// </summary>
		public abstract string Description { get; }

		/// <summary>
		/// Gets the <see cref="StudyBuilderNode"/> node that is encapsulated by this <see cref="StudyComposerItemBase{T}"/>.
		/// </summary>
		public T Node
		{
			get { return _node; }
			protected set
			{
				if (_node != value)
				{
					if (_node != null)
						_node.PropertyChanged -= Node_PropertyChanged;

					_node = value;

					if (_node != null)
						_node.PropertyChanged += Node_PropertyChanged;

					FirePropertyChanged("Node");
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="StudyBuilderNode"/> node that is encapsulated by this <see cref="StudyComposerItemBase{T}"/>.
		/// </summary>
		/// <remarks>
		/// Returns the same value as <see cref="Node"/>, but hidden to provide the strongly-typed alternative.
		/// </remarks>
		StudyBuilderNode IStudyComposerItem.Node
		{
			get { return this.Node; }
		}

		/// <summary>
		/// Gets the <see cref="StudyBuilderNode"/> node that is encapsulated by this <see cref="StudyComposerItemBase{T}"/>.
		/// </summary>
		/// <remarks>
		/// Returns the same value as <see cref="Node"/>, but hidden to provide the strongly-typed alternative, as well as to rename
		/// the field to make it more clear that the item is the encapsulated node.
		/// </remarks>
		object IGalleryItem.Item
		{
			get { return this.Node; }
		}

		/// <summary>
		/// Gets an <see cref="Image"/> icon that can be used to represent the item in thumbnail views.
		/// </summary>
		public Image Icon
		{
			get { return _icon; }
			protected set
			{
				if (_icon != value)
				{
					_icon = value;
					FirePropertyChanged("Icon");
				}
			}
		}

		/// <summary>
		/// Gets an <see cref="Image"/> icon that can be used to represent the item in thumbnail views.
		/// </summary>
		/// <remarks>
		/// Returns the same value as <see cref="Icon"/>, but hidden to provide the strongly-typed alternative, as well as to rename
		/// the field to make it more clear that this has nothing to do with the <see cref="SeriesItem.Images"/> field, which lists
		/// the images in a series.
		/// </remarks>
		Image IGalleryItem.Image
		{
			get { return this.Icon; }
		}

		/// <summary>
		/// Regenerates the icon for a specific icon size.
		/// </summary>
		/// <param name="iconSize">The <see cref="Size"/> of the icon to generate.</param>
		public abstract void UpdateIcon(Size iconSize);

		/// <summary>
		/// Regenerates the icon for the default icon size (64x64).
		/// </summary>
		public void UpdateIcon()
		{
			UpdateIcon(new Size(64, 64));
		}

		/// <summary>
		/// Gets a string representation of the item.
		/// </summary>
		/// <remarks>In most cases, this is simply the name label of the item.</remarks>
		/// <returns>A string representation of the item.</returns>
		public override string ToString()
		{
			return this.Name;
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public abstract StudyComposerItemBase<T> Clone();

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		IStudyComposerItem IStudyComposerItem.Clone()
		{
			return this.Clone();
		}

		private void Node_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnNodePropertyChanged(e.PropertyName);
		}

		protected virtual void OnNodePropertyChanged(string propertyName) {}

		/// <summary>
		/// Raises the <see cref="PropertyChanged"/> event for the given property.
		/// </summary>
		/// <param name="propertyName"></param>
		protected void FirePropertyChanged(string propertyName)
		{
			if (_propertyChanged != null)
				_propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
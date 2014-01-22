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
using System.Threading;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Clipboard
{
	/// <summary>
	/// Defines an item that can be stored in the clipboard.
	/// </summary>
	public interface IClipboardItem
	{
		/// <summary>
		/// Returns the actual clipboard item.
		/// </summary>
		object Item { get; }

		/// <summary>
		/// Gets extension data associated with the clipboard item.
		/// </summary>
		ExtensionData ExtensionData { get; }

		/// <summary>
		/// Returns the display rectangle of the clipboard item.
		/// </summary>
		Rectangle DisplayRectangle { get; }

		/// <summary>
		/// Gets whether or not the clipboard item is locked.
		/// </summary>
		/// <remarks>
		/// Locking indicates that asynchronous operations are potentially
		/// being performed with this <see cref="IClipboardItem"/>, and thus
		/// that the item may not be deleted from the clipboard, which
		/// would otherwise result in <see cref="Item"/>'s disposal.
		/// </remarks>
		bool IsLocked { get; }

		/// <summary>
		/// Locks a clipboard item.
		/// </summary>
		/// <remarks>
		/// This method should be called when an asynchronous operation is about to
		/// be performed with this <see cref="IClipboardItem"/>.  Locking prevents
		/// removal of the <see cref="IClipboardItem"/> from the clipboard, which
		/// would result in <see cref="Item"/>'s disposal.
		/// </remarks>
		void Lock();

		/// <summary>
		/// Unlocks a clipboard item.
		/// </summary>
		/// <remarks>
		/// <remarks>
		/// This method should be called when an asynchronous operation, performed 
		/// with this <see cref="IClipboardItem"/>, has completed.  Locking prevents
		/// removal of the <see cref="IClipboardItem"/> from the clipboard, which
		/// would result in <see cref="Item"/>'s disposal.
		/// </remarks>
		/// </remarks>
		void Unlock();
	}

	public class ClipboardItem : IClipboardItem, IGalleryItem
	{
		private object _item;
		private Image _image;
		private readonly string _name;
		private readonly string _description;
		private readonly Rectangle _displayRectangle;
		private int _lockCount;
		private ExtensionData _extensionData;

		/// <summary>
		/// Initializes a new instance of <see cref="ClipboardItem"/>.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="image"></param>
		/// <param name="name"></param>
		/// <param name="description"></param>
		/// <param name="displayRectangle"></param>
		public ClipboardItem(object item, Image image, string name, string description, Rectangle displayRectangle)
		{
			_item = item;
			_image = image;
			_name = name ?? string.Empty;
			_description = description ?? string.Empty;
			_displayRectangle = displayRectangle;
			_extensionData = new ExtensionData();
		}

		public ExtensionData ExtensionData
		{
			get { return _extensionData; }
		}

		public object Item
		{
			get { return _item; }
		}

		/// <summary>
		/// Gets the thumbnail image for the clipboard item.
		/// </summary>
		public Image Image
		{
			get { return _image; }
		}

		/// <summary>
		/// Gets the label for the clipboard item.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		public string Description
		{
			get { return _description; }
		}

		public Rectangle DisplayRectangle
		{
			get { return _displayRectangle; }
		}

		public void Lock()
		{
			Interlocked.Increment(ref _lockCount);
		}

		public void Unlock()
		{
			Interlocked.Decrement(ref _lockCount);
		}

		public bool IsLocked
		{
			get { return Thread.VolatileRead(ref _lockCount) != 0; }
		}

		public void Dispose()
		{
			if (_item != null && _item is IDisposable)
			{
				((IDisposable) _item).Dispose();
				_item = null;
			}
			if (_image != null)
			{
				_image.Dispose();
				_image = null;
			}
			if (_extensionData != null)
			{
				_extensionData.Dispose();
				_extensionData = null;
			}
		}

		#region IGalleryItem Implementation

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { }
			remove { }
		}

		object IGalleryItem.Image
		{
			get { return Image; }
		}

		string IGalleryItem.Name
		{
			get { return Name; }
			set { throw new InvalidOperationException("Renaming clipboard items is not supported."); }
		}

		#endregion
	}
}
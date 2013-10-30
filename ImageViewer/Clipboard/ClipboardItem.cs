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

	internal class ClipboardItem : IClipboardItem, IGalleryItem
	{
		private object _item;
		private Image _image;
		private readonly string _name;
		private readonly string _description;
		private readonly Rectangle _displayRectangle;
		private int _lockCount;

		public ClipboardItem(object item, Image image, string name, string description, Rectangle displayRectangle)
		{
			_item = item;
			_image = image;
			_name = name;
			_description = description;
			_displayRectangle = displayRectangle;
		}

		public object Item
		{
			get { return _item; }
		}

		public object Image
		{
			get { return _image; }
		}

		public string Name
		{
			get { return _name; }
			set { throw new NotImplementedException("Renaming clipboard items is not yet supported."); }
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
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
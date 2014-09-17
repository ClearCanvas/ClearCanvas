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
using System.Collections.Generic;
using System.Diagnostics;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	internal delegate void NotifyChangedDelegate();

	internal class ImageBoxFrameSelectionStrategy : IDisposable
	{
		private readonly IImageBox _imageBox;
		private readonly int _window;
		private readonly NotifyChangedDelegate _notifyChanged;
		private readonly Predicate<Frame> _canFetchFrame;

		private readonly object _syncLock = new object();
		private readonly Queue<Frame> _frames;

		private int _currentIndex = -1;

		public ImageBoxFrameSelectionStrategy(IImageBox imageBox, int window, Predicate<Frame> canFetchFrame, NotifyChangedDelegate notifyChanged)
		{
			_canFetchFrame = canFetchFrame;
			_notifyChanged = notifyChanged;
			_imageBox = imageBox;
			_imageBox.Drawing += OnImageBoxDrawing;

			_window = window;
			_frames = new Queue<Frame>();
			Refresh(true);
		}

		private void OnImageBoxDrawing(object sender, EventArgs e)
		{
			Refresh(false);
		}

		public void OnDisplaySetChanged()
		{
			Refresh(true);
		}

		private void Refresh(bool force)
		{
			lock (_syncLock)
			{
				if (_imageBox.DisplaySet == null || _imageBox.DisplaySet.PresentationImages.Count == 0)
				{
					_frames.Clear();
					return;
				}

				if (!force && _currentIndex == _imageBox.TopLeftPresentationImageIndex)
					return;

				_frames.Clear();

				_currentIndex = _imageBox.TopLeftPresentationImageIndex;
				int numberOfImages = _imageBox.DisplaySet.PresentationImages.Count;
				int lastImageIndex = numberOfImages - 1;

				int selectionWindow;
				if (_window >= 0)
				{
					selectionWindow = 2 * _window + 1;
				}
				else 
				{
					//not terribly efficient, but by default will end up including all images.
					selectionWindow = 2 * numberOfImages + 1;
				}

				int offsetFromCurrent = 0;
				for (int i = 0; i < selectionWindow; ++i)
				{
					int index = _currentIndex + offsetFromCurrent;

					if (index >= 0 && index <= lastImageIndex)
					{
						IPresentationImage image = _imageBox.DisplaySet.PresentationImages[index];
						if (image is IImageSopProvider)
						{
							Frame frame = ((IImageSopProvider) image).Frame;
							if (_canFetchFrame(frame))
								_frames.Enqueue(frame);
						}
					}

					if (offsetFromCurrent == 0)
						++offsetFromCurrent;
					else if (offsetFromCurrent > 0)
						offsetFromCurrent = -offsetFromCurrent;
					else
						offsetFromCurrent = -offsetFromCurrent + 1;
				}
			}

			//trigger another round of retrievals.
			_notifyChanged();
		}

		public Frame GetNextFrame()
		{
			lock (_syncLock)
			{
				if (_frames.Count == 0)
					return null;

				return _frames.Dequeue();
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			_imageBox.Drawing -= OnImageBoxDrawing;
		}

		#endregion
	}
}

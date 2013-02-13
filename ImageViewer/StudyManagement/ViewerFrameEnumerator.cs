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
using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;
using System.Collections;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	internal class ViewerFrameEnumerator : IBlockingEnumerator<Frame>, IDisposable
	{
		private readonly IImageViewer _viewer;

		private readonly object _syncLock = new object();
		private readonly Queue<Frame> _framesToProcess;
		private readonly Predicate<Frame> _canFetchFrame;

		private readonly Dictionary<IImageBox, ImageBoxFrameSelectionStrategy> _imageBoxStrategies;
		private IEnumerator<KeyValuePair<IImageBox, ImageBoxFrameSelectionStrategy>> _strategyEnumerator;
		private IImageBox _selectedImageBox;

		private readonly int _selectedWeight;
		private readonly int _unselectedWeight;
		private readonly int _imageWindow;

		private volatile bool _isBlocking = true;

		public ViewerFrameEnumerator(IImageViewer viewer, int selectedWeight, int unselectedWeight, int imageWindow, Predicate<Frame> canFetchFrame)
		{
			_viewer = viewer;
			_selectedWeight = selectedWeight;
			_unselectedWeight = unselectedWeight;
			_imageWindow = imageWindow;

			_framesToProcess = new Queue<Frame>();
			_canFetchFrame = canFetchFrame;

			_viewer.PhysicalWorkspace.ImageBoxes.ItemAdded += OnImageBoxAdded;
			_viewer.PhysicalWorkspace.ImageBoxes.ItemRemoved += OnImageBoxRemoved;
			_viewer.PhysicalWorkspace.ImageBoxes.ItemChanging += OnImageBoxChanging;
			_viewer.PhysicalWorkspace.ImageBoxes.ItemChanged += OnImageBoxChanged;

			_viewer.EventBroker.ImageBoxSelected += OnImageBoxSelected;
			_viewer.EventBroker.DisplaySetChanged += OnDisplaySetChanged;

			_imageBoxStrategies = new Dictionary<IImageBox, ImageBoxFrameSelectionStrategy>();

			foreach(IImageBox imageBox in _viewer.PhysicalWorkspace.ImageBoxes)
				_imageBoxStrategies[imageBox] = new ImageBoxFrameSelectionStrategy(imageBox, _imageWindow, _canFetchFrame, OnImageBoxDataChanged);

			_selectedImageBox = _viewer.SelectedImageBox;
		}

		private void OnImageBoxRemoved(object sender, ListEventArgs<IImageBox> e)
		{
			lock (_syncLock)
			{
				if (_imageBoxStrategies.ContainsKey(e.Item))
					_imageBoxStrategies[e.Item].Dispose();

				_imageBoxStrategies.Remove(e.Item);

				OnImageBoxesChanged();
			}
		}

		private void OnImageBoxChanging(object sender, ListEventArgs<IImageBox> e)
		{
			lock (_syncLock)
			{
				if (_imageBoxStrategies.ContainsKey(e.Item))
					_imageBoxStrategies[e.Item].Dispose();

				_imageBoxStrategies.Remove(e.Item);

				OnImageBoxesChanged();
			}
		}

		private void OnImageBoxChanged(object sender, ListEventArgs<IImageBox> e)
		{
			lock (_syncLock)
			{
				_imageBoxStrategies[e.Item] = new ImageBoxFrameSelectionStrategy(e.Item, _imageWindow, _canFetchFrame, OnImageBoxDataChanged);

				OnImageBoxesChanged();
			}
		}

		private void OnImageBoxAdded(object sender, ListEventArgs<IImageBox> e)
		{
			lock (_syncLock)
			{
				_imageBoxStrategies[e.Item] = new ImageBoxFrameSelectionStrategy(e.Item, _imageWindow, _canFetchFrame, OnImageBoxDataChanged);

				OnImageBoxesChanged();
			}
		}


		private void OnImageBoxSelected(object sender, ImageBoxSelectedEventArgs e)
		{
			lock (_syncLock)
			{
				if (_imageBoxStrategies.ContainsKey(e.SelectedImageBox))
					_selectedImageBox = e.SelectedImageBox;
				else
					_selectedImageBox = null;

				Monitor.PulseAll(_syncLock);
			}
		}

		private void OnDisplaySetChanged(object sender, DisplaySetChangedEventArgs e)
		{
			lock(_syncLock)
			{
				if (e.NewDisplaySet != null)
				{
					if (_imageBoxStrategies.ContainsKey(e.NewDisplaySet.ImageBox))
						_imageBoxStrategies[e.NewDisplaySet.ImageBox].OnDisplaySetChanged();
				}

				Monitor.PulseAll(_syncLock);
			}
		}

		private void OnImageBoxesChanged()
		{
			_strategyEnumerator = null;
			_framesToProcess.Clear();
			Monitor.PulseAll(_syncLock);
		}

		private void OnImageBoxDataChanged()
		{
			lock (_syncLock)
			{
				Monitor.PulseAll(_syncLock);
			}
		}

		private IEnumerable<Frame> GetFrames()
		{
			while (_isBlocking)
			{
				Frame nextFrame = null;
				lock(_syncLock)
				{
					if (_framesToProcess.Count == 0)
					{
						GetNextRoundOfFrames();
						if (_framesToProcess.Count == 0 && _isBlocking)
						{
							Platform.Log(LogLevel.Debug, "ViewerFrameEnumerator: no frames left.");
							Monitor.Wait(_syncLock);
						}
					}

					if (_framesToProcess.Count > 0)
						nextFrame = _framesToProcess.Dequeue();
				}

				if (nextFrame != null)
					yield return nextFrame;
			}
		}

		private void GetNextRoundOfFrames()
		{
			lock (_syncLock)
			{
				if (_strategyEnumerator == null)
					_strategyEnumerator = _imageBoxStrategies.GetEnumerator();

				while (_strategyEnumerator != null)
				{
					AddSelectedImageBoxFrames();

					int count = 0;
					while (true)
					{
						if (!_strategyEnumerator.MoveNext())
						{
							_strategyEnumerator = null;
							break;
						}

						if (_strategyEnumerator.Current.Key != _selectedImageBox)
						{
							if (_imageBoxStrategies.ContainsKey(_strategyEnumerator.Current.Key))
							{
								Frame frame = _imageBoxStrategies[_strategyEnumerator.Current.Key].GetNextFrame();
								if (frame != null)
								{
									_framesToProcess.Enqueue(frame);
									Monitor.Pulse(_syncLock);

									if (++count >= _unselectedWeight)
										break;
								}
							}
						}
					}
				}
			}
		}

		private void AddSelectedImageBoxFrames()
		{
			ImageBoxFrameSelectionStrategy strategy;
			if (_selectedImageBox != null && _imageBoxStrategies.ContainsKey(_selectedImageBox))
			{
				strategy = _imageBoxStrategies[_selectedImageBox];
				for (int i = 0; i < _selectedWeight; ++i)
				{
					Frame nextFrame = strategy.GetNextFrame();
					if (nextFrame != null)
					{
						_framesToProcess.Enqueue(nextFrame);
						Monitor.Pulse(_syncLock);
					}
					else
					{
						break;
					}
				}
			}
		}

		#region IBlockingEnumerator<Frame> Members

		public bool IsBlocking
		{
			get { return _isBlocking; }
			set
			{
				if (_isBlocking == value)
					return;

				_isBlocking = value;
				if (!_isBlocking)
				{
					lock (_syncLock)
					{
						Monitor.PulseAll(_syncLock);
					}
				}
			}
		}

		#endregion

		#region IEnumerable<Frame> Members

		public IEnumerator<Frame> GetEnumerator()
		{
			return GetFrames().GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetFrames().GetEnumerator();
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			IsBlocking = false;

			_viewer.PhysicalWorkspace.ImageBoxes.ItemAdded -= OnImageBoxAdded;
			_viewer.PhysicalWorkspace.ImageBoxes.ItemRemoved -= OnImageBoxRemoved;
			_viewer.PhysicalWorkspace.ImageBoxes.ItemChanging -= OnImageBoxChanging;
			_viewer.PhysicalWorkspace.ImageBoxes.ItemChanged -= OnImageBoxChanged;

			_viewer.EventBroker.ImageBoxSelected -= OnImageBoxSelected;
			_viewer.EventBroker.DisplaySetChanged -= OnDisplaySetChanged;

			if (_imageBoxStrategies != null)
			{
				foreach (ImageBoxFrameSelectionStrategy strategy in _imageBoxStrategies.Values)
				{
					try
					{
						strategy.Dispose();
					}
					catch(Exception e)
					{
						Platform.Log(LogLevel.Warn, e, "Error disposing frame selection strategy.");
					}
				}

				_imageBoxStrategies.Clear();
			}
		}

		#endregion
	}
}

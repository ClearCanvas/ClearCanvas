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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[ExtensionPoint]
	public sealed class CineApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(CineApplicationComponentViewExtensionPoint))]
	public class CineApplicationComponent : ImageViewerToolComponent
	{
		#region Private Fields

		private bool _enabled;

		private readonly int _minimumScale;
		private readonly int _maximumScale;
		private readonly int _minimumScaleWaitMilliseconds;
		private volatile int _currentScaleValue;
		private bool _reverse;

		private IImageBox _selectedImageBox;

		private MemorableUndoableCommand _memorableCommand;

		private volatile bool _stopThread;
		private readonly object _threadLock;
		private Thread _cineThread;
		private SynchronizationContext _uiThreadSynchronizationContext;

		#endregion

		public CineApplicationComponent(IDesktopWindow desktopWindow)
			: base(desktopWindow)
		{
			_enabled = true;

			_minimumScale = 0;
			_maximumScale = 100;
			_minimumScaleWaitMilliseconds = 250;
			_currentScaleValue = 50;

			_selectedImageBox = null;
			_memorableCommand = null;

			_threadLock = new object();
			_stopThread = false;
			_cineThread = null;
			_uiThreadSynchronizationContext = null;
		}

		#region Public Members

		public bool Enabled
		{
			get { return _enabled; }
			private set
			{
				if (value == _enabled)
					return;

				_enabled = value;
				NotifyPropertyChanged("Enabled");
			}
		}

		public int MinimumScale
		{
			get { return _minimumScale; }
		}

		public int MaximumScale
		{
			get { return _maximumScale; }
		}

		public int CurrentScaleValue
		{
			get { return _currentScaleValue; }
			set
			{
				value = Math.Min(value, _maximumScale);
				_currentScaleValue = Math.Max(value, _minimumScale);
				NotifyPropertyChanged("CurrentScaleValue");
			}
		}

		public bool Reverse
		{
			get { return _reverse; }
			set 
			{
				_reverse = value;
				NotifyPropertyChanged("Reverse");
			}
		}

		public bool Running
		{
			get { return _cineThread != null; }
		}

		public void StartCine()
		{
			if (Running)
				return;

			if (!CaptureBeginState())
				return;

			this.ImageViewer.PhysicalWorkspace.Enabled = false;

			_stopThread = false;
			_cineThread = new Thread(RunThread);
			_cineThread.Priority = ThreadPriority.Lowest;
			_cineThread.Start();

			NotifyPropertyChanged("Running");
		}

		public void StopCine()
		{
			if (!Running)
				return;

			_stopThread = true;
			lock (_threadLock)
			{
				Monitor.Pulse(_threadLock);
			}

			_cineThread.Join();
			_cineThread = null;

			CommitEndState();

			NotifyPropertyChanged("Running");

			this.ImageViewer.PhysicalWorkspace.Enabled = true;
		}

		#endregion

		#region Overrides

		public override void Start()
		{
			_uiThreadSynchronizationContext = SynchronizationContext.Current;
			if (_uiThreadSynchronizationContext == null)
				throw new ApplicationException(SR.ExceptionValidSynchronizationContextMustExist);

			Enabled = CanStart();

			base.Start();
		}

		public override void Stop()
		{
			StopCine();

			base.Stop();

			// ImageViewer may not have been unassigned, so manually execute the important part of OnActiveImageViewerChanged
			if (this.ImageViewer != null)
			{
				this.ImageViewer.EventBroker.TileSelected -= OnTileSelected;
				this.ImageViewer.CommandHistory.CurrentCommandChanging -= OnCommandChanging;
			}
		}

		protected override void OnActiveImageViewerChanging(ActiveImageViewerChangedEventArgs e)
		{
			StopCine();
		}

		protected override void OnActiveImageViewerChanged(ActiveImageViewerChangedEventArgs e)
		{
			if (e.DeactivatedImageViewer != null)
			{
				e.DeactivatedImageViewer.EventBroker.TileSelected -= OnTileSelected;
				e.DeactivatedImageViewer.CommandHistory.CurrentCommandChanging -= OnCommandChanging;
			}

			if (e.ActivatedImageViewer != null)
			{
				e.ActivatedImageViewer.EventBroker.TileSelected += OnTileSelected;
				e.ActivatedImageViewer.CommandHistory.CurrentCommandChanging += OnCommandChanging;
			}

			Enabled = CanStart();
		}

		#endregion

		#region Private Members
		#region Event Handlers

		private void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			bool canStart = CanStart();

			if (!Running)
			{
				Enabled = canStart;

				return;
			}

			if (canStart)
			{
				if (this.ImageViewer.SelectedImageBox != _selectedImageBox)
					CommitEndState();
			}
			else
			{
				StopCine();
				Enabled = false;
			}
		}

		private void OnCommandChanging(object sender, EventArgs e)
		{
			StopCine();
		}

		#endregion

		private bool CanStart()
		{
			if (ImageViewer == null)
				return false;

			return CanStart(this.ImageViewer.SelectedImageBox);
		}

		private static bool CanStart(IImageBox imageBox)
		{
			if (imageBox == null || 
					imageBox.DisplaySet == null ||
					imageBox.TopLeftPresentationImageIndex < 0 ||
					imageBox.Tiles.Count >= imageBox.DisplaySet.PresentationImages.Count)
				return false;

			return true;
		}

		private bool CaptureBeginState()
		{
			if (CanStart())
			{
				_selectedImageBox = ImageViewer.SelectedImageBox;
				_memorableCommand = new MemorableUndoableCommand(_selectedImageBox);
				_memorableCommand.BeginState = _selectedImageBox.CreateMemento();

				return true;
			}

			return false;
		}

		private void CommitEndState()
		{
			if (_memorableCommand != null)
			{
				_selectedImageBox.ImageViewer.CommandHistory.CurrentCommandChanging -= OnCommandChanging;

				if (!_memorableCommand.BeginState.Equals(_memorableCommand.EndState))
				{
					DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(_selectedImageBox);
					historyCommand.Name = SR.CommandCine;
					historyCommand.Enqueue(_memorableCommand);
					_selectedImageBox.ImageViewer.CommandHistory.AddCommand(historyCommand);
				}

				_selectedImageBox.ImageViewer.CommandHistory.CurrentCommandChanging += OnCommandChanging;
			}

			_selectedImageBox = null;
			_memorableCommand = null;
		}

		private void AdvanceImage()
		{
			if (!_stopThread)
			{
				if (_memorableCommand == null)
				{
					if (!CaptureBeginState())
					{
						StopCine();
						return;
					}
				}

				if (_reverse)
				{
					if (_selectedImageBox.TopLeftPresentationImageIndex == 0)
					{
						_selectedImageBox.TopLeftPresentationImageIndex = _selectedImageBox.DisplaySet.PresentationImages.Count - 1;
					}
					else
					{
						--_selectedImageBox.TopLeftPresentationImageIndex;
					}
				}
				else
				{
					if (_selectedImageBox.TopLeftPresentationImageIndex == _selectedImageBox.DisplaySet.PresentationImages.Count - _selectedImageBox.Tiles.Count)
					{
						_selectedImageBox.TopLeftPresentationImageIndex = 0;
					}
					else
					{
						++_selectedImageBox.TopLeftPresentationImageIndex;
					}
				}

				_memorableCommand.EndState = _selectedImageBox.CreateMemento();
				_selectedImageBox.Draw();
			}

			lock (_threadLock)
			{
				Monitor.Pulse(_threadLock);
			}
		}

		private void RunThread()
		{
			lock (_threadLock)
			{
				while (!_stopThread)
				{
					_uiThreadSynchronizationContext.Post(delegate { AdvanceImage(); }, null);
					Monitor.Wait(_threadLock); //wait until the message is processed.

					if (_stopThread)
						break;

					int wait = _minimumScaleWaitMilliseconds -(int) (_minimumScaleWaitMilliseconds * _currentScaleValue / (float) _maximumScale);
					wait = Math.Max(wait, 5); //otherwise, the main thread becomes unresponsive.
					Monitor.Wait(_threadLock, wait);
				}
			}
		}

		#endregion
	}
}

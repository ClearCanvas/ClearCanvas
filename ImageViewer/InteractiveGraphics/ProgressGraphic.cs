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
using ClearCanvas.ImageViewer.Graphics;
using Timer = ClearCanvas.Common.Utilities.Timer;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A dialog-like graphic for displaying the current progress of some abstract operation in a scene graph.
	/// </summary>
	public partial class ProgressGraphic : CompositeGraphic
	{
		private const bool _defaultAutoClose = false;
		private const bool _defaultDrawImmediately = true;
		private const ProgressBarGraphicStyle _defaultStyle = ProgressBarGraphicStyle.Blocks;

		private readonly ProgressBarGraphicStyle _progressBarStyle;
		private readonly ProgressCompositeGraphic _graphics;
		private readonly bool _autoClose;

		private volatile bool _drawPending;

		private IProgressProvider _progressProvider;
		private SynchronizationContext _synchronizationContext;

		/// <summary>
		/// Initializes a new <see cref="ProgressGraphic"/>.
		/// </summary>
		/// <param name="progressProvider">The provider of progress information for which the <see cref="ProgressGraphic"/> will display updates.</param>
		/// <param name="autoClose">A value indicating whether or not the <see cref="ProgressGraphic"/> should automatically remove and dispose itself when the progress provider reports task completion or cancellation.</param>
		/// <param name="progressBarStyle">The style of progress bar to be displayed.</param>
		public ProgressGraphic(IProgressProvider progressProvider, bool autoClose = _defaultAutoClose, ProgressBarGraphicStyle progressBarStyle = _defaultStyle)
		{
			_autoClose = autoClose;
			_progressBarStyle = progressBarStyle;

			_progressProvider = progressProvider;
			_progressProvider.ProgressUpdated += OnProgressUpdated;

			_synchronizationContext = SynchronizationContext.Current;

			Graphics.Add(_graphics = new ProgressCompositeGraphic(_progressBarStyle));
		}

		/// <summary>
		/// Initializes a new <see cref="ProgressGraphic"/>.
		/// </summary>
		/// <param name="progressProvider">The provider of progress information for which the <see cref="ProgressGraphic"/> will display updates.</param>
		/// <param name="autoClose">A value indicating whether or not the <see cref="ProgressGraphic"/> should automatically remove and dispose itself when the progress provider reports task completion or cancellation.</param>
		/// <param name="progressBarStyle">The style of progress bar to be displayed.</param>
		[Obsolete("Implement the IProgressProvider interface on the source and use the corresponding constructor overload instead.")]
		public ProgressGraphic(IProgressGraphicProgressProvider progressProvider, bool autoClose = _defaultAutoClose, ProgressBarGraphicStyle progressBarStyle = _defaultStyle)
			: this(new LegacyProgressProviderAdapter(progressProvider), autoClose, progressBarStyle) {}

		/// <summary>
		/// Releases all resources used by this <see cref="ProgressGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_progressProvider != null)
				{
					_progressProvider.ProgressUpdated -= OnProgressUpdated;
					_progressProvider = null;
				}
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// This property value is no longer used.
		/// </summary>
		[Obsolete("This property value is no longer used.")]
		public int AnimationTick { get; set; }

		/// <summary>
		/// Gets a value indicating whether or not the <see cref="ProgressGraphic"/> will remove and dispose itself when the progress provider reports task completion or cancellation.
		/// </summary>
		public bool AutoClose
		{
			get { return _autoClose; }
		}

		/// <summary>
		/// Forces the <see cref="ProgressGraphic"/> to remove and dispose itself immediately.
		/// </summary>
		/// <remarks>
		/// Calling this method will invoke <see cref="IDrawable.Draw"/> on the scene graph, so do not call this method from a drawing operation.
		/// </remarks>
		public void Close()
		{
			if (ParentGraphic != null)
			{
				var parent = ParentGraphic;
				((CompositeGraphic) ParentGraphic).Graphics.Remove(this);
				parent.Draw();
			}
			Dispose();
		}

		public override bool Visible
		{
			get { return base.Visible && ParentPresentationImage != null && ParentPresentationImage.Tile != null; }
			set { base.Visible = value; }
		}

		/// <summary>
		/// Called by the framework just before the <see cref="ProgressGraphic"/> is rendered.
		/// </summary>
		public override void OnDrawing()
		{
			if (_synchronizationContext == null)
				_synchronizationContext = SynchronizationContext.Current;

			_drawPending = false;
			if (_progressProvider != null)
			{
				float progress;
				string message;

				if (!_progressProvider.GetProgress(out progress, out message))
				{
					_progressProvider.ProgressUpdated -= OnProgressUpdated;
					_progressProvider = null;

					if (_autoClose && _synchronizationContext != null)
					{
						_graphics.Visible = Visible = false;
						_synchronizationContext.Post(s => Close(), null);
					}
				}

				_graphics.Progress = Math.Max(0f, Math.Min(1f, progress));
				_graphics.Text = message;
			}

			base.OnDrawing();
		}

		private void OnProgressUpdated(object sender, EventArgs e)
		{
			// if we didn't get a SynchronizationContext from the thread that calls .Draw(), then we can't update the progress bar
			// also use the pending flag to ensure we don't excessively post a large number of redraws if the events are happening faster than we can draw
			if (_synchronizationContext == null || _drawPending || !Visible) return;

			_drawPending = true;
			_synchronizationContext.Post(s => Draw(), null);
		}

		#region Static Helpers

		/// <summary>
		/// Creates and displays <see cref="ProgressGraphic"/>.
		/// </summary>
		/// <param name="task">The <see cref="BackgroundTask"/> to execute. The task is automatically started if it is not already running.</param>
		/// <param name="parentCollections">The graphics collections on which the progress graphic should be shown.</param>
		/// <param name="autoClose">A value indicating whether or not the progress graphic should be automatically removed when the task is terminated.</param>
		/// <param name="progressBarStyle">The style of the progress bar.</param>
		/// <param name="drawImmediately">Specifies that a draw should be performed immediately after creation (should be set to FALSE if calling from a draw routine in the same scene graph, as that would otherwise cause a <see cref="StackOverflowException"/>.)</param>
		public static void Show(BackgroundTask task, IEnumerable<GraphicCollection> parentCollections, bool autoClose = _defaultAutoClose, ProgressBarGraphicStyle progressBarStyle = _defaultStyle, bool drawImmediately = _defaultDrawImmediately)
		{
			if (!task.IsRunning)
				task.Run();
			var provider = new BackgroundTaskProgressAdapter(task);
			foreach (var parentCollection in parentCollections)
				Show(provider, parentCollection, autoClose, progressBarStyle, drawImmediately);
		}

		/// <summary>
		/// Creates and displays a <see cref="ProgressGraphic"/>.
		/// </summary>
		/// <param name="task">The <see cref="BackgroundTask"/> to execute. The task is automatically started if it is not already running.</param>
		/// <param name="parentCollection">The graphics collection on which the progress graphic should be shown.</param>
		/// <param name="autoClose">A value indicating whether or not the progress graphic should be automatically removed when the task is terminated.</param>
		/// <param name="progressBarStyle">The style of the progress bar.</param>
		/// <param name="drawImmediately">Specifies that a draw should be performed immediately after creation (should be set to FALSE if calling from a draw routine in the same scene graph, as that would otherwise cause a <see cref="StackOverflowException"/>.)</param>
		public static void Show(BackgroundTask task, GraphicCollection parentCollection, bool autoClose = _defaultAutoClose, ProgressBarGraphicStyle progressBarStyle = _defaultStyle, bool drawImmediately = _defaultDrawImmediately)
		{
			if (!task.IsRunning)
				task.Run();
			Show(new BackgroundTaskProgressAdapter(task), parentCollection, autoClose, progressBarStyle, drawImmediately);
		}

		/// <summary>
		/// Creates and displays a <see cref="ProgressGraphic"/>.
		/// </summary>
		/// <param name="source">The source from which progress information is retrieved and displayed.</param>
		/// <param name="parentCollection">The graphics collection on which the progress graphic should be shown.</param>
		/// <param name="autoClose">A value indicating whether or not the progress graphic should be automatically removed when the task is terminated.</param>
		/// <param name="progressBarStyle">The style of the progress bar.</param>
		/// <param name="drawImmediately">Specifies that a draw should be performed immediately after creation (should be set to FALSE if calling from a draw routine in the same scene graph, as that would otherwise cause a <see cref="StackOverflowException"/>.)</param>
		public static void Show(IProgressProvider source, GraphicCollection parentCollection, bool autoClose = _defaultAutoClose, ProgressBarGraphicStyle progressBarStyle = _defaultStyle, bool drawImmediately = _defaultDrawImmediately)
		{
			ProgressGraphic progressGraphic = new ProgressGraphic(source, autoClose, progressBarStyle);
			parentCollection.Add(progressGraphic);
			if (drawImmediately) progressGraphic.Draw();
		}

		/// <summary>
		/// Creates and displays a <see cref="ProgressGraphic"/>.
		/// </summary>
		/// <param name="source">The source from which progress information is retrieved and displayed.</param>
		/// <param name="parentCollection">The graphics collection on which the progress graphic should be shown.</param>
		/// <param name="autoClose">A value indicating whether or not the progress graphic should be automatically removed when the task is terminated.</param>
		/// <param name="progressBarStyle">The style of the progress bar.</param>
		/// <param name="drawImmediately">Specifies that a draw should be performed immediately after creation (should be set to FALSE if calling from a draw routine in the same scene graph, as that would otherwise cause a <see cref="StackOverflowException"/>.)</param>
		[Obsolete("Implement the IProgressProvider interface on the source and use the corresponding overload instead.")]
		public static void Show(IProgressGraphicProgressProvider source, GraphicCollection parentCollection, bool autoClose = _defaultAutoClose, ProgressBarGraphicStyle progressBarStyle = _defaultStyle, bool drawImmediately = _defaultDrawImmediately)
		{
			ProgressGraphic progressGraphic = new ProgressGraphic(source, autoClose, progressBarStyle);
			parentCollection.Add(progressGraphic);
			if (drawImmediately) progressGraphic.Draw();
		}

		#endregion

		#region Adapter Classes

		[Obsolete]
		private class LegacyProgressProviderAdapter : IProgressProvider
		{
			private readonly IProgressGraphicProgressProvider _realProvider;
			private event EventHandler _progressUpdated;
			private Timer _timer;

			public LegacyProgressProviderAdapter(IProgressGraphicProgressProvider realProvider)
			{
				_realProvider = realProvider;
			}

			public event EventHandler ProgressUpdated
			{
				add
				{
					_progressUpdated += value;

					if (_timer == null)
					{
						_timer = new Timer(OnTimer, null, 100);
						_timer.Start();
					}
				}
				remove
				{
					_progressUpdated -= value;

					if (_progressUpdated == null && _timer != null)
					{
						_timer.Dispose();
						_timer = null;
					}
				}
			}

			private void OnTimer(object state)
			{
				EventsHelper.Fire(_progressUpdated, this, EventArgs.Empty);
			}

			public bool GetProgress(out float progress, out string message)
			{
				return _realProvider.IsRunning(out progress, out message);
			}
		}

		private class BackgroundTaskProgressAdapter : IProgressProvider
		{
			private readonly BackgroundTask _backgroundTask;
			private event EventHandler _progressUpdated;
			private DelayedEventPublisher<BackgroundTaskProgressEventArgs> _delayedProgressEventPublisher;

			public BackgroundTaskProgressAdapter(BackgroundTask backgroundTask)
			{
				_backgroundTask = backgroundTask;
			}

			public event EventHandler ProgressUpdated
			{
				add
				{
					_progressUpdated += value;

					if (_delayedProgressEventPublisher == null)
					{
						_delayedProgressEventPublisher = new DelayedEventPublisher<BackgroundTaskProgressEventArgs>(NotifyProgressUpdated, 1000, DelayedEventPublisherTriggerMode.Periodic);
						_backgroundTask.ProgressUpdated += _delayedProgressEventPublisher.Publish;
						_backgroundTask.Terminated += OnTaskTerminated;
					}
				}
				remove
				{
					_progressUpdated -= value;

					if (_progressUpdated == null && _delayedProgressEventPublisher != null)
					{
						_backgroundTask.ProgressUpdated -= _delayedProgressEventPublisher.Publish;
						_backgroundTask.Terminated -= OnTaskTerminated;
						_delayedProgressEventPublisher.Dispose();
						_delayedProgressEventPublisher = null;
					}
				}
			}

			private void NotifyProgressUpdated(object sender, BackgroundTaskProgressEventArgs backgroundTaskProgressEventArgs)
			{
				EventsHelper.Fire(_progressUpdated, this, EventArgs.Empty);
			}

			private void OnTaskTerminated(object sender, BackgroundTaskTerminatedEventArgs e)
			{
				EventsHelper.Fire(_progressUpdated, this, EventArgs.Empty);
			}

			public bool GetProgress(out float progress, out string message)
			{
				progress = 0f;
				message = string.Empty;

				if (_backgroundTask.LastBackgroundTaskProgress != null)
				{
					progress = _backgroundTask.LastBackgroundTaskProgress.Progress.Percent/100f;
					message = _backgroundTask.LastBackgroundTaskProgress.Progress.Message;
				}
				return _backgroundTask.IsRunning;
			}
		}

		#endregion
	}

	#region IProgressProvider Interface

	/// <summary>
	/// Interface for providers of progress information about some abstract operation.
	/// </summary>
	public interface IProgressProvider
	{
		/// <summary>
		/// Fires when updated progress information is available.
		/// </summary>
		event EventHandler ProgressUpdated;

		/// <summary>
		/// Called to get progress information about the abstract operation.
		/// </summary>
		/// <param name="progress">A fractional number between 0 and 1 inclusive indicating the current progress of the abstract operation.</param>
		/// <param name="message">An optional message describing the current progress of the abstract operation.</param>
		/// <returns>A value indicating whether or not the abstract operation is currently running.</returns>
		bool GetProgress(out float progress, out string message);
	}

	/// <summary>
	/// Interface for providers of progress information about some abstract operation.
	/// </summary>
	[Obsolete("Implement the IProgressProvider interface on the source instead.")]
	public interface IProgressGraphicProgressProvider
	{
		/// <summary>
		/// Called to get progress information about the abstract operation.
		/// </summary>
		/// <param name="progress">A fractional number between 0 and 1 inclusive indicating the current progress of the abstract operation.</param>
		/// <param name="message">An optional message describing the current progress of the abstract operation.</param>
		/// <returns>A value indicating whether or not the abstract operation is currently running.</returns>
		bool IsRunning(out float progress, out string message);
	}

	#endregion
}
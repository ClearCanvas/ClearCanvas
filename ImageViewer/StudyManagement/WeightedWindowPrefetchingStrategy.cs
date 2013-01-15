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
using ClearCanvas.Common.Utilities;
using FrameBlockingThreadPool = ClearCanvas.ImageViewer.Common.BlockingThreadPool<ClearCanvas.ImageViewer.StudyManagement.Frame>;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A base implementation for an <see cref="IPrefetchingStrategy"/> based on a window of frames around a visible frame,
	/// with preference given to selected and unselected <see cref="IImageBox"/>es depending on a weighting factor.
	/// </summary>
	public class WeightedWindowPrefetchingStrategy : PrefetchingStrategy
	{
		private readonly ICorePrefetchingStrategy _coreStrategy;
		private ViewerFrameEnumerator _imageBoxEnumerator;
		private FrameBlockingThreadPool _retrieveThreadPool;
		private SimpleBlockingThreadPool _decompressThreadPool;
		private bool _isStarted = false;
		private bool _enabled = true;

		private ThreadPriority _retrievalThreadPriority = ThreadPriority.BelowNormal;
		private ThreadPriority _decompressionThreadPriority = ThreadPriority.Lowest;
		private int _retrievalThreadConcurrency = 5;
		private int _decompressionThreadConcurrency = 1;
		private int? _frameLookAheadCount = 20;
		private int _selectedImageBoxWeight = 3;
		private int _unselectedImageBoxWeight = 2;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">A unique name for the strategy, normally corresponding to the name of the parent <see cref="IStudyLoader"/>.</param>
		/// <param name="description">An informative description.</param>
		public WeightedWindowPrefetchingStrategy(string name, string description)
			:this(new SimpleCorePrefetchingStrategy(), name, description)
		{
		}

		/// <summary>
		/// Initializes a new instance of a <see cref="WeightedWindowPrefetchingStrategy"/>.
		/// </summary>
		/// <param name="coreStrategy">The core prefetching strategy used to load images.</param>
		/// <param name="name">The name of the <see cref="IPrefetchingStrategy"/>.</param>
		/// <param name="description">The description of the <see cref="IPrefetchingStrategy"/>.</param>
		public WeightedWindowPrefetchingStrategy(ICorePrefetchingStrategy coreStrategy, string name, string description)
			: base(name, description)
		{
			Platform.CheckForNullReference(coreStrategy, "coreStrategy");
			_coreStrategy = coreStrategy;
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not the strategy is enabled.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if an attempt was made to set the property when the strategy has already been started.</exception>
		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				CheckNotStarted();
				_enabled = value;
			}
		}

		/// <summary>
		/// Gets or sets the priority of the threads to be used for frame pixel data retrieval.
		/// </summary>
		/// <remarks>It is strongly recommended that <see cref="ThreadPriority.AboveNormal"/> and <see cref="ThreadPriority.Highest"/> not be used.</remarks>
		/// <exception cref="InvalidOperationException">Thrown if an attempt was made to set the property when the strategy has already been started.</exception>
		public ThreadPriority RetrievalThreadPriority
		{
			get { return _retrievalThreadPriority; }
			set
			{
				CheckNotStarted();
				_retrievalThreadPriority = value;
			}
		}

		/// <summary>
		/// Gets or sets the priority of the threads to be used for frame pixel data compression.
		/// </summary>
		/// <remarks>It is strongly recommended that <see cref="ThreadPriority.AboveNormal"/> and <see cref="ThreadPriority.Highest"/> not be used.</remarks>
		/// <exception cref="InvalidOperationException">Thrown if an attempt was made to set the property when the strategy has already been started.</exception>
		public ThreadPriority DecompressionThreadPriority
		{
			get { return _decompressionThreadPriority; }
			set
			{
				CheckNotStarted(); 
				_decompressionThreadPriority = value;
			}
		}

		/// <summary>
		/// Gets or sets the number of concurrent threads to be used for frame pixel data retrieval. Valid range is 1 or more.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if an attempt was made to set the property to an invalid value.</exception>
		/// <exception cref="InvalidOperationException">Thrown if an attempt was made to set the property when the strategy has already been started.</exception>
		public int RetrievalThreadConcurrency
		{
			get { return _retrievalThreadConcurrency; }
			set
			{
				CheckNotStarted();
				Platform.CheckPositive(value, "RetrievalThreadConcurrency");
				_retrievalThreadConcurrency = value;
			}
		}

		/// <summary>
		/// Gets or sets the number of concurrent threads to be used for frame pixel data decompression. Valid range is 0 or more.
		/// </summary>
		/// <remarks>Setting the thread concurrency to 0 effectively means that decompression will not be done ahead of time.</remarks>
		/// <exception cref="ArgumentException">Thrown if an attempt was made to set the property to an invalid value.</exception>
		/// <exception cref="InvalidOperationException">Thrown if an attempt was made to set the property when the strategy has already been started.</exception>
		public int DecompressionThreadConcurrency
		{
			get { return _decompressionThreadConcurrency; }
			set
			{
				CheckNotStarted();
				Platform.CheckNonNegative(value, "DecompressionThreadConcurrency");
				_decompressionThreadConcurrency = value;
			}
		}

		/// <summary>
		/// Gets or sets the number of frames to process in advance. Valid range is 0 or more.
		/// </summary>
		/// <remarks>
		/// <para>The look-ahead is performed in both directions from the currently selected frame. The maximum possible window size would thus be 2 times the look-ahead size.</para>
		/// <para>Setting the look-ahead size to null specifies an infinite look-ahead scope.</para>
		/// </remarks>
		/// <exception cref="ArgumentException">Thrown if an attempt was made to set the property to an invalid value.</exception>
		/// <exception cref="InvalidOperationException">Thrown if an attempt was made to set the property when the strategy has already been started.</exception>
		public int? FrameLookAheadCount
		{
			get { return _frameLookAheadCount; }
			set
			{
				CheckNotStarted(); 
				if (value.HasValue)
					Platform.CheckNonNegative(value.Value, "FrameLookAheadCount");
				_frameLookAheadCount = value;
			}
		}

		/// <summary>
		/// Gets or sets the relative weighting given to loading frames in the selected <see cref="IImageBox"/>. Valid range is 1 or more.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if an attempt was made to set the property to an invalid value.</exception>
		/// <exception cref="InvalidOperationException">Thrown if an attempt was made to set the property when the strategy has already been started.</exception>
		public int SelectedImageBoxWeight
		{
			get { return _selectedImageBoxWeight; }
			set
			{
				CheckNotStarted();
				Platform.CheckPositive(value, "SelectedImageBoxWeight");
				_selectedImageBoxWeight = value;
			}
		}

		/// <summary>
		/// Gets or sets the relative weighting given to loading frames in unselected <see cref="IImageBox"/>. Valid range is 0 or more.
		/// </summary>
		/// <remarks>Setting the relative weight to 0 effectively means that only frames in the selected <see cref="IImageBox"/> will be preloaded.</remarks>
		/// <exception cref="ArgumentException">Thrown if an attempt was made to set the property to an invalid value.</exception>
		/// <exception cref="InvalidOperationException">Thrown if an attempt was made to set the property when the strategy has already been started.</exception>
		public int UnselectedImageBoxWeight
		{
			get { return _unselectedImageBoxWeight; }
			set
			{
				CheckNotStarted();
				Platform.CheckNonNegative(value, "UnselectedImageBoxWeight");
				_unselectedImageBoxWeight = value;
			}
		}

		protected override void Start()
		{
			if (!_enabled)
				return;

			_isStarted = true;

			_imageBoxEnumerator = new ViewerFrameEnumerator(ImageViewer, SelectedImageBoxWeight, UnselectedImageBoxWeight, FrameLookAheadCount.GetValueOrDefault(-1), CanRetrieveFrame);

			_retrieveThreadPool = new FrameBlockingThreadPool(_imageBoxEnumerator, DoRetrieveFrame)
			                      	{
			                      		ThreadPoolName = GetThreadPoolName("Retrieve"),
			                      		Concurrency = RetrievalThreadConcurrency,
			                      		ThreadPriority = _retrievalThreadPriority
			                      	};
			_retrieveThreadPool.Start();

			if (DecompressionThreadConcurrency <= 0)
				return;

			_decompressThreadPool = new SimpleBlockingThreadPool(DecompressionThreadConcurrency)
			                        	{
			                        		ThreadPoolName = GetThreadPoolName("Decompress"),
			                        		ThreadPriority = _decompressionThreadPriority
			                        	};
			_decompressThreadPool.Start();
		}

		protected override void Stop()
		{
			if (_retrieveThreadPool != null)
			{
				_retrieveThreadPool.Stop(false);
				_retrieveThreadPool = null;
			}

			if (_decompressThreadPool != null)
			{
				_decompressThreadPool.Stop(false);
				_decompressThreadPool = null;
			}

			if (_imageBoxEnumerator != null)
			{
				_imageBoxEnumerator.Dispose();
				_imageBoxEnumerator = null;
			}

			_isStarted = false;
		}

		private bool CanRetrieveFrame(Frame frame)
		{
			return _coreStrategy.CanRetrieveFrame(frame);
		}

		private void RetrieveFrame(Frame frame)
		{
			_coreStrategy.RetrieveFrame(frame);
		}

		private bool CanDecompressFrame(Frame frame)
		{
			return _decompressThreadPool != null &&  _coreStrategy.CanDecompressFrame(frame);
		}

		private void DecompressFrame(Frame frame)
		{
			_coreStrategy.DecompressFrame(frame);
		}

		private void DoRetrieveFrame(Frame frame)
		{
			try
			{
				RetrieveFrame(frame);

				if (CanDecompressFrame(frame))
					_decompressThreadPool.Enqueue(() => DoDecompressFrame(frame));
			}
			catch (Exception ex)
			{
				// don't let an uncaught exception crash the entire preloader
				Platform.Log(LogLevel.Warn, ex, "An error occured while trying to preload a frame.");
			}
		}

		private void DoDecompressFrame(Frame frame)
		{
			try
			{
				DecompressFrame(frame);
			}
			catch (Exception ex)
			{
				// don't let an uncaught exception crash the entire preloader
				Platform.Log(LogLevel.Warn, ex, "An error occured while trying to preload a frame.");
			}
		}

		private string GetThreadPoolName(string operationName)
		{
			var strategyName = Name;
			if (string.IsNullOrEmpty(strategyName))
				strategyName = GetType().FullName;
			return string.Format("{0}:{1}", strategyName, operationName);
		}

		private void CheckNotStarted()
		{
			if (_isStarted)
				throw new InvalidOperationException("Parameter may not be changed once the preloading strategy has been started.");
		}
	}
}
#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Linq;
using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <remarks>
	/// This type is part of a trial API and is not intended for general use.
	/// </remarks>
	[Cloneable(false)]
	public class AsyncDicomGrayscalePresentationImage : DicomGrayscalePresentationImage, IProgressProvider
	{
		[CloneIgnore]
		private readonly object _waitPixelData = new object();

		[CloneIgnore]
		private DelayedEventPublisher _delayedEventPublisher;

		public AsyncDicomGrayscalePresentationImage(AsyncFrame frame)
			: base(frame)
		{
			Initialize();
		}

		protected AsyncDicomGrayscalePresentationImage(DicomGrayscalePresentationImage source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);

			Initialize();
		}

		private void Initialize()
		{
			var asyncFrame = Frame;
			asyncFrame.AsyncProgressChanged += OnAsyncProgressChanged;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				var asyncFrame = Frame;
				asyncFrame.AsyncProgressChanged -= OnAsyncProgressChanged;

				if (_delayedEventPublisher != null)
				{
					_delayedEventPublisher.Dispose();
					_delayedEventPublisher = null;
				}
			}
			base.Dispose(disposing);
		}

		public string LoadingMessageFormat { get; set; }

		public new AsyncFrame Frame
		{
			get { return (AsyncFrame) base.Frame; }
		}

		#region Progress Provider Implementation

		private event EventHandler _progressUpdated;

		private void OnAsyncProgressChanged(object sender, AsyncPixelDataProgressEventArgs e)
		{
			if (_delayedEventPublisher != null)
				_delayedEventPublisher.Publish(sender, e);
		}

		private void OnDelayedProgressChanged(object sender, EventArgs e)
		{
			if (Visible)
				EventsHelper.Fire(_progressUpdated, this, new EventArgs());
		}

		event EventHandler IProgressProvider.ProgressUpdated
		{
			add { _progressUpdated += value; }
			remove { _progressUpdated -= value; }
		}

		bool IProgressProvider.GetProgress(out float progress, out string message)
		{
			var asyncFrame = Frame;

			var messageFormat = LoadingMessageFormat;
			if (string.IsNullOrEmpty(messageFormat))
				messageFormat = SR.MessageFormatReloading;

			progress = asyncFrame.AsyncProgressPercent/100f;
			message = string.Format(messageFormat, asyncFrame.AsyncProgressPercent);
			return !asyncFrame.IsAsyncLoaded;
		}

		#endregion

		public override void Draw(DrawArgs drawArgs)
		{
			if (_delayedEventPublisher == null && SynchronizationContext.Current != null)
				_delayedEventPublisher = new DelayedEventPublisher(OnDelayedProgressChanged, 1000, DelayedEventPublisherTriggerMode.Periodic);

			var asyncFrame = Frame;
			using (asyncFrame.AcquireLock())
			{
				if (!asyncFrame.IsAsyncLoaded)
				{
					if (!Visible) // if this is an off-screen draw, wait for data to be loaded
					{
						lock (_waitPixelData)
						{
							if (!asyncFrame.IsAsyncLoaded)
							{
								var completionHandler = new EventHandler((s, e) =>
								                                         	{
								                                         		lock (_waitPixelData)
								                                         		{
								                                         			Monitor.Pulse(_waitPixelData);
								                                         		}
								                                         	});
								var onFrameAsyncLoaded = new AsyncPixelDataEventHandler(completionHandler);
								var onFrameAsyncFaulted = new AsyncPixelDataFaultEventHandler(completionHandler);

								asyncFrame.AsyncLoaded += onFrameAsyncLoaded;
								asyncFrame.AsyncFaulted += onFrameAsyncFaulted;
								asyncFrame.GetNormalizedPixelData();

								// check the flag again, in case the event actually fired before we hooked up the handler
								if (!asyncFrame.IsAsyncLoaded)
									Monitor.Wait(_waitPixelData);

								asyncFrame.AsyncLoaded -= onFrameAsyncLoaded;
								asyncFrame.AsyncFaulted -= onFrameAsyncFaulted;
							}
						}
					}
					else if (!ApplicationGraphics.OfType<ProgressGraphic>().Any())
					{
						ProgressGraphic.Show(this, ApplicationGraphics, true, ProgressBarGraphicStyle.Continuous, false);
					}
				}

				base.Draw(drawArgs);
			}
		}
	}
}
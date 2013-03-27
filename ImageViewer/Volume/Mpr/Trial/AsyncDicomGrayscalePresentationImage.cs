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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <remarks>
	/// This type is part of a trial API and is not intended for general use.
	/// </remarks>
	[Cloneable(false)]
	public class AsyncDicomGrayscalePresentationImage : DicomGrayscalePresentationImage
	{
		[CloneIgnore]
		private readonly object _waitPixelData = new object();

		[CloneIgnore]
		private SynchronizationContext _synchronizationContext;

		[CloneIgnore]
		private ProgressProvider _progressProvider;

		public AsyncDicomGrayscalePresentationImage(Frame frame)
			: base(frame)
		{
			Initialize();
		}

		public AsyncDicomGrayscalePresentationImage(IFrameReference frameReference)
			: base(frameReference)
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
			_progressProvider = new ProgressProvider(this);

			var asyncFrame = Frame as AsyncFrame;
			if (asyncFrame != null) asyncFrame.AsyncLoaded += AsyncDicomGrayscalePresentationImage_AsyncLoaded;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_progressProvider != null)
				{
					_progressProvider.Dispose();
					_progressProvider = null;
				}

				var asyncFrame = Frame as AsyncFrame;
				if (asyncFrame != null) asyncFrame.AsyncLoaded -= AsyncDicomGrayscalePresentationImage_AsyncLoaded;
			}
			base.Dispose(disposing);
		}

		public string LoadingMessageFormat { get; set; }

		private void AsyncDicomGrayscalePresentationImage_AsyncLoaded(object sender, AsyncPixelDataEventArgs e)
		{
			if (Tile != null && _synchronizationContext != null)
			{
				//_synchronizationContext.Post(s=>Tile.Draw(), null);
			}
		}

		public override void Draw(DrawArgs drawArgs)
		{
			var asyncFrame = Frame as AsyncFrame;
			if (asyncFrame != null)
			{
				if (_synchronizationContext == null)
					_synchronizationContext = SynchronizationContext.Current;

				if (!asyncFrame.IsAsyncLoaded)
				{
					if (Tile == null)
					{
						lock (_waitPixelData)
						{
							if (!asyncFrame.IsAsyncLoaded)
							{
								var onFrameAsyncLoaded = new AsyncPixelDataEventHandler((s, e) =>
								                                                        	{
								                                                        		lock (_waitPixelData)
								                                                        		{
								                                                        			Monitor.Pulse(_waitPixelData);
								                                                        		}
								                                                        	});
								asyncFrame.AsyncLoaded += onFrameAsyncLoaded;
								asyncFrame.GetNormalizedPixelData();
								Monitor.Wait(_waitPixelData);
								asyncFrame.AsyncLoaded -= onFrameAsyncLoaded;
							}
						}
					}
					else if (!ApplicationGraphics.OfType<ProgressGraphic>().Any())
					{
						ProgressGraphic.Show(_progressProvider, ApplicationGraphics, true, ProgressBarGraphicStyle.Continuous, false);
					}
				}
			}

			base.Draw(drawArgs);
		}

		private class ProgressProvider : IProgressProvider, IDisposable
		{
			private readonly AsyncDicomGrayscalePresentationImage _owner;

			public ProgressProvider(AsyncDicomGrayscalePresentationImage owner)
			{
				_owner = owner;

				var asyncFrame = _owner.Frame as AsyncFrame;
				if (asyncFrame != null) asyncFrame.AsyncProgressChanged += OnAsyncProgressChanged;
			}

			public void Dispose()
			{
				var asyncFrame = _owner.Frame as AsyncFrame;
				if (asyncFrame != null) asyncFrame.AsyncProgressChanged -= OnAsyncProgressChanged;
			}

			private void OnAsyncProgressChanged(object sender, AsyncPixelDataProgressEventArgs e)
			{
				EventsHelper.Fire(ProgressUpdated, this, new EventArgs());
			}

			#region Implementation of IProgressProvider

			public event EventHandler ProgressUpdated;

			public bool GetProgress(out float progress, out string message)
			{
				var asyncFrame = _owner.Frame as AsyncFrame;
				if (asyncFrame != null)
				{
					var messageFormat = _owner.LoadingMessageFormat;
					if (string.IsNullOrEmpty(messageFormat))
						messageFormat = Volume.Mpr.SR.MessageFormatReloading;

					progress = asyncFrame.AsyncProgressPercent/100f;
					message = string.Format(messageFormat, asyncFrame.AsyncProgressPercent);
					return !asyncFrame.IsAsyncLoaded;
				}
				else
				{
					progress = 100f;
					message = string.Empty;
					return false;
				}
			}

			#endregion
		}
	}
}
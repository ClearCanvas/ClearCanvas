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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyLoaders.Streaming;
using ClearCanvas.ImageViewer.StudyManagement;
using System;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using System.Collections.Generic;
using System.Threading;

namespace ClearCanvas.ImageViewer.TestTools
{
	[ExtensionPoint]
	public sealed class StreamingAnalysisComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(StreamingAnalysisComponentViewExtensionPoint))]
	public class StreamingAnalysisComponent : ApplicationComponent
	{
		private int _retrieveConcurrency = 5;
		private int _decompressConcurrency = 5;

		private List<StreamingPerformanceInfo> _performanceInfo = new List<StreamingPerformanceInfo>();
		private SynchronizationContext _synchronizationContext;
		private readonly IStudyBrowserToolContext _browserToolContext;
		private BlockingCollection<IFrameReference> _framesToRetrieve;
		private BlockingCollection<IFrameReference> _framesToDecompress;
		private ImageViewer.Common.BlockingThreadPool<IFrameReference> _retrievePool;
		private ImageViewer.Common.BlockingThreadPool<IFrameReference> _decompressPool;

		public StreamingAnalysisComponent(IStudyBrowserToolContext browserToolContext)
		{
			_browserToolContext = browserToolContext;
		}

		public IList<StreamingPerformanceInfo> PerformanceInfo
		{
			get { return _performanceInfo.AsReadOnly(); }
		}

		public bool RetrieveActive
		{
			get { return _retrievePool.Active; }
			set
			{
				if (_retrievePool.Active)
					_retrievePool.Stop(false);
				else 
					_retrievePool.Start();

				NotifyPropertyChanged("RetrieveActive");
			}
		}

		public int RetrieveConcurrency
		{
			get { return _retrieveConcurrency; }
			set
			{
				if (_retrieveConcurrency != value)
				{
					bool start = _retrievePool.Active;
					_retrievePool.Stop(false);
					_retrieveConcurrency = value;
					_retrievePool.Concurrency = _retrieveConcurrency;
					if (start)
						_retrievePool.Start();

					NotifyPropertyChanged("RetrieveConcurrency");
				}
			}
		}

		public ThreadPriority RetrieveThreadPriority
		{
			get { return _retrievePool.ThreadPriority; }
			set
			{
				if (RetrieveThreadPriority != value)
				{
					bool start = _retrievePool.Active;
					_retrievePool.Stop(false);
					_retrievePool.ThreadPriority = value;
					if (start)
						_retrievePool.Start();

					NotifyPropertyChanged("RetrieveThreadPriority");
				}
			}
		}

		public int NumberOfRetrieveItems
		{
			get { return _framesToRetrieve.Count; }	
		}

		public bool DecompressActive
		{
			get { return _decompressPool.Active; }
			set
			{
				if (_decompressPool.Active)
					_decompressPool.Stop(false);
				else
					_decompressPool.Start();

				NotifyPropertyChanged("DecompressActive");
			}
		}

		public int DecompressConcurrency
		{
			get { return _decompressConcurrency; }
			set
			{
				if (_decompressConcurrency != value)
				{
					bool start = _decompressPool.Active;
					_decompressPool.Stop(false);
					_decompressConcurrency = value;
					_decompressPool.Concurrency = _decompressConcurrency;
					if (start)
						_decompressPool.Start();
					
					NotifyPropertyChanged("DecompressConcurrency");
				}
			}
		}

		public ThreadPriority DecompressThreadPriority
		{
			get { return _decompressPool.ThreadPriority; }
			set
			{
				if (DecompressThreadPriority != value)
				{
					bool start = _decompressPool.Active;
					_decompressPool.Stop(false);
					_decompressPool.ThreadPriority = value;
					if (start)
						_decompressPool.Start();

					NotifyPropertyChanged("DecompressThreadPriority");
				}
			}
		}
		public int NumberOfDecompressItems
		{
			get { return _framesToDecompress.Count; }
		}

		public bool CanAddSelectedStudies
		{
			get
			{
				if (_browserToolContext.SelectedStudy == null)
					return false;

				foreach (var study in _browserToolContext.SelectedStudies)
				{
					if (study.Server == null)
						return false;

                    var ae = (IApplicationEntity)study.Server;
					if (ae.StreamingParameters == null)
						return false;
				}

				return true;
			}
		}

		public void AddSelectedStudies()
		{
			BlockingOperation.Run(delegate
			                      	{
			                      		foreach (var study in _browserToolContext.SelectedStudies)
			                      			LoadStudy(study);
			                      	});
		}

		public void ClearRetrieveItems()
		{
			_performanceInfo.Clear();

			bool restart = _retrievePool.Active;
			_retrievePool.Stop(false);

			List<IFrameReference> remaining;
			_framesToRetrieve.Clear(out remaining);
			foreach (IFrameReference reference in remaining)
				reference.Dispose();

			if (restart)
				_retrievePool.Start();

			NotifyPropertyChanged("NumberOfRetrieveItems");
		}

		public void ClearDecompressItems()
		{
			bool restart = _decompressPool.Active;
			_decompressPool.Stop(false);

			List<IFrameReference> remaining;
			_framesToDecompress.Clear(out remaining);
			foreach (IFrameReference reference in remaining)
				reference.Dispose();

			if (restart)
				_decompressPool.Start();

			NotifyPropertyChanged("NumberOfDecompressItems");
		}

		public override void Start()
		{
			_synchronizationContext = SynchronizationContext.Current;
			_browserToolContext.SelectedStudyChanged += new EventHandler(OnSelectedStudyChanged);
			_framesToRetrieve = new BlockingCollection<IFrameReference>();
			_retrievePool = new ImageViewer.Common.BlockingThreadPool<IFrameReference>(_framesToRetrieve, RetrieveFrame);
			_retrievePool.Concurrency = _retrieveConcurrency;

			_framesToDecompress = new ImageViewer.Common.BlockingCollection<IFrameReference>();
			_decompressPool = new ImageViewer.Common.BlockingThreadPool<IFrameReference>(_framesToDecompress, DecompressFrame);
			_decompressPool.Concurrency = _decompressConcurrency;

			base.Start();
		}

		public override void Stop()
		{
			_retrievePool.Stop(false);
			_decompressPool.Stop(false);

			ClearRetrieveItems();
			ClearDecompressItems();

			base.Stop();
		}

		private void LoadStudy(StudyTableItem study)
		{
			List<IFrameReference> frames = new List<IFrameReference>();

		    var loader = study.Server.GetService<IStudyLoader>();
			loader.Start(new StudyLoaderArgs(study.StudyInstanceUid, study.Server, null));
			Sop sop;
			while ((sop = loader.LoadNextSop()) != null)
			{
				using (sop)
				{
					if (sop.IsImage)
					{
						foreach (Frame frame in ((ImageSop)sop).Frames)
							frames.Add(frame.CreateTransientReference());
					}
				}
			}

			_framesToRetrieve.AddRange(frames);
			NotifyPropertyChanged("NumberOfRetrieveItems");
		}

		private void RetrieveFrame(IFrameReference frameReference)
		{
			try
			{

				Frame frame = frameReference.Frame;
				IStreamingSopFrameData frameData =
					(IStreamingSopFrameData)frame.ParentImageSop.DataSource.GetFrameData(frame.FrameNumber);
				frameData.RetrievePixelData();

				_framesToDecompress.Add(frameReference);
				_synchronizationContext.Post(OnRetrievedFrame, frameData.LastRetrievePerformanceInfo);
			}
			catch (Exception e)
			{
				frameReference.Dispose();
				Platform.Log(LogLevel.Error, e);
			}
		}

		private void DecompressFrame(IFrameReference frameReference)
		{
			try
			{
				frameReference.Frame.GetNormalizedPixelData();
				_synchronizationContext.Post(OnDecompressedFrame, frameReference);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
			finally
			{
				frameReference.Dispose();
			}
		}

		private void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			NotifyPropertyChanged("CanAddSelectedStudies");
		}

		private void OnRetrievedFrame(object frameDataTransferInfo)
		{
			StreamingPerformanceInfo transferInfo = frameDataTransferInfo as StreamingPerformanceInfo;
			if (transferInfo != null)
				_performanceInfo.Add(transferInfo);

			NotifyPropertyChanged("NumberOfRetrieveItems");
			NotifyPropertyChanged("NumberOfDecompressItems");
		}

		private void OnDecompressedFrame(object nothing)
		{
			NotifyPropertyChanged("NumberOfDecompressItems");
		}
	}
}

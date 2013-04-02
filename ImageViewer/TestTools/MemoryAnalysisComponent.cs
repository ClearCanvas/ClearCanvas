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
using System.Linq;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using Timer=ClearCanvas.Common.Utilities.Timer;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.TestTools
{
	[ExtensionPoint]
	public sealed class MemoryAnalysisComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(MemoryAnalysisComponentViewExtensionPoint))]
	public class MemoryAnalysisComponent : ApplicationComponent
	{
	    private class LargeObject : ILargeObjectContainer
	    {
	        private readonly LargeObjectContainerData _data;
	        private volatile byte[] _bytes;

	        public LargeObject(byte[] bytes)
	        {
	            _bytes = bytes;
	            _data = new LargeObjectContainerData(Guid.NewGuid())
	                        {
	                            BytesHeldCount = _bytes.Length,
	                            LargeObjectCount = 1,
	                            RegenerationCost = RegenerationCost.Low
	                        };
	            _data.UpdateLastAccessTime();

	            MemoryManager.Add(this);
	        }

            public byte[] Buffer { get { return _bytes; } }

    	    #region ILargeObjectContainer Members

            public Guid Identifier
            {
                get { return _data.Identifier; }
            }

            public int LargeObjectCount
            {
                get { return _data.LargeObjectCount; }
            }

            public long BytesHeldCount
            {
                get { return _data.BytesHeldCount; }
            }

            public DateTime LastAccessTime
            {
                get { return _data.LastAccessTime; }
            }

            public RegenerationCost RegenerationCost
            {
                get { return _data.RegenerationCost; }
            }

            public bool IsLocked
            {
                get { return _data.IsLocked; }
            }

            public void Lock()
            {
                _data.Lock();
            }

            public void Unlock()
            {
                _data.Unlock();
            }

            public void Unload()
            {
                if (IsLocked)
                    return;

                _bytes = null;
                
                _data.BytesHeldCount = 0;
                _data.LargeObjectCount = 0;
                
                MemoryManager.Remove(this);
            }

            #endregion
        }

        private byte _nextFillNumber;

        private readonly List<byte[]> _heldMemory;

        private readonly List<LargeObject> _largeObjects;
		private double _memoryHeldMB = 0;
        private Timer _timer;

	    private volatile SynchronizationContext _synchronizationContext;
	    private readonly object _waitLock = new object();
        private Thread _refreshProcessInfoThread;

		private double _memoryMark;
		private readonly IDesktopWindow _desktopWindow;

	    public MemoryAnalysisComponent(IDesktopWindow desktopWindow)
		{
			_desktopWindow = desktopWindow;
            _heldMemory = new List<byte[]>();
		    _largeObjects = new List<LargeObject>();

            LargeObjectBufferSizeKB = 512;
		    LargeObjectRepeatCount = 1;

		    MemoryIncrementKB = 512;
		    MemoryRepeatCount = 1;
		}

        public string SystemFreeMemoryMB { get; set; }
        public string ProcessVirtualMemoryMB { get; set; }
	    public string ProcessPrivateBytesMB { get; set; }
	    public string ProcessWorkingSetMB { get; set; }

        public int LargeObjectBufferSizeKB { get; set; }
        public int LargeObjectRepeatCount { get; set; }

		public string TotalLargeObjectMemoryMB
		{
			get
			{
				return (MemoryManager.LargeObjectBytesCount / 1024F / 1024F).ToString("F3");
			}	
		}

		public string HeapMemoryMB
		{
            get { return (GC.GetTotalMemory(false) / 1024F / 1024F).ToString("F3"); }
		}

	    public int MemoryIncrementKB { get; set; }
	    public int MemoryRepeatCount { get; set; }

		public string HeldMemoryMB
		{
			get { return _memoryHeldMB.ToString("F3"); }	
		}

		public double MemoryMarkMB
		{
			get { return _memoryMark; }
		}

		public double MemoryDifferenceMB
		{
			get { return GC.GetTotalMemory(false)/1024F/1024F - _memoryMark; }	
		}

		public void MarkMemory()
		{
		    _memoryMark = GC.GetTotalMemory(true)/1024F/1024F;
			NotifyPropertyChanged("MemoryMarkMB");
			NotifyPropertyChanged("HeapMemoryMB");
			NotifyPropertyChanged("MemoryDifferenceMB");
		}

		public void AddHeldMemory()
		{
			try
			{
                if (MemoryIncrementKB > 0)
                {
                    MemoryManager.Execute(delegate
                                          {
                                              var added = new List<byte[]>();
                                                for (int i = 0; i < MemoryRepeatCount; i++)
                                                {
                                                    var buffer = new byte[MemoryIncrementKB*1024];
                                                    added.Add(buffer);
                                                    _memoryHeldMB += MemoryIncrementKB / 1024F;
                                                }

                                              _heldMemory.AddRange(added);
                                              UseBuffers(added);
                                          });
                }
			}
			catch(OutOfMemoryException)
			{
				_desktopWindow.ShowMessageBox("Out of memory!", MessageBoxActions.Ok);
			}

			NotifyPropertyChanged("HeapMemoryMB");
			NotifyPropertyChanged("HeldMemoryMB");
		}

		public void ReleaseHeldMemory()
		{
			_heldMemory.Clear();
			_memoryHeldMB = 0;
			NotifyPropertyChanged("HeapMemoryMB");
			NotifyPropertyChanged("HeldMemoryMB");
		}

        public void UseHeldMemory()
        {
            var buffers = _heldMemory.ToArray();
            UseBuffers(buffers);
        }

		public void ConsumeMaximumMemory()
		{
			try
			{
				var memory = new List<byte[]>();
				while (true)
				{
                    memory.Add(new byte[MemoryIncrementKB * 1024]);
				}
			}
			catch(OutOfMemoryException)
			{
			}

			NotifyPropertyChanged("HeapMemoryMB");
			NotifyPropertyChanged("HeldMemoryMB");
		}

        public void AddLargeObjects()
        {
            if (LargeObjectBufferSizeKB > 0)
            {
                MemoryManager.Execute(delegate
                                          {
                                              var added = new List<byte[]>();
                                              for (int i = 0; i < LargeObjectRepeatCount; i++)
                                              {
                                                  var buffer = new byte[LargeObjectBufferSizeKB*1024];
                                                  var large = new LargeObject(buffer);
                                                  _largeObjects.Add(large);
                                                  added.Add(buffer);
                                              }

                                              UseBuffers(added);
                                          });
            }

            NotifyPropertyChanged("TotalLargeObjectMemoryMB");
        }

        public void ReleaseAllLargeObjects()
        {
            _largeObjects.Clear();

            bool keepGoing = true;
            EventHandler<MemoryCollectedEventArgs> del = delegate(object sender, MemoryCollectedEventArgs args)
            {
                if (args.IsLast)
                    keepGoing = args.BytesCollectedCount > 0;
            };

            MemoryManager.MemoryCollected += del;

            try
            {

                MemoryManager.Execute(delegate
                {
                    if (keepGoing)
                        throw new OutOfMemoryException();
                }, TimeSpan.FromSeconds(30));
            }
            catch (Exception)
            {
            }
            finally
            {
                MemoryManager.MemoryCollected -= del;
            }

            GC.Collect();
            NotifyPropertyChanged("TotalLargeObjectMemoryMB");
        }

        public void ReleaseHeldLargeObjects()
        {
            _largeObjects.Clear();
            GC.Collect();
            NotifyPropertyChanged("TotalLargeObjectMemoryMB");
        }

        public void UseLargeObjects()
        {
            var buffers = _largeObjects.Select(l => l.Buffer).ToArray().Where(b => b != null).ToArray();
            UseBuffers(buffers);
        }

		public void Collect()
		{
			MemoryManager.Collect(true);
			GC.Collect();
		}

		public override void Start()
		{
			base.Start();

            OnTimer(null);
            
            _timer = new Timer(OnTimer, null, 2000);
			_timer.Start();

		    _synchronizationContext = SynchronizationContext.Current;
            _refreshProcessInfoThread = new Thread(PollProcessInfo);
            _refreshProcessInfoThread.Start();
		}

		public override void Stop()
		{
			base.Stop();

			_timer.Stop();
			_timer.Dispose();
			_timer = null;

		    _synchronizationContext = null;
            lock (_waitLock) { Monitor.Pulse(_waitLock); }
            _refreshProcessInfoThread.Join();
		}

		private void OnTimer(object nothing)
		{
			if (_timer != null)
			{
                NotifyPropertyChanged("HeapMemoryMB");
				NotifyPropertyChanged("MemoryDifferenceMB");
				NotifyPropertyChanged("TotalLargeObjectMemoryMB");
			}
		}

        private void PollProcessInfo(object ignore)
        {
            do
            {
                var sync = _synchronizationContext;
                if (sync == null)
                    break;

                    //Calling refresh takes a long time, which is why it's async
                    Process process = Process.GetCurrentProcess();
                    process.Refresh();

                    var systemFree = SystemResources.GetAvailableMemory(SizeUnits.Bytes);

                    sync.Post(delegate
                                  {
                                      if (_synchronizationContext == null)
                                          return;

                                      SystemFreeMemoryMB = String.Format("{0:F3}", systemFree / 1024F / 1024F);
                                      ProcessPrivateBytesMB = String.Format("{0:F3}", process.PrivateMemorySize64/1024F/1024F);
                                      ProcessVirtualMemoryMB = String.Format("{0:F3}", process.VirtualMemorySize64/1024F/1024F);
                                      ProcessWorkingSetMB = String.Format("{0:F3}", process.WorkingSet64/1024F/1024F);

                                      NotifyPropertyChanged("SystemFreeMemoryMB");
                                      NotifyPropertyChanged("ProcessPrivateBytesMB");
                                      NotifyPropertyChanged("ProcessVirtualMemoryMB");
                                      NotifyPropertyChanged("ProcessWorkingSetMB");

                                      CleanupDeadLargeObjects();

                                  }, null);

                lock (_waitLock)
                {
                    Monitor.Wait(_waitLock, 3000);
                }

            } while(true);
        }

	    private void CleanupDeadLargeObjects()
	    {
	        var deadOnes = _largeObjects.Where(l => l.Buffer == null).ToArray();
            foreach (var deadOne in deadOnes)
                _largeObjects.Remove(deadOne);
	    }

	    private void UseBuffers(ICollection<byte[]> buffers)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                foreach (var buffer in buffers)
                    FillBuffer(buffer);
            }, null);
        }

        private unsafe void FillBuffer(byte[] buffer)
        {
            var fill = _nextFillNumber++;
            fixed (byte* p = buffer)
            {
                byte* pb = p;
                for (int i = 0; i < buffer.Length; ++i)
                    *pb++ = fill;
            }
        }
	}
}

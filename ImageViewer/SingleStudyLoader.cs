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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Threading;

namespace ClearCanvas.ImageViewer
{
	public partial class ImageViewerComponent
	{
		internal class SingleStudyLoader : IDisposable
		{
			private readonly SynchronizationContext _uiThreadContext;
			private readonly ImageViewerComponent _viewer;
			private readonly LoadStudyArgs _args;
			private readonly StudyItem _studyItem;

		    private string _studyLoaderName;

			private readonly object _syncLock = new object();
			private List<Sop> _sops;
			private bool _disposed;

			public SingleStudyLoader(SynchronizationContext uiThreadContext, ImageViewerComponent viewer, LoadStudyArgs args)
				: this(uiThreadContext, viewer)
			{
				_args = args;
			}

			public SingleStudyLoader(SynchronizationContext uiThreadContext, ImageViewerComponent viewer, StudyItem studyItem)
				: this(uiThreadContext, viewer)
			{
				_studyItem = studyItem;
			}

			private SingleStudyLoader(SynchronizationContext uiThreadContext, ImageViewerComponent viewer)
			{
				Platform.CheckForNullReference(uiThreadContext, "uiThreadContext");

				IsValidPrior = false;
				_uiThreadContext = uiThreadContext;
				_viewer = viewer;
				LoadOnlineOnly = false;
			}

			private bool IsRunningOnUiThread
			{
				get { return _uiThreadContext == SynchronizationContext.Current; }
			}

			private bool IsStudyInStudyTree
			{
				get { return _viewer.StudyTree.GetStudy(StudyInstanceUid) != null; }
			}

			public bool LoadOnlineOnly { get; set; }

			public StudyItem StudyItem
			{
				get { return _studyItem; }	
			}

			public string StudyInstanceUid
			{
				get
				{
					if (_args != null)
						return _args.StudyInstanceUid;
					else
						return _studyItem.StudyInstanceUid;
				}
			}

			public IDicomServiceNode Server
			{
				get
				{
					if (_args != null)
						return _args.Server;
					else
						return _studyItem.Server;
				}
			}

			public bool IsValidPrior { get; private set; }

			public Exception Error { get; private set; }

			public void LoadStudy()
			{
				try
				{
					List<Sop> sops = LoadSops();
					lock (_syncLock)
					{
						if (_disposed)
						{
							DisposeSops(sops);
							return;
						}

						_sops = sops;
					}

					OnSopsLoaded();
				}
				catch (Exception e)
				{
					OnLoadPriorStudyFailed(e);
				}
			}

			private List<Sop> LoadSops()
			{
			    IStudyLoader studyLoader;
			    try
			    {
                    studyLoader = Server.GetService<IStudyLoader>();
                    _studyLoaderName = studyLoader.Name;
                }
			    catch (Exception e)
			    {
                    //For legacy reasons, so code expecting one of these exceptions will still get one.
                    throw new StudyLoaderNotFoundException(e);
			    }

                var args = new StudyLoaderArgs(StudyInstanceUid, Server, _args != null ? _args.StudyLoaderOptions : null);
				int total;
				var sops = new List<Sop>();
				
				try
				{
					if (LoadOnlineOnly && StudyItem != null)
					{
					    // TODO (CR Apr 2012): try to get rid of this.
						//This stinks, but we pre-emptively throw the offline/nearline exception
						//to avoid trying to load a prior when we know it's not online.
						switch (StudyItem.InstanceAvailability)
						{
							case "OFFLINE":
								throw new OfflineLoadStudyException(StudyInstanceUid);
							case "NEARLINE":
								throw new NearlineLoadStudyException(StudyInstanceUid);
							default:
								break;
						}
					}

                    total = studyLoader.Start(args);
					if (total <= 0)
						throw new NotFoundLoadStudyException(args.StudyInstanceUid);
				}
				catch (LoadStudyException)
				{
					throw;
				}
				catch (Exception e)
				{
					throw new LoadStudyException(args.StudyInstanceUid, e);
				}

				try
				{
					while (true)
					{
                        Sop sop = studyLoader.LoadNextSop();
						if (sop == null)
							break;

						sops.Add(sop);
					}

					if (sops.Count == 0)
						throw new LoadStudyException(args.StudyInstanceUid, total, total);

					return sops;
				}
				catch (Exception e)
				{
					DisposeSops(sops);
					throw new LoadStudyException(args.StudyInstanceUid, total, total, e);
				}
			}

			private void AddSops()
			{
				int total = 0;
				int failed = 0;

				//don't try to load something that's already there.
				if (IsStudyInStudyTree)
				{
					Platform.Log(LogLevel.Debug, "Study '{0} already exists in study tree.", StudyInstanceUid);
					return;
				}

				IsValidPrior = true;

				List<Sop> sops = _sops;
				_sops = null;

				foreach (Sop sop in sops)
				{
					try
					{
						_viewer.StudyTree.AddSop(sop);
					}
					catch (SopValidationException e)
					{
						++failed;
						Platform.Log(LogLevel.Error, e);
						sop.Dispose();
					}
					catch (Exception e)
					{
						++failed;
						Platform.Log(LogLevel.Error, e);
					}

					++total;
				}

				Study study = _viewer.StudyTree.GetStudy(StudyInstanceUid);

				LoadStudyException error = null;
				if (failed > 0 || study == null)
					error = new LoadStudyException(StudyInstanceUid, total, failed);

				if (study != null)
				{
					_viewer.EventBroker.OnStudyLoaded(new StudyLoadedEventArgs(study, error));

                    //We have to use the viewer's instance of the loader so that we don't end up with a prefetching
                    //strategy per study loaded into the viewer.
                    IPrefetchingStrategy prefetchingStrategy = _viewer.StudyLoaders[_studyLoaderName].PrefetchingStrategy;
					if (prefetchingStrategy != null)
						prefetchingStrategy.Start(_viewer);
				}

				if (error != null)
					throw error;
			}

			private void OnLoadPriorStudyFailed()
			{
				//don't report for an existing study or one that was partially loaded.
				if (!IsStudyInStudyTree)
				{
					Platform.Log(LogLevel.Error, Error,
						"Failed to load prior study '{0}' from server '{1}'.",
						StudyInstanceUid, Server);

					IsValidPrior = true;
					if (_args != null)
						_viewer.EventBroker.OnStudyLoadFailed(new StudyLoadFailedEventArgs(_args, Error));
					else
						_viewer.EventBroker.OnStudyLoadFailed(new StudyLoadFailedEventArgs(_studyItem, Error));
				}
			}

			private void OnSopsLoaded()
			{
				if (IsRunningOnUiThread)
				{
					lock (_syncLock)
					{
						if (!_disposed)
						{
							try
							{
								AddSops();
								Monitor.Pulse(_syncLock);
							}
							catch (Exception e)
							{
								OnLoadPriorStudyFailed(e);
							}
						}
					}
				}
				else
				{
					lock (_syncLock)
					{
						if (!_disposed)
						{
							_uiThreadContext.Post(delegate { OnSopsLoaded(); }, null);
							Monitor.Wait(_syncLock);
						}
					}
				}
			}
			
			private void OnLoadPriorStudyFailed(object error)
			{
				if (IsRunningOnUiThread)
				{
					lock (_syncLock)
					{
						try
						{
							if (!_disposed)
							{
								Error = (Exception)error;
								OnLoadPriorStudyFailed();
							}
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Error, e, "Unexpected error occurred while firing StudyLoadFailed event.");
						}
						finally
						{
							Monitor.Pulse(_syncLock);
						}
					}
				}
				else
				{
					lock (_syncLock)
					{
						if (!_disposed)
						{
							_uiThreadContext.Post(OnLoadPriorStudyFailed, error);
							Monitor.Wait(_syncLock);
						}
					}
				}
			}

			private static void DisposeSops(List<Sop> sops)
			{
				foreach (Sop sop in sops)
				{
					try
					{
						sop.Dispose();
					}
					catch (Exception ex)
					{
						Platform.Log(LogLevel.Error, ex);
					}
				}

				sops.Clear();
			}

			private void DisposeSops()
			{
				List<Sop> sops;

				lock(_syncLock)
				{
					_disposed = true;
					sops = _sops;
					_sops = null;
					Monitor.Pulse(_syncLock);
				}

				if (sops != null)
					DisposeSops(sops);
			}

		    #region IDisposable Members

			public void Dispose()
			{
				try
				{
					DisposeSops();
                    GC.SuppressFinalize(this);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

			#endregion
		}
	}
}
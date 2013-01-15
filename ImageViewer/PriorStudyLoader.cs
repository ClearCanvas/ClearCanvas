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

using System.Collections.Generic;
using ClearCanvas.ImageViewer.StudyManagement;
using System;
using ClearCanvas.Common;
using System.Threading;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Exception class for when loading prior studies has failed.
	/// </summary>
	/// <remarks>
	/// Because loading of priors is handled automatically by the framework, this class is both
	/// thrown an caught by the framework, but is handled by the <see cref="ExceptionHandler"/>.
	/// This is done to allow for custom exception handling.
	/// </remarks>
	public class LoadPriorStudiesException : LoadMultipleStudiesException
	{
        public LoadPriorStudiesException(ICollection<Exception> exceptions, int totalStudies, bool findResultsComplete)
			: base(FormatMessage(exceptions, totalStudies, findResultsComplete), exceptions, totalStudies)
		{
			FindFailed = false;
            FindResultsComplete = findResultsComplete;
        }

		public LoadPriorStudiesException()
			: base("The query for prior studies has failed.", new List<Exception>(), 0)
		{
			FindFailed = true;
		    FindResultsComplete = false;
		}

        /// <summary>
        /// Gets whether or not the find results can be considered complete.
        /// </summary>
        public readonly bool FindResultsComplete;

		/// <summary>
		/// Gets whether or not it was the find operation that failed (e.g. <see cref="IPriorStudyFinder"/>).
		/// </summary>
		public readonly bool FindFailed;

        private static string FormatMessage(ICollection<Exception> exceptions, int totalStudies, bool findResultsComplete)
		{
            if (!findResultsComplete)
            {
                if (exceptions.Count == 0)
                    return "Prior study search results may be incomplete.";

                return String.Format("Prior study search results may be incomplete, and {0} of {1} prior studies produced one or more errors while loading.", exceptions.Count, totalStudies);
            }

			return String.Format("{0} of {1} prior studies produced one or more errors while loading.", exceptions.Count, totalStudies);
		}
	}

	/// <summary>
	/// Defines the interface for automatic loading of related (or 'prior') studies.
	/// </summary>
	/// <remarks>
	/// The <see cref="ImageViewerComponent"/> automatically loads prior studies
	/// asynchronously in the background so that developers don't have to worry about such details.
	/// The only part of loading priors that can be customized is the search algorithm, by
	/// implementing <see cref="IPriorStudyFinder"/>.
	/// </remarks>
	public interface IPriorStudyLoader
	{
		/// <summary>
		/// Gets whether or not the <see cref="IPriorStudyLoader"></see> is actively searching for and/or loading priors.
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// Occurs when <see cref="IsActive"/> has changed.
		/// </summary>
		event EventHandler IsActiveChanged;
	}

	public partial class ImageViewerComponent
	{
		//NOTE: Async because otherwise the name conflicts with the ImageViewer.PriorStudyLoader property.
		internal class AsynchronousPriorStudyLoader : IPriorStudyLoader
		{
			private readonly ImageViewerComponent _imageViewer;
			private readonly List<SingleStudyLoader> _singleStudyLoaders;

			private volatile bool _isActive;
			private volatile bool _stop;

			private Thread _thread;
			private SynchronizationContext _synchronizationContext;

			private readonly IPriorStudyFinder _priorStudyFinder;
			private StudyItemList _queryResults;
		    private bool _findResultsComplete;
            private bool _findFailed;

			public AsynchronousPriorStudyLoader(ImageViewerComponent imageViewer, IPriorStudyFinder priorStudyFinder)
			{
				_imageViewer = imageViewer;
				_singleStudyLoaders = new List<SingleStudyLoader>();
				_priorStudyFinder = priorStudyFinder ?? PriorStudyFinder.Null;
				_priorStudyFinder.SetImageViewer(_imageViewer);
			}

			#region IPriorStudyLoader

            public bool IsActive { get { return _isActive; } }
		    public event EventHandler IsActiveChanged;

			#endregion

			public void Start()
			{
				if (_thread != null)
					return;

				if (_priorStudyFinder == PriorStudyFinder.Null)
					return;

				_stop = false;
				_isActive = true;
                EventsHelper.Fire(IsActiveChanged, this, EventArgs.Empty);

				_synchronizationContext = SynchronizationContext.Current;
				_thread = new Thread(Run) {Priority = ThreadPriority.BelowNormal, IsBackground = false};
			    _thread.Start();
			}

			public void Stop()
			{
				if (_stop || _thread == null)
					return;

				_stop = true;
				_priorStudyFinder.Cancel();
				DisposeLoaders();
				_thread.Join();
				_thread = null;
			}

			private void Run()
			{
				try
				{
					FindAndAddPriors();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "An unexpected error has occurred while finding/adding prior studies.");
				}
				finally
				{
				    _isActive = false;
					_synchronizationContext.Post(OnComplete, null);
				}
			}

			private void FindAndAddPriors()
			{
				try
				{
				    var result = _priorStudyFinder.FindPriorStudies();
                    if (result == null)
                        return;

                    _findResultsComplete = result.ResultsComplete;
				    _queryResults = result.Studies;
					if (_queryResults.Count == 0)
						return;
				}
				catch (Exception e)
				{
					_queryResults = new StudyItemList();
					_findFailed = true;
				    _findResultsComplete = false;
					Platform.Log(LogLevel.Error, e, "The search for prior studies has failed.");
					return;
				}

				foreach (StudyItem result in _queryResults)
				{
					if (_stop)
						break;

					var loader = new SingleStudyLoader(_synchronizationContext, _imageViewer, result){ LoadOnlineOnly = true };

					_singleStudyLoaders.Add(loader);
					loader.LoadStudy();
				}
			}


			private void OnComplete(object nothing)
			{
                //We set the _isActive member on the worker thread so that code blocking on the UI thread
                //waiting for the flag to be set could see the change without having to "Post" or show a progress
                //dialog, or something to that effect.  However, so client code does not have to worry about multiple
                //threads, we fire this event on the UI thread.
                EventsHelper.Fire(IsActiveChanged, this, EventArgs.Empty);

                if (_stop)
					return;

				try
				{
					VerifyLoadPriors();
				}
				catch(Exception e)
				{
					ExceptionHandler.Report(e, _imageViewer.DesktopWindow);
				}
			}

			private int GetValidPriorCount()
			{
				int count = 0;
				foreach (SingleStudyLoader loader in _singleStudyLoaders)
				{
					if (loader.IsValidPrior)
						++count;
				}

				return count;
			}

			private List<Exception> GetLoadErrors()
			{
				var errors = new List<Exception>();

				foreach (SingleStudyLoader loader in _singleStudyLoaders)
				{
					if (loader.IsValidPrior && loader.Error != null)
						errors.Add(loader.Error);
				}

				return errors;
			}

			private void VerifyLoadPriors()
			{
				if (_findFailed)
					throw new LoadPriorStudiesException();

                var errors = GetLoadErrors();
				if (errors.Count > 0)
					throw new LoadPriorStudiesException(errors, GetValidPriorCount(), _findResultsComplete);

                if (!_findResultsComplete)
                    throw new LoadPriorStudiesException(new List<Exception>(), GetValidPriorCount(), _findResultsComplete);
			}

			private void DisposeLoaders()
			{
				foreach (SingleStudyLoader loader in _singleStudyLoaders)
					loader.Dispose();

				_singleStudyLoaders.Clear();
			}
		}
	}
}
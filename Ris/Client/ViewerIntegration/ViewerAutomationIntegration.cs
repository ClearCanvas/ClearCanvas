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
using System.Linq;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.Automation;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.Ris.Client.ViewerIntegration
{
	[ExceptionPolicyFor(typeof(OpenStudyException))]
	[ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
	internal class FaultExceptionPolicy : IExceptionPolicy
	{
		#region IExceptionPolicy Members

		public void Handle(Exception e, IExceptionHandlingContext exceptionHandlingContext)
		{
			//We told the viewer automation service to handle showing contract fault
			//messages to the user, so we just log the exception.
			Platform.Log(LogLevel.Info, e);
		}

		#endregion
	}

	internal class QueryFailedException : Exception
	{
		public QueryFailedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}

	internal class OpenStudyException : Exception
	{
		public OpenStudyException(IEnumerable<StudyRootStudyIdentifier> studyRootStudyIdentifiers, Exception innerException)
			: base(FormatMessage(studyRootStudyIdentifiers), innerException)
		{
		}

		private static string FormatMessage(IEnumerable<StudyRootStudyIdentifier> studyRootStudyIdentifiers)
		{
			var q = from i in studyRootStudyIdentifiers
					select string.Format("A# {0}, UID {1}", i.AccessionNumber, i.StudyInstanceUid);

			return String.Format("Failed to open studies:\n{0}", string.Join("\n", q));
		}
	}

	[ExtensionOf(typeof(ViewerIntegrationExtensionPoint))]
	public class ViewerAutomationIntegration : IViewerIntegration
	{
		#region ViewerService

		class ViewerService : IDisposable
		{
			private readonly IViewerAutomation _viewerAutomation;
			private readonly IStudyLocatorBridge _studyLocatorBridge;

			public ViewerService()
			{
				_viewerAutomation = Platform.GetService<IViewerAutomation>();
				_studyLocatorBridge = new StudyLocatorBridge(Platform.GetService<IStudyLocator>());
			}

			public IEnumerable<Viewer> GetActiveViewers()
			{
				try
				{
					return _viewerAutomation.GetViewers(new GetViewersRequest()).Viewers;
				}
				catch (FaultException<NoViewersFault>)
				{
					return new Viewer[0];
				}
			}

			public bool ActivateViewer(Viewer viewer)
			{
				try
				{
					_viewerAutomation.ActivateViewer(new ActivateViewerRequest { Viewer = viewer });
					return true;
				}
				catch (FaultException<ViewerNotFoundFault>)
				{
					return false;
				}
			}

			public IList<StudyRootStudyIdentifier> LocateStudiesByUid(IList<string> studyInstanceUids)
			{
				try
				{
					LocateFailureInfo[] failures;
					var studyIdentifiers = _studyLocatorBridge.LocateStudyByInstanceUid(studyInstanceUids, out failures);

					// study locator bridge doesn't necessarily preserve order of inputs -> outputs, so we
					// order the outputs ourselves here to match the order of the inputs.
					// Note however that the output sequence may be shorter than the input sequence, because studies that
					// can't be located are ignored (hence the null check).
					var q = from uid in studyInstanceUids
							let sid = studyIdentifiers.FirstOrDefault(i => i.StudyInstanceUid == uid)
							where sid != null
							select sid;

					return q.ToList();
				}
				catch (FaultException<QueryFailedFault> e)
				{
					throw new QueryFailedException(String.Format("Query failed for studies matching UID {0}.", string.Join(", ", studyInstanceUids)), e);
				}
			}

			public Viewer OpenViewer(StudyRootStudyIdentifier studyRootStudyIdentifier)
			{
				return OpenViewer(new[] { studyRootStudyIdentifier });
			}

			public Viewer OpenViewer(IList<StudyRootStudyIdentifier> studyRootStudyIdentifiers)
			{
				try
				{
					var request = new OpenStudiesRequest
					{
						ActivateIfAlreadyOpen = false,	// we want to control this manually
						ReportFaultToUser = true,
						StudiesToOpen = studyRootStudyIdentifiers.Select(i => new OpenStudyInfo(i)).ToList()
					};

					var result = _viewerAutomation.OpenStudies(request);
					return result.Viewer;
				}
				catch (FaultException<StudyNotFoundFault> e) { throw new OpenStudyException(studyRootStudyIdentifiers, e); }
				catch (FaultException<StudyOfflineFault> e) { throw new OpenStudyException(studyRootStudyIdentifiers, e); }
				catch (FaultException<StudyNearlineFault> e) { throw new OpenStudyException(studyRootStudyIdentifiers, e); }
				catch (FaultException<StudyInUseFault> e) { throw new OpenStudyException(studyRootStudyIdentifiers, e); }
				catch (FaultException<OpenStudiesFault> e) { throw new OpenStudyException(studyRootStudyIdentifiers, e); }
			}

			public void CloseViewer(Viewer viewer)
			{
				try
				{
					_viewerAutomation.CloseViewer(new CloseViewerRequest { Viewer = viewer });
				}
				catch (FaultException<ViewerNotFoundFault>)
				{
					// eat this exception, as it really just means that the user has already closed the viewer workspace
				}
			}

			public void Dispose()
			{
				var a = _viewerAutomation as IDisposable;
				if (a != null)
					a.Dispose();
				_studyLocatorBridge.Dispose();
			}
		}

		#endregion

		#region ViewerInfo class

		class ViewerInfo : IStudyViewer
		{
			private readonly ViewerAutomationIntegration _owner;

			public ViewerInfo(ViewerAutomationIntegration owner, Viewer viewer, string[] studyInstanceUids)
			{
				_owner = owner;
				Viewer = viewer;
				StudyInstanceUids = studyInstanceUids;
			}

			public Viewer Viewer { get; private set; }
			public string[] StudyInstanceUids { get; private set; }

			bool IStudyViewer.Activate()
			{
				return ActivateViewer(this);
			}

			void IStudyViewer.Close()
			{
				_owner.CloseViewer(this);
			}
		}

		#endregion

		private readonly object _openViewersSyncRoot = new object();
		private readonly List<ViewerInfo> _openViewers = new List<ViewerInfo>();
		private readonly Timer _cleanupTimer;
		private const uint CleanupPeriod = 200000;	// every 10 minutes
		//private const uint CleanupPeriod = 600000;	// every 10 minutes

		public ViewerAutomationIntegration()
		{
			_cleanupTimer = new Timer(ignore => OnCleanupTimer(), null, CleanupPeriod, CleanupPeriod);
		}

		#region IViewerIntegration Members

		/// <summary>
		/// Opens the specified studies for viewing.
		/// </summary>
		/// <param name="args"></param>
		/// <returns>One or more instances of a study viewer.</returns>
		/// <remarks>
		/// Call this method to request that studies matching the specified UIDs be
		/// displayed to the user in one or more image viewers.  Note that the implementation
		/// may return an already open viewer matching the specified arguments, if one exists.
		/// </remarks>
		public IStudyViewer[] ViewStudies(ViewStudiesArgs args)
		{
			return args.InstancePerStudy
					? ViewStudiesMultiInstance(args.StudyInstanceUids)
					: ViewStudiesOneInstance(args.StudyInstanceUids);
		}

		#endregion

		private void OnCleanupTimer()
		{
			try
			{
				RemoveDeadViewerEntries();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		private IStudyViewer[] ViewStudiesOneInstance(string[] studyInstanceUids)
		{
			lock (_openViewersSyncRoot)
			{
				using (var service = new ViewerService())
				{
					var activeViewers = service.GetActiveViewers();
					var vi = _openViewers.FirstOrDefault(Both(ViewerContainsAll(studyInstanceUids), ViewerIsActive(activeViewers)));
					if (vi != null)
					{
						service.ActivateViewer(vi.Viewer);
						return new IStudyViewer[] { vi };
					}

					// locate the studies
					// note that any studies that cannot be located are silently ignored
					//
					// only the first ('primary') study in a single viewer is used, as this is the only way to get hanging protocols to apply correctly
					// all priors/linked studies should be loaded automatically via prior/related search anyway, there should probably be some explicit mechanism here
					var locatedStudies = service.LocateStudiesByUid(new[] {studyInstanceUids.FirstOrDefault()});
					if (!locatedStudies.Any())
						return new IStudyViewer[0];

					vi = new ViewerInfo(this, service.OpenViewer(locatedStudies), studyInstanceUids);
					_openViewers.Add(vi);
					return new IStudyViewer[] {vi};
				}
			}
		}

		private IStudyViewer[] ViewStudiesMultiInstance(string[] studyInstanceUids)
		{
			lock (_openViewersSyncRoot)
			{
				using (var service = new ViewerService())
				{
					var activeViewers = service.GetActiveViewers().ToList();

					// for any studies that already have an open viewer, just activate it
					var needViewer = new List<string>();
					foreach (var uid in studyInstanceUids)
					{
						var vi = _openViewers.FirstOrDefault(Both(ViewerHasPrimary(uid), ViewerIsActive(activeViewers)));
						if (vi != null)
							continue;

						needViewer.Add(uid);
					}

					if (needViewer.Any())
					{
						// locate the remaining studies and open new viewers for them
						// note that any studies that cannot be located are silently ignored - no viewer is opened for them
						var locatedStudies = service.LocateStudiesByUid(needViewer);
						foreach (var studyRootStudyIdentifier in locatedStudies)
						{
							var vi = new ViewerInfo(this, service.OpenViewer(studyRootStudyIdentifier),
													new[] { studyRootStudyIdentifier.StudyInstanceUid });
							_openViewers.Add(vi);
							activeViewers.Add(vi.Viewer); // update this list, as it is used again in final query
						}
					}

					// important to return viewers in same order as input array,
					// which is why we do this final query
					var viewers = (from uid in studyInstanceUids
								   let vi = _openViewers.FirstOrDefault(Both(ViewerHasPrimary(uid), ViewerIsActive(activeViewers)))
								   where vi != null
								   select vi).ToList();

					// ensure the first instance is made active
					if (viewers.Any())
						service.ActivateViewer(viewers.First().Viewer);

					return viewers.Cast<IStudyViewer>().ToArray();
				}
			}
		}

		private void CloseViewer(ViewerInfo vi)
		{
			lock (_openViewersSyncRoot)
			{
				using (var service = new ViewerService())
				{
					service.CloseViewer(vi.Viewer);
					_openViewers.Remove(vi);
				}
			}
		}

		private void RemoveDeadViewerEntries()
		{
			lock (_openViewersSyncRoot)
			{
				using (var service = new ViewerService())
				{
					var isActiveTest = ViewerIsActive(service.GetActiveViewers());
					var deadEntries = _openViewers.Where(vi => !isActiveTest(vi)).ToList();
					foreach (var vi in deadEntries)
					{
						_openViewers.Remove(vi);
					}
				}
			}
		}

		private static bool ActivateViewer(ViewerInfo vi)
		{
			using (var service = new ViewerService())
			{
				return service.ActivateViewer(vi.Viewer);
			}
		}

		private static Func<ViewerInfo, bool> ViewerIsActive(IEnumerable<Viewer> activeViewers)
		{
			return vi => activeViewers.Any(v => v.Identifier == vi.Viewer.Identifier);
		}

		private static Func<ViewerInfo, bool> ViewerContainsAll(string[] studyInstanceUids)
		{
			return vi => studyInstanceUids.All(uid => vi.StudyInstanceUids.Contains(uid));
		}

		private static Func<ViewerInfo, bool> ViewerHasPrimary(string primaryStudyInstanceUid)
		{
			return vi => vi.Viewer.PrimaryStudyInstanceUid == primaryStudyInstanceUid;
		}

		private static Func<ViewerInfo, bool> Both(Func<ViewerInfo, bool> x, Func<ViewerInfo, bool> y)
		{
			return vi => x(vi) && y(vi);
		}
	}
}

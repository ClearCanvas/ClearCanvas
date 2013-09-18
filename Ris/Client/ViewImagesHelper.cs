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
using System.Security;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines an extension point for integrating image viewing capabilities.
	/// </summary>
	[ExtensionPoint]
	public class ViewerIntegrationExtensionPoint : ExtensionPoint<IViewerIntegration>
	{
	}



	/// <summary>
	/// Helper class for interacting with an integrated image viewer.
	/// </summary>
	/// <remarks>
	/// Methods on this class should always be invoked from the UI thread only.
	/// </remarks>
	public static class ViewImagesHelper
	{
		/// <summary>
		/// Options to the <see cref="ViewImagesHelper.ViewStudies"/> method.
		/// </summary>
		public class ViewStudiesOptions
		{
			/// <summary>
			/// Specifies that it should interact with the user.
			/// </summary>
			/// <remarks>
			/// If set to true, then the <see cref="DesktopWindow"/> property must be set.
			/// </remarks>
			public bool Interactive { get; set; }

			/// <summary>
			/// Specifies that if no studies are found for the specified procedure, look for studies associated
			/// with any procedure for the same order. Default is false.
			/// </summary>
			public bool FallbackToOrder { get; set; }

			/// <summary>
			/// Specifies the desktop window to use for user-interaction.
			/// </summary>
			public IDesktopWindow DesktopWindow { get; set; }
		}

		class StudyInstanceUidCache
		{
			private const int MaxSize = 25;
			private readonly Dictionary<string, List<string>> _map = new Dictionary<string, List<string>>();

			public void Put(EntityRef entityRef, IEnumerable<string> studyInstanceUids)
			{
				// super cheap size limiting strategy, because we don't really care enough to implement
				// LRU or time-based expiry or anything more sophisticated
				if (_map.Count == MaxSize)
				{
					_map.Clear();
				}

				_map[MakeKey(entityRef)] = new List<string>(studyInstanceUids);
			}

			public IList<string> Get(EntityRef entityRef)
			{
				List<string> entries;
				return _map.TryGetValue(MakeKey(entityRef), out entries) ?
					(IList<string>) entries.AsReadOnly() : new string[0];
			}

			public bool Contains(EntityRef entityRef)
			{
				return _map.ContainsKey(MakeKey(entityRef));
			}

			private static string MakeKey(EntityRef entityRef)
			{
				// use a cache key that is independent of entity-ref version
				return entityRef.ToString(false, true);
			}
		}


		private static readonly IViewerIntegration _viewer;
		private static readonly StudyInstanceUidCache _studyInstanceUidCache = new StudyInstanceUidCache();

		/// <summary>
		/// Class initializer.
		/// </summary>
		static ViewImagesHelper()
		{
			try
			{
				_viewer = (IViewerIntegration)(new ViewerIntegrationExtensionPoint()).CreateExtension();
			}
			catch (NotSupportedException)
			{
				Platform.Log(LogLevel.Debug, "No viewer integration extension found.");
			}
		}

		#region Public API

		/// <summary>
		/// Gets a value indicating if image viewing is supported.
		/// </summary>
		/// <remarks>
		/// Image viewing is supported if a viewer integration extension was successfully instantiated.
		/// </remarks>
		public static bool IsSupported
		{
			get { return _viewer != null; }
		}

		/// <summary>
		/// Gets a value indicating whether the current user has permission to view images.
		/// </summary>
		public static bool UserHasAccessToViewImages
		{
			get { return Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.Images.View); }
		}

		/// <summary>
		/// Opens all studies associated with the specified order.
		/// </summary>
		/// <param name="orderRef">Reference to the order for which to open images.</param>
		/// <param name="options"> </param>
		/// <returns>One or more instances of a study viewer.</returns>
		public static IStudyViewer[] ViewStudies(EntityRef orderRef, ViewStudiesOptions options)
		{
			CheckSupported();
			CheckAccess();

			var studyUids = GetStudiesForOrder(orderRef);
			var viewers = _viewer.ViewStudies(new ViewStudiesArgs { StudyInstanceUids = studyUids.ToArray(), InstancePerStudy = false });
			return CheckStudiesFound(viewers, options);
		}

		/// <summary>
		/// Opens studies associated with the specified order and procedures.
		/// </summary>
		/// <param name="orderRef">Reference to the order for which to open studies.</param>
		/// <param name="procedureRefs">Reference to the specific procedures for which to open studies - all procedures must belong to the parent order.</param>
		/// <param name="options"> </param>
		/// <returns>One or more instances of a study viewer.</returns>
		public static IStudyViewer[] ViewStudies(EntityRef orderRef, IEnumerable<EntityRef> procedureRefs, ViewStudiesOptions options)
		{
			CheckSupported();
			CheckAccess();

			var studyUids = GetStudiesForProcedures(orderRef, procedureRefs.ToList());
			var viewers = _viewer.ViewStudies(new ViewStudiesArgs {StudyInstanceUids = studyUids.ToArray(), InstancePerStudy = ViewImagesSettings.Default.OpenSeparateLinkedProcedureStudies});

			if (!viewers.Any() && options.FallbackToOrder)
			{
				// check if there might be other studies
				var allStudyUidsForOrder = GetStudiesForOrder(orderRef);
				if(allStudyUidsForOrder.Count > studyUids.Count)
				{
					// there are other studies associated with this order that could be opened
					// if we're in interactive mode, confirm with the user that we should do this
					if (options.Interactive && !Confirm(SR.MessageConfirmOpenStudiesForRelatedProcedures, options.DesktopWindow))
						return new IStudyViewer[0];

					viewers = _viewer.ViewStudies(new ViewStudiesArgs { StudyInstanceUids = allStudyUidsForOrder.ToArray(), InstancePerStudy = false });
					return CheckStudiesFound(viewers, options);
				}
			}

			return CheckStudiesFound(viewers, options);
		}

		#endregion

		private static IList<string> GetStudiesForOrder(EntityRef orderRef)
		{
			if (!_studyInstanceUidCache.Contains(orderRef))
				UpdateCache(orderRef);

			// now that we are certain to have everything we need in the cache, 
			// return the list of study uids
			return _studyInstanceUidCache.Get(orderRef);
		}

		private static IList<string> GetStudiesForProcedures(EntityRef orderRef, IList<EntityRef> procedureRefs)
		{
			if(procedureRefs.Any(pr => !_studyInstanceUidCache.Contains(pr)))
				UpdateCache(orderRef);

			// now that we are certain to have everything we need in the cache, 
			// return the list of study uids
			return procedureRefs.Select(pr => _studyInstanceUidCache.Get(pr).Single()).ToList();
		}

		private static void UpdateCache(EntityRef orderRef)
		{
			Platform.GetService<IBrowsePatientDataService>(service =>
			{
				var request = new GetOrderDetailRequest(orderRef, false, true, false, false, false, false);
				var response = service.GetData(new GetDataRequest { GetOrderDetailRequest = request }).GetOrderDetailResponse;
				// populate cache
				_studyInstanceUidCache.Put(orderRef, response.Order.Procedures.Select(p => p.StudyInstanceUid));
				foreach (var proc in response.Order.Procedures)
				{
					_studyInstanceUidCache.Put(proc.ProcedureRef, new[]{ proc.StudyInstanceUid });
				}
			});
		}

		private static bool Confirm(string message, IDesktopWindow desktopWindow)
		{
			var action = desktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo);
			return (action == DialogBoxAction.Yes);
		}

		private static IStudyViewer[] CheckStudiesFound(IStudyViewer[] viewers, ViewStudiesOptions options)
		{
			if (!viewers.Any() && options.Interactive)
				options.DesktopWindow.ShowMessageBox(SR.MessageNoStudiesFound, MessageBoxActions.Ok);
			return viewers;
		}

		private static void CheckSupported()
		{
			if (_viewer == null)
				throw new NotSupportedException("No viewer integration extension found.");
		}

		private static void CheckAccess()
		{
			if (!UserHasAccessToViewImages)
				throw new SecurityException("Access to images denied.");
		}

	}
}

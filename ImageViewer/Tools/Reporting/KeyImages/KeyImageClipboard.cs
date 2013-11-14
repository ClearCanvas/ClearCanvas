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
using System.Security.Policy;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Clipboard;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	partial class KeyImageClipboard
	{
		//TODO (Phoenix5): #10730 - remove this when it's fixed.
		[ThreadStatic]
		private static Dictionary<IImageViewer, KeyImageClipboard> _keyImageClipboards;

		[ThreadStatic]
		private static Dictionary<IDesktopWindow, IShelf> _clipboardShelves;

		private static IWorkItemActivityMonitor _activityMonitor;

		private static Dictionary<IImageViewer, KeyImageClipboard> KeyImageClipboards
		{
			get { return _keyImageClipboards ?? (_keyImageClipboards = new Dictionary<IImageViewer, KeyImageClipboard>()); }
		}

		private static Dictionary<IDesktopWindow, IShelf> ClipboardShelves
		{
			get { return _clipboardShelves ?? (_clipboardShelves = new Dictionary<IDesktopWindow, IShelf>()); }
		}

		#region Event Handling

		private static void OnWorkspaceActivated(object sender, ItemEventArgs<Workspace> e)
		{
			IShelf shelf = GetClipboardShelf(e.Item.DesktopWindow);
			if (shelf == null)
				return;

			KeyImageClipboardComponent clipboardComponent = shelf.Component as KeyImageClipboardComponent;
			if (clipboardComponent == null)
				return;

			clipboardComponent.Clipboard = GetKeyImageClipboard(e.Item);
		}

		private static void OnClipboardShelfClosed(object sender, ClosedEventArgs e)
		{
			IShelf clipboardShelf = (IShelf) sender;
			clipboardShelf.Closed -= OnClipboardShelfClosed;

			foreach (KeyValuePair<IDesktopWindow, IShelf> pair in ClipboardShelves)
			{
				if (pair.Value == clipboardShelf)
				{
					ClipboardShelves.Remove(pair.Key);
					break;
				}
			}
		}

		#endregion

		#region Private Methods

		private static IShelf GetClipboardShelf(IDesktopWindow desktopWindow)
		{
			if (ClipboardShelves.ContainsKey(desktopWindow))
				return ClipboardShelves[desktopWindow];
			else
				return null;
		}

		private static void ManageActivityMonitorConnection()
		{
			if (KeyImageClipboards.Count == 0 && _activityMonitor != null)
			{
				try
				{
					_activityMonitor.IsConnectedChanged -= DummyEventHandler;
					_activityMonitor.Dispose();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, e, "Failed to unsubscribe from activity monitor events.");
				}
				finally
				{
					_activityMonitor = null;
				}
			}
			else if (KeyImageClipboards.Count > 0 && _activityMonitor == null && WorkItemActivityMonitor.IsSupported)
			{
				try
				{
					_activityMonitor = WorkItemActivityMonitor.Create();
					//we subscribe to something to keep the connection open.
					_activityMonitor.IsConnectedChanged += DummyEventHandler;
				}
				catch (Exception e)
				{
					_activityMonitor = null;
					Platform.Log(LogLevel.Warn, e, "Failed to subscribe to activity monitor events.");
				}
			}
		}

		private static void DummyEventHandler(object sender, EventArgs e) {}

		#endregion

		#region Internal Methods

		internal static KeyImageClipboard GetKeyImageClipboard(Workspace workspace)
		{
			IImageViewer viewer = ImageViewerComponent.GetAsImageViewer(workspace);
			return GetKeyImageClipboard(viewer);
		}

		internal static KeyImageClipboard GetKeyImageClipboard(IImageViewer viewer)
		{
			if (!PermissionsHelper.IsInRole(AuthorityTokens.Study.KeyImages))
				throw new PolicyException(SR.ExceptionViewKeyImagePermissionDenied);

			if (viewer != null)
				return KeyImageClipboards[viewer];
			else
				return null;
		}

		internal static KeyImageClipboard GetKeyImageClipboard(IDesktopWindow desktopWindow)
		{
			IShelf shelf = GetClipboardShelf(desktopWindow);
			if (shelf == null)
				return null;

			if (!PermissionsHelper.IsInRole(AuthorityTokens.Study.KeyImages))
				throw new PolicyException(SR.ExceptionViewKeyImagePermissionDenied);

			KeyImageClipboardComponent component = shelf.Component as KeyImageClipboardComponent;
			if (component != null)
				return component.Clipboard;
			else
				return null;
		}

		#region Event Publishing

		internal static void OnDesktopWindowOpened(IDesktopWindow desktopWindow)
		{
			desktopWindow.Workspaces.ItemActivationChanged += OnWorkspaceActivated;
			ClipboardShelves[desktopWindow] = null;
		}

		internal static void OnDesktopWindowClosed(IDesktopWindow desktopWindow)
		{
			desktopWindow.Workspaces.ItemActivationChanged -= OnWorkspaceActivated;
			ClipboardShelves.Remove(desktopWindow);
		}

		internal static void OnViewerOpened(IImageViewer viewer)
		{
			if (!KeyImageClipboards.ContainsKey(viewer))
				KeyImageClipboards[viewer] = new KeyImageClipboard(viewer.StudyTree);

			ManageActivityMonitorConnection();
		}

		internal static void OnViewerClosed(IImageViewer viewer)
		{
			var info = GetKeyImageClipboard(viewer);
			KeyImageClipboards.Remove(viewer);

			if (info != null)
			{
				try
				{
					info.Publish();
				}
				catch (Exception e)
				{
					//Should never happen because KeyImagePublisher.Publish doesn't throw exceptions.
					ExceptionHandler.Report(e, Application.ActiveDesktopWindow);
				}
				finally
				{
					info.Dispose();
				}
			}

			ManageActivityMonitorConnection();
		}

		#endregion

		#endregion

		#region Public Methods

		public static void Add(IPresentationImage image)
		{
			Platform.CheckForNullReference(image, "image");
			Platform.CheckForNullReference(image.ImageViewer, "image.ImageViewer");

			if (!PermissionsHelper.IsInRole(AuthorityTokens.Study.KeyImages))
				throw new PolicyException(SR.ExceptionCreateKeyImagePermissionDenied);

			var info = GetKeyImageClipboard(image.ImageViewer);
			if (info == null)
				throw new ArgumentException("The specified image's viewer is not valid.", "image");

			IImageSopProvider sopProvider = image as IImageSopProvider;
			if (sopProvider == null)
				throw new ArgumentException("The image must be an IImageSopProvider.", "image");

			var item = (info.CurrentContext ?? new KeyImageInformation()).CreateKeyImageItem(image, hasChanges : true);
			item.FlagHasChanges();
			info.ClipboardItems.Add(item);
		}

		public static void Show(IDesktopWindow desktopWindow)
		{
			Show(desktopWindow, ShelfDisplayHint.DockLeft);
		}

		public static void Show(ShelfDisplayHint displayHint)
		{
			Show(null, displayHint);
		}

		public static void Show(IDesktopWindow desktopWindow, ShelfDisplayHint displayHint)
		{
			if (!KeyImageClipboardComponent.HasViewPlugin) return;

			if (!PermissionsHelper.IsInRole(AuthorityTokens.Study.KeyImages))
				throw new PolicyException(SR.ExceptionViewKeyImagePermissionDenied);

			desktopWindow = desktopWindow ?? Application.ActiveDesktopWindow;

			IShelf shelf = GetClipboardShelf(desktopWindow);
			if (shelf != null)
			{
				shelf.Activate();
			}
			else
			{
				Workspace activeWorkspace = desktopWindow.ActiveWorkspace;
				var info = GetKeyImageClipboard(activeWorkspace);
				ClipboardComponent component = new KeyImageClipboardComponent(info);
				shelf = ApplicationComponent.LaunchAsShelf(desktopWindow, component, SR.TitleKeyImages, displayHint);
				shelf.Closed += OnClipboardShelfClosed;

				ClipboardShelves[desktopWindow] = shelf;
			}
		}

		public static void Show()
		{
			Show(null);
		}

		#endregion
	}
}
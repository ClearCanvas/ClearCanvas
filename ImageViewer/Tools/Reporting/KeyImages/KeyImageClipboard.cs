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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Clipboard;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Security.Policy;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	public class KeyImageClipboard
	{
		public const string MenuSite = "keyimageclipboard-contextmenu";
		public const string ToolbarSite = "keyimageclipboard-toolbar";

        //TODO (Phoenix5): #10730 - remove this when it's fixed.
        [ThreadStatic] private static Dictionary<IImageViewer, KeyImageInformation> _keyImageInformation;
        [ThreadStatic] private static Dictionary<IDesktopWindow, IShelf> _clipboardShelves;

		private static IWorkItemActivityMonitor _activityMonitor;

		static KeyImageClipboard()
		{
		    try
		    {
                HasViewPlugin = ViewFactory.CreateAssociatedView(typeof(KeyImageClipboardComponent)) != null;
		    }
		    catch (Exception)
		    {
		        HasViewPlugin = false;
		    }
		}

	    private static Dictionary<IImageViewer, KeyImageInformation> KeyImageInformation
	    {
            get
            {
                return _keyImageInformation ?? (_keyImageInformation = new Dictionary<IImageViewer, KeyImageInformation>());
            }
	    }

        private static Dictionary<IDesktopWindow, IShelf> ClipboardShelves
        {
            get
            {
                return _clipboardShelves ?? (_clipboardShelves = new Dictionary<IDesktopWindow, IShelf>());
            }
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

			clipboardComponent.KeyImageInformation = GetKeyImageInformation(e.Item) ?? new KeyImageInformation();
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
            if (KeyImageInformation.Count == 0 && _activityMonitor != null)
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
            else if (KeyImageInformation.Count > 0 && _activityMonitor == null && WorkItemActivityMonitor.IsSupported)
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

		private static void DummyEventHandler(object sender, EventArgs e)
		{
		}

		#endregion

		#region Internal Methods


		internal static KeyImageInformation GetKeyImageInformation(Workspace workspace)
		{
			IImageViewer viewer = ImageViewerComponent.GetAsImageViewer(workspace);
			return GetKeyImageInformation(viewer);
		}

		internal static KeyImageInformation GetKeyImageInformation(IImageViewer viewer)
		{
            // TODO (CR Phoenix5 - Med): Clinical as well
			if (!PermissionsHelper.IsInRole(AuthorityTokens.Study.KeyImages))
				throw new PolicyException(SR.ExceptionViewKeyImagePermissionDenied);

			if (viewer != null)
                return KeyImageInformation[viewer];
			else
				return null;
		}

		internal static KeyImageInformation GetKeyImageInformation(IDesktopWindow desktopWindow)
		{
			IShelf shelf = GetClipboardShelf(desktopWindow);
			if (shelf == null)
				return null;

            // TODO (CR Phoenix5 - Med): Clinical as well
			if (!PermissionsHelper.IsInRole(AuthorityTokens.Study.KeyImages))
				throw new PolicyException(SR.ExceptionViewKeyImagePermissionDenied);

			KeyImageClipboardComponent component = shelf.Component as KeyImageClipboardComponent;
			if (component != null)
				return component.KeyImageInformation;
			else
				return null;
		}

	    internal static bool HasViewPlugin { get; private set; }

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
            if (!KeyImageInformation.ContainsKey(viewer))
                KeyImageInformation[viewer] = new KeyImageInformation();

			ManageActivityMonitorConnection();
		}

		internal static void OnViewerClosed(IImageViewer viewer)
		{
			KeyImageInformation info = GetKeyImageInformation(viewer);
            KeyImageInformation.Remove(viewer);

			if (info != null)
			{				
				using(info)
				{
					try
					{
						var publisher = new KeyImagePublisher(info);
						publisher.Publish();
					}
					catch (Exception e)
					{
                        //Should never happen because KeyImagePublisher.Publish doesn't throw exceptions.
                        ExceptionHandler.Report(e, Application.ActiveDesktopWindow);
					}
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

            // TODO (CR Phoenix5 - Med): Clinical as well
			if (!PermissionsHelper.IsInRole(AuthorityTokens.Study.KeyImages))
				throw new PolicyException(SR.ExceptionCreateKeyImagePermissionDenied);

			KeyImageInformation info = GetKeyImageInformation(image.ImageViewer);
			if (info == null)
				throw new ArgumentException("The specified image's viewer is not valid.", "image");

			IImageSopProvider sopProvider = image as IImageSopProvider;
			if (sopProvider == null)
				throw new ArgumentException("The image must be an IImageSopProvider.", "image");

			info.ClipboardItems.Add(ClipboardComponent.CreatePresentationImageItem(image));
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
            // TODO (CR Phoenix5 - Med): Clinical as well
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
				KeyImageInformation info = GetKeyImageInformation(activeWorkspace) ?? new KeyImageInformation();
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
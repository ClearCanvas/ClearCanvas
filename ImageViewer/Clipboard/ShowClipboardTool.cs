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
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.BaseTools;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard
{
	[ExtensionPoint]
	public sealed class ClipboardToolbarToolExtensionPoint : ExtensionPoint<ITool> {}

	[MenuAction("show", "global-menus/MenuView/MenuShowClipboard", "Show")]
	[DropDownButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarShowClipboard", "Show", "ClipboardMenuModel")]
	[Tooltip("show", "TooltipShowClipboard")]
	[IconSet("show", "Icons.ShowClipboardToolSmall.png", "Icons.ShowClipboardToolMedium.png", "Icons.ShowClipboardToolLarge.png")]
	//
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class ShowClipboardTool : ImageViewerTool
	{
		private class ToolContextProxy : IImageViewerToolContext
		{
			private readonly IImageViewerToolContext _realContext;

			public ToolContextProxy(IImageViewerToolContext realContext)
			{
				_realContext = realContext;
			}

			#region IImageViewerToolContext Members

			public IImageViewer Viewer
			{
				get { return _realContext.Viewer; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _realContext.DesktopWindow; }
			}

			public event CancelEventHandler ViewerClosing
			{
				add { _realContext.ViewerClosing += value; }
				remove { _realContext.ViewerClosing -= value; }
			}

			#endregion
		}

		public const string ClipboardToolbarDropdownSite = "clipboard-toolbar-dropdown";

		[ThreadStatic]
		private static IShelf _shelf;

		private ToolSet _toolSet;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <remarks>
		/// A no-args constructor is required by the framework.  Do not remove.
		/// </remarks>
		public ShowClipboardTool() {}

		public override void Initialize()
		{
			base.Initialize();

			object[] tools;

			try
			{
				tools = new ClipboardToolbarToolExtensionPoint().CreateExtensions();
			}
			catch (NotSupportedException)
			{
				tools = new object[0];
				Platform.Log(LogLevel.Debug, "No clipboard toolbar drop-down items found.");
			}
			catch (Exception e)
			{
				tools = new object[0];
				Platform.Log(LogLevel.Debug, "Failed to create clipboard toolbar drop-down items.", e);
			}

			_toolSet = new ToolSet(tools, new ToolContextProxy(Context));
		}

		protected override void Dispose(bool disposing)
		{
			_toolSet.Dispose();
			base.Dispose(disposing);
		}

		public ActionModelNode ClipboardMenuModel
		{
			get { return ActionModelRoot.CreateModel(typeof (ShowClipboardTool).FullName, ClipboardToolbarDropdownSite, _toolSet.Actions); }
		}

		public void Show()
		{
			if (_shelf == null)
			{
				ClipboardComponent clipboardComponent = new ClipboardComponent(Clipboard.ClipboardSiteToolbar, Clipboard.ClipboardSiteMenu, Clipboard.Default, false);

				_shelf = ApplicationComponent.LaunchAsShelf(
					this.Context.DesktopWindow,
					clipboardComponent,
					SR.TitleClipboard,
					"Clipboard",
					ShelfDisplayHint.DockLeft | ShelfDisplayHint.DockAutoHide);

				_shelf.Closed += OnShelfClosed;
			}
			else
			{
				_shelf.Show();
			}
		}

		private static void OnShelfClosed(object sender, ClosedEventArgs e)
		{
			_shelf.Closed -= OnShelfClosed;
			_shelf = null;
		}
	}
}
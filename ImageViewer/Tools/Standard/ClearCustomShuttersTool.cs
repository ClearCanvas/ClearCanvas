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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.PresentationStates;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("clear", "imageviewer-contextmenu/MenuClearCustomShutters", "Clear")]
	[IconSet("clear", "Icons.ClearCustomShuttersToolSmall.png", "Icons.ClearCustomShuttersToolMedium.png", "Icons.ClearCustomShuttersToolLarge.png")]
	[VisibleStateObserver("clear", "Visible", "VisibleChanged")]
	//
	[ButtonAction("clearToolbar", "global-toolbars/ToolbarStandard/ToolbarClearCustomShutters", "Clear")]
	[Tooltip("clearToolbar", "TooltipClearCustomShutters")]
	[IconSet("clearToolbar", "Icons.ClearCustomShuttersToolSmall.png", "Icons.ClearCustomShuttersToolMedium.png", "Icons.ClearCustomShuttersToolLarge.png")]
	[EnabledStateObserver("clearToolbar", "Visible", "VisibleChanged")]
	//
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class ClearCustomShuttersTool : ImageViewerTool
	{
		private bool _visible;

		public ClearCustomShuttersTool() {}

		public bool Visible
		{
			get { return _visible; }
			set
			{
				if (_visible == value)
					return;

				_visible = value;
				EventsHelper.Fire(VisibleChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler VisibleChanged;

		public override void Initialize()
		{
			base.Initialize();
			base.Context.Viewer.EventBroker.ImageDrawing += OnImageDrawing;
		}

		protected override void Dispose(bool disposing)
		{
			base.Context.Viewer.EventBroker.ImageDrawing -= OnImageDrawing;
			base.Dispose(disposing);
		}

		public void Clear()
		{
			if (base.SelectedPresentationImage == null)
				return;

			if (base.SelectedPresentationImage is IDicomPresentationImage)
			{
				IDicomPresentationImage dicomImage = (IDicomPresentationImage) base.SelectedPresentationImage;
				GeometricShuttersGraphic shuttersGraphic = DrawShutterTool.GetGeometricShuttersGraphic(dicomImage);
				if (shuttersGraphic == null) return;

				DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(shuttersGraphic);
				foreach (GeometricShutter shutter in shuttersGraphic.CustomShutters)
					historyCommand.Enqueue(new RemoveGeometricShutterUndoableCommand(shuttersGraphic, shutter));

				historyCommand.Execute();

				historyCommand.Name = SR.CommandClearCustomShutters;
				base.Context.Viewer.CommandHistory.AddCommand(historyCommand);
				Visible = false;
			}
		}

		protected override void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			base.OnTileSelected(sender, e);
			UpdateVisible();
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			base.OnPresentationImageSelected(sender, e);
			UpdateVisible();
		}

		private void OnImageDrawing(object sender, ImageDrawingEventArgs e)
		{
			if (e.PresentationImage == base.SelectedPresentationImage)
				UpdateVisible();
		}

		private void UpdateVisible()
		{
			Visible = HasCustomShutters(base.SelectedPresentationImage);
		}

		private static bool HasCustomShutters(IPresentationImage image)
		{
			if (image != null && image is IDicomPresentationImage)
			{
				IDicomPresentationImage dicomImage = (IDicomPresentationImage) image;
				GeometricShuttersGraphic shuttersGraphic = DrawShutterTool.GetGeometricShuttersGraphic(dicomImage);
				if (shuttersGraphic != null)
					return shuttersGraphic.CustomShutters.Count > 0;
			}

			return false;
		}
	}
}
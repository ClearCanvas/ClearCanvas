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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Tools.Standard.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuInvert", "Apply", InitiallyAvailable = false)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuInvert", "Apply")]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarInvert", "Apply", KeyStroke = XKeys.I)]
	[Tooltip("activate", "TooltipInvert")]
	[IconSet("activate", "Icons.InvertToolSmall.png", "Icons.InvertToolMedium.png", "Icons.InvertToolLarge.png")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [GroupHint("activate", "Tools.Image.Manipulation.Invert")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class InvertTool : ImageViewerTool
	{
		private readonly VoiLutImageOperation _operation;
		private ToolModalityBehaviorHelper _toolBehavior;

		public InvertTool()
		{
			_operation = new VoiLutImageOperation(Invert);
		}

		public override void Initialize()
		{
			base.Initialize();

			_toolBehavior = new ToolModalityBehaviorHelper(ImageViewer);
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			base.Enabled = _operation.GetOriginator(e.SelectedPresentationImage) != null;
		}

		public void Apply()
		{
			if (_operation.GetOriginator(this.Context.Viewer.SelectedPresentationImage) == null)
				return;

			ImageOperationApplicator applicator = new ImageOperationApplicator(SelectedPresentationImage, _operation);
			UndoableCommand historyCommand = _toolBehavior.Behavior.SelectedImageInvertTool ? applicator.ApplyToReferenceImage() : applicator.ApplyToAllImages();
			if (historyCommand != null)
			{
				historyCommand.Name = SR.CommandInvert;
				Context.Viewer.CommandHistory.AddCommand(historyCommand);
			}
		}

		private void Invert(IPresentationImage image)
		{
			IVoiLutManager manager = (IVoiLutManager)_operation.GetOriginator(image);
			manager.Invert = !manager.Invert;
		}
	}
}

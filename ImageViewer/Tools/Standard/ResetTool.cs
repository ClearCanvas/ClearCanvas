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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Graphics3D;
using ClearCanvas.ImageViewer.Tools.Standard.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuReset", "Activate", InitiallyAvailable = false)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuReset", "Activate")]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarReset", "Activate")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipReset")]
	[IconSet("activate", "Icons.ResetToolSmall.png", "Icons.ResetToolMedium.png", "Icons.ResetToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Orientation.Reset")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class ResetTool : ImageViewerTool
	{
		private readonly ResetImageOperation _operation;
		private ToolModalityBehaviorHelper _toolBehavior;

		public ResetTool()
		{
			_operation = new ResetImageOperation(Apply);
		}

		public override void Initialize()
		{
			base.Initialize();

			_toolBehavior = new ToolModalityBehaviorHelper(ImageViewer);
		}

		public void Activate()
		{
			if (!_operation.AppliesTo(SelectedPresentationImage))
				return;

			var applicator = new ImageOperationApplicator(SelectedPresentationImage, _operation);
			var historyCommand = _toolBehavior.Behavior.SelectedImageResetTool ? applicator.ApplyToReferenceImage() : applicator.ApplyToAllImages();
			if (historyCommand != null)
			{
				historyCommand.Name = SR.CommandReset;
				Context.Viewer.CommandHistory.AddCommand(historyCommand);
			}
		}

		public void Apply(IPresentationImage image)
		{
			var transform = ResetImageOperation.GetSpatialTransform(image);
			if (transform != null)
				transform.Reset();

			var transform3D = ResetImageOperation.GetSpatialTransform3D(image);
			if (transform3D != null)
				transform3D.Reset();
		}

		private class ResetImageOperation : BasicImageOperation
		{
			public ResetImageOperation(ApplyDelegate applyDelegate)
				: base(i => new MemorableAdapter(i), applyDelegate) {}

			public static IImageSpatialTransform GetSpatialTransform(IPresentationImage image)
			{
				return image is ISpatialTransformProvider ? ((ISpatialTransformProvider) image).SpatialTransform as IImageSpatialTransform : null;
			}

			public static ISpatialTransform3D GetSpatialTransform3D(IPresentationImage image)
			{
				return image is ISpatialTransform3DProvider ? ((ISpatialTransform3DProvider) image).SpatialTransform3D : null;
			}

			private class MemorableAdapter : IMemorable
			{
				private readonly IPresentationImage _image;

				internal MemorableAdapter(IPresentationImage image)
				{
					_image = image;
				}

				public object CreateMemento()
				{
					var spatialTransform = GetSpatialTransform(_image);
					var spatialTransformMemento = spatialTransform != null ? spatialTransform.CreateMemento() : null;

					var spatialTransform3D = GetSpatialTransform3D(_image);
					var spatialTransform3DMemento = spatialTransform3D != null ? spatialTransform3D.CreateMemento() : null;

					return new[] {spatialTransformMemento, spatialTransform3DMemento};
				}

				public void SetMemento(object memento)
				{
					var array = memento as object[];
					if (array == null) return;

					var spatialTransform = GetSpatialTransform(_image);
					if (spatialTransform != null && array.Length > 0) spatialTransform.SetMemento(array[0]);

					var spatialTransform3D = GetSpatialTransform3D(_image);
					if (spatialTransform3D != null && array.Length > 1) spatialTransform3D.SetMemento(array[1]);
				}
			}
		}
	}
}
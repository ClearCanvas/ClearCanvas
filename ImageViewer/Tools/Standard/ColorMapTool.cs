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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Tools.Standard.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ColorMapTool : ImageViewerTool
	{
		private class ColorMapActionContainer : UndoableOperation<IPresentationImage>
		{
			private readonly ColorMapTool _ownerTool;
			private readonly MenuAction _action;
			private readonly ColorMapDescriptor _descriptor;

			public ColorMapActionContainer(ColorMapTool ownerTool, ColorMapDescriptor descriptor, int index)
			{
				_ownerTool = ownerTool;
				_descriptor = descriptor;

				string actionId = String.Format("apply{0}", index);
				ActionPath actionPath = new ActionPath(String.Format("imageviewer-contextmenu/ColourMaps/colourMap{0}", index), _ownerTool._resolver);
				_action = new MenuAction(actionId, actionPath, ClickActionFlags.None, _ownerTool._resolver);
				_action.GroupHint = new GroupHint("Tools.Image.Manipulation.Lut.ColourMaps");
				_action.Label = _descriptor.Description;
				_action.SetClickHandler(this.Apply);
			}
			
			public ClickAction Action
			{
				get { return _action; }
			}

			private void Apply()
			{
				ImageOperationApplicator applicator = new ImageOperationApplicator(_ownerTool.SelectedPresentationImage, this);
				UndoableCommand historyCommand = _ownerTool._toolBehavior.Behavior.SelectedImageColorMapTool ? applicator.ApplyToReferenceImage() : applicator.ApplyToAllImages();
				if (historyCommand != null)
				{
					historyCommand.Name = SR.CommandColorMap;
					_ownerTool.Context.Viewer.CommandHistory.AddCommand(historyCommand);
				}
			}

			public override IMemorable GetOriginator(IPresentationImage image)
			{
				if (image is IColorMapProvider)
					return ((IColorMapProvider) image).ColorMapManager;

				return null;
			}

			public override void Apply(IPresentationImage image)
			{
				((IColorMapManager)GetOriginator(image)).InstallColorMap(_descriptor);
			}
		}

		private readonly ActionResourceResolver _resolver;
		private ToolModalityBehaviorHelper _toolBehavior;

		public ColorMapTool()
		{
			_resolver = new ActionResourceResolver(this);
		}

		public override void Initialize()
		{
			base.Initialize();

			_toolBehavior = new ToolModalityBehaviorHelper(ImageViewer);
		}

		public override IActionSet Actions
		{
			get
			{
				return new ActionSet(GetActions());
			}
		}

		private IEnumerable<IAction> GetActions()
		{
			if (this.SelectedPresentationImage is IColorMapProvider)
			{
				int i = 0;
				foreach (ColorMapDescriptor descriptor in ((IColorMapProvider)this.SelectedPresentationImage).ColorMapManager.AvailableColorMaps)
				{
					ColorMapActionContainer container = new ColorMapActionContainer(this, descriptor, ++i);
					yield return container.Action;
				}
			}
			else
			{
				yield break;
			}
		}
	}
}

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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	// NOTE: Because the synchronization tools act based on events coming from the viewer event broker,
	// they cannot be responsible for drawing the images because that would result in repeated (unnecessary)
	// Draw() calls.  The reference line tool is dependent on the stacking synchronization tool in that 
	// it needs to synchronize the images first, before the reference lines are drawn, otherwise images
	// that are about to become invisible will be unnecessarily drawn.
	//
	// This class acts as a mediator, listening to events from the viewer event broker and coordinating
	// the synchronization tools (the order they are called in, as well as drawing the affected images
	// such that none are redrawn unnecessarily).
	//
	// There may be a more general solution for coordinating draws, but this problem may be 
	// more or less isolated to synchronization tools, so we will just use this class for
	// now rather than spend the time to develop a general solution unnecessarily.

	internal class SynchronizationToolCoordinator
	{
		#region SynchronizationToolCompositeGraphic class

		// The mediator provides each tool with its own CompositeGraphic to work with, while
		// maintaining control over the order in which these graphics appear.
		private class SynchronizationToolCompositeGraphic : CompositeGraphic
		{
			public SynchronizationToolCompositeGraphic()
			{
				// reference lines on the bottom
				base.Graphics.Add(new ReferenceLineCompositeGraphic());
				// reference points on top
				base.Graphics.Add(new CompositeGraphic());
			}

			public ReferenceLineCompositeGraphic ReferenceLineCompositeGraphic
			{
				get { return (ReferenceLineCompositeGraphic)base.Graphics[0]; }
			}

			public CompositeGraphic SpatialLocatorCompositeGraphic
			{
				get { return (CompositeGraphic)base.Graphics[1]; }
			}
		}

		#endregion

		#region Private Fields

		[ThreadStatic]
		private static Dictionary<IImageViewer, SynchronizationToolCoordinator> _coordinators;

		private readonly IImageViewer _viewer;
		
		private StackingSynchronizationTool _stackingSynchronizationTool;
		private ReferenceLineTool _referenceLineTool;
		private SpatialLocatorTool _spatialLocatorTool;

		private int _referenceCount = 0;

		#endregion

		private SynchronizationToolCoordinator(IImageViewer viewer)
		{
			_viewer = viewer;
		}

		private static Dictionary<IImageViewer, SynchronizationToolCoordinator> Coordinators
		{
			get
			{
				if (_coordinators == null)
					_coordinators = new Dictionary<IImageViewer, SynchronizationToolCoordinator>();
				return _coordinators;
			}
		}

		public void SetStackingSynchronizationTool(StackingSynchronizationTool tool)
		{
			_stackingSynchronizationTool = tool;
		}

		public void SetReferenceLineTool(ReferenceLineTool tool)
		{
			_referenceLineTool = tool;
		}

		public void SetSpatialLocatorTool(SpatialLocatorTool tool)
		{
			_spatialLocatorTool = tool;
		}

		private static void Draw<T>(IEnumerable<T> itemsToDraw)
			where T : IDrawable
		{
			foreach (IDrawable drawable in itemsToDraw)
				drawable.Draw();
		}

		private void DrawImageBoxes(IEnumerable<IImageBox> imageBoxes)
		{
			List<IImageBox> imageBoxesToDraw = new List<IImageBox>(imageBoxes);

			//Then calculate the reference lines.
			_referenceLineTool.RefreshAllReferenceLines();

			foreach (IPresentationImage image in _referenceLineTool.GetImagesToRedraw())
			{
				//Only draw images that won't be drawn as a result of the image boxes being drawn.
				if (!imageBoxesToDraw.Contains(image.ParentDisplaySet.ImageBox))
					image.Draw();
			}

			Draw(imageBoxesToDraw);
		}

		private void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			if (e.SelectedTile.PresentationImage != null)
			{
				//the presentation image selected event will fire and take care of this.
				return;
			}

			_referenceLineTool.RefreshAllReferenceLines();
			Draw(_referenceLineTool.GetImagesToRedraw());
		}

		private void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			_stackingSynchronizationTool.SynchronizeImageBoxes();
			OnSynchronizedImageBoxes();
		}

		private static SynchronizationToolCompositeGraphic GetSynchronizationToolCompositeGraphic(IPresentationImage image)
		{
			if (image is IApplicationGraphicsProvider)
			{
				GraphicCollection overlayGraphics = ((IApplicationGraphicsProvider)image).ApplicationGraphics;
				SynchronizationToolCompositeGraphic container = CollectionUtils.SelectFirst(overlayGraphics,
																	delegate(IGraphic graphic)
																	{
																		return graphic is SynchronizationToolCompositeGraphic;
																	}) as SynchronizationToolCompositeGraphic;

				if (container == null)
					overlayGraphics.Insert(0, container = new SynchronizationToolCompositeGraphic());

				container.Visible = true;
				return container;
			}

			return null;
		}


		public static SynchronizationToolCoordinator Get(IImageViewer viewer)
		{
			if (!Coordinators.ContainsKey(viewer))
			{
				SynchronizationToolCoordinator coordinator = new SynchronizationToolCoordinator(viewer);

				viewer.EventBroker.PresentationImageSelected += coordinator.OnPresentationImageSelected;
				viewer.EventBroker.TileSelected += coordinator.OnTileSelected;
			
				Coordinators.Add(viewer, coordinator);
			}

			DicomImagePlane.InitializeCache();

			++Coordinators[viewer]._referenceCount;
			return Coordinators[viewer];
		}

		public void Release()
		{
			DicomImagePlane.ReleaseCache();

			--_referenceCount;
			if (_referenceCount <= 0)
			{
				_viewer.EventBroker.PresentationImageSelected -= OnPresentationImageSelected;
				_viewer.EventBroker.TileSelected -= OnTileSelected;

				Coordinators.Remove(_viewer);
			}
		}

		public ReferenceLineCompositeGraphic GetReferenceLineCompositeGraphic(IPresentationImage image)
		{
			SynchronizationToolCompositeGraphic container = GetSynchronizationToolCompositeGraphic(image);
			if (container == null)
				return null;

			return container.ReferenceLineCompositeGraphic;
		}

		public CompositeGraphic GetSpatialLocatorCompositeGraphic(IPresentationImage image)
		{
			SynchronizationToolCompositeGraphic container = GetSynchronizationToolCompositeGraphic(image);
			if (container == null)
				return null;

			return container.SpatialLocatorCompositeGraphic;
		}

		public void OnSynchronizedImageBoxes()
		{
			DrawImageBoxes(_stackingSynchronizationTool.GetImageBoxesToDraw());
		}

		public void OnSpatialLocatorCrosshairsUpdated()
		{
			//The spatial locator and stacking sync tool conflict.
			_stackingSynchronizationTool.SynchronizeActive = false;

			DrawImageBoxes(_spatialLocatorTool.GetImageBoxesToRedraw());
		}

		public void OnSpatialLocatorStopped()
		{
			Draw(_spatialLocatorTool.GetImageBoxesToRedraw());
		}
		
		public void OnRefreshedReferenceLines()
		{
			Draw(_referenceLineTool.GetImagesToRedraw());
		}
	}
}

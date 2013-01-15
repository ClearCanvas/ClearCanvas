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

using System.Drawing;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public abstract class MeasurementTool : MouseImageViewerTool
	{
		private int _serialNumber;
		private InteractiveGraphicBuilder _graphicBuilder;
		private DrawableUndoableCommand _undoableCommand;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="tooltipPrefix">The tooltip prefix, which usually describes the tool's function.</param>
		protected MeasurementTool(string tooltipPrefix)
			: base(tooltipPrefix)
		{
			this.Behaviour |= MouseButtonHandlerBehaviour.SuppressContextMenu | MouseButtonHandlerBehaviour.SuppressOnTileActivate;
		}

		protected virtual string RoiNameFormat
		{
			get { return string.Empty; }
		}

		protected abstract string CreationCommandName { get; }

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			if (_graphicBuilder != null)
				return _graphicBuilder.Start(mouseInformation);

            if (!CanStart(mouseInformation.Tile.PresentationImage))
                return false;

			RoiGraphic roiGraphic = CreateRoiGraphic();

			_graphicBuilder = CreateGraphicBuilder(roiGraphic.Subject);
			_graphicBuilder.GraphicComplete += OnGraphicBuilderComplete;
			_graphicBuilder.GraphicCancelled += OnGraphicBuilderCancelled;

		    AddRoiGraphic(  mouseInformation.Tile.PresentationImage, 
                            roiGraphic, 
                            (IOverlayGraphicsProvider)mouseInformation.Tile.PresentationImage);

			roiGraphic.Suspend();
			try
			{
				if (_graphicBuilder.Start(mouseInformation))
					return true;
			}
			finally
			{
				roiGraphic.Resume(true);
			}

			this.Cancel();
			return false;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_graphicBuilder != null)
				return _graphicBuilder.Track(mouseInformation);

			return false;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (_graphicBuilder == null)
				return false;

			if (_graphicBuilder.Stop(mouseInformation))
				return true;

			_graphicBuilder = null;
			_undoableCommand = null;
			return false;
		}

		public override void Cancel()
		{
			if (_graphicBuilder == null)
				return;

			_graphicBuilder.Cancel();
		}

		public override CursorToken GetCursorToken(Point point)
		{
			if (_graphicBuilder != null)
				return _graphicBuilder.GetCursorToken(point);

			return base.GetCursorToken(point);
		}

        protected virtual bool CanStart(IPresentationImage image)
        {
            return image != null && image is IOverlayGraphicsProvider;
        }

	    protected RoiGraphic CreateRoiGraphic()
	    {
	        return CreateRoiGraphic(true);
	    }

        protected RoiGraphic CreateRoiGraphic(bool initiallySelected)
		{
			//When you create a graphic from within a tool (particularly one that needs capture, like a multi-click graphic),
			//see it through to the end of creation.  It's just cleaner, not to mention that if this tool knows how to create it,
			//it should also know how to (and be responsible for) cancelling it and/or deleting it appropriately.
			IGraphic graphic = CreateGraphic();
			IAnnotationCalloutLocationStrategy strategy = CreateCalloutLocationStrategy();

			RoiGraphic roiGraphic;
			if (strategy == null)
				roiGraphic = new RoiGraphic(graphic);
			else
				roiGraphic = new RoiGraphic(graphic, strategy);

			if (Settings.Default.AutoNameMeasurements && !string.IsNullOrEmpty(this.RoiNameFormat))
				roiGraphic.Name = string.Format(this.RoiNameFormat, ++_serialNumber);
			else
				roiGraphic.Name = string.Empty;

			roiGraphic.State = initiallySelected ? roiGraphic.CreateSelectedState() : roiGraphic.CreateInactiveState();

			return roiGraphic;
		}

	    protected void AddRoiGraphic(IPresentationImage image, RoiGraphic roiGraphic, IOverlayGraphicsProvider provider)
        {
            _undoableCommand = new DrawableUndoableCommand(image);
            _undoableCommand.Enqueue(new AddGraphicUndoableCommand(roiGraphic, provider.OverlayGraphics));
            _undoableCommand.Name = CreationCommandName;
            _undoableCommand.Execute();

            OnRoiCreation(roiGraphic);
        }

	    protected abstract InteractiveGraphicBuilder CreateGraphicBuilder(IGraphic subjectGraphic);

		protected abstract IGraphic CreateGraphic();

		protected virtual IAnnotationCalloutLocationStrategy CreateCalloutLocationStrategy()
		{
			return null;
		}

		protected virtual void OnRoiCreation(RoiGraphic roiGraphic) {}

		private void OnGraphicBuilderComplete(object sender, GraphicEventArgs e)
		{
			_graphicBuilder.GraphicComplete -= OnGraphicBuilderComplete;
			_graphicBuilder.GraphicCancelled -= OnGraphicBuilderCancelled;

			_graphicBuilder.Graphic.ImageViewer.CommandHistory.AddCommand(_undoableCommand);
			_graphicBuilder.Graphic.Draw();

			_undoableCommand = null;

			_graphicBuilder = null;
		}

		private void OnGraphicBuilderCancelled(object sender, GraphicEventArgs e)
		{
			_graphicBuilder.GraphicComplete -= OnGraphicBuilderComplete;
			_graphicBuilder.GraphicCancelled -= OnGraphicBuilderCancelled;

			_undoableCommand.Unexecute();
			_undoableCommand = null;

			_graphicBuilder = null;
		}
	}
}
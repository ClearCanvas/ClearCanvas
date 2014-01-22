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
using System.Drawing;
using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	partial class DicomGraphicAnnotation
	{
		[Cloneable(true)]
		[DicomSerializableGraphicAnnotation(typeof (DicomGraphicAnnotationSerializer))]
		private class SubjectGraphic : CompositeGraphic, ICursorTokenProvider, IMouseButtonHandler, IExportedActionsProvider
		{
			[CloneIgnore]
			private IMouseButtonHandler _capturedHandler = null;

			public void SetColor(Color value)
			{
				TraverseChildGraphics(Graphics, graphic =>
				                                	{
				                                		if (graphic is IVectorGraphic)
				                                			((IVectorGraphic) graphic).Color = value;
				                                		else if (graphic is IControlGraphic)
				                                			((IControlGraphic) graphic).Color = value;
				                                	});
			}

			public void SetEnabled(bool value)
			{
				TraverseChildGraphics(Graphics, graphic =>
				                                	{
				                                		var controlGraphic = graphic as IControlGraphic;
				                                		if (controlGraphic != null) (controlGraphic).Enabled = value;
				                                	});
			}

			public MouseButtonHandlerBehaviour Behaviour
			{
				get { return _capturedHandler != null ? _capturedHandler.Behaviour : MouseButtonHandlerBehaviour.None; }
			}

			public CursorToken GetCursorToken(Point point)
			{
				// TODO (CR Oct 2011): This pattern could be reused outside the context
				// of a "Control" graphic. Perhaps we could move it out to a "GraphicsController"
				// class that enumerates through a list of graphics looking for a handler.
				CursorToken cursor = null;

				if (_capturedHandler != null)
				{
					var provider = _capturedHandler as ICursorTokenProvider;
					if (provider != null)
						cursor = (provider).GetCursorToken(point);
				}

				if (cursor == null)
				{
					foreach (IGraphic graphic in EnumerateChildGraphics(true))
					{
						if (!graphic.Visible)
							continue;

						ICursorTokenProvider provider = graphic as ICursorTokenProvider;
						if (provider != null)
						{
							cursor = provider.GetCursorToken(point);
							if (cursor != null)
								break;
						}
					}
				}

				return cursor;
			}

			public IActionSet GetExportedActions(string site, IMouseInformation mouseInformation)
			{
				return EnumerateChildGraphics(true).OfType<IExportedActionsProvider>()
					.Select(p => p.GetExportedActions(site, mouseInformation))
					.Where(a => a != null)
					.Aggregate((IActionSet) new ActionSet(), (a, b) => a.Union(b));
			}

			public bool Start(IMouseInformation mouseInformation)
			{
				bool result = false;

				if (_capturedHandler != null)
				{
					result = _capturedHandler.Start(mouseInformation);
					if (result) return true;
				}

				_capturedHandler = null;
				foreach (IGraphic graphic in EnumerateChildGraphics(true))
				{
					if (!graphic.Visible)
						continue;

					IMouseButtonHandler handler = graphic as IMouseButtonHandler;
					if (handler != null)
					{
						result = handler.Start(mouseInformation);
						if (result)
						{
							_capturedHandler = handler;
							break;
						}
					}
				}

				return result;
			}

			public bool Track(IMouseInformation mouseInformation)
			{
				bool result = false;

				if (_capturedHandler != null)
					return _capturedHandler.Track(mouseInformation);

				foreach (IGraphic graphic in EnumerateChildGraphics(true))
				{
					if (!graphic.Visible)
						continue;

					IMouseButtonHandler handler = graphic as IMouseButtonHandler;
					if (handler != null)
					{
						result = handler.Track(mouseInformation);
						if (result)
							break;
					}
				}

				return result;
			}

			public bool Stop(IMouseInformation mouseInformation)
			{
				bool result = false;

				if (_capturedHandler != null)
				{
					result = _capturedHandler.Stop(mouseInformation);
					if (!result)
					{
						_capturedHandler = null;
						return false;
					}
				}

				return result;
			}

			public void Cancel()
			{
				if (_capturedHandler != null)
				{
					_capturedHandler.Cancel();
					_capturedHandler = null;
				}
			}

			private static void TraverseChildGraphics(IEnumerable<IGraphic> graphics, Action<IGraphic> action)
			{
				if (graphics == null) return;

				foreach (var graphic in graphics)
				{
					action(graphic);

					var compositeGraphic = graphic as CompositeGraphic;
					if (compositeGraphic != null)
						TraverseChildGraphics((compositeGraphic).Graphics, action);
				}
			}
		}
	}
}
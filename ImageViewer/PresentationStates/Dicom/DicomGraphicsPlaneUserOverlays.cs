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

using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	public partial class DicomGraphicsPlane
	{
		private class UserOverlaysCollection : IDicomGraphicsPlaneOverlays
		{
			private readonly DicomGraphicsPlane _owner;
			private readonly List<OverlayPlaneGraphic> _overlays;

			public UserOverlaysCollection(DicomGraphicsPlane owner)
			{
				_owner = owner;
				_overlays = new List<OverlayPlaneGraphic>();
			}

			public int Count
			{
				get { return _overlays.Count; }
			}

			public void ActivateAsLayer(OverlayPlaneGraphic overlay, string layerId)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(_overlays.Contains(overlay), "Overlay must be part of collection.");
				ActivateAsLayer(_overlays.IndexOf(overlay), layerId);
			}

			public void ActivateAsLayer(int index, string layerId)
			{
				OverlayPlaneGraphic overlay = _overlays[index];
				if (overlay != null)
				{
					if (overlay.ParentGraphic is ShutterCollection)
					{
						ShutterCollection shutterCollection = (ShutterCollection) overlay.ParentGraphic;
						shutterCollection.Deactivate(overlay);
						shutterCollection.Graphics.Remove(overlay);
					}

					ILayer layer = _owner.Layers[layerId];
					if (overlay.ParentGraphic is CompositeGraphic && overlay.ParentGraphic != layer)
						((CompositeGraphic) overlay.ParentGraphic).Graphics.Remove(overlay);
					if (overlay.ParentGraphic == null)
						layer.Graphics.Add(overlay);
					overlay.Visible = true;
				}
			}

			public void ActivateAsShutter(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(_overlays.Contains(overlay), "Overlay must be part of collection.");
				ActivateAsShutter(_overlays.IndexOf(overlay));
			}

			public void ActivateAsShutter(int index)
			{
				OverlayPlaneGraphic overlay = _overlays[index];
				if (overlay != null)
				{
					if (overlay.ParentGraphic is CompositeGraphic && overlay.ParentGraphic != _owner.Shutters)
						((CompositeGraphic) overlay.ParentGraphic).Graphics.Remove(overlay);
					if (overlay.ParentGraphic == null)
						_owner.Shutters.Add(overlay);
					_owner.Shutters.Activate(overlay);
				}
				else
				{
					_owner.Shutters.DeactivateAll();
				}
			}

			public void Deactivate(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(_overlays.Contains(overlay), "Overlay must be part of collection.");
				Deactivate(_overlays.IndexOf(overlay));
			}

			public void Deactivate(int index)
			{
				OverlayPlaneGraphic overlay = _overlays[index];
				if (overlay != null)
				{
					if (overlay.ParentGraphic == _owner.Shutters)
						_owner.Shutters.Deactivate(overlay);
					if (overlay.ParentGraphic is LayerGraphic && _owner._layers.Graphics.Contains(overlay.ParentGraphic))
						ActivateAsLayer(index, string.Empty);
				}
			}

			public OverlayPlaneGraphic this[int index]
			{
				get { return _overlays[index]; }
			}

			public void Add(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(CanAdd(overlay), "Overlay is already part of a different collection.");
				if (overlay.ParentGraphic == null)
					_owner.Layers.InactiveLayer.Graphics.Add(overlay);
				_overlays.Add(overlay);
			}

			public void Remove(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(_overlays.Contains(overlay), "Overlay must be part of collection.");
				_overlays.Remove(overlay);
			}

			public bool Contains(int index)
			{
				return index >= 0 && index < _overlays.Count;
			}

			public bool Contains(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				return _overlays.Contains(overlay);
			}

			public void Clear()
			{
				_overlays.Clear();
			}

			public bool IsShutter(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(_overlays.Contains(overlay), "Overlay must be part of collection.");
				return (overlay.ParentGraphic == _owner.Shutters);
			}

			public bool IsLayer(OverlayPlaneGraphic overlay)
			{
				return !string.IsNullOrEmpty(GetLayer(overlay));
			}

			public string GetLayer(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(_overlays.Contains(overlay), "Overlay must be part of collection.");
				if (overlay.ParentGraphic is ILayer && overlay.ParentGraphic.ParentGraphic == _owner.Layers)
					return ((ILayer) overlay.ParentGraphic).Id;
				return null;
			}

			public IEnumerator<OverlayPlaneGraphic> GetEnumerator()
			{
				return _overlays.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			private bool CanAdd(OverlayPlaneGraphic overlay)
			{
				if (overlay.ParentGraphic == null)
					// The overlay has no current parent
					return true;

				if (overlay.ParentGraphic is LayerGraphic)
					// The owning DicomGraphicsPlane should be the parent of the LayerCollection that is the parent of the LayerGraphic
					return (overlay.ParentGraphic.ParentGraphic is LayerCollection && overlay.ParentGraphic.ParentGraphic.ParentGraphic == _owner);

				if (overlay.ParentGraphic is ShutterCollection)
					// The owning DicomGraphicsPlane should be the parent of the ShutterCollection that is the parent of the overlay
					return (overlay.ParentGraphic.ParentGraphic == _owner);

				return false;
			}
		}
	}
}
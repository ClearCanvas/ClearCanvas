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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Represents a collection of overlay planes available on an image.
	/// </summary>
	public interface IDicomGraphicsPlaneOverlays : IEnumerable<OverlayPlaneGraphic>
	{
		/// <summary>
		/// Gets the overlay plane at a specific index.
		/// </summary>
		/// <param name="index">The index of the overlay plane.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index is out of range.</exception>
		OverlayPlaneGraphic this[int index] { get; }

		/// <summary>
		/// Gets the number of overlay planes in the collection.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Activates the specified overlay plane in the given layer.
		/// </summary>
		/// <param name="overlay">The overlay plane to activate. This overlay must be part of the collection.</param>
		/// <param name="layerId">The ID of the layer on which to activate the overlay plane.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="overlay"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="overlay"/> is not part of the collection.</exception>
		void ActivateAsLayer(OverlayPlaneGraphic overlay, string layerId);

		/// <summary>
		/// Activates the specified overlay plane in the given layer.
		/// </summary>
		/// <param name="index">The index of the overlay plane to activate.</param>
		/// <param name="layerId">The ID of the layer on which to activate the overlay plane.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index is out of range.</exception>
		void ActivateAsLayer(int index, string layerId);

		/// <summary>
		/// Activates the specified overlay plane as a bitmap display shutter.
		/// </summary>
		/// <param name="overlay">The overlay plane to activate. This overlay must be part of the collection.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="overlay"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="overlay"/> is not part of the collection.</exception>
		void ActivateAsShutter(OverlayPlaneGraphic overlay);

		/// <summary>
		/// Activates the specified overlay plane as a bitmap display shutter.
		/// </summary>
		/// <param name="index">The index of the overlay plane to activate.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index is out of range.</exception>
		void ActivateAsShutter(int index);

		/// <summary>
		/// Deactivates the specified overlay plane.
		/// </summary>
		/// <param name="overlay">The overlay plane to activate. This overlay must be part of the collection.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="overlay"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="overlay"/> is not part of the collection.</exception>
		void Deactivate(OverlayPlaneGraphic overlay);

		/// <summary>
		/// Deactivates the specified overlay plane.
		/// </summary>
		/// <param name="index">The index of the overlay plane to activate.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index is out of range.</exception>
		void Deactivate(int index);

		/// <summary>
		/// Adds the specified overlay plane to the collection.
		/// </summary>
		/// <param name="overlay">The overlay plane to add to the collection. This overlay must not be a part of any other <see cref="IDicomGraphicsPlaneOverlays"/> collection.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="overlay"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="overlay"/> is part of another <see cref="IDicomGraphicsPlaneOverlays"/> collection.</exception>
		void Add(OverlayPlaneGraphic overlay);

		/// <summary>
		/// Removes the specified overlay plane from the collection.
		/// </summary>
		/// <param name="overlay">The overlay plane to remove from the collection. This overlay must be part of the collection.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="overlay"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="overlay"/> is not part of the collection.</exception>
		void Remove(OverlayPlaneGraphic overlay);

		/// <summary>
		/// Checks if the specified overlay plane is part of the collection.
		/// </summary>
		/// <param name="index">The index of the overlay plane to check for.</param>
		/// <returns>True if the specified overlay plane is part of the collection; False otherwise.</returns>
		bool Contains(int index);

		/// <summary>
		/// Checks if the specified overlay plane is part of the collection.
		/// </summary>
		/// <param name="overlay">The overlay plane to check for.</param>
		/// <returns>True if the specified overlay plane is part of the collection; False otherwise.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="overlay"/> is null.</exception>
		bool Contains(OverlayPlaneGraphic overlay);

		/// <summary>
		/// Clears the collection.
		/// </summary>
		void Clear();

		/// <summary>
		/// Checks if the specified overlay plane is currently activated as a bitmap display shutter.
		/// </summary>
		/// <param name="overlay">The overlay plane to check for.</param>
		/// <returns>True if the specified overlay plane is activated as a bitmap display shutter; False otherwise.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="overlay"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="overlay"/> is not part of the collection.</exception>
		bool IsShutter(OverlayPlaneGraphic overlay);

		/// <summary>
		/// Checks if the specified overlay plane is currently activated on a layer.
		/// </summary>
		/// <param name="overlay">The overlay plane to check for.</param>
		/// <returns>True if the specified overlay plane is activated on a layer; False otherwise.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="overlay"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="overlay"/> is not part of the collection.</exception>
		bool IsLayer(OverlayPlaneGraphic overlay);

		/// <summary>
		/// Gets the ID of the layer on which the specified overlay plane is currently activated, if applicable.
		/// </summary>
		/// <param name="overlay">The overlay plane to check for.</param>
		/// <returns>The layer ID of the overlay plane to check for.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="overlay"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="overlay"/> is not part of the collection.</exception>
		string GetLayer(OverlayPlaneGraphic overlay);
	}

	public partial class DicomGraphicsPlane
	{
		private class OverlaysCollection : IDicomGraphicsPlaneOverlays
		{
			private readonly DicomGraphicsPlane _owner;
			private readonly OverlayPlaneGraphic[] _overlays;

			public OverlaysCollection(DicomGraphicsPlane owner)
			{
				_owner = owner;
				_overlays = new OverlayPlaneGraphic[16];
			}

			public int Count
			{
				get
				{
					int count = 0;
					foreach (OverlayPlaneGraphic overlay in this)
						count++;
					return count;
				}
			}

			public void ActivateAsLayer(OverlayPlaneGraphic overlay, string layerId)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(_overlays[overlay.Index] == overlay, "Overlay must be part of collection.");
				ActivateAsLayer(overlay.Index, layerId);
			}

			public void ActivateAsLayer(int index, string layerId)
			{
				Platform.CheckArgumentRange(index, 0, 15, "index");
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
						((CompositeGraphic)overlay.ParentGraphic).Graphics.Remove(overlay);
					if (overlay.ParentGraphic == null)
						layer.Graphics.Add(overlay);
					overlay.Visible = true;
				}
			}

			public void ActivateAsShutter(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(_overlays[overlay.Index] == overlay, "Overlay must be part of collection.");
				ActivateAsShutter(overlay.Index);
			}

			public void ActivateAsShutter(int index)
			{
				Platform.CheckArgumentRange(index, 0, 15, "index");
				OverlayPlaneGraphic overlay = _overlays[index];
				if (overlay != null)
				{
					if (overlay.ParentGraphic is CompositeGraphic && overlay.ParentGraphic != _owner.Shutters)
						((CompositeGraphic)overlay.ParentGraphic).Graphics.Remove(overlay);
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
				Platform.CheckTrue(_overlays[overlay.Index] == overlay, "Overlay must be part of collection.");
				Deactivate(overlay.Index);
			}

			public void Deactivate(int index)
			{
				Platform.CheckArgumentRange(index, 0, 15, "index");
				OverlayPlaneGraphic overlay = _overlays[index];
				if (overlay != null)
				{
					if (overlay.ParentGraphic == _owner.Shutters)
						_owner.Shutters.Deactivate(overlay);
					if (overlay.ParentGraphic is LayerGraphic && _owner._layers.Graphics.Contains(overlay.ParentGraphic))
						this.ActivateAsLayer(index, string.Empty);
				}
			}

			public OverlayPlaneGraphic this[int index]
			{
				get
				{
					Platform.CheckArgumentRange(index, 0, 15, "index");
					return _overlays[index];
				}
			}

			public void Add(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(CanAdd(overlay), "Overlay is already part of a different collection.");
				if(overlay.ParentGraphic == null)
					_owner.Layers.InactiveLayer.Graphics.Add(overlay);
				_overlays[overlay.Index] = overlay;
			}

			public void Remove(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(_overlays[overlay.Index] == overlay, "Overlay must be part of collection.");
				_overlays[overlay.Index] = null;
			}

			public bool Contains(int index)
			{
				if (index < 0 || index > 15)
					return false;
				return _overlays[index] != null;
			}

			public bool Contains(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				return _overlays[overlay.Index] == overlay;
			}

			public void Clear()
			{
				// important! don't dispose anything since we don't own them...
				_overlays[0] = null;
				_overlays[1] = null;
				_overlays[2] = null;
				_overlays[3] = null;
				_overlays[4] = null;
				_overlays[5] = null;
				_overlays[6] = null;
				_overlays[7] = null;
				_overlays[8] = null;
				_overlays[9] = null;
				_overlays[10] = null;
				_overlays[11] = null;
				_overlays[12] = null;
				_overlays[13] = null;
				_overlays[14] = null;
				_overlays[15] = null;
			}

			public bool IsShutter(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(_overlays[overlay.Index] == overlay, "Overlay must be part of collection.");
				return (overlay.ParentGraphic == _owner.Shutters);
			}

			public bool IsLayer(OverlayPlaneGraphic overlay)
			{
				return !string.IsNullOrEmpty(GetLayer(overlay));
			}

			public string GetLayer(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(_overlays[overlay.Index] == overlay, "Overlay must be part of collection.");
				if (overlay.ParentGraphic is ILayer && overlay.ParentGraphic.ParentGraphic == _owner.Layers)
					return ((ILayer) overlay.ParentGraphic).Id;
				return null;
			}

			public IEnumerator<OverlayPlaneGraphic> GetEnumerator()
			{
				foreach (OverlayPlaneGraphic overlay in _overlays)
					if (overlay != null)
						yield return overlay;
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
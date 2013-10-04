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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Represents a collection of available DICOM graphic layers.
	/// </summary>
	public interface IDicomGraphicsPlaneLayers : IList<string>, IEnumerable<ILayer>
	{
		/// <summary>
		/// Gets the inactive layer.
		/// </summary>
		/// <remarks>
		/// The inactive layer is a special layer with an empty (0-length) <see cref="ILayer.Id"/>.
		/// This layer is never shown in the scene graph, and <see cref="IGraphic"/>s can be moved here to disable their display.
		/// Alternatively, the <see cref="IGraphic.Visible"/> property can be set on individual objects,
		/// but certain DICOM objects (such as overlays from presentation states) require a layer to be
		/// explicitly identified and, if the identified layer is the 0-length ID, should be disabled.
		/// </remarks>
		ILayer InactiveLayer { get; }

		/// <summary>
		/// Gets the layer with the specified ID.
		/// </summary>
		/// <remarks>
		/// If a layer with the specified ID does not exist, it is automatically created and returned.
		/// </remarks>
		/// <param name="layerId">The ID of the layer.</param>
		ILayer this[string layerId] { get; }

		/// <summary>
		/// Gets the layer at the specified index.
		/// </summary>
		/// <param name="index">The index of the layer.</param>
		new ILayer this[int index] { get; }

		/// <summary>
		/// Adds a layer with the specified ID.
		/// </summary>
		/// <param name="layerId">The ID of the layer.</param>
		/// <returns>The layer that was added.</returns>
		/// <exception cref="ArgumentException">Thrown if a layer with the same ID already exists.</exception>
		new ILayer Add(string layerId);

		/// <summary>
		/// Inserts a layer with the specified ID at the specified index.
		/// </summary>
		/// <param name="index">The index at which to insert the layer.</param>
		/// <param name="layerId">The ID of the layer.</param>
		/// <returns>The layer that was inserted.</returns>
		/// <exception cref="ArgumentException">Thrown if a layer with the same ID already exists.</exception>
		new ILayer Insert(int index, string layerId);

        /// <summary>
        /// Enables or disables all layers.
        /// </summary>
        bool Enabled { get; set; }
	}

	/// <summary>
	/// Represents a single DICOM graphics layer.
	/// </summary>
	public interface ILayer
	{
		/// <summary>
		/// Gets the ID string of this layer.
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Gets or sets a textual description of this layer.
		/// </summary>
		string Description { get; set; }

		/// <summary>
		/// Gets or sets a value indicating that this layer is visible.
		/// </summary>
		bool Visible { get; set; }

		/// <summary>
		/// Gets the <see cref="DicomGraphicsPlane"/> to which this layer belongs.
		/// </summary>
		DicomGraphicsPlane Owner { get; }

		/// <summary>
		/// Gets a collection of graphics on this layer.
		/// </summary>
		GraphicCollection Graphics { get; }
	}

	public partial class DicomGraphicsPlane
	{
		private static string FormatLayerId(string layerId)
		{
			if (string.IsNullOrEmpty(layerId) || layerId.TrimEnd().Length == 0)
				return string.Empty;
			StringBuilder sb = new StringBuilder();
			foreach (char c in layerId.Trim().ToUpperInvariant())
			{
				if (char.IsLetterOrDigit(c) || c == ' ' || c == '_')
				{
					sb.Append(c);
					if (sb.Length >= 16)
						break;
				}
			}
			if (sb.Length == 0)
				throw new ArgumentException("The supplied layer ID did not contain any valid characters.", "layerId");
			return sb.ToString();
		}

		[Cloneable(true)]
		private class LayerCollection : CompositeGraphic, IDicomGraphicsPlaneLayers
		{
			[CloneIgnore]
			private readonly Dictionary<string, LayerGraphic> _layers = new Dictionary<string, LayerGraphic>();

			[OnCloneComplete]
			private void OnCloneComplete()
			{
				foreach (LayerGraphic graphic in base.Graphics)
					_layers.Add(graphic.Id, graphic);
			}

            public bool Enabled
            {
                get { return base.Visible; }
                set { base.Visible = value; }
            }

		    public ILayer InactiveLayer
			{
				get { return this[string.Empty]; }
			}

			public ILayer this[string layerId]
			{
				get
				{
					layerId = FormatLayerId(layerId);
					if (!_layers.ContainsKey(layerId))
						return this.Add(layerId);
					return _layers[layerId];
				}
			}

			public ILayer this[int index]
			{
				get
				{
					Platform.CheckArgumentRange(index, 0, _layers.Count - 1, "index");
					return (LayerGraphic) base.Graphics[index];
				}
			}

			public int Count
			{
				get { return _layers.Count; }
			}

			public bool Contains(string layerId)
			{
				layerId = FormatLayerId(layerId);
				return _layers.ContainsKey(layerId);
			}

			public ILayer Add(string layerId)
			{
				layerId = FormatLayerId(layerId);
				if (_layers.ContainsKey(layerId))
					throw new ArgumentException("A layer with the same ID already exists.", "layerId");

				LayerGraphic layer = new LayerGraphic(layerId);
				_layers.Add(layerId, layer);
				base.Graphics.Add(layer);
				return layer;
			}

			public bool Remove(string layerId)
			{
				layerId = FormatLayerId(layerId);
				if (!_layers.ContainsKey(layerId))
					return false;

				base.Graphics.Remove(_layers[layerId]);
				_layers.Remove(layerId);
				return true;
			}

			public ILayer Insert(int index, string layerId)
			{
				Platform.CheckArgumentRange(index, 0, _layers.Count, "index");

				layerId = FormatLayerId(layerId);
				if (_layers.ContainsKey(layerId))
					throw new ArgumentException("A layer with the same ID already exists.", "layerId");

				LayerGraphic layer = new LayerGraphic(layerId);
				_layers.Add(layerId, layer);
				base.Graphics.Insert(index, layer);
				return layer;
			}

			public void RemoveAt(int index)
			{
				Platform.CheckArgumentRange(index, 0, _layers.Count - 1, "index");
				LayerGraphic layer = (LayerGraphic) base.Graphics[index];
				_layers.Remove(layer.Id);
				base.Graphics.Remove(layer);
			}

			public int IndexOf(string layerId)
			{
				layerId = FormatLayerId(layerId);
				if (!_layers.ContainsKey(layerId))
					return -1;

				return base.Graphics.IndexOf(_layers[layerId]);
			}

			public void Clear()
			{
				_layers.Clear();
				base.Graphics.Clear();
			}

			public IEnumerator<ILayer> GetEnumerator()
			{
				for (int n = 0; n < base.Graphics.Count; n++)
					yield return (LayerGraphic) base.Graphics[n];
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			#region IList<string> Members

			string IList<string>.this[int index]
			{
				get { return this[index].Id; }
				set { throw new NotSupportedException("Renaming layers is not supported at this time."); }
			}

			void ICollection<string>.Add(string layerId)
			{
				this.Add(layerId);
			}

			void IList<string>.Insert(int index, string layerId)
			{
				this.Insert(index, layerId);
			}

			void ICollection<string>.CopyTo(string[] array, int arrayIndex)
			{
				foreach (string layerId in (IEnumerable<string>) this)
					array[arrayIndex++] = layerId;
			}

			bool ICollection<string>.IsReadOnly
			{
				get { return false; }
			}

			IEnumerator<string> IEnumerable<string>.GetEnumerator()
			{
				for (int n = 0; n < base.Graphics.Count; n++)
					yield return ((LayerGraphic) base.Graphics[n]).Id;
			}

			#endregion
		}

		[Cloneable(true)]
		private sealed class LayerGraphic : CompositeGraphic, ILayer
		{
			private string _id;
			private int[] _displayCIELabColor;
			private int? _displayGrayscaleColor;
			private string _description;

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			internal LayerGraphic() {}

			internal LayerGraphic(string layerId)
			{
				_id = FormatLayerId(layerId);
				this.Visible = true;
			}

			public string Id
			{
				get { return _id; }
			}

			public string Description
			{
				get { return _description; }
				set { _description = value; }
			}

			public int? DisplayGrayscaleColor
			{
				get { return _displayGrayscaleColor; }
				set { _displayGrayscaleColor = value; }
			}

			public int[] DisplayCIELabColor
			{
				get { return _displayCIELabColor; }
				set { _displayCIELabColor = value; }
			}

			public DicomGraphicsPlane Owner
			{
				get
				{
					IGraphic layerCollection = base.ParentGraphic;
					if (layerCollection == null)
						return null;
					return (DicomGraphicsPlane) layerCollection.ParentGraphic;
				}
			}

			public override bool Visible
			{
				get { return base.Visible; }
				set
				{
					if (string.IsNullOrEmpty(_id))
					{
						base.Visible = false;
						return;
					}
					base.Visible = value;
				}
			}
		}
	}
}
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	/// <summary>
	/// A display shutter graphic consisting of a combination of a circular, rectangular, and polygonal shutter.
	/// </summary>
	[Cloneable]
	public class GeometricShuttersGraphic : CompositeGraphic, IShutterGraphic
	{
		internal const string DefaultName = "Geometric Shutters";

		private readonly Rectangle _imageRectangle;

		[CloneIgnore]
		private readonly List<GeometricShutter> _dicomShutters;

		[CloneIgnore]
		private readonly ReadOnlyCollection<GeometricShutter> _readOnlyDicomShutters;

		[CloneIgnore]
		private readonly ObservableList<GeometricShutter> _customShutters;

		[CloneIgnore]
		private ColorImageGraphic _imageGraphic;

		[CloneIgnore]
		private GeometricShutterCache.ICacheItem _cacheItem;

		private Color _fillColor = Color.Black;

		/// <summary>
		/// Constructs a new <see cref="GeometricShuttersGraphic"/> with the specified dimensions.
		/// </summary>
		/// <param name="rows">The number of rows in the display shutter.</param>
		/// <param name="columns">The number of columns in the display shutter.</param>
		public GeometricShuttersGraphic(int rows, int columns)
		{
			_imageRectangle = new Rectangle(0, 0, columns, rows);

			_customShutters = new ObservableList<GeometricShutter>();
			_customShutters.ItemAdded += OnCustomShuttersChanged;
			_customShutters.ItemRemoved += OnCustomShuttersChanged;
			_customShutters.ItemChanging += OnCustomShuttersChanged;
			_customShutters.ItemChanged += OnCustomShuttersChanged;

			_dicomShutters = new List<GeometricShutter>();
			_readOnlyDicomShutters = new ReadOnlyCollection<GeometricShutter>(_dicomShutters);

			base.Name = DefaultName;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected GeometricShuttersGraphic(GeometricShuttersGraphic source, ICloningContext context)
			: this(source._imageRectangle.Height, source._imageRectangle.Width)
		{
			context.CloneFields(source, this);

			foreach (GeometricShutter shutter in source._customShutters)
				_customShutters.Add(shutter.Clone());

			foreach (GeometricShutter shutter in source._dicomShutters)
				_dicomShutters.Add(shutter.Clone());
		}

		internal void AddDicomShutter(GeometricShutter dicomShutter)
		{
			_dicomShutters.Add(dicomShutter);
			Invalidate();
		}

		private bool HasShutters
		{
			get { return _customShutters.Count > 0 || _dicomShutters.Count > 0; }
		}

		/// <summary>
		/// Gets a readonly collection of the <see cref="GeometricShutter"/>s.
		/// </summary>
		public ReadOnlyCollection<GeometricShutter> DicomShutters
		{
			get { return _readOnlyDicomShutters; }
		}

		/// <summary>
		/// Gets a list of custom display shutters.
		/// </summary>
		public ObservableList<GeometricShutter> CustomShutters
		{
			get { return _customShutters; }
		}

		/// <summary>
		/// Gets or sets the presentation color which should replace the shuttered pixels.
		/// </summary>
		public Color FillColor
		{
			get { return _fillColor; }
			set
			{
				if (_fillColor == value)
					return;

				_fillColor = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Fires the <see cref="Graphic.Drawing"/> event.  Should be called by an <see cref="IRenderer"/>
		/// for each object just before it is drawn/rendered, hence the reason it is public.
		/// </summary>
		public override void OnDrawing()
		{
			base.OnDrawing();

			if (_imageGraphic != null || !HasShutters)
				return;

			_cacheItem = GeometricShutterCache.GetCacheItem(GetAllShutters(), _imageRectangle, _fillColor);
			//IMPORTANT: don't just pass the value of Raw, otherwise, memory management won't work (hard reference).
			_imageGraphic = new ColorImageGraphic(_cacheItem.PixelData.Rows, _cacheItem.PixelData.Columns, () => _cacheItem.PixelData.Raw);
			base.Graphics.Add(_imageGraphic);
		}

		/// <summary>
		/// Releases all resources used by this <see cref="CompositeGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && _cacheItem != null)
				_cacheItem = null;
		}

		private List<GeometricShutter> GetAllShutters()
		{
			var shutters = new List<GeometricShutter>(_dicomShutters);
			shutters.AddRange(_customShutters);
			return shutters;
		}

		private void Invalidate()
		{
			if (_imageGraphic != null)
			{
				base.Graphics.Remove(_imageGraphic);
				_imageGraphic.Dispose();
				_imageGraphic = null;

				//don't de-allocate and reallocate unnecessarily.
				if (!HasShutters)
					_cacheItem = null;
			}
		}

		private void OnCustomShuttersChanged(object sender, ListEventArgs<GeometricShutter> e)
		{
			Invalidate();
		}

		#region IShutterGraphic Members

		/// <summary>
		/// Gets or sets the 16-bit grayscale presentation value which should replace the shuttered pixels.
		/// </summary>
		/// <remarks>
		/// The display of shuttered pixels is no longer defined by the data source but is now
		/// implementation specific. The <see cref="IShutterGraphic.PresentationValue"/> and <see cref="IShutterGraphic.PresentationColor"/>
		/// properties allows this display to be customized by client code.
		/// </remarks>
		ushort IShutterGraphic.PresentationValue
		{
			get { return 0; }
			set { }
		}

		/// <summary>
		/// Gets or sets the presentation color which should replace the shuttered pixels.
		/// </summary>
		/// <remarks>
		/// The display of shuttered pixels is no longer defined by the data source but is now
		/// implementation specific. The <see cref="IShutterGraphic.PresentationValue"/> and <see cref="IShutterGraphic.PresentationColor"/>
		/// properties allows this display to be customized by client code.
		/// </remarks>
		Color IShutterGraphic.PresentationColor
		{
			get { return this.FillColor; }
			set { this.FillColor = value; }
		}

		#endregion
	}
}
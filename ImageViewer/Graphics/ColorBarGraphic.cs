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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A colour bar graphic depicting the full spectrum of displayable colours for a given colour map.
	/// </summary>
	[Cloneable]
	public class ColorBarGraphic : CompositeGraphic, IColorMapProvider, IColorMapInstaller
	{
		[CloneIgnore]
		private readonly ColorMapManager _colorMapManagerProxy;

		[CloneIgnore]
		private GrayscaleImageGraphic _colorBar;

		[CloneIgnore]
		private IGradientPixelData _gradientPixelData;

		private ColorBarOrientation _orientation;
		private PointF _location;
		private Size _size;
		private bool _reversed;

		/// <summary>
		/// Initializes a new vertical <see cref="ColorBarGraphic"/>.
		/// </summary>
		public ColorBarGraphic()
			: this(ColorBarOrientation.Vertical) {}

		/// <summary>
		/// Initializes a new <see cref="ColorBarGraphic"/> in the specified orientation.
		/// </summary>
		/// <param name="orientation">A value specifying the desired orientation of the colour bar.</param>
		public ColorBarGraphic(ColorBarOrientation orientation)
			: this(125, 15, orientation) {}

		/// <summary>
		/// Initializes a new <see cref="ColorBarGraphic"/> with the specified logical dimensions and orientation.
		/// </summary>
		/// <param name="length">The desired logical length of the colour bar along which the spectrum of colours will be ordered.</param>
		/// <param name="width">The desired logical width of the colour bar.</param>
		/// <param name="orientation">A value specifying the desired orientation of the colour bar.</param>
		public ColorBarGraphic(int length, int width, ColorBarOrientation orientation)
			: this(orientation == ColorBarOrientation.Horizontal ? new Size(length, width) : new Size(width, length), orientation) {}

		/// <summary>
		/// Initializes a new <see cref="ColorBarGraphic"/> with the specified physical dimensions and inferred orientation.
		/// </summary>
		/// <param name="size">The desired physical size of the colour bar. The orientation of the colour bar is inferred from the longer dimension.</param>
		public ColorBarGraphic(Size size)
			: this(size, size.Width > size.Height ? ColorBarOrientation.Horizontal : ColorBarOrientation.Vertical) {}

		/// <summary>
		/// Initializes a new <see cref="ColorBarGraphic"/> with the specified physical dimensions and orientation.
		/// </summary>
		/// <param name="size">The desired physical size of the colour bar.</param>
		/// <param name="orientation">A value specifying the desired orientation of the colour bar.</param>
		public ColorBarGraphic(Size size, ColorBarOrientation orientation)
		{
			_size = size;
			_location = new PointF(0, 0);
			_orientation = orientation;
			_reversed = false;
			_gradientPixelData = null;
			_colorMapManagerProxy = new ColorMapManager(new ColorMapInstallerProxy());
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected ColorBarGraphic(ColorBarGraphic source, ICloningContext context)
		{
			context.CloneFields(source, this);

			_colorMapManagerProxy = new ColorMapManager(new ColorMapInstallerProxy());
			if (source._colorMapManagerProxy.ColorMap != null)
				_colorMapManagerProxy.SetMemento(source._colorMapManagerProxy.CreateMemento());

			if (source._gradientPixelData != null)
				_gradientPixelData = source._gradientPixelData.Clone();
		}

		/// <summary>
		/// Releases all resources used by this <see cref="ColorBarGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_colorBar = null;

				if (_gradientPixelData != null)
				{
					_gradientPixelData.Dispose();
					_gradientPixelData = null;
				}
			}

			base.Dispose(disposing);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_colorBar = (GrayscaleImageGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g is GrayscaleImageGraphic);
		}

		/// <summary>
		/// Gets or sets the logical width of the colour bar.
		/// </summary>
		/// <remarks>
		/// Depending on the value of <see cref="Orientation"/>, the logical width may or may not be coincident with the physical width.
		/// </remarks>
		public int Width
		{
			get { return _orientation == ColorBarOrientation.Horizontal ? _size.Height : _size.Width; }
			set { this.Size = _orientation == ColorBarOrientation.Horizontal ? new Size(_size.Width, value) : new Size(value, _size.Height); }
		}

		/// <summary>
		/// Gets or sets the logical length of the colour bar along which the spectrum of colours are ordered.
		/// </summary>
		/// <remarks>
		/// Depending on the value of <see cref="Orientation"/>, the logical length may or may not be coincident with the physical height.
		/// </remarks>
		public int Length
		{
			get { return _orientation == ColorBarOrientation.Horizontal ? _size.Width : _size.Height; }
			set { this.Size = _orientation == ColorBarOrientation.Horizontal ? new Size(value, _size.Height) : new Size(_size.Width, value); }
		}

		/// <summary>
		/// Gets or sets the physical size of the colour bar.
		/// </summary>
		public Size Size
		{
			get { return _size; }
			set
			{
				if (_size != value)
				{
					_size = value;
					this.OnSizeChanged();
					this.OnVisualStateChanged("Size");
				}
			}
		}

		/// <summary>
		/// Gets or sets the position of the top-left corner of the colour bar.
		/// </summary>
		/// <remarks>
		/// This value may be in either the <see cref="CoordinateSystem.Source"/> or <see cref="CoordinateSystem.Destination"/>
		/// coordinate system depending on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		public PointF Location
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source && base.ParentGraphic != null)
					return base.ParentGraphic.SpatialTransform.ConvertToDestination(_location);
				return _location;
			}
			set
			{
				if (base.CoordinateSystem == CoordinateSystem.Source && base.ParentGraphic != null)
					value = base.ParentGraphic.SpatialTransform.ConvertToDestination(value);
				if (_location != value)
				{
					_location = value;
					this.SpatialTransform.TranslationX = _location.X;
					this.SpatialTransform.TranslationY = _location.Y;
					this.OnLocationChanged();
					//TODO (CR Sept 2010): VisualStateChanged?
				}
			}
		}

		/// <summary>
		/// Gets or sets the orientation of the colour bar.
		/// </summary>
		public ColorBarOrientation Orientation
		{
			get { return _orientation; }
			set
			{
				if (_orientation != value)
				{
					_orientation = value;
					this.OnOrientationChanged();
					this.OnVisualStateChanged("Orientation");
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not the spectrum should be reversed in order.
		/// </summary>
		/// <remarks>
		/// Normally, the colours of the spectrum are arranged such that left-to-right or top-to-bottom
		/// (depending on <see cref="Orientation"/>) coincides with increasing pixel intensity values.
		/// This behaviour can be reversed using this property such that low intensity values are to the right
		/// or bottom, and high intensity values are to the left or top.
		/// </remarks>
		public bool Reversed
		{
			get { return _reversed; }
			set
			{
				if (_reversed != value)
				{
					_reversed = value;
					this.OnReversedChanged();
					this.OnVisualStateChanged("Reversed");
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="GrayscaleImageGraphic"/> of the uncolourised colour bar.
		/// </summary>
		protected GrayscaleImageGraphic ColorBar
		{
			get
			{
				if (_colorBar == null)
				{
					base.Graphics.Add(_colorBar = CreateVerticalGradient());
					this.UpdateColorBar();
				}
				return _colorBar;
			}
		}

		/// <summary>
		/// Moves the <see cref="ColorBarGraphic"/> by a specified offset.
		/// </summary>
		/// <remarks>
		/// <paramref name="delta"/> may be in either the <see cref="CoordinateSystem.Source"/> or <see cref="CoordinateSystem.Destination"/>
		/// coordinate system depending on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		/// <param name="delta">The offset by which to move the colour bar.</param>
		public override void Move(SizeF delta)
		{
			this.Location += delta;
		}

		/// <summary>
		/// Called when the value of <see cref="Location"/> changes.
		/// </summary>
		protected virtual void OnLocationChanged() {}

		/// <summary>
		/// Called when the value of <see cref="Size"/> changes.
		/// </summary>
		protected virtual void OnSizeChanged()
		{
			this.UnloadColorBar();
		}

		/// <summary>
		/// Called when the value of <see cref="Reversed"/> changes.
		/// </summary>
		protected virtual void OnReversedChanged()
		{
			this.UpdateColorBar();
		}

		/// <summary>
		/// Called when the value of <see cref="Orientation"/> changes.
		/// </summary>
		protected virtual void OnOrientationChanged()
		{
			this.UpdateColorBar();
		}

		/// <summary>
		/// Called to create a vertical black and white gradient graphic with black at the top and white at the bottom.
		/// </summary>
		/// <remarks>
		/// This method is called to create a normalized gradient. User options such as <see cref="Reversed"/> and <see cref="Orientation"/> are applied automatically.
		/// </remarks>
		/// <returns>A new normalized gradient graphic.</returns>
		protected virtual GrayscaleImageGraphic CreateVerticalGradient()
		{
			if (_gradientPixelData != null)
				_gradientPixelData.Dispose();
			_gradientPixelData = GradientPixelData.GetGradient(this.Length, this.Width);
			return new GrayscaleImageGraphic(_gradientPixelData.Length, _gradientPixelData.Width, 8, 8, 7, false, false, 1, 0, () => _gradientPixelData.Data);
		}

		/// <summary>
		/// Called to unload the colour bar graphic and any associated resources.
		/// </summary>
		protected virtual void UnloadColorBar()
		{
			if (_colorBar != null)
			{
				base.Graphics.Remove(_colorBar);
				_colorBar.Dispose();
				_colorBar = null;
			}

			if (_gradientPixelData != null)
			{
				_gradientPixelData.Dispose();
				_gradientPixelData = null;
			}
		}

		private void UpdateColorBar()
		{
			if (_colorBar != null)
			{
				bool horizontal = _orientation == ColorBarOrientation.Horizontal;
				_colorBar.SpatialTransform.RotationXY = horizontal ? -90 : 0;
				_colorBar.SpatialTransform.TranslationX = horizontal ? -_colorBar.Columns : 0;
				_colorBar.VoiLutManager.Invert = _reversed;
			}
		}

		/// <summary>
		/// Creates the <see cref="ISpatialTransform"/> for this <see cref="ColorBarGraphic"/>.
		/// </summary>
		protected override SpatialTransform CreateSpatialTransform()
		{
			return new InvariantSpatialTransform(this);
		}

		/// <summary>
		/// Called by the framework just before the <see cref="ColorBarGraphic"/> is rendered.
		/// </summary>
		public override void OnDrawing()
		{
			// ensure the colorbar is created and the colormap is up to date
			var colorBar = this.ColorBar;
			colorBar.ColorMapManager.SetMemento(_colorMapManagerProxy.CreateMemento());

			base.OnDrawing();
		}

		#region IColorMapProvider Members

		/// <summary>
		/// Gets the <see cref="IColorMapManager"/> associated with this <see cref="ColorBarGraphic"/>.
		/// </summary>
		public IColorMapManager ColorMapManager
		{
			get { return _colorMapManagerProxy; }
		}

		#endregion

		#region IColorMapInstaller Members

		/// <summary>
		/// Gets the currently installed colour map.
		/// </summary>
		public IColorMap ColorMap
		{
			get { return this.ColorMapManager.ColorMap; }
		}

		/// <summary>
		/// Installs a colour map by name.
		/// </summary>
		public void InstallColorMap(string name)
		{
			this.ColorMapManager.InstallColorMap(name);
		}

		/// <summary>
		/// Installs a colour map by <see cref="ColorMapDescriptor">descriptor</see>.
		/// </summary>
		public void InstallColorMap(ColorMapDescriptor descriptor)
		{
			this.ColorMapManager.InstallColorMap(descriptor);
		}

		/// <summary>
		/// Installs a colour map.
		/// </summary>
		public void InstallColorMap(IColorMap colorMap)
		{
			this.ColorMapManager.InstallColorMap(colorMap);
		}

		IEnumerable<ColorMapDescriptor> IColorMapInstaller.AvailableColorMaps
		{
			get { return this.ColorMapManager.AvailableColorMaps; }
		}

		#endregion

		#region GradientPixelData Class

		private interface IGradientPixelData : IDisposable
		{
			int Width { get; }
			int Length { get; }
			byte[] Data { get; }
			IGradientPixelData Clone();
		}

		private class GradientPixelData : IDisposable
		{
			//TODO (CR Sept 2010): General comment.  For something that is essentially created through a factory
			//you could use simple proxies with reference counting instead of "transient references".  The transient
			//reference idea was invented for objects that are publicly exposed (like Sop) and can't be "hidden" behind
			//a proxy/interface, but still need to "live" after it's original owner/creator has disposed it.  I've always wanted to scrap it.
			//Also, for anything that is essentially a simple data buffer that can be recreated at will from some
			//basic parameters, you could avoid reference counting entirely and use a weak reference cache.  A
			//weak reference cache is ideal for this type of object and avoids problems with forgetting to dispose
			//references, which ultimately causes memory leaks.

			//TODO (CR Sept 2010): this object should be synchronized, especially since we could end up modifying
			//it from multiple threads (e.g. UI thread and clipboard worker).
			private static readonly Dictionary<Size, GradientPixelData> _cachedGradients = new Dictionary<Size, GradientPixelData>();
			private readonly Size _normalizedSize;
			private byte[] _data;

			public static IGradientPixelData GetGradient(int length, int width)
			{
				var normalizedSize = new Size(width, length);
				if (_cachedGradients.ContainsKey(normalizedSize))
					return _cachedGradients[normalizedSize].CreateTransientReference();
				//TODO (CR Sept 2010): because a transient reference is always created and returned, the "real"
				//object will never get disposed, and hence _selfDisposed will never be true.
				//_cachedGradients is therefore a memory leak, albeit small (there will only ever be one item in it based on current usage).
				return new GradientPixelData(normalizedSize).CreateTransientReference();
			}

			private GradientPixelData(Size normalizedSize)
			{
				_normalizedSize = normalizedSize;
				_cachedGradients.Add(normalizedSize, this);
			}

			protected void Dispose(bool disposing)
			{
				if (disposing)
				{
					_data = null;
					_cachedGradients.Remove(_normalizedSize);
				}
			}

			public byte[] Data
			{
				get
				{
					//TODO (CR Sept 2010): since this could be shared between threads, it should technically be sync'ed,
					//but it doesn't really do any harm not to (you might just end up with the odd "rogue" one).
					if (_data == null)
					{
						int bufferSize = _normalizedSize.Height*_normalizedSize.Width;
						var buffer = new byte[bufferSize];
						for (int n = 0; n < bufferSize; n++)
							buffer[n] = (byte) (((int) (n/_normalizedSize.Width))*255f/(_normalizedSize.Height - 1));
						_data = buffer;
					}
					return _data;
				}
			}

			#region Transient Reference Support

			private class GradientReference : IGradientPixelData
			{
				private GradientPixelData _gradientPixelData;

				public GradientReference(GradientPixelData gradientPixelData)
				{
					_gradientPixelData = gradientPixelData;
					_gradientPixelData.OnReferenceCreated();
				}

				public int Length
				{
					get { return _gradientPixelData._normalizedSize.Height; }
				}

				public int Width
				{
					get { return _gradientPixelData._normalizedSize.Width; }
				}

				public byte[] Data
				{
					get { return _gradientPixelData.Data; }
				}

				public IGradientPixelData Clone()
				{
					return _gradientPixelData.CreateTransientReference();
				}

				public void Dispose()
				{
					if (_gradientPixelData != null)
					{
						_gradientPixelData.OnReferenceDisposed();
						_gradientPixelData = null;
					}
				}
			}

			private readonly object _syncLock = new object();
			private int _transientReferenceCount = 0;
			private bool _selfDisposed = false;

			private void OnReferenceDisposed()
			{
				lock (_syncLock)
				{
					if (_transientReferenceCount > 0)
						--_transientReferenceCount;

					if (_transientReferenceCount == 0 && _selfDisposed)
						DisposeInternal();
				}
			}

			private void OnReferenceCreated()
			{
				lock (_syncLock)
				{
					if (_transientReferenceCount == 0 && _selfDisposed)
						throw new ObjectDisposedException("");

					++_transientReferenceCount;
				}
			}

			private void DisposeInternal()
			{
				try
				{
					this.Dispose(true);
					GC.SuppressFinalize(this);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Debug, e);
				}
			}

			/// <summary>
			/// Creates a new 'transient reference' to this <see cref="GradientPixelData"/>.
			/// </summary>
			public IGradientPixelData CreateTransientReference()
			{
				return new GradientReference(this);
			}

			/// <summary>
			/// Implementation of the <see cref="IDisposable"/> pattern.
			/// </summary>
			public void Dispose()
			{
				lock (_syncLock)
				{
					//TODO (CR Sept 2010): this will never be called; see comments above.
					_selfDisposed = true;

					//Only dispose for real when self has been disposed and all the transient references have been disposed.
					if (_transientReferenceCount == 0)
						DisposeInternal();
				}
			}

			#endregion
		}

		#endregion

		#region ColorMapInstallerProxy Class

		private class ColorMapInstallerProxy : IColorMapInstaller
		{
			private IColorMap _colorMap;
			private string _colorMapName = string.Empty;

			public IColorMap ColorMap
			{
				get
				{
					if (_colorMap == null && !string.IsNullOrEmpty(_colorMapName))
					{
						using (var lutFactory = LutFactory.Create())
						{
							_colorMap = lutFactory.GetColorMap(_colorMapName);
						}
					}
					return _colorMap;
				}
			}

			public void InstallColorMap(string name)
			{
				if (_colorMapName != name)
				{
					_colorMapName = name;
					_colorMap = null;
				}
			}

			public void InstallColorMap(ColorMapDescriptor descriptor)
			{
				this.InstallColorMap(descriptor.Name);
			}

			public void InstallColorMap(IColorMap colorMap)
			{
				if (_colorMap != colorMap)
				{
					_colorMap = colorMap;
					_colorMapName = null;
				}
			}

			public IEnumerable<ColorMapDescriptor> AvailableColorMaps
			{
				get
				{
					using (var lutFactory = LutFactory.Create())
					{
						return lutFactory.AvailableColorMaps;
					}
				}
			}
		}

		#endregion
	}

	/// <summary>
	/// Enumeration of values specifying the orientation of a <see cref="ColorBarGraphic"/>.
	/// </summary>
	public enum ColorBarOrientation
	{
		/// <summary>
		/// Specifies that the color bar is oriented horizontally.
		/// </summary>
		Horizontal,

		/// <summary>
		/// Specifies that the color bar is oriented vertically.
		/// </summary>
		Vertical
	}
}
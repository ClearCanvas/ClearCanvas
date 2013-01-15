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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[Cloneable]
	internal class ColorMapManagerProxy : IColorMapManager
	{
		[CloneIgnore]
		private readonly XColorMapInstaller _placeholderColorMapInstaller;

		[CloneIgnore]
		private readonly IColorMapManager _placeholderColorMapManager;

		[CloneIgnore]
		private readonly ILayerOpacityManager _layerOpacityManager;

		[CloneIgnore]
		private IColorMapManager _realColorMapManager;

		public ColorMapManagerProxy()
		{
			_placeholderColorMapManager = new ColorMapManager(_placeholderColorMapInstaller = new XColorMapInstaller());
			_layerOpacityManager = new XLayerOpacityManager(this);
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected ColorMapManagerProxy(ColorMapManagerProxy source, ICloningContext context)
		{
			context.CloneFields(source, this);

			_placeholderColorMapManager = new ColorMapManager(_placeholderColorMapInstaller = source._placeholderColorMapInstaller.Clone());
			_layerOpacityManager = new XLayerOpacityManager(this);
		}

		public void SetRealColorMapManager(IColorMapManager realColorMapManager)
		{
			_realColorMapManager = realColorMapManager;
			InstallColorMap();
		}

		private void InstallColorMap()
		{
			if (_realColorMapManager != null)
				_realColorMapManager.InstallColorMap(_placeholderColorMapManager.ColorMap);
		}

		public ILayerOpacityManager LayerOpacityManager
		{
			get { return _layerOpacityManager; }
		}

		#region IColorMapManager Members

		[Obsolete("Use the ColorMap property instead.")]
		IColorMap IColorMapManager.GetColorMap()
		{
			return _placeholderColorMapManager.GetColorMap();
		}

		#endregion

		#region IColorMapInstaller Members

		IColorMap IColorMapInstaller.ColorMap
		{
			get { return _placeholderColorMapManager.ColorMap; }
		}

		void IColorMapInstaller.InstallColorMap(string name)
		{
			_placeholderColorMapManager.InstallColorMap(name);
			InstallColorMap();
		}

		void IColorMapInstaller.InstallColorMap(ColorMapDescriptor descriptor)
		{
			_placeholderColorMapManager.InstallColorMap(descriptor);
			InstallColorMap();
		}

		void IColorMapInstaller.InstallColorMap(IColorMap colorMap)
		{
			_placeholderColorMapManager.InstallColorMap(colorMap);
			InstallColorMap();
		}

		IEnumerable<ColorMapDescriptor> IColorMapInstaller.AvailableColorMaps
		{
			get { return _placeholderColorMapManager.AvailableColorMaps; }
		}

		#endregion

		#region IMemorable Members

		public object CreateMemento()
		{
			return _placeholderColorMapManager.CreateMemento();
		}

		public void SetMemento(object memento)
		{
			_placeholderColorMapManager.SetMemento(memento);
			InstallColorMap();
		}

		#endregion

		#region XLayerOpacityManager Class

		private class XLayerOpacityManager : ILayerOpacityManager
		{
			private readonly ColorMapManagerProxy _owner;

			public XLayerOpacityManager(ColorMapManagerProxy owner)
			{
				_owner = owner;
			}

			public bool Enabled
			{
				get { return true; }
				set { }
			}

			public float Opacity
			{
				get { return _owner._placeholderColorMapInstaller.Opacity; }
				set
				{
					Platform.CheckTrue(value >= 0f && value <= 1f, "Opacity must be between 0 and 1.");
					if (_owner._placeholderColorMapInstaller.Opacity != value)
					{
						_owner._placeholderColorMapInstaller.Opacity = value;
						_owner.InstallColorMap();
					}
				}
			}

			public bool Thresholding
			{
				get { return _owner._placeholderColorMapInstaller.Thresholding; }
				set
				{
					if (_owner._placeholderColorMapInstaller.Thresholding != value)
					{
						_owner._placeholderColorMapInstaller.Thresholding = value;
						_owner.InstallColorMap();
					}
				}
			}

			public object CreateMemento()
			{
				return new Memento(Opacity, Thresholding);
			}

			public void SetMemento(object @object)
			{
				if (@object is Memento)
				{
					var memento = (Memento) @object;
					var isChanged = false;
					if (_owner._placeholderColorMapInstaller.Opacity != memento.Opacity)
					{
						_owner._placeholderColorMapInstaller.Opacity = memento.Opacity;
						isChanged = true;
					}
					if (_owner._placeholderColorMapInstaller.Thresholding != memento.Thresholding)
					{
						_owner._placeholderColorMapInstaller.Thresholding = memento.Thresholding;
						isChanged = true;
					}
					if (isChanged)
					{
						_owner.InstallColorMap();
					}
				}
			}

			//TODO (CR Sept 2010): unless it's not actually used outside of this class, all mementos should override Equals
			private class Memento
			{
				public readonly float Opacity;
				public readonly bool Thresholding;

				public Memento(float opacity, bool thresholding)
				{
					Opacity = opacity;
					Thresholding = thresholding;
				}
			}
		}

		#endregion

		#region XColorMapInstaller Class

		private class XColorMapInstaller : IColorMapInstaller
		{
			private IColorMap _alphaColorMap;
			private IColorMap _colorMap;
			private string _colorMapName = HotIronColorMapFactory.ColorMapName;
			private bool _thresholding = false;
			private float _opacity = 0.5f;

			public XColorMapInstaller() {}

			public XColorMapInstaller Clone()
			{
				var clone = new XColorMapInstaller();
				clone._alphaColorMap = _alphaColorMap != null ? (IColorMap) _alphaColorMap.Clone() : null;
				clone._colorMap = _colorMap != null ? (IColorMap) _colorMap.Clone() : null;
				clone._colorMapName = _colorMapName;
				clone._thresholding = _thresholding;
				clone._opacity = _opacity;
				return clone;
			}

			public bool Thresholding
			{
				get { return _thresholding; }
				set
				{
					if (_thresholding != value)
					{
						_thresholding = value;
						_alphaColorMap = null;
					}
				}
			}

			public float Opacity
			{
				get { return _opacity; }
				set
				{
					if (_opacity != value)
					{
						_opacity = value;
						_alphaColorMap = null;
					}
				}
			}

			public IColorMap ColorMap
			{
				get
				{
					if (_alphaColorMap == null)
					{
						if (!string.IsNullOrEmpty(_colorMapName))
							_alphaColorMap = AlphaColorMapFactory.GetColorMap(_colorMapName, (byte) (byte.MaxValue*_opacity), _thresholding);
						else if (_colorMap != null)
							_alphaColorMap = AlphaColorMapFactory.GetColorMap(_colorMap, (byte) (byte.MaxValue*_opacity), _thresholding);
					}
					return _alphaColorMap;
				}
			}

			public void InstallColorMap(string name)
			{
				if (_colorMapName != name)
				{
					_colorMapName = name;
					_colorMap = null;
					_alphaColorMap = null;
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
					_alphaColorMap = null;
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
}
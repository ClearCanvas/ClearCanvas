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
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A Color Map Manager, which is responsible for managing installation and restoration
	/// of color maps via the Memento pattern.
	/// </summary>
	public sealed class ColorMapManager : IColorMapManager
	{
		#region Private Fields

		private readonly IColorMapInstaller _colorMapInstaller;
		
		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ColorMapManager(IColorMapInstaller colorMapInstaller)
		{
			Platform.CheckForNullReference(colorMapInstaller, "colorMapInstaller");
			_colorMapInstaller = colorMapInstaller;
		}

		#region IColorMapManager Members

		/// <summary>
		/// Gets the currently installed color map.
		/// </summary>
		public IColorMap GetColorMap()
		{
			return _colorMapInstaller.ColorMap;
		}

		#endregion

		#region IColorMapInstaller Members

		/// <summary>
		/// Gets the currently installed color map.
		/// </summary>
		public IColorMap ColorMap
		{
			get { return _colorMapInstaller.ColorMap; }
		}

		/// <summary>
		/// Installs a color map by name.
		/// </summary>
		public void InstallColorMap(string name)
		{
			_colorMapInstaller.InstallColorMap(name);
		}

		/// <summary>
		/// Installs a color map by <see cref="ColorMapDescriptor">descriptor</see>.
		/// </summary>
		public void InstallColorMap(ColorMapDescriptor descriptor)
		{
			_colorMapInstaller.InstallColorMap(descriptor);
		}

		/// <summary>
		/// Installs a color map.
		/// </summary>
		public void InstallColorMap(IColorMap colorMap)
		{
			_colorMapInstaller.InstallColorMap(colorMap);
		}

		/// <summary>
		/// Gets <see cref="ColorMapDescriptor"/>s for all the different types of available color maps.
		/// </summary>
		public IEnumerable<ColorMapDescriptor> AvailableColorMaps
		{
			get
			{
				return _colorMapInstaller.AvailableColorMaps;
			}
		}

		#endregion

		#region IMemorable Members

		/// <summary>
		/// Captures enough information to restore the currently installed color map.
		/// </summary>
		public object CreateMemento()
		{
			return new ColorMapMemento(_colorMapInstaller.ColorMap);
		}

		/// <summary>
		/// Restores the previously installed color map and/or it's state.
		/// </summary>
		public void SetMemento(object memento)
		{
			ColorMapMemento colorMapMemento = (ColorMapMemento) memento;

			if (_colorMapInstaller.ColorMap != colorMapMemento.Originator)
				_colorMapInstaller.InstallColorMap(colorMapMemento.Originator);

			if (colorMapMemento.InnerMemento != null)
				_colorMapInstaller.ColorMap.SetMemento(colorMapMemento.InnerMemento);
		}

		#endregion
	}
}

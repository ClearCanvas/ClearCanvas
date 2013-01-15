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

using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Provides a description of a color map.
	/// </summary>
	/// <seealso cref="IColorMapFactory"/>
	public sealed class ColorMapDescriptor
	{
		#region Private Fields

		private readonly string _name;
		private readonly string _description;

		#endregion

		#region Private Constructor

		private ColorMapDescriptor(string name, string description)
		{
			_name = name;
			_description = description;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the name of the factory.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets a brief description of the factory.
		/// </summary>
		public string Description
		{
			get { return _description; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates a <see cref="ColorMapDescriptor"/> given an input <see cref="IColorMapFactory"/>.
		/// </summary>
		public static ColorMapDescriptor FromFactory(IColorMapFactory factory)
		{
			Platform.CheckForNullReference(factory, "factory");
			return new ColorMapDescriptor(factory.Name, factory.Description);
		}

		#endregion
	}
}

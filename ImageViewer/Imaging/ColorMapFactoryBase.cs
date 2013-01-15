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

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A base implementation for <see cref="IColorMap"/> factories.
	/// </summary>
	/// <typeparam name="T">Must be derived from <see cref="IColorMap"/> and have a parameterless default constructor.</typeparam>
	/// <seealso cref="IColorMapFactory"/>
	/// <seealso cref="IColorMap"/>
	public abstract class ColorMapFactoryBase<T> : IColorMapFactory
		where T : IColorMap, new()
	{
		#region Protected Constructor

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected ColorMapFactoryBase()
		{
		}

		#endregion

		#region IColorMapFactory Members

		/// <summary>
		/// Gets a name that should be unique when compared to other <see cref="IColorMapFactory"/>s.
		/// </summary>
		/// <remarks>
		/// This name should not be a resource string, as it should be language-independent.
		/// </remarks>
		public abstract string Name { get; }

		/// <summary>
		/// Gets a brief description of the factory.
		/// </summary>
		public abstract string Description { get; }

		/// <summary>
		/// Creates and returns a <see cref="IColorMap"/>.
		/// </summary>
		public IColorMap Create()
		{
			return new T();
		}

		#endregion
	}
}

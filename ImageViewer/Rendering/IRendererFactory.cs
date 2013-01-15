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

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// Thrown when an <see cref="IRendererFactory"/> cannot initialize, 
	/// for example when the required hardware is not present.
	/// </summary>
	public class RendererFactoryInitializationException : Exception
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public RendererFactoryInitializationException(string message)
			: base(message)
		{
		}
	}

	/// <summary>
	/// A factory for <see cref="IRenderer"/>s.
	/// </summary>
	public interface IRendererFactory
	{
		/// <summary>
		/// Initializes a <see cref="IRendererFactory"/>.
		/// </summary>
		/// <exception cref="RendererFactoryInitializationException">
		/// Thrown when the <see cref="IRendererFactory"/> cannot initialize, for example
		/// when the required hardware is not present.
		/// </exception>
		void Initialize();

		/// <summary>
		/// Gets an <see cref="IRenderer"/>.
		/// </summary>
		IRenderer GetRenderer();
	}
}

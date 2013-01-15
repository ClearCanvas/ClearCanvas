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
	/// An extension point for custom <see cref="IInitialVoiLutProvider"/>s.
	/// </summary>
	/// <seealso cref="IInitialVoiLutProvider"/>
	public sealed class InitialVoiLutProviderExtensionPoint : ExtensionPoint<IInitialVoiLutProvider>
	{
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public InitialVoiLutProviderExtensionPoint()
		{
		}
	}

	//TODO (cr Oct 2009): deprecate and use something with a more appropriate name?

	/// <summary>
	/// A provider of an image's Initial Voi Lut.
	/// </summary>
	/// <remarks>
	/// Implementors can apply logic based on the input <see cref="IPresentationImage"/> 
	/// to decide what type of Lut to return.
	/// </remarks>
	/// <seealso cref="InitialVoiLutProviderExtensionPoint"/>
	public interface IInitialVoiLutProvider
	{
		/// <summary>
		/// Determines and returns the initial Voi Lut that should be applied to the input <see cref="IPresentationImage"/>.
		/// </summary>
		/// <param name="presentationImage">The <see cref="IPresentationImage"/> whose intial Lut is to be determined.</param>
		/// <returns>The Voi Lut as an <see cref="IVoiLut"/>.</returns>
		IVoiLut GetLut(IPresentationImage presentationImage);
	}
}

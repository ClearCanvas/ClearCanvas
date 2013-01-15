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
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines a factory for getting a VOI LUT appropriate for an <see cref="ImageGraphic"/>.
	/// </summary>
	public interface IGraphicVoiLutFactory
	{
		/// <summary>
		/// Creates a Voi LUT suitable for the given <paramref name="imageGraphic"/>.
		/// </summary>
		/// <returns>The VOI LUT as an <see cref="IVoiLut"/>.</returns>
		IVoiLut CreateVoiLut(ImageGraphic imageGraphic);
	}

	/// <summary>
	/// A base class defines a factory for getting a VOI LUT appropriate for an <see cref="ImageGraphic"/>.
	/// </summary>
	public abstract class GraphicVoiLutFactory : IGraphicVoiLutFactory
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		protected GraphicVoiLutFactory() {}

		/// <summary>
		/// Creates a Voi LUT suitable for the given <paramref name="imageGraphic"/>.
		/// </summary>
		/// <returns>The VOI LUT as an <see cref="IVoiLut"/>.</returns>
		public abstract IVoiLut CreateVoiLut(ImageGraphic imageGraphic);

		/// <summary>
		/// Defines the method for creating a Voi LUT suitable for the given <paramref name="imageGraphic"/>.
		/// </summary>
		/// <returns>The VOI LUT as an <see cref="IVoiLut"/>.</returns>
		public delegate IVoiLut CreateVoiLutDelegate(ImageGraphic imageGraphic);

		/// <summary>
		/// Creates a new factory that wraps the given delegate.
		/// </summary>
		/// <param name="createVoiLutDelegate">A <see cref="CreateVoiLutDelegate"/> delegate to
		/// get a VOI LUT appropriate for the given <see cref="ImageGraphic"/>.
		/// This method should generally be static, as the factory may only be reference-copied when the parent graphic is cloned.</param>
		/// <returns>The VOI LUT as an <see cref="IVoiLut"/>.</returns>
		public static GraphicVoiLutFactory Create(CreateVoiLutDelegate createVoiLutDelegate)
		{
			return new DelegateGraphicVoiLutFactory(createVoiLutDelegate);
		}

		private class DelegateGraphicVoiLutFactory : GraphicVoiLutFactory
		{
			private readonly CreateVoiLutDelegate _createVoiLutDelegate;

			public DelegateGraphicVoiLutFactory(CreateVoiLutDelegate createVoiLutDelegate)
			{
				Platform.CheckForNullReference(createVoiLutDelegate, "createVoiLutDelegate");
				_createVoiLutDelegate = createVoiLutDelegate;
			}

			public override IVoiLut CreateVoiLut(ImageGraphic imageGraphic)
			{
				return _createVoiLutDelegate(imageGraphic);
			}
		}
	}
}
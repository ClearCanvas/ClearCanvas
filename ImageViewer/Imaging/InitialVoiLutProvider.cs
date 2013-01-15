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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.Imaging
{
	//TODO (cr Oct 2009): get rid of this.
	/// <summary>
	/// A factory that provides the initial voi lut for a given <see cref="IPresentationImage"/>.
	/// </summary>
	public sealed class InitialVoiLutProvider : IInitialVoiLutProvider
	{
		#region Private Fields

		private static readonly InitialVoiLutProvider _instance = new InitialVoiLutProvider();

		private readonly IInitialVoiLutProvider _extensionProvider;

		#endregion

		private InitialVoiLutProvider()
		{
			try
			{
				_extensionProvider = new InitialVoiLutProviderExtensionPoint().CreateExtension() as IInitialVoiLutProvider;
			}
			catch (NotSupportedException e)
			{
				Platform.Log(LogLevel.Info, e);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#region Public Members

		/// <summary>
		/// The single instance of the provider/factory.
		/// </summary>
		public static InitialVoiLutProvider Instance
		{
			get { return _instance; }
		}

		#region IInitialVoiLutProvider Members

		/// <summary>
		/// Determines and returns the initial Voi Lut that should be applied to the input <see cref="IPresentationImage"/>.
		/// </summary>
		/// <param name="presentationImage">The <see cref="IPresentationImage"/> whose intial Lut is to be determined.</param>
		/// <returns>The Voi Lut as an <see cref="IVoiLut"/>.</returns>
		public IVoiLut GetLut(IPresentationImage presentationImage)
		{
			IVoiLut lut = null;
			if (_extensionProvider != null)
				lut = _extensionProvider.GetLut(presentationImage);

			return lut;
		}

		#endregion
		#endregion
	}
}

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
using System.Drawing;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// The default (grayscale) color map.
	/// </summary>
	internal sealed class GrayscaleColorMapFactory : ColorMapFactoryBase<GrayscaleColorMap>
	{
		/// <summary>
		/// Returns the factory name.
		/// </summary>
		public static readonly string FactoryName = "Grayscale";

		/// <summary>
		/// Default Constructor.
		/// </summary>
		public GrayscaleColorMapFactory()
		{
		}

		/// <summary>
		/// Returns the Name of the factory.
		/// </summary>
		public override string Name
		{
			get { return FactoryName; }
		}

		/// <summary>
		/// Returns a brief description of the Factory.
		/// </summary>
		public override string Description
		{
			get { return SR.DescriptionGrayscaleColorMap; }
		}
	}

	/// <summary>
	/// A Grayscale Color Map.
	/// </summary>
	/// <remarks>
	/// This class should not be instantiated directly, but through the corresponding factory.
	/// </remarks>
	internal class GrayscaleColorMap : ColorMap
	{
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public GrayscaleColorMap()
			: base()
		{
		}

		/// <summary>
		/// Generates the Lut.
		/// </summary>
        protected override void Create()
		{
		    int j = 0;
		    double maxGrayLevel = Length - 1;
		    int min = MinInputValue;
		    int max = MaxInputValue;

		    const int alphaMask = 0xFF << 24;

		    for (int i = min; i <= max; i++)
		    {
		        double scale = j++/maxGrayLevel;
		        byte value = (byte) Math.Round(byte.MaxValue*scale);
		        int color = alphaMask | (value << 16) | (value << 8) | value;
		        this[i] = color;
		    }
		}

	    /// <summary>
		/// Returns an abbreviated description of the Lut.
		/// </summary>
		public override string GetDescription()
		{
			return SR.DescriptionGrayscaleColorMap;
		}
	}
}
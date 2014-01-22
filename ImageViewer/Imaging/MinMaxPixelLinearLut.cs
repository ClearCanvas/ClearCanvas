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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A Linear LUT whose <see cref="WindowWidth"/> and <see cref="WindowCenter"/> 
	/// are calculated based on the minimum and maximum values in the raw pixel data.
	/// </summary>
	/// <seealso cref="CalculatedVoiLutLinear"/>
	[Cloneable]
	public sealed class MinMaxPixelLinearLut : CalculatedVoiLutLinear
	{
		#region Private Fields

		private readonly double _windowWidth;
		private readonly double _windowCenter;

		#endregion

		#region Public Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="minPixelValue">The minimum raw pixel value in the pixel data.</param>
		/// <param name="maxPixelValue">The maximum raw pixel value in the pixel data.</param>
		/// <param name="modalityLut">The modality LUT to use for calculating <see cref="AlgorithmCalculatedVoiLutLinear.WindowWidth"/>
		/// and <see cref="AlgorithmCalculatedVoiLutLinear.WindowCenter"/>, if applicable.</param>
		public MinMaxPixelLinearLut(int minPixelValue, int maxPixelValue, IModalityLut modalityLut)
		{
			double windowStart = minPixelValue;
			double windowEnd = maxPixelValue;

			if (modalityLut != null)
			{
				windowStart = modalityLut[windowStart];
				windowEnd = modalityLut[windowEnd];
			}

			// round the window to one decimal place so it's not ridiculous
			// value is calculated anyway and thus has no significance outside of display
			var windowWidth = Math.Max(windowEnd - windowStart + 1, 1);
			_windowWidth = Math.Round(windowWidth, 1);
			_windowCenter = Math.Round(windowStart + windowWidth/2, 1);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="minPixelValue">The minimum raw pixel value in the pixel data.</param>
		/// <param name="maxPixelValue">The maximum raw pixel value in the pixel data.</param>
		public MinMaxPixelLinearLut(int minPixelValue, int maxPixelValue)
			: this(minPixelValue, maxPixelValue, null) {}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		private MinMaxPixelLinearLut(MinMaxPixelLinearLut source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		#endregion

		#region Overrides

		public override double WindowWidth
		{
			get { return _windowWidth; }
		}

		public override double WindowCenter
		{
			get { return _windowCenter; }
		}

		/// <summary>
		/// Gets an abbreviated description of the LUT.
		/// </summary>
		public override string GetDescription()
		{
			return string.Format(SR.FormatDescriptionMinMaxCalculatedLinearLut, WindowWidth, WindowCenter);
		}

		#endregion
	}
}
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

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	public partial class WindowLevelTool
	{
		private static readonly SensitivityMap Sensitivity = new SensitivityMap();

		private double CurrentSensitivity
		{
			get
			{
				if (SelectedVoiLutProvider != null)
				{
					var voiLutManager = SelectedVoiLutProvider.VoiLutManager;
					if (voiLutManager != null && voiLutManager.VoiLut != null)
					{
						// compute the effective bit depth of the image (that is, the bit depth of values on which the W/L tool operates, rather than that of the raw pixel data)
						// the formula to compute this is: ceil(log2(EFFECTIVE_VALUE_RANGE))
						var effectiveBitDepth = (int) Math.Ceiling(Math.Log(Math.Abs(voiLutManager.VoiLut.MaxOutputValue - voiLutManager.VoiLut.MinOutputValue), 2));
						return Sensitivity[effectiveBitDepth];
					}
				}
				return 10;
			}
		}

		private class SensitivityMap
		{
			private static readonly double[] _increment;

			static SensitivityMap()
			{
				_increment = new double[16];
				_increment[0] = 0.05;
				_increment[1] = 0.1;
				_increment[2] = 0.5;
				_increment[3] = 0.5;
				_increment[4] = 1;
				_increment[5] = 1;
				_increment[6] = 1;
				_increment[7] = 1;
				_increment[8] = 5;
				_increment[9] = 5;
				_increment[10] = 5;
				_increment[11] = 5;
				_increment[12] = 10;
				_increment[13] = 10;
				_increment[14] = 10;
				_increment[15] = 10;
			}

			public double this[int bitDepth]
			{
				get
				{
					if (bitDepth > 16)
						bitDepth = 16;
					if (bitDepth < 1)
						bitDepth = 1;
					return _increment[bitDepth - 1];
				}
			}
		}
	}
}
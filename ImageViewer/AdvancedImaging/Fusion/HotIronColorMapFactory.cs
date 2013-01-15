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

using System.Drawing;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	public class HotIronColorMapFactory : IColorMapFactory
	{
		public static readonly string ColorMapName = "HOT_IRON";
		private const string _colorMapDescription = "Hot Iron";

		public string Name
		{
			get { return ColorMapName; }
		}

		public string Description
		{
			get { return _colorMapDescription; }
		}

		public IColorMap Create()
		{
			return new DicomPaletteColorMap(_colorMapDescription, PaletteColorLut.HotIron);
		}

		private class DicomPaletteColorMap : ColorMap
		{
			private readonly PaletteColorLut _paletteColorLut;
			private readonly string _description;

			public DicomPaletteColorMap(PaletteColorLut paletteColorLut)
				: this(string.Empty, paletteColorLut) {}

			public DicomPaletteColorMap(string description, PaletteColorLut paletteColorLut)
			{
				_description = description;
				_paletteColorLut = paletteColorLut;
			}

			protected override void Create()
			{
				int valueIndex = 0;
				int valueFullScale = this.Length - 1;
				int min = MinInputValue;
				int max = MaxInputValue;

				int countColors = _paletteColorLut.Data.Length;
				for (int i = min; i <= max; i++)
				{
					// scale the input value index to the range of colors available
					float scaledColorIndex = (countColors - 1f)*(valueIndex++)/valueFullScale;

					// the fractional part of the scaled index is the interpolation step between two adjacent colors
					int colorIndex = (int) scaledColorIndex;
					float interpolationStep = scaledColorIndex - colorIndex;

					// if the index is within the LUT's bounds, interpolate between the adjacent colors
					if (colorIndex + 1 < countColors)
					{
						Color startColor = _paletteColorLut.Data[colorIndex];
						Color endColor = _paletteColorLut.Data[colorIndex + 1];
						Color color = Color.FromArgb(255,
						                             Interpolate(interpolationStep, startColor.R, endColor.R),
						                             Interpolate(interpolationStep, startColor.G, endColor.G),
						                             Interpolate(interpolationStep, startColor.B, endColor.B));
						this[i] = color.ToArgb();
					}
					else
					{
						// otherwise, just return the last value (should only happen once, i.e. valueIndex==valueFullScale)
						this[i] = _paletteColorLut.Data[countColors - 1].ToArgb();
					}
				}
			}

			private static int Interpolate(float value, byte low, byte high)
			{
				return 0x000000FF & (int) (value*(high - low) + low);
			}

			public override string GetKey()
			{
				return string.Format("{0}[{1}]", base.GetKey(), _paletteColorLut.SourceSopInstanceUid);
			}

			public override string GetDescription()
			{
				return _description;
			}
		}
	}
}
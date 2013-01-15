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
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	public class DynamicTe : IMemorable
	{
		public class DynamicTeMemento : IEquatable<DynamicTeMemento>
		{
			private double _te;
			private int _threshold;
			private int _opacity;

			public DynamicTeMemento(double te, int threshold, int opacity)
			{
				_te = te;
				_threshold = threshold;
				_opacity = opacity;
			}

			public double Te
			{
				get { return _te; }
			}

			public int Threshold
			{
				get { return _threshold; }
			}

			public int Opacity
			{
				get { return _opacity; }
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;

				return this.Equals(obj as DynamicTeMemento);
			}

			#region IEquatable<DynamicTeMemento> Members

			public bool Equals(DynamicTeMemento other)
			{
				if (other == null)
					return false;

				return Te == other.Te && Threshold == other.Threshold && Opacity == other.Opacity;
			}

			#endregion
		}

		private GrayscaleImageGraphic _imageGraphic;
		private ColorImageGraphic _probabilityOverlay;
		private double _te;
		private byte[] _protonDensityMap;
		private byte[] _t2Map;
		private byte[] _probabilityMap;
		private int _threshold;
		private int _opacity;

		public DynamicTe(
			GrayscaleImageGraphic imageGraphic,
			byte[] protonDensityMap,
			byte[] t2Map,
			ColorImageGraphic probabilityOverlay,
			byte[] probabilityMap)
		{
			_imageGraphic = imageGraphic;
			_protonDensityMap = protonDensityMap;
			_t2Map = t2Map;

			_probabilityOverlay = probabilityOverlay;
			_probabilityMap = probabilityMap;
		}

		internal byte[] ProtonDensityMap
		{
			get { return _protonDensityMap; }
		}

		internal byte[] T2Map
		{
			get { return _t2Map; }
		}

		internal byte[] ProbabilityMap
		{
			get { return _probabilityMap; }
		}

		public double Te
		{
			get { return _te; }
			set
			{
				if (value != _te)
				{
					if (value < 0.0)
						_te = 0.0;
					else
						_te = value;

					CalculatePixelData();
				}
			}
		}

		public bool ProbabilityMapVisible
		{
			get { return _probabilityOverlay.Visible; }
			set { _probabilityOverlay.Visible = value; }
		}

		public void ApplyProbabilityThreshold(int threshold, int opacity)
		{
			if (!this.ProbabilityMapVisible)
				return;

			_threshold = threshold;
			_opacity = opacity;

			GrayscalePixelData probabilityPixelData = new GrayscalePixelData(
				_imageGraphic.Rows,
				_imageGraphic.Columns,
				_imageGraphic.BitsPerPixel,
				_imageGraphic.BitsStored,
				_imageGraphic.HighBit,
				_imageGraphic.IsSigned,
				_probabilityMap);

			probabilityPixelData.ForEachPixel(
				delegate(int i, int x, int y, int pixelIndex)
					{
						if (probabilityPixelData.GetPixel(pixelIndex) < threshold)
							_probabilityOverlay.PixelData.SetPixel(pixelIndex, Color.FromArgb(opacity, Color.Red));
						else
							_probabilityOverlay.PixelData.SetPixel(pixelIndex, Color.Empty);
					});
		}

		private unsafe void CalculatePixelData()
		{
			byte[] pixelData = _imageGraphic.PixelData.Raw;
			int sizeInPixels = _imageGraphic.SizeInPixels;

			fixed (byte* pPixelData = pixelData)
			{
				fixed (byte* pProtonDensityMap = _protonDensityMap)
				{
					fixed (byte* pT2Map = _t2Map)
					{
						if (_imageGraphic.IsSigned)
						{
							for (int i = 0; i < sizeInPixels; i++)
							{
								short* pShortPixelData = (short*)pPixelData;
								short* pShortProtonDensityMap = (short*)pProtonDensityMap;
								short* pShortT2Map = (short*)pT2Map;

								short pixelValue = (short)(pShortProtonDensityMap[i] * Math.Exp(-this.Te / pShortT2Map[i]));
								pShortPixelData[i] = pixelValue;
							}
						}
						else
						{
							for (int i = 0; i < sizeInPixels; i++)
							{
								ushort* pShortPixelData = (ushort*)pPixelData;
								ushort* pShortProtonDensityMap = (ushort*)pProtonDensityMap;
								ushort* pShortT2Map = (ushort*)pT2Map;

								ushort pixelValue = (ushort)(pShortProtonDensityMap[i] * Math.Exp(-this.Te / pShortT2Map[i]));
								pShortPixelData[i] = pixelValue;
							}
						}
					}
				}
			}
		}

		#region IMemorable Members

		public object CreateMemento()
		{
			return new DynamicTeMemento(this.Te, _threshold, _opacity);
		}

		public void SetMemento(object memento)
		{
			DynamicTeMemento teMemento = (DynamicTeMemento) memento;

			this.Te = teMemento.Te;
			ApplyProbabilityThreshold(teMemento.Threshold, teMemento.Opacity);
			_imageGraphic.Draw();
		}

		#endregion
	}
}

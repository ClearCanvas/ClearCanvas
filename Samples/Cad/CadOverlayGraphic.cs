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

using ClearCanvas.ImageViewer.Graphics;
using System.Drawing;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.Samples.Cad
{
	public class CadOverlayGraphic : ColorImageGraphic, IMemorable
	{
		public class CadMemento : IMemento
		{
			private int _threshold;
			private int _opacity;

			public CadMemento(int threshold, int opacity)
			{
				_threshold = threshold;
				_opacity = opacity;
			}

			public int Threshold
			{
				get { return _threshold; }
			}

			public int Opacity
			{
				get { return _opacity; }
			}
		}

		private int _threshold;
		private int _opacity;
		private GrayscaleImageGraphic _image;

		public CadOverlayGraphic(GrayscaleImageGraphic image) 
			: base(image.Rows, image.Columns)
		{
			_image = image;		
		}

		public int Threshold
		{
			get { return _threshold; }
			set { _threshold = value; }
		}

		public int Opacity
		{
			get { return _opacity; }
			set { _opacity = value; }
		}

		private Color OverlayColor
		{
			get
			{
				int alpha = (int)(this.Opacity / 100.0f * 255);
				return Color.FromArgb(alpha, Color.Red);
			}
		}

		public void Analyze()
		{
			this.PixelData.ForEachPixel(
				delegate(int i, int x, int y, int pixelIndex)
					{
						int pixelValue = _image.PixelData.GetPixel(pixelIndex);
						int hounsfieldValue = _image.ModalityLut[pixelValue];
						if (hounsfieldValue > this.Threshold)
							this.PixelData.SetPixel(pixelIndex, this.OverlayColor);
						else
							this.PixelData.SetPixel(pixelIndex, Color.Empty);
					});

			Draw();
		}

		#region IMemorable Members

		public IMemento CreateMemento()
		{
			return new CadMemento(_threshold, _opacity);
		}

		public void SetMemento(IMemento memento)
		{
			CadMemento cadMemento = memento as CadMemento;
			Platform.CheckForInvalidCast(cadMemento, "memento", "CadMemento");

			_threshold = cadMemento.Threshold;
			_opacity = cadMemento.Opacity;
			Analyze();
		}

		#endregion
	}
}
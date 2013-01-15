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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	//TODO: might be useful in core code some day
	internal sealed class GrayscaleIconSet : IconSet
	{
		public GrayscaleIconSet(IconSet source)
			: base(source.SmallIcon, source.MediumIcon, source.LargeIcon) {}

		public override Image CreateIcon(IconSize iconSize, IResourceResolver resourceResolver)
		{
			//TODO: make unsafe. Not enabling unsafe code just for this, though.
			var bitmap = (Bitmap) base.CreateIcon(iconSize, resourceResolver);
			for (int x = 0; x < bitmap.Width; ++x)
			{
				for (int y = 0; y < bitmap.Height; ++y)
				{
					var pixel = bitmap.GetPixel(x, y);
					int gray = (int) (pixel.R*0.3f + pixel.G*0.59F + pixel.B*0.11f);
					if (gray > 255)
						gray = 255;

					bitmap.SetPixel(x, y, Color.FromArgb(pixel.A, gray, gray, gray));
				}
			}
			return bitmap;
		}

		public override string GetIconKey(IconSize iconSize, IResourceResolver resourceResolver)
		{
			return base.GetIconKey(iconSize, resourceResolver) + "_ConvertedToGrayscale";
		}
	}
}
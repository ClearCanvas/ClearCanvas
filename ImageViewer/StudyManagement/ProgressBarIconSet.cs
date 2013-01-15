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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	enum ProgressBarColor
	{
		Green,
		Yellow,
		Red
	}

	class ProgressBarIconSet : IconSet
	{
		private readonly Size _dimensions;
		private readonly decimal _percent;
		private readonly ProgressBarColor _color;

		public ProgressBarIconSet(string name, Size dimensions, decimal percent, ProgressBarColor color)
			: base(name)
		{
			_dimensions = dimensions;
			_percent = percent;
			_color = color;
		}

		public override Image CreateIcon(IconSize iconSize, IResourceResolver resourceResolver)
		{
			var bitmap = new Bitmap(_dimensions.Width, _dimensions.Height);
			using(var g = System.Drawing.Graphics.FromImage(bitmap))
			{
				g.FillRectangle(Brushes.White, 0, 0, _dimensions.Width - 1, _dimensions.Height - 1);
				g.DrawRectangle(Pens.DarkGray, 0, 0, _dimensions.Width - 1, _dimensions.Height - 1);
				g.FillRectangle(GetBrush(), 1, 1,
					Math.Min((int)(_dimensions.Width*_percent/100), _dimensions.Width - 2),
					_dimensions.Height - 2);
			}
			return bitmap;
		}

		private Brush GetBrush()
		{
			switch (_color)
			{
				case ProgressBarColor.Green:
					return Brushes.LimeGreen;
				case ProgressBarColor.Yellow:
					return Brushes.Yellow;
				case ProgressBarColor.Red:
					return Brushes.Red;
			}
			throw new NotImplementedException();
		}
	}
}

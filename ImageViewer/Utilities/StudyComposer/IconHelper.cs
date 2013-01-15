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
using ClearCanvas.Common;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	internal sealed class IconHelper
	{
		public IconHelper() : this(new Size(100, 100), Color.DarkGray, 1) {}
		public IconHelper(int iconWidth, int iconHeight) : this(new Size(iconWidth, iconHeight)) {}
		public IconHelper(Size iconSize) : this(iconSize, Color.DarkGray, 1) {}
		public IconHelper(Color borderColor, int borderWidth) : this(new Size(100, 100), borderColor, borderWidth) {}
		public IconHelper(int iconWidth, int iconHeight, Color borderColor, int borderWidth) : this(new Size(iconWidth, iconHeight), borderColor, borderWidth) {}

		public IconHelper(Size iconSize, Color borderColor, int borderWidth)
		{
			_iconSize = iconSize;
			_borderColor = borderColor;
			_borderWidth = borderWidth;
		}

		#region Properties

		private Color _backgroundColor = Color.Black;

		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set { _backgroundColor = value; }
		}

		private Color _borderColor;

		public Color BorderColor
		{
			get { return _borderColor; }
			set { _borderColor = value; }
		}

		private int _borderWidth;

		public int BorderWidth
		{
			get { return _borderWidth; }
			set { _borderWidth = value; }
		}

		private Size _iconSize;

		public Size IconSize
		{
			get { return _iconSize; }
			set { _iconSize = value; }
		}

		private int _stackSize = 3;

		public int StackSize
		{
			get { return _stackSize; }
			set { _stackSize = value; }
		}

		private Size _stackOffset = new Size(10, 10);

		public Size StackOffset
		{
			get { return _stackOffset; }
			set { _stackOffset = value; }
		}

		public int StackHorizontalOffset
		{
			get { return _stackOffset.Width; }
			set { _stackOffset = new Size(value, _stackOffset.Height); }
		}

		public int StackVerticalOffset
		{
			get { return _stackOffset.Height; }
			set { _stackOffset = new Size(_stackOffset.Width, value); }
		}

		#endregion

		public Bitmap CreateImageIcon(Image image)
		{
			return GetImageIcon(delegate(int width, int height)
            	{
            		if (image == null)
            			return null;

					try {
						return new Bitmap(image, width, height);
					} catch (InvalidOperationException) {
						using (Image i2 = (Image)image.Clone()) {
							return new Bitmap(i2, width, height);
						}
					}
            	});
		}

		public Bitmap CreateImageIcon(IPresentationImage image)
		{
			Platform.CheckForNullReference(image, "image");
			return GetImageIcon(delegate(int width, int height) { return image.DrawToBitmap(width, height); });
		}

		public Bitmap CreateStackIcon(Image image)
		{
			return GetStackIcon(delegate(int width, int height)
            	{
            		if (image == null)
            			return null;

            		try
            		{
            			return new Bitmap(image, width, height);
            		}
            		catch (InvalidOperationException)
            		{
            			using (Image i2 = (Image) image.Clone())
            			{
            				return new Bitmap(i2, width, height);
            			}
            		}
            	});
		}

		public Bitmap CreateStackIcon(IDisplaySet displaySet)
		{
			Platform.CheckForNullReference(displaySet, "displaySet");
			IPresentationImage image = GetMiddlePresentationImage(displaySet);
			return GetStackIcon(delegate(int width, int height) { return image.DrawToBitmap(width, height); });
		}

		public Bitmap CreateStackIcon(IImageSet imageSet)
		{
			Platform.CheckForNullReference(imageSet, "imageSet");
			IPresentationImage image = GetMiddlePresentationImage(imageSet);
			return GetStackIcon(delegate(int width, int height) { return image.DrawToBitmap(width, height); });
		}

		private delegate Bitmap BitmapCreatorDelegate(int width, int height);

		private Bitmap GetImageIcon(BitmapCreatorDelegate creator)
		{
			Bitmap icon = new Bitmap(_iconSize.Width, _iconSize.Height);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(icon))
			{
				using (Bitmap bmp = creator(_iconSize.Width, _iconSize.Height))
				{
					if (bmp != null)
						g.DrawImage(bmp, Point.Empty);
					else
						g.FillRectangle(new SolidBrush(_backgroundColor), new Rectangle(Point.Empty, icon.Size));
					DrawBorder(icon, _borderColor, _borderWidth);
				}
			}
			return icon;
		}

		private Bitmap GetStackIcon(BitmapCreatorDelegate creator)
		{
			Bitmap icon = new Bitmap(_iconSize.Width, _iconSize.Height);
			int subWidth = _iconSize.Width - (_stackSize - 1)*Math.Abs(_stackOffset.Width);
			int subHeight = _iconSize.Height - (_stackSize - 1)*Math.Abs(_stackOffset.Height);

			int offsetH = 0;
			if (_stackOffset.Width < 0)
			{
				offsetH = (_stackSize - 1)*Math.Abs(_stackOffset.Width);
			}

			int offsetV = 0;
			if (_stackOffset.Height < 0)
			{
				offsetV = (_stackSize - 1)*Math.Abs(_stackOffset.Height);
			}

			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(icon))
			{
				using (Bitmap bmp = creator(subWidth, subHeight))
				{
					for (int n = _stackSize - 1; n >= 0; n--)
					{
						Point point = new Point(n*_stackOffset.Width + offsetH, n*_stackOffset.Height + offsetV);
						if (bmp != null)
							g.DrawImage(bmp, point);
						else
							g.FillRectangle(new SolidBrush(_backgroundColor), point.X, point.Y, subWidth, subHeight);
						DrawBorder(icon, new Rectangle(point, new Size(subWidth, subHeight)), _borderColor, _borderWidth);
					}
				}
			}
			return icon;
		}

		private static void DrawBorder(Image icon, Color penColor, int penWidth)
		{
			DrawBorder(icon, new Rectangle(Point.Empty, icon.Size), penColor, penWidth);
		}

		private static void DrawBorder(Image icon, Rectangle area, Color penColor, int penWidth)
		{
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(icon))
			{
				using (Pen pen = new Pen(penColor))
				{
					for (int n = 0; n < penWidth; n++)
					{
						g.DrawRectangle(pen, area.X + n, area.Y + n, area.Width - 2*n - 1, area.Height - 2*n - 1);
					}
				}
			}
		}

		private static IPresentationImage GetMiddlePresentationImage(IDisplaySet displaySet)
		{
			if (displaySet.PresentationImages.Count == 0)
				throw new ArgumentException("The display set must contain at least one image.");
			return displaySet.PresentationImages[(displaySet.PresentationImages.Count - 1)/2];
		}

		private static IPresentationImage GetMiddlePresentationImage(IImageSet imageSet)
		{
			if (imageSet.DisplaySets.Count == 0)
				throw new ArgumentException("The image set must contain at least one display set.");
			return GetMiddlePresentationImage(imageSet.DisplaySets[0]);
		}
	}
}
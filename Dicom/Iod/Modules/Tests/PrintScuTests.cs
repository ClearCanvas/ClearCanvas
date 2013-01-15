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

#if UNIT_TESTS

using System;
using ClearCanvas.Dicom.Network.Scu;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Iod.Modules.Tests
{
	[TestFixture]
	public class FilmBoxTests
	{
		[Test]
		public void FilmDPI_Test()
		{
			var filmBox = new PrintScu.FilmBox(300, 600);
			Assert.AreEqual(filmBox.RequestedResolutionId, RequestedResolution.None);

			// Default DPI is used when RequestedResolution.None.
			Assert.AreEqual(filmBox.FilmDPI, 300);

			filmBox.RequestedResolutionId = RequestedResolution.High;
			Assert.AreEqual(filmBox.FilmDPI, 600);

			filmBox.RequestedResolutionId = RequestedResolution.Standard;
			Assert.AreEqual(filmBox.FilmDPI, 300);
		}

		[Test]
		public void SizeInPixels_Portrait_Test()
		{
			var filmBox = new PrintScu.FilmBox(300, 600)
			{
				FilmSizeId = FilmSize.Dimension_8in_x_10in,
				FilmOrientation = FilmOrientation.Portrait
			};

			Assert.AreEqual(filmBox.FilmOrientation, FilmOrientation.Portrait);
			Assert.AreEqual(filmBox.SizeInPixels.Width, 8 * 300);
			Assert.AreEqual(filmBox.SizeInPixels.Height, 10 * 300);

			// Portrait orientation is used, even if FilmOrientation.None.
			filmBox.FilmOrientation = FilmOrientation.None;
			Assert.AreEqual(filmBox.FilmOrientation, FilmOrientation.None);
			Assert.AreEqual(filmBox.SizeInPixels.Width, 8 * 300);
			Assert.AreEqual(filmBox.SizeInPixels.Height, 10 * 300);
		}

		[Test]
		public void SizeInPixels_Landscape_Test()
		{
			var filmBox = new PrintScu.FilmBox(300, 600)
			{
				FilmSizeId = FilmSize.Dimension_8in_x_10in,
				FilmOrientation = FilmOrientation.Landscape
			};

			Assert.AreEqual(filmBox.FilmOrientation, FilmOrientation.Landscape);
			Assert.AreEqual(filmBox.SizeInPixels.Width, 10 * 300);
			Assert.AreEqual(filmBox.SizeInPixels.Height, 8 * 300);
		}
	}

	[TestFixture]
	public class ImageBoxTests
	{
		private const int _floatingPointDigits = 4;

		[Test]
		public void Standard_Format_Portrait_Test()
		{
			var filmBox = new PrintScu.FilmBox(300, 600)
				{
					FilmOrientation = FilmOrientation.Portrait,
					FilmSizeId = FilmSize.Dimension_8in_x_10in,
					ImageDisplayFormat = ImageDisplayFormat.Standard_2x4
				};

			var imageBox = new PrintScu.ImageBox(filmBox, null);
			var filmBoxSize = filmBox.SizeInPixels;

			// Layout is 2x4, meaning 2 columns, 4 rows.
			// ImageBoxes are ordered top->bottom, left->right
			imageBox.ImageBoxPosition = 1;
			var imageBoxSize = imageBox.SizeInPixels;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 4);
			Assert.AreEqual(Math.Round(imageBox.PhysicalWidth, _floatingPointDigits), Math.Round(8 * LengthInMillimeter.Inch / 2, _floatingPointDigits));

			imageBox.ImageBoxPosition = 8;
			imageBoxSize = imageBox.SizeInPixels;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 4);
			Assert.AreEqual(Math.Round(imageBox.PhysicalWidth, _floatingPointDigits), Math.Round(8 * LengthInMillimeter.Inch / 2, _floatingPointDigits));
		}

		[Test]
		public void Standard_Format_Landscape_Test()
		{
			var filmBox = new PrintScu.FilmBox(300, 600)
			{
				FilmOrientation = FilmOrientation.Landscape,
				FilmSizeId = FilmSize.Dimension_8in_x_10in,
				ImageDisplayFormat = ImageDisplayFormat.Standard_2x4
			};

			var imageBox = new PrintScu.ImageBox(filmBox, null);
			var filmBoxSize = filmBox.SizeInPixels;

			// Layout is 2x4, meaning 2 columns, 4 rows.
			// ImageBoxes are ordered top->bottom, left->right
			imageBox.ImageBoxPosition = 1;
			var imageBoxSize = imageBox.SizeInPixels;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 4);
			Assert.AreEqual(Math.Round(imageBox.PhysicalWidth, _floatingPointDigits), Math.Round(10 * LengthInMillimeter.Inch / 2, _floatingPointDigits));

			imageBox.ImageBoxPosition = 8;
			imageBoxSize = imageBox.SizeInPixels;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 4);
			Assert.AreEqual(Math.Round(imageBox.PhysicalWidth, _floatingPointDigits), Math.Round(10 * LengthInMillimeter.Inch / 2, _floatingPointDigits));
		}

		[Test]
		public void Row_Format_Portrait_Test()
		{
			var filmBox = new PrintScu.FilmBox(300, 600)
				{
					FilmOrientation = FilmOrientation.Portrait,
					FilmSizeId = FilmSize.Dimension_8in_x_10in,
					ImageDisplayFormat = ImageDisplayFormat.Row_1_2
				};

			var imageBox = new PrintScu.ImageBox(filmBox, null);
			var filmBoxSize = filmBox.SizeInPixels;

			// Layout is Row 1,2, meaning 1 column in top row and 2 columns in bottom row
			imageBox.ImageBoxPosition = 1;
			var imageBoxSize = imageBox.SizeInPixels;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);
			Assert.AreEqual(Math.Round(imageBox.PhysicalWidth, _floatingPointDigits), Math.Round(8 * LengthInMillimeter.Inch, _floatingPointDigits));

			imageBox.ImageBoxPosition = 2;
			imageBoxSize = imageBox.SizeInPixels;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);
			Assert.AreEqual(Math.Round(imageBox.PhysicalWidth, _floatingPointDigits), Math.Round(8 * LengthInMillimeter.Inch / 2, _floatingPointDigits));

			imageBox.ImageBoxPosition = 3;
			imageBoxSize = imageBox.SizeInPixels;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);
			Assert.AreEqual(Math.Round(imageBox.PhysicalWidth, _floatingPointDigits), Math.Round(8 * LengthInMillimeter.Inch / 2, _floatingPointDigits));
		}

		[Test]
		public void Row_Format_Landscape_Test()
		{
			var filmBox = new PrintScu.FilmBox(300, 600)
			{
				FilmOrientation = FilmOrientation.Landscape,
				FilmSizeId = FilmSize.Dimension_8in_x_10in,
				ImageDisplayFormat = ImageDisplayFormat.Row_1_2
			};

			var imageBox = new PrintScu.ImageBox(filmBox, null);
			var filmBoxSize = filmBox.SizeInPixels;

			// Layout is Row 1,2, meaning 1 column in top row and 2 columns in bottom row
			imageBox.ImageBoxPosition = 1;
			var imageBoxSize = imageBox.SizeInPixels;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);
			Assert.AreEqual(Math.Round(imageBox.PhysicalWidth, _floatingPointDigits), Math.Round(10 * LengthInMillimeter.Inch, _floatingPointDigits));

			imageBox.ImageBoxPosition = 2;
			imageBoxSize = imageBox.SizeInPixels;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);
			Assert.AreEqual(Math.Round(imageBox.PhysicalWidth, _floatingPointDigits), Math.Round(10 * LengthInMillimeter.Inch / 2, _floatingPointDigits));

			imageBox.ImageBoxPosition = 3;
			imageBoxSize = imageBox.SizeInPixels;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);
			Assert.AreEqual(Math.Round(imageBox.PhysicalWidth, _floatingPointDigits), Math.Round(10 * LengthInMillimeter.Inch / 2, _floatingPointDigits));
		}

		[Test]
		public void Column_Format_Portrait_Test()
		{
			var filmBox = new PrintScu.FilmBox(300, 600)
				{
					FilmOrientation = FilmOrientation.Portrait,
					FilmSizeId = FilmSize.Dimension_8in_x_10in,
					ImageDisplayFormat = ImageDisplayFormat.COL_1_2
				};

			var imageBox = new PrintScu.ImageBox(filmBox, null);
			var filmBoxSize = filmBox.SizeInPixels;

			// Layout is Column 1,2, meaning 1 row in the left column and 2 rows in the right column
			imageBox.ImageBoxPosition = 1;
			var imageBoxSize = imageBox.SizeInPixels;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height);
			Assert.AreEqual(Math.Round(imageBox.PhysicalWidth, _floatingPointDigits), Math.Round(8 * LengthInMillimeter.Inch / 2, _floatingPointDigits));

			imageBox.ImageBoxPosition = 2;
			imageBoxSize = imageBox.SizeInPixels;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);
			Assert.AreEqual(Math.Round(imageBox.PhysicalWidth, _floatingPointDigits), Math.Round(8 * LengthInMillimeter.Inch / 2, _floatingPointDigits));

			imageBox.ImageBoxPosition = 3;
			imageBoxSize = imageBox.SizeInPixels;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);
			Assert.AreEqual(Math.Round(imageBox.PhysicalWidth, _floatingPointDigits), Math.Round(8 * LengthInMillimeter.Inch / 2, _floatingPointDigits));
		}

		[Test]
		public void Column_Format_Landscape_Test()
		{
			var filmBox = new PrintScu.FilmBox(300, 600)
			{
				FilmOrientation = FilmOrientation.Landscape,
				FilmSizeId = FilmSize.Dimension_8in_x_10in,
				ImageDisplayFormat = ImageDisplayFormat.COL_1_2
			};

			var imageBox = new PrintScu.ImageBox(filmBox, null);
			var filmBoxSize = filmBox.SizeInPixels;

			// Layout is Column 1,2, meaning 1 row in the left column and 2 rows in the right column
			imageBox.ImageBoxPosition = 1;
			var imageBoxSize = imageBox.SizeInPixels;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height);
			Assert.AreEqual(Math.Round(imageBox.PhysicalWidth, _floatingPointDigits), Math.Round(10 * LengthInMillimeter.Inch / 2, _floatingPointDigits));

			imageBox.ImageBoxPosition = 2;
			imageBoxSize = imageBox.SizeInPixels;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);
			Assert.AreEqual(Math.Round(imageBox.PhysicalWidth, _floatingPointDigits), Math.Round(10 * LengthInMillimeter.Inch / 2, _floatingPointDigits));

			imageBox.ImageBoxPosition = 3;
			imageBoxSize = imageBox.SizeInPixels;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);
			Assert.AreEqual(Math.Round(imageBox.PhysicalWidth, _floatingPointDigits), Math.Round(10 * LengthInMillimeter.Inch / 2, _floatingPointDigits));
		}
	}
}

#endif
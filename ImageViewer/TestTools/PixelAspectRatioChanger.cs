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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.TestTools
{
	public class PixelAspectRatioChanger
	{
		public PixelAspectRatioChanger()
		{
			NewAspectRatio = new PixelAspectRatio(0, 0);
		}

		public bool RemoveCalibration { get; set; }
		public bool IncreasePixelDimensions { get; set; }
		public PixelAspectRatio NewAspectRatio { get; set; }

		private PixelDataInfo OldInfo { get; set; }
		private PixelDataInfo NewInfo { get; set; }

		public void ChangeAspectRatio(DicomFile file)
		{
			if (NewAspectRatio.IsNull || FloatComparer.AreEqual(NewAspectRatio.Value, 1))
				throw new InvalidOperationException("Invalid new aspect ratio");

			if (file.TransferSyntax.Encapsulated)
				file.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian);

			OldInfo = new PixelDataInfo(file.DataSet);
			if (!OldInfo.IsSquare)
				throw new ArgumentException("Pixels are already non-square.");

			NewInfo = OldInfo.Clone();
			NewInfo.AspectRatio = NewAspectRatio;

			if (IncreasePixelDimensions)
			{
				if (NewAspectRatio.Value < 1)
					NewInfo.Rows = (int)(OldInfo.Rows / NewAspectRatio.Value + 0.5);
				else
					NewInfo.Columns = (int)(OldInfo.Columns * NewAspectRatio.Value + 0.5);
			}
			else
			{
				if (NewAspectRatio.Value < 1)
					NewInfo.Columns = (int)(OldInfo.Columns * NewAspectRatio.Value + 0.5);
				else
					NewInfo.Rows = (int)(OldInfo.Rows / NewAspectRatio.Value + 0.5);
			}

			float rowScale = OldInfo.Rows / (float)NewInfo.Rows;
			float colScale = OldInfo.Columns / (float)NewInfo.Columns;

			if (RemoveCalibration)
			{
				NewInfo.PixelSpacing = new PixelSpacing(0, 0);
				NewInfo.ImagerPixelSpacing = new PixelSpacing(0, 0);
			}
			else
			{
				NewInfo.PixelSpacing = new PixelSpacing(NewInfo.PixelSpacing.Row * rowScale, NewInfo.PixelSpacing.Column * colScale);
				NewInfo.ImagerPixelSpacing = new PixelSpacing(NewInfo.ImagerPixelSpacing.Row * rowScale, NewInfo.ImagerPixelSpacing.Column * colScale);
			}

			ValidateNewInfo();

			NewInfo.SeriesDescription = (OldInfo.SeriesDescription ?? "") + String.Format(" ({0}:{1}, dim/cal={2}/{3})",
						NewAspectRatio.Row, NewAspectRatio.Column,
						IncreasePixelDimensions ? "y" : "n",
						NewInfo.IsCalibrated ? "y" : "n");

			NewInfo.PlanarConfiguration = 0;

			PixelData oldPixelData = OldInfo.GetPixelData();
			PixelData newPixelData = NewInfo.GetPixelData();

			for (int row = 0; row < NewInfo.Rows; ++row)
			{
				for (int column = 0; column < NewInfo.Columns; ++column)
				{
					var sourcePoint = new PointF(column * colScale, row * rowScale);
					int interpolated = PerformBilinearInterpolationAt(oldPixelData, sourcePoint);
					newPixelData.SetPixel(column, row, interpolated);
				}
			}

			NewInfo.SetPixelData(newPixelData);
			NewInfo.UpdateDataSet(file.DataSet);
		}

		private void ValidateNewInfo()
		{
			SizeF? dimensions = NewInfo.GetRealDimensions();
			if (dimensions.HasValue)
			{
				SizeF? oldDimensions = OldInfo.GetRealDimensions();
				if (Math.Abs(dimensions.Value.Width - oldDimensions.Value.Width) > OldInfo.GetPixelSpacing().Column || 
						Math.Abs(dimensions.Value.Height - oldDimensions.Value.Height) > OldInfo.GetPixelSpacing().Row)
					throw new ApplicationException("Inconsistent real size");
			}
			else
			{
				SizeF effDimensions = NewInfo.GetEffectiveDimensions();
				SizeF oldEffDimensions = OldInfo.GetEffectiveDimensions();

				if (IncreasePixelDimensions)
				{
					if (NewInfo.AspectRatio.Value >= 1)
						oldEffDimensions = new SizeF(oldEffDimensions.Width * NewInfo.AspectRatio.Value, oldEffDimensions.Height * NewInfo.AspectRatio.Value);
					else
						oldEffDimensions = new SizeF(oldEffDimensions.Width / NewInfo.AspectRatio.Value, oldEffDimensions.Height / NewInfo.AspectRatio.Value);
				}

				if (Math.Abs(effDimensions.Width - oldEffDimensions.Width) >= NewInfo.AspectRatio.Column ||
						Math.Abs(effDimensions.Height - oldEffDimensions.Height) >= NewInfo.AspectRatio.Row)
					throw new ApplicationException("Effective sizes not consistent");
			}
		}

		private static int PerformBilinearInterpolationAt(PixelData pixelData, PointF point00)
		{
			if (point00.Y < 0)
				point00.Y = 0;
			if (point00.X < 0)
				point00.X = 0;

			if (point00.X > (pixelData.Columns - 1.001F))
				point00.X = (pixelData.Columns - 1.001F);
			if (point00.Y > (pixelData.Rows - 1.001F))
				point00.Y = (pixelData.Rows - 1.001F);

			var srcPointInt00 = new Point((int)point00.X, (int)point00.Y);

			var arrayOfValues = new float[2, 2] { { 0, 0 }, { 0, 0 } };

			if (pixelData is ColorPixelData)
			{
				var colorPixelData = (ColorPixelData)pixelData;
				//Just test the R value, the calculation is done in exactly the same way 
				//for G & B, so if it's OK for the R channel it's OK for them too.

				//Get the 4 neighbour pixels for performing bilinear interpolation.
				arrayOfValues[0, 0] = colorPixelData.GetPixelAsColor(srcPointInt00.X, srcPointInt00.Y).R;
				arrayOfValues[0, 1] = colorPixelData.GetPixelAsColor(srcPointInt00.X, srcPointInt00.Y + 1).R;
				arrayOfValues[1, 0] = colorPixelData.GetPixelAsColor(srcPointInt00.X + 1, srcPointInt00.Y).R;
				arrayOfValues[1, 1] = colorPixelData.GetPixelAsColor(srcPointInt00.X + 1, srcPointInt00.Y + 1).R;
				var r = (byte)Interpolate(arrayOfValues, point00, srcPointInt00);

				arrayOfValues[0, 0] = colorPixelData.GetPixelAsColor(srcPointInt00.X, srcPointInt00.Y).G;
				arrayOfValues[0, 1] = colorPixelData.GetPixelAsColor(srcPointInt00.X, srcPointInt00.Y + 1).G;
				arrayOfValues[1, 0] = colorPixelData.GetPixelAsColor(srcPointInt00.X + 1, srcPointInt00.Y).G;
				arrayOfValues[1, 1] = colorPixelData.GetPixelAsColor(srcPointInt00.X + 1, srcPointInt00.Y + 1).G;
				var g = (byte)Interpolate(arrayOfValues, point00, srcPointInt00);

				arrayOfValues[0, 0] = colorPixelData.GetPixelAsColor(srcPointInt00.X, srcPointInt00.Y).B;
				arrayOfValues[0, 1] = colorPixelData.GetPixelAsColor(srcPointInt00.X, srcPointInt00.Y + 1).B;
				arrayOfValues[1, 0] = colorPixelData.GetPixelAsColor(srcPointInt00.X + 1, srcPointInt00.Y).B;
				arrayOfValues[1, 1] = colorPixelData.GetPixelAsColor(srcPointInt00.X + 1, srcPointInt00.Y + 1).B;
				var b = (byte)Interpolate(arrayOfValues, point00, srcPointInt00);

				return Color.FromArgb(255, r, g, b).ToArgb();
			}
			else
			{
				var grayscalePixelData = (GrayscalePixelData)pixelData;
				if (pixelData.BitsAllocated == 16)
				{
					//Get the 4 neighbour pixels for performing bilinear interpolation.
					arrayOfValues[0, 0] = grayscalePixelData.GetPixel(srcPointInt00.X, srcPointInt00.Y);
					arrayOfValues[0, 1] = grayscalePixelData.GetPixel(srcPointInt00.X, srcPointInt00.Y + 1);
					arrayOfValues[1, 0] = grayscalePixelData.GetPixel(srcPointInt00.X + 1, srcPointInt00.Y);
					arrayOfValues[1, 1] = grayscalePixelData.GetPixel(srcPointInt00.X + 1, srcPointInt00.Y + 1);
					return Interpolate(arrayOfValues, point00, srcPointInt00);
				}
				else // if (pixelData.BitsAllocated == 8)
				{
					//Get the 4 neighbour pixels for performing bilinear interpolation.
					arrayOfValues[0, 0] = grayscalePixelData.GetPixel(srcPointInt00.X, srcPointInt00.Y);
					arrayOfValues[0, 1] = grayscalePixelData.GetPixel(srcPointInt00.X, srcPointInt00.Y + 1);
					arrayOfValues[1, 0] = grayscalePixelData.GetPixel(srcPointInt00.X + 1, srcPointInt00.Y);
					arrayOfValues[1, 1] = grayscalePixelData.GetPixel(srcPointInt00.X + 1, srcPointInt00.Y + 1);
					return Interpolate(arrayOfValues, point00, srcPointInt00);
				}
			}
		}

		private static int Interpolate(float[,] values, PointF point, Point point00)
		{
			const float fixedScale = 128;
			const int fixedPrecision = 7;

			//this actually performs the bilinear interpolation within the source image using 4 neighbour pixels.
			float dx = point.X - point00.X;
			float dy = point.Y - point00.Y;

			var dyFixed = (int)(dy * fixedScale);
			var dxFixed = (int)(dx * fixedScale);

			int yInterpolated1 = (((int)(values[0, 0])) << fixedPrecision) +
								 ((dyFixed * ((int)((values[0, 1] - values[0, 0])) << fixedPrecision)) >> fixedPrecision);
			int yInterpolated2 = (((int)(values[1, 0])) << fixedPrecision) +
								 ((dyFixed * ((int)((values[1, 1] - values[1, 0])) << fixedPrecision)) >> fixedPrecision);
			int interpolated = (yInterpolated1 + (((dxFixed) * (yInterpolated2 - yInterpolated1)) >> fixedPrecision)) >>
							   fixedPrecision;

			return interpolated;
		}
	}
}

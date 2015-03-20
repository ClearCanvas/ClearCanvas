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
using System.Linq;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.StudyManagement;
using vtk;

namespace ClearCanvas.ImageViewer.Tools.Volume.VTK
{
	public class VolumePresentationImage : PresentationImage, IAssociatedTissues
	{
		#region Private fields

		private IDisplaySet _displaySet;
		private vtkImageData _vtkImageData;
		private short _minimumPixelValue;

		#endregion

		public VolumePresentationImage(IDisplaySet displaySet)
		{
			_displaySet = displaySet;

			ValidateSliceData();
		}

		#region Public properties

		/// <summary>
		/// Gets the dimensions of the image.
		/// </summary>
		public override Size SceneSize
		{
			get { return new Size(Width, Height); }
		}

		#region IAssociatedTissues Members

		public GraphicCollection TissueLayers
		{
			get { return this.SceneGraph.Graphics; }
		}

		#endregion

		public override IRenderer ImageRenderer
		{
			get
			{
				if (base.ImageRenderer == null)
					base.ImageRenderer = new VolumePresentationImageRenderer();

				return base.ImageRenderer;
			}
		}

		public vtkImageData VtkImageData
		{
			get
			{
				if (_vtkImageData == null)
					_vtkImageData = CreateVolumeImageData();

				return _vtkImageData;
			}
		}

		public short MinimumPixelValue
		{
			get { return _minimumPixelValue; }
		}

		public int Width
		{
			get { return GetImageGraphic().Columns; }
		}

		public int Height
		{
			get { return GetImageGraphic().Rows; }
		}

		public int Depth
		{
			get { return _displaySet.PresentationImages.Count; }
		}

		public int SizeInVoxels
		{
			get { return this.Width*this.Height*this.Depth; }
		}

		public double RescaleSlope
		{
			get { return GetFirstFrame().RescaleSlope; }
		}

		public double RescaleIntercept
		{
			get { return GetFirstFrame().RescaleIntercept; }
		}

		#endregion

		public override IPresentationImage CreateFreshCopy()
		{
			return new VolumePresentationImage(_displaySet);
		}

		private IPresentationImage GetDicomPresentationImage(int i)
		{
			return _displaySet.PresentationImages[i];
		}

		private IPresentationImage GetDicomPresentationImage()
		{
			return GetDicomPresentationImage(0);
		}

		private ImageSop GetImageSop()
		{
			return ((IImageSopProvider) GetDicomPresentationImage()).ImageSop;
		}

		private ImageGraphic GetImageGraphic()
		{
			return ((IImageGraphicProvider) GetDicomPresentationImage()).ImageGraphic;
		}

		private Frame GetFirstFrame()
		{
			return GetImageSop().Frames[1];
		}

		private bool IsDataUnsigned()
		{
			return (GetFirstFrame().PixelRepresentation == 0);
		}

		private vtkImageData CreateVolumeImageData()
		{
			vtkImageData imageData = new vtkImageData();
			imageData.SetDimensions(this.Width, this.Height, this.Depth);
			imageData.SetSpacing(GetFirstFrame().PixelSpacing.Column, GetFirstFrame().PixelSpacing.Row, GetSliceSpacing());
			imageData.AllocateScalars();
			imageData.SetScalarTypeToUnsignedShort();
			imageData.GetPointData().SetScalars(BuildVolumeImageData());

			return imageData;
		}

		private vtkUnsignedShortArray BuildVolumeImageData()
		{
			ushort[] volumeData = new ushort[this.SizeInVoxels];

			int imageIndex = 0;

			if (IsDataUnsigned())
			{
				foreach (var slice in _displaySet.PresentationImages.OfType<IImageGraphicProvider>())
				{
					AddUnsignedSliceToVolume(volumeData, slice, imageIndex);
					imageIndex++;
				}
			}
			else
			{
				FindMinimumPixelValue();

				foreach (var slice in _displaySet.PresentationImages.OfType<IImageGraphicProvider>())
				{
					AddSignedSliceToVolume(volumeData, slice, imageIndex);
					imageIndex++;
				}
			}

			vtkUnsignedShortArray vtkVolumeData = new vtkUnsignedShortArray();
			vtkVolumeData.SetArray(volumeData, new VtkIdType(volumeData.Length), 1);

			return vtkVolumeData;
		}

		private void FindMinimumPixelValue()
		{
			_minimumPixelValue = short.MaxValue;

			foreach (var slice in _displaySet.PresentationImages.OfType<IImageGraphicProvider>())
			{
				byte[] sliceData = slice.ImageGraphic.PixelData.Raw;
				int length = sliceData.Length/2;

				for (int i = 0; i < length; i += 2)
				{
					ushort lowbyte = sliceData[i];
					ushort highbyte = sliceData[i + 1];
					short pixelValue = (short) ((highbyte << 8) | lowbyte);

					if (pixelValue < _minimumPixelValue)
						_minimumPixelValue = pixelValue;
				}
			}
		}

		private void AddUnsignedSliceToVolume(ushort[] volumeData, IImageGraphicProvider slice, int imageIndex)
		{
			byte[] sliceData = slice.ImageGraphic.PixelData.Raw;
			int start = imageIndex*sliceData.Length/2;
			int end = start + sliceData.Length/2;

			int j = 0;

			for (int i = start; i < end; i++)
			{
				ushort lowbyte = sliceData[j];
				ushort highbyte = sliceData[j + 1];
				volumeData[i] = (ushort) ((highbyte << 8) | lowbyte);
				j += 2;
			}
		}

		private void AddSignedSliceToVolume(ushort[] volumeData, IImageGraphicProvider slice, int imageIndex)
		{
			byte[] sliceData = slice.ImageGraphic.PixelData.Raw;
			int start = imageIndex*sliceData.Length/2;
			int end = start + sliceData.Length/2;

			int j = 0;

			for (int i = start; i < end; i++)
			{
				ushort lowbyte = sliceData[j];
				ushort highbyte = sliceData[j + 1];

				short val = (short) ((highbyte << 8) | lowbyte);
				volumeData[i] = (ushort) (val - _minimumPixelValue);

				j += 2;
			}
		}

		private void ValidateSliceData() {}

		private double GetSliceSpacing()
		{
			if (_displaySet.PresentationImages.Count > 1)
			{
				Frame slice1 = ((IImageSopProvider) GetDicomPresentationImage(0)).ImageSop.Frames[1];
				Frame slice2 = ((IImageSopProvider) GetDicomPresentationImage(1)).ImageSop.Frames[1];
				double sliceSpacing = Math.Abs(slice2.ImagePositionPatient.Z - slice1.ImagePositionPatient.Z);

				return sliceSpacing;
			}
			else
			{
				return 0.0d;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _vtkImageData != null)
			{
				_vtkImageData.Dispose();
			}

			base.Dispose(disposing);
		}
	}
}
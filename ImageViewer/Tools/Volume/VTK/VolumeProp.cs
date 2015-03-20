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

using vtk;

namespace ClearCanvas.ImageViewer.Tools.Volume.VTK
{
	internal class VolumeProp : IVtkProp
	{
		private VolumeGraphic _volumeLayer;
		private vtkVolume _vtkVolume;
		private vtkPiecewiseFunction _opacityTransferFunction;
		private vtkColorTransferFunction _colorTransferFunction;

		public VolumeProp(VolumeGraphic volumeLayer)
		{
			_volumeLayer = volumeLayer;
		}

		public vtkProp VtkProp
		{
			get
			{
				if (_vtkVolume == null)
					CreateVolumeRendering();

				return _vtkVolume;
			}
		}

		private void CreateVolumeRendering()
		{
			_opacityTransferFunction = new vtkPiecewiseFunction();
			_opacityTransferFunction.ClampingOff();

			_colorTransferFunction = new vtkColorTransferFunction();
			_colorTransferFunction.SetColorSpaceToRGB();
			_colorTransferFunction.ClampingOff();

			SetOpacityTransferFunction();
			SetColorTransferFunction();

			vtkVolumeProperty volumeProperty = new vtkVolumeProperty();
			volumeProperty.ShadeOn();
			volumeProperty.SetInterpolationTypeToLinear();
			volumeProperty.SetColor(_colorTransferFunction);
			volumeProperty.SetScalarOpacity(_opacityTransferFunction);
			volumeProperty.SetDiffuse(0.7);
			volumeProperty.SetAmbient(0.1);
			volumeProperty.SetSpecular(.3);
			volumeProperty.SetSpecularPower(20);

			//vtkOpenGLVolumeTextureMapper2D volumeMapper = new vtkOpenGLVolumeTextureMapper2D();
			//vtkOpenGLVolumeTextureMapper3D volumeMapper = new vtkOpenGLVolumeTextureMapper3D();
			//volumeMapper.SetPreferredMethodToNVidia();
			//volumeMapper.SetSampleDistance(1.0f);
			//int supported = volumeMapper.IsRenderSupported(volumeProperty);

			vtkFixedPointVolumeRayCastMapper volumeMapper = new vtkFixedPointVolumeRayCastMapper();
			//vtkVolumeRayCastMapper volumeMapper = new vtkVolumeRayCastMapper();
			volumeMapper.SetInput(_volumeLayer.GetImageData());
			////vtkVolumeRayCastCompositeFunction rayCastFunction = new vtkVolumeRayCastCompositeFunction();
			////volumeMapper.SetVolumeRayCastFunction(rayCastFunction);
			//vtkVolumeRayCastIsosurfaceFunction rayCastFunction = new vtkVolumeRayCastIsosurfaceFunction();
			//volumeMapper.SetVolumeRayCastFunction(rayCastFunction);

			_vtkVolume = new vtkVolume();
			_vtkVolume.SetMapper(volumeMapper);
			_vtkVolume.SetProperty(volumeProperty);
		}

		private void SetOpacityTransferFunction()
		{
			_opacityTransferFunction.RemoveAllPoints();
			_opacityTransferFunction.AddPoint(_volumeLayer.GetWindowLeft(), 0.0);
			_opacityTransferFunction.AddPoint(
				_volumeLayer.GetRescaledLevel(),
				(double) _volumeLayer.TissueSettings.Opacity);
			_opacityTransferFunction.AddPoint(_volumeLayer.GetWindowRight(), 0.0);
		}

		private void SetColorTransferFunction()
		{
			_colorTransferFunction.RemoveAllPoints();

			double R = _volumeLayer.TissueSettings.MinimumColor.R/255.0f;
			double G = _volumeLayer.TissueSettings.MinimumColor.G/255.0f;
			double B = _volumeLayer.TissueSettings.MinimumColor.B/255.0f;

			_colorTransferFunction.AddRGBPoint(_volumeLayer.GetWindowLeft(), R, G, B);

			R = _volumeLayer.TissueSettings.MaximumColor.R/255.0f;
			G = _volumeLayer.TissueSettings.MaximumColor.G/255.0f;
			B = _volumeLayer.TissueSettings.MaximumColor.B/255.0f;

			_colorTransferFunction.AddRGBPoint(_volumeLayer.GetWindowRight(), R, G, B);
		}

		public void ApplySetting(string setting)
		{
			if (setting == "Visible")
			{
				if (_volumeLayer.TissueSettings.Visible)
					_vtkVolume.VisibilityOn();
				else
					_vtkVolume.VisibilityOff();
			}
			else
			{
				SetOpacityTransferFunction();

				if (setting != "Opacity")
					SetColorTransferFunction();
			}
			_vtkVolume.Update();
		}
	}
}
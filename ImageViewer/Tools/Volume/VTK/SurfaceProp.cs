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
	internal class SurfaceProp : IVtkProp
	{
		private VolumeGraphic _volumeGraphic;
		private vtkActor _vtkActor;
		private vtkContourFilter _contourFilter;

		public SurfaceProp(VolumeGraphic volumeLayer)
		{
			_volumeGraphic = volumeLayer;
		}

		public vtkProp VtkProp
		{
			get
			{
				if (_vtkActor == null)
					CreateSurfaceRendering();

				return _vtkActor;
			}
		}

		private void CreateSurfaceRendering()
		{
			_contourFilter = new vtkContourFilter();
			_contourFilter.SetInput(_volumeGraphic.GetImageData());
			_contourFilter.SetValue(0, _volumeGraphic.GetRescaledLevel());

			vtkPolyDataNormals normals = new vtkPolyDataNormals();
			normals.SetInputConnection(_contourFilter.GetOutputPort());
			normals.SetFeatureAngle(60.0);

			vtkStripper stripper = new vtkStripper();
			stripper.SetInputConnection(normals.GetOutputPort());

			vtkPolyDataMapper mapper = new vtkPolyDataMapper();
			mapper.SetInputConnection(stripper.GetOutputPort());
			mapper.ScalarVisibilityOff();

			_vtkActor = new vtkActor();
			_vtkActor.SetMapper(mapper);
			_vtkActor.GetProperty().SetSpecular(.3);
			_vtkActor.GetProperty().SetSpecularPower(20);
			ApplySetting("Opacity");
			ApplySetting("Level");
		}

		public void ApplySetting(string setting)
		{
			if (setting == "Visible")
			{
				if (_volumeGraphic.TissueSettings.Visible)
					_vtkActor.VisibilityOn();
				else
					_vtkActor.VisibilityOff();

				_vtkActor.ApplyProperties();
			}
			else if (setting == "Opacity")
			{
				_vtkActor.GetProperty().SetOpacity((double) _volumeGraphic.TissueSettings.Opacity);
				_vtkActor.ApplyProperties();
			}
			else if (setting == "Level")
			{
				_contourFilter.SetValue(0, _volumeGraphic.GetRescaledLevel());
				double R = _volumeGraphic.TissueSettings.MinimumColor.R/255.0f;
				double G = _volumeGraphic.TissueSettings.MinimumColor.G/255.0f;
				double B = _volumeGraphic.TissueSettings.MinimumColor.B/255.0f;
				_vtkActor.GetProperty().SetDiffuseColor(R, G, B);
			}
		}
	}
}
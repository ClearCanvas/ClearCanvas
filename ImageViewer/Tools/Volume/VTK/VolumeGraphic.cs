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
using System.ComponentModel;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using vtk;

namespace ClearCanvas.ImageViewer.Tools.Volume.VTK
{
	public enum RenderingMethod
	{
		Surface,
		Volume
	}

	public class VolumeGraphic : Graphic
	{
		private TissueSettings _tissueSettings;
		private IVtkProp _surfaceProp;
		private IVtkProp _volumeProp;
		private RenderingMethod _renderingMethod = RenderingMethod.Surface;

		public VolumeGraphic(TissueSettings tissueSettings)
		{
			_tissueSettings = tissueSettings;
			_tissueSettings.VolumeGraphic = this;
			_tissueSettings.PropertyChanged += OnTissueSettingsChanged;
		}

		public RenderingMethod RenderingMethod
		{
			get { return _renderingMethod; }
			set
			{
				if (_renderingMethod != value)
				{
					_renderingMethod = value;

					if (_renderingMethod == RenderingMethod.Surface)
					{
						this.SurfaceProp.VtkProp.VisibilityOn();
						this.SurfaceProp.ApplySetting("Opacity");
						this.SurfaceProp.ApplySetting("Level");

						this.VolumeProp.VtkProp.VisibilityOff();
					}
					else
					{
						this.VolumeProp.VtkProp.VisibilityOn();
						this.VolumeProp.ApplySetting("Opacity");
						this.VolumeProp.ApplySetting("Level");
						this.VolumeProp.ApplySetting("Window");

						this.SurfaceProp.VtkProp.VisibilityOff();
					}
				}
			}
		}

		public TissueSettings TissueSettings
		{
			get { return _tissueSettings; }
		}

		public vtkProp VtkProp
		{
			get
			{
				if (this.RenderingMethod == RenderingMethod.Surface)
					return this.SurfaceProp.VtkProp;
				else
					return this.VolumeProp.VtkProp;
			}
		}

		private IVtkProp SurfaceProp
		{
			get
			{
				if (_surfaceProp == null)
					_surfaceProp = new SurfaceProp(this);

				return _surfaceProp;
			}
		}

		private IVtkProp VolumeProp
		{
			get
			{
				if (_volumeProp == null)
					_volumeProp = new VolumeProp(this);

				return _volumeProp;
			}
		}

		private VolumePresentationImage ParentVolumePresentationImage
		{
			get { return this.ParentPresentationImage as VolumePresentationImage; }
		}

		internal vtkImageData GetImageData()
		{
			vtkImageData imageData = this.ParentVolumePresentationImage.VtkImageData;
			return imageData;
		}

		internal double GetWindowLeft()
		{
			return GetRescaledLevel() - (double) _tissueSettings.Window/2;
		}

		internal double GetWindowRight()
		{
			return GetRescaledLevel() + (double) _tissueSettings.Window/2;
		}

		internal double GetRescaledLevel()
		{
			return (double) _tissueSettings.Level -
			       this.ParentVolumePresentationImage.RescaleIntercept -
			       this.ParentVolumePresentationImage.MinimumPixelValue;
		}

		private void OnTissueSettingsChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "SurfaceRenderingSelected")
			{
				if (_tissueSettings.SurfaceRenderingSelected)
					this.RenderingMethod = RenderingMethod.Surface;
			}
			else if (e.PropertyName == "VolumeRenderingSelected")
			{
				if (_tissueSettings.VolumeRenderingSelected)
					this.RenderingMethod = RenderingMethod.Volume;
			}
			else
			{
				if (_tissueSettings.SurfaceRenderingSelected)
					_surfaceProp.ApplySetting(e.PropertyName);
				else
					_volumeProp.ApplySetting(e.PropertyName);
			}
		}

		public override bool HitTest(Point point)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void Move(SizeF delta)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
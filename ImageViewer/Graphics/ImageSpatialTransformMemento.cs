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

namespace ClearCanvas.ImageViewer.Graphics
{
	internal class ImageSpatialTransformMemento : IEquatable<ImageSpatialTransformMemento>
	{
		private bool _scaleToFit;
		private object _spatialTransformMemento;

		public ImageSpatialTransformMemento(bool scaleToFit, object spatialTransformMemento)
		{
			_scaleToFit = scaleToFit;
			_spatialTransformMemento = spatialTransformMemento;
		}

		public object SpatialTransformMemento
		{
			get { return _spatialTransformMemento; }
		}

		public bool ScaleToFit
		{
			get { return _scaleToFit; }
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			return this.Equals(obj as ImageSpatialTransformMemento);
		}

		#region IEquatable<ImageSpatialTransformMemento>

		public bool Equals(ImageSpatialTransformMemento other)
		{
			if (other == null)
				return false;

			return other.ScaleToFit == ScaleToFit && this.SpatialTransformMemento.Equals(other.SpatialTransformMemento);
		}

		#endregion
	}
}

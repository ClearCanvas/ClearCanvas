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
	internal class SpatialTransformMemento : IEquatable<SpatialTransformMemento>
	{
		private float _scale;
		private float _translationX;
		private float _translationY;
		private int _rotationXY;
		private bool _flipX;
		private bool _flipY;

		public SpatialTransformMemento()
		{
		}

		public float Scale
		{
			get { return _scale; }
			set { _scale = value; }
		}

		public float TranslationX
		{
			get { return _translationX; }
			set { _translationX = value; }
		}

		public float TranslationY
		{
			get { return _translationY; }
			set { _translationY = value; }
		}

		public bool FlipX
		{
			get { return _flipX; }
			set { _flipX = value; }
		}

		public bool FlipY
		{
			get { return _flipY; }
			set { _flipY = value; }
		}

		public int RotationXY
		{
			get { return _rotationXY; }
			set { _rotationXY = value; }
		}

		public override int GetHashCode()
		{
			// Normally, you would calculate a hash code dependent on immutable
			// member fields since we've given this object value type semantics
			// because of how we've overridden Equals().  However, this is a memento
			// and thus the actualy contents of the memento are irrelevant if they 
			// were ever put in a hashtable.  We are in fact interested in the instance.
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			return this.Equals(obj as SpatialTransformMemento);
		}

		#region IEquatable<SpatialTransformMemento> Members

		public bool Equals(SpatialTransformMemento other)
		{
			if (other == null)
				return false;

			return (this.Scale == other.Scale &&
					this.TranslationX == other.TranslationX &&
					this.TranslationY == other.TranslationY &&
					this.FlipY == other.FlipY &&
					this.FlipX == other.FlipX &&
					this.RotationXY == other.RotationXY);
		}

		#endregion
	}
}

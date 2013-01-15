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

using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Indicates the type of data represented by the overlay plane.
	/// </summary>
	public sealed class OverlayPlaneType
	{
		/// <summary>
		/// Indicates that the overlay plane represents some generic graphics.
		/// </summary>
		public static readonly OverlayPlaneType Graphics = new OverlayPlaneType(OverlayType.G, "Graphics");

		/// <summary>
		/// Indicates that the overlay plane represents a region of interest.
		/// </summary>
		public static readonly OverlayPlaneType ROI = new OverlayPlaneType(OverlayType.R, "ROI");

		/// <summary>
		/// Gets the DICOM enumerated value for the overlay plane type.
		/// </summary>
		public readonly OverlayType OverlayType;

		/// <summary>
		/// Gets a string description of the overlay plane type.
		/// </summary>
		public readonly string Description;

		private OverlayPlaneType(OverlayType type, string description)
		{
			this.OverlayType = type;
			this.Description = description;
		}

		/// <summary>
		/// Gets a hash code for the type suitable of hash tables.
		/// </summary>
		public override int GetHashCode()
		{
			return this.OverlayType.GetHashCode();
		}

		/// <summary>
		/// Tests to see if this object is equivalent to <paramref name="obj">another</paramref> <see cref="OverlayPlaneType"/> or <see cref="OverlayType"/>.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is OverlayPlaneType)
				return this.OverlayType == ((OverlayPlaneType) obj).OverlayType;
			else if (obj is OverlayType)
				return this.OverlayType == (OverlayType) obj;
			return false;
		}

		/// <summary>
		/// Gets a string that represents the current <see cref="OverlayPlaneType"/>.
		/// </summary>
		public override string ToString()
		{
			return this.Description;
		}

		/// <summary>
		/// Implicitly casts <paramref name="type"/> as a <see cref="OverlayType"/>.
		/// </summary>
		public static implicit operator OverlayType(OverlayPlaneType type)
		{
			if (type == null)
				return OverlayType.None;
			return type.OverlayType;
		}

		/// <summary>
		/// Implicitly casts <paramref name="type"/> as a <see cref="OverlayPlaneType"/>.
		/// </summary>
		public static implicit operator OverlayPlaneType(OverlayType type)
		{
			switch (type)
			{
				case OverlayType.G:
					return Graphics;
				case OverlayType.R:
					return ROI;
				case OverlayType.None:
				default:
					return null;
			}
		}
	}
}
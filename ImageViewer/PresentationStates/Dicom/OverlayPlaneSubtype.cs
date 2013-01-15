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
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Represents defined terms for identifying the intended purpose of the <see cref="OverlayPlaneType"/>.
	/// </summary>
	public class OverlayPlaneSubtype
	{
		/// <summary>
		/// Identifies that the overlay is a user created graphic annotation (e.g. operator).
		/// </summary>
		public static readonly OverlayPlaneSubtype User = new OverlayPlaneSubtype(OverlaySubtype.User);

		/// <summary>
		/// Identifies that the overlay is a machine or algorithm generated graphic annotation, such as output of a Computer Assisted Diagnosis algorithm.
		/// </summary>
		public static readonly OverlayPlaneSubtype Automated = new OverlayPlaneSubtype(OverlaySubtype.Automated);

		/// <summary>
		/// Gets the represented defined term.
		/// </summary>
		public readonly OverlaySubtype OverlaySubtype;

		/// <summary>
		/// Constructs a defined term.
		/// </summary>
		/// <param name="definedTerm">The defined term.</param>
		public OverlayPlaneSubtype(string definedTerm)
		{
			if (string.IsNullOrEmpty(definedTerm))
				throw new ArgumentNullException("definedTerm");
			this.OverlaySubtype = new CustomOverlaySubtype(definedTerm);
		}

		/// <summary>
		/// Constructs a defined term.
		/// </summary>
		/// <param name="definedTerm">The defined term.</param>
		public OverlayPlaneSubtype(OverlaySubtype definedTerm)
		{
			if (definedTerm == null)
				throw new ArgumentNullException("definedTerm");
			this.OverlaySubtype = definedTerm;
		}

		/// <summary>
		/// Gets a hash code for the defined term suitable of hash tables.
		/// </summary>
		public override sealed int GetHashCode()
		{
			return this.OverlaySubtype.GetHashCode();
		}

		/// <summary>
		/// Tests to see if this object is equivalent to <paramref name="obj">another</paramref> <see cref="OverlayPlaneSubtype"/> or <see cref="OverlaySubtype"/>.
		/// </summary>
		public override sealed bool Equals(object obj)
		{
			if (obj is OverlayPlaneSubtype)
				return this.OverlaySubtype.Equals(((OverlayPlaneSubtype) obj).OverlaySubtype);
			else if (obj is OverlaySubtype)
				return this.OverlaySubtype.Equals(obj);
			return false;
		}

		/// <summary>
		/// Gets a string that represents the current <see cref="OverlayPlaneSubtype"/>.
		/// </summary>
		public override sealed string ToString()
		{
			return this.OverlaySubtype.ToString();
		}

		/// <summary>
		/// Implicitly casts <paramref name="type"/> as a <see cref="OverlaySubtype"/>.
		/// </summary>
		public static implicit operator OverlaySubtype(OverlayPlaneSubtype type)
		{
			if (type == null)
				return null;
			return type.OverlaySubtype;
		}

		/// <summary>
		/// Explicitly casts <paramref name="type"/> as a <see cref="OverlayPlaneSubtype"/>.
		/// </summary>
		public static explicit operator OverlayPlaneSubtype(OverlaySubtype type)
		{
			if (type == null)
				return null;
			return new OverlayPlaneSubtype(type);
		}

		private class CustomOverlaySubtype : OverlaySubtype
		{
			public CustomOverlaySubtype(string definedTerm) : base(definedTerm) {}
		}
	}
}
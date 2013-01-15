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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal class VoiLutMemento : IEquatable<VoiLutMemento>
	{
		private readonly IVoiLut _originatingLut;
		private readonly object _innerMemento;
		private readonly bool _invert;

		public VoiLutMemento(IVoiLut originatingLut, bool invert)
		{
			Platform.CheckForNullReference(originatingLut, "originatingLut");
			_originatingLut = originatingLut;
			_innerMemento = originatingLut.CreateMemento();
			_invert = invert;
		}

		public IVoiLut OriginatingLut
		{
			get { return _originatingLut; }
		}

		public object InnerMemento
		{
			get { return _innerMemento; }
		}

		public bool Invert
		{
			get { return _invert; }
		}

		public override int GetHashCode()
		{
			return 0x76E013EC ^ _originatingLut.GetHashCode() ^ _invert.GetHashCode() ^ (_innerMemento != null ? _innerMemento.GetHashCode() : 0);
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(obj, this) || obj is VoiLutMemento && Equals((VoiLutMemento) obj);
		}

		public bool Equals(VoiLutMemento other)
		{
			return other != null && _originatingLut.Equals(other._originatingLut) && Equals(_innerMemento, other._innerMemento) && _invert == other._invert;
		}
	}
}
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
	internal sealed class ColorMapMemento : IEquatable<ColorMapMemento>
	{
		#region Private Fields

		private readonly IColorMap _originator;
		private readonly object _innerMemento;

		#endregion

		public ColorMapMemento(IColorMap originator)
		{
			Platform.CheckForNullReference(originator, "originator");
			_originator = originator;
			_innerMemento = originator.CreateMemento();
		}

		#region Public Members

		public IColorMap Originator
		{
			get { return _originator; }
		}

		public object InnerMemento
		{
			get { return _innerMemento; }
		}

		public override int GetHashCode()
		{
			return 0x4462FFBB ^ _originator.GetHashCode() ^ (_innerMemento != null ? _innerMemento.GetHashCode() : 0);
		}

		public override bool Equals(object obj)
		{
			return obj is ColorMapMemento && Equals((ColorMapMemento) obj);
		}

		public bool Equals(ColorMapMemento other)
		{
			return other != null && _originator.Equals(other._originator) && Equals(_innerMemento, other._innerMemento);
		}

		#endregion
	}
}
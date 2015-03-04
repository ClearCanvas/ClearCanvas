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

using System.Linq;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// An observable container for <see cref="AnnotationBox"/>es.
	/// </summary>
	[Cloneable(false)]
	public class AnnotationBoxList : ObservableList<AnnotationBox>
	{
		internal AnnotationBoxList()
		{
		}

        private AnnotationBoxList(AnnotationBoxList source, ICloningContext context)
            : base(source.Count)
		{
			foreach (AnnotationBox box in source)
				this.Add(box.Clone());
		}

		/// <summary>
		/// Creates a deep clone of this <see cref="AnnotationBoxList"/>.
		/// </summary>
		/// <returns>A deep clone of this <see cref="AnnotationBoxList"/>.</returns>
		public AnnotationBoxList Clone()
		{
			var clone = new AnnotationBoxList(this, null);
			return clone;
		}
	}
}

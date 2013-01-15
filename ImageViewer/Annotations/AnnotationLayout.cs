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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// Base class for <see cref="IAnnotationLayout"/>s.
	/// </summary>
	[Cloneable(true)]
	public class AnnotationLayout : IAnnotationLayout
	{
		private readonly AnnotationBoxList _annotationBoxes;

		/// <summary>
		/// Constructor.
		/// </summary>
		public AnnotationLayout()
		{
			_annotationBoxes = new AnnotationBoxList();
			Visible = true;
		}

		/// <summary>
		/// Gets the <see cref="AnnotationBox"/>es that define the layout.
		/// </summary>
		public AnnotationBoxList AnnotationBoxes
		{
			get { return _annotationBoxes; }
		}

		#region IAnnotationLayout Members

		/// <summary>
		/// Gets the <see cref="AnnotationBox"/>es that define the layout.
		/// </summary>
		IEnumerable<AnnotationBox> IAnnotationLayout.AnnotationBoxes
		{
			get { return _annotationBoxes; }
		}

		/// <summary>
		/// Gets or sets whether the <see cref="IAnnotationLayout"/> is visible.
		/// </summary>
		public bool Visible { get; set; }

		IAnnotationLayout IAnnotationLayout.Clone()
		{
			return Clone();
		}

		/// <summary>
		/// Creates a deep clone of this <see cref="AnnotationLayout"/>.
		/// </summary>
		public AnnotationLayout Clone()
		{
			return CloneBuilder.Clone(this) as AnnotationLayout;
		}
		
		#endregion
	}
}

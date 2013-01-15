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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Annotations
{
	[Cloneable]
	internal sealed class StoredAnnotationLayout : IAnnotationLayout
	{
		private readonly string _identifier;
		private readonly List<StoredAnnotationBoxGroup> _annotationBoxGroups = new List<StoredAnnotationBoxGroup>();
		private bool _visible = true;

		public StoredAnnotationLayout(string identifier)
		{
			Platform.CheckForEmptyString(identifier, "identifier");
			_identifier = identifier;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">This parameter is unused.</param>
		private StoredAnnotationLayout(StoredAnnotationLayout source, ICloningContext context)
		{
			this._identifier = source._identifier;
			this._visible = source._visible;
			foreach (StoredAnnotationBoxGroup group in source._annotationBoxGroups)
			{
				if (group == null)
					continue;
				this._annotationBoxGroups.Add(group.Clone());
			}
		}

		public string Identifier
		{
			get { return _identifier; }
		}

		public IList<StoredAnnotationBoxGroup> AnnotationBoxGroups
		{
			get { return _annotationBoxGroups; }
		}

		#region IAnnotationLayout Members

		public IEnumerable<AnnotationBox> AnnotationBoxes
		{
			get
			{
				foreach (StoredAnnotationBoxGroup group in _annotationBoxGroups)
				{
					foreach (AnnotationBox box in group.AnnotationBoxes)
						yield return box;
				}
			}
		}

		public bool Visible
		{
			get { return _visible; }
			set { _visible = value; }
		}

		public IAnnotationLayout Clone()
		{
			return new StoredAnnotationLayout(this, null);
		}

		#endregion
	}
}

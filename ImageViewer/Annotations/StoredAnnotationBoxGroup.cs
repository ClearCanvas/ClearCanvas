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
	internal sealed class StoredAnnotationBoxGroup
	{
		private readonly string _identifier;
		private readonly AnnotationBox _defaultBoxSettings;
		private readonly AnnotationBoxList _annotationBoxes;

		public StoredAnnotationBoxGroup(string identifier)
		{
			Platform.CheckForEmptyString(identifier, "identifier");
			_identifier = identifier;
			_defaultBoxSettings = new AnnotationBox();
			_annotationBoxes = new AnnotationBoxList();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">This parameter is unused.</param>
		private StoredAnnotationBoxGroup(StoredAnnotationBoxGroup source, ICloningContext context)
		{
			this._identifier = source._identifier;
			this._defaultBoxSettings = source._defaultBoxSettings.Clone();
			this._annotationBoxes = source._annotationBoxes.Clone();
		}

		public string Identifier
		{
			get { return _identifier; }
		}

		public AnnotationBox DefaultBoxSettings
		{
			get { return _defaultBoxSettings; }
		}

		public IList<AnnotationBox> AnnotationBoxes
		{
			get { return _annotationBoxes; }
		}

		public StoredAnnotationBoxGroup Clone()
		{
			return new StoredAnnotationBoxGroup(this, null);
		}
	}
}
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

using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// An <see cref="AnnotationItem"/> that returns fixed text from <see cref="GetAnnotationText"/>.
	/// </summary>
	/// <seealso cref="AnnotationItem"/>
	public class BasicTextAnnotationItem : AnnotationItem
	{
		private string _annotationText;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="identifier">The unique identifier of the <see cref="BasicTextAnnotationItem"/>.</param>
		/// <param name="displayName">The <see cref="BasicTextAnnotationItem"/>'s display name.</param>
		/// <param name="label">The <see cref="BasicTextAnnotationItem"/>'s label.</param>
		/// <param name="annotationText">The text to return from <see cref="GetAnnotationText"/>.</param>
		public BasicTextAnnotationItem(string identifier, string displayName, string label, string annotationText)
			: base(identifier, displayName, label)
		{
			Platform.CheckForEmptyString(annotationText, "annotationText");
			_annotationText = annotationText;
		}

		/// <summary>
		/// Gets or sets the text to be returned from <see cref="GetAnnotationText"/>.
		/// </summary>
		public string AnnotationText
		{
			get { return _annotationText; }
			set
			{
				Platform.CheckForEmptyString(value, "value");
				_annotationText = value;
			}
		}

		/// <summary>
		/// Gets the annotation text for display on the overlay.
		/// </summary>
		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			return _annotationText;
		}
	}
}

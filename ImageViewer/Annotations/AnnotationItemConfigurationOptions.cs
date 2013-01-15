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

using ClearCanvas.Common.Utilities;
namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// Configures how the <see cref="IAnnotationItem"/> text should be formatted/shown 
	/// in an <see cref="AnnotationBox"/>.
	/// </summary>
	/// <seealso cref="IAnnotationItem"/>
	[Cloneable(true)]
	public sealed class AnnotationItemConfigurationOptions
	{
		private bool _showLabel = false;
		private bool _showLabelIfValueEmpty = false;

		/// <summary>
		/// Constructor.
		/// </summary>
		public AnnotationItemConfigurationOptions()
		{
		}

		/// <summary>
		/// Gets or sets whether or not to show the label (<see cref="IAnnotationItem.GetLabel"/>).
		/// </summary>
		public bool ShowLabel
		{
			get { return _showLabel; }
			set { _showLabel = value; }
		}

		/// <summary>
		/// Gets or sets whether or not to show the label (<see cref="IAnnotationItem.GetLabel"/>) even
		/// if the overlay text (<see cref="IAnnotationItem.GetAnnotationText"/>) is empty.
		/// </summary>
		public bool ShowLabelIfValueEmpty
		{
			get { return _showLabelIfValueEmpty; }
			set { _showLabelIfValueEmpty = value; }
		}

		/// <summary>
		/// Creates a deep clone of this object.
		/// </summary>
		public AnnotationItemConfigurationOptions Clone()
		{
			AnnotationItemConfigurationOptions clone = new AnnotationItemConfigurationOptions();
			clone._showLabel = this._showLabel;
			clone._showLabelIfValueEmpty = this._showLabelIfValueEmpty;
			return clone;
		}
	}
}

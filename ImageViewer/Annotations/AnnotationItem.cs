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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// Base implementation of <see cref="IAnnotationItem"/>.
	/// </summary>
	/// <seealso cref="IAnnotationItem"/>
	public abstract class AnnotationItem : IAnnotationItem
	{
		private readonly IAnnotationResourceResolver _annotationResourceResolver;
		private readonly IResourceResolver _standardResourceResolver;
		private readonly string _identifier;
		private readonly string _displayName;
		private readonly string _label;

		/// <summary>
		/// A constructor that uses the <see cref="AnnotationItem"/>'s unique identifier to determine
		/// the display name and label using an <see cref="IAnnotationResourceResolver"/>.
		/// </summary>
		/// <param name="identifier">The unique identifier of the <see cref="AnnotationItem"/>.</param>
		/// <param name="annotationResourceResolver">The object that will resolve the display name and label 
		/// from the <see cref="AnnotationItem"/>'s unique identifier.</param>
		protected AnnotationItem(string identifier, IAnnotationResourceResolver annotationResourceResolver)
		{
			Platform.CheckForEmptyString(identifier, "identifier");

			_annotationResourceResolver = annotationResourceResolver ?? new AnnotationResourceResolver(this);
			_identifier = identifier;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="identifier">The unique identifier of the <see cref="AnnotationItem"/>.</param>
		/// <param name="displayName">The <see cref="AnnotationItem"/>'s display name.</param>
		/// <param name="label">The <see cref="AnnotationItem"/>'s label.</param>
		protected AnnotationItem(string identifier, string displayName, string label)
			: this(identifier, displayName, label, null) {}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="identifier">The unique identifier of the <see cref="AnnotationItem"/>.</param>
		/// <param name="displayName">The <see cref="AnnotationItem"/>'s display name.</param>
		/// <param name="label">The <see cref="AnnotationItem"/>'s label.</param>
		/// <param name="resourceResolver">The object that will resolve the display name and label parameters as the keys representing localized strings.</param>
		protected AnnotationItem(string identifier, string displayName, string label, IResourceResolver resourceResolver)
		{
			Platform.CheckForEmptyString(identifier, "identifier");
			Platform.CheckForEmptyString(displayName, "displayName");

			_standardResourceResolver = resourceResolver ?? new ResourceResolver(GetType(), false);
			_identifier = identifier;
			_displayName = displayName;
			_label = label ?? "";
		}

		#region IAnnotationItem Members

		/// <summary>
		/// Gets a unique identifier.
		/// </summary>
		public string GetIdentifier()
		{
			return _identifier;
		}

		/// <summary>
		/// Gets a user friendly display name.
		/// </summary>
		public string GetDisplayName()
		{
			if (_annotationResourceResolver != null) return _annotationResourceResolver.ResolveDisplayName(_identifier);
			return _standardResourceResolver != null ? _standardResourceResolver.LocalizeString(_displayName) : _displayName;
		}

		/// <summary>
		/// Gets the label that can be shown on the overlay depending on the <see cref="AnnotationBox"/>'s 
		/// configuration (see <see cref="AnnotationItemConfigurationOptions"/>).
		/// </summary>
		public string GetLabel()
		{
			if (_annotationResourceResolver != null) return _annotationResourceResolver.ResolveLabel(_identifier);
			return _standardResourceResolver != null ? _standardResourceResolver.LocalizeString(_label) : _label;
		}

		/// <summary>
		/// Gets the annotation text to display on the overlay for <paramref name="presentationImage"/>.
		/// </summary>
		public abstract string GetAnnotationText(IPresentationImage presentationImage);

		#endregion
	}
}

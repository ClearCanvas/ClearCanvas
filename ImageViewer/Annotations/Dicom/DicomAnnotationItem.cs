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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Annotations.Dicom
{
	/// <summary>
	/// A specialization of <see cref="AnnotationItem"/> for showing dicom tag data on the overlay.
	/// </summary>
	/// <seealso cref="AnnotationItem"/>
	public class DicomAnnotationItem<T>: AnnotationItem
	{
		private readonly FrameDataRetrieverDelegate<T> _sopDataRetrieverDelegate;
		private readonly ResultFormatterDelegate<T> _resultFormatterDelegate;

		/// <summary>
		/// A constructor that uses the <see cref="DicomAnnotationItem{T}"/>'s unique identifier to determine
		/// the display name and label using an <see cref="IAnnotationResourceResolver"/>.
		/// </summary>
		/// <param name="identifier">The unique identifier of the <see cref="DicomAnnotationItem{T}"/>.</param>
		/// <param name="resolver">The object that will resolve the display name and label 
		/// from the <see cref="DicomAnnotationItem{T}"/>'s unique identifier.</param>
		/// <param name="sopDataRetrieverDelegate">A delegate used to retrieve the Dicom tag data.</param>
		/// <param name="resultFormatterDelegate">A delegate that will format the Dicom tag data as a string.</param>
		public DicomAnnotationItem
			(
				string identifier,
				IAnnotationResourceResolver resolver,
				FrameDataRetrieverDelegate<T> sopDataRetrieverDelegate,
				ResultFormatterDelegate<T> resultFormatterDelegate
			)
			: base(identifier, resolver)
		{
			Platform.CheckForNullReference(sopDataRetrieverDelegate, "sopDataRetrieverDelegate");
			Platform.CheckForNullReference(resultFormatterDelegate, "resultFormatterDelegate");

			_sopDataRetrieverDelegate = sopDataRetrieverDelegate;
			_resultFormatterDelegate = resultFormatterDelegate;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="identifier">The unique identifier of the <see cref="DicomAnnotationItem{T}"/>.</param>
		/// <param name="displayName">The <see cref="DicomAnnotationItem{T}"/>'s display name.</param>
		/// <param name="label">The <see cref="DicomAnnotationItem{T}"/>'s label.</param>
		/// <param name="sopDataRetrieverDelegate">A delegate used to retrieve the Dicom tag data.</param>
		/// <param name="resultFormatterDelegate">A delegate that will format the Dicom tag data as a string.</param>
		public DicomAnnotationItem
			(
				string identifier,
				string displayName,
				string label,
				FrameDataRetrieverDelegate<T> sopDataRetrieverDelegate,
				ResultFormatterDelegate<T> resultFormatterDelegate
			)
			: base(identifier, displayName, label)
		{
			Platform.CheckForNullReference(sopDataRetrieverDelegate, "sopDataRetrieverDelegate");
			Platform.CheckForNullReference(resultFormatterDelegate, "resultFormatterDelegate");

			_sopDataRetrieverDelegate = sopDataRetrieverDelegate;
			_resultFormatterDelegate = resultFormatterDelegate;
		}

		/// <summary>
		/// Gets the annotation text for display on the overlay.
		/// </summary>
		/// <remarks>
		/// The input <see cref="IPresentationImage"/> must implement <see cref="IImageSopProvider"/> in 
		/// order for a non-empty string to be returned.
		/// </remarks>
		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			IImageSopProvider associatedDicom = presentationImage as IImageSopProvider;
			if (associatedDicom == null)
				return "";

			return _resultFormatterDelegate(_sopDataRetrieverDelegate(associatedDicom.Frame));
		}
	}
}

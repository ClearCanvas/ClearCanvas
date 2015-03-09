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
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Annotations
{
	[Cloneable(false)]
	internal class MammogramAnnotationLayoutProxy : IAnnotationLayout
	{
		[CloneIgnore]
		private DicomGrayscalePresentationImage _ownerImage;

		[CloneIgnore]
		private Dictionary<object, AnnotationBox> _annotationBoxes;

		private string _layoutId = null;
		private bool _visible = true;

		public MammogramAnnotationLayoutProxy() {}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected MammogramAnnotationLayoutProxy(MammogramAnnotationLayoutProxy source, ICloningContext context)
		{
			context.CloneFields(source, this);

			// clone the annotation boxes with their state!
			if (source._annotationBoxes != null)
			{
				var annotationBoxes = new Dictionary<object, AnnotationBox>();
				foreach (var sourceBox in source._annotationBoxes.Values)
				{
					var cloneBox = (AnnotationBox) CloneBuilder.Clone(sourceBox);

					// if the box has an item, use the item's identifier as the key
					object key = cloneBox;
					if (cloneBox.AnnotationItem != null)
					{
						key = cloneBox.AnnotationItem.GetIdentifier();

						// if for some reason the key is a duplicate, use the box as the key (but keep it as part of the layout!)
						if (annotationBoxes.ContainsKey(key))
							key = cloneBox;
					}
					annotationBoxes.Add(key, cloneBox);
				}
				_annotationBoxes = annotationBoxes;
			}
		}

		public DicomGrayscalePresentationImage OwnerImage
		{
			get { return _ownerImage; }
			set { _ownerImage = value; }
		}

		public bool Visible
		{
			get { return _visible; }
			set { _visible = value; }
		}

		public IEnumerable<AnnotationBox> AnnotationBoxes
		{
			get
			{
				// we only need to rebuild annotation boxes if the layout ID has changed
				var layoutId = GetCurrentLayoutId();
				if (layoutId != _layoutId || _annotationBoxes == null)
				{
					// index the annotation boxes in the new layout
					var annotationBoxes = new Dictionary<object, AnnotationBox>();
					foreach (var annotationBox in AnnotationLayoutFactory.CreateLayout(layoutId).AnnotationBoxes)
					{
						// if the box has an item, use the item's identifier as the key
						object key = annotationBox;
						if (annotationBox.AnnotationItem != null)
						{
							key = annotationBox.AnnotationItem.GetIdentifier();

							// if for some reason the key is a duplicate, use the box as the key (but keep it as part of the layout!)
							if (annotationBoxes.ContainsKey(key))
								key = annotationBox;
						}
						annotationBoxes.Add(key, annotationBox);

						// if the old layout has an item with the same key, copy its visibility state over
						if (_annotationBoxes != null && _annotationBoxes.ContainsKey(key))
							annotationBox.Visible = _annotationBoxes[key].Visible;
					}

					// update variables
					_annotationBoxes = annotationBoxes;
					_layoutId = layoutId;
				}
				return _annotationBoxes.Values;
			}
		}

		public IAnnotationLayout Clone()
		{
			return (IAnnotationLayout) CloneBuilder.Clone(this);
		}

		private string GetCurrentLayoutId()
		{
			if (_ownerImage != null && _ownerImage.SpatialTransform is MammographyImageSpatialTransform)
			{
				string effectiveRowOrientation, effectiveColumnOrientation;
				((MammographyImageSpatialTransform) _ownerImage.SpatialTransform).GetEffectivePosteriorPatientOrientation(out effectiveRowOrientation, out effectiveColumnOrientation);
				var filterCandidates = new List<KeyValuePair<string, string>>();
				filterCandidates.Add(new KeyValuePair<string, string>("Modality", _ownerImage.ImageSop.Modality));
				filterCandidates.Add(new KeyValuePair<string, string>("PatientOrientation_Row", effectiveRowOrientation));
				filterCandidates.Add(new KeyValuePair<string, string>("PatientOrientation_Col", effectiveColumnOrientation));
				return DicomFilteredAnnotationLayoutStore.Instance.GetMatchingStoredLayoutId(filterCandidates);
			}
			return string.Empty;
		}
	}
}
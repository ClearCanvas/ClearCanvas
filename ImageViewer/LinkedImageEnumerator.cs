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

namespace ClearCanvas.ImageViewer
{
	internal class LinkedImageEnumerator : IEnumerable<IPresentationImage>
	{
		private readonly IPresentationImage _referenceImage;
		private bool _includeAllImageSets;
		private bool _excludeReferenceImage;

		public LinkedImageEnumerator(IPresentationImage referenceImage)
		{
			_referenceImage = referenceImage;
			_includeAllImageSets = false;
			_excludeReferenceImage = false;
		}

		public bool IncludeAllImageSets
		{
			get { return _includeAllImageSets; }
			set { _includeAllImageSets = value; }
		}

		public bool ExcludeReferenceImage
		{
			get { return _excludeReferenceImage; }
			set { _excludeReferenceImage = value; }
		}

		public IPresentationImage ReferenceImage
		{
			get { return _referenceImage; }
		}

		private IEnumerable<IPresentationImage> GetAllLinkedImages()
		{
			IDisplaySet parentDisplaySet = _referenceImage.ParentDisplaySet;
			IImageSet parentImageSet = parentDisplaySet.ParentImageSet;

			// If display set is linked and selected, then iterate through all the linked images
			// from the other linked display sets
			if (parentDisplaySet.Linked)
			{
				if (_includeAllImageSets)
				{
					foreach (IImageSet imageSet in parentImageSet.ParentLogicalWorkspace.ImageSets)
					{
						foreach (IDisplaySet displaySet in imageSet.LinkedDisplaySets)
						{
							foreach (IPresentationImage image in GetAllLinkedImages(displaySet))
								yield return image;
						}
					}
				}
				else
				{
					foreach (IDisplaySet currentDisplaySet in parentImageSet.LinkedDisplaySets)
					{
						foreach (IPresentationImage image in GetAllLinkedImages(currentDisplaySet))
							yield return image;
					}
				}
			}
			// If display set is just selected, then iterate through all the linked images
			// in that display set.
			else
			{
				foreach (IPresentationImage image in GetAllLinkedImages(parentDisplaySet))
					yield return image;
			}
		}

		private IEnumerable<IPresentationImage> GetAllLinkedImages(IDisplaySet displaySet)
		{
			foreach (IPresentationImage image in displaySet.LinkedPresentationImages)
			{
				if (image != _referenceImage)
					yield return image;
			}
		}

		private IEnumerable<IPresentationImage> GetImages()
		{
			if (!_excludeReferenceImage)
				yield return _referenceImage;

			foreach (IPresentationImage image in this.GetAllLinkedImages())
				yield return image;
		}

		#region IEnumerable<IPresentationImage> Members

		public IEnumerator<IPresentationImage> GetEnumerator()
		{
			return GetImages().GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetImages().GetEnumerator();
		}

		#endregion
	}
}

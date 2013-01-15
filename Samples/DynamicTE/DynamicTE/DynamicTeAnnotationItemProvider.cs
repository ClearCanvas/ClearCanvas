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
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class DynamicTeAnnotationItemProvider : IAnnotationItemProvider
	{
		private List<IAnnotationItem> _annotationItems;

		public DynamicTeAnnotationItemProvider()
		{
		}


		#region IAnnotationItemProvider Members

		public string GetIdentifier()
		{
			return "AnnotationItemProviders.Presentation";
		}

		public string GetDisplayName()
		{
			return "Presentation";
		}

		public IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			if (_annotationItems == null)
			{
				_annotationItems = new List<IAnnotationItem>();
				_annotationItems.Add(new DynamicTeAnnotationItem(this));
			}

			return _annotationItems;
		}

		#endregion
	}
}

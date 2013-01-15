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

using System;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	internal class DynamicTeAnnotationItem : IAnnotationItem
	{
		public DynamicTeAnnotationItem(IAnnotationItemProvider ownerProvider)
		{ 
		
		}

		#region IAnnotationItem Members

		public string GetIdentifier()
		{
			return "Presentation.DynamicTe";
		}

		public string GetDisplayName()
		{
			return "DynamicTe";
		}

		public string GetLabel()
		{
			return "DynamicTe";
		}

		public string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage == null)
				return string.Empty;

			IDynamicTeProvider teProvider = presentationImage as IDynamicTeProvider;

			if (teProvider == null)
				return string.Empty;

			string annotationText = String.Format("Echo Time: {0:F2}", teProvider.DynamicTe.Te);

			return annotationText;
		}

		#endregion
	}
}

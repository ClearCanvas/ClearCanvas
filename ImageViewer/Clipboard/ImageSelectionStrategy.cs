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

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard
{
	public delegate IEnumerable<IPresentationImage> GetImagesDelegate(IDisplaySet displaySet);

	public class ImageSelectionStrategy : IImageSelectionStrategy
	{
		private readonly string _description;
		private readonly GetImagesDelegate _getImagesDelegate;

		public ImageSelectionStrategy(string description, GetImagesDelegate getImagesDelegate)
		{
			Platform.CheckForNullReference(getImagesDelegate, "getImagesDelegate");
			_description = description ?? "";
			_getImagesDelegate = getImagesDelegate;
		}

		#region IImageSelectionStrategy Members

		public string Description
		{
			get { return _description; }
		}

		public IEnumerable<IPresentationImage> GetImages(IDisplaySet displaySet)
		{
			return _getImagesDelegate(displaySet);
		}

		#endregion
	}
}
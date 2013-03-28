#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <remarks>
	/// This type is part of a trial API and is not intended for general use.
	/// </remarks>
	public class AsyncPresentationImageFactory : PresentationImageFactory
	{
		private static readonly AsyncPresentationImageFactory _defaultInstance = new AsyncPresentationImageFactory();

		protected override IPresentationImage CreateImage(Frame frame)
		{
			if (frame.PhotometricInterpretation == PhotometricInterpretation.Unknown)
				throw new Exception("Photometric interpretation is unknown.");

			IDicomPresentationImage image;

			if (!frame.PhotometricInterpretation.IsColor)
				image = new AsyncDicomGrayscalePresentationImage(frame);
			else
				image = new DicomColorPresentationImage(frame);

			if (image.PresentationState == null || Equals(image.PresentationState, PresentationState.DicomDefault))
				image.PresentationState = DefaultPresentationState;

			return image;
		}

		public new static List<IPresentationImage> Create(ImageSop imageSop)
		{
			return _defaultInstance.CreateImages(imageSop);
		}

		public new static IPresentationImage Create(Frame frame)
		{
			return _defaultInstance.CreateImage(frame);
		}
	}
}
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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	public class DynamicTePresentationImage 
		: DicomGrayscalePresentationImage, 
		IDynamicTeProvider
	{
		#region Private fields

		private Frame _frame;
		private DynamicTe _dynamicTe;
		private ColorImageGraphic _probabilityOverlay;

		#endregion

		public DynamicTePresentationImage(
			Frame frame, 
			byte[] protonDensityMap,
			byte[] t2Map,
			byte[] probabilityMap)
			: base(frame)
		{
			Platform.CheckForNullReference(frame, "imageSop");

			_frame = frame;

			// TODO (Norman): DicomFilteredAnnotationLayoutProvider was made internal.  Either need to derive
			// this class from DicomGrayscalePresentationImage or create a layout provider.
			//this.AnnotationLayoutProvider = new DicomFilteredAnnotationLayoutProvider(this);

			AddProbabilityOverlay();
			_dynamicTe = new DynamicTe(
				this.ImageGraphic as GrayscaleImageGraphic, 
				protonDensityMap, 
				t2Map,
				_probabilityOverlay,
				probabilityMap);
		}

		public DynamicTe DynamicTe
		{
			get { return _dynamicTe; }
		}

		public bool ProbabilityOverlayVisible
		{
			get { return _probabilityOverlay.Visible; }
			set { _probabilityOverlay.Visible = value; }
		}

		public override IPresentationImage CreateFreshCopy()
		{
 			 return new DynamicTePresentationImage(
				 _frame, 
				 this.DynamicTe.ProtonDensityMap, 
				 this.DynamicTe.T2Map,
				 this.DynamicTe.ProbabilityMap);
		}


		private void AddProbabilityOverlay()
		{
			_probabilityOverlay = new ColorImageGraphic(_frame.Rows, _frame.Columns);
			this.OverlayGraphics.Add(_probabilityOverlay);
		}
	}
}

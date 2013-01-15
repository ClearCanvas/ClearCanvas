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
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
	public sealed class MinMaxAlgorithmPresetVoiLutOperationComponent : DefaultPresetVoiLutOperationComponent
	{
		public MinMaxAlgorithmPresetVoiLutOperationComponent()
		{
		}

		public override string Name
		{
			get { return SR.MinMaxAlgorithmPresetVoiLutOperationComponentName; }
		}

		public override string Description
		{
			get { return SR.MinMaxAlgorithmPresetVoiLutOperationComponentDescription; }
		}

		public override bool AppliesTo(IPresentationImage presentationImage)
		{
			return base.AppliesTo(presentationImage) && LutHelper.IsGrayScaleImage(presentationImage);
		}

		public override void Apply(IPresentationImage presentationImage)
		{
			if (!AppliesTo(presentationImage))
				throw new InvalidOperationException("The input presentation image is not supported.");

			IVoiLutManager manager = ((IVoiLutProvider)presentationImage).VoiLutManager;
			IVoiLut currentLut = manager.VoiLut;

			if (currentLut is MinMaxPixelCalculatedLinearLut)
				return;

			GrayscalePixelData pixelData = (GrayscalePixelData)((IImageGraphicProvider) presentationImage).ImageGraphic.PixelData;

			IModalityLutProvider modalityLutProvider = presentationImage as IModalityLutProvider;
			if (modalityLutProvider != null)
				manager.InstallVoiLut(new MinMaxPixelCalculatedLinearLut(pixelData, modalityLutProvider.ModalityLut));
			else
				manager.InstallVoiLut(new MinMaxPixelCalculatedLinearLut(pixelData));
		}
	}
}
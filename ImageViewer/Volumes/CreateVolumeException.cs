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
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Volumes
{
	[Serializable]
	public class CreateVolumeException : Exception
	{
		public CreateVolumeException()
			: base("An unexpected exception was encountered while creating the volume.") {}

		public CreateVolumeException(string message)
			: base(message) {}

		public CreateVolumeException(string message, Exception innerException)
			: base(message, innerException) {}

		protected CreateVolumeException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}

	[Serializable]
	public class UnsupportedSourceImagesException : CreateVolumeException
	{
		public UnsupportedSourceImagesException()
			: base("Source images are of an unsupported type.") {}

		protected UnsupportedSourceImagesException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}

	[Serializable]
	public class UnsupportedPixelFormatSourceImagesException : CreateVolumeException
	{
		public UnsupportedPixelFormatSourceImagesException()
			: base("Source images must be 16-bit monochrome images.") {}

		protected UnsupportedPixelFormatSourceImagesException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}

	[Serializable]
	public class InconsistentRescaleFunctionTypeException : CreateVolumeException
	{
		public InconsistentRescaleFunctionTypeException()
			: base("Multiple types of rescale function outputs were found in the source frames. All the rescale functions of the source frames must output in the same units.") {}

		protected InconsistentRescaleFunctionTypeException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}

	[Serializable]
	public class InconsistentImageLateralityException : CreateVolumeException
	{
		public InconsistentImageLateralityException()
			: base("Multiple image lateralities were found in the source frames. All the source frames must be of the same anatomical laterality.") {}

		protected InconsistentImageLateralityException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}

	[Serializable]
	public class InsufficientFramesException : CreateVolumeException
	{
		public InsufficientFramesException()
			: base("Insufficient frames from which to create a volume. At least three are required.") {}

		protected InsufficientFramesException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}

	[Serializable]
	public class NullSourceSeriesException : CreateVolumeException
	{
		public NullSourceSeriesException()
			: base("One or more source frames are missing study and/or series information.") {}

		protected NullSourceSeriesException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}

	[Serializable]
	public class MultipleSourceSeriesException : CreateVolumeException
	{
		public MultipleSourceSeriesException()
			: base("Multiple studies/series were found in the source frames. All source frames must be from the same study and series.") {}

		protected MultipleSourceSeriesException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}

	[Serializable]
	public class NullFrameOfReferenceException : CreateVolumeException
	{
		public NullFrameOfReferenceException()
			: base("One or more source frames do not specify the frame of reference.") {}

		protected NullFrameOfReferenceException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}

	[Serializable]
	public class MultipleFramesOfReferenceException : CreateVolumeException
	{
		public MultipleFramesOfReferenceException()
			: base("Multiple frames of reference were found in the source frames. All source frames must have the same frame of reference.") {}

		protected MultipleFramesOfReferenceException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}

	[Serializable]
	public class NullImageOrientationException : CreateVolumeException
	{
		public NullImageOrientationException()
			: base("One or more source frames do not have the image orientation defined.") {}

		protected NullImageOrientationException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}

	[Serializable]
	public class MultipleImageOrientationsException : CreateVolumeException
	{
		public MultipleImageOrientationsException()
			: base("Mulitple image orientations were found in the source frames. All source frames must have the same image orientation.") {}

		protected MultipleImageOrientationsException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}

	[Serializable]
	public class UnevenlySpacedFramesException : CreateVolumeException
	{
		public UnevenlySpacedFramesException()
			: base("Source frames must be evenly spaced.") {}

		protected UnevenlySpacedFramesException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}

	[Serializable]
	public class UncalibratedFramesException : CreateVolumeException
	{
		public UncalibratedFramesException()
			: base("Source frames must be calibrated.") {}

		protected UncalibratedFramesException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}

	[Serializable]
	public class AnisotropicPixelAspectRatioException : CreateVolumeException
	{
		public AnisotropicPixelAspectRatioException()
			: base("Source frames must have isotropic pixel aspect ratio.") {}

		protected AnisotropicPixelAspectRatioException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}

	[Serializable]
	public class UnsupportedGantryTiltAxisException : CreateVolumeException
	{
		public UnsupportedGantryTiltAxisException()
			: base("Source frames have a gantry tilt about an unsupported axis.") {}

		protected UnsupportedGantryTiltAxisException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}

	[ExceptionPolicyFor(typeof (CreateVolumeException))]
	[ExtensionOf(typeof (ExceptionPolicyExtensionPoint))]
	public class CreateVolumeExceptionPolicy : IExceptionPolicy
	{
		public void Handle(Exception ex, IExceptionHandlingContext exceptionHandlingContext)
		{
			var message = SR.MessageVolumeSourceUnexpectedException;
			if (ex is InsufficientFramesException)
				message = SR.MessageVolumeSourceMinimumThreeImages;
			else if (ex is UnsupportedSourceImagesException)
				message = SR.MessageVolumeSourceAreNotSupported;
			else if (ex is UnsupportedPixelFormatSourceImagesException)
				message = SR.MessageVolumeSourceMustBe16BitGreyscale;
			else if (ex is MultipleFramesOfReferenceException)
				message = SR.MessageVolumeSourceMustBeSingleFrameOfReference;
			else if (ex is MultipleImageOrientationsException)
				message = SR.MessageVolumeSourceMustBeSameImageOrientationPatient;
			else if (ex is MultipleSourceSeriesException)
				message = SR.MessageVolumeSourceMustBeSingleSeries;
			else if (ex is NullFrameOfReferenceException)
				message = SR.MessageVolumeSourceMustSpecifyFrameOfReference;
			else if (ex is NullImageOrientationException)
				message = SR.MessageVolumeSourceMustDefineImageOrientationPatient;
			else if (ex is NullSourceSeriesException)
				message = SR.MessageVolumeSourceMustBeSingleSeries;
			else if (ex is UnevenlySpacedFramesException)
				message = SR.MessageVolumeSourceMustBeEvenlySpacedForMpr;
			else if (ex is UncalibratedFramesException)
				message = SR.MessageVolumeSourceMustBeCalibrated;
			else if (ex is AnisotropicPixelAspectRatioException)
				message = SR.MessageVolumeSourceMayNotHaveAnisotropicPixels;
			else if (ex is UnsupportedGantryTiltAxisException)
				message = SR.MessageVolumeSourceMayBotBeGantrySlewed;
			else if (ex is InconsistentRescaleFunctionTypeException)
				message = SR.MessageVolumeSourceInconsistentRescaleUnits;
			else if (ex is InconsistentImageLateralityException)
				message = SR.MessageVolumeSourceInconsistentImageLaterality;
			exceptionHandlingContext.ShowMessageBox(message);
		}
	}
}
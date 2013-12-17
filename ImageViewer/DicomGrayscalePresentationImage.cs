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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A DICOM grayscale <see cref="PresentationImage"/>.
	/// </summary>
	[Cloneable]
	public class DicomGrayscalePresentationImage : GrayscalePresentationImage, IDicomPresentationImage, IDicomVoiLutsProvider
	{
		[CloneIgnore]
		private IFrameReference _frameReference;

		[CloneIgnore]
		private CompositeGraphic _dicomGraphics;

		[CloneIgnore]
		private IPatientCoordinateMapping _patientCoordinateMapping;

		[CloneIgnore]
		private IPatientPresentation _patientPresentation;

		[CloneIgnore]
		private readonly DicomVoiLuts _dicomVoiLuts;

		/// <summary>
		/// Initializes a new instance of <see cref="DicomGrayscalePresentationImage"/>.
		/// </summary>
		/// <param name="frame">The <see cref="Frame"/> from which to construct the image.</param>
		/// <remarks>
		/// This constructor provides a convenient means of associating a <see cref="Frame"/> with a <see cref="GrayscalePresentationImage"/>.
		/// </remarks>
		public DicomGrayscalePresentationImage(Frame frame)
			: this(frame.CreateTransientReference()) {}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomGrayscalePresentationImage"/>.
		/// </summary>
		/// <param name="frameReference">A <see cref="IFrameReference">reference</see> to the frame from which to construct the image.</param>
		public DicomGrayscalePresentationImage(IFrameReference frameReference)
			: base(frameReference.Frame.Rows,
			       frameReference.Frame.Columns,
			       frameReference.Frame.BitsAllocated,
			       frameReference.Frame.BitsStored,
			       frameReference.Frame.HighBit,
			       frameReference.Frame.PixelRepresentation != 0,
			       frameReference.Frame.PhotometricInterpretation == PhotometricInterpretation.Monochrome1,
			       frameReference.Frame.RescaleSlope,
			       frameReference.Frame.RescaleIntercept,
			       frameReference.Frame.NormalizedPixelSpacing.Column,
			       frameReference.Frame.NormalizedPixelSpacing.Row,
			       frameReference.Frame.PixelAspectRatio.Column,
			       frameReference.Frame.PixelAspectRatio.Row,
			       frameReference.Frame.GetNormalizedPixelData)
		{
			_frameReference = frameReference;
			_dicomVoiLuts = new DicomVoiLuts(this);
			base.PresentationState = PresentationState.DicomDefault;

			if (ImageSop.Modality == "MG")
			{
				// use a special image spatial transform for digital mammography
				CompositeImageGraphic.SpatialTransform = new MammographyImageSpatialTransform(CompositeImageGraphic, Frame.Rows, Frame.Columns, Frame.NormalizedPixelSpacing.Column, Frame.NormalizedPixelSpacing.Row, Frame.PixelAspectRatio.Column, Frame.PixelAspectRatio.Row, Frame.PatientOrientation, Frame.Laterality);
			}

			if (ImageSop.Modality == "PT" && frameReference.Frame.IsSubnormalRescale)
			{
				// some PET images have such a small slope that all stored pixel values map to one single value post-modality LUT
				// we detect this condition here and apply the inverse of the modality LUT as a normalization function for VOI purposes
				// http://groups.google.com/group/comp.protocols.dicom/browse_thread/thread/8930b159cb2a8e73?pli=1
				ImageGraphic.NormalizationLut = new NormalizationLutLinear(frameReference.Frame.RescaleSlope, frameReference.Frame.RescaleIntercept);
			}

			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected DicomGrayscalePresentationImage(DicomGrayscalePresentationImage source, ICloningContext context)
			: base(source, context)
		{
			Frame frame = source.Frame;
			_frameReference = frame.CreateTransientReference();
			_dicomVoiLuts = new DicomVoiLuts(this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_dicomGraphics = CollectionUtils.SelectFirst(base.CompositeImageGraphic.Graphics,
			                                             test => test.Name == "DICOM") as CompositeGraphic;

			if (AnnotationLayout is MammogramAnnotationLayoutProxy)
				((MammogramAnnotationLayoutProxy) AnnotationLayout).OwnerImage = this;

			Initialize();
		}

		private void Initialize()
		{
			if (base.ImageGraphic.VoiLutFactory == null)
			{
				base.ImageGraphic.VoiLutFactory = GraphicVoiLutFactory.Create(GetInitialVoiLut);
			}

			if (_dicomGraphics == null)
			{
				_dicomGraphics = new CompositeGraphic();
				_dicomGraphics.Name = "DICOM";

				// insert the DICOM graphics layer right after the image graphic (both contain domain-level graphics)
				IGraphic imageGraphic = CollectionUtils.SelectFirst(base.CompositeImageGraphic.Graphics, g => g is ImageGraphic);
				base.CompositeImageGraphic.Graphics.Insert(base.CompositeImageGraphic.Graphics.IndexOf(imageGraphic) + 1, _dicomGraphics);
			}
		}

		private static IVoiLut GetInitialVoiLut(IGraphic graphic)
		{
			GrayscaleImageGraphic grayImageGraphic = (GrayscaleImageGraphic) graphic;
			IVoiLut lut = InitialVoiLutProvider.Instance.GetLut(graphic.ParentPresentationImage);
			if (lut == null)
				lut = new MinMaxPixelCalculatedLinearLut(grayImageGraphic.PixelData, grayImageGraphic.ModalityLut);
			return lut;
		}

		/// <summary>
		/// Creates a fresh copy of the <see cref="DicomGrayscalePresentationImage"/>.
		/// </summary>
		/// <remarks>
		/// This will instantiate a fresh copy of this <see cref="DicomGrayscalePresentationImage"/>
		/// using the same construction parameters as the original.
		/// </remarks>
		/// <returns></returns>		
		public override IPresentationImage CreateFreshCopy()
		{
			DicomGrayscalePresentationImage image = new DicomGrayscalePresentationImage(Frame);
			image.PresentationState = this.PresentationState;
			return image;
		}

		#region IImageSopProvider members

		/// <summary>
		/// Gets this presentation image's associated <see cref="ImageSop"/>.
		/// </summary>
		/// <remarks>
		/// Use <see cref="ImageSop"/> to access DICOM tags.
		/// </remarks>
		public ImageSop ImageSop
		{
			get { return Frame.ParentImageSop; }
		}

		/// <summary>
		/// Gets this presentation image's associated <see cref="Frame"/>.
		/// </summary>
		public Frame Frame
		{
			get { return _frameReference.Frame; }
		}

		#endregion

		#region ISopProvider Members

		Sop ISopProvider.Sop
		{
			get { return _frameReference.Sop; }
		}

		#endregion

		#region IDicomVoiLutsProvider Members

		/// <summary>
		/// Gets a collection of DICOM-defined VOI LUTs from the image header and/or any associated presentation state.
		/// </summary>
		public IDicomVoiLuts DicomVoiLuts
		{
			get { return _dicomVoiLuts; }
		}

		#endregion

		#region IDicomPresentationImage Members

		/// <summary>
		/// Gets this presentation image's collection of domain-level graphics.
		/// </summary>
		/// <remarks>
		/// Use <see cref="IDicomPresentationImage.DicomGraphics"/> to add DICOM-defined graphics that you want to
		/// overlay the image at the domain-level. These graphics are rendered
		/// before any <see cref="IApplicationGraphicsProvider.ApplicationGraphics"/>
		/// and before any <see cref="IOverlayGraphicsProvider.OverlayGraphics"/>.
		/// </remarks>
		public GraphicCollection DicomGraphics
		{
			get { return _dicomGraphics.Graphics; }
		}

		#endregion

		#region IPatientPresentationProvider Members

		public IPatientPresentation PatientPresentation
		{
			get { return _patientPresentation ?? (_patientPresentation = CreatePatientPresentation()); }
		}

		protected virtual IPatientPresentation CreatePatientPresentation()
		{
			return new BasicPatientPresentation(this);
		}

		#endregion

		#region IPatientCoordinateMappingProvider Members

		public IPatientCoordinateMapping PatientCoordinateMapping
		{
			get { return _patientCoordinateMapping ?? (_patientCoordinateMapping = CreatePatientCoordinateMapping()); }
		}

		protected virtual IPatientCoordinateMapping CreatePatientCoordinateMapping()
		{
			return new PatientCoordinateMapping(Frame);
		}

		#endregion

		/// <summary>
		/// Dispose method.  Inheritors should override this method to do any additional cleanup.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && _frameReference != null)
			{
				_frameReference.Dispose();
				_frameReference = null;
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Creates the <see cref="IAnnotationLayout"/> for this image.
		/// </summary>
		/// <returns></returns>
		protected override IAnnotationLayout CreateAnnotationLayout()
		{
			if (ImageSop.Modality == "MG")
				return new MammogramAnnotationLayoutProxy {OwnerImage = this};
			return DicomAnnotationLayoutFactory.CreateLayout(this);
		}

		/// <summary>
		/// Returns the Instance Number as a string.
		/// </summary>
		/// <returns>The Instance Number as a string.</returns>
		public override string ToString()
		{
			return Frame.ParentImageSop.InstanceNumber.ToString();
		}
	}
}
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
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A DICOM colour <see cref="PresentationImage"/>.
	/// </summary>
	[Cloneable]
	public class DicomColorPresentationImage : ColorPresentationImage, IDicomPresentationImage
	{
		[CloneIgnore]
		private IFrameReference _frameReference;

		[CloneIgnore]
		private CompositeGraphic _dicomGraphics;

		[CloneIgnore]
		private IPatientPresentation _patientPresentation;

		[CloneIgnore]
		private IPatientCoordinateMapping _patientCoordinateMapping;

		/// <summary>
		/// Initializes a new instance of <see cref="DicomColorPresentationImage"/>.
		/// </summary>
		/// <param name="frame">The <see cref="Frame"/> from which to construct the image.</param>
		/// <remarks>
		/// This constructor provides a convenient means of associating a <see cref="Frame"/> with a <see cref="ColorPresentationImage"/>.
		/// </remarks>
		public DicomColorPresentationImage(Frame frame)
			: this(frame.CreateTransientReference()) {}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomColorPresentationImage"/>.
		/// </summary>
		/// <param name="frameReference">A <see cref="IFrameReference">reference</see> to the frame from which to construct the image.</param>
		public DicomColorPresentationImage(IFrameReference frameReference)
			: base(frameReference.Frame.Rows,
			       frameReference.Frame.Columns,
			       frameReference.Frame.NormalizedPixelSpacing.Column,
			       frameReference.Frame.NormalizedPixelSpacing.Row,
			       frameReference.Frame.PixelAspectRatio.Column,
			       frameReference.Frame.PixelAspectRatio.Row,
			       frameReference.Frame.GetNormalizedPixelData)
		{
			_frameReference = frameReference;
			base.PresentationState = PresentationState.DicomDefault;
			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected DicomColorPresentationImage(DicomColorPresentationImage source, ICloningContext context)
			: base(source, context)
		{
			Frame frame = source.Frame;
			_frameReference = frame.CreateTransientReference();
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_dicomGraphics = CollectionUtils.SelectFirst(base.CompositeImageGraphic.Graphics,
			                                             delegate(IGraphic test) { return test.Name == "DICOM"; }) as CompositeGraphic;

			Initialize();
		}

		private void Initialize()
		{
			base.VoiLutsEnabled = DicomPresentationImageSettings.Default.AllowWindowingOnColorImages;

			if (base.ImageGraphic.VoiLutFactory == null)
			{
				base.ImageGraphic.VoiLutFactory = GraphicVoiLutFactory.Create(
					graphic => InitialVoiLutProvider.Instance.GetLut(graphic.ParentPresentationImage));
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

		/// <summary>
		/// Creates a fresh copy of the <see cref="DicomColorPresentationImage"/>.
		/// </summary>
		/// <remarks>
		/// This will instantiate a fresh copy of this <see cref="DicomColorPresentationImage"/>
		/// using the same construction parameters as the original.
		/// </remarks>
		/// <returns></returns>		
		public override IPresentationImage CreateFreshCopy()
		{
			DicomColorPresentationImage image = new DicomColorPresentationImage(Frame);
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
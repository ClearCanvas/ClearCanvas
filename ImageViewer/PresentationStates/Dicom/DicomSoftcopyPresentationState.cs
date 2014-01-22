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
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Base class for DICOM Softcopy Presentation State objects, as defined in DICOM PS 3.3 A.33.
	/// </summary>
	/// <remarks>
	/// <para>
	/// At this time, the only supported softcopy presentation states are the following:
	/// </para>
	/// <list type="table">
	/// <listheader><dicom>Reference</dicom><pstate><see cref="PresentationState">Presentation State</see> Class</pstate><pimage><see cref="IPresentationImage">Presentation Image</see> Class</pimage></listheader>
	/// <item><dicom>PS 3.3 A.33.1</dicom><pstate><see cref="DicomGrayscaleSoftcopyPresentationState">Grayscale Softcopy Presentation State</see></pstate><pimage><see cref="DicomGrayscalePresentationImage"/></pimage></item>
	/// <item><dicom>PS 3.3 A.33.2</dicom><pstate><see cref="DicomColorSoftcopyPresentationState">Color Softcopy Presentation State</see></pstate><pimage><see cref="DicomColorPresentationImage"/></pimage></item>
	/// </list>
	/// </remarks>
	[Cloneable]
	public abstract class DicomSoftcopyPresentationState : PresentationState
	{
		[CloneCopyReference]
		private readonly SopClass _presentationSopClass;

		[CloneIgnore]
		private readonly DicomFile _dicomFile;

		private bool _serialized;
		private int _presentationInstanceNumber;
		private string _presentationSopInstanceUid;
		private DateTime? _presentationSeriesDateTime;
		private int? _presentationSeriesNumber;
		private string _presentationSeriesInstanceUid;
		private string _presentationLabel;
		private string _sourceAETitle;
		private string _stationName;
		private Institution _institution;
		private string _specificCharacterSet = @"ISO_IR 192";

		/// <summary>
		/// Constructs a serialization-capable DICOM softcopy presentation state object.
		/// </summary>
		/// <param name="presentationStateSopClass">The SOP class of this type of softcopy presentation state.</param>
		protected DicomSoftcopyPresentationState(SopClass presentationStateSopClass)
		{
			_presentationSopClass = presentationStateSopClass;
			_dicomFile = new DicomFile();

			_serialized = false;
			_sourceAETitle = string.Empty;
			_stationName = string.Empty;
			_institution = Institution.Empty;
			Manufacturer = "ClearCanvas Inc.";
			ManufacturersModelName = ProductInformation.GetName(true, false);
			DeviceSerialNumber = string.Empty;
			SoftwareVersions = ProductInformation.GetVersion(true, true, true);
			_presentationInstanceNumber = 1;
			_presentationSopInstanceUid = string.Empty;
			_presentationSeriesDateTime = Platform.Time;
			_presentationSeriesNumber = null;
			_presentationSeriesInstanceUid = string.Empty;
			_presentationLabel = "FOR_PRESENTATION";
		}

		/// <summary>
		/// Constructs a deserialization-only DICOM softcopy presentation state object.
		/// </summary>
		/// <param name="psSopClass">The SOP class of this type of softcopy presentation state.</param>
		/// <param name="dicomFile">The presentation state file.</param>
		protected DicomSoftcopyPresentationState(SopClass psSopClass, DicomFile dicomFile)
		{
			if (dicomFile.MediaStorageSopClassUid != psSopClass.Uid)
			{
				string message = string.Format("The specified DICOM file is not of a compatible SOP Class. Expected: {0}; Found: {1}",
				                               psSopClass, SopClass.GetSopClass(dicomFile.MediaStorageSopClassUid));
				throw new ArgumentException(message, "dicomFile");
			}

			_presentationSopClass = psSopClass;
			_dicomFile = dicomFile;

			_serialized = true;
			_sourceAETitle = _dicomFile.SourceApplicationEntityTitle;
			_stationName = _dicomFile.DataSet[DicomTags.StationName].ToString();
			_institution = Institution.GetInstitution(_dicomFile);
			Manufacturer = _dicomFile.DataSet[DicomTags.Manufacturer].ToString();
			ManufacturersModelName = _dicomFile.DataSet[DicomTags.ManufacturersModelName].ToString();
			DeviceSerialNumber = _dicomFile.DataSet[DicomTags.DeviceSerialNumber].ToString();
			SoftwareVersions = _dicomFile.DataSet[DicomTags.SoftwareVersions].ToString();
			_presentationInstanceNumber = _dicomFile.DataSet[DicomTags.InstanceNumber].GetInt32(0, 0);
			_presentationSopInstanceUid = _dicomFile.DataSet[DicomTags.SopInstanceUid].ToString();
			_presentationSeriesDateTime = DateTimeParser.ParseDateAndTime(_dicomFile.DataSet, 0, DicomTags.SeriesDate, DicomTags.SeriesTime);
			_presentationSeriesNumber = GetNullableInt32(_dicomFile.DataSet[DicomTags.SeriesNumber], 0);
			_presentationSeriesInstanceUid = _dicomFile.DataSet[DicomTags.SeriesInstanceUid].ToString();
			_presentationLabel = _dicomFile.DataSet[DicomTags.ContentLabel].ToString();
		}

		/// <summary>
		/// Constructs a deserialization-only DICOM softcopy presentation state object.
		/// </summary>
		/// <param name="psSopClass">The SOP class of this type of softcopy presentation state.</param>
		/// <param name="dataSource">An attribute collection containing the presentation state.</param>
		protected DicomSoftcopyPresentationState(SopClass psSopClass, DicomAttributeCollection dataSource)
			: this(psSopClass, new DicomFile("", CreateMetaInfo(dataSource), dataSource)) {}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected DicomSoftcopyPresentationState(DicomSoftcopyPresentationState source, ICloningContext context)
		{
			context.CloneFields(source, this);
			_dicomFile = new DicomFile("", source._dicomFile.MetaInfo.Copy(), source._dicomFile.DataSet.Copy());
		}

		/// <summary>
		/// Gets the SOP class of this type of softcopy presentation state.
		/// </summary>
		public SopClass PresentationSopClass
		{
			get { return _presentationSopClass; }
		}

		/// <summary>
		/// Gets the SOP class UID of this type of softcopy presentation state.
		/// </summary>
		public string PresentationSopClassUid
		{
			get { return _presentationSopClass.Uid; }
		}

		/// <summary>
		/// Gets the specific character set for the presentation state's data set.
		/// </summary>
		/// <remarks>
		/// <para>
		/// By default, text attribute values will be encoded using UTF-8 Unicode (ISO-IR 192).
		/// If set to NULL or empty, values will be encoded using the default character repertoire (ISO-IR 6).
		/// </para>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public string SpecificCharacterSet
		{
			get { return _specificCharacterSet; }
			set
			{
				AssertNotSerialized();
				_specificCharacterSet = value;
			}
		}

		/// <summary>
		/// Gets or sets the presentation state series instance UID.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public string PresentationSeriesInstanceUid
		{
			get { return _presentationSeriesInstanceUid; }
			set
			{
				AssertNotSerialized();
				_presentationSeriesInstanceUid = value;
			}
		}

		/// <summary>
		/// Gets or sets the presentation state SOP instance UID.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public string PresentationSopInstanceUid
		{
			get { return _presentationSopInstanceUid; }
			set
			{
				AssertNotSerialized();
				_presentationSopInstanceUid = value;
			}
		}

		/// <summary>
		/// Gets or sets the presentation state instance number.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public int PresentationInstanceNumber
		{
			get { return _presentationInstanceNumber; }
			set
			{
				AssertNotSerialized();
				_presentationInstanceNumber = value;
			}
		}

		/// <summary>
		/// Gets or sets the presentation state series number.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public int? PresentationSeriesNumber
		{
			get { return _presentationSeriesNumber; }
			set
			{
				AssertNotSerialized();
				_presentationSeriesNumber = value;
			}
		}

		/// <summary>
		/// Gets or sets the presentation state series date/time.
		/// </summary>
		/// <remarks>
		/// <para>This property affects only the SeriesDate and SeriesTime attributes. The PresentationCreationDateTime is always the timestamp from when the call to <see cref="Serialize"/> is made.</para>
		/// <para>This property may only be set if the presentation state has not yet been serialized to a file.</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public DateTime? PresentationSeriesDateTime
		{
			get { return _presentationSeriesDateTime; }
			set
			{
				AssertNotSerialized();
				_presentationSeriesDateTime = value;
			}
		}

		/// <summary>
		/// Gets or sets the presentation state content label.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public string PresentationContentLabel
		{
			get { return _presentationLabel; }
			set
			{
				AssertNotSerialized();
				_presentationLabel = value;
			}
		}

		/// <summary>
		/// Gets or sets the instance creator's workstation AE title.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public string SourceAETitle
		{
			get { return _sourceAETitle; }
			set
			{
				AssertNotSerialized();
				_sourceAETitle = value;
			}
		}

		/// <summary>
		/// Gets or sets the instance creator's workstation name.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public string StationName
		{
			get { return _stationName; }
			set
			{
				AssertNotSerialized();
				_stationName = value;
			}
		}

		/// <summary>
		/// Gets or sets the instance creator's institution.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		internal Institution Institution
		{
			get { return _institution; }
			set
			{
				AssertNotSerialized();
				_institution = value;
			}
		}

		/// <summary>
		/// Gets or sets the workstation's manufacturer.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public string Manufacturer { get; private set; }

		/// <summary>
		/// Gets or sets the workstation's model name.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public string ManufacturersModelName { get; private set; }

		/// <summary>
		/// Gets or sets the workstation's serial number.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public string DeviceSerialNumber { get; private set; }

		/// <summary>
		/// Gets or sets the workstation's software version numbers (backslash-delimited for multiple values).
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public string SoftwareVersions { get; private set; }

		/// <summary>
		/// Gets the DICOM file containing the presentation state after serialization.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has not yet been serialized to a file.</exception>
		public DicomFile DicomFile
		{
			get
			{
				AssertSerialized();
				return _dicomFile;
			}
		}

		/// <summary>
		/// Gets the underlying DICOM data set.
		/// </summary>
		protected DicomAttributeCollection DataSet
		{
			get { return _dicomFile.DataSet; }
		}

		/// <summary>
		/// Gets or sets a value controlling whether or not annotations in the presentation state are deserialized as interactive objects.
		/// </summary>
		public bool DeserializeInteractiveAnnotations { get; set; }

		private void AssertSerialized()
		{
			const string msg = "This presentation state has not been serialized to a file.";
			if (!_serialized) throw new InvalidOperationException(msg);
		}

		private void AssertNotSerialized()
		{
			const string msg = "This presentation state has already been serialized to a file.";
			if (_serialized) throw new InvalidOperationException(msg);
		}

		/// <summary>
		/// Serializes the presentation state of the given images to the current state object.
		/// </summary>
		/// <param name="images">The images whose presentation states are to be serialized.</param>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public override void Serialize(IEnumerable<IPresentationImage> images)
		{
			AssertNotSerialized();

			// create UIDs if needed now
			PresentationSeriesInstanceUid = CreateUid(PresentationSeriesInstanceUid);
			PresentationSopInstanceUid = CreateUid(PresentationSopInstanceUid);

			_serialized = true;

			var imageList = images.ToList();
			var sopInstanceFactory = new PrototypeSopInstanceFactory {Institution = Institution, StationName = StationName, SpecificCharacterSet = SpecificCharacterSet};
			sopInstanceFactory.InitializeDataSet(imageList.OfType<IImageSopProvider>().First().ImageSop.DataSource, DataSet);

			GeneralSeriesModuleIod generalSeriesModule = new GeneralSeriesModuleIod(DataSet);
			generalSeriesModule.InitializeAttributes();
			generalSeriesModule.SeriesDateTime = PresentationSeriesDateTime;
			generalSeriesModule.SeriesDescription = PresentationContentLabel;
			generalSeriesModule.SeriesInstanceUid = PresentationSeriesInstanceUid;
			generalSeriesModule.SeriesNumber = PresentationSeriesNumber;

			PresentationSeriesModuleIod presentationSeriesModule = new PresentationSeriesModuleIod(DataSet);
			presentationSeriesModule.InitializeAttributes();
			presentationSeriesModule.Modality = Modality.PR;

			SopCommonModuleIod sopCommonModule = new SopCommonModuleIod(DataSet);
			sopCommonModule.SopInstanceUid = PresentationSopInstanceUid;
			sopCommonModule.SopClassUid = PresentationSopClass.Uid;

			PresentationStateIdentificationModuleIod presentationStateIdentificationModule = new PresentationStateIdentificationModuleIod(DataSet);
			presentationStateIdentificationModule.InitializeAttributes();
			presentationStateIdentificationModule.ContentLabel = PresentationContentLabel;
			presentationStateIdentificationModule.InstanceNumber = PresentationInstanceNumber;
			presentationStateIdentificationModule.PresentationCreationDateTime = Platform.Time;

			PerformSerialization(imageList);

			_dicomFile.SourceApplicationEntityTitle = SourceAETitle;
			_dicomFile.MediaStorageSopClassUid = PresentationSopClassUid;
			_dicomFile.MediaStorageSopInstanceUid = PresentationSopInstanceUid;
		}

		/// <summary>
		/// Deserializes the presentation state from the current state object into the given images.
		/// </summary>
		/// <param name="images">The images to which the presentation state is to be deserialized.</param>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has not yet been serialized to a file.</exception>
		public override void Deserialize(IEnumerable<IPresentationImage> images)
		{
			AssertSerialized();
			PerformDeserialization(images.ToList());
		}

		/// <summary>
		/// Clears the presentation states of the given images.
		/// </summary>
		/// <remarks>
		/// Whether all presentation state concepts defined by the implementation are cleared, or only the
		/// objects actually defined by this particular state object are cleared, is up to the implementation.
		/// </remarks>
		/// <param name="image">The images whose presentation states are to be cleared.</param>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has not yet been serialized to a file.</exception>
		public override void Clear(IEnumerable<IPresentationImage> image)
		{
			AssertSerialized();
		}

		/// <summary>
		/// Called by the base <see cref="DicomSoftcopyPresentationState"/> to invoke presentation state serialization of the specified images.
		/// </summary>
		/// <param name="images">The images whose presentation states are to be serialized.</param>
		protected abstract void PerformSerialization(IList<IPresentationImage> images);

		/// <summary>
		/// Called by the base <see cref="DicomSoftcopyPresentationState"/> to invoke presentation state deserialization to the specified images.
		/// </summary>
		/// <param name="images">The images to which the presentation state is to be deserialized.</param>
		protected abstract void PerformDeserialization(IList<IPresentationImage> images);

		#region Protected Helper Methods

		private static int? GetNullableInt32(DicomAttribute attribute, int index)
		{
			int result;
			if (attribute.TryGetInt32(index, out result))
				return result;
			return null;
		}

		private static string CreateUid(string uidHint)
		{
			if (string.IsNullOrEmpty(uidHint))
				return DicomUid.GenerateUid().UID;
			return uidHint;
		}

		private static DicomAttributeCollection CreateMetaInfo(DicomAttributeCollection dataset)
		{
			DicomAttributeCollection metainfo = new DicomAttributeCollection();
			metainfo[DicomTags.MediaStorageSopClassUid].SetStringValue(dataset[DicomTags.SopClassUid].ToString());
			metainfo[DicomTags.MediaStorageSopInstanceUid].SetStringValue(dataset[DicomTags.SopInstanceUid].ToString());
			return metainfo;
		}

		/// <summary>
		/// Creates a <see cref="ImageSopInstanceReferenceMacro"/> to the given <see cref="ImageSop"/>.
		/// </summary>
		/// <param name="sop">The image SOP to which a reference is to be constructed.</param>
		/// <returns>An image SOP instance reference macro item.</returns>
		protected static ImageSopInstanceReferenceMacro CreateImageSopInstanceReference(ImageSop sop)
		{
			ImageSopInstanceReferenceMacro imageReference = new ImageSopInstanceReferenceMacro();
			imageReference.ReferencedSopClassUid = sop.SopClassUid;
			imageReference.ReferencedSopInstanceUid = sop.SopInstanceUid;
			return imageReference;
		}

		/// <summary>
		/// Creates a <see cref="ImageSopInstanceReferenceMacro"/> to the given <see cref="Frame"/>.
		/// </summary>
		/// <param name="frame">The image SOP frame to which a reference is to be constructed.</param>
		/// <returns>An image SOP instance reference macro item.</returns>
		protected static ImageSopInstanceReferenceMacro CreateImageSopInstanceReference(Frame frame)
		{
			ImageSopInstanceReferenceMacro imageReference = new ImageSopInstanceReferenceMacro();
			imageReference.ReferencedSopClassUid = frame.ParentImageSop.SopClassUid;
			imageReference.ReferencedSopInstanceUid = frame.SopInstanceUid;
			imageReference.ReferencedFrameNumber.SetInt32(0, frame.FrameNumber);
			return imageReference;
		}

		#endregion

		#region Static Helpers

		/// <summary>
		/// Represents the callback method to initialize the instance properties of a <see cref="DicomSoftcopyPresentationState"/>.
		/// </summary>
		/// <param name="presentationState">A new, uninitialized presentation state SOP instance.</param>
		public delegate void InitializeDicomSoftcopyPresentationStateCallback(DicomSoftcopyPresentationState presentationState);

		private static void DefaultInitializeDicomSoftcopyPresentationStateCallback(DicomSoftcopyPresentationState presentationState) {}

		/// <summary>
		/// Creates a <see cref="DicomSoftcopyPresentationState"/> for a given image.
		/// </summary>
		/// <param name="image">The image for which the presentation state should be created.</param>
		/// <returns>One of the derived <see cref="DicomSoftcopyPresentationState"/> classes, depending on the type of the <paramref name="image"/>.</returns>
		/// <exception cref="ArgumentException">Thrown if softcopy presentation states for the type of the given <paramref name="image"/> are not supported.</exception>
		/// <seealso cref="DicomSoftcopyPresentationState"/>
		public static DicomSoftcopyPresentationState Create(IPresentationImage image)
		{
			return Create(image, null);
		}

		/// <summary>
		/// Creates a <see cref="DicomSoftcopyPresentationState"/> for a given image.
		/// </summary>
		/// <param name="image">The image for which the presentation state should be created.</param>
		/// <param name="callback">A callback method that initializes the instance properties of the created <see cref="DicomSoftcopyPresentationState"/>.</param>
		/// <returns>One of the derived <see cref="DicomSoftcopyPresentationState"/> classes, depending on the type of the <paramref name="image"/>.</returns>
		/// <exception cref="ArgumentException">Thrown if softcopy presentation states for the type of the given <paramref name="image"/> are not supported.</exception>
		/// <seealso cref="DicomSoftcopyPresentationState"/>
		public static DicomSoftcopyPresentationState Create(IPresentationImage image, InitializeDicomSoftcopyPresentationStateCallback callback)
		{
			callback = callback ?? DefaultInitializeDicomSoftcopyPresentationStateCallback;

			if (image is DicomGrayscalePresentationImage)
			{
				DicomGrayscaleSoftcopyPresentationState grayscaleSoftcopyPresentationState = new DicomGrayscaleSoftcopyPresentationState();
				callback.Invoke(grayscaleSoftcopyPresentationState);
				grayscaleSoftcopyPresentationState.Serialize(image);
				return grayscaleSoftcopyPresentationState;
			}
			else if (image is DicomColorPresentationImage)
			{
				DicomColorSoftcopyPresentationState colorSoftcopyPresentationState = new DicomColorSoftcopyPresentationState();
				callback.Invoke(colorSoftcopyPresentationState);
				colorSoftcopyPresentationState.Serialize(image);
				return colorSoftcopyPresentationState;
			}
			else
			{
				throw new ArgumentException("DICOM presentation state serialization is not supported for that type of image.");
			}
		}

		/// <summary>
		/// Creates a minimal number of <see cref="DicomSoftcopyPresentationState"/>s for the given images.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Presentation state instances can contain information for multiple images, but the images must all be of the same type,
		/// and contain non-conflicting presentation state information. This method creates a minimal number of presentation
		/// state objects for the collection of given images.
		/// </para>
		/// </remarks>
		/// <param name="images">The images for which presentation states are to be created.</param>
		/// <returns>A dictionary mapping of presentation images to its associated presentation state instance.</returns>
		/// <exception cref="ArgumentException">Thrown if softcopy presentation states are not supported for the type of any one of the given <paramref name="images"/>.</exception>
		/// <seealso cref="DicomSoftcopyPresentationState"/>
		public static IDictionary<IPresentationImage, DicomSoftcopyPresentationState> Create(IEnumerable<IPresentationImage> images)
		{
			return Create(images, null);
		}

		/// <summary>
		/// Creates a minimal number of <see cref="DicomSoftcopyPresentationState"/>s for the given images.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Presentation state instances can contain information for multiple images, but the images must all be of the same type,
		/// and contain non-conflicting presentation state information. This method creates a minimal number of presentation
		/// state objects for the collection of given images.
		/// </para>
		/// </remarks>
		/// <param name="images">The images for which presentation states are to be created.</param>
		/// <param name="callback">A callback method that initializes the instance properties of the created <see cref="DicomSoftcopyPresentationState"/>s.</param>
		/// <returns>A dictionary mapping of presentation images to its associated presentation state instance.</returns>
		/// <exception cref="ArgumentException">Thrown if softcopy presentation states are not supported for the type of any one of the given <paramref name="images"/>.</exception>
		/// <seealso cref="DicomSoftcopyPresentationState"/>
		public static IDictionary<IPresentationImage, DicomSoftcopyPresentationState> Create(IEnumerable<IPresentationImage> images, InitializeDicomSoftcopyPresentationStateCallback callback)
		{
			callback = callback ?? DefaultInitializeDicomSoftcopyPresentationStateCallback;

			List<IPresentationImage> grayscaleImages = new List<IPresentationImage>();
			List<IPresentationImage> colorImages = new List<IPresentationImage>();

			foreach (IPresentationImage image in images)
			{
				if (image is DicomGrayscalePresentationImage)
				{
					grayscaleImages.Add(image);
				}
				else if (image is DicomColorPresentationImage)
				{
					colorImages.Add(image);
				}
				else
				{
					throw new ArgumentException("DICOM presentation state serialization is not supported for that type of image.");
				}
			}

			Dictionary<IPresentationImage, DicomSoftcopyPresentationState> presentationStates = new Dictionary<IPresentationImage, DicomSoftcopyPresentationState>();
			if (grayscaleImages.Count > 0)
			{
				DicomGrayscaleSoftcopyPresentationState grayscaleSoftcopyPresentationState = new DicomGrayscaleSoftcopyPresentationState();
				callback.Invoke(grayscaleSoftcopyPresentationState);
				grayscaleSoftcopyPresentationState.Serialize(grayscaleImages);
				foreach (IPresentationImage image in grayscaleImages)
				{
					presentationStates.Add(image, grayscaleSoftcopyPresentationState);
				}
			}
			if (colorImages.Count > 0)
			{
				DicomColorSoftcopyPresentationState colorSoftcopyPresentationState = new DicomColorSoftcopyPresentationState();
				callback.Invoke(colorSoftcopyPresentationState);
				colorSoftcopyPresentationState.Serialize(colorImages);
				foreach (IPresentationImage image in colorImages)
				{
					presentationStates.Add(image, colorSoftcopyPresentationState);
				}
			}
			return presentationStates;
		}

		/// <summary>
		/// Loads a presentation state from a file.
		/// </summary>
		/// <param name="presentationState">The DICOM file containing the presentation state SOP instance.</param>
		/// <returns>A <see cref="DicomSoftcopyPresentationState"/> object of the correct type.</returns>
		/// <exception cref="ArgumentException">Thrown if the given <paramref name="presentationState"/> is not a supported presentation state SOP class.</exception>
		/// <seealso cref="DicomSoftcopyPresentationState"/>
		public static DicomSoftcopyPresentationState Load(DicomFile presentationState)
		{
			if (presentationState.MediaStorageSopClassUid == DicomGrayscaleSoftcopyPresentationState.SopClass.Uid)
			{
				return new DicomGrayscaleSoftcopyPresentationState(presentationState);
			}
			else if (presentationState.MediaStorageSopClassUid == DicomColorSoftcopyPresentationState.SopClass.Uid)
			{
				return new DicomColorSoftcopyPresentationState(presentationState);
			}
			else
			{
				throw new ArgumentException("DICOM presentation state deserialization is not supported for that SOP class.");
			}
		}

		/// <summary>
		/// Loads a number of presentation states from multiple files.
		/// </summary>
		/// <param name="presentationStates">The DICOM files containing the presentation state SOP instances.</param>
		/// <returns>An enumeration of <see cref="DicomSoftcopyPresentationState"/> objects.</returns>
		/// <exception cref="ArgumentException">Thrown if one of the given <paramref name="presentationStates"/> is not a supported presentation state SOP class.</exception>
		/// <seealso cref="DicomSoftcopyPresentationState"/>
		public static IEnumerable<DicomSoftcopyPresentationState> Load(IEnumerable<DicomFile> presentationStates)
		{
			foreach (DicomFile presentationState in presentationStates)
			{
				if (presentationState.MediaStorageSopClassUid == DicomGrayscaleSoftcopyPresentationState.SopClass.Uid)
				{
					yield return new DicomGrayscaleSoftcopyPresentationState(presentationState);
				}
				else if (presentationState.MediaStorageSopClassUid == DicomColorSoftcopyPresentationState.SopClass.Uid)
				{
					yield return new DicomColorSoftcopyPresentationState(presentationState);
				}
			}
		}

		/// <summary>
		/// Loads a presentation state from a data set.
		/// </summary>
		/// <param name="presentationState">The data set containing the presentation state SOP instance.</param>
		/// <returns>A <see cref="DicomSoftcopyPresentationState"/> object of the correct type.</returns>
		/// <exception cref="ArgumentException">Thrown if the given <paramref name="presentationState"/> is not a supported presentation state SOP class.</exception>
		/// <seealso cref="DicomSoftcopyPresentationState"/>
		public static DicomSoftcopyPresentationState Load(IDicomAttributeProvider presentationState)
		{
			if (presentationState[DicomTags.SopClassUid].ToString() == DicomGrayscaleSoftcopyPresentationState.SopClass.Uid)
			{
				return new DicomGrayscaleSoftcopyPresentationState(presentationState);
			}
			else if (presentationState[DicomTags.SopClassUid].ToString() == DicomColorSoftcopyPresentationState.SopClass.Uid)
			{
				return new DicomColorSoftcopyPresentationState(presentationState);
			}
			else
			{
				throw new ArgumentException("DICOM presentation state deserialization is not supported for that SOP class.");
			}
		}

		/// <summary>
		/// Loads a number of presentation states from multiple data sets.
		/// </summary>
		/// <param name="presentationStates">The data sets containing the presentation state SOP instances.</param>
		/// <returns>An enumeration of <see cref="DicomSoftcopyPresentationState"/> objects.</returns>
		/// <exception cref="ArgumentException">Thrown if one of the given <paramref name="presentationStates"/> is not a supported presentation state SOP class.</exception>
		/// <seealso cref="DicomSoftcopyPresentationState"/>
		public static IEnumerable<DicomSoftcopyPresentationState> Load(IEnumerable<IDicomAttributeProvider> presentationStates)
		{
			foreach (IDicomAttributeProvider presentationState in presentationStates)
			{
				if (presentationState[DicomTags.SopClassUid].ToString() == DicomGrayscaleSoftcopyPresentationState.SopClass.Uid)
				{
					yield return new DicomGrayscaleSoftcopyPresentationState(presentationState);
				}
				else if (presentationState[DicomTags.SopClassUid].ToString() == DicomColorSoftcopyPresentationState.SopClass.Uid)
				{
					yield return new DicomColorSoftcopyPresentationState(presentationState);
				}
			}
		}

		/// <summary>
		/// Tests to see if softcopy presentation states are supported for the type of the given image.
		/// </summary>
		/// <param name="image">The image whose support for softcopy presentation states is to be checked.</param>
		/// <returns>True if softcopy presentation states are supported for the type of the given image; False otherwise.</returns>
		/// <seealso cref="DicomSoftcopyPresentationState"/>
		public static bool IsSupported(IPresentationImage image)
		{
			return (image is DicomGrayscalePresentationImage) || (image is DicomColorPresentationImage);
		}

		#endregion
	}
}
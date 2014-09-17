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
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.ImageViewer.KeyObjects;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines the factory methods for creating <see cref="IPresentationImage"/>s.
	/// </summary>
	public interface IPresentationImageFactory
	{
		/// <summary>
		/// Sets the <see cref="StudyTree"/> to be used by the <see cref="IPresentationImageFactory"/> when resolving referenced SOPs.
		/// </summary>
		/// <param name="studyTree">The <see cref="StudyTree"/> to be used for resolving referenced SOPs.</param>
		void SetStudyTree(StudyTree studyTree);

		/// <summary>
		/// Creates the presentation images for a given image SOP.
		/// </summary>
		/// <param name="sop">The image SOP from which presentation images are to be created.</param>
		/// <returns>A list of created presentation images.</returns>
		List<IPresentationImage> CreateImages(Sop sop);

		/// <summary>
		/// Creates the presentation image for a given image frame.
		/// </summary>
		/// <param name="frame">The image frame from which a presentation image is to be created.</param>
		/// <returns>The created presentation image.</returns>
		IPresentationImage CreateImage(Frame frame);
	}

	/// <summary>
	/// A factory class which creates <see cref="IPresentationImage"/>s.
	/// </summary>
	public class PresentationImageFactory : IPresentationImageFactory
	{
		private static readonly PresentationImageFactory _defaultInstance = new PresentationImageFactory();

		private StudyTree _studyTree;

		/// <summary>
		/// Constructs a <see cref="PresentationImageFactory"/>.
		/// </summary>
		public PresentationImageFactory() {}

		/// <summary>
		/// Constructs a <see cref="PresentationImageFactory"/>.
		/// </summary>
		public PresentationImageFactory(StudyTree studyTree)
		{
			_studyTree = studyTree;
		}

		public PresentationState DefaultPresentationState { get; set; }

		/// <summary>
		/// Gets the <see cref="StudyTree"/> used by the factory to resolve referenced SOPs.
		/// </summary>
		protected StudyTree StudyTree
		{
			get { return _studyTree; }
		}

		#region IPresentationImageFactory Members

		void IPresentationImageFactory.SetStudyTree(StudyTree studyTree)
		{
			_studyTree = studyTree;
		}

		IPresentationImage IPresentationImageFactory.CreateImage(Frame frame)
		{
			return Create(frame);
		}

		#endregion

		/// <summary>
		/// Creates the presentation image for a given image frame.
		/// </summary>
		/// <param name="frame">The image frame from which a presentation image is to be created.</param>
		/// <returns>The created presentation image.</returns>
		public virtual IPresentationImage CreateImage(Frame frame)
		{
			if (frame.PhotometricInterpretation == PhotometricInterpretation.Unknown)
				throw new Exception("Photometric interpretation is unknown.");

			IDicomPresentationImage image;

			if (!frame.PhotometricInterpretation.IsColor)
				image = new DicomGrayscalePresentationImage(frame);
			else
				image = new DicomColorPresentationImage(frame);

			if (image.PresentationState == null || Equals(image.PresentationState, PresentationState.DicomDefault))
				image.PresentationState = DefaultPresentationState;

			return image;
		}

		/// <summary>
		/// Creates the presentation images for a given image SOP.
		/// </summary>
		/// <param name="imageSop">The image SOP from which presentation images are to be created.</param>
		/// <returns>A list of created presentation images.</returns>
		protected virtual List<IPresentationImage> CreateImages(ImageSop imageSop)
		{
			return CollectionUtils.Map(imageSop.Frames, (Frame frame) => CreateImage(frame));
		}

		/// <summary>
		/// Creates the presentation images for a given image SOP.
		/// </summary>
		/// <param name="sop">The image SOP from which presentation images are to be created.</param>
		/// <returns>A list of created presentation images.</returns>
		public virtual List<IPresentationImage> CreateImages(Sop sop)
		{
			if (sop.IsImage)
				return CreateImages((ImageSop) sop);

			if (sop.SopClassUid == SopClass.KeyObjectSelectionDocumentStorageUid)
				return CreateImages(new KeyObjectSelectionDocumentIod(sop));

			return new List<IPresentationImage>();
		}

		/// <summary>
		/// Creates the presentation images for a given key object selection document.
		/// </summary>
		/// <param name="keyObjectDocument">The key object selection document from which presentation images are to be created.</param>
		/// <returns>A list of created presentation images.</returns>
		protected virtual List<IPresentationImage> CreateImages(KeyObjectSelectionDocumentIod keyObjectDocument)
		{
			List<IPresentationImage> images = new List<IPresentationImage>();

			if (_studyTree == null)
			{
				Platform.Log(LogLevel.Warn, "Key object document cannot be used to create images because there is no study tree to build from.");
			}
			else
			{
				try
				{
					IList<IKeyObjectContentItem> content = new KeyImageDeserializer(keyObjectDocument).Deserialize();
					var evidence = new HierarchicalSopInstanceReferenceDictionary(keyObjectDocument.KeyObjectDocument.CurrentRequestedProcedureEvidenceSequence);
					foreach (IKeyObjectContentItem item in content)
					{
						try
						{
							var contentItem = item as KeyImageContentItem;
							if (contentItem != null)
								images.AddRange(CreateImages(contentItem, evidence));
							else
								Platform.Log(LogLevel.Warn, "Unsupported key object content value type");
						}
						catch (Exception ex)
						{
							// catches KO errors with individual content items, allowing as many items as possible to be created
							Platform.Log(LogLevel.Warn, ex, SR.MessageKeyObjectDeserializeFailure);
						}
					}
				}
				catch (Exception ex)
				{
					// catches KO errors with the entire document
					Platform.Log(LogLevel.Warn, ex, SR.MessageKeyObjectDeserializeFailure);
				}
			}

			// return a KO error placeholder, otherwise the sop will be treated later as an unsupported sop class
			if (images.Count == 0 && keyObjectDocument.DataSource is Sop)
				images.Add(PlaceholderDisplaySetFactory.CreatePlaceholderImage((Sop) keyObjectDocument.DataSource, SR.MessageKeyObjectDeserializeFailure));
			return images;
		}

		protected virtual List<IPresentationImage> CreateImages(KeyImageContentItem item, HierarchicalSopInstanceReferenceDictionary currentRequestedProcedureEvidence)
		{
			List<IPresentationImage> images = new List<IPresentationImage>();

			var imageRef = LookupKeyImageEvidence(item.ReferencedImageSopInstanceUid, item.FrameNumber, currentRequestedProcedureEvidence);
			var presentationStateRef = LookupPresentationStateEvidence(item.PresentationStateSopInstanceUid, currentRequestedProcedureEvidence);

			var imageSop = FindSop<ImageSop>(imageRef.SopInstanceUid, imageRef.StudyInstanceUid);
			if (imageSop != null)
			{
				int frameNumber = imageRef.FrameNumber.GetValueOrDefault(-1);
				if (item.FrameNumber.HasValue)
				{
					// FramesCollection is a 1-based index!!!
					if (frameNumber > 0 && frameNumber <= imageSop.Frames.Count)
					{
						images.Add(Create(imageSop.Frames[frameNumber]));
					}
					else
					{
						Platform.Log(LogLevel.Error, "The referenced key image {0} does not have a frame {1} (referenced in Key Object Selection {2})", item.ReferencedImageSopInstanceUid, frameNumber, item.Source.SopCommon.SopInstanceUid);
						images.Add(new KeyObjectPlaceholderImage(imageRef, presentationStateRef, SR.MessageReferencedKeyImageFrameNotFound));
					}
				}
				else
				{
					images.AddRange(imageSop.Frames.Select(Create));
				}

				var presentationStateSop = FindSop<Sop>(presentationStateRef.SopInstanceUid, presentationStateRef.StudyInstanceUid);
				if (presentationStateSop != null)
				{
					foreach (IPresentationImage image in images)
					{
						if (image is IPresentationStateProvider)
						{
							try
							{
								IPresentationStateProvider presentationStateProvider = (IPresentationStateProvider) image;
								presentationStateProvider.PresentationState = DicomSoftcopyPresentationState.Load(presentationStateSop);
							}
							catch (Exception ex)
							{
								Platform.Log(LogLevel.Warn, ex, SR.MessagePresentationStateReadFailure);
							}
						}
					}
				}
			}
			else
			{
				Platform.Log(LogLevel.Warn, "The referenced key image {0} is not loaded as part of the current study (referenced in Key Object Selection {1})", item.ReferencedImageSopInstanceUid, item.Source.SopCommon.SopInstanceUid);
				images.Add(new KeyObjectPlaceholderImage(imageRef, presentationStateRef, SR.MessageReferencedKeyImageFromOtherStudy));
			}

			return images;
		}

		/// <summary>
		/// Creates an appropriate subclass of <see cref="BasicPresentationImage"/>
		/// for each <see cref="Frame"/> in the input <see cref="ImageSop"/>.
		/// </summary>
		public static List<IPresentationImage> Create(ImageSop imageSop)
		{
			return _defaultInstance.CreateImages(imageSop);
		}

		/// <summary>
		/// Creates an appropriate subclass of <see cref="BasicPresentationImage"/>
		/// based on the <see cref="Frame"/>'s photometric interpretation.
		/// </summary>
		public static IPresentationImage Create(Frame frame)
		{
			return _defaultInstance.CreateImage(frame);
		}

		#region Private KO Helpers

		private KeyImageReference LookupKeyImageEvidence(string sopInstanceUid, int? frameNumber, HierarchicalSopInstanceReferenceDictionary evidenceDictionary)
		{
			if (string.IsNullOrEmpty(sopInstanceUid))
				return null;

			var result = evidenceDictionary.FirstOrDefault(e => e.SopInstanceUid == sopInstanceUid);
			return new KeyImageReference(result.StudyInstanceUid, result.SeriesInstanceUid, result.SopClassUid, result.SopInstanceUid, frameNumber);
		}

		private PresentationStateReference LookupPresentationStateEvidence(string sopInstanceUid, HierarchicalSopInstanceReferenceDictionary evidenceDictionary)
		{
			if (string.IsNullOrEmpty(sopInstanceUid))
				return null;

			var result = evidenceDictionary.FirstOrDefault(e => e.SopInstanceUid == sopInstanceUid);
			return new PresentationStateReference(result.StudyInstanceUid, result.SeriesInstanceUid, result.SopClassUid, result.SopInstanceUid);
		}

		private T FindSop<T>(string sopInstanceUid, string studyInstanceUid)
			where T : Sop
		{
			if (string.IsNullOrEmpty(sopInstanceUid))
				return null;

			string sameStudyUid = studyInstanceUid;
			Study sameStudy = _studyTree.GetStudy(sameStudyUid);

			return sameStudy != null ? sameStudy.Series.Select(series => series.Sops[sopInstanceUid]).OfType<T>().FirstOrDefault() : null;
		}

		#endregion
	}
}
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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Tests
{
	/// <summary>
	/// Generates <see cref="ImageSop"/>s and <see cref="IDisplaySet"/>s for testing of image fusion functionality.
	/// </summary>
	public class TestDisplaySetGenerator : IDisposable
	{
		private readonly IList<ISopDataSource> _baseSopDataSources;
		private readonly IList<ISopDataSource> _overlaySopDataSources;
		private IList<ImageSop> _baseSops;
		private IList<ImageSop> _overlaySops;

		public TestDisplaySetGenerator(IEnumerable<ISopDataSource> baseSopDataSources, IEnumerable<ISopDataSource> overlaySopDataSources)
		{
			_baseSopDataSources = new List<ISopDataSource>(baseSopDataSources).AsReadOnly();
			_overlaySopDataSources = new List<ISopDataSource>(overlaySopDataSources).AsReadOnly();
		}

		public void Dispose()
		{
			if (_baseSops != null)
			{
				Dispose(_baseSops);
				_baseSops = null;
			}

			if (_overlaySops != null)
			{
				Dispose(_overlaySops);
				_overlaySops = null;
			}
		}

		public IList<ImageSop> BaseSops
		{
			get
			{
				if (_baseSops == null)
				{
					_baseSops = new List<ImageSop>(GetSops(_baseSopDataSources)).AsReadOnly();
				}
				return _baseSops;
			}
		}

		public IList<ImageSop> OverlaySops
		{
			get
			{
				if (_overlaySops == null)
				{
					_overlaySops = new List<ImageSop>(GetSops(_overlaySopDataSources)).AsReadOnly();
				}
				return _overlaySops;
			}
		}

		public IDisplaySet CreateBaseDisplaySet()
		{
			return CreateDisplaySet(this.BaseSops);
		}

		public IDisplaySet CreateOverlayDisplaySet()
		{
			return CreateDisplaySet(this.OverlaySops);
		}

		public IDisplaySet CreateFusionDisplaySet()
		{
			return CreateFusionDisplaySet(this.BaseSops, this.OverlaySops);
		}

		private static DisplaySet CreateFusionDisplaySet(IEnumerable<ImageSop> baseSops, IEnumerable<ImageSop> overlaySops)
		{
			var descriptor = new PETFusionDisplaySetDescriptor(
				GetSeriesIdentifier(CollectionUtils.FirstElement(baseSops).DataSource),
				GetSeriesIdentifier(CollectionUtils.FirstElement(overlaySops).DataSource),
				false);
			var displaySet = new DisplaySet(descriptor);

			using (var fusionOverlayData = new FusionOverlayData(GetFrames(overlaySops)))
			{
				int bitDepth = fusionOverlayData.Frames[0].ParentImageSop.DataSource[DicomTags.BitsStored].GetInt32(0, 16);
				foreach (var baseFrame in GetFrames(baseSops))
				{
					using (var fusionOverlaySlice = fusionOverlayData.CreateOverlaySlice(baseFrame))
					{
						var fus = new FusionPresentationImage(baseFrame, fusionOverlaySlice);
						fus.BaseVoiLutManager.InstallVoiLut(new IdentityVoiLinearLut());
						fus.OverlayVoiLutManager.InstallVoiLut(new IdentityVoiLinearLut());
						displaySet.PresentationImages.Add(fus);
					}
				}
			}

			return displaySet;
		}

		private static DisplaySet CreateDisplaySet(IEnumerable<ImageSop> sops)
		{
			var descriptor = new XDisplaySetDescriptor(CollectionUtils.FirstElement(sops).DataSource);
			var displaySet = new DisplaySet(descriptor);
			foreach (var sop in sops)
			{
				foreach (var image in PresentationImageFactory.Create(sop))
				{
					if (image is IVoiLutProvider)
						((IVoiLutProvider) image).VoiLutManager.InstallVoiLut(new IdentityVoiLinearLut());
					displaySet.PresentationImages.Add(image);
				}
			}
			return displaySet;
		}

		private static IEnumerable<ImageSop> GetSops(IEnumerable<ISopDataSource> sopDataSources)
		{
			foreach (var sopDataSource in sopDataSources)
			{
				yield return new ImageSop(sopDataSource);
			}
		}

		private static ISeriesIdentifier GetSeriesIdentifier(IDicomAttributeProvider dicomAttributeProvider)
		{
			var identifier = new SeriesIdentifier();
			identifier.Modality = dicomAttributeProvider[DicomTags.Modality].ToString();
			identifier.SeriesDescription = dicomAttributeProvider[DicomTags.SeriesDescription].ToString();
			identifier.SeriesInstanceUid = dicomAttributeProvider[DicomTags.SeriesInstanceUid].ToString();
			identifier.SeriesNumber = dicomAttributeProvider[DicomTags.SeriesNumber].GetInt32(0, 0);
			identifier.StudyInstanceUid = dicomAttributeProvider[DicomTags.StudyInstanceUid].ToString();
			return identifier;
		}

		private static List<Frame> GetFrames(IEnumerable<ImageSop> sops)
		{
			List<Frame> list = new List<Frame>();
			foreach (var sop in sops)
				list.AddRange(sop.Frames);
			return list;
		}

		private static void Dispose<T>(IEnumerable<T> list) where T : IDisposable
		{
			foreach (var item in list)
				item.Dispose();
		}

		[Cloneable]
		private class XDisplaySetDescriptor : DisplaySetDescriptor
		{
			[CloneCopyReference]
			private ISeriesIdentifier _seriesIdentifier;

			public XDisplaySetDescriptor(IDicomAttributeProvider dicomAttributeProvider)
			{
				_seriesIdentifier = GetSeriesIdentifier(dicomAttributeProvider);
			}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			/// <param name="source">The source object from which to clone.</param>
			/// <param name="context">The cloning context object.</param>
			protected XDisplaySetDescriptor(XDisplaySetDescriptor source, ICloningContext context)
			{
				context.CloneFields(source, this);
			}

			public override string Name
			{
				get { return _seriesIdentifier.SeriesDescription; }
				set { }
			}

			public override string Description
			{
				get { return _seriesIdentifier.SeriesDescription; }
				set { }
			}

			public override int Number
			{
				get { return _seriesIdentifier.SeriesNumber.GetValueOrDefault(0); }
				set { }
			}

			public override string Uid
			{
				get { return _seriesIdentifier.SeriesInstanceUid; }
				set { }
			}
		}
	}
}

#endif
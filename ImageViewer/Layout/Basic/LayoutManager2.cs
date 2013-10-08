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
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	partial class LayoutManager
	{
		internal class ImageSetFiller
		{
			private readonly Dictionary<string, DisplaySetFactory> _modalityDisplaySetFactories = new Dictionary<string, DisplaySetFactory>();
			private readonly ModalityDisplaySetFactory _modalityDisplaySetFactory;
			private readonly IDisplaySetFactory _defaultDisplaySetFactory;

			private readonly IDisplaySetCreationOptions _displaySetCreationOptions;

			public ImageSetFiller(StudyTree studyTree, IDisplaySetCreationOptions displaySetCreationOptions)
			{
				_displaySetCreationOptions = displaySetCreationOptions;
				foreach (IModalityDisplaySetCreationOptions option in displaySetCreationOptions)
					_modalityDisplaySetFactories[option.Modality] = new DisplaySetFactory(option);

				_modalityDisplaySetFactory = new ModalityDisplaySetFactory();
				_defaultDisplaySetFactory = new BasicDisplaySetFactory();

				foreach (IDisplaySetFactory displaySetFactory in _modalityDisplaySetFactories.Values)
					displaySetFactory.SetStudyTree(studyTree);

				_modalityDisplaySetFactory.SetStudyTree(studyTree);
				_defaultDisplaySetFactory.SetStudyTree(studyTree);
			}

			public void AddMultiSeriesDisplaySets(IImageSet imageSet, Study study)
			{
				var multiSeriesModalities = (from option in _displaySetCreationOptions
				                             where option.CreateAllImagesDisplaySet
				                             join series in study.Series
				                             	on option.Modality equals series.Modality
				                             select option.Modality).Distinct();

				foreach (var modality in multiSeriesModalities)
				{
					//Add all the "all images" per modality display sets for the entire study at the top of the list.
					var displaySet = _modalityDisplaySetFactory.CreateDisplaySet(study, modality);
					if (displaySet != null)
						imageSet.DisplaySets.Add(displaySet);
				}
			}

			public void AddSeriesDisplaySets(IImageSet imageSet, Series series)
			{
				var factory = GetDisplaySetFactory(series.Modality);
				if (factory == null)
				{
					factory = _defaultDisplaySetFactory;
				}
				else
				{
					var modalityDisplaySetExists =
						null != (from displaySet in imageSet.DisplaySets
						         where displaySet.Descriptor is ModalityDisplaySetDescriptor
						               && ((ModalityDisplaySetDescriptor) displaySet.Descriptor).Modality == series.Modality
						         select displaySet).FirstOrDefault();

					//Tell the factory whether we've created an "all images" display set containing
					//all the images in the entire study for the given modality.

					((DisplaySetFactory) factory).ModalityDisplaySetExists = modalityDisplaySetExists;
				}

				//Add all the display sets derived from single series next.
				List<IDisplaySet> displaySets = factory.CreateDisplaySets(series);
				foreach (IDisplaySet displaySet in displaySets)
					imageSet.DisplaySets.Add(displaySet);
			}

			private IDisplaySetFactory GetDisplaySetFactory(string modality)
			{
				modality = modality ?? String.Empty;

				DisplaySetFactory factory;
				if (_modalityDisplaySetFactories.TryGetValue(modality, out factory))
					return factory;

				return null;
			}
		}

		internal class DisplaySetFactory : ImageViewer.DisplaySetFactory
		{
			private readonly IModalityDisplaySetCreationOptions _creationOptions;

			private readonly MREchoDisplaySetFactory _echoFactory;
			private readonly MultiFrameStackDisplaySetFactory _multiFrameStackFactory;
			private readonly MixedMultiFrameDisplaySetFactory _mixedMultiFrameFactory;
			private readonly BasicDisplaySetFactory _basicFactory;
			private readonly IDisplaySetFactory _placeholderDisplaySetFactory;

			private readonly IList<IDisplaySetFactory> _externalFactories;

			public DisplaySetFactory(IModalityDisplaySetCreationOptions creationOptions)
			{
				_creationOptions = creationOptions;

				PresentationState defaultPresentationState = new DicomPresentationState {ShowGrayscaleInverted = creationOptions.ShowGrayscaleInverted};

				var imageFactory = (PresentationImageFactory) PresentationImageFactory;
				imageFactory.DefaultPresentationState = defaultPresentationState;

				_basicFactory = new BasicDisplaySetFactory(imageFactory) {CreateSingleImageDisplaySets = _creationOptions.CreateSingleImageDisplaySets};

				if (creationOptions.SplitMultiEchoSeries)
					_echoFactory = new MREchoDisplaySetFactory(imageFactory);

				if (creationOptions.SplitMultiStackSeries)
					_multiFrameStackFactory = new MultiFrameStackDisplaySetFactory(imageFactory);

				if (_creationOptions.SplitMixedMultiframes)
					_mixedMultiFrameFactory = new MixedMultiFrameDisplaySetFactory(imageFactory);

				var externalFactories = new List<IDisplaySetFactory>();
				foreach (IDisplaySetFactoryProvider provider in new DisplaySetFactoryProviderExtensionPoint().CreateExtensions())
					externalFactories.AddRange(provider.CreateDisplaySetFactories(imageFactory));

				_externalFactories = externalFactories.AsReadOnly();

				_placeholderDisplaySetFactory = new PlaceholderDisplaySetFactory();
			}

			public bool ModalityDisplaySetExists { get; set; }

			private bool CreateAllImagesDisplaySet
			{
				get { return _creationOptions.CreateAllImagesDisplaySet; }
			}

			private bool ShowOriginalSeries
			{
				get { return _creationOptions.ShowOriginalSeries; }
			}

			private bool CreateSingleImageDisplaySets
			{
				get { return _creationOptions.CreateSingleImageDisplaySets; }
			}

			private bool SplitMultiEchoSeries
			{
				get { return _creationOptions.SplitMultiEchoSeries; }
			}

			private bool ShowOriginalMREchoSeries
			{
				get { return _creationOptions.ShowOriginalMultiEchoSeries; }
			}

			private bool SplitMultiStackSeries
			{
				get { return _creationOptions.SplitMultiStackSeries; }
			}

			private bool ShowOriginalStackSeries
			{
				get { return _creationOptions.ShowOriginalMultiStackSeries; }
			}

			private bool SplitMixedMultiframeSeries
			{
				get { return _creationOptions.SplitMixedMultiframes; }
			}

			private bool ShowOriginalMixedMultiframeSeries
			{
				get { return _creationOptions.ShowOriginalMixedMultiframeSeries; }
			}

			public override void SetStudyTree(StudyTree studyTree)
			{
				base.SetStudyTree(studyTree);

				_basicFactory.SetStudyTree(studyTree);

				if (_echoFactory != null)
					_echoFactory.SetStudyTree(studyTree);

				if (_multiFrameStackFactory != null)
					_multiFrameStackFactory.SetStudyTree(studyTree);

				if (_mixedMultiFrameFactory != null)
					_mixedMultiFrameFactory.SetStudyTree(studyTree);

				_placeholderDisplaySetFactory.SetStudyTree(studyTree);

				foreach (var factory in _externalFactories)
					factory.SetStudyTree(studyTree);
			}

			public override List<IDisplaySet> CreateDisplaySets(Series series)
			{
				var displaySets = new List<IDisplaySet>();

				bool showOriginal = true;
				if (SplitMultiEchoSeries)
				{
					_echoFactory.SplitStacks = SplitMultiStackSeries;
					List<IDisplaySet> echoDisplaySets = _echoFactory.CreateDisplaySets(series);
					if (echoDisplaySets.Count > 0 && !ShowOriginalMREchoSeries)
						showOriginal = false;

					displaySets.AddRange(echoDisplaySets);
				}
				else if (SplitMultiStackSeries)
				{
					var stackDisplaySets = _multiFrameStackFactory.CreateDisplaySets(series);
					if (stackDisplaySets.Count > 0 && !ShowOriginalStackSeries)
						showOriginal = false;

					displaySets.AddRange(stackDisplaySets);
				}

				if (SplitMixedMultiframeSeries)
				{
					List<IDisplaySet> multiFrameDisplaySets = _mixedMultiFrameFactory.CreateDisplaySets(series);
					if (multiFrameDisplaySets.Count > 0 && !ShowOriginalMixedMultiframeSeries)
						showOriginal = false;

					displaySets.AddRange(multiFrameDisplaySets);
				}

				bool modalityDegenerateCase = CreateAllImagesDisplaySet && !ModalityDisplaySetExists;
				bool singleImageDegenerateCase = false;

				if (CreateSingleImageDisplaySets)
				{
					//The factory will only create single image display sets and will not create a series 
					//display set for the degenerate case of one image in a series. In the case where
					//the user wants to see "single image" display sets, we actually create a series
					//display set (below) for the degenerate case, because that's technically more correct.
					_basicFactory.CreateSingleImageDisplaySets = true;
					var singleImageDisplaySets = new List<IDisplaySet>();
					foreach (IDisplaySet displaySet in _basicFactory.CreateDisplaySets(series))
						singleImageDisplaySets.Add(displaySet);

					displaySets.AddRange(singleImageDisplaySets);

					singleImageDegenerateCase = singleImageDisplaySets.Count == 0;
				}

				//Show the original if:
				// 1. A previous part of this method hasn't already disabled it.
				// 2. The user wants to see it, or
				// 3. It's a degenerate case
				showOriginal = showOriginal && (ShowOriginalSeries || modalityDegenerateCase || singleImageDegenerateCase);
				if (showOriginal)
				{
					//The factory will create series display sets only.
					_basicFactory.CreateSingleImageDisplaySets = false;
					foreach (IDisplaySet displaySet in _basicFactory.CreateDisplaySets(series))
						displaySets.Add(displaySet);
				}

				bool anyDisplaySetsCreated = displaySets.Count > 0 || ModalityDisplaySetExists;
				if (!anyDisplaySetsCreated)
					displaySets.AddRange(_placeholderDisplaySetFactory.CreateDisplaySets(series));

				foreach (var factory in _externalFactories)
					displaySets.AddRange(factory.CreateDisplaySets(series));

				return displaySets;
			}
		}
	}
}
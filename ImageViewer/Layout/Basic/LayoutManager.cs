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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Configuration;

namespace ClearCanvas.ImageViewer.Layout.Basic
{

	[ExtensionOf(typeof(LayoutManagerExtensionPoint))]
	public partial class LayoutManager : ImageViewer.LayoutManager
	{
		#region LayoutHookContext

		class LayoutHookContext : IHpLayoutHookContext
		{
		    private readonly LayoutManager _layoutManager;

		    public LayoutHookContext(IImageViewer viewer, LayoutManager layoutManager)
			{
				this.ImageViewer = viewer;
		        this._layoutManager = layoutManager;
			}

			public IImageViewer ImageViewer { get; private set; }

		    public void PerformDefaultPhysicalWorkspaceLayout()
		    {
		        if (_layoutManager!=null)
		        {
		            _layoutManager.LayoutPhysicalWorkspace();
		        }
		    }

            public void PerformDefaultFillPhysicalWorkspace()
            {
                if (_layoutManager != null)
                {
                    _layoutManager.FillPhysicalWorkspace();
                }
            }

        }

		#endregion

	    private readonly IPatientReconciliationStrategy _reconciliationStrategy = new DefaultPatientReconciliationStrategy();
	    private ImageSetFiller _imageSetFiller;
	    private IHpLayoutHook _layoutHook;
	    private IDisplaySetCreationOptions _displaySetCreationOptions;

	    public LayoutManager()
		{
			AllowEmptyViewer = ViewerLaunchSettings.AllowEmptyViewer;
		}

		public override void SetImageViewer(IImageViewer imageViewer)
		{
			base.SetImageViewer(imageViewer);

			StudyTree studyTree = null;
			if (imageViewer != null)
				studyTree = imageViewer.StudyTree;

			_reconciliationStrategy.SetStudyTree(studyTree);

            _imageSetFiller = new ImageSetFiller(studyTree, DisplaySetCreationOptions);
		}

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing || !(_layoutHook is IDisposable))
                return;
            
            ((IDisposable)_layoutHook).Dispose();
            _layoutHook = null;
        }

	    public IHpLayoutHook LayoutHook
	    {
	        get
	        {
	            if (_layoutHook == null)
	            {
	                try
	                {
	                    _layoutHook = (IHpLayoutHook)new HpLayoutHookExtensionPoint().CreateExtension();
	                }
	                catch (NotSupportedException)
	                {
	                    _layoutHook = HpLayoutHook.Default;
	                }
	            }

                return _layoutHook;
	        }
            set
            {
                _layoutHook = value;
            }
	    }

	    public IDisplaySetCreationOptions DisplaySetCreationOptions
	    {
	        get
	        {
                if (_displaySetCreationOptions == null)
                    _displaySetCreationOptions = new DisplaySetCreationOptions();

                return _displaySetCreationOptions;
	        }    
	    }

		#region Logical Workspace building 

		protected override IComparer<Series> GetSeriesComparer()
		{
			return new CompositeComparer<Series>(new DXSeriesPresentationIntentComparer(), base.GetSeriesComparer());
		}

        protected override IPatientData ReconcilePatient(IStudyRootData studyData)
		{
            var reconciled = _reconciliationStrategy.ReconcilePatientInformation(studyData);
			if (reconciled != null)
				return new StudyRootStudyIdentifier(reconciled, new StudyIdentifier());

            return base.ReconcilePatient(studyData);
		}

		protected override void FillImageSet(IImageSet imageSet, Study study)
		{
            _imageSetFiller.AddMultiSeriesDisplaySets(imageSet, study);

            base.FillImageSet(imageSet, study);
		}

	    protected override void UpdateImageSet(IImageSet imageSet, Series series)
		{
            _imageSetFiller.AddSeriesDisplaySets(imageSet, series);
		}

		#endregion

		protected override void LayoutAndFillPhysicalWorkspace()
		{
            if (LayoutHook.HandleLayout(new LayoutHookContext(ImageViewer, this)))
                    return;

		    // hooks did not handle it, so call base class
			base.LayoutAndFillPhysicalWorkspace();
		}

		protected override void LayoutPhysicalWorkspace()
		{
			StoredLayout layout = null;

			//take the first opened study, enumerate the modalities and compute the union of the layout configuration (in case there are multiple modalities in the study).
			if (LogicalWorkspace.ImageSets.Count > 0)
			{
				IImageSet firstImageSet = LogicalWorkspace.ImageSets[0];
				foreach (IDisplaySet displaySet in firstImageSet.DisplaySets)
				{
					if (displaySet.PresentationImages.Count <= 0)
						continue;

					if (layout == null)
						layout = LayoutSettings.GetMinimumLayout();

                    StoredLayout storedLayout = LayoutSettings.DefaultInstance.GetLayout(displaySet.PresentationImages[0] as IImageSopProvider);
					layout.ImageBoxRows = Math.Max(layout.ImageBoxRows, storedLayout.ImageBoxRows);
					layout.ImageBoxColumns = Math.Max(layout.ImageBoxColumns, storedLayout.ImageBoxColumns);
					layout.TileRows = Math.Max(layout.TileRows, storedLayout.TileRows);
					layout.TileColumns = Math.Max(layout.TileColumns, storedLayout.TileColumns);
				}
			}

			if (layout == null)
                layout = LayoutSettings.DefaultInstance.DefaultLayout;

			PhysicalWorkspace.SetImageBoxGrid(layout.ImageBoxRows, layout.ImageBoxColumns);
			for (int i = 0; i < PhysicalWorkspace.ImageBoxes.Count; ++i)
				PhysicalWorkspace.ImageBoxes[i].SetTileGrid(layout.TileRows, layout.TileColumns);
		}

		#region Comparers

        /// TODO (CR Nov 2011): I think we can actually remove this now, as the other comparers also do it.
        private class DXSeriesPresentationIntentComparer : DicomSeriesComparer
		{
			public override int Compare(Sop x, Sop y)
			{
				// this sorts FOR PRESENTATION series to the beginning.
				// FOR PROCESSING and unspecified series are considered equal for the purposes of sorting by intent.
				const string forPresentation = "FOR PRESENTATION";
				int presentationIntentX = GetPresentationIntent(x) == forPresentation ? 0 : 1;
				int presentationIntentY = GetPresentationIntent(y) == forPresentation ? 0 : 1;
				int result = presentationIntentX - presentationIntentY;
				if (Reverse)
					return -result;
				return result;
			}

			private static string GetPresentationIntent(Sop sop)
			{
				DicomAttribute attribute;
				if (sop.TryGetAttribute(DicomTags.PresentationIntentType, out attribute))
					return (attribute.ToString() ?? string.Empty).ToUpperInvariant();
				return string.Empty;
			}
		}

	    /// TODO (CR Sep 2011): Move to ImageViewer.Comparers.
		private class CompositeComparer<T> : IComparer<T>
		{
			private readonly IList<IComparer<T>> _comparers;

			public CompositeComparer(params IComparer<T>[] comparers)
			{
				Platform.CheckForNullReference(comparers, "comparers");
				_comparers = new List<IComparer<T>>(comparers);
			}

			public int Compare(T x, T y)
			{
				foreach (IComparer<T> comparer in _comparers)
				{
					int result = comparer.Compare(x, y);
					if (result != 0)
						return result;
				}
				return 0;
			}
		}

		#endregion
	}
}

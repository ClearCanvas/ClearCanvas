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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Automation;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	public partial class ContextMenuLayoutTool
	{
		private class DefaultContextMenuActionFactory : ActionFactory
		{
			public DefaultContextMenuActionFactory()
			{ }

			#region Unavailable

			private string GetActionMessage(IDicomImageSetDescriptor descriptor)
			{
				if (descriptor.IsOffline)
                    return ClearCanvas.ImageViewer.SR.MessageInfoStudyOffline;
				else if (descriptor.IsNearline)
                    return ClearCanvas.ImageViewer.SR.MessageInfoStudyNearline;
				else if (descriptor.IsInUse)
                    return ClearCanvas.ImageViewer.SR.MessageInfoStudyInUse;
				else if (descriptor.IsNotLoadable)
                    return ClearCanvas.ImageViewer.SR.MessageInfoNoStudyLoader;
				else
                    return ClearCanvas.ImageViewer.SR.MessageInfoStudyCouldNotBeLoaded;
			}

            private string GetActionLabel(IDicomImageSetDescriptor descriptor)
			{
				if (descriptor.IsOffline)
					return String.Format(SR.LabelFormatStudyUnavailable, SR.Offline);
				else if (descriptor.IsNearline)
					return String.Format(SR.LabelFormatStudyUnavailable, SR.Nearline);
				else if (descriptor.IsInUse)
					return String.Format(SR.LabelFormatStudyUnavailable, SR.InUse);
				else if (descriptor.IsNotLoadable)
					return String.Format(SR.LabelFormatStudyUnavailable, SR.Unavailable);
				else
					return SR.LabelStudyCouldNotBeLoaded;
			}


			private IClickAction CreateUnavailableStudyAction(IActionFactoryContext context)
			{
			    var descriptor = context.ImageSet.Descriptor as IDicomImageSetDescriptor;
                if (descriptor == null || descriptor.LoadStudyError == null)
                    return null;

				return CreateMenuAction(context, GetActionLabel(descriptor),
										() => context.DesktopWindow.ShowMessageBox(GetActionMessage(descriptor), MessageBoxActions.Ok));
			}

			#endregion

			#region Display Sets

			internal static void AssignDisplaySetToImageBox(IImageBox imageBox, IDisplaySet displaySet)
			{
                MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(imageBox);
                memorableCommand.BeginState = imageBox.CreateMemento();

                // always create a 'fresh copy' to show in the image box.  We never want to show
                // the 'originals' (e.g. the ones in IImageSet.DisplaySets) because we want them 
                // to remain clean and unaltered - consider them to be templates for what actually
                // gets shown.
                imageBox.DisplaySet = displaySet.CreateFreshCopy();

                imageBox.Draw();
                //this.ImageViewer.SelectedImageBox[0, 0].Select();

                memorableCommand.EndState = imageBox.CreateMemento();

                DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(imageBox);
                historyCommand.Enqueue(memorableCommand);
                imageBox.ImageViewer.CommandHistory.AddCommand(historyCommand);
            }

		    internal static void AssignDisplaySetToImageBox(IImageViewer viewer, IDisplaySet displaySet)
			{
                AssignDisplaySetToImageBox(viewer.SelectedImageBox, displaySet);
			}

			private IAction[] CreateDisplaySetActions(IActionFactoryContext context)
			{
				List<IAction> actions = new List<IAction>();

				IImageBox imageBox = context.ImageViewer.SelectedImageBox;
				if (imageBox != null && !imageBox.DisplaySetLocked)
				{
					foreach (IDisplaySet displaySet in context.ImageSet.DisplaySets)
					{
						IDisplaySet theDisplaySet = displaySet;
						MenuAction action = CreateMenuAction(context, displaySet.Name,
											() => AssignDisplaySetToImageBox(context.ImageViewer, theDisplaySet));

						action.Checked = context.ImageViewer.SelectedImageBox != null &&
							context.ImageViewer.SelectedImageBox.DisplaySet != null &&
							context.ImageViewer.SelectedImageBox.DisplaySet.Uid == theDisplaySet.Uid;

						actions.Add(action);
					}
				}

				return actions.ToArray();
			}

			#endregion

			public override IAction[] CreateActions(IActionFactoryContext context)
			{
				List<IAction> actions = new List<IAction>();

			    var unavailable = CreateUnavailableStudyAction(context);
                if (unavailable != null)
                    actions.Add(unavailable);
				else
					actions.AddRange(CreateDisplaySetActions(context));

				return actions.ToArray();
			}
		}
    }

    #region Oto

    partial class ContextMenuLayoutTool : IDisplaySetLayout
    {
        DisplaySetInfo IDisplaySetLayout.AssignDisplaySet(DisplaySetInfo displaySetInfo)
        {
            var imageBox = base.Context.Viewer.SelectedImageBox;
            switch (displaySetInfo.DisplaySetType)
            {
                case DisplaySetType.Series:
                    return AssignSeriesDisplaySet(imageBox, displaySetInfo);
                case DisplaySetType.SingleImage:
                    return AssignSingleImageDisplaySet(imageBox, displaySetInfo);
                case DisplaySetType.AllImages:
                    return AssignAllImagesDisplaySet(imageBox, displaySetInfo);
                case DisplaySetType.MREcho:
                    return AssignMREchoDisplaySet(imageBox, displaySetInfo);
                case DisplaySetType.MixedMultiframeMultiframeImage:
                    return AssignMixedMultiframeMultiframeImageDisplaySet(imageBox, displaySetInfo);
                case DisplaySetType.MixedMultiframeSingleImages:
                    return AssignMixedMultiframeSingleImagesDisplaySet(imageBox, displaySetInfo);
                default:
                    return AssignUnknownDisplaySetType(imageBox, displaySetInfo);
            }
        }

        DisplaySetInfo IDisplaySetLayout.GetDisplaySetAt(RectangularGrid.Location imageBoxLocation)
        {
            IDisplaySet displaySet = base.Context.Viewer.PhysicalWorkspace[imageBoxLocation.Row, imageBoxLocation.Column].DisplaySet;
            return GetDisplaySetInfo(displaySet);
        }

        private DisplaySetInfo GetDisplaySetInfo(IDisplaySet displaySet)
        {
            if (displaySet == null)
                return new DisplaySetInfo {DisplaySetType = DisplaySetType.None};

            if (displaySet.Descriptor is SeriesDisplaySetDescriptor)
            {
                var descriptor = (SeriesDisplaySetDescriptor) displaySet.Descriptor;
                return new DisplaySetInfo
                           {
                               DisplaySetType = DisplaySetType.Series,
                               DisplaySetUid = displaySet.Uid,
                               DisplaySetName = displaySet.Name,
                               SeriesDescription = descriptor.SourceSeries.SeriesDescription,
                               StudyInstanceUid = descriptor.SourceSeries.StudyInstanceUid,
                               //AccessionNumber = descriptor.SourceSeries.AccessionNumber,
                               Modality = descriptor.SourceSeries.Modality,
                               SeriesInstanceUid = descriptor.SourceSeries.SeriesInstanceUid,
                               SeriesNumber = descriptor.SourceSeries.SeriesNumber
                           };
            }
            if (displaySet.Descriptor is SingleImageDisplaySetDescriptor)
            {
                var descriptor = (SingleImageDisplaySetDescriptor) displaySet.Descriptor;
                var frame = ((IImageSopProvider) displaySet.PresentationImages[0]).Frame;
                return new DisplaySetInfo
                           {
                               DisplaySetType = DisplaySetType.SingleImage,
                               DisplaySetUid = displaySet.Uid,
                               DisplaySetName = displaySet.Name,
                               StudyInstanceUid = descriptor.SourceSeries.StudyInstanceUid,
                               //AccessionNumber = descriptor.SourceSeries.AccessionNumber,
                               Modality = descriptor.SourceSeries.Modality,
                               SeriesInstanceUid = descriptor.SourceSeries.SeriesInstanceUid,
                               SeriesDescription = descriptor.SourceSeries.SeriesDescription,
                               SeriesNumber = descriptor.SourceSeries.SeriesNumber,
                               InstanceNumber = frame.ParentImageSop.InstanceNumber,

                               FrameNumber = frame.ParentImageSop.Frames.Count > 1 ? frame.FrameNumber : (int?)null,
                            };
            }
            if (displaySet.Descriptor is MREchoDisplaySetDescriptor)
            {
                var descriptor = (MREchoDisplaySetDescriptor)displaySet.Descriptor;
                return new DisplaySetInfo
                {
                    DisplaySetType = DisplaySetType.MREcho,
                    DisplaySetUid = displaySet.Uid,
                    DisplaySetName = displaySet.Name,
                    StudyInstanceUid = descriptor.SourceSeries.StudyInstanceUid,
                    //AccessionNumber = descriptor.SourceSeries.AccessionNumber,
                    Modality = descriptor.SourceSeries.Modality,
                    SeriesInstanceUid = descriptor.SourceSeries.SeriesInstanceUid,
                    SeriesDescription = descriptor.SourceSeries.SeriesDescription,
                    SeriesNumber = descriptor.SourceSeries.SeriesNumber,
                    EchoNumber = descriptor.EchoNumber
                };
            }
            if (displaySet.Descriptor is SingleImagesDisplaySetDescriptor)
            {
                var descriptor = (SingleImagesDisplaySetDescriptor)displaySet.Descriptor;
                return new DisplaySetInfo
                {
                    DisplaySetType = DisplaySetType.MixedMultiframeSingleImages,
                    DisplaySetUid = displaySet.Uid,
                    DisplaySetName = displaySet.Name,
                    StudyInstanceUid = descriptor.SourceSeries.StudyInstanceUid,
                    //AccessionNumber = descriptor.SourceSeries.AccessionNumber,
                    Modality = descriptor.SourceSeries.Modality,
                    SeriesInstanceUid = descriptor.SourceSeries.SeriesInstanceUid,
                    SeriesDescription = descriptor.SourceSeries.SeriesDescription,
                    SeriesNumber = descriptor.SourceSeries.SeriesNumber
                };
            }
            if (displaySet.Descriptor is MultiframeDisplaySetDescriptor)
            {
                var descriptor = (MultiframeDisplaySetDescriptor)displaySet.Descriptor;
                var frame = ((IImageSopProvider)displaySet.PresentationImages[0]).Frame;
                return new DisplaySetInfo
                {
                    DisplaySetType = DisplaySetType.MixedMultiframeMultiframeImage,
                    DisplaySetUid = displaySet.Uid,
                    DisplaySetName = displaySet.Name,
                    StudyInstanceUid = descriptor.SourceSeries.StudyInstanceUid,
                    //AccessionNumber = descriptor.SourceSeries.AccessionNumber,
                    Modality = descriptor.SourceSeries.Modality,
                    SeriesInstanceUid = descriptor.SourceSeries.SeriesInstanceUid,
                    SeriesDescription = descriptor.SourceSeries.SeriesDescription,
                    SeriesNumber = descriptor.SourceSeries.SeriesNumber,
                    InstanceNumber = frame.ParentImageSop.InstanceNumber,
                };
            }
            if (displaySet.Descriptor is ModalityDisplaySetDescriptor)
            {
                var descriptor = (ModalityDisplaySetDescriptor)displaySet.Descriptor;
                return new DisplaySetInfo
                {
                    DisplaySetType = DisplaySetType.Series,
                    StudyInstanceUid = descriptor.SourceSeries.StudyInstanceUid,
                    //AccessionNumber = descriptor.SourceSeries.AccessionNumber,
                    DisplaySetUid = displaySet.Uid,
                    Modality = descriptor.Modality,
                };
            }

            return new DisplaySetInfo
            {
                DisplaySetType = DisplaySetType.Unknown,
                DisplaySetUid = displaySet.Uid,
                DisplaySetName = displaySet.Name,
            };
        }

        private DisplaySetInfo AssignUnknownDisplaySetType(IImageBox imageBox, DisplaySetInfo displaySetInfo)
        {
            foreach (var imageSet in Context.Viewer.LogicalWorkspace.ImageSets)
            {
                var displaySets = string.IsNullOrEmpty(displaySetInfo.DisplaySetUid)
                    ? imageSet.DisplaySets.Where(d => d.Descriptor.Name == displaySetInfo.DisplaySetName)
                    : imageSet.DisplaySets.Where(d => d.Descriptor.Uid == displaySetInfo.DisplaySetUid);
                var displaySet = displaySets.FirstOrDefault();
                if (displaySet != null)
                {
                    DefaultContextMenuActionFactory.AssignDisplaySetToImageBox(imageBox, displaySet);
                    return GetDisplaySetInfo(displaySet);
                }
            }

            throw new InvalidOperationException("Display set not found.");
        }

        private DisplaySetInfo AssignSeriesDisplaySet(IImageBox imageBox, DisplaySetInfo displaySetInfo)
	    {
            foreach (var imageSet in Context.Viewer.LogicalWorkspace.ImageSets)
            {
                var displaySets = imageSet.DisplaySets
                        .Where(d => Match((IDicomDisplaySetDescriptor)d.Descriptor, DisplaySetType.Series))
                        .Where(d => Match((IDicomDisplaySetDescriptor)d.Descriptor, displaySetInfo));
                var displaySet = displaySets.FirstOrDefault();
                if (displaySet != null)
                {
                    DefaultContextMenuActionFactory.AssignDisplaySetToImageBox(imageBox, displaySet);
                    return GetDisplaySetInfo(displaySet);
                }
            }

            throw new InvalidOperationException("Display set not found.");
        }


        private DisplaySetInfo AssignSingleImageDisplaySet(IImageBox imageBox, DisplaySetInfo displaySetInfo)
	    {
            foreach (var imageSet in Context.Viewer.LogicalWorkspace.ImageSets)
            {
                var displaySets = imageSet.DisplaySets
                    .Where(d => Match((IDicomDisplaySetDescriptor) d.Descriptor, DisplaySetType.SingleImage))
                    .Where(d => Match((IDicomDisplaySetDescriptor) d.Descriptor, displaySetInfo))
                    .Where(d => Match(((IImageSopProvider) d.PresentationImages[0]).Frame, displaySetInfo));
                var displaySet = displaySets.FirstOrDefault();
                if (displaySet != null)
                {
                    DefaultContextMenuActionFactory.AssignDisplaySetToImageBox(imageBox, displaySet);
                    return GetDisplaySetInfo(displaySet);
                }
            }

            throw new InvalidOperationException("Display set not found.");
        }

        private DisplaySetInfo AssignAllImagesDisplaySet(IImageBox imageBox, DisplaySetInfo displaySetInfo)
	    {
            foreach (var imageSet in Context.Viewer.LogicalWorkspace.ImageSets)
            {
                var displaySets = imageSet.DisplaySets
                    .Where(d => Match((IDicomDisplaySetDescriptor)d.Descriptor, DisplaySetType.AllImages))
                    .Where(d=> Match(((ModalityDisplaySetDescriptor)d.Descriptor), displaySetInfo));
                var displaySet = displaySets.FirstOrDefault();
                if (displaySet != null)
                {
                    DefaultContextMenuActionFactory.AssignDisplaySetToImageBox(imageBox, displaySet);
                    return GetDisplaySetInfo(displaySet);
                }
            }

            throw new InvalidOperationException("Display set not found.");
        }

        private DisplaySetInfo AssignMREchoDisplaySet(IImageBox imageBox, DisplaySetInfo displaySetInfo)
	    {
            foreach (var imageSet in Context.Viewer.LogicalWorkspace.ImageSets)
            {
                var displaySets = imageSet.DisplaySets
                    .Where(d => Match((IDicomDisplaySetDescriptor) d.Descriptor, DisplaySetType.MREcho))
                    .Where(d => Match((IDicomDisplaySetDescriptor) d.Descriptor, displaySetInfo))
                    .Where(d => Match((MREchoDisplaySetDescriptor) d.Descriptor, displaySetInfo));
                var displaySet = displaySets.FirstOrDefault();
                if (displaySet != null)
                {
                    DefaultContextMenuActionFactory.AssignDisplaySetToImageBox(imageBox, displaySet);
                    return GetDisplaySetInfo(displaySet);
                }
            }

            throw new InvalidOperationException("Display set not found.");
	    }

        private DisplaySetInfo AssignMixedMultiframeMultiframeImageDisplaySet(IImageBox imageBox, DisplaySetInfo displaySetInfo)
	    {
            foreach (var imageSet in Context.Viewer.LogicalWorkspace.ImageSets)
            {
                var displaySets = imageSet.DisplaySets
                    .Where(d => Match((IDicomDisplaySetDescriptor)d.Descriptor, DisplaySetType.MixedMultiframeMultiframeImage))
                    .Where(d => Match((IDicomDisplaySetDescriptor)d.Descriptor, displaySetInfo))
                    .Where(d => Match((MultiframeDisplaySetDescriptor)d.Descriptor, displaySetInfo));
                var displaySet = displaySets.FirstOrDefault();
                if (displaySet != null)
                {
                    DefaultContextMenuActionFactory.AssignDisplaySetToImageBox(imageBox, displaySet);
                    return GetDisplaySetInfo(displaySet);
                }
            }

            throw new InvalidOperationException("Display set not found.");
	    }

        private DisplaySetInfo AssignMixedMultiframeSingleImagesDisplaySet(IImageBox imageBox, DisplaySetInfo displaySetInfo)
        {
            foreach (var imageSet in Context.Viewer.LogicalWorkspace.ImageSets)
            {
                var displaySets = imageSet.DisplaySets
                    .Where(d => Match((IDicomDisplaySetDescriptor) d.Descriptor, DisplaySetType.MixedMultiframeSingleImages))
                    .Where(d => Match((IDicomDisplaySetDescriptor) d.Descriptor, displaySetInfo));
                var displaySet = displaySets.FirstOrDefault();
                if (displaySet != null)
                {
                    DefaultContextMenuActionFactory.AssignDisplaySetToImageBox(imageBox, displaySet);
                    return GetDisplaySetInfo(displaySet);
                }
            }

            throw new InvalidOperationException("Display set not found.");
        }

        private bool Match(IDicomDisplaySetDescriptor descriptor, DisplaySetType displaySetType)
        {
            switch (displaySetType)
            {
                case DisplaySetType.Series:
                    return descriptor is SeriesDisplaySetDescriptor;
                case DisplaySetType.SingleImage:
                    return descriptor is SingleImageDisplaySetDescriptor;
                case DisplaySetType.AllImages:
                    return descriptor is ModalityDisplaySetDescriptor;
                case DisplaySetType.MREcho:
                    return descriptor is MREchoDisplaySetDescriptor;
                case DisplaySetType.MixedMultiframeSingleImages:
                    return descriptor is SingleImagesDisplaySetDescriptor;
                case DisplaySetType.MixedMultiframeMultiframeImage:
                    return descriptor is MultiframeDisplaySetDescriptor;
                case DisplaySetType.None:
                default:
                    return false;
            }
        }

        private bool Match(MultiframeDisplaySetDescriptor descriptor, DisplaySetInfo displaySetInfo)
        {
            if (displaySetInfo.InstanceNumber.HasValue && descriptor.InstanceNumber != displaySetInfo.InstanceNumber.Value)
                return false;
            return true;
        }

        private bool Match(ModalityDisplaySetDescriptor descriptor, DisplaySetInfo displaySetInfo)
        {
            if (!String.IsNullOrEmpty(displaySetInfo.Modality) && descriptor.Modality != displaySetInfo.Modality)
                return false;
            return true;
        }

        private bool Match(MREchoDisplaySetDescriptor descriptor, DisplaySetInfo displaySetInfo)
        {
            if (displaySetInfo.EchoNumber.HasValue && descriptor.EchoNumber != displaySetInfo.EchoNumber.Value)
                return false;
            return true;
        }

        private bool Match(IDicomDisplaySetDescriptor descriptor, DisplaySetInfo displaySetInfo)
        {
            if (!Match(descriptor, displaySetInfo.DisplaySetType))
                return false;

            if (!String.IsNullOrEmpty(displaySetInfo.DisplaySetUid) && descriptor.Uid != displaySetInfo.DisplaySetUid)
                return false;
            if (!String.IsNullOrEmpty(displaySetInfo.Modality) && descriptor.SourceSeries.Modality != displaySetInfo.Modality)
                return false;
            if (!String.IsNullOrEmpty(displaySetInfo.StudyInstanceUid) && descriptor.SourceSeries.StudyInstanceUid != displaySetInfo.StudyInstanceUid)
                return false;
            if (!String.IsNullOrEmpty(displaySetInfo.SeriesInstanceUid) && descriptor.SourceSeries.SeriesInstanceUid != displaySetInfo.SeriesInstanceUid)
                return false;
            if (!String.IsNullOrEmpty(displaySetInfo.SeriesDescription) && descriptor.SourceSeries.SeriesDescription != displaySetInfo.SeriesDescription)
                return false;
            if (displaySetInfo.SeriesNumber.HasValue && descriptor.SourceSeries.SeriesNumber != displaySetInfo.SeriesNumber.Value)
                return false;
            return true;
        }

        private bool Match(Frame frame, DisplaySetInfo displaySetInfo)
        {
            if (displaySetInfo.FrameNumber.HasValue && frame.FrameNumber != displaySetInfo.FrameNumber.Value)
                return false;

            if (displaySetInfo.InstanceNumber.HasValue && frame.ParentImageSop.InstanceNumber != displaySetInfo.InstanceNumber.Value)
                return false;

            return true;
        }
    }

    #endregion
}
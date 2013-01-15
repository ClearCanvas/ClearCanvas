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
using System.Security.Policy;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
    public class KeyImageDisplaySet
    {
        public static void AddKeyImage(IPresentationImage image)
        {
            Platform.CheckForNullReference(image, "image");
            Platform.CheckForNullReference(image.ImageViewer, "image.ImageViewer");

            if (!PermissionsHelper.IsInRole(AuthorityTokens.KeyImages))
                throw new PolicyException(SR.ExceptionCreateKeyImagePermissionDenied);

            var sopProvider = image as IImageSopProvider;
            if (sopProvider == null)
                throw new ArgumentException("The image must be an IImageSopProvider.", "image");

            IDisplaySet displaySet = null;

            foreach (var set in image.ImageViewer.LogicalWorkspace.ImageSets)
            {
                if (set.Descriptor.Equals(image.ParentDisplaySet.ParentImageSet.Descriptor))
                {
                    foreach (var d in set.DisplaySets)
                    {
                        var displaySetDescriptor = d.Descriptor as KeyImageDisplaySetDescriptor;

                        if (displaySetDescriptor != null
                            &&
                            displaySetDescriptor.SourceStudy.StudyInstanceUid.Equals(sopProvider.Sop.StudyInstanceUid))
                        {
                            displaySet = d;
                            break;
                        }
                    }

                    break;
                }
            }

            if (displaySet == null)
            {
                var displaySetDescriptor = new KeyImageDisplaySetDescriptor(new StudyIdentifier(sopProvider.ImageSop));
                displaySet = new DisplaySet(displaySetDescriptor);
                bool displaySetAdded = false;
                foreach (var imageSet in image.ImageViewer.LogicalWorkspace.ImageSets)
                {
                    if (imageSet.Descriptor.Equals(image.ParentDisplaySet.ParentImageSet.Descriptor))
                    {
                        imageSet.DisplaySets.Add(displaySet);
                        displaySetAdded = true;
                        break;
                    }
                }
                if (!displaySetAdded)
                {
                    throw new ApplicationException(SR.MessageCreateKeyImageFailed);
                }
            }

            var presentationImage = image.CreateFreshCopy();
            
            var presentationState = DicomSoftcopyPresentationState.Create(image);
            var basicImage = presentationImage as BasicPresentationImage;
            if (basicImage != null)
                basicImage.PresentationState = presentationState;

            displaySet.PresentationImages.Add(presentationImage);

            foreach (var imageBox in image.ImageViewer.PhysicalWorkspace.ImageBoxes)
            {
                if (imageBox.DisplaySet != null && imageBox.DisplaySet.Descriptor.Uid == displaySet.Descriptor.Uid)
                {
                    var physicalImage = presentationImage.CreateFreshCopy();

                    presentationState = DicomSoftcopyPresentationState.Create(image);
                    basicImage = physicalImage as BasicPresentationImage;
                    if (basicImage != null)
                        basicImage.PresentationState = presentationState;

                    imageBox.DisplaySet.PresentationImages.Add(physicalImage);

                    imageBox.Draw();
                }
            }
        }

        public static void RemoveKeyImage(IPresentationImage image)
        {
            Platform.CheckForNullReference(image, "image");
            Platform.CheckForNullReference(image.ImageViewer, "image.ImageViewer");

            if (!PermissionsHelper.IsInRole(AuthorityTokens.KeyImages))
                throw new PolicyException(SR.ExceptionCreateKeyImagePermissionDenied);
            foreach (var imageSet in image.ImageViewer.LogicalWorkspace.ImageSets)
            {
                foreach (var d in imageSet.DisplaySets)
                {
                    var displaySetDescriptor = d.Descriptor as KeyImageDisplaySetDescriptor;

                    if (displaySetDescriptor != null)
                    {
                        foreach (var i in d.PresentationImages)
                        {
                            if (i.Uid == image.Uid)
                            {
                                
                            }                            
                        }
                    }
                }
            }
        }
    }
}

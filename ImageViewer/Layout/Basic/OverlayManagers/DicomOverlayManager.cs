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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

namespace ClearCanvas.ImageViewer.Layout.Basic.OverlayManagers
{
    public class DicomOverlayManager : OverlayManager
    {
        public DicomOverlayManager()
            : base(SR.NameDicomOverlay, SR.NameDicomOverlay)
        {
            IsConfigurable = false;
        }

        public override bool IsSelectedByDefault(string modality)
        {
            return true;
        }

        public override void SetOverlayVisible(IPresentationImage image, bool visible)
        {
            if (image is IDicomPresentationImage)
            {
                foreach (OverlayPlaneGraphic overlayGraphic in GetOverlayPlanesGraphic(image as IDicomPresentationImage))
                    overlayGraphic.Visible = visible;
            }
        }

        private static IEnumerable<OverlayPlaneGraphic> GetOverlayPlanesGraphic(IDicomPresentationImage image)
        {
            DicomGraphicsPlane dicomGraphicsPlane = DicomGraphicsPlane.GetDicomGraphicsPlane(image, false);
            if (dicomGraphicsPlane != null)
            {
                foreach (ILayer layer in (IEnumerable<ILayer>)dicomGraphicsPlane.Layers)
                {
                    foreach (OverlayPlaneGraphic overlayGraphic in CollectionUtils.Select(layer.Graphics, graphic => graphic is OverlayPlaneGraphic))
                        yield return overlayGraphic;
                }
            }
        }
    }
}

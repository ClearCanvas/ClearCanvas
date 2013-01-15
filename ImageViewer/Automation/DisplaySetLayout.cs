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
using ClearCanvas.ImageViewer.Layout;

namespace ClearCanvas.ImageViewer.Automation
{
    public enum DisplaySetType
    {
        None = 0,
        Unknown,
        Series,
        SingleImage,
        AllImages,
        MixedMultiframeMultiframeImage,
        MixedMultiframeSingleImages,
        MREcho
    }

    public class DisplaySetInfo
    {
        public DisplaySetType DisplaySetType { get; set; }
        public string DisplaySetUid { get; set; }
        public string DisplaySetName { get; set; }
        public string StudyInstanceUid { get; set; }
        //public string AccessionNumber { get; set; }
        public string SeriesInstanceUid { get; set; }
        public string Modality { get; set; }
        public int? SeriesNumber { get; set; }
        public string SeriesDescription { get; set; }
        public int? InstanceNumber { get; set; }
        public int? FrameNumber { get; set; }
        public int? EchoNumber { get; set; }

    }

    public interface IDisplaySetLayout
    {
        DisplaySetInfo AssignDisplaySet(DisplaySetInfo displaySetInfo);
        DisplaySetInfo GetDisplaySetAt(RectangularGrid.Location imageBoxLocation);
    }
}
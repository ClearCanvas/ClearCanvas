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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Comparers;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
    [ExtensionPoint]
    public sealed class ImageComparerFactoryExtensionPoint : ExtensionPoint<IImageComparerFactory>
    {
    }

    public class ImageComparer
    {
        private readonly IComparer<IPresentationImage> _comparer;

        public ImageComparer(string name, string description, IComparer<IPresentationImage> comparer)
        {
            Name = name;
            Description = description;
            _comparer = comparer;
        }

        #region IImageComparer Members

        public string Name { get; private set; }
        public string Description { get; private set; }
        public IComparer<IPresentationImage> GetComparer(bool reverse)
        {
            return !reverse ? _comparer : new ReverseComparer<IPresentationImage>(_comparer);
        }

        #endregion

        public static List<ImageComparer> CreateAll()
        {
            List<ImageComparer> comparers = CreateStockComparers();

            try
            {
                foreach (IImageComparerFactory factory in new ImageComparerFactoryExtensionPoint().CreateExtensions())
                    comparers.AddRange(factory.CreateComparers());
            }
            catch (NotSupportedException)
            {
            }

            comparers.Sort((c1, c2) => String.Compare(c1.Description, c2.Description));
            return comparers;
        }

        private static List<ImageComparer> CreateStockComparers()
        {
            return new List<ImageComparer>
                       {
                           new ImageComparer("Instance Number", "SortByImageNumberDescription", new InstanceAndFrameNumberComparer()),
                           new ImageComparer("Acquisition Time", "SortByAcquisitionTimeDescription", new AcquisitionTimeComparer()),
                           new ImageComparer("Slice Location", "SortBySliceLocationDescription", new SliceLocationComparer())
                       };
        }
    }

    public interface IImageComparerFactory
    {
        List<ImageComparer> CreateComparers();
    }
}
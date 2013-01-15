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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    [ExtensionPoint]
    public sealed class ImageBoxFillingStrategyExtensionPoint : ExtensionPoint<IImageBoxFillingStrategy> { }

    public interface IImageBoxFillingStrategyContext
    {
        IImageViewer ImageViewer { get; }
        IPhysicalWorkspace PhysicalWorkspace { get; }
        ILogicalWorkspace LogicalWorkspace { get; }

        void SetImageBoxLayout();
        void SetTileLayout(IImageBox imageBox);
    }

    public interface IImageBoxFillingStrategy
    {
        void SetContext(IImageBoxFillingStrategyContext context);
        void FillImageBoxes();
    }

    internal class ImageBoxFillingStrategyContext : IImageBoxFillingStrategyContext
    {
        private readonly Action<IPhysicalWorkspace> _setImageBoxLayout;
        private readonly Action<IImageBox> _setTileLayout;

        public ImageBoxFillingStrategyContext(IImageViewer viewer, Action<IPhysicalWorkspace> setImageBoxLayout, Action<IImageBox> setTileLayout)
        {
            ImageViewer = viewer;
            _setImageBoxLayout = setImageBoxLayout;
            _setTileLayout = setTileLayout;
        }

        #region IImageBoxFillingStrategyContext Members

        public IImageViewer ImageViewer { get; set; }

        public IPhysicalWorkspace PhysicalWorkspace
        {
            get { return ImageViewer.PhysicalWorkspace; }
        }

        public ILogicalWorkspace LogicalWorkspace
        {
            get { return ImageViewer.LogicalWorkspace; }
        }

        public void SetImageBoxLayout()
        {
            _setImageBoxLayout(PhysicalWorkspace);
        }

        public void SetTileLayout(IImageBox imageBox)
        {
            _setTileLayout(imageBox);
        }

        #endregion
    }

    public class ImageBoxFillingStrategy : IImageBoxFillingStrategy
    {
        public ImageBoxFillingStrategy()
        {
            FillEmptySpace = true;
        }

        void IImageBoxFillingStrategy.SetContext(IImageBoxFillingStrategyContext context)
        {
            Context = context;
        }

        private IImageBoxFillingStrategyContext Context { get; set; }
        private IPhysicalWorkspace PhysicalWorkspace { get { return Context.PhysicalWorkspace; } }
        private ILogicalWorkspace LogicalWorkspace { get { return Context.LogicalWorkspace; } }

        public bool FillEmptySpace { get; set; }

        public void FillImageBoxes()
        {
            bool isOldLayoutRectangular = PhysicalWorkspace.Rows > 0 && PhysicalWorkspace.Columns > 0;
            if (isOldLayoutRectangular)
                SetImageBoxLayout();
            else
                SetImageBoxLayoutSimple();
        }

        private void SetImageBoxLayout()
        {
            //Capture state before layout change.
            object[,] oldImageBoxMementos = GetImageBoxMementos();

            //Set image box layout.
            Context.SetImageBoxLayout();

            int newRows = PhysicalWorkspace.Rows;
            int newColumns = PhysicalWorkspace.Columns;

            if (newRows > 0 && newColumns > 0)
            {
                //Both new and old layouts are rectangular.
                Queue offScreenMementos = GetOffScreenImageBoxMementos(oldImageBoxMementos);

                int oldRows = oldImageBoxMementos.GetLength(0);
                int oldColumns = oldImageBoxMementos.GetLength(1);

                int sameRows = Math.Min(oldRows, newRows);
                int sameColumns = Math.Min(oldColumns, newColumns);

                // Try to keep existing display sets in the same row/column position, if possible.
                for (int row = 0; row < sameRows; ++row)
                {
                    for (int column = 0; column < sameColumns; ++column)
                    {
                        object memento = oldImageBoxMementos[row, column];
                        if (memento == null)
                            Context.SetTileLayout(PhysicalWorkspace[row, column]);
                        else
                            PhysicalWorkspace[row, column].SetMemento(memento);
                    }
                }

                // Fill in available image boxes, preferably with display sets that went 'off-screen',
                // followed by new ones that are not already visible.
                FillImageBoxes(GetAvailableEmptyImageBoxes(), offScreenMementos);
            }
            else
            {
                Context.SetImageBoxLayout();
                FillImageBoxes(PhysicalWorkspace.ImageBoxes, oldImageBoxMementos);
            }
        }

        private void SetImageBoxLayoutSimple()
        {
            //Either the old or new layout (or both) is non-rectangular.
            var oldMementos = CollectionUtils.Map<ImageBox, object>(PhysicalWorkspace.ImageBoxes, imageBox => imageBox.CreateMemento());
            Context.SetImageBoxLayout();
            FillImageBoxes(PhysicalWorkspace.ImageBoxes, oldMementos);
        }
        
        private object[,] GetImageBoxMementos()
        {
            int rows = PhysicalWorkspace.Rows;
            int columns = PhysicalWorkspace.Columns;

            object[,] mementos = new object[rows, columns];

            for (int row = 0; row < rows; ++row)
            {
                for (int column = 0; column < columns; ++column)
                {
                    IImageBox imageBox = PhysicalWorkspace[row, column];
                    if (imageBox.DisplaySet != null)
                        mementos[row, column] = imageBox.CreateMemento();
                }
            }

            return mementos;
        }

        private Queue GetOffScreenImageBoxMementos(object[,] oldImageBoxMementos)
        {
            //TODO (cr Oct 2009): this whole thing with mementos wouldn't be necessary
            //if we had a SetImageBoxGrid(imageBox[,])

            int oldRows = oldImageBoxMementos.GetLength(0);
            int oldColumns = oldImageBoxMementos.GetLength(1);

            int newRows = PhysicalWorkspace.Rows;
            int newColumns = PhysicalWorkspace.Columns;

            int sameRows = Math.Min(oldRows, newRows);
            int sameColumns = Math.Min(oldColumns, newColumns);

            Queue offScreenMementos = new Queue();
            //Get mementos for all the display sets that have gone off-screen, from top-to-bottom, left-to-right.

            for (int row = 0; row < sameRows; ++row)
            {
                for (int column = sameColumns; column < oldColumns; ++column)
                {
                    object memento = oldImageBoxMementos[row, column];
                    if (memento != null)
                        offScreenMementos.Enqueue(memento);
                }
            }

            for (int row = sameRows; row < oldRows; ++row)
            {
                for (int column = 0; column < oldColumns; ++column)
                {
                    object memento = oldImageBoxMementos[row, column];
                    if (memento != null)
                        offScreenMementos.Enqueue(memento);
                }
            }

            return offScreenMementos;
        }

        private IEnumerable<IImageBox> GetAvailableEmptyImageBoxes()
        {
            Stack<IImageBox> imageBoxes = new Stack<IImageBox>();

            //go top to bottom, right to left, stopping before the first non-empty image box.
            for (int row = 0; row < PhysicalWorkspace.Rows; ++row)
            {
                for (int column = PhysicalWorkspace.Columns - 1; column >= 0; --column)
                {
                    IImageBox imageBox = PhysicalWorkspace[row, column];
                    if (imageBox.DisplaySet == null)
                        imageBoxes.Push(imageBox);
                    else
                        break; //skip to the next row
                }

                while (imageBoxes.Count > 0)
                    yield return imageBoxes.Pop();
            }
        }

        private List<IImageSet> GetVisibleImageSets()
        {
            List<IImageSet> visibleImageSets = new List<IImageSet>();
            foreach (IImageBox imageBox in PhysicalWorkspace.ImageBoxes)
            {
                IDisplaySet displaySet = imageBox.DisplaySet;
                if (displaySet == null)
                    continue;

                IImageSet imageSet = displaySet.ParentImageSet;
                if (imageSet == null || visibleImageSets.Contains(imageSet))
                    continue;

                visibleImageSets.Add(imageSet);
            }

            return visibleImageSets;
        }

        private IDisplaySet GetNextDisplaySet()
        {
            foreach (IImageSet imageSet in GetVisibleImageSets())
            {
                foreach (IDisplaySet displaySet in imageSet.DisplaySets)
                {
                    bool alreadyVisible = false;
                    foreach (IImageBox imageBox in PhysicalWorkspace.ImageBoxes)
                    {
                        if (imageBox.DisplaySet != null && imageBox.DisplaySet.Uid == displaySet.Uid)
                        {
                            alreadyVisible = true;
                            break;
                        }
                    }

                    if (!alreadyVisible)
                        return displaySet.CreateFreshCopy();
                }
            }

            return null;
        }

        private void FillImageBoxes(IEnumerable<IImageBox> imageBoxes, IEnumerable oldImageBoxMementos)
        {
            var enumerator = oldImageBoxMementos.GetEnumerator();
            foreach (IImageBox imageBox in imageBoxes)
            {
                if (enumerator.MoveNext())
                {
                    imageBox.SetMemento(enumerator.Current);
                }
                else
                {
                    Context.SetTileLayout(imageBox);
                    if (FillEmptySpace)
                        imageBox.DisplaySet = GetNextDisplaySet();
                }
            }
        }

        internal static IImageBoxFillingStrategy Create()
        {
            try
            {
                return (IImageBoxFillingStrategy)new ImageBoxFillingStrategyExtensionPoint().CreateExtension();
            }
            catch (NotSupportedException)
            {
                return new ImageBoxFillingStrategy();
            }
        }

    }
}

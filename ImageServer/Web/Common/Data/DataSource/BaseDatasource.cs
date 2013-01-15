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
using System.Text;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
    public class BaseDataSource
    {
        protected Array resizeArray(Array oldArray, int newSize)
        {
            int oldSize = oldArray.Length;
            Type elementType = oldArray.GetType().GetElementType();
            System.Array newArray = System.Array.CreateInstance(elementType, newSize);
            int preserveLength = System.Math.Min(oldSize, newSize);
            if (preserveLength > 0)
                System.Array.Copy(oldArray, newArray, preserveLength);


            return newArray;
        }

        protected int adjustCopyLength(int startRowIndex, int maximumRows, int arrayLength)
        {
            int copyLength = 0;
            if (startRowIndex > 0)
            {
                copyLength = (startRowIndex + maximumRows) > arrayLength
                                 ? arrayLength - startRowIndex
                                 : maximumRows;
            }
            else
            {
                copyLength = arrayLength < maximumRows ? arrayLength : maximumRows;
            }

            return copyLength;
        }
    }
}

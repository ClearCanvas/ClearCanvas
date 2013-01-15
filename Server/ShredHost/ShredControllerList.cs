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
using System.Collections.ObjectModel;
using ClearCanvas.Common;

namespace ClearCanvas.Server.ShredHost
{
    internal class ShredControllerList : MarshallableList<ShredController>
    {
        public ShredControllerList()
        {

        }

        public ReadOnlyCollection<ShredController> AllShredInfo
        {
            get { return this.ContainedObjects; }
        }

        public ShredController this[int index]
        {
            get
            {
                foreach (ShredController shredController in this.ContainedObjects)
                {
                    if (shredController.Id == index)
                    {
                        return shredController;
                    }
                }

                string message = "Could not find ShredController object with Id = " + index.ToString();
                throw new System.IndexOutOfRangeException(message);
            }
        }

        public WcfDataShred[] WcfDataShredCollection
        {
            get
            {
                WcfDataShred[] shreds = new WcfDataShred[this.ContainedObjects.Count];

                int i = 0;
                foreach (ShredController shredController in this.ContainedObjects)
                {
                    shreds[i++] = shredController.WcfDataShred;
                }

                return shreds;
            }
        }
    }
}

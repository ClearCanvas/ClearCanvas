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
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Core.Diagnostics
{
    public delegate void ErrorDelegate();

    public static class RandomError
    {
        public static void Generate(bool condition, string description, ErrorDelegate del)
        {
            if (condition == true)
            {
                Random ran = new Random();
                if (ran.Next() % 10 == 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("\n\n\t**********************************************************************************************************\n");
                    sb.AppendFormat("\t                 SIMULATING ERROR: {0}\n", description);
                    sb.AppendFormat("\t**********************************************************************************************************\n");
                    Platform.Log(LogLevel.Error, sb.ToString());
                    if (del!=null)
                        del();
                    else
                    {
                        throw new Exception("Simulated Random Exception");
                    }
                }
            }
        }

        public static void Generate(bool condition, string description)
        {
            Generate(condition, description, null);
        }
    }
}

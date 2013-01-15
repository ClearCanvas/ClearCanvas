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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using System.IO;

namespace ClearCanvas.Enterprise.Core.Imex
{
    /// <summary>
    /// Defines an extension point for XML data importer/exporters.
    /// </summary>
    [ExtensionPoint]
    public class XmlDataImexExtensionPoint : ExtensionPoint<IXmlDataImex>
    {
    }

    /// <summary>
    /// Abstract base class for <see cref="Import"/> and <see cref="Export"/> applications.
    /// </summary>
    public abstract class ImexApplicationBase<TCmdLine> : IApplicationRoot
        where TCmdLine : CommandLine, new()
    {
        #region IApplicationRoot Members

        /// <summary>
        /// Called by the platform to run the application.
        /// </summary>
        public void RunApplication(string[] args)
        {
            TCmdLine cmdLine = new TCmdLine();

            try
            {
                cmdLine.Parse(args);
                Execute(cmdLine);
            }
            catch (CommandLineException e)
            {
                Console.WriteLine(e.Message);
                cmdLine.PrintUsage(Console.Out);
                Console.WriteLine("List of supported data-classes:");
                PrintImexDataClasses(Console.Out);
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine("Invalid data class: " + e.Message);
                Console.WriteLine("List of supported data-classes:");
                PrintImexDataClasses(Console.Out);
            }
        }

        #endregion

        /// <summary>
        /// Executes the action specified by the command line arguments.
        /// </summary>
        /// <param name="cmdLine"></param>
        protected abstract void Execute(TCmdLine cmdLine);


        private void PrintImexDataClasses(TextWriter writer)
        {
            foreach (string w in ImexUtils.ListImexDataClasses())
                writer.WriteLine(w);
        }

    }
}

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
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Imex
{
    /// <summary>
    /// Export application.
    /// </summary>
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class Export : ImexApplicationBase<ExportCommandLine>
    {
        /// <summary>
        /// Executes the action specified by the command line arguments.
        /// </summary>
        /// <param name="cmdLine"></param>
        protected override void Execute(ExportCommandLine cmdLine)
        {
            if (cmdLine.AllClasses)
            {
                ExportAllClasses(cmdLine);
            }
            else if (!string.IsNullOrEmpty(cmdLine.DataClass))
            {
                ExportOneClass(cmdLine);
            }
            else 
                throw new CommandLineException("Must specify either /class:[data-class] or /all.");
        }

        /// <summary>
        /// Exports a single data-class.
        /// </summary>
        /// <param name="cmdLine"></param>
        private void ExportOneClass(ExportCommandLine cmdLine)
        {
            IXmlDataImex imex = ImexUtils.FindImexForDataClass(cmdLine.DataClass);
            int itemsPerFile = cmdLine.ItemsPerFile > 0
                                   ? cmdLine.ItemsPerFile
                                   : ImexUtils.GetDefaultItemsPerFile(imex.GetType());

            string directory;
            string baseFileName;

            // if the path has no extension, assume it specifies a directory
            if(string.IsNullOrEmpty(Path.GetExtension(cmdLine.Path)))
            {
                // use the name of the dataclass as a base filename
                directory = cmdLine.Path;
                baseFileName = cmdLine.DataClass;
            }
            else
            {
                // use the specified file name as the base filename
                directory = Path.GetDirectoryName(cmdLine.Path);
                baseFileName = Path.GetFileNameWithoutExtension(cmdLine.Path);
            }

            ImexUtils.Export(imex, directory, baseFileName, itemsPerFile);
        }

        /// <summary>
        /// Exports all data-classes for which an Imex extension exists.
        /// </summary>
        /// <param name="cmdLine"></param>
        private void ExportAllClasses(ExportCommandLine cmdLine)
        {
            string path = cmdLine.Path;

            // assume the supplied path is a directory, and create it if it doesn't exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (IXmlDataImex imex in new XmlDataImexExtensionPoint().CreateExtensions())
            {
                ImexDataClassAttribute a = AttributeUtils.GetAttribute<ImexDataClassAttribute>(imex.GetType());
                if (a != null)
                {
                    ImexUtils.Export(imex, Path.Combine(path, a.DataClass), a.DataClass, a.ItemsPerFile);
                }
            }
        }
    }
}

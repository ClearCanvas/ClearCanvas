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
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Imex
{
    /// <summary>
    /// Defines utilities for working with Imex classes, and for transferring Imex data to and from the
    /// filesystem.
    /// </summary>
    public static class ImexUtils
    {
        #region ImportItem class

        private class ImportItem : IImportItem
        {
            private XmlReader _reader;

            public ImportItem(XmlReader reader)
            {
                _reader = reader;
            }

            public XmlReader Read()
            {
                return _reader;
            }
        }

        #endregion

        /// <summary>
        /// Defines the root XML tag under which data is exported.
        /// </summary>
        public const string RootTag = "Items";

        /// <summary>
        /// Returns a list of data-classes for which an Imex extension exists.
        /// </summary>
        /// <returns></returns>
        public static string[] ListImexDataClasses()
        {
            List<string> dataClasses = new List<string>();
            foreach (ExtensionInfo info in new XmlDataImexExtensionPoint().ListExtensions())
            {
                ImexDataClassAttribute a = AttributeUtils.GetAttribute<ImexDataClassAttribute>(info.ExtensionClass.Resolve());
                if (a != null)
                    dataClasses.Add(a.DataClass);
            }
            return dataClasses.ToArray();
        }

        /// <summary>
        /// Finds the imex that supports the specified data-class.
        /// </summary>
        /// <param name="dataClass"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Indicates that no imex was found that supports the specified data-class.</exception>
        public static IXmlDataImex FindImexForDataClass(string dataClass)
        {
            return (IXmlDataImex)new XmlDataImexExtensionPoint().CreateExtension(
                delegate(ExtensionInfo info)
                {
                    return CollectionUtils.Contains(AttributeUtils.GetAttributes<ImexDataClassAttribute>(info.ExtensionClass.Resolve()),
                        delegate(ImexDataClassAttribute a)
                        {
                            return a != null && a.DataClass.Equals(
                                dataClass, StringComparison.InvariantCultureIgnoreCase);
                        });
                });
        }

        /// <summary>
        /// Gets the default number of items exported per file, for the specified Imex extension class.
        /// </summary>
        /// <param name="imexClass"></param>
        /// <returns></returns>
        public static int GetDefaultItemsPerFile(Type imexClass)
        {
            ImexDataClassAttribute a = AttributeUtils.GetAttribute<ImexDataClassAttribute>(imexClass);
            return a == null ? 0 : a.ItemsPerFile;
        }

        /// <summary>
        /// Exports data from the specified imex, to the specified directory using the specified base filename.
        /// </summary>
        /// <param name="imex"></param>
        /// <param name="directory"></param>
        /// <param name="baseFileName"></param>
        /// <param name="itemsPerFile"></param>
        public static void Export(IXmlDataImex imex, string directory, string baseFileName, int itemsPerFile)
        {
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // set a flag indicating whether all items should be exported to a single file
            bool oneFile = (itemsPerFile == 0);

            // if one file, set itemsPerFile to max value (which is effectively the same thing)
            if(oneFile)
                itemsPerFile = int.MaxValue;

            int itemCount = 0;
            int fileCount = 0;

            StreamWriter sw = null;
            XmlTextWriter writer = null;
            foreach (IExportItem item in imex.Export())
            {
                if(itemCount % itemsPerFile == 0)
                {
                    // close current file
                    if(writer != null)
                    {
                        writer.WriteEndElement();
                        writer.Close();
                        sw.Close();
                    }

                    // start new file
                    string file = oneFile ?  Path.Combine(directory, baseFileName)
                        : Path.Combine(directory, baseFileName + (++fileCount));
                    if (!file.EndsWith(".xml"))
                        file += ".xml";

                    // delete if already exists
                    File.Delete(file);

                    sw = new StreamWriter(File.OpenWrite(file));
                    writer = new XmlTextWriter(sw);
                    writer.Formatting = System.Xml.Formatting.Indented;
                    writer.WriteStartElement(RootTag);
                }

                item.Write(writer);

                itemCount++;
            }
            if (writer != null)
            {
                writer.WriteEndElement();
                writer.Close();
                sw.Close();
            }
        }

        /// <summary>
        /// Imports data to the specified imex, from the specified path, which may be either a single file
        /// or a directory containing a set of .xml files.
        /// </summary>
        /// <param name="imex"></param>
        /// <param name="path"></param>
        public static void Import(IXmlDataImex imex, string path)
        {
            Platform.CheckForNullReference(path, "path");   

            // determine list of source files to import
            List<string> fileList = new List<string>();
            if(File.Exists(path))
            {
                fileList.Add(path);
            }
            else if(Directory.Exists(path))
            {
                fileList.AddRange(Directory.GetFiles(path, "*.xml"));
            }
            else 
                throw new ArgumentException(string.Format("{0} is not a valid source file or directory.", path));

            imex.Import(ReadSourceFiles(fileList));
        }

        /// <summary>
        /// Reads the specified set of XML source files, yielding each item as an <see cref="IImportItem"/>.
        /// </summary>
        /// <param name="sourceFiles"></param>
        /// <returns></returns>
        private static IEnumerable<IImportItem> ReadSourceFiles(IEnumerable<string> sourceFiles)
        {
            foreach (string sourceFile in sourceFiles)
            {
                using (StreamReader reader = File.OpenText(sourceFile))
                {
                    XmlTextReader xmlReader = new XmlTextReader(reader);
                    xmlReader.WhitespaceHandling = WhitespaceHandling.None;

                    // advance to root tag
                    if (xmlReader.ReadToFollowing(RootTag))
                    {
                        // advance to first child
                        while (xmlReader.Read() && xmlReader.NodeType != XmlNodeType.Element) ;

                        // if child nodes exist, read them
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            string itemTag = xmlReader.Name;
                            for (bool more = true; more; more = xmlReader.ReadToNextSibling(itemTag))
                            {
                                yield return new ImportItem(xmlReader.ReadSubtree());
                            }
                        }
                    }
                    xmlReader.Close();
                }
            }
        }
    }
}

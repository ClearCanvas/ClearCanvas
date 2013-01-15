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

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom.DicomDump
{
    class DicomDump
    {

        static DicomDumpOptions _options = DicomDumpOptions.Default;

        static void PrintCommandLine()
        {
            Console.WriteLine("DicomDump Parameters:");
            Console.WriteLine("\t-h\tDisplay this help information");
            Console.WriteLine("\t-g\tInclude group lengths");
            Console.WriteLine("\t-c\tAllow more than 80 characters per line");
            Console.WriteLine("\t-l\tDisplay long values");
            Console.WriteLine("All other parameters are considered filenames to list.");
        }

        static bool ParseArgs(string[] args)
        {
            foreach (String arg in args)
            {
                if (arg.ToLower().Equals("-h"))
                {
                    PrintCommandLine();
                    return false;
                }
                else if (arg.ToLower().Equals("-g"))
                {
                    _options &= ~DicomDumpOptions.KeepGroupLengthElements;
                }
                else if (arg.ToLower().Equals("-c"))
                {
                    _options &= ~DicomDumpOptions.Restrict80CharactersPerLine;
                }
                else if (arg.ToLower().Equals("-l"))
                {
                    _options &= ~DicomDumpOptions.ShortenLongValues;
                }
            }
            return true;
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintCommandLine();
                return;
            }

            if (false == ParseArgs(args))
                return;

            foreach (String filename in args)
            {
                if (filename.StartsWith("-"))
                    continue;

                DicomFile file = new DicomFile(filename);

                DicomReadOptions readOptions = DicomReadOptions.Default;

				try
				{
					file.Load(readOptions);
				}
				catch (Exception e)
				{
					Console.WriteLine("Unexpected exception when loading file: {0}", e.Message);
				}

                StringBuilder sb = new StringBuilder();

                file.Dump(sb, "", _options);

                Console.WriteLine(sb.ToString());
            }
        }
    }
}

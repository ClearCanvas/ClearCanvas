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
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Collections;

namespace ClearCanvas.Utilities.BuildTasks
{
    public class VersionWebFiles : Microsoft.Build.Utilities.Task
    {
        public override bool Execute()
        {
            foreach (string file in _webFileList)
            {
                string extension = Path.GetExtension(file);

                if (extension == ".css")
                    VersionCss(file);
                else if (extension == ".htm" || extension == ".html")
                    VersionHtm(file);
                else if (extension == ".js")
                    VersionJs(file);
                else
                {
                    //No Action
                }                
            }
            return true;
        }

        private void VersionCss(string file)
        {
            string cssVersionLine = @"/*" + _versionStatement + _versionString + @"*/";

            ArrayList lines = ReadFile(file);

            lines.Insert(0, cssVersionLine);

            WriteFile(file, lines); 
        }

        private void VersionHtm(string file)
        {
            string htmlVersionLine = @"<!--" + _versionStatement + _versionString + @"-->";

            ArrayList lines = ReadFile(file);            

            foreach (string line in lines)
            {
                if (line.StartsWith(@"<html") && line.EndsWith(@">"))
                {
                    lines.Insert(lines.IndexOf(line) + 1, htmlVersionLine);
                    break;
                }
            }

            WriteFile(file, lines);           
        }

        private void VersionJs(string file)
        {
            string jsVersionLine = @"//" + _versionStatement + _versionString;

            ArrayList lines = ReadFile(file);

            lines.Insert(0, jsVersionLine);

            WriteFile(file, lines); 
        }

        private ArrayList ReadFile(string file)
        {
            ArrayList lines = new ArrayList();

            StreamReader reader = new StreamReader(file);
            string fileline;

            while ((fileline = reader.ReadLine()) != null)
                lines.Add(fileline);
            reader.Close();

            return lines;
        }

        private void WriteFile(string file, ArrayList lines)
        {
            StreamWriter writer = new StreamWriter(file);

            foreach (string newLine in lines)
                writer.WriteLine(newLine);
            writer.Close();
        }

        [Required]
        public string WebFileList
        {
            set { _webFileList = value.Split(';'); }
        }

        [Required]
        public string VersionString
        {
            set { _versionString = value; }
        }

        
        private string _versionStatement = "Ris Web Component Version Number: ";
        private string[] _webFileList;
        private string _versionString;        
    }
}

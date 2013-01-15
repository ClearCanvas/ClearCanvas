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
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.IO;
using System.Xml;

namespace ClearCanvas.Utilities.BuildTasks
{
    public class NUnitLogParser : Microsoft.Build.Utilities.Task
    {
        public override bool Execute()
        {
            XmlTextReader reader = new XmlTextReader(_filename);
            StreamWriter writer = new StreamWriter(_filename + ".log");
            string name;
            string success;
            string message;
            string stackTrace;

            writer.WriteLine("NUnit Failures for Build Performed on: " + DateTime.Now.ToString());
            writer.WriteLine();

            while (reader.Read())
            {
                if (reader.Name.EndsWith("test-case"))
                {

                    reader.MoveToNextAttribute();  //Name
                    name = reader.Value;
                    reader.MoveToNextAttribute(); //Executed
                    reader.MoveToNextAttribute(); //Success
                    success = reader.Value;

                    if (success == "False")
                    {
                        advanceToElement(ref reader, "message");
                        message = reader.ReadElementContentAsString();

                        advanceToElement(ref reader, "stack-trace");
                        stackTrace = reader.ReadElementContentAsString();

                        writer.Write(_failCount.ToString() + ") " + name);
                        _failCount++;
                        writer.WriteLine(" - " + message);
                        writer.WriteLine(stackTrace);
                        writer.WriteLine();
                    }
                }
            }

            writer.Close();
            reader.Close();
            return true;
        }

        private void advanceToElement(ref XmlTextReader reader, string element)
        {
            while (!reader.Name.EndsWith(element))
            {
                reader.Read();
            }
        }

        [Required]
        public string Filename
        {
            set { _filename = value; }
        }

        private string _filename;
        private int _failCount = 1;

    }
}

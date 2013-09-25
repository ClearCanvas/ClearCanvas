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
using System.Linq;
using System.Security;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;
using System.Globalization;


namespace ClearCanvas.Dicom.DataDictionaryGenerator
{
    public struct Tag
    {
        public uint nTag;
        public String tag;
        public String name;
		public String unEscapedName;
		public String vr;
        public String vm;
        public String retired;
        public String varName;
        public String dicomVarName;
    }

    public struct SopClass
    {
        public String Name;
        public String Uid;
        public String Type;
        public String VarName;

		public bool IsMeta
		{
			get
			{
				return Name.ToLower().Contains("meta");
			}
		}

    	public bool IsImage
    	{
    		get
    		{
				//As of DICOM 2011, this was true. Need to re-evaluate each time.
    			return Name.ToLower().Contains("image storage") || Name == "Enhanced US Volume Storage";
    		}
    	}

		public bool IsStorage
		{
			get
			{
				//As of DICOM 2011, this was true. Need to re-evaluate each time.
				var lower = Name.ToLower();
				return lower.Contains("storage") && !lower.Contains("storage commit");
			}
		}
	}

    public class Parser
    {
        public SortedList<uint, Tag> Tags = new SortedList<uint, Tag>();
        public SortedList SopClasses = new SortedList();
        public SortedList MetaSopClasses = new SortedList();
		public SortedList TranferSyntaxes = new SortedList();

		public void DumpSopClassesXml(string fileName)
		{
			using (var fileStream = File.Open(fileName, FileMode.Create, FileAccess.Write))
			{
				using (var writer = new StreamWriter(fileStream, Encoding.UTF8))
				{
					writer.WriteLine("<SopClassList>");
					foreach (DictionaryEntry sopClassEntry in SopClasses)
					{
						var sopClass = (SopClass)sopClassEntry.Value;
						writer.WriteLine("   <SopClass name =\"" + sopClass.Name + "\"");
						writer.WriteLine("             isImage=\"" + sopClass.IsImage + "\"");
						writer.WriteLine("             isStorage=\"" + sopClass.IsStorage + "\"");
						writer.WriteLine("             uid=\"" + sopClass.Uid + "\""); 
						writer.Write(    "             variableName=\"" + sopClass.VarName+ "\"");
						//writer.Write(    "             type=\"" + sopClass.Type + "\""); 
						writer.WriteLine("/>");
					}

					writer.WriteLine("</SopClassList>");
				}
			}
		}

		public void DumpSopClassesCSV(string fileName)
		{
			using (var fileStream = File.Open(fileName, FileMode.Create, FileAccess.Write))
			{
				using (var writer = new StreamWriter(fileStream, Encoding.UTF8))
				{
					writer.WriteLine("name,isImage,isStorage,uid,variableName");
					foreach (DictionaryEntry sopClassEntry in SopClasses)
					{
						var sopClass = (SopClass)sopClassEntry.Value;
						writer.Write(sopClass.Name);
						writer.Write(",");
						writer.Write(sopClass.IsImage);
						writer.Write(",");
						writer.Write(sopClass.IsStorage);
						writer.Write(",");
						writer.Write(sopClass.Uid);
						writer.Write(",");
						writer.Write(sopClass.VarName);
						writer.WriteLine();
					}
				}
			}
		}

        /// <summary>
        /// Formats to pascal.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string FormatToPascal(string value)
        {
            if (value == null)
                return null;

            var sb = new StringBuilder();
            bool lastCharWasSpace = false;
            foreach (char c in value)
            {
                if (c.ToString() == " ")
                    lastCharWasSpace = true;
                else if (lastCharWasSpace || sb.Length == 0)
                    sb.Append(c.ToString().ToUpperInvariant());
                else
                    sb.Append(c.ToString().ToLowerInvariant());
            }
            return sb.ToString();
        }

        public static string CreateVariableName(string input)
        {
            // Now create the variable name
            var charSeparators = new[] {'(', ')', ',', ' ', '\'', '�', '�', '-', '/', '&', '[', ']', '@', '.'};

            // just remove apostrophes so casing is correct
            string tempString = input.Replace("’", ""); 
            tempString = tempString.Replace("'", "");
            tempString = tempString.Replace("(", "");
            tempString = tempString.Replace(")", "");
            tempString = tempString.Replace("–", "");
            tempString = tempString.Replace("μ", "U");
            tempString = tempString.Replace("µ", "U");
            
            String[] nodes = tempString.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);

            string output = "";
            foreach (String node in nodes)
                output += FormatToPascal(node);

            return output;
        }

        public static void CreateNames(ref Tag thisTag)
        {
            thisTag.varName = CreateVariableName(thisTag.name);

            // Handling leading digits in names
            if (thisTag.varName.Length > 0 && char.IsDigit(thisTag.varName[0]))
                thisTag.varName = thisTag.dicomVarName;

            if (thisTag.retired != null 
             && thisTag.retired.Equals("RET") 
             && !thisTag.varName.EndsWith("Retired"))
                thisTag.varName += "Retired";
        	thisTag.name = thisTag.name.Replace("’", "'");
        	thisTag.unEscapedName = thisTag.name;
            thisTag.name = SecurityElement.Escape(thisTag.name);
        }

        public void ParseFile(String filename)
        {
            TextReader tReader = new StreamReader(filename);
            var settings = new XmlReaderSettings
                               {
                                   CheckCharacters = false,
                                   ValidationType = ValidationType.None,
                                   ConformanceLevel = ConformanceLevel.Fragment,
                                   IgnoreProcessingInstructions = true
                               };
            XmlReader reader = XmlReader.Create(tReader, settings);
            var columnArray = new String[10];
            int colCount = -1;
            bool isTag = true;
            bool isUid = true;
            try
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        bool isFirst = true;
                        if (reader.Name == "w:tbl")
                        {
                            while (reader.Read())
                            {
                                if (reader.IsStartElement())
                                {
                                    if (reader.Name == "w:tc")
                                    {
                                        colCount++;
                                    }
                                    else if (reader.Name == "w:t")
                                    {
                                        String val = reader.ReadString();
                                        //if (val != "(")
                                        if (columnArray[colCount] == null)
                                            columnArray[colCount] = val;
                                        else
                                            columnArray[colCount] += val;
                                    }
                                }
                                if ((reader.NodeType == XmlNodeType.EndElement)
                                    && (reader.Name == "w:tr"))
                                {
                                    if (isFirst)
                                    {
                                        if (columnArray[0] == "Tag")
                                        {
                                            isTag = true;
                                            isUid = false;
                                        }
                                        else
                                        {
                                            isTag = false;
                                            isUid = true;
                                        }

                                        isFirst = false;
                                    }
                                    else
                                    {
                                        if (isTag)
                                        {
                                            var thisTag = new Tag();
                                            if (columnArray[0] != null && columnArray[0] != "Tag")
                                            {
                                                thisTag.tag = columnArray[0];
                                                thisTag.name = columnArray[1];
                                                thisTag.dicomVarName = columnArray[2];
                                                if (columnArray[3] != null)
                                                    thisTag.vr = columnArray[3].Trim();
                                                if (columnArray[4] != null)
                                                    thisTag.vm = columnArray[4].Trim();
                                                thisTag.retired = columnArray[5];

                                                // Handle repeating groups
                                                if (thisTag.tag[3] == 'x')
                                                    thisTag.tag = thisTag.tag.Replace("xx", "00");

                                                var charSeparators = new[] { '(', ')', ',', ' ' };

                                                String[] nodes = thisTag.tag.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                                                UInt32 group, element; 
                                                if (UInt32.TryParse(nodes[0],NumberStyles.HexNumber,null, out group)
                                                 && UInt32.TryParse(nodes[1], NumberStyles.HexNumber,null, out element)
                                                    && thisTag.name != null)
                                                {
                                                    thisTag.nTag = element | group << 16;

                                                    CreateNames(ref thisTag);

                                                    if (!thisTag.varName.Equals("Item")
                                                     && !thisTag.varName.Equals("ItemDelimitationItem")
                                                     && !thisTag.varName.Equals("SequenceDelimitationItem")
                                                     && !thisTag.varName.Equals("GroupLength"))
                                                        Tags.Add(thisTag.nTag, thisTag);
                                                }
                                            }
                                        }
                                        else if (isUid)
                                        {

                                            if (columnArray[0] != null)
                                            {
                                                var thisUid = new SopClass
                                                                  {
                                                                      Uid = columnArray[0] ?? string.Empty,
                                                                      Name = columnArray[1] ?? string.Empty,
                                                                      Type = columnArray[2] ?? string.Empty
                                                                  };


                                                thisUid.VarName = CreateVariableName(thisUid.Name);

                                                // Take out the invalid chars in the name, and replace with escape characters.
                                                thisUid.Name = SecurityElement.Escape(thisUid.Name);

                                                if (thisUid.Type == "SOP Class")
                                                {
                                                    // Handling leading digits in names
                                                    if (thisUid.VarName.Length > 0 && char.IsDigit(thisUid.VarName[0]))
                                                        thisUid.VarName = "Sop" + thisUid.VarName;
                                                    SopClasses.Add(thisUid.Name, thisUid);
                                                }
                                                else if (thisUid.Type == "Transfer Syntax")
                                                {
                                                    int index = thisUid.VarName.IndexOf(':');
                                                    if (index != -1)
                                                        thisUid.VarName = thisUid.VarName.Remove(index);

                                                    TranferSyntaxes.Add(thisUid.Name, thisUid);
                                                }
                                                else if (thisUid.Type == "Meta SOP Class")
                                                {
                                                    // Handling leading digits in names
                                                    if (thisUid.VarName.Length > 0 && char.IsDigit(thisUid.VarName[0]))
                                                        thisUid.VarName = "Sop" + thisUid.VarName;
                                                    MetaSopClasses.Add(thisUid.Name, thisUid);
                                                }
                                            }
                                        }
                                    }

                                    colCount = -1;
                                    for (int i = 0; i < columnArray.Length; i++)
                                        columnArray[i] = null;
                                }

                                if ((reader.NodeType == XmlNodeType.EndElement)
                                 && (reader.Name == "w:tbl"))
                                    break; // end of table
                            }
                        }
                    }
                }
            }
            catch (XmlException)
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Unexpected exception: {0}", e.Message));
            }
        }
    }
}

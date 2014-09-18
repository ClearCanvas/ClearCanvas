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
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace ClearCanvas.Dicom.DataDictionaryGenerator
{
    public partial class MainForm : Form
    {
        XmlDocument _transferSyntaxDoc = null;
		Parser _parse = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            _parse = new Parser();

            _parse.ParseFile(openFileDialog1.FileName);
			//_parse.DumpSopClassesXml("c:\\stewart\\temp\\SopClasses.xml");
			//_parse.DumpSopClassesCSV("c:\\stewart\\temp\\SopClasses.csv");
            AddGroupZeroTags(_parse.Tags);
        }

        private void OpenFile_MouseDown(object sender, MouseEventArgs e)
        {
            openFileDialog1.ShowDialog();
        }


        private void AddGroupZeroTags(SortedList<uint, Tag> tags)
        {
            var thisTag = new Tag
                              {
                                  name = "Affected SOP Class UID",
                                  tag = "(0000,0002)",
                                  vr = "UI",
                                  vm = "1",
                                  retired = "",
                                  nTag = 0x00000002
                              };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Requested SOP Class UID",
                              tag = "(0000,0003)",
                              vr = "UI",
                              vm = "1",
                              retired = "",
                              nTag = 0x00000003
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Command Field",
                              tag = "(0000,0100)",
                              vr = "US",
                              vm = "1",
                              retired = "",
                              nTag = 0x00000100
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Message ID",
                              tag = "(0000,0110)",
                              vr = "US",
                              vm = "1",
                              retired = "",
                              nTag = 0x00000110
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Message ID Being Responded To",
                              tag = "(0000,0120)",
                              vr = "US",
                              vm = "1",
                              retired = "",
                              nTag = 0x00000120
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Move Destination",
                              tag = "(0000,0600)",
                              vr = "AE",
                              vm = "1",
                              retired = "",
                              nTag = 0x00000600
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Priority",
                              tag = "(0000,0700)",
                              vr = "US",
                              vm = "1",
                              retired = "",
                              nTag = 0x00000700
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Data Set Type",
                              tag = "(0000,0800)",
                              vr = "US",
                              vm = "1",
                              retired = "",
                              nTag = 0x00000800
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Status",
                              tag = "(0000,0900)",
                              vr = "US",
                              vm = "1",
                              retired = "",
                              nTag = 0x00000900
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Offending Element",
                              tag = "(0000,0901)",
                              vr = "AT",
                              vm = "1-n",
                              retired = "",
                              nTag = 0x00000901
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Error Comment",
                              tag = "(0000,0902)",
                              vr = "LO",
                              vm = "1",
                              retired = "",
                              nTag = 0x00000902
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Error ID",
                              tag = "(0000,0903)",
                              vr = "US",
                              vm = "1",
                              retired = "",
                              nTag = 0x00000903
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Affected SOP Instance UID",
                              tag = "(0000,1000)",
                              vr = "UI",
                              vm = "1",
                              retired = "",
                              nTag = 0x00001000
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Requested SOP Instance UID",
                              tag = "(0000,1001)",
                              vr = "UI",
                              vm = "1",
                              retired = "",
                              nTag = 0x00001001
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Event Type ID",
                              tag = "(0000,1002)",
                              vr = "US",
                              vm = "1",
                              retired = "",
                              nTag = 0x000001002
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Attribute Identifier List",
                              tag = "(0000,1005)",
                              vr = "AT",
                              vm = "1-n",
                              retired = "",
                              nTag = 0x000001005
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Action Type ID",
                              tag = "(0000,1008)",
                              vr = "US",
                              vm = "1",
                              retired = "",
                              nTag = 0x000001008
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Number of Remaining Sub-operations",
                              tag = "(0000,1020)",
                              vr = "US",
                              vm = "1",
                              retired = "",
                              nTag = 0x00001020
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Number of Completed Sub-operations",
                              tag = "(0000,1021)",
                              vr = "US",
                              vm = "1",
                              retired = "",
                              nTag = 0x00001021
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Number of Failed Sub-operations",
                              tag = "(0000,1022)",
                              vr = "US",
                              vm = "1",
                              retired = "",
                              nTag = 0x00001022
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Number of Warning Sub-operations",
                              tag = "(0000,1023)",
                              vr = "US",
                              vm = "1",
                              retired = "",
                              nTag = 0x00001023
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Move Originator Application Entity Title",
                              tag = "(0000,1030)",
                              vr = "AE",
                              vm = "1",
                              retired = "",
                              nTag = 0x00001030
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Move Originator Message ID",
                              tag = "(0000,1031)",
                              vr = "US",
                              vm = "1",
                              retired = "",
                              nTag = 0x00001031
                          };
            Parser.CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag
                          {
                              name = "Group 2 Length",
                              tag = "(0002,0000)",
                              vr = "UL",
                              vm = "1",
                              retired = "",
                              nTag = 0x00020000
                          };
            Parser.CreateNames(ref thisTag);
            //tags.Add(thisTag.nTag, thisTag);
            
        }

        private void OpenTransferSyntax_Click(object sender, EventArgs e)
        {
            openFileDialog_TransferSyntax.ShowDialog();

            _transferSyntaxDoc = new XmlDocument();

            _transferSyntaxDoc.Load(openFileDialog_TransferSyntax.FileName);
        }

        private void GenerateCode_Click(object sender, EventArgs e)
        {
            if ((_transferSyntaxDoc == null) || (_parse == null))
                return;

            var gen = new CodeGenerator(_parse.Tags, _parse.TranferSyntaxes, _parse.SopClasses, _parse.MetaSopClasses, _transferSyntaxDoc);

            gen.WriteTags("DicomTags.cs");

            gen.WriteTransferSyntaxes("TransferSyntaxes.cs");

            gen.WriteSopClasses("SopClasses.cs");

            gen.WriteSqlInsert("sopClassInsert.sql");

            gen.WriteTagsText("DicomTagDictionary.data");
        }
    }
}
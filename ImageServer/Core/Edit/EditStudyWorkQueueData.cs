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
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Common.WorkQueue;

namespace ClearCanvas.ImageServer.Core.Edit
{
    [WorkQueueDataType("67DC30CA-F0A9-4A93-84DD-2E4B6C7F92C1")]
    public class UpdateItem : DataContractBase
    {
        private DicomTag _dicomTag;

    	/// <summary>
        /// *** For serialization ***
        /// </summary>
        public UpdateItem()
        {
            
        }
        public UpdateItem(uint tag, string originalValue, string newValue)
        {
            DicomTag = DicomTagDictionary.GetDicomTag(tag);
            OriginalValue = XmlUtils.XmlCharacterScrub(originalValue);
            Value = XmlUtils.XmlCharacterScrub(newValue);
        }

        [XmlIgnore]
        public DicomTag DicomTag
        {
            get { return _dicomTag; }
            set { _dicomTag = value; }
        }

        [XmlAttribute("TagValue")]
        public string TagValue
        {
            get
            { 
                return _dicomTag.HexString; 
            }
            set { 
                // NO-OP 
                uint tag;
                if (uint.TryParse(value, NumberStyles.HexNumber, null, out tag))
                {
                    _dicomTag = DicomTagDictionary.GetDicomTag(tag);
                }
            }
        }

        [XmlAttribute("TagName")]
        public string TagName
        {
            get
            {
                return _dicomTag != null ? _dicomTag.Name : null;
            }
            set
            {
                // NO-OP 
            }
        }

    	[XmlAttribute("Value")]
    	public string Value { get; set; }

    	[XmlAttribute("OriginalValue")]
    	public string OriginalValue { get; set; }
    }

	/// <summary>
	/// Edit request descriptor
	/// </summary>
    [WorkQueueDataType("FCBD6C6F-9D34-4CAB-A852-1158E6BBE9B2")]
    public class EditRequest : DataContractBase
    {
    	public List<UpdateItem> UpdateEntries { get; set; }

    	public string UserId { get; set; }

    	public string Reason { get; set; }

    	public DateTime? TimeStamp { get; set; }

        public EditType EditType { get; set; }
    }

    [WorkQueueDataType("6CC69CE1-AE40-4C18-9A4D-F40C1EED38C6")]
    public class EditStudyWorkQueueData : WorkQueueData
    {
    	public EditStudyWorkQueueData()
    	{
    		EditRequest = new EditRequest();
    	}

    	public EditRequest EditRequest { get; set; }
    }

    public class EditStudyWorkQueueDataParser
    {
        public EditStudyWorkQueueData Parse(XmlElement element)
        {
            Platform.CheckForNullReference(element, "element");

            if (element.Name == "editstudy")
            {
                return ParseOldXml(element);
            }
        	return XmlUtils.Deserialize<EditStudyWorkQueueData>(element);
        }

        private static EditStudyWorkQueueData ParseOldXml(XmlElement element)
        {
            var data = new EditStudyWorkQueueData();

            var compiler = new WebEditStudyCommandCompiler();
            List<BaseImageLevelUpdateCommand> updateCommands = compiler.Compile(element);

            foreach (BaseImageLevelUpdateCommand command in updateCommands)
            {
                if (data.EditRequest.UpdateEntries==null)
                    data.EditRequest.UpdateEntries = new List<UpdateItem>();

                // convert BaseImageLevelUpdateCommand to UpdateItem
                string value = command.UpdateEntry.Value != null ? command.UpdateEntry.Value.ToString() : null;
                data.EditRequest.UpdateEntries.Add(new UpdateItem(command.UpdateEntry.TagPath.Tag.TagValue, command.UpdateEntry.OriginalValue, value));
            }

            return data;
           
        }
    }
}
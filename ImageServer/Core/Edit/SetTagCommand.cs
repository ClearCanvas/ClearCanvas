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
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageServer.Common.Helpers;

namespace ClearCanvas.ImageServer.Core.Edit
{
	/// <summary>
	/// Represents a command that can be executed on an <see cref="DicomFile"/>
	/// </summary>
	/// <remarks>
	/// This class is serializable.
	/// </remarks>
	[XmlRoot("SetTag")]
	public class SetTagCommand : BaseImageLevelUpdateCommand
	{
	    #region Private Fields

	    #endregion

		#region Constructors
		/// <summary>
		/// **** For serialization purpose. ****
		/// </summary>
		public SetTagCommand()
			: base("SetTag")
		{
		}

        /// <summary>
        /// Creates an instance of <see cref="SetTagCommand"/> that can be used to update the specified dicom tag with the specified value
        /// </summary>
        /// <remarks>
        /// <see cref="BaseImageLevelUpdateCommand.File"/> must be set prior to <see cref="BaseImageLevelUpdateCommand.OnExecute"></see>
        /// </remarks>
        public SetTagCommand(DicomAttribute attribute, string newValue)
            : this(attribute.Tag.TagValue, String.Empty, newValue)
        {
        }


		/// <summary>
		/// Creates an instance of <see cref="SetTagCommand"/> that can be used to update the specified dicom tag with the specified value
		/// </summary>
		/// <remarks>
		/// <see cref="BaseImageLevelUpdateCommand.File"/> must be set prior to <see cref="BaseImageLevelUpdateCommand.OnExecute"></see>
		/// </remarks>
		public SetTagCommand(uint tag, string originalValue, string value)
			: this()
		{
		    var dicomTag = DicomTagDictionary.GetDicomTag(tag);
            UpdateEntry.TagPath = new DicomTagPath { Tag = dicomTag };

            if (!string.IsNullOrEmpty(value))
            {
                int maxLength = dicomTag.VR.Equals(DicomVr.PNvr) ? 64 : (int)dicomTag.VR.MaximumLength;
                if (value.Length > maxLength)
                    value = value.Substring(0, maxLength);
            }

		    UpdateEntry.Value = value;
            UpdateEntry.OriginalValue = originalValue;
		}

        /// <summary>
        /// Creates an instance of <see cref="SetTagCommand"/> that can be used to update the specified dicom tag with the specified value
        /// </summary>
        /// <remarks>
        /// <see cref="BaseImageLevelUpdateCommand.File"/> must be set prior to <see cref="BaseImageLevelUpdateCommand.OnExecute"></see>
        /// </remarks>
        public SetTagCommand(uint tag, string newValue)
            : this(tag,String.Empty, newValue)
        {
            
        }

        /// <summary>
        /// Creates an instance of <see cref="SetTagCommand"/> that can be used to set the value of the specified dicom tag 
        /// in the <see cref="DicomFile"/>
        /// </summary>
        /// <remarks>
        /// <see cref="BaseImageLevelUpdateCommand.File"/> must be set prior to <see cref="BaseImageLevelUpdateCommand.OnExecute"></see>
        /// </remarks>
        public SetTagCommand(DicomFile file, uint tag, string originalValue, string value)
            : this(tag, originalValue, value)
        {
            File = file;
            
        }
		#endregion

		#region Public Properties

	    /// <summary>
		/// Gets the name of the Dicom tag affected by this command.
		/// **** For XML serialization purpose. ****
		/// </summary>
		[XmlAttribute(AttributeName = "TagName")]
		public string TagName
		{
			get { return UpdateEntry.TagPath.Tag.Name; }

			// Leave 'set' to public. It's used for serialization
            set {
				// NO-OP
			}
		}

        /// <summary>
        /// Gets/Sets the original value of the tag to be updated.
        /// **** For XML serialization purpose. ****
        /// </summary>
        [XmlAttribute(AttributeName = "OriginalValue")]
        public string OriginalValue
	    {
            get { return UpdateEntry.OriginalValue; }
            set
            {
                UpdateEntry.OriginalValue = value;
            }
	    }

		/// <summary>
		/// Gets or sets the Dicom tag value to be used by this command when updating the dicom file.
		/// </summary>
		[XmlAttribute(AttributeName = "Value")]
		public string Value
		{
			get
			{
				if (UpdateEntry == null)
					return null;

				return UpdateEntry.Value != null ? UpdateEntry.Value.ToString() : null;
			}
			set
			{
				UpdateEntry.Value = value;
			}
		}

		[XmlAttribute(AttributeName = "TagPath")]
		public string TagPath
		{
			get { return UpdateEntry.TagPath.HexString(); }
			set
			{
				DicomTagPathConverter converter = new DicomTagPathConverter();
				UpdateEntry.TagPath = (DicomTagPath)converter.ConvertFromString(value);
			}
		}

		[XmlIgnore]
		public DicomTag Tag
		{
			get
			{
				return UpdateEntry.TagPath.Tag;
			}
		}

	    #endregion

		#region IImageLevelCommand Members

		public override bool Apply(DicomFile file, OriginalAttributesSequence originalAttributes)
		{
			if (UpdateEntry != null)
			{
				DicomAttribute attr = FindAttribute(file.DataSet, UpdateEntry);
				if (attr != null)
				{
					var copiedAttrib = attr.Copy();
					originalAttributes.ModifiedAttributesSequence[copiedAttrib.Tag] = copiedAttrib;

				    UpdateEntry.OriginalValue = attr.ToString();
					try
					{
					    var desiredValue = UpdateEntry.GetStringValue();
                        attr.SetStringValue(desiredValue);

                        if (!string.IsNullOrEmpty(desiredValue))
                        {
                            //Make sure the data is not garbled when stored into file
                            EnsureCharacterSetIsGood(file, attr, desiredValue);
                        }
                       
					}
                    catch(DicomCharacterSetException)
                    {
                        throw; //rethrow
                    }
					catch (DicomDataException)
					{
                        //TODO: Why do we ignore it?
						Platform.Log(LogLevel.Warn, "Unexpected exception when updating tag {0} to value {1}, leaving current value: {2}",
						             UpdateEntry.TagPath, UpdateEntry.GetStringValue(),
						             attr.ToString());
						UpdateEntry.Value = attr.ToString();
					}
				}
			}

			return true;
		}

        public bool CanSaveInUnicode
	    {
            get
            {
                return ImageServer.Common.Settings.Default.AllowedConvertToUnicodeOnEdit;
            }
	    }

	    public override string ToString()
		{
			return String.Format("Set {0}={1} [Original={2}]", UpdateEntry.TagPath.Tag, UpdateEntry.Value, UpdateEntry.OriginalValue?? "N/A or TBD");
		}
		
        #endregion

		protected static DicomAttribute FindAttribute(DicomAttributeCollection collection, ImageLevelUpdateEntry entry)
		{
			if (collection == null)
				return null;

			if (entry.TagPath.Parents != null)
			{
				foreach (DicomTag tag in entry.TagPath.Parents)
				{
					DicomAttribute sq = collection[tag] as DicomAttributeSQ;
					if (sq == null)
					{
						throw new Exception(String.Format("Invalid tag value: {0}({1}) is not a SQ VR", tag, tag.Name));
					}
					if (sq.IsEmpty)
					{
						DicomSequenceItem item = new DicomSequenceItem();
						sq.AddSequenceItem(item);
					}

					DicomSequenceItem[] items = sq.Values as DicomSequenceItem[];
					Platform.CheckForNullReference(items, "items");
					collection = items[0];
				}
			}

			return collection[entry.TagPath.Tag];
        }


        #region Private Methods

        /// <summary>
        /// Compares two string and return the position where they differ.
        /// </summary>
        /// <remarks>The position is zero-based.</remarks>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        private static int Diff(DicomTag tag, string s1, string s2)
        {
            if (s1 != null)
            {
                s1 = s1.Trim(new char[] { tag.VR.PadChar, '\0' });
            }
            if (s2 != null)
            {
                s2 = s2.Trim(new char[] { tag.VR.PadChar, '\0' });
            }

            
            if (string.Equals(s1,s2)) 
                return -1;

            if (s1==null || s2==null)
            {
                // Because we call string.Equals(), if one of them is null, the other is not null
                return 0;
            }

            int index = 0;
            for (; index < s1.Length && index < s2.Length; index++)
            {
                var  ch = s1[index];
                if (!s2[index].Equals(ch))
                    break;
            }

            return index;
        }


        /// <summary>
        /// Makes sure the new value can be encoded with the specific character set in the dicom file header. If necessary and if it's allowed,
        /// convert the dicom file to use unicode (ISO_IR 192).
        /// </summary>
        /// <param name="file"></param>
        /// <param name="attr"></param>
        /// <param name="desiredValue"></param>
        /// <exception cref="DicomCharacterSetException">Thrown if unicode is not allowed or (for some reason) the value cannot be encoded using unicode</exception>
        private void EnsureCharacterSetIsGood(DicomFile file, DicomAttribute attr, string desiredValue)
        {
            var encodedValue = attr.GetEncodedString(file.TransferSyntax, file.DataSet.SpecificCharacterSet);

            var diff = Diff(attr.Tag, encodedValue, desiredValue);
            if (diff < 0)
            {
                // it's all good
                return;
            }

            // The current specific character set does not support the new value. Try to encode it using unicode (if it's configured)
            if (!CanSaveInUnicode)
            {
                Platform.Log(LogLevel.Debug, "'{0}' cannot be encoded using current Character Set and Unicode is not allowed. Aborting..", desiredValue);
                
                //Throw DicomCharacterSetException
                var characterSetDescriptions = SpecificCharacterSetParser.GetCharacterSetInfoDescriptions(file.DataSet.SpecificCharacterSet);
                string instanceNumber = file.DataSet[DicomTags.InstanceNumber].ToString();
                string instanceUid = file.DataSet[DicomTags.SopInstanceUid].ToString();
                char badChar = diff >= desiredValue.Length ? desiredValue[desiredValue.Length - 1] : desiredValue[diff];
                var error = string.Format("SOP {5}\n\nCannot set {0} to {1}. Character {2} is not covered by character set {3} [{4}].",
                                          UpdateEntry.TagPath.Tag.Name, desiredValue, badChar, file.DataSet.SpecificCharacterSet,
                                          StringUtilities.Combine(characterSetDescriptions, ","),
                                          string.Format("#{0} [{1}]", instanceNumber, instanceUid)
                    );

                Platform.Log(LogLevel.Error, error);
                throw new DicomCharacterSetException(UpdateEntry.TagPath.Tag.TagValue, file.DataSet.SpecificCharacterSet, desiredValue, error);
            }


            // The attribute value has been set, only need to set specific character set to unicode
            const string newSpecificCharacterSet = "ISO_IR 192";

            // Before we commit the change, let's verify again if that's good.
            Platform.Log(LogLevel.Debug, "'{0}'cannot be encoded using current Character Set and Unicode is allowed. Checking for value consistency if switching to Unicode encoding", desiredValue);
            encodedValue = attr.GetEncodedString(file.TransferSyntax, newSpecificCharacterSet);
            diff = Diff(attr.Tag, encodedValue, desiredValue);
            if (diff >= 0)
            {
                // not ok?
                var characterSetDescriptions = SpecificCharacterSetParser.GetCharacterSetInfoDescriptions(file.DataSet.SpecificCharacterSet);
                string instanceNumber = file.DataSet[DicomTags.InstanceNumber].ToString();
                string instanceUid = file.DataSet[DicomTags.SopInstanceUid].ToString();
                char badChar = diff >= desiredValue.Length ? desiredValue[desiredValue.Length - 1] : desiredValue[diff];
                var error = string.Format("SOP {5}\n\nCannot set {0} to {1}. Character {2} is not covered by character set {3} [{4}]. Attempt to use {6} did not solve the problem.",
                                          UpdateEntry.TagPath.Tag.Name, desiredValue, badChar, file.DataSet.SpecificCharacterSet,
                                          StringUtilities.Combine(characterSetDescriptions, ","),
                                          string.Format("#{0} [{1}]", instanceNumber, instanceUid),
                                          newSpecificCharacterSet
                    );

                Platform.Log(LogLevel.Error, error);
                throw new DicomCharacterSetException(UpdateEntry.TagPath.Tag.TagValue, file.DataSet.SpecificCharacterSet, desiredValue, error);
            }

            Platform.Log(LogLevel.Debug, "Specific Character Set for SOP {0} is now changed to unicode", file.MediaStorageSopInstanceUid);

            file.DataSet.SpecificCharacterSet = newSpecificCharacterSet;
            file.DataSet[DicomTags.SpecificCharacterSet].SetStringValue(newSpecificCharacterSet);
        }

        #endregion

    }
}
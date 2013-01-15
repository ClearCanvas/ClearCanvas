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
using System.Text;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Class representing a DICOM Message to be transferred over the network.
    /// </summary>
    /// <seealso cref="DicomMessageBase"/>
    /// <seealso cref="DicomFile"/>
    public class DicomMessage : DicomMessageBase
    {
        #region Private Members
        private TransferSyntax _transferSyntax = TransferSyntax.ExplicitVrLittleEndian;
        #endregion

        #region Command Element Properties
        /// <summary>
        /// The affected SOP Class UID associated with the oepration.
        /// </summary>
        public string AffectedSopClassUid
        {
            get { return MetaInfo[DicomTags.AffectedSopClassUid].GetString(0,String.Empty); }
            set { MetaInfo[DicomTags.AffectedSopClassUid].Values = value; }
        }
        /// <summary>
        /// The requested SOP Class UID associated with the operation.
        /// </summary>
        public string RequestedSopClassUid
        {
            get { return MetaInfo[DicomTags.RequestedSopClassUid].GetString(0, String.Empty); }
            set { MetaInfo[DicomTags.RequestedSopClassUid].Values = value; }
        }
        /// <summary>
        /// This field distinguishes the DIMSE operation conveyed by this Message.
        /// </summary>
        public DicomCommandField CommandField
        {
            get
            {
                ushort command = MetaInfo[DicomTags.CommandField].GetUInt16(0, (ushort)DicomCommandField.CStoreRequest);
                return (DicomCommandField)command;
            }
            set
            {
                MetaInfo[DicomTags.CommandField].Values = (ushort)value;
            }
        }
        /// <summary>
        /// An implementation specific value which distinguishes thsi Message from other Messages.
        /// </summary>
        public ushort MessageId
        {
            get { return MetaInfo[DicomTags.MessageId].GetUInt16(0, 0); }
            set { MetaInfo[DicomTags.MessageId].Values = value; }
        }
        /// <summary>
        /// Shall be set to the value of the Message ID (0000,0110) field used in the associated request Message.
        /// </summary>
        public ushort MessageIdBeingRespondedTo
        {
            get { return MetaInfo[DicomTags.MessageIdBeingRespondedTo].GetUInt16(0, 0); }
            set { MetaInfo[DicomTags.MessageIdBeingRespondedTo].Values = value; }
        }
        /// <summary>
        /// Shall be set to the DICOM AE Ttile of the destination DICOM AE for which the C-STORE sub-operations are being performed.
        /// </summary>
        public string MoveDestination
        {
            get { return MetaInfo[DicomTags.MoveDestination].GetString(0, String.Empty); }
            set { MetaInfo[DicomTags.MoveDestination].Values = value; }
        }
        /// <summary>
        /// The priority shall be set to one of the following values: 
        /// <para>LOW = 0002H</para>
        /// <para>MEDIUM = 0000H</para>
        /// <para>HIGH = 0001H</para>
        /// </summary>
        public DicomPriority Priority
        {
            get
            {
                ushort priority = MetaInfo[DicomTags.Priority].GetUInt16(0, (ushort)DicomPriority.Medium);
                return (DicomPriority)priority;
            }
            set
            {
                MetaInfo[DicomTags.Priority].Values = (ushort)value;
            }
        }
        /// <summary>
        /// This field indicates if a Data Set is present in the Message.  This field shall be set to the value
        /// of 0101H if no Data Set is present, any other value indicates a Data Set is included in the Message.
        /// </summary>
        public ushort DataSetType
        {
            get { return MetaInfo[DicomTags.DataSetType].GetUInt16(0, 0); }
            set { MetaInfo[DicomTags.DataSetType].Values = value; }
        }
        /// <summary>
        /// Confirmation status of the operation.
        /// </summary>
        public DicomStatus Status
        {
            get
            {
                ushort status = MetaInfo[DicomTags.Status].GetUInt16(0, 0);
                return DicomStatuses.Lookup(status);
            }
            set
            {
                MetaInfo[DicomTags.Status].Values = value.Code;
            }
        }
        /// <summary>
        /// If status is Cxxx, then this field contains a list of the elements in which the error was detected.
        /// </summary>
        public uint[] OffendingElement
        {
            get { return (uint[])MetaInfo[DicomTags.OffendingElement].Values; }
            set { MetaInfo[DicomTags.OffendingElement].Values = value; }
        }
        /// <summary>
        /// This field contains an application-specific text description of the error detected.
        /// </summary>
        public string ErrorComment
        {
            get { return MetaInfo[DicomTags.ErrorComment].GetString(0, String.Empty); }
            set { MetaInfo[DicomTags.ErrorComment].Values = value; }
        }
        /// <summary>
        /// This field shall optionally contain an application-specific error code.
        /// </summary>
        public ushort ErrorId
        {
            get { return MetaInfo[DicomTags.ErrorId].GetUInt16(0, 0); }
            set { MetaInfo[DicomTags.ErrorId].Values = value; }
        }
        /// <summary>
        /// Contains the UID of the SOP Instance for which this operation occurred.
        /// </summary>
        public string AffectedSopInstanceUid
        {
            get { return MetaInfo[DicomTags.AffectedSopInstanceUid].GetString(0, String.Empty); }
            set { MetaInfo[DicomTags.AffectedSopInstanceUid].Values = value; }
        }
        /// <summary>
        /// Contains the UID of the SOP Instance for which this operation occurred.
        /// </summary>
        public string RequestedSopInstanceUid
        {
            get { return MetaInfo[DicomTags.RequestedSopInstanceUid].GetString(0, String.Empty); }
            set { MetaInfo[DicomTags.RequestedSopInstanceUid].Values = value; }
        }
        /// <summary>
        /// Values for this field are application-specific.
        /// </summary>
        public ushort EventTypeId
        {
            get { return MetaInfo[DicomTags.EventTypeId].GetUInt16(0, 0); }
            set { MetaInfo[DicomTags.EventTypeId].Values = value; }
        }
        /// <summary>
        /// This field contains an Attribute Tag for each of the n Attributes applicable.
        /// </summary>
        public uint[] AttributeIdentifierList
        {
            get { return (uint[])MetaInfo[DicomTags.AttributeIdentifierList].Values; }
            set { MetaInfo[DicomTags.AttributeIdentifierList].Values = value; }
        }
        /// <summary>
        /// Values for this field are application-specific.
        /// </summary>
        public ushort ActionTypeId
        {
            get { return MetaInfo[DicomTags.ActionTypeId].GetUInt16(0, 0); }
            set { MetaInfo[DicomTags.ActionTypeId].Values = value; }
        }
        /// <summary>
        /// The number of reamining C-STORE sub-operations to be 
        /// invoked for the operation.
        /// </summary>
        public ushort NumberOfRemainingSubOperations
        {
            get { return MetaInfo[DicomTags.NumberOfRemainingSubOperations].GetUInt16(0, 0); }
            set { MetaInfo[DicomTags.NumberOfRemainingSubOperations].Values = value; }
        }
        /// <summary>
        /// The number of C-STORE sub-operations associated with this operation which have 
        /// completed successfully.
        /// </summary>
        public ushort NumberOfCompletedSubOperations
        {
            get { return MetaInfo[DicomTags.NumberOfCompletedSubOperations].GetUInt16(0, 0); }
            set { MetaInfo[DicomTags.NumberOfCompletedSubOperations].Values = value; }
        }
        /// <summary>
        /// The number of C-STORE sub-operations associated with this operation which
        /// have failed.
        /// </summary>
        public ushort NumberOfFailedSubOperations
        {
            get { return MetaInfo[DicomTags.NumberOfFailedSubOperations].GetUInt16(0, 0); }
            set { MetaInfo[DicomTags.NumberOfFailedSubOperations].Values = value; }
        }
        /// <summary>
        /// The number of C-STORE sub-operations associated with this operation which 
        /// generated warning responses.
        /// </summary>
        public ushort NumberOfWarningSubOperations
        {
            get { return MetaInfo[DicomTags.NumberOfWarningSubOperations].GetUInt16(0, 0); }
            set { MetaInfo[DicomTags.NumberOfWarningSubOperations].Values = value; }
        }
        /// <summary>
        /// Contains the DICOM AE Title of the DICOM AE which invoked the C-MOVE operation from which this
        /// C-STORE sub-operation is being performed.
        /// </summary>
        public string MoveOriginatorApplicationEntityTitle
        {
            get { return MetaInfo[DicomTags.MoveOriginatorApplicationEntityTitle].GetString(0, String.Empty); }
            set { MetaInfo[DicomTags.MoveOriginatorApplicationEntityTitle].Values = value; }
        }
        /// <summary>
        /// Contains the Message ID (0000,0110) of the C-MOVE-RQ Message from which this
        /// C-STORE sub-operations is being performed.
        /// </summary>
        public ushort MoveOriginatorMessageId
        {
            get { return MetaInfo[DicomTags.MoveOriginatorMessageId].GetUInt16(0, 0); }
            set { MetaInfo[DicomTags.MoveOriginatorMessageId].Values = value; }
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// A <see cref="DicomAttributeCollection"/> instance representing the group 0x000 elements within the message.
        /// </summary>
        public DicomAttributeCollection CommandSet
        {
            get { return MetaInfo; }
        }

        /// <summary>
        /// The <see cref="SopClass"/> associated with the message.
        /// </summary>
        /// <remarks>If the SOP Clas is unknown, an new SopClass instance is
        /// returned with the SOP Class UID set appropriately.</remarks>
        public SopClass SopClass
        {
            get
            {
                String sopClassUid = DataSet[DicomTags.SopClassUid].GetString(0, String.Empty);

                SopClass sop = SopClass.GetSopClass(sopClassUid) ??
                               new SopClass("Unknown Sop Class", sopClassUid, false);

                return sop;
            }
        }

        public override TransferSyntax TransferSyntax
        {
            get { return _transferSyntax; }
            set { _transferSyntax = value; }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for creating a new DicomMessage instance from existing command and data sets.
        /// </summary>
        /// <param name="command">The command set.</param>
        /// <param name="data">The data set.</param>
        public DicomMessage(DicomAttributeCollection command, DicomAttributeCollection data)
        {
            MetaInfo = command ?? new DicomAttributeCollection(0x00000000, 0x0000FFFF);

            DataSet = data ?? new DicomAttributeCollection(0x00040000,0xFFFFFFFF);
        }

        /// <summary>
        /// Creates a new DicomMessage instance from an existing <see cref="DicomFile"/>.
        /// </summary>
        /// <remarks>
        /// This method creates a new command set for the DicomMessage, but shares the DataSet with <paramref name="file"/>.
        /// </remarks>
        /// <param name="file">The <see cref="DicomFile"/> to change into a DicomMessage.</param>
        public DicomMessage(DicomFile file)
        {
            _transferSyntax = file.TransferSyntax;
            MetaInfo = new DicomAttributeCollection(0x00000000,0x0000FFFF);
            DataSet = file.DataSet;
        }

        /// <summary>
        /// Default constructor that creates an empty message.
        /// </summary>
        public DicomMessage()
        {
            MetaInfo = new DicomAttributeCollection(0x00000000, 0x0000FFFF);
            DataSet = new DicomAttributeCollection(0x00040000, 0xFFFFFFFF);
        }
        #endregion

        #region Dump
        /// <summary>
        /// Dump the contents of the message to a StringBuilder.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="prefix"></param>
        /// <param name="options"></param>
        public override void Dump(StringBuilder sb, string prefix, DicomDumpOptions options)
        {
            if (sb == null) throw new NullReferenceException("sb");
            sb.Append(prefix).Append("Command Elements:").AppendLine();
            MetaInfo.Dump(sb, prefix, options);
            sb.AppendLine().Append(prefix).Append("Data Set:").AppendLine();
            DataSet.Dump(sb, prefix, options);
            sb.AppendLine();
        }
        #endregion
    }
}

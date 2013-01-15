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

namespace ClearCanvas.Dicom.Iod.Iods
{
	public abstract class QueryIodBase : IodBase
	{
		#region Constructors
        /// <summary>
		/// Initializes a new instance of the <see cref="QueryIodBase"/> class.
        /// </summary>
        public QueryIodBase() : base()
        {
            SetAttributeFromEnum(DicomAttributeProvider[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Series);
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="QueryIodBase"/> class.
        /// </summary>
		public QueryIodBase(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
            SetAttributeFromEnum(DicomAttributeProvider[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Series);
        }
        #endregion


		/// <summary>
		/// Gets or sets the specific character set.
		/// </summary>
		/// <value>The specific character set.</value>
		public string SpecificCharacterSet
		{
			get { return DicomAttributeProvider[DicomTags.SpecificCharacterSet].GetString(0, String.Empty); }
			set { DicomAttributeProvider[DicomTags.SpecificCharacterSet].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the Retrieve AE Title.
		/// </summary>
		/// <value>The Retrieve AE Title.</value>
		public string RetrieveAeTitle
		{
			get { return DicomAttributeProvider[DicomTags.RetrieveAeTitle].GetString(0, String.Empty); }
			set { DicomAttributeProvider[DicomTags.RetrieveAeTitle].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the Storage Media Fileset Id.
		/// </summary>
		/// <value>The media Fileset Id.</value>
		public string StorageMediaFileSetId
		{
			get { return DicomAttributeProvider[DicomTags.StorageMediaFileSetId].GetString(0, String.Empty); }
			set { DicomAttributeProvider[DicomTags.StorageMediaFileSetId].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the Storage Media Fileset Uid.
		/// </summary>
		/// <value>The media Fileset Uid.</value>
		public string StorageMediaFileSetUid
		{
			get { return DicomAttributeProvider[DicomTags.StorageMediaFileSetUid].GetString(0, String.Empty); }
			set { DicomAttributeProvider[DicomTags.StorageMediaFileSetUid].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the query retrieve level.
		/// </summary>
		/// <value>The query retrieve level.</value>
		public QueryRetrieveLevel QueryRetrieveLevel
		{
			get
			{
				if (!DicomAttributeProvider[DicomTags.QueryRetrieveLevel].IsEmpty)
				{
					try
					{
						return (QueryRetrieveLevel)Enum.Parse(typeof(QueryRetrieveLevel), DicomAttributeProvider[DicomTags.QueryRetrieveLevel].GetString(0, QueryRetrieveLevel.None.ToString()), true);
					}
					catch (Exception)
					{
						return QueryRetrieveLevel.None;
					}
				}
				return QueryRetrieveLevel.None;

			}
			set
			{
				SetAttributeFromEnum(DicomAttributeProvider[DicomTags.QueryRetrieveLevel], value);
			}
		}

		/// <summary>
		/// Gets or sets the Instance Availability
		/// </summary>
		public InstanceAvailability InstanceAvailability
		{
			get
			{
				if (!DicomAttributeProvider[DicomTags.InstanceAvailability].IsEmpty)
				{
					try
					{
						return (InstanceAvailability)Enum.Parse(typeof(InstanceAvailability), DicomAttributeProvider[DicomTags.InstanceAvailability].GetString(0, InstanceAvailability.Unknown.ToString()), true);
					}
					catch (Exception)
					{
						return InstanceAvailability.Unknown;
					}
				}
				return InstanceAvailability.Unknown;
			}
			set { SetAttributeFromEnum(DicomAttributeProvider[DicomTags.InstanceAvailability], value); }
		}
	}

	#region InstanceAvailability Enum
	/// <summary>
	/// <see cref="DicomTags.InstanceAvailability"/>
	/// </summary>
	public enum InstanceAvailability
	{
		/// <summary>
		/// The instances are immediately available
		/// </summary>
		Online,
		/// <summary>
		/// The instances need to be retrieved from relatively slow media such as optical disk or tape
		/// </summary>
		Nearline,
		/// <summary>
		/// The instances need to be retrieved by manual intervention
		/// </summary>
		Offline,
		/// <summary>
		/// The instances cannot be retrieved. Note that SOP Instances that are unavailable may have an 
		/// alternate representation that is available (see section C.6.1.1.5.1).
		/// </summary>
		Unknown
	}

	#endregion

	#region QueryRetrieveLevel Enum
	/// <summary>
	/// 
	/// </summary>
	public enum QueryRetrieveLevel
	{
		/// <summary>
		/// 
		/// </summary>
		None,
		/// <summary>
		/// 
		/// </summary>
		Patient,
		/// <summary>
		/// 
		/// </summary>
		Study,
		/// <summary>
		/// 
		/// </summary>
		Series,
		/// <summary>
		/// 
		/// </summary>
		Image
	}

	#endregion
}

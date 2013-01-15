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
using ClearCanvas.Dicom.Utilities.Xml;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// Provides access to the common settings of the image server from external assemlies.
    /// </summary>
    public class ImageServerCommonConfiguration
    {
        /// <summary>
        /// Retrieves the default <see cref="StudyXmlOutputSettings"/> based on the configuration settings.
        /// </summary>
        static public StudyXmlOutputSettings DefaultStudyXmlOutputSettings
        {
            get
            {
                StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
				if (Settings.Default.StudyHeaderIncludePrivateTags)
					settings.IncludePrivateValues = StudyXmlTagInclusion.IncludeTagValue;
				else
					settings.IncludePrivateValues = StudyXmlTagInclusion.IgnoreTag;

				if (Settings.Default.StudyHeaderIncludeUNTags)
					settings.IncludeUnknownTags = StudyXmlTagInclusion.IncludeTagValue;
				else
					settings.IncludeUnknownTags = StudyXmlTagInclusion.IgnoreTag;

                settings.MaxTagLength = Settings.Default.StudyHeaderMaxValueLength;
            	settings.IncludeLargeTags = StudyXmlTagInclusion.IncludeTagExclusion;

            	settings.IncludeSourceFileName = false;
                return settings;
            }
        }

        static public string DefaultStudyRootFolder
        {
            get
            {
                return Settings.Default.DefaultStudyRootFolder;
            }
        }

        static public bool UseReceiveDateAsStudyFolder
        {
            get { return Settings.Default.UseReceiveDateAsFolder; }
        }

        static public int TooManyStudyMoveWarningThreshold
        {
            get
            {
                return Settings.Default.TooManyStudyMoveWarningThreshold;
            }
        }

        public static String TemporaryPath
        {
            get
            {
                return Settings.Default.TemporaryPath;
            }
        }

        public static class Device
        {
            public static short MaxConnections
            {
                get
                {
                    return Settings.Default.DeviceConfig_MaxConnections;
                }
            }
        }

		public static int WorkQueueMaxFailureCount
    	{
    		get
    		{
				return Settings.Default.WorkQueueMaxFailureCount;
    		}
    	}
        
    }
}

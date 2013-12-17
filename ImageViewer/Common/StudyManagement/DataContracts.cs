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

using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Dicom.ServiceModel.Query;
using System;
using System.Text.RegularExpressions;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    public static class StudyManagementNamespace
    {
        public const string Value = ImageViewerNamespace.Value + "/studyManagement";
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetStudyCountRequest : DataContractBase
    {
        [DataMember(IsRequired = false)]
        public StudyEntry Criteria { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetStudyCountResult : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public int StudyCount { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetStudyEntriesRequest : DataContractBase
    {
        // TODO (CR Jun 2012): Should criteria be expanded to actual supported properties, rather than an "entry". Confusing.
        [DataMember(IsRequired = false)]
        public StudyEntry Criteria { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetStudyEntriesResult : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public IList<StudyEntry> StudyEntries  { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetSeriesEntriesRequest : DataContractBase
    {
        // TODO (CR Jun 2012): Should criteria be expanded to actual supported properties, rather than an "entry". Confusing.
        [DataMember(IsRequired = false)]
        public SeriesEntry Criteria { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetSeriesEntriesResult : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public IList<SeriesEntry> SeriesEntries { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetImageEntriesRequest : DataContractBase
    {
        // TODO (CR Jun 2012): Should criteria be expanded to actual supported properties, rather than an "entry". Confusing.
        [DataMember(IsRequired = false)]
        public ImageEntry Criteria { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetImageEntriesResult : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public IList<ImageEntry> ImageEntries { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public abstract class StoreEntry : DataContractBase
    {
    }

    public interface IStoreEntry
    {
        Identifier Identifier { get; }
    }

    public interface IStoreEntry<out T> where T : Identifier
    {
        T Identifier { get; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class StudyEntry : StoreEntry, IStoreEntry, IStoreEntry<StudyRootStudyIdentifier>
    {
        [DataMember(IsRequired = true)]
        public StudyRootStudyIdentifier Study { get; set; }

        [DataMember(IsRequired = false)]
        public StudyEntryData Data { get; set; }

        #region IStoreEntry Members

        Identifier IStoreEntry.Identifier
        {
            get { return Study; }
        }

        #endregion

        #region IStoreEntry<StudyRootStudyIdentifier> Members

        StudyRootStudyIdentifier IStoreEntry<StudyRootStudyIdentifier>.Identifier
        {
            get { return Study; }
        }

        #endregion
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class SeriesEntry : StoreEntry, IStoreEntry, IStoreEntry<SeriesIdentifier>
    {
        [DataMember(IsRequired = true)]
        public SeriesIdentifier Series { get; set; }

        [DataMember(IsRequired = false)]
        public SeriesEntryData Data { get; set; }

        #region IStoreEntry Members

        Identifier IStoreEntry.Identifier
        {
            get { return Series; }
        }

        #endregion

        #region IStoreEntry<SeriesIdentifier> Members

        SeriesIdentifier IStoreEntry<SeriesIdentifier>.Identifier
        {
            get { return Series; }
        }

        #endregion
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class ImageEntry : StoreEntry, IStoreEntry, IStoreEntry<ImageIdentifier>
    {
        [DataMember(IsRequired = true)]
        public ImageIdentifier Image { get; set; }

        [DataMember(IsRequired = false)]
        public ImageEntryData Data { get; set; }

        #region IStoreEntry Members

        Identifier IStoreEntry.Identifier
        {
            get { return Image; }
        }

        #endregion

        #region IStoreEntry<ImageIdentifier> Members

        ImageIdentifier IStoreEntry<ImageIdentifier>.Identifier
        {
            get { return Image; }
        }

        #endregion
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetStorageConfigurationResult
    {
        [DataMember(IsRequired = true)]
        public StorageConfiguration Configuration { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetStorageConfigurationRequest
    { }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class UpdateStorageConfigurationResult
    {
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class UpdateStorageConfigurationRequest
    {
        [DataMember(IsRequired = true)]
        public StorageConfiguration Configuration { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class StorageConfiguration
    {
		[DataContract(Namespace = StudyManagementNamespace.Value)]
        public class DeletionRule : IEquatable<DeletionRule>
        {
            [DataMember(IsRequired = true)]
            public bool Enabled { get; set; }

            [DataMember(IsRequired = true)]
            public int TimeValue { get; set; }

            [DataMember(IsRequired = true)]
            public TimeUnit TimeUnit { get; set; }

            public DeletionRule Clone()
            {
                return new DeletionRule
                {
                    Enabled = this.Enabled,
                    TimeUnit = this.TimeUnit,
                    TimeValue = this.TimeValue
                };
            }

            public bool Equals(DeletionRule other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.Enabled.Equals(Enabled) && other.TimeValue == TimeValue && Equals(other.TimeUnit, TimeUnit);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(DeletionRule)) return false;
                return Equals((DeletionRule)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = Enabled.GetHashCode();
                    result = (result * 397) ^ TimeValue;
                    result = (result * 397) ^ TimeUnit.GetHashCode();
                    return result;
                }
            }
		}

        public const double AutoMinimumFreeSpace = -1;

        private string _fileStoreDirectory;
        private bool _fileStoreDiskspaceInitialized;
        private Diskspace _fileStoreDiskspace;
        private double _minimumFreeSpacePercent = AutoMinimumFreeSpace;

        #region Data Members

        [DataMember(IsRequired = true)]
        public string FileStoreDirectory
        {
            get { return _fileStoreDirectory; }
            set
            {
                _fileStoreDirectory = value;
                FileStoreDiskSpace = null;
            }
        }

        [DataMember(IsRequired = true)]
        public double MinimumFreeSpacePercent
        {
            get { return _minimumFreeSpacePercent; }
            set
            {
                if (value < 0)
                {
                    _minimumFreeSpacePercent = AutoMinimumFreeSpace;
                    return;
                }

                if (value > 100)
                    throw new ArgumentException("Value must be between 0 and 100.", "MinimumFreeSpacePercent");

                _minimumFreeSpacePercent = value;
            }
        }

        [DataMember(IsRequired = true)]
        public DeletionRule DefaultDeletionRule { get; set; }

        #endregion

        public string FileStoreRootPath
        {
            get
            {
                if (String.IsNullOrEmpty(FileStoreDirectory))
                    return null;

                if (FileStoreDirectory.IndexOfAny(Path.GetInvalidPathChars(), 0) >= 0)
                    return null;

                if (!Path.IsPathRooted(FileStoreDirectory))
                    return null;

                var root = Path.GetPathRoot(FileStoreDirectory);
                if (root == null)
                    return null;

                if (!Regex.IsMatch(root, @"[A-Za-z]:\\.*"))
                    return null;

                return root;
            }
        }

	    public string FileStoreIncomingFolder
	    {
		    get { return Path.Combine(FileStoreDirectory, "Incoming"); }
	    }

        public bool FileStoreDriveExists
        {
            get
            {
                var root = FileStoreRootPath;
                if (root == null)
                    return false;

                return Directory.Exists(root);
            }
        }

        public bool IsFileStoreDriveValid
        {
            get
            {
                return FileStoreRootPath != null;
            }
        }

        public string FileStoreDriveName
        {
            get
            {
                if (!FileStoreDriveExists)
                    return null;

                return FileStoreDiskSpace.DriveInfo.Name;
            }
        }

        public Diskspace FileStoreDiskSpace
        {
            get
            {
                InitializeDiskSpace();
                return _fileStoreDiskspace;
            }
            internal set
            {
                _fileStoreDiskspace = value;
                _fileStoreDiskspaceInitialized = _fileStoreDiskspace != null;
            }
        }

        public bool AutoCalculateMinimumFreeSpacePercent { get { return MinimumFreeSpacePercent < 0; } }

        public long MinimumFreeSpaceBytes
        {
            get
            {
                CheckDiskspaceAvailable();
                if (MinimumFreeSpacePercent < 0)
                    throw new InvalidOperationException("MinimumFreeSpacePercent must be set.");

                return (long)(FileStoreDiskSpace.TotalSpace * MinimumFreeSpacePercent / 100);
            }
            set { MinimumFreeSpacePercent = (double)value / FileStoreDiskSpace.TotalSpace* 100; }
        }

        public double MaximumUsedSpacePercent
        {
            get
            {
                if (MinimumFreeSpacePercent < 0)
                    throw new InvalidOperationException("MinimumFreeSpacePercent must be set.");

                return 100F - MinimumFreeSpacePercent;
            }
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentException("Value must be between 0 and 100.", "MaximumUsedSpacePercent");

                MinimumFreeSpacePercent = 100 - value;
            }
        }

        public long MaximumUsedSpaceBytes
        {
            get
            {
                CheckDiskspaceAvailable();
                return FileStoreDiskSpace.TotalSpace - MinimumFreeSpaceBytes;
            }
            set { MinimumFreeSpaceBytes = FileStoreDiskSpace.TotalSpace - value; }
        }

        /// <summary>
        /// Gets a value indicating whether or not max used space has been exceeeded.
        /// ***  Note: this is a snapshot value. For "real-time" status, use <see cref="LocalStorageMonitor"/>  ***
        /// </summary>
        public bool IsMaximumUsedSpaceExceeded
        {
            get
            {
                CheckDiskspaceAvailable();
                return FileStoreDiskSpace.UsedSpacePercent > MaximumUsedSpacePercent;
            }
        }

		public StorageConfiguration Clone()
		{
			return new StorageConfiguration
			{
				FileStoreDirectory = this.FileStoreDirectory,
				MinimumFreeSpacePercent = this.MinimumFreeSpacePercent,
                DefaultDeletionRule = this.DefaultDeletionRule == null ? null : this.DefaultDeletionRule.Clone()
			};
		}

        private bool InitializeDiskSpace()
        {
            if (_fileStoreDiskspaceInitialized)
                return true;

            //Try to initialize it only once after the file store directory changes.
            _fileStoreDiskspaceInitialized = true;

            var root = FileStoreRootPath;
            if (root == null)
                return false;

            if (!Directory.Exists(root))
                return false;

            try
            {
                var driveInfo = new DriveInfo(root);
                _fileStoreDiskspace = new Diskspace(driveInfo);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private void CheckDiskspaceAvailable()
        {
            Platform.CheckMemberIsSet(FileStoreDiskSpace, "FileStoreDiskSpace");
            Platform.CheckTrue(FileStoreDiskSpace.IsAvailable, "FileStoreDiskSpace.IsAvailable");
        }
    }
}

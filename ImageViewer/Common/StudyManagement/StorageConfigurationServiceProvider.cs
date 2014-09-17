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
using System.Globalization;
using System.IO;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
	[ExtensionOf(typeof (ServiceProviderExtensionPoint))]
	internal class StorageConfigurationServiceProvider : IServiceProvider
	{
		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			if (serviceType != typeof (IStorageConfiguration))
				return null;

			return new StorageConfigurationProxy();
		}

		#endregion
	}

	internal class StorageConfigurationProxy : IStorageConfiguration
	{
		#region Defaults

		private string DefaultFileStoreLocation
		{
			get { return Path.Combine(Platform.ApplicationDataDirectory, "filestore"); }
		}

		#endregion

		// TODO (CR Jun 2012): Move to a helper class?

		#region Implementation of IStorageConfiguration

		public GetStorageConfigurationResult GetConfiguration(GetStorageConfigurationRequest request)
		{
			var storageSettings = new StorageSettings();
			var deletionSettings = new StudyDeletionSettings();

			var configuration = new StorageConfiguration
			                    	{
			                    		FileStoreDirectory = storageSettings.FileStoreDirectory,
			                    		MinimumFreeSpacePercent = storageSettings.MinimumFreeSpacePercent,
			                    		DefaultDeletionRule = new StorageConfiguration.DeletionRule
			                    		                      	{
			                    		                      		Enabled = deletionSettings.Enabled,
			                    		                      		TimeUnit = deletionSettings.TimeUnit,
			                    		                      		TimeValue = deletionSettings.TimeValue
			                    		                      	}
			                    	};

			Complete(configuration);
			return new GetStorageConfigurationResult {Configuration = configuration};
		}

		public UpdateStorageConfigurationResult UpdateConfiguration(UpdateStorageConfigurationRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckForNullReference(request.Configuration, "Configuration");
			Platform.CheckForEmptyString(request.Configuration.FileStoreDirectory, "FileStoreDirectory");

			var newConfiguration = request.Configuration.Clone();

			//Trim before saving.
			newConfiguration.FileStoreDirectory = request.Configuration.FileStoreDirectory.Trim();
			Complete(newConfiguration);

			var storageSettings = new StorageSettings();
			var deletionSettings = new StudyDeletionSettings();
			var storageProxy = storageSettings.GetProxy();
			storageProxy.FileStoreDirectory = newConfiguration.FileStoreDirectory;
			storageProxy.MinimumFreeSpacePercent = newConfiguration.MinimumFreeSpacePercent;
			storageProxy.Save();

			if (newConfiguration.DefaultDeletionRule == null)
			{
				deletionSettings.Reset();
				deletionSettings.Save();
			}
			else
			{
				var rule = newConfiguration.DefaultDeletionRule;
				var deletionProxy = deletionSettings.GetProxy();
				deletionProxy.Enabled = rule.Enabled;
				deletionProxy.TimeUnit = rule.TimeUnit;
				deletionProxy.TimeValue = rule.TimeValue;
				deletionProxy.Save();
			}

			return new UpdateStorageConfigurationResult();
		}

		#endregion

		private void Complete(StorageConfiguration configuration)
		{
			if (String.IsNullOrEmpty(configuration.FileStoreDirectory))
				configuration.FileStoreDirectory = DefaultFileStoreLocation;

			if (configuration.AutoCalculateMinimumFreeSpacePercent)
				configuration.MinimumFreeSpaceBytes = ComputeMinimumFreeSpaceBytes(configuration.FileStoreDirectory);

			if (configuration.DefaultDeletionRule == null)
				configuration.DefaultDeletionRule = new StorageConfiguration.DeletionRule
				                                    	{
				                                    		Enabled = false,
				                                    		TimeUnit = TimeUnit.Weeks,
				                                    		TimeValue = 1
				                                    	};
		}

		private long ComputeMinimumFreeSpaceBytes(string fileStoreDirectory)
		{
			fileStoreDirectory = fileStoreDirectory ?? DefaultFileStoreLocation;
			var driveInfo = new DriveInfo(fileStoreDirectory[0].ToString(CultureInfo.InvariantCulture));
			return (long) (driveInfo.TotalSize*0.05);
		}
	}
}
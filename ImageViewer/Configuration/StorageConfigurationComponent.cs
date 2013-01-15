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
using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Services;

namespace ClearCanvas.ImageViewer.Configuration
{
    //TODO (Marmot):Move to IV.StudyManagement?

    /// <summary>
    /// Extension point for views onto <see cref="StorageConfigurationComponent"/>
    /// </summary>
    [ExtensionPoint]
	public sealed class StorageConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(StorageConfigurationComponentViewExtensionPoint))]
    public class StorageConfigurationComponent : ConfigurationApplicationComponent
    {
        private DelayedEventPublisher _delaySetFileStoreDirectory;
        private StorageConfiguration _configuration;
        private string _fileStoreDriveName;
        private string _fileStoreDirectory;
        private string _currentFileStoreDirectory;
        private bool _hasFileStoreEverChanged;
        private StorageConfiguration.DeletionRule _currentDeletionRule;

        private IWorkItemActivityMonitor _activityMonitor;

        #region Overrides

        public override void Start()
		{
            _delaySetFileStoreDirectory = new DelayedEventPublisher(RealSetFileStoreDirectory);
            _activityMonitor = WorkItemActivityMonitor.Create();
            _activityMonitor.IsConnectedChanged += ActivityMonitorOnIsConnectedChanged;

            _configuration = StudyStore.GetConfiguration();
            _fileStoreDirectory = _currentFileStoreDirectory = _configuration.FileStoreDirectory;
            
            UpdateFileStoreDriveName();
            MaximumUsedSpaceChanged();

			_currentDeletionRule = _configuration.DefaultDeletionRule.Clone();
			
			base.Start();
		}

        public override void Stop()
        {
            base.Stop();

            if (_delaySetFileStoreDirectory != null)
            {
                _delaySetFileStoreDirectory.Cancel();
                _delaySetFileStoreDirectory.Dispose();
                _delaySetFileStoreDirectory = null;
            }

            if (_activityMonitor != null)
            {
                _activityMonitor.IsConnectedChanged -= ActivityMonitorOnIsConnectedChanged;
                _activityMonitor.Dispose();
            }
        }

        public override void Save()
        {
		    try
		    {
                if (HasFileStoreChanged)
                    _hasFileStoreEverChanged = true;

                StudyStore.UpdateConfiguration(_configuration);

				// if the default deletion rule was modified, kick-off a re-apply rules work item
				if(HasDeletionRuleChanged)
				{
					var bridge = new ReapplyRulesBridge();
					bridge.ReapplyAll(new RulesEngineOptions { ApplyDeleteActions = true });
				}

                _fileStoreDirectory = _currentFileStoreDirectory = _configuration.FileStoreDirectory;
                _currentDeletionRule = _configuration.DefaultDeletionRule.Clone();

                //Just update all kinds of properties.
		        NotifyFileStoreChanged();
                NotifyDeletionRuleChanged(null);
                NotifyServiceControlLinkPropertiesChanged();

                ShowValidation(false);
		    }
		    catch (Exception e)
		    {
                ExceptionHandler.Report(e, Host.DesktopWindow);
		    }
        }

        public override bool HasValidationErrors
        {
            get
            {
                _delaySetFileStoreDirectory.PublishNow();
                if (HasDeletionRuleChanged && !IsLocalServiceRunning)
                    return true;

                return base.HasValidationErrors;
            }
        }

        public override bool Modified
        {
            get
            {
                return base.Modified;
            }
            protected set
            {
                base.Modified = value;
                NotifyServiceControlLinkPropertiesChanged();
            }
        }

        #endregion

        #region Presentation Model

        #region File Store

        public bool CanChangeFileStore
        {
            get
            {
                //Changing the deletion rule requires the service to be running,
                //whereas changing the file store requires it to be stopped.
                return !HasDeletionRuleChanged;
            }
        }

        public string FileStoreDirectory
        {
            get { return _fileStoreDirectory; }
            set
            {
                if (Equals(value, _fileStoreDirectory))
                    return;

                Modified = true;
                _fileStoreDirectory = value;
                _delaySetFileStoreDirectory.Publish(this, EventArgs.Empty);
            }
        }
        
        public bool HasFileStoreChanged
        {
            get { return _configuration.FileStoreDirectory != _currentFileStoreDirectory; }
        }

        public string FileStoreChangedMessage
        {
            get
            {
                if (!HasFileStoreChanged)
                    return String.Empty;

                return SR.MessageFileStoreChanged;
            }
        }

        public string FileStoreChangedDescription
        {
            get
            {
                if (!HasFileStoreChanged)
                    return String.Empty;

                return SR.DescriptionFileStoreChanged;
            }
        }

        public void ChangeFileStore()
        {
            var args = new SelectFolderDialogCreationArgs(FileStoreDirectory) { Prompt = SR.TitleSelectFileStore, AllowCreateNewFolder = true };
            var result = base.Host.DesktopWindow.ShowSelectFolderDialogBox(args);
            if (result.Action != DialogBoxAction.Ok)
                return;

            try
            {
                FileStoreDirectory = result.FileName;
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, Host.DesktopWindow);
            }
        }

        #endregion
        #region Disk Space

        public string TotalSpaceBytesDisplay
        {
            get
            {
                if (!IsDiskspaceAvailable)
                    return SR.NotApplicable;

                return Diskspace.FormatBytes(_configuration.FileStoreDiskSpace.TotalSpace, "F3");
            }
        }

        public double UsedSpacePercent
        {
            get
            {
                if (!IsDiskspaceAvailable)
                    return 0;

                return _configuration.FileStoreDiskSpace.UsedSpacePercent;
            }
        }

        public string UsedSpacePercentDisplay
        {
            get { return UsedSpacePercent.ToString("F3"); }
        }

        public string UsedSpaceBytesDisplay
        {
            get
            {
                if (!IsDiskspaceAvailable)
                    return SR.NotApplicable;

                return Diskspace.FormatBytes(_configuration.FileStoreDiskSpace.UsedSpace, "F3");
            }
        }

        public double MaximumUsedSpacePercent
        {
            get
            {
                if (!IsDiskspaceAvailable)
                    return 0;

                return _configuration.MaximumUsedSpacePercent;
            }
            set
            {
                if (!IsDiskspaceAvailable)
                    return;

                if (Equals(value, _configuration.MaximumUsedSpacePercent))
                    return;

                value = Math.Min(value, 100);
                value = Math.Max(value, 10);

                _configuration.MaximumUsedSpacePercent = value;
                this.Modified = true;
                MaximumUsedSpaceChanged();
            }
        }

        public string MaximumUsedSpaceDisplay
        {
            get
            {
                if (!IsDiskspaceAvailable)
                    return SR.NotApplicable;

                return Diskspace.FormatBytes(_configuration.MaximumUsedSpaceBytes, "F3");
            }
        }

        public bool IsMaximumUsedSpaceExceeded
        {
            get
            {
                if (!IsDiskspaceAvailable)
                    return false;

                return _configuration.IsMaximumUsedSpaceExceeded;
            }
        }

        public string MaximumUsedSpaceExceededMessage
        {
            get
            {
                if (!IsMaximumUsedSpaceExceeded)
                    return String.Empty;

                return StudyManagement.SR.MessageMaximumDiskUsageExceeded;
            }
        }

        public string MaximumUsedSpaceExceededDescription
        {
            get
            {
                if (!IsMaximumUsedSpaceExceeded)
                    return String.Empty;

                return StudyManagement.SR.DescriptionMaximumDiskUsageExceeded;
            }
        }

        #endregion

        #region Service Control

        public string LocalServiceControlLinkText
        {
            get
            {
                if (DoesLocalServiceHaveToStop)
                    return SR.LinkLabelStopLocalService;

                if (DoesLocalServiceHaveToStart)
                    return SR.LinkLabelStartLocalService;
                
                return String.Empty;
            }
        }

        public bool IsLocalServiceControlLinkVisible
        {
            get { return DoesLocalServiceHaveToStop || DoesLocalServiceHaveToStart; }
        }

        public void LocalServiceControlLinkClicked()
        {
            if (DoesLocalServiceHaveToStop)
            {
                try
                {
                    BlockingOperation.Run(LocalServiceProcess.Stop);
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Debug, e);
                    Host.DesktopWindow.ShowMessageBox(SR.MessageUnableToStopLocalService, MessageBoxActions.Ok);
                }
            }
            else if (DoesLocalServiceHaveToStart)
            {
                try
                {
                    BlockingOperation.Run(LocalServiceProcess.Start);
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Debug, e);
                    Host.DesktopWindow.ShowMessageBox(SR.MessageUnableToStartLocalService, MessageBoxActions.Ok);
                }
            }
        }

        #endregion

        #region Study Deletion

        public bool CanChangeDeletionRule
        {
            get
            {
                //Changing the deletion rule requires the service to be running,
                //whereas changing the file store requires it to be stopped.
                return !HasFileStoreChanged;
            }
        }

        public bool HasDeletionRuleChanged
        {
            get { return !Equals(_configuration.DefaultDeletionRule, _currentDeletionRule); }
        }

        //Yeah, it's a custom validation message, since there's no corresponding "property" (it's multiple, actually).
        public string DeletionRuleValidationMessage
        {
            get
            {
                if (base.ValidationVisible && HasDeletionRuleChanged && !IsLocalServiceRunning)
                    return SR.ValidationCannotChangeDeletionRule;

                return String.Empty;
            }
        }

        public bool DeleteStudies
        {
            get { return _configuration.DefaultDeletionRule.Enabled; }
            set
            {
                _delaySetFileStoreDirectory.PublishNow();
                if (!CanChangeDeletionRule)
                {
                    NotifyDeletionRuleChanged("DeleteStudies");
                    return;
                }

                if (value != _configuration.DefaultDeletionRule.Enabled)
                {
                    var ddr = _configuration.DefaultDeletionRule;
                    ddr.Enabled = value;
                    this.Modified = true;
                    NotifyDeletionRuleChanged("DeleteStudies");
                    // bug #10050: when first enabled, time value should be defaulted to 1 instead of 0
                    if (ddr.Enabled && ddr.TimeValue < 1)
                        this.DeleteTimeValue = 1;
                }
            }
        }

        public int DeleteTimeValue
        {
            get { return _configuration.DefaultDeletionRule.TimeValue; }
            set
            {
                _delaySetFileStoreDirectory.PublishNow();
                if (!CanChangeDeletionRule)
                {
                    NotifyDeletionRuleChanged("DeleteTimeValue");
                    return;
                }

                if (value != _configuration.DefaultDeletionRule.TimeValue)
                {
                    _configuration.DefaultDeletionRule.TimeValue = value;
                    this.Modified = true;
                    NotifyDeletionRuleChanged("DeleteTimeValue");
                }
            }
        }

        public IList DeleteTimeUnits
        {
            get { return Enum.GetValues(typeof(TimeUnit)); }
        }

        public string FormatTimeUnit(object obj)
        {
            var unit = (TimeUnit)obj;
            return unit.GetDescription();
        }

        public TimeUnit DeleteTimeUnit
        {
            get { return _configuration.DefaultDeletionRule.TimeUnit; }
            set
            {
                if (!CanChangeDeletionRule)
                {
                    NotifyDeletionRuleChanged("DeleteTimeUnit");
                    return;
                }

                if (value != _configuration.DefaultDeletionRule.TimeUnit)
                {
                    _configuration.DefaultDeletionRule.TimeUnit = value;
                    this.Modified = true;
                    NotifyDeletionRuleChanged("DeleteTimeUnit");
                }
            }
        }

        #endregion

        #region Help

        public string InfoMessage
        {
            get
            {
                if (HasFileStoreChanged || HasDeletionRuleChanged)
                    return SR.MessageCannotChangeFileStoreAndStudyDeletionSimultaneously;
                
                return String.Empty;
            }
        }

        public string HelpMessage
        {
            get
            {
                return SR.MessageStorageHelp;
            }
        }

        public void Help()
        {
            Host.DesktopWindow.ShowMessageBox(SR.DescriptionStorageOptions, MessageBoxActions.Ok);
        }

        #endregion
        #endregion

        #region Private Properties

        private bool IsLocalServiceRunning
        {
            get { return _activityMonitor.IsConnected; }
        }

        private bool DoesLocalServiceHaveToStop
        {
            get { return IsLocalServiceRunning && HasFileStoreChanged; }
        }

        private bool DoesLocalServiceHaveToStart
        {
            get
            {
                if (IsLocalServiceRunning)
                    return false;

                //If there is a change to the file store that needs to be saved, then we don't want the "Start Service" link to show.
                if (HasFileStoreChanged)
                    return false;

                //If the change to the file store has been saved, or the deletion rule needs to be saved, then we do want the "Start Service" link to show.
                return _hasFileStoreEverChanged || HasDeletionRuleChanged;
            }
        }

        private bool IsDiskspaceAvailable
        {
            get { return _configuration.FileStoreDriveExists && _configuration.FileStoreDiskSpace.IsAvailable; }
        }

        #endregion
        #region Private Methods

        [ValidationMethodFor("FileStoreDirectory")]
        private ValidationResult ValidateFileStorePath()
        {
            if (!HasFileStoreChanged)
                return new ValidationResult(true, String.Empty);

            if (!_configuration.IsFileStoreDriveValid)
                return new ValidationResult(false, SR.ValidationDriveInvalid);

            if (!_configuration.FileStoreDriveExists)
                return new ValidationResult(false, String.Format(SR.ValidationDriveDoesNotExist, _configuration.FileStoreRootPath));

            if (DoesLocalServiceHaveToStop)
                return new ValidationResult(false, SR.ValidationMessageCannotChangeFileStore);

            return new ValidationResult(true, String.Empty);
        }

        private void UpdateFileStoreDriveName()
        {
            string value;
            if (!_configuration.FileStoreDriveExists)
                value = null;
            else
                value = _configuration.FileStoreDriveName.ToUpper();

            if (Equals(value, _fileStoreDriveName))
                return;

            _fileStoreDriveName = value;

            UsedSpaceChanged();
            MaximumUsedSpaceChanged();
        }

        private void UsedSpaceChanged()
        {
            NotifyPropertyChanged("UsedSpacePercent");
            NotifyPropertyChanged("UsedSpacePercentDisplay");
            NotifyPropertyChanged("UsedSpaceBytesDisplay");
        }

        private void MaximumUsedSpaceChanged()
        {
            NotifyPropertyChanged("MaximumUsedSpace");
            NotifyPropertyChanged("MaximumUsedSpaceDisplay");
            NotifyPropertyChanged("IsMaximumUsedSpaceExceeded");
            NotifyPropertyChanged("MaximumUsedSpaceExceededMessage");
            NotifyPropertyChanged("MaximumUsedSpaceExceededDescription");
        }

        private void ActivityMonitorOnIsConnectedChanged(object sender, EventArgs eventArgs)
        {
            NotifyPropertyChanged("IsLocalServiceRunning");

            NotifyServiceControlLinkPropertiesChanged();

            if (!HasValidationErrors)
                ShowValidation(false);
        }

        private void RealSetFileStoreDirectory(object sender, EventArgs e)
        {
            if (Equals(_fileStoreDirectory, _configuration.FileStoreDirectory))
                return;

            _configuration.FileStoreDirectory = _fileStoreDirectory;

            UpdateFileStoreDriveName();

            NotifyFileStoreChanged();
        }

        private void NotifyFileStoreChanged()
        {
            NotifyPropertyChanged("FileStoreDirectory");
            NotifyPropertyChanged("HasFileStoreChanged");
            NotifyPropertyChanged("InfoMessage");
            NotifyPropertyChanged("FileStoreChangedMessage");
            NotifyPropertyChanged("FileStoreChangedDescription");

            NotifyPropertyChanged("CanChangeDeletionRule");
            NotifyServiceControlLinkPropertiesChanged();
        }

        private void NotifyDeletionRuleChanged(string propertyName)
        {
            if (!String.IsNullOrEmpty(propertyName))
                NotifyPropertyChanged(propertyName);

            NotifyPropertyChanged("HasDeletionRuleChanged");
            NotifyPropertyChanged("DeletionRuleValidationMessage");
            NotifyPropertyChanged("InfoMessage");
            NotifyPropertyChanged("CanChangeFileStore");
            NotifyServiceControlLinkPropertiesChanged();
        }

        private void NotifyServiceControlLinkPropertiesChanged()
        {
            NotifyPropertyChanged("IsLocalServiceControlLinkVisible");
            NotifyPropertyChanged("LocalServiceControlLinkText");
        }

        #endregion
    }
}

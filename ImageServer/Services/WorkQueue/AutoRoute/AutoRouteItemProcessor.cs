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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.Dicom;
using ClearCanvas.ImageServer.Services.WorkQueue.WebMoveStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.AutoRoute
{
	/// <summary>
	/// Processor for 'AutoRoute <see cref="WorkQueue"/> entries
	/// </summary>
	[StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.None)]
	public class AutoRouteItemProcessor : BaseItemProcessor, ICancelable
	{
		private const int TEMP_BLACKOUT_DURATION = 5; // seconds
		// cache the list of device that has exceeded the limit
		// to prevent repeat db hits when a server processes 
		// multiple study move entries to the same device
		private static readonly ServerCache<ServerEntityKey, Device> _tempBlackoutDevices =
			new ServerCache<ServerEntityKey, Device>(TimeSpan.FromSeconds(TEMP_BLACKOUT_DURATION),
			                                         TimeSpan.FromSeconds(TEMP_BLACKOUT_DURATION));

		#region Private Members

		private readonly object _syncLock = new object();
		private Dictionary<string, List<WorkQueueUid>> _uidMaps;
		private Device _device;
		private const short UNLIMITED = -1;

		#endregion

		#region Virtual Protected Methods


		/// <summary>
		/// Convert Uids into SopInstance
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerable<StorageInstance> GetStorageInstanceList()
		{
			string studyPath = StorageLocation.GetStudyPath();
			StudyXml studyXml = LoadStudyXml(StorageLocation);

			var list = new Dictionary<string, StorageInstance>();
			foreach (WorkQueueUid uid in WorkQueueUidList)
			{
				if (list.ContainsKey(uid.SopInstanceUid))
				{
					Platform.Log(LogLevel.Warn, "AutoRoute WorkQueueUid {0} is a duplicate.", uid.Key);
					continue; // duplicate;}
				}
				SeriesXml seriesXml = studyXml[uid.SeriesInstanceUid];
				InstanceXml instanceXml = seriesXml[uid.SopInstanceUid];

				string seriesPath = Path.Combine(studyPath, uid.SeriesInstanceUid);
				string instancePath = Path.Combine(seriesPath, uid.SopInstanceUid + ServerPlatform.DicomFileExtension);
				var instance = new StorageInstance(instancePath)
					{
						SopClass = instanceXml.SopClass,
						TransferSyntax = instanceXml.TransferSyntax,
						SopInstanceUid = instanceXml.SopInstanceUid,
						StudyInstanceUid = studyXml.StudyInstanceUid,
						PatientId = studyXml.PatientId,
						PatientsName = studyXml.PatientsName
					};

				list.Add(uid.SopInstanceUid, instance);
			}

			return list.Values;
		}

		/// <summary>
		/// Called when all instances have been sent
		/// </summary>
		protected virtual void OnComplete()
		{
			PostProcessing(WorkQueueItem,
			               WorkQueueProcessorStatus.Pending, // will go to Idle the next time around if there's no item left.
			               WorkQueueProcessorDatabaseUpdate.None);
		}

		protected virtual void OnInstanceSent(StorageInstance instance)
		{
			List<WorkQueueUid> foundUids = FindWorkQueueUids(instance);

			if (instance.SendStatus.Equals(DicomStatuses.SOPClassNotSupported))
			{
				WorkQueueItem.FailureDescription =
					String.Format("SOP Class not supported by remote device: {0}",
					              instance.SopClass.Name);
				Platform.Log(LogLevel.Warn,
				             "Unable to transfer SOP Instance, SOP Class is not supported by remote device: {0}",
				             instance.SopClass.Name);
			}

			if (instance.SendStatus.Status == DicomState.Failure)
			{
				WorkQueueItem.FailureDescription = instance.SendStatus.Description;
				if (foundUids != null)
				{
					foreach (WorkQueueUid uid in foundUids)
					{
						uid.FailureCount++;
						UpdateWorkQueueUid(uid);
					}
				}
			}
			else if (foundUids != null)
			{
				foreach (WorkQueueUid uid in foundUids)
				{
					DeleteWorkQueueUid(uid);
					WorkQueueUidList.Remove(uid);
				}
			}


		}

		#endregion

		#region Protected Properties

		protected IList<StorageInstance> InstanceList { get; set; }


		protected Device DestinationDevice
		{
			get
			{
				lock (_syncLock)
				{
					if (_device == null)
					{
						using (var context = new ServerExecutionContext())
							_device = Device.Load(context.ReadContext, WorkQueueItem.DeviceKey);
					}
				}

				return _device;
			}
		}

		#endregion

		#region Overridden Protected Method

		protected override bool Initialize(Model.WorkQueue item, out string failureDescription)
		{
			if (!base.Initialize(item, out failureDescription))
				return false;

			if (!LoadReadableStorageLocation(item))
			{
				failureDescription = "Unable to find readable storage location";
				Platform.Log(LogLevel.Warn, "Unable to find readable storage location for WorkQueue item: {0}", item.Key);
				return false;
			}

			LoadUids(item);
			InstanceList = new List<StorageInstance>(GetStorageInstanceList());
			return true;
		}

		public bool HasPendingItems
		{
			get { return InstanceList != null && InstanceList.Count > 0; }
		}

		/// <summary>
		/// Process a <see cref="WorkQueue"/> item of type AutoRoute.
		/// </summary>
		protected override void ProcessItem(Model.WorkQueue item)
		{
			if (WorkQueueItem.ScheduledTime >= WorkQueueItem.ExpirationTime && !HasPendingItems)
			{
				Platform.Log(LogLevel.Debug, "Removing Idle {0} entry : {1}", item.WorkQueueTypeEnum, item.GetKey().Key);
				base.PostProcessing(item, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.None);
				return;
			}

			if (!HasPendingItems)
			{
				// nothing to process, change to idle state
				PostProcessing(item, WorkQueueProcessorStatus.Idle, WorkQueueProcessorDatabaseUpdate.None);
				return;
			}

			Platform.Log(LogLevel.Info,
			             "Moving study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4} to {5}...",
			             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId, Study.AccessionNumber,
			             ServerPartition.Description, DestinationDevice.AeTitle);

			// Load remote device information from the database.
			Device device = DestinationDevice;
			if (device == null)
			{
				item.FailureDescription = String.Format("Unknown auto-route destination \"{0}\"", item.DeviceKey);
				Platform.Log(LogLevel.Error, item.FailureDescription);

				PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal); // Fatal Error
				return;
			}

			if (device.Dhcp && device.IpAddress.Length == 0)
			{
				item.FailureDescription = String.Format(
					"Auto-route destination is a DHCP device with no known IP address: \"{0}\"", device.AeTitle);
				Platform.Log(LogLevel.Error,
				             item.FailureDescription);

				PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal); // Fatal error
				return;
			}


			// Now setup the StorageSCU component
			int sendCounter = 0;
			using (var scu = new ImageServerStorageScu(ServerPartition, device))
			{
				using (var context = new ServerExecutionContext())
					// set the preferred syntax lists
					scu.LoadPreferredSyntaxes(context.ReadContext);

				// Load the Instances to Send into the SCU component
				scu.AddStorageInstanceList(InstanceList);

				// Set an event to be called when each image is transferred
				scu.ImageStoreCompleted += (sender, e) =>
					{
						var instance = e.StorageInstance;
						if (instance.SendStatus.Status == DicomState.Success
						    || instance.SendStatus.Status == DicomState.Warning
						    || instance.SendStatus.Equals(DicomStatuses.SOPClassNotSupported))
						{
							sendCounter++;
							OnInstanceSent(instance);
						}

						if (instance.SendStatus.Status == DicomState.Failure)
						{
							scu.FailureDescription = instance.SendStatus.Description;
							if (false == String.IsNullOrEmpty(instance.ExtendedFailureDescription))
							{
								scu.FailureDescription = String.Format("{0} [{1}]", scu.FailureDescription,
								                                       instance.ExtendedFailureDescription);
							}
						}


						if (CancelPending && !(this is WebMoveStudyItemProcessor) && !scu.Canceled)
						{
							Platform.Log(LogLevel.Info, "Auto-route canceled due to shutdown for study: {0}",
							             StorageLocation.StudyInstanceUid);
							item.FailureDescription = "Operation was canceled due to server shutdown request.";
							scu.Cancel();
						}
					};

				try
				{
					// Block until send is complete
					scu.Send();

					// Join for the thread to exit
					scu.Join();
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex, "Error occurs while sending images to {0} : {1}", device.AeTitle, ex.Message);
				}
				finally
				{
					if (scu.FailureDescription.Length > 0)
					{
						item.FailureDescription = scu.FailureDescription;
						scu.Status = ScuOperationStatus.Failed;
					}

					// Reset the WorkQueue entry status
					if ((InstanceList.Count > 0 && sendCounter != InstanceList.Count) // not all sop were sent
					    || scu.Status == ScuOperationStatus.Failed
					    || scu.Status == ScuOperationStatus.ConnectFailed)
					{
						PostProcessingFailure(item, WorkQueueProcessorFailureType.NonFatal); // failures occurred}
					}
					else
					{
						OnComplete();
					}
				}
			}
		}

		protected override bool CanStart()
		{
			if (InstanceList == null || InstanceList.Count == 0)
				return true;

			if (DeviceIsBusy(DestinationDevice))
			{
				DateTime newScheduledTime = Platform.Time.AddSeconds(WorkQueueProperties.ProcessDelaySeconds);
				PostponeItem(newScheduledTime, newScheduledTime.AddSeconds(WorkQueueProperties.ExpireDelaySeconds),
				             "Devices is busy. Max connection limit has been reached for the device");
				return false;
			}

			return true;
		}

		private bool DeviceIsBusy(Device device)
		{
			bool busy = false;
			if (device.ThrottleMaxConnections != UNLIMITED)
			{
				if (_tempBlackoutDevices.ContainsKey(device.Key))
				{
					busy = true;
				}
				else
				{
					List<Model.WorkQueue> currentMoves;

					using (IReadContext context = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
					{
						currentMoves = device.GetAllCurrentMoveEntries(context);
					}

					if (currentMoves != null && currentMoves.Count > device.ThrottleMaxConnections)
					{
						Platform.Log(LogLevel.Warn, "Connection limit on device {0} has been reached. Max = {1}.",
						             device.AeTitle, device.ThrottleMaxConnections);

						// blackout for 5 seconds
						_tempBlackoutDevices.Add(device.Key, device);
						busy = true;
					}
				}
			}

			return busy;
		}

		#endregion

		#region Private Methods

		private List<WorkQueueUid> FindWorkQueueUids(StorageInstance instance)
		{
			if (_uidMaps == null)
			{
				if (WorkQueueUidList != null)
				{
					_uidMaps = new Dictionary<string, List<WorkQueueUid>>();
					foreach (WorkQueueUid uid in WorkQueueUidList)
					{
						if (!String.IsNullOrEmpty(uid.SopInstanceUid))
						{
							if (!_uidMaps.ContainsKey(uid.SopInstanceUid))
								_uidMaps.Add(uid.SopInstanceUid, new List<WorkQueueUid>());

							_uidMaps[uid.SopInstanceUid].Add(uid);
						}
						else
						{
							_uidMaps = null;
							if (uid.SeriesInstanceUid.Equals(instance.SeriesInstanceUid))
							{
								return new List<WorkQueueUid>(1) {uid};
							}
						}
					}
				}
			}

			if (_uidMaps != null)
			{
				List<WorkQueueUid> foundUids;
				if (_uidMaps.TryGetValue(instance.SopInstanceUid, out foundUids))
				{
					return foundUids;
				}
			}

			return null;
		}

		#endregion
	}
}

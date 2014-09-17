#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Rules.StudyAutoRouteAction
{

	/// <summary>
	/// <see cref="ServerDatabaseCommand"/> derived class for use with <see cref="ServerCommandProcessor"/> for inserting AutoRoute WorkQueue entries into the Persistent Store.
	/// </summary>
	public class InsertStudyAutoRouteCommand : ServerDatabaseCommand
	{
		private readonly ServerActionContext _context;
		private readonly string _deviceAe;
		private readonly DateTime? _scheduledTime;
		private readonly QCStatusEnum _qcStatus;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="context">A contentxt in which to apply the AutoRoute request.</param>
		/// <param name="device">The AE Title of the device to AutoRoute to.</param>
		/// <param name="e">An option required QC StatusEnum value</param>
		public InsertStudyAutoRouteCommand(ServerActionContext context, string device, QCStatusEnum e)
			: base("Update/Insert an AutoRoute WorkQueue Entry")
		{
			Platform.CheckForNullReference(context, "ServerActionContext");

			_context = context;
			_deviceAe = device;
			_qcStatus = e;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="context">A contentxt in which to apply the AutoRoute request.</param>
		/// <param name="device">The AE Title of the device to AutoRoute to.</param>
		/// <param name="scheduledTime">The scheduled time for the AutoRoute.</param>
		/// <param name="e">An option required QC StatusEnum value</param>
		public InsertStudyAutoRouteCommand(ServerActionContext context, string device, DateTime scheduledTime, QCStatusEnum e)
			: base("Update/Insert an AutoRoute WorkQueue Entry")
		{
			Platform.CheckForNullReference(context, "ServerActionContext");

			_context = context;
			_deviceAe = device;
			_scheduledTime = scheduledTime;
			_qcStatus = e;
		}

		/// <summary>
		/// Do the insertion of the AutoRoute.
		/// </summary>
		protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
		{
			var deviceSelectCriteria = new DeviceSelectCriteria();
			deviceSelectCriteria.AeTitle.EqualTo(_deviceAe);
			deviceSelectCriteria.ServerPartitionKey.EqualTo(_context.ServerPartitionKey);

			var selectDevice = updateContext.GetBroker<IDeviceEntityBroker>();

			var dev = selectDevice.FindOne(deviceSelectCriteria);
			if (dev == null)
			{
				Platform.Log(LogLevel.Warn,
				             "Device '{0}' on partition {1} not in database for autoroute request!  Ignoring request.", _deviceAe,
				             _context.ServerPartition.AeTitle);

				ServerPlatform.Alert(
					AlertCategory.Application, AlertLevel.Warning,
					SR.AlertComponentAutorouteRule, AlertTypeCodes.UnableToProcess, null, TimeSpan.FromMinutes(5),
					SR.AlertAutoRouteUnknownDestination, _deviceAe, _context.ServerPartition.AeTitle);

				return;
			}

			if (!dev.AllowAutoRoute)
			{
				Platform.Log(LogLevel.Warn,
				             "Study Auto-route attempted to device {0} on partition {1} with autoroute support disabled.  Ignoring request.",
				             dev.AeTitle, _context.ServerPartition.AeTitle);

				ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Warning, SR.AlertComponentAutorouteRule,
				                     AlertTypeCodes.UnableToProcess, null, TimeSpan.FromMinutes(5),
				                     SR.AlertAutoRouteDestinationAEDisabled, dev.AeTitle, _context.ServerPartition.AeTitle);

				return;
			}

			if (_qcStatus != null)
			{
				var studyBroker = updateContext.GetBroker<IStudyEntityBroker>();
				var studySelect = new StudySelectCriteria();
				studySelect.StudyStorageKey.EqualTo(_context.StudyLocationKey);
				studySelect.ServerPartitionKey.EqualTo(_context.ServerPartitionKey);
				var study = studyBroker.FindOne(studySelect);
				if (!study.QCStatusEnum.Equals(_qcStatus))
				{
					Platform.Log(LogLevel.Debug,
					             "Ignoring Auto-route where the QCStatusEnum status must be {0}, but database has {1} for study {2}",
					             _qcStatus.Description, study.QCStatusEnum.Description, study.StudyInstanceUid);
					return;
				}
			}

			var parms = new InsertWorkQueueParameters
			{
				WorkQueueTypeEnum = WorkQueueTypeEnum.StudyAutoRoute,
				ScheduledTime = _scheduledTime.HasValue
									? _scheduledTime.Value
									: Platform.Time.AddSeconds(30),
				StudyStorageKey = _context.StudyLocationKey,
				ServerPartitionKey = _context.ServerPartitionKey,
				DeviceKey = dev.GetKey()
			};
			var broker = updateContext.GetBroker<IInsertWorkQueue>();

			if (broker.FindOne(parms) == null)
			{
				throw new ApplicationException("InsertWorkQueue for Study Auto-Route failed");
			}
		}
	}
}

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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core.Command;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Reconcile.Discard
{
	/// <summary>
	/// Command for discarding images that need to be reconciled.
	/// </summary>
	class DiscardImagesCommand : ReconcileCommandBase
	{
		#region Constructors

		public DiscardImagesCommand(ReconcileStudyProcessorContext context)
			: base("Discard image", false, context)
		{
		}

		#endregion

		#region Overidden Protected Methods

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			Platform.CheckForNullReference(Context, "Context");
			Platform.CheckForNullReference(Context.ReconcileWorkQueueData, "Context.ReconcileWorkQueueData");

			foreach (WorkQueueUid uid in Context.WorkQueueUidList)
			{

			    string imagePath = GetReconcileUidPath(uid);

				try
				{
					using (var processor = new ServerCommandProcessor(String.Format("Deleting {0}", uid.SopInstanceUid)))
					{
						var deleteFile = new FileDeleteCommand(imagePath, true);
						var deleteUid = new DeleteWorkQueueUidCommand(uid);
						processor.AddCommand(deleteFile);
						processor.AddCommand(deleteUid);
						Platform.Log(ServerPlatform.InstanceLogLevel, deleteFile.ToString());
						if (!processor.Execute())
						{
							throw new Exception(String.Format("Unable to discard image {0}", uid.SopInstanceUid));
						}
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "Unexpected exception discarding file: {0}", imagePath);
					SopInstanceProcessor.FailUid(uid, true);
				}
			}
		}

		protected override void OnUndo()
		{

		}

		#endregion
	}
}
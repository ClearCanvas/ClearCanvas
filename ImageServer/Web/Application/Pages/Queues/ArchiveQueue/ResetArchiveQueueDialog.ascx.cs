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
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data;
using SR = Resources.SR;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue
{
	public partial class ResetArchiveQueueDialog : System.Web.UI.UserControl
	{
		#region Private Members
		private IList<Model.ArchiveQueue> _archiveQueueItemList;
		#endregion Private Members
		
		#region Public Properties
		/// <summary>
		/// Sets / Gets the <see cref="ServerEntityKey"/> of the <see cref="WorkQueue"/> item associated with this dialog
		/// </summary>
		/// 
		public IList<Model.ArchiveQueue> ArchiveQueueItemList
		{
			get { return _archiveQueueItemList; }
			set { _archiveQueueItemList = value; }
		}

		#endregion Public Properties

		#region Protected Methods

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			PreResetConfirmDialog.Confirmed += PreResetConfirmDialog_Confirmed;
		}

		#endregion Protected Methods

		#region Private Methods
		void PreResetConfirmDialog_Confirmed(object data)
		{
			List<Model.ArchiveQueue> key = data as List<Model.ArchiveQueue>;

			if (key != null)
			{
					ArchiveQueueController controller = new ArchiveQueueController();
					DateTime scheduledTime = key[0].ScheduledTime;
					if (scheduledTime < Platform.Time)
						scheduledTime = Platform.Time.AddSeconds(60);

					bool successful = false;
					try
					{
						successful = controller.ResetArchiveQueueItem(key, scheduledTime);
						if (successful)
						{
							Platform.Log(LogLevel.Info, "Archive Queue item(s) reset by user ");
						}
						else
						{
							Platform.Log(LogLevel.Error,
										 "PreResetConfirmDialog_Confirmed: Unable to reset archive queue item.");

							MessageBox.Message = SR.ArchiveQueueResetFailed;
							MessageBox.MessageType =
								MessageBox.MessageTypeEnum.ERROR;
							MessageBox.Show();
						}
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e,
										 "PreResetConfirmDialog_Confirmed: Unable to reset work queue item." );

						MessageBox.Message = SR.ArchiveQueueResetFailed;
						MessageBox.MessageType =
							MessageBox.MessageTypeEnum.ERROR;
						MessageBox.Show();
					}
				
			}
		}

		#endregion Private Methods

		#region Public Methods

		public override void DataBind()
		{
			if (ArchiveQueueItemList != null)
			{
				ArchiveQueueAdaptor adaptor = new ArchiveQueueAdaptor();
			}

			base.DataBind();
		}

		/// <summary>
		/// Displays the dialog box for reseting the <see cref="WorkQueue"/> entry.
		/// </summary>
		/// <remarks>
		/// The <see cref="ArchiveQueueItemList"/> to be deleted must be set prior to calling <see cref="Show"/>.
		/// </remarks>

		public void Show()
		{
			DataBind();

			if (_archiveQueueItemList != null)
			{
				PreResetConfirmDialog.Data = ArchiveQueueItemList;
				PreResetConfirmDialog.MessageType =
					MessageBox.MessageTypeEnum.YESNO;
				if (_archiveQueueItemList.Count > 1)
					PreResetConfirmDialog.Message = SR.MultipleArchiveQueueResetConfirm;
				else
					PreResetConfirmDialog.Message = SR.ArchiveQueueResetConfirm;
				PreResetConfirmDialog.Show();
			}
		}

		/// <summary>
		/// Closes the dialog box
		/// </summary>
		public void Hide()
		{
			PreResetConfirmDialog.Close();
			MessageBox.Close();
		}

		#endregion Public Methods
	}
}
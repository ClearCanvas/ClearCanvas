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

using System.Web.UI;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
	public partial class StudyHistoryChangeDescPanel : System.Web.UI.UserControl
	{
		private StudyHistory _historyRecord;

		public StudyHistory HistoryRecord
		{
			get { return _historyRecord; }
			set { _historyRecord = value; }
		}

		public override void DataBind()
		{
			if (_historyRecord != null)
			{
				// Use different control to render the content of the ChangeDescription column.
				IStudyHistoryColumnControlFactory render = GetColumnControlFactory(_historyRecord);
				Control ctrl = render.GetChangeDescColumnControl(this, _historyRecord);
				SummaryPlaceHolder.Controls.Add(ctrl);
			}
			base.DataBind();
		}

		private static IStudyHistoryColumnControlFactory GetColumnControlFactory(StudyHistory record)
		{
			if (record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.StudyReconciled)
				return new ReconcileStudyRendererFactory();
			if (record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.WebEdited ||
			    record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.ExternalEdit)
				return new StudyEditRendererFactory();
			if (record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.Duplicate)
				return new ProcessDuplicateChangeLogRendererFactory();
			if (record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.Reprocessed)
				return new StudyReprocessedChangeLogRendererFactory();
			if (record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.SeriesDeleted)
				return new SeriesDeletionChangeLogRendererFactory();
			if (record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.StudyCompress
			 || record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.SopCompress)
				return new CompressionHistoryRendererFactory();
			return new DefaultStudyHistoryRendererFactory();
		}
	}
}
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
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code
{
    internal class ReconcileStudyRendererFactory : IStudyHistoryColumnControlFactory
    {
        public Control GetChangeDescColumnControl(Control parent, StudyHistory historyRecord)
        {
            ReconcileHistoryDetailsColumn control = parent.Page.LoadControl("~/Pages/Studies/StudyDetails/Controls/ReconcileHistoryDetailsColumn.ascx") as ReconcileHistoryDetailsColumn;
            control.HistoryRecord = historyRecord;
            return control;
        }
    }

    internal class ReconcileHistoryRecord : StudyHistoryRecordBase
    {
        #region Private Fields

        private StudyReconcileDescriptor _updateDescription;
        #endregion

        #region Public Properties

        public StudyReconcileDescriptor UpdateDescription
        {
            get { return _updateDescription; }
            set { _updateDescription = value; }
        }

        #endregion
    }

}
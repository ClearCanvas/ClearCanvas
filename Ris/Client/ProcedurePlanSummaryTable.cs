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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
    public class ProcedurePlanSummaryTableItem
    {
        private readonly ProcedureDetail _rpDetail;
        private readonly ProcedureStepDetail _mpsDetail;

        public ProcedurePlanSummaryTableItem(ProcedureDetail rpDetail, ProcedureStepDetail mpsDetail)
        {
            _rpDetail = rpDetail;
            _mpsDetail = mpsDetail;
        }

        #region Public Properties

        public ProcedureDetail Procedure
        {
            get { return _rpDetail; }
        }

        public ProcedureStepDetail ModalityProcedureStep
        {
            get { return _mpsDetail; }
        }

        #endregion
    }

    public class ProcedurePlanSummaryTable : Table<Checkable<ProcedurePlanSummaryTableItem>>
    {
        private static readonly int NumRows = 2;
        private static readonly int ProcedureDescriptionRow = 1;

        private event EventHandler _checkedRowsChanged;

        public ProcedurePlanSummaryTable()
            : this(NumRows)
        {
        }

        private ProcedurePlanSummaryTable(int cellRowCount)
            : base(cellRowCount)
        {
            this.Columns.Add(new TableColumn<Checkable<ProcedurePlanSummaryTableItem>, bool>(
                                 "X",
                                 delegate(Checkable<ProcedurePlanSummaryTableItem> checkable) { return checkable.IsChecked; },
                                 delegate(Checkable<ProcedurePlanSummaryTableItem> checkable, bool isChecked)
                                     {
                                         checkable.IsChecked = isChecked;
                                         EventsHelper.Fire(_checkedRowsChanged, this, EventArgs.Empty);
                                     },
                                 0.1f));

            this.Columns.Add(new TableColumn<Checkable<ProcedurePlanSummaryTableItem>, string>(
                                 SR.ColumnStatus,
                                 delegate(Checkable<ProcedurePlanSummaryTableItem> checkable)
                                 {
                                 	return FormatStatus(checkable.Item);
                                 },
                                 0.5f));

            this.Columns.Add(new TableColumn<Checkable<ProcedurePlanSummaryTableItem>, string>(
                                 SR.ColumnModality,
                                 delegate(Checkable<ProcedurePlanSummaryTableItem> checkable) { return checkable.Item.ModalityProcedureStep.Modality.Name; },
                                 0.5f));

			DateTimeTableColumn<Checkable<ProcedurePlanSummaryTableItem>> scheduledStartTimeColumn = 
                new DateTimeTableColumn<Checkable<ProcedurePlanSummaryTableItem>>(
				     SR.ColumnScheduledTime,
				     delegate(Checkable<ProcedurePlanSummaryTableItem> checkable) { return checkable.Item.ModalityProcedureStep.ScheduledStartTime; },
				     0.5f);

			this.Columns.Add(scheduledStartTimeColumn);

			this.Columns.Add(new DateTimeTableColumn<Checkable<ProcedurePlanSummaryTableItem>>(
								 SR.ColumnCheckInTime,
								 delegate(Checkable<ProcedurePlanSummaryTableItem> checkable) { return checkable.Item.Procedure.CheckInTime; },
								 0.5f));

			this.Columns.Add(new DateTimeTableColumn<Checkable<ProcedurePlanSummaryTableItem>>(
								 SR.ColumnStartTime,
								 delegate(Checkable<ProcedurePlanSummaryTableItem> checkable) { return checkable.Item.ModalityProcedureStep.StartTime; },
								 0.5f));


            this.Columns.Add(new TableColumn<Checkable<ProcedurePlanSummaryTableItem>, string>(SR.ColumnProcedureDescription,
                                delegate(Checkable<ProcedurePlanSummaryTableItem> checkable)
                                {
									// if MPS description is identical to procedure type name, don't put redundant text
									if(checkable.Item.ModalityProcedureStep.Description == checkable.Item.Procedure.Type.Name)
									{
										return ProcedureFormat.Format(checkable.Item.Procedure);
									}
									else
									{
										// MPS desc is different, so append it
										return string.Format("{0} - {1}",
											ProcedureFormat.Format(checkable.Item.Procedure),
											checkable.Item.ModalityProcedureStep.Description);
									}
                                },
                                0.5f,
                                ProcedureDescriptionRow));

            this.Sort(new TableSortParams(scheduledStartTimeColumn, true));
        }

    	private static string FormatStatus(ProcedurePlanSummaryTableItem item)
    	{
			if (item.ModalityProcedureStep.State.Code == "SC")
			{
				return item.Procedure.CheckInTime.HasValue
				       	? string.Format(SR.FormatStatusCheckedIn, item.ModalityProcedureStep.State.Value)
				       	: item.ModalityProcedureStep.State.Value;
			}

			if(item.ModalityProcedureStep.State.Code == "CM")
			{
				// bug #3336 : apparently having this say Completed was confusing people into
				// thinking the procedure was completed, when in fact it is just the MPS
				// however, hardcoding this is dumb!!! this should probably be re-visited in future (JR)
				return "Performed";
			}

    		return item.ModalityProcedureStep.State.Value;
    	}

    	public event EventHandler CheckedRowsChanged
        {
            add { _checkedRowsChanged += value; }
            remove { _checkedRowsChanged -= value; }
        }
    }
}
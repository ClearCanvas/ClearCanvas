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
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Workflow;
using Iesi.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Report entity
    /// </summary>
	public partial class Report : ClearCanvas.Enterprise.Core.Entity
	{
        /// <summary>
        /// Constructor for creating a new radiology report for the specified procedure.
        /// </summary>
        /// <param name="procedure">The procedure being reported.</param>
        public Report(Procedure procedure)
        {
            _procedures = new HashedSet<Procedure>();
            _parts = new List<ReportPart>();

            _procedures.Add(procedure);
            procedure.Reports.Add(this);

            // create the main report part
            ReportPart mainReport = new ReportPart(this, 0);
            _parts.Add(mainReport);
        }
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
		
        /// <summary>
        /// Gets a value indicating whether this report has any addenda.
        /// </summary>
        public virtual bool HasAddenda
        {
            get { return _parts.Count > 1; }
        }

        /// <summary>
        /// Gets the active (modifiable) report part, or null if no report part is active.
        /// </summary>
        public virtual ReportPart ActivePart
        {
            get
            {
                ReportPart lastPart = CollectionUtils.LastElement(_parts);
                return lastPart.IsModifiable ? lastPart : null;
            }
        }

		/// <summary>
		/// Gets the time this report was created.
		/// </summary>
    	public virtual DateTime CreationTime
    	{
			get { return _parts[0].CreationTime; }
		}

		/// <summary>
		/// Gets the time this report was marked 'preliminary', or null if has not been marked preliminary.
		/// </summary>
    	public virtual DateTime? PreliminaryTime
    	{
    		get { return _parts.Count == 0 ? null : _parts[0].PreliminaryTime; }
    	}

		/// <summary>
		/// Gets the time this report was completed (marked final), or null if it is not completed.
		/// </summary>
    	public virtual DateTime? CompletedTime
    	{
			get { return _parts.Count == 0 ? null : _parts[0].CompletedTime; }
    	}

		/// <summary>
		/// Gets the time this report was cancelled, or null if it is not cancelled.
		/// </summary>
    	public virtual DateTime? CancelledTime
    	{
			get { return _parts.Count == 0 ? null : _parts[0].CancelledTime; }
    	}

		/// <summary>
		/// Gets the time that the first addendum was completed, or null if no completed addenda exist.
		/// </summary>
    	public virtual DateTime? CorrectedTime
    	{
			get { return _parts.Count < 2 ? null : _parts[1].CompletedTime; }
    	}

        /// <summary>
        /// Adds a new part to this report.
        /// </summary>
        /// <returns></returns>
        public virtual ReportPart AddAddendum()
        {
            if(_status == ReportStatus.X)
				throw new WorkflowException("Cannot add an addendum because the report was cancelled.");
			if (this.ActivePart != null)
                throw new WorkflowException("Cannot add an addendum because the report, or a previous addendum, has not been completed.");

            ReportPart part = new ReportPart(this, _parts.Count);
            _parts.Add(part);

            UpdateStatus();
            
            return part;
        }

        /// <summary>
        /// Links a <see cref="Procedure"/> to this report, meaning that the report covers
        /// this radiology procedure.
        /// </summary>
        /// <param name="procedure"></param>
        protected internal virtual void LinkProcedure(Procedure procedure)
        {
            if (_procedures.Contains(procedure))
                throw new WorkflowException("The procedure is already associated with this report.");

            // does the procedure already have a report?
            Report otherReport = procedure.ActiveReport;
            if (otherReport != null && !this.Equals(otherReport))
                throw new WorkflowException("Cannot link this procedure because it already has an active report.");

            _procedures.Add(procedure);
            procedure.Reports.Add(this);
        }

        /// <summary>
        /// Called by this report or by a child report part to tell this report to update its status.
        /// </summary>
        protected internal virtual void UpdateStatus()
        {
            // if the main report part is cancelled, the report is cancelled
            // (cancelling an addendum alone does not cancel the report)
            if (_parts.Count > 0 && _parts[0].Status == ReportPartStatus.X)
            {
                _status = ReportStatus.X;
            }
            else
            if (_status == ReportStatus.D)
            {
				// check for final parts
				if (CollectionUtils.Contains(_parts,
						delegate(ReportPart part) { return part.Status == ReportPartStatus.F; }))
				{
					_status = ReportStatus.F;
				}
				// check for prelim parts
				else if(CollectionUtils.Contains(_parts,
						delegate(ReportPart part) { return part.Status == ReportPartStatus.P; }))
				{
					_status = ReportStatus.P;
				}
            }
            else
            if (_status == ReportStatus.P || _status == ReportStatus.F)
            {
                int numCompletedParts =
                    CollectionUtils.Select(_parts,
                        delegate(ReportPart part) { return part.Status == ReportPartStatus.F; }).Count;

                // transition from F to C if addendum completed
                if (_status == ReportStatus.F && numCompletedParts > 1)
                {
                    _status = ReportStatus.C;
                }
                // transition from P to F if initial report part completed
                else if (_status == ReportStatus.P && numCompletedParts > 0)
                {
                    _status = ReportStatus.F;
                }
            }
        }

		/// <summary>
		/// Shifts the object in time by the specified number of minutes, which may be negative or positive.
		/// </summary>
		/// <remarks>
		/// The method is not intended for production use, but is provided for the purpose
		/// of generating back-dated data for demos and load-testing.
		/// </remarks>
		/// <param name="minutes"></param>
		protected internal virtual void TimeShift(int minutes)
		{
			foreach (ReportPart part in _parts)
			{
				part.TimeShift(minutes);
			}
		}
	}
}

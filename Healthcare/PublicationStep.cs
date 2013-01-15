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
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{


	/// <summary>
	/// PublicationStep entity
	/// </summary>
	public class PublicationStep : ReportingProcedureStep
	{
		private int _failureCount;
		private DateTime? _lastFailureTime;

		public PublicationStep()
		{
		}

		public PublicationStep(ReportingProcedureStep previousStep)
			: base(previousStep)
		{
			_failureCount = 0;
			_lastFailureTime = null;
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		public override string Name
		{
			get { return "Publication"; }
		}

		public virtual int FailureCount
		{
			get { return _failureCount; }
		}

		public virtual DateTime? LastFailureTime
		{
			get { return _lastFailureTime; }
		}

		public virtual void Fail()
		{
			_failureCount++;
			_lastFailureTime = Platform.Time;
		}

		protected override void OnStateChanged(ActivityStatus previousState, ActivityStatus newState)
		{
			// complete the report part when publication is complete
			if (newState == ActivityStatus.CM)
			{
				this.ReportPart.Complete();

				// if step corresponds to the initial report (not an addendum), mark procedure(s) as
				// being complete
				if (this.ReportPart.Index == 0)
				{
					foreach (Procedure procedure in this.AllProcedures)
					{
						procedure.Complete((DateTime) this.EndTime);
					}
				}
			}

			base.OnStateChanged(previousState, newState);
		}

		protected override ProcedureStep CreateScheduledCopy()
		{
			return new PublicationStep(this);
		}
	}
}
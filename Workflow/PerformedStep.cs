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
using System.Text;
using Iesi.Collections;
using ClearCanvas.Enterprise;
using ClearCanvas.Common;
using Iesi.Collections.Generic;

namespace ClearCanvas.Workflow
{
    /// <summary>
    /// Base class for a workflow performed step.  A performed step records part or all of the performance of
    /// one or more workflow activities (i.e. the relationship between activities and performed steps is many-to-many).
    /// The use of performed steps is entirely optional.  It is perfectly possible to use the Activity model
    /// without recording performed steps.
    /// Note: this class has been coded for compatability with NHibernate mapping.
    /// </summary>
    public abstract class PerformedStep : PersistentFsm<PerformedStepStatus>
    {
        private Iesi.Collections.Generic.ISet<Activity> _activities;
        private ActivityPerformer _performer;
        private DateTime _startTime;
        private DateTime? _endTime;

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public PerformedStep()
            :this(null, null, new PerformedStepStatusTransitionLogic())
        {
        }

        /// <summary>
        /// Constructor that allows the performer to be set
        /// </summary>
        /// <param name="performer"></param>
        public PerformedStep(ActivityPerformer performer)
            :this(performer, null, new PerformedStepStatusTransitionLogic())
        {
        }

		/// <summary>
		/// Constructor that allows the performer to be set, and the start-time to be specified.
		/// </summary>
		/// <param name="performer"></param>
		/// <param name="startTime"></param>
		public PerformedStep(ActivityPerformer performer, DateTime? startTime)
			: this(performer, startTime, new PerformedStepStatusTransitionLogic())
		{
		}

        /// <summary>
        /// Protected constructor
        /// </summary>
        /// <param name="performer"></param>
        /// <param name="startTime"></param>
        /// <param name="transitionLogic"></param>
        protected PerformedStep(ActivityPerformer performer, DateTime? startTime, IFsmTransitionLogic<PerformedStepStatus> transitionLogic)
            : base(PerformedStepStatus.IP, transitionLogic)
        {
            _activities = new HashedSet<Activity>();
			_startTime = startTime ?? Platform.Time;
            _performer = performer;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the set of associated activities.  Do not add or remove elements directly from this collection.
        /// Instead use the <see cref="Activity.AddPerformedStep"/> and <see cref="Activity.RemovePerformedStep"/> methods.
        /// </summary>
        public virtual Iesi.Collections.Generic.ISet<Activity> Activities
        {
            get { return _activities; }
        }

        /// <summary>
        /// Gets or sets the performer of this step
        /// </summary>
        public virtual ActivityPerformer Performer
        {
            get { return _performer; }
            set { _performer = value; }
        }

        /// <summary>
        /// Gets the start time of this step.
        /// This property allows protected set for NHibernate mapping compatability.
        /// </summary>
        public virtual DateTime StartTime
        {
            get { return _startTime; }
            protected set { _startTime = value; }
        }

        /// <summary>
        /// Gets the end time of this step.
        /// This property allows protected set for NHibernate mapping compatability.
        /// </summary>
        public virtual DateTime? EndTime
        {
            get { return _endTime; }
            protected set { _endTime = value; }
        }

        #endregion

        /// <summary>
        /// Discontinues this step
        /// </summary>
        public virtual void Discontinue()
        {
			Discontinue((DateTime?)null);
        }

		/// <summary>
		/// Discontinues this step
		/// </summary>
		public virtual void Discontinue(DateTime? endTime)
		{
			ChangeState(PerformedStepStatus.DC);
			_endTime = endTime ?? Platform.Time;
		}

		        /// <summary>
        /// Completes this step
        /// </summary>
		public virtual void Complete()
        {
			Complete((DateTime?)null);
        }

    	/// <summary>
        /// Completes this step
        /// </summary>
        public virtual void Complete(DateTime? endTime)
        {
            ChangeState(PerformedStepStatus.CM);
			_endTime = endTime ?? Platform.Time;
		}
    }
}

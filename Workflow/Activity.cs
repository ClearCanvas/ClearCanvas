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
    /// Base class for a workflow activity.  An activity is the basic unit of workflow.
    /// Note: this class has been coded for compatability with NHibernate mapping.
    /// </summary>
    public abstract class Activity : PersistentFsm<ActivityStatus>
    {
        private Iesi.Collections.Generic.ISet<PerformedStep> _performedSteps;
        private ActivityScheduling _scheduling;
        private ActivityPerformer _performer;
        private DateTime? _startTime;
        private DateTime? _endTime;

        /// <summary>
        /// Constructor
        /// </summary>
        public Activity()
            :this(new ActivityStatusTransitionLogic())
        {
        }

        /// <summary>
        /// Protected constructor that allows for customized FSM logic.  Typically there should be no need to customize the FSM logic.
        /// </summary>
        /// <param name="transitionLogic"></param>
        protected Activity(IFsmTransitionLogic<ActivityStatus> transitionLogic)
            : base(ActivityStatus.SC, transitionLogic)
        {
            _performedSteps = new HashedSet<PerformedStep>();
            _scheduling = new ActivityScheduling();
        }

        /// <summary>
        /// Gets the set of associated performed steps.  Do not add or remove elements directly from this collection.
        /// Instead use the <see cref="AddPerformedStep"/> and <see cref="RemovePerformedStep"/> methods.
        /// </summary>
        public virtual Iesi.Collections.Generic.ISet<PerformedStep> PerformedSteps
        {
            get { return _performedSteps; }
        }

        /// <summary>
        /// Gets the scheduling for this activity.  May return null, in which case the activity has
        /// no scheduling information.
        /// </summary>
        public virtual ActivityScheduling Scheduling
        {
            get { return _scheduling; }
            protected set { _scheduling = value; }
        }


        /// <summary>
        /// Gets the performer of this step.
        /// This property allows private set for NHibernate mapping compatability.
        /// </summary>
        public virtual ActivityPerformer Performer
        {
            get { return _performer; }
            protected set { _performer = value; }
        }

        /// <summary>
        /// Gets the time that this activity was started, which corresponds to the first transition
        /// to the <see cref="ActivityStatus.IP"/> state.
        /// This property allows private set for NHibernate mapping compatability.
        /// </summary>
        public virtual DateTime? StartTime
        {
            get { return _startTime; }
            protected set { _startTime = value; }
        }

        /// <summary>
        /// Gets the time at which this activity was terminated, which corresponds to the first transition
        /// to either the <see cref="ActivityStatus.CM"/> or <see cref="ActivityStatus.DC"/> states.
        /// </summary>
        public virtual DateTime? EndTime
        {
            get { return _endTime; }
            protected set { _endTime = value; }
        }

        /// <summary>
        /// Schedules or re-schedules the activity at the specified start time. The end time will be
        /// set to null.
        /// </summary>
        /// <param name="startTime"></param>
        public virtual void Schedule(DateTime? startTime)
        {
            Schedule(startTime, null);
        }

        /// <summary>
        /// Schedules or re-schedules the activity at the specified start time. The end time is optional.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public virtual void Schedule(DateTime? startTime, DateTime? endTime)
        {
            if (this.State != ActivityStatus.SC)
                throw new WorkflowException("Schedule is only allowed from Scheduled state");

            if (_scheduling == null)
                _scheduling = new ActivityScheduling();

            this.Scheduling.StartTime = startTime;
            this.Scheduling.EndTime = endTime;

            OnSchedulingChanged();
        }

        /// <summary>
        /// Assigns this activity to be performed by the specified performer.  The value may be null,
        /// in which case the activity is un-assigned.
        /// </summary>
        /// <param name="performer"></param>
        public virtual void Assign(ActivityPerformer performer)
        {
            if (this.State != ActivityStatus.SC)
                throw new WorkflowException("Assignment of scheduled performer only allowed from scheduled state");

            if (_scheduling == null)
                _scheduling = new ActivityScheduling();

            this.Scheduling.Performer = performer;

            OnSchedulingChanged();
        }

        /// <summary>
        /// Starts the activity, setting the state to <see cref="ActivityStatus.IP"/> and recording the specified performer.
        /// </summary>
        /// <param name="performer"></param>
        public virtual void Start(ActivityPerformer performer)
        {
			this.Start(performer, (DateTime?)null);
        }

		/// <summary>
		/// Starts the activity, setting the state to <see cref="ActivityStatus.IP"/> and recording the specified performer
		/// and start-time.  If start-time is null, the current time is used.
		/// </summary>
		/// <param name="performer"></param>
		/// <param name="startTime"></param>
		public virtual void Start(ActivityPerformer performer, DateTime? startTime)
		{
			Platform.CheckForNullReference(performer, "performer");

			_performer = performer;
			_startTime = startTime ?? Platform.Time;
			ChangeState(ActivityStatus.IP);
		}

        /// <summary>
        /// Suspends the activity, setting the state to <see cref="ActivityStatus.SU"/>
        /// </summary>
        public virtual void Suspend()
        {
            ChangeState(ActivityStatus.SU);
        }

        /// <summary>
        /// Suspends the activity, setting the state to <see cref="ActivityStatus.IP"/>.
        /// Note that the activity must be in the <see cref="ActivityStatus.SU"/> state or an exception will be thrown.
        /// </summary>
        public virtual void Resume()
        {
            if (this.State != ActivityStatus.SU)
                throw new WorkflowException("Resume is only allowed from Suspended state");

            ChangeState(ActivityStatus.IP);
        }

        /// <summary>
        /// Discontinues the activity, setting the state to <see cref="ActivityStatus.DC"/>.
        /// </summary>
        public virtual void Discontinue()
        {
			this.Discontinue((DateTime?)null);
        }

		/// <summary>
		/// Discontinues the activity, setting the state to <see cref="ActivityStatus.DC"/>,
		/// and recording the specified end-time.  If end-time is null the current time is used.
		/// </summary>
		public virtual void Discontinue(DateTime? endTime)
		{
			_endTime = endTime ?? Platform.Time;

			// automatically discontinue any performed-steps that have not terminated
			foreach (PerformedStep step in _performedSteps)
			{
				if(!step.IsTerminated)
					step.Discontinue(_endTime);
			}

			// set state DC
			ChangeState(ActivityStatus.DC);
		}

        /// <summary>
        /// Completes the activity, setting the state to <see cref="ActivityStatus.CM"/>.
        /// </summary>
        /// <remarks>
		/// An exception will be thrown if the performer was not previously specified in the <see cref="Start"/> method.
        /// </remarks>
        public virtual void Complete()
        {
			Complete((DateTime?)null);
        }

		/// <summary>
		/// Completes the activity, setting the state to <see cref="ActivityStatus.CM"/>, and recording the specified
		/// end-time.  If end-time is null, the current time is used.
		/// </summary>
		/// <remarks>
		/// An exception will be thrown if the performer was not previously specified in the <see cref="Start"/> method.
		/// </remarks>
		public virtual void Complete(DateTime? endTime)
		{
			if (_performer == null)
				throw new WorkflowException("Performer must be assigned");

			_endTime = endTime ?? Platform.Time;

			// if start-time was never set, make it equal to the end time
			if (_startTime == null)
				_startTime = _endTime;

			ChangeState(ActivityStatus.CM);
		}

        /// <summary>
        /// Completes the activity, setting the state to <see cref="ActivityStatus.CM"/>.
        /// </summary>
        /// <remarks>
		/// This overload allows the performer to be specified, which is necessary if the activity is being completed directly
		/// from the scheduled state, and hence the performer was not previously established.  Note that if a performer
		/// has been previously established, and the specified performer is different, an exception will be thrown.
        /// </remarks>
        public virtual void Complete(ActivityPerformer performer)
        {
			Complete(performer, Platform.Time);
        }

		/// <summary>
		/// Completes the activity, setting the state to <see cref="ActivityStatus.CM"/>, and recording the specified end-time.
		/// If end-time is null, the current time is used.
		/// </summary>
		/// <remarks>
		/// This overload allows the performer to be specified, which is necessary if the activity is being completed directly
		/// from the scheduled state, and hence the performer was not previously established.  Note that if a performer
		/// has been previously established, and the specified performer is different, an exception will be thrown.
		/// </remarks>
		public virtual void Complete(ActivityPerformer performer, DateTime? endTime)
		{
			Platform.CheckForNullReference(performer, "performer");
			if (_performer == null)
			{
				_performer = performer;
			}
			else if (!_performer.Equals(performer))
			{
				throw new WorkflowException("Peformer already assigned");
			}

			this.Complete(endTime);
		}

        /// <summary>
        /// Adds the specified performed step to the set of <see cref="PerformedStep"/> objects associated with this activity,
        /// and adds this activity to the set of activities associated with the specified performed step.
        /// </summary>
        /// <param name="step"></param>
        public virtual void AddPerformedStep(PerformedStep step)
        {
            // because we are dealing with ISet, no need to worry about duplicate entries, since the set doesn't allow it
            _performedSteps.Add(step);
            step.Activities.Add(this);
        }

        /// <summary>
        /// Removes the specified performed step from the set of <see cref="PerformedStep"/> objects associated with this activity,
        /// and removes this activity from the set of activities associated with the specified performed step.
        /// </summary>
        /// <param name="step"></param>
        public virtual void RemovePerformedStep(PerformedStep step)
        {
            if (_performedSteps.Contains(step))
            {
                _performedSteps.Remove(step);
            }

            if (step.Activities.Contains(this))
            {
                step.Activities.Remove(this);
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
		public override void TimeShift(int minutes)
		{
			_startTime = _startTime.HasValue ? _startTime.Value.AddMinutes(minutes) : _startTime;
			_endTime = _endTime.HasValue ? _endTime.Value.AddMinutes(minutes) : _endTime;

			if(_scheduling != null)
			{
				_scheduling.TimeShift(minutes);
			}

			base.TimeShift(minutes);
		}

        protected virtual void OnSchedulingChanged()
        {
		}
    }
}
